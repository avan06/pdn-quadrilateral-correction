using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using PaintDotNet;
using PaintDotNet.Controls;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using PaintDotNet.DirectWrite;
using PaintDotNet.Direct2D1;

namespace QuadrilateralCorrectionEffect
{
    [DefaultEvent(nameof(ValueChanged))]
    internal class QuadControl : Direct2DPictureBox
    {
        private static readonly Cursor handOpen = new Cursor(typeof(QuadControl), "Resources.HandOpen.cur");
        private static readonly Cursor handGrab = new Cursor(typeof(QuadControl), "Resources.HandGrab.cur");

        public QuadControl()
        {
            this.SizeMode = Direct2DPictureBoxSizeMode.StretchBitmap;
            this.EnableAlphaCheckerboard = true;
            this.BitmapInterpolationMode = InterpolationMode.Linear;
            this.TabStop = false;

            nubTL = new Nub(0, 0);
            nubTR = new Nub(this.ClientSize.Width - 1, 0);
            nubBR = new Nub(this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            nubBL = new Nub(0, this.ClientSize.Height - 1);
        }

        #region Image Source
        internal void SetImageFromSourceRegion(
            RegionPtr<ColorBgra32> sourceRegion,
            int width,
            int height)
        {
            IBitmap<ColorBgra32> newBitmap = null;

            try
            {
                newBitmap = (IBitmap<ColorBgra32>)this.ImagingFactory.CreateBitmap(
                    width,
                    height,
                    PixelFormats.Bgra32);

                using (IBitmapLock<ColorBgra32> bitmapLock = newBitmap.Lock(new RectInt32(0, 0, width, height), BitmapLockOptions.Write))
                {
                    RegionPtr<ColorBgra32> destinationRegion = bitmapLock.AsRegionPtr();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            destinationRegion[x, y] = sourceRegion[x, y];
                        }
                    }
                }

                SetPreviewBitmap(newBitmap);
                newBitmap = null; // ownership transferred to QuadControl
            }
            finally
            {
                newBitmap?.Dispose();
            }
        }
        #endregion

        #region Fields
        private IBitmap<ColorBgra32> previewBitmap;
        private IBitmapSource previewBitmapSource;
        private SizeInt32 previewBitmapSize;

        private bool MouseIsDown = false; // True if mouse button is down
        private Size MouseFromNub = Size.Empty;
        private const int RadiusSmall = 3; // nb Radius * 2 + 1 = size
        private const int RadiusLarge = 5;
        private const int RadiusHover = 16;
        private const int DeadZone = 30;
        private Nub nubTL, nubTR, nubBR, nubBL; // four Nubs to store coordinates and activation states

        // Magnifier variables.
        private IDeviceBitmap magnifierDeviceBitmap;
        private ITextFormat magnifierTextFormat;
        private bool showMagnifier = false;
        private Point magnifierMouseLocation = Point.Empty;
        private const int MagnifierSize = 160;
        private const int MagnifierMinZoom = 2;
        private const int MagnifierMaxZoom = 12;
        private int MagnifierZoom = 4;
        #endregion

        #region Properties
        // four publicly accessible get/sets which map the internal location variables
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point NubTL
        {
            get => nubTL.Location;
            set
            {
                nubTL.Location = value;
                OnValueChanged();
                this.Invalidate();
            }
        }

        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point NubTR
        {
            get => nubTR.Location;
            set
            {
                nubTR.Location = value;
                OnValueChanged();
                this.Invalidate();
            }
        }

        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point NubBR
        {
            get => nubBR.Location;
            set
            {
                nubBR.Location = value;
                OnValueChanged();
                this.Invalidate();
            }
        }

        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point NubBL
        {
            get => nubBL.Location;
            set
            {
                nubBL.Location = value;
                OnValueChanged();
                this.Invalidate();
            }
        }

        [Category("Behavior")]
        [Description("When enabled, clicking anywhere on the control moves the nearest nub to the mouse position and begins dragging it.")]
        [DefaultValue(true)]
        public bool MoveNearestNubOnClick { get; set; } = true;

        [Category("Behavior")]
        [Description("When enabled, dragging a nub snaps it to nearby high-contrast lines in the image.")]
        [DefaultValue(true)]
        public bool LineSnapEnabled { get; set; } = true;

        [Category("Behavior")]
        [Description("Search radius, in control pixels, used when snapping a nub to a nearby line.")]
        [DefaultValue(8)]
        public int LineSnapSearchRadius { get; set; } = 8;

        [Category("Behavior")]
        [Description("Minimum Sobel edge strength required for line snapping.")]
        [DefaultValue(120)]
        public int LineSnapMinEdgeStrength { get; set; } = 120;

        [Category("Behavior")]
        [Description("When enabled, nubs may be dragged outside the image bounds.")]
        [DefaultValue(false)]
        public bool AllowNubsOutsideImage { get; set; } = false;

        internal QuadrilateralCorrectionEffect.Nub SelectedNub
        {
            get
            {
                if (nubTL.Selected)
                    return QuadrilateralCorrectionEffect.Nub.TopLeft;

                if (nubTR.Selected)
                    return QuadrilateralCorrectionEffect.Nub.TopRight;

                if (nubBR.Selected)
                    return QuadrilateralCorrectionEffect.Nub.BottomRight;

                if (nubBL.Selected)
                    return QuadrilateralCorrectionEffect.Nub.BottomLeft;

                return QuadrilateralCorrectionEffect.Nub.None;
            }
        }
        #endregion

        #region Events
        [Category("Action")]
        public event EventHandler ValueChanged;

        protected void OnValueChanged()
        {
            this.ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Rendering
        protected override void OnRenderForeground(
            PaintDotNet.Direct2D1.IDeviceContext deviceContext,
            RectFloat clipRect,
            RectFloat bitmapRect)
        {
            ArgumentNullException.ThrowIfNull(deviceContext);


            using ISolidColorBrush whiteBrush = deviceContext.CreateSolidColorBrush(Color.White);
            using ISolidColorBrush blackBrush = deviceContext.CreateSolidColorBrush(Color.Black);
            using ISolidColorBrush selectedBrush = deviceContext.CreateSolidColorBrush(Color.DodgerBlue);

            using IStrokeStyle dashStyle = this.Direct2DFactory.CreateStrokeStyle(
                new StrokeStyleProperties { DashStyle = DashStyle.Dash }, []
            );

            using IStrokeStyle dotStyle = this.Direct2DFactory.CreateStrokeStyle(
                new StrokeStyleProperties { DashStyle = DashStyle.Dot }, []
            );

            // Draw quadrilateral
            deviceContext.DrawLine(nubTL.Point2Float, nubTR.Point2Float, whiteBrush, 1.0f, dashStyle);
            deviceContext.DrawLine(nubTR.Point2Float, nubBR.Point2Float, whiteBrush, 1.0f, dashStyle);
            deviceContext.DrawLine(nubBR.Point2Float, nubBL.Point2Float, whiteBrush, 1.0f, dashStyle);
            deviceContext.DrawLine(nubBL.Point2Float, nubTL.Point2Float, whiteBrush, 1.0f, dashStyle);

            deviceContext.DrawLine(nubTL.Point2Float, nubTR.Point2Float, blackBrush, 1.0f, dotStyle);
            deviceContext.DrawLine(nubTR.Point2Float, nubBR.Point2Float, blackBrush, 1.0f, dotStyle);
            deviceContext.DrawLine(nubBR.Point2Float, nubBL.Point2Float, blackBrush, 1.0f, dotStyle);
            deviceContext.DrawLine(nubBL.Point2Float, nubTL.Point2Float, blackBrush, 1.0f, dotStyle);

            // Draw Nubs
            // Top Left control nub
            DrawNub(deviceContext, nubTL, 0, 0, whiteBrush, blackBrush, selectedBrush);
            // Top Right control nub
            DrawNub(deviceContext, nubTR, -1, 0, whiteBrush, blackBrush, selectedBrush);
            // Bottom Right control nub
            DrawNub(deviceContext, nubBR, -1, -1, whiteBrush, blackBrush, selectedBrush);
            // Bottom Left control nub
            DrawNub(deviceContext, nubBL, 0, -1, whiteBrush, blackBrush, selectedBrush);

            // Draw local magnified view after the nubs so it stays visible.
            DrawMagnifier(deviceContext);
        }

        private void DrawNub(
            PaintDotNet.Direct2D1.IDeviceContext deviceContext,
            Nub nub,
            int offsetX,
            int offsetY,
            IDeviceBrush whiteBrush,
            IDeviceBrush blackBrush,
            IDeviceBrush selectedBrush)
        {
            int radius = (nub.Hovered || nub.Selected) ? RadiusLarge : RadiusSmall;

            float diameter = (radius * 2) + 1;
            float d2dRadius = diameter / 2.0f;

            Ellipse bounds = new Ellipse(nub.Location.X + offsetX + 0.5f, nub.Location.Y + offsetY + 0.5f, d2dRadius, d2dRadius);

            deviceContext.DrawEllipse(bounds, whiteBrush, 4.0f);
            deviceContext.DrawEllipse(bounds, nub.Selected ? selectedBrush : blackBrush, 1.6f);
        }
        #endregion

        #region Resource Management
        protected override void OnInvalidateDeviceResources()
        {
            DisposeMagnifierDeviceResources();

            base.OnInvalidateDeviceResources();
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                this.Bitmap = null;

                DisposeMagnifierDeviceResources();
                magnifierTextFormat?.Dispose();
                magnifierTextFormat = null;

                DisposePreviewBitmapResources();
            }

            base.OnDispose(disposing);
        }

        private void DisposePreviewBitmapResources()
        {
            previewBitmapSource?.Dispose();
            previewBitmapSource = null;

            previewBitmap?.Dispose();
            previewBitmap = null;

            previewBitmapSize = default;
        }

        private void DisposeMagnifierDeviceResources()
        {
            magnifierDeviceBitmap?.Dispose();
            magnifierDeviceBitmap = null;
        }
        #endregion

        #region Mouse events
        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseIsDown = true; // because the mouse button is down
            magnifierMouseLocation = e.Location; // has the location of the mouse pointer when the button is pressed

            if (e.Button == MouseButtons.Right)
            {
                // find which control nub is being activated (if any)
                if (NearNub(e.Location, nubTL))
                {
                    SelectNub(nubTL);
                }
                else if (NearNub(e.Location, nubTR))
                {
                    SelectNub(nubTR);
                }
                else if (NearNub(e.Location, nubBR))
                {
                    SelectNub(nubBR);
                }
                else if (NearNub(e.Location, nubBL))
                {
                    SelectNub(nubBL);
                }

                showMagnifier = HasSelectedNub();
                this.Focus();
            }
            else
            {
                if (MoveNearestNubOnClick && e.Button == MouseButtons.Left)
                {
                    Nub nub = GetNearestNub(e.Location);
                    Point mouseLocation = new Point(ClampToWidth(e.X), ClampToHeight(e.Y));

                    nub.Location = SnapToNearestLineIfEnabled(mouseLocation);
                    mouseLocation = nub.Location;

                    GrabNub(nub, mouseLocation);
                    showMagnifier = true;
                    OnValueChanged();
                }
                else
                {
                    if (NearNub(e.Location, nubTL))
                    {
                        GrabNub(nubTL, e.Location);
                    }
                    else if (NearNub(e.Location, nubTR))
                    {
                        GrabNub(nubTR, e.Location);
                    }
                    else if (NearNub(e.Location, nubBR))
                    {
                        GrabNub(nubBR, e.Location);
                    }
                    else if (NearNub(e.Location, nubBL))
                    {
                        GrabNub(nubBL, e.Location);
                    }

                    showMagnifier = HasGrabbedNub();
                }
            }

            this.Invalidate();

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            nubTL.Grabbed = false;
            nubTR.Grabbed = false;
            nubBR.Grabbed = false;
            nubBL.Grabbed = false;
            MouseIsDown = false;

            // Keep magnifier visible when a nub is selected for keyboard fine-tuning.
            showMagnifier = HasSelectedNub();

            this.Invalidate();
            OnValueChanged();

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            magnifierMouseLocation = e.Location;

            if (!MouseIsDown)
            {
                if (NearNub(e.Location, nubTL))
                {
                    HoverNub(nubTL);
                }
                else if (NearNub(e.Location, nubTR))
                {
                    HoverNub(nubTR);
                }
                else if (NearNub(e.Location, nubBR))
                {
                    HoverNub(nubBR);
                }
                else if (NearNub(e.Location, nubBL))
                {
                    HoverNub(nubBL);
                }
                else
                {
                    UnHoverNubs();
                }

                // Show while hovering selected nub only; otherwise avoid visual noise.
                showMagnifier = HasSelectedNub();
            }
            else if (nubTL.Grabbed)
            {
                Point targetPoint = GetDragTargetPoint(nubTL, e);

                nubTL.Location = SnapToNearestLineIfEnabled(targetPoint);

                showMagnifier = true;
            }
            else if (nubTR.Grabbed)
            {
                Point targetPoint = GetDragTargetPoint(nubTR, e);

                nubTR.Location = SnapToNearestLineIfEnabled(targetPoint);

                showMagnifier = true;
            }
            else if (nubBR.Grabbed)
            {
                Point targetPoint = GetDragTargetPoint(nubBR, e);

                nubBR.Location = SnapToNearestLineIfEnabled(targetPoint);

                showMagnifier = true;
            }
            else if (nubBL.Grabbed)
            {
                Point targetPoint = GetDragTargetPoint(nubBL, e);

                nubBL.Location = SnapToNearestLineIfEnabled(targetPoint);

                showMagnifier = true;
            }

            this.Invalidate();
            if (MouseIsDown)
                OnValueChanged();

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!MouseIsDown && !HasSelectedNub())
            {
                showMagnifier = false;
            }

            this.Invalidate();
            UnHoverNubs();

            base.OnMouseLeave(e);
        }
        #endregion

        #region Keyboard events
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int step = ((keyData & Keys.Control) == Keys.Control) ? 5 : 1;

            Keys keyCode = keyData & Keys.KeyCode;

            if (keyCode == Keys.Add || keyCode == Keys.Oemplus)
            {
                MagnifierZoom = Math.Min(MagnifierMaxZoom, MagnifierZoom + 1);
                showMagnifier = HasSelectedNub() || HasGrabbedNub();
                this.Invalidate();

                return true;
            }

            if (keyCode == Keys.Subtract || keyCode == Keys.OemMinus)
            {
                MagnifierZoom = Math.Max(MagnifierMinZoom, MagnifierZoom - 1);
                showMagnifier = HasSelectedNub() || HasGrabbedNub();
                this.Invalidate();

                return true;
            }

            if (keyCode == Keys.Left || keyCode == Keys.Right || keyCode == Keys.Up || keyCode == Keys.Down)
            {
                Nub nub = GetSelectedNub();

                if (nub != null)
                {
                    switch (keyCode)
                    {
                        case Keys.Left:
                            nub.X = ClampToWidth(nub.X - step);
                            break;

                        case Keys.Right:
                            nub.X = ClampToWidth(nub.X + step);
                            break;

                        case Keys.Up:
                            nub.Y = ClampToHeight(nub.Y - step);
                            break;

                        case Keys.Down:
                            nub.Y = ClampToHeight(nub.Y + step);
                            break;
                    }

                    showMagnifier = true;
                    magnifierMouseLocation = nub.Location;

                    OnValueChanged();
                    this.Invalidate();

                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region Nub Helpers
        private void SelectNub(Nub nub)
        {
            Nub targetNub = null;

            if (nub.Location == nubTL.Location)
            {
                targetNub = nubTL;
            }
            else if (nub.Location == nubTR.Location)
            {
                targetNub = nubTR;
            }
            else if (nub.Location == nubBR.Location)
            {
                targetNub = nubBR;
            }
            else if (nub.Location == nubBL.Location)
            {
                targetNub = nubBL;
            }

            bool wasSelected = targetNub?.Selected ?? false;
            Point targetLocation = targetNub?.Location ?? nub.Location;

            nubTL.Selected = false;
            nubTR.Selected = false;
            nubBR.Selected = false;
            nubBL.Selected = false;

            nubTL.Grabbed = false;
            nubTR.Grabbed = false;
            nubBR.Grabbed = false;
            nubBL.Grabbed = false;

            nubTL.Hovered = false;
            nubTR.Hovered = false;
            nubBR.Hovered = false;
            nubBL.Hovered = false;

            if (targetNub != null)
            {
                targetNub.Selected = !wasSelected;
                targetNub.Hovered = true;

                magnifierMouseLocation = targetLocation;
            }

            this.Cursor = targetNub != null && targetNub.Selected
                ? handOpen
                : Cursors.Default;
        }

        private void GrabNub(Nub nub, Point mouseLocation)
        {
            nubTL.Grabbed = false;
            nubTR.Grabbed = false;
            nubBR.Grabbed = false;
            nubBL.Grabbed = false;

            nubTL.Hovered = false;
            nubTR.Hovered = false;
            nubBR.Hovered = false;
            nubBL.Hovered = false;

            nubTL.Selected = false;
            nubTR.Selected = false;
            nubBR.Selected = false;
            nubBL.Selected = false;

            if (nub.Location == nubTL.Location)
            {
                nubTL.Grabbed = true;
                nubTL.Hovered = true;
            }
            else if (nub.Location == nubTR.Location)
            {
                nubTR.Grabbed = true;
                nubTR.Hovered = true;
            }
            else if (nub.Location == nubBR.Location)
            {
                nubBR.Grabbed = true;
                nubBR.Hovered = true;
            }
            else if (nub.Location == nubBL.Location)
            {
                nubBL.Grabbed = true;
                nubBL.Hovered = true;
            }

            MouseFromNub.Width = mouseLocation.X - nub.X;
            MouseFromNub.Height = mouseLocation.Y - nub.Y;

            magnifierMouseLocation = nub.Location;
            this.Cursor = handGrab;
        }

        private void UnHoverNubs()
        {
            nubTL.Hovered = false;
            nubTR.Hovered = false;
            nubBR.Hovered = false;
            nubBL.Hovered = false;

            this.Cursor = Cursors.Default;
        }

        private void HoverNub(Nub nub)
        {
            nubTL.Hovered = false;
            nubTR.Hovered = false;
            nubBR.Hovered = false;
            nubBL.Hovered = false;

            if (nub.Location == nubTL.Location)
            {
                nubTL.Hovered = true;
            }
            else if (nub.Location == nubTR.Location)
            {
                nubTR.Hovered = true;
            }
            else if (nub.Location == nubBR.Location)
            {
                nubBR.Hovered = true;
            }
            else if (nub.Location == nubBL.Location)
            {
                nubBL.Hovered = true;
            }

            this.Cursor = handOpen;
        }

        private Nub GetSelectedNub()
        {
            if (nubTL.Selected) return nubTL;
            if (nubTR.Selected) return nubTR;
            if (nubBR.Selected) return nubBR;
            if (nubBL.Selected) return nubBL;

            return null;
        }

        private Nub GetActiveNub()
        {
            if (nubTL.Grabbed || nubTL.Selected) return nubTL;
            if (nubTR.Grabbed || nubTR.Selected) return nubTR;
            if (nubBR.Grabbed || nubBR.Selected) return nubBR;
            if (nubBL.Grabbed || nubBL.Selected) return nubBL;

            return null;
        }

        private bool HasSelectedNub()
        {
            return nubTL.Selected || nubTR.Selected || nubBR.Selected || nubBL.Selected;
        }

        private bool HasGrabbedNub()
        {
            return nubTL.Grabbed || nubTR.Grabbed || nubBR.Grabbed || nubBL.Grabbed;
        }

        private Nub GetNearestNub(Point mouseLocation)
        {
            Nub nearestNub = nubTL;
            int nearestDistanceSquared = GetDistanceSquared(mouseLocation, nubTL);

            int distanceSquared = GetDistanceSquared(mouseLocation, nubTR);
            if (distanceSquared < nearestDistanceSquared)
            {
                nearestNub = nubTR;
                nearestDistanceSquared = distanceSquared;
            }

            distanceSquared = GetDistanceSquared(mouseLocation, nubBR);
            if (distanceSquared < nearestDistanceSquared)
            {
                nearestNub = nubBR;
                nearestDistanceSquared = distanceSquared;
            }

            distanceSquared = GetDistanceSquared(mouseLocation, nubBL);
            if (distanceSquared < nearestDistanceSquared)
            {
                nearestNub = nubBL;
                nearestDistanceSquared = distanceSquared;
            }

            return nearestNub;
        }

        private static int GetDistanceSquared(Point mouseLocation, Nub nub)
        {
            int xDist = mouseLocation.X - nub.X;
            int yDist = mouseLocation.Y - nub.Y;

            return xDist * xDist + yDist * yDist;
        }

        private static bool NearNub(Point mouseLocation, Nub nub)
        {
            int xDist = mouseLocation.X - nub.X;
            int yDist = mouseLocation.Y - nub.Y;
            return (Math.Sqrt(xDist * xDist + yDist * yDist) <= RadiusHover);
        }
        #endregion

        #region Magnifier

        private void DrawMagnifier(PaintDotNet.Direct2D1.IDeviceContext deviceContext)
        {
            if (!showMagnifier || !HasPreviewBitmap || this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
                return;

            Nub activeNub = GetActiveNub();

            if (activeNub == null)
                return;

            Point nubClientPoint = activeNub.Location;

            if (nubClientPoint.X < 0 || nubClientPoint.Y < 0 || nubClientPoint.X >= this.ClientSize.Width || nubClientPoint.Y >= this.ClientSize.Height)
                return;

            Point imagePoint = ClientToImagePoint(nubClientPoint);

            Rectangle srcRect = GetMagnifierSourceRectangle(imagePoint);

            if (srcRect.Width <= 0 || srcRect.Height <= 0)
                return;

            Rectangle destRect = GetMagnifierDestinationRectangle(nubClientPoint);

            RectFloat targetRect = RectFloat.FromEdges(destRect.Left, destRect.Top, destRect.Right, destRect.Bottom);
            RectFloat sourceRect = RectFloat.FromEdges(srcRect.Left, srcRect.Top, srcRect.Right, srcRect.Bottom);

            if (magnifierDeviceBitmap == null)
                magnifierDeviceBitmap = deviceContext.CreateBitmap(previewBitmapSource);

            using ISolidColorBrush backgroundBrush = deviceContext.CreateSolidColorBrush(Color.FromArgb(230, Color.White));
            using ISolidColorBrush borderBrush = deviceContext.CreateSolidColorBrush(Color.Black);
            using ISolidColorBrush crossBrush = deviceContext.CreateSolidColorBrush(Color.Red);
            using ISolidColorBrush textBackBrush = deviceContext.CreateSolidColorBrush(Color.FromArgb(200, Color.White));
            using ISolidColorBrush textBrush = deviceContext.CreateSolidColorBrush(Color.Black);

            // Background first, same as old GDI+ implementation.
            deviceContext.FillRectangle(targetRect, backgroundBrush);

            // Draw cropped image area into magnifier rectangle with nearest-neighbor scaling.
            RectFloat? targetRectNullable = targetRect;
            RectFloat? sourceRectNullable = sourceRect;

            deviceContext.DrawBitmap(magnifierDeviceBitmap, in targetRectNullable, 1.0f, InterpolationMode.NearestNeighbor, in sourceRectNullable);

            // Border.
            deviceContext.DrawRectangle(targetRect, borderBrush, 1.0f);

            float scaleX = (float)destRect.Width / srcRect.Width;
            float scaleY = (float)destRect.Height / srcRect.Height;

            float centerX = destRect.Left + ((imagePoint.X - srcRect.Left) + 0.5f) * scaleX;
            float centerY = destRect.Top + ((imagePoint.Y - srcRect.Top) + 0.5f) * scaleY;

            // Crosshair.
            deviceContext.DrawLine(new Point2Float(centerX, destRect.Top), new Point2Float(centerX, destRect.Bottom), crossBrush, 1.0f);
            deviceContext.DrawLine(new Point2Float(destRect.Left, centerY), new Point2Float(destRect.Right, centerY), crossBrush, 1.0f);

            DrawMagnifierText(deviceContext, destRect, imagePoint, textBackBrush, textBrush);
        }

        private void DrawMagnifierText(
            PaintDotNet.Direct2D1.IDeviceContext deviceContext,
            Rectangle destRect,
            Point imagePoint,
            IDeviceBrush textBackBrush,
            IDeviceBrush textBrush)
        {
            string text = $"X:{imagePoint.X}, Y:{imagePoint.Y}, {MagnifierZoom}x";


            if (magnifierTextFormat == null)
            {
                float fontSizeDip = this.Font.SizeInPoints * 96.0f / 72.0f;

                magnifierTextFormat = this.DirectWriteFactory.CreateTextFormat(
                    this.Font.FontFamily.Name,
                    null,
                    FontWeight.Normal,
                    PaintDotNet.DirectWrite.FontStyle.Normal,
                    FontStretch.Normal,
                    fontSizeDip,
                    null);
                magnifierTextFormat.WordWrapping = WordWrapping.NoWrap;
            }

            // Match the old GDI+ MeasureString + DrawString layout.
            const float backLeftMargin = 3.0f;
            const float backBottomOffset = 5.0f;
            const float textOffsetX = 2.0f;
            const float textOffsetY = 1.0f;

            float maxTextWidth = Math.Max(1.0f, destRect.Width - 6.0f);
            float maxTextHeight = Math.Max(1.0f, destRect.Height);

            using ITextLayout textLayout = this.DirectWriteFactory.CreateTextLayout(
                text,
                magnifierTextFormat,
                maxTextWidth,
                maxTextHeight);

            TextMetrics metrics = textLayout.Metrics;

            float textWidth = (float)Math.Ceiling(metrics.WidthIncludingTrailingWhitespace);
            float textHeight = (float)Math.Ceiling(metrics.Height);

            float backLeft = destRect.Left + backLeftMargin;
            float backTop = destRect.Bottom - textHeight - backBottomOffset;
            float backRight = backLeft + textWidth + (textOffsetX * 2.0f);
            float backBottom = backTop + textHeight + (textOffsetY * 2.0f);

            RectFloat textBackRect = RectFloat.FromEdges(
                backLeft,
                backTop,
                backRight,
                backBottom);

            deviceContext.FillRectangle(textBackRect, textBackBrush);

            RectFloat textRect = RectFloat.FromEdges(
                textBackRect.Left + textOffsetX,
                textBackRect.Top + textOffsetY,
                textBackRect.Right - textOffsetX,
                textBackRect.Bottom - textOffsetY);

            deviceContext.DrawText(
                text,
                magnifierTextFormat,
                textRect,
                textBrush,
                DrawTextOptions.Clip);
        }

        private Rectangle GetMagnifierDestinationRectangle(Point nubClientPoint)
        {
            int offset = 20;

            Rectangle destRect = new Rectangle(
                magnifierMouseLocation.X + offset,
                magnifierMouseLocation.Y + offset,
                MagnifierSize,
                MagnifierSize);

            if (destRect.Right > this.ClientSize.Width)
            {
                destRect.X = magnifierMouseLocation.X - MagnifierSize - offset;
            }

            if (destRect.Bottom > this.ClientSize.Height)
            {
                destRect.Y = magnifierMouseLocation.Y - MagnifierSize - offset;
            }

            if (destRect.Left < 0)
            {
                destRect.X = Math.Min(this.ClientSize.Width - MagnifierSize, nubClientPoint.X + offset);
            }

            if (destRect.Top < 0)
            {
                destRect.Y = Math.Min(this.ClientSize.Height - MagnifierSize, nubClientPoint.Y + offset);
            }

            if (destRect.Left < 0)
            {
                destRect.X = 0;
            }

            if (destRect.Top < 0)
            {
                destRect.Y = 0;
            }

            return destRect;
        }

        private Point ClientToImagePoint(Point clientPoint)
        {
            if (!HasPreviewBitmap || this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
            {
                return Point.Empty;
            }

            int x = (int)Math.Round((double)clientPoint.X * (previewBitmapSize.Width - 1) / Math.Max(1, this.ClientSize.Width - 1));
            int y = (int)Math.Round((double)clientPoint.Y * (previewBitmapSize.Height - 1) / Math.Max(1, this.ClientSize.Height - 1));

            x = Math.Max(0, Math.Min(previewBitmapSize.Width - 1, x));
            y = Math.Max(0, Math.Min(previewBitmapSize.Height - 1, y));

            return new Point(x, y);
        }

        private Rectangle GetMagnifierSourceRectangle(Point imagePoint)
        {
            int zoom = Math.Max(1, MagnifierZoom);

            int srcSize = Math.Max(1, MagnifierSize / zoom);

            if (HasPreviewBitmap)
            {
                srcSize = Math.Min(srcSize, Math.Min(previewBitmapSize.Width, previewBitmapSize.Height));
            }

            int halfSrcSize = srcSize / 2;

            int srcX = imagePoint.X - halfSrcSize;
            int srcY = imagePoint.Y - halfSrcSize;

            if (HasPreviewBitmap)
            {
                srcX = Math.Max(0, Math.Min(previewBitmapSize.Width - srcSize, srcX));
                srcY = Math.Max(0, Math.Min(previewBitmapSize.Height - srcSize, srcY));
            }

            return new Rectangle(srcX, srcY, srcSize, srcSize);
        }

        #endregion

        #region Preview Bitmap
        private void SetPreviewBitmap(IBitmap<ColorBgra32> bitmap)
        {
            this.Bitmap = null;

            DisposeMagnifierDeviceResources();
            DisposePreviewBitmapResources();

            previewBitmap = bitmap;

            if (previewBitmap != null)
            {
                previewBitmapSize = previewBitmap.Size;

                // Direct2D CreateBitmap prefers a D2D-friendly premultiplied BGRA source.
                previewBitmapSource = this.ImagingFactory.CreateFormatConvertedBitmap(
                    previewBitmap,
                    PixelFormats.Pbgra32,
                    default,
                    null,
                    0.0,
                    default);

                this.Bitmap = previewBitmapSource;
            }
            else
            {
                previewBitmapSize = default;
            }

            this.InvalidateBitmap();
            this.Invalidate();
        }

        private bool HasPreviewBitmap
        {
            get => previewBitmapSource != null
                && previewBitmapSize.Width > 0
                && previewBitmapSize.Height > 0;
        }
        #endregion

        #region Coordinate Helpers
        private int ClampToWidth(int x)
        {
            if (AllowNubsOutsideImage)
                return x;
            return (x < 0) ? 0 : (x > this.ClientSize.Width - 1) ? this.ClientSize.Width - 1 : x;
        }

        private int ClampToHeight(int y)
        {
            if (AllowNubsOutsideImage)
                return y;
            return (y < 0) ? 0 : (y > this.ClientSize.Height - 1) ? this.ClientSize.Height - 1 : y;
        }

        private Point GetDragTargetPoint(Nub nub, MouseEventArgs e)
        {
            int x = nub.X;
            int y = nub.Y;

            if (e.Button == MouseButtons.Middle)
            {
                if (e.X <= nub.X - DeadZone)
                {
                    x = ClampToWidth(e.X + DeadZone);
                }
                else if (e.X >= nub.X + DeadZone)
                {
                    x = ClampToWidth(e.X - DeadZone);
                }

                if (e.Y <= nub.Y - DeadZone)
                {
                    y = ClampToHeight(e.Y + DeadZone);
                }
                else if (e.Y >= nub.Y + DeadZone)
                {
                    y = ClampToHeight(e.Y - DeadZone);
                }
            }
            else
            {
                x = ClampToWidth(e.X - MouseFromNub.Width);
                y = ClampToHeight(e.Y - MouseFromNub.Height);
            }

            return new Point(x, y);
        }
        #endregion

        #region Line Snap
        private Point SnapToNearestLineIfEnabled(Point controlPoint)
        {
            if (AllowNubsOutsideImage)
            {
                if (controlPoint.X < 0 || controlPoint.Y < 0 || controlPoint.X >= this.ClientSize.Width || controlPoint.Y >= this.ClientSize.Height)
                {
                    // Outside image: no image pixels to snap against.
                    return controlPoint;
                }
            }
            else
            {
                controlPoint = new Point(
                    ClampToWidth(controlPoint.X),
                    ClampToHeight(controlPoint.Y));
            }

            if (!LineSnapEnabled ||
                !HasPreviewBitmap ||
                previewBitmapSize.Width < 3 ||
                previewBitmapSize.Height < 3 ||
                this.ClientSize.Width < 3 ||
                this.ClientSize.Height < 3)
            {
                return controlPoint;
            }

            int searchRadius = Math.Max(0, LineSnapSearchRadius);
            if (searchRadius == 0)
            {
                return controlPoint;
            }

            Point bestPoint = controlPoint;
            int bestStrength = LineSnapMinEdgeStrength;
            int bestDistanceSquared = int.MaxValue;

            using (IBitmapLock<ColorBgra32> bitmapLock = previewBitmap.Lock(
                new RectInt32(0, 0, previewBitmap.Size),
                BitmapLockOptions.Read))
            {
                RegionPtr<ColorBgra32> region = bitmapLock.AsRegionPtr();

                int searchRadiusSquared = searchRadius * searchRadius;

                for (int dy = -searchRadius; dy <= searchRadius; dy++)
                {
                    for (int dx = -searchRadius; dx <= searchRadius; dx++)
                    {
                        int distanceSquared = dx * dx + dy * dy;
                        if (distanceSquared > searchRadiusSquared)
                        {
                            continue;
                        }

                        Point candidate = AllowNubsOutsideImage
                            ? new Point(controlPoint.X + dx, controlPoint.Y + dy)
                            : new Point(
                                ClampToWidth(controlPoint.X + dx),
                                ClampToHeight(controlPoint.Y + dy));

                        Point imagePoint = ClientToImagePoint(candidate);

                        // Sobel needs a 1-pixel border.
                        if (imagePoint.X <= 0 ||
                            imagePoint.Y <= 0 ||
                            imagePoint.X >= previewBitmapSize.Width - 1 ||
                            imagePoint.Y >= previewBitmapSize.Height - 1)
                        {
                            continue;
                        }

                        //get sobel edge strength
                        int tl = GetLuma(region[imagePoint.X - 1, imagePoint.Y - 1]);
                        int tc = GetLuma(region[imagePoint.X, imagePoint.Y - 1]);
                        int tr = GetLuma(region[imagePoint.X + 1, imagePoint.Y - 1]);

                        int ml = GetLuma(region[imagePoint.X - 1, imagePoint.Y]);
                        int mr = GetLuma(region[imagePoint.X + 1, imagePoint.Y]);

                        int bl = GetLuma(region[imagePoint.X - 1, imagePoint.Y + 1]);
                        int bc = GetLuma(region[imagePoint.X, imagePoint.Y + 1]);
                        int br = GetLuma(region[imagePoint.X + 1, imagePoint.Y + 1]);

                        int gx = -tl - (2 * ml) - bl + tr + (2 * mr) + br;
                        int gy = -tl - (2 * tc) - tr + bl + (2 * bc) + br;

                        int strength = Math.Abs(gx) + Math.Abs(gy);

                        if (strength > bestStrength ||
                            (strength == bestStrength && distanceSquared < bestDistanceSquared))
                        {
                            bestStrength = strength;
                            bestDistanceSquared = distanceSquared;
                            bestPoint = candidate;
                        }
                    }
                }
            }

            return bestPoint;
        }

        private static int GetLuma(ColorBgra32 color)
        {
            return (int)Math.Round((0.299 * color.R) + (0.587 * color.G) + (0.114 * color.B));
        }
        #endregion

        private class Nub
        {
            internal Point Location
            {
                get => new Point(this.X, this.Y);
                set
                {
                    this.X = value.X;
                    this.Y = value.Y;
                }
            }
            internal int X { get; set; }
            internal int Y { get; set; }
            internal bool Grabbed { get; set; }
            internal bool Hovered { get; set; }
            internal bool Selected { get; set; }
            internal Nub(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            internal Point2Float Point2Float => new Point2Float(this.X, this.Y);
        }
    }

    internal enum Nub
    {
        None,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft
    }
}
