using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace QuadrilateralCorrectionEffect
{
    [DefaultEvent(nameof(ValueChanged))]
    internal class QuadControl : PictureBox
    {
        private static readonly Cursor handOpen = new Cursor(typeof(QuadControl), "Resources.HandOpen.cur");
        private static readonly Cursor handGrab = new Cursor(typeof(QuadControl), "Resources.HandGrab.cur");

        public QuadControl()
        {
            this.BackgroundImage = new Bitmap(typeof(QuadControl), "Resources.CheckerBoard.png");
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            this.TabStop = false;
            this.BorderStyle = BorderStyle.FixedSingle;

            nubTL = new Nub(0, 0);
            nubTR = new Nub(this.ClientSize.Width - 1, 0);
            nubBR = new Nub(this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            nubBL = new Nub(0, this.ClientSize.Height - 1);
        }

        #region Variables
        private bool MouseIsDown = false; // True if mouse button is down
        private Size MouseFromNub = Size.Empty;
        private const int RadiusSmall = 3; // nb Radius * 2 + 1 = size
        private const int RadiusLarge = 5;
        private const int RadiusHover = 16;
        private const int DeadZone = 30;
        private Nub nubTL, nubTR, nubBR, nubBL; // four Nubs to store coordinates and activation states

        // Magnifier variables.
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

        #region Event handler
        [Category("Action")]
        public event EventHandler ValueChanged;

        protected void OnValueChanged()
        {
            this.ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        protected override void OnPaint(PaintEventArgs pe)
        {
            DrawPreviewImage(pe.Graphics);

            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pe.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            // Draw quadrilateral
            using (Pen outlinePen = new Pen(Color.Black))
            {
                outlinePen.Color = Color.White;
                outlinePen.DashStyle = DashStyle.Dash;
                pe.Graphics.DrawLine(outlinePen, nubTL.Location, nubTR.Location);
                pe.Graphics.DrawLine(outlinePen, nubTR.Location, nubBR.Location);
                pe.Graphics.DrawLine(outlinePen, nubBR.Location, nubBL.Location);
                pe.Graphics.DrawLine(outlinePen, nubBL.Location, nubTL.Location);

                outlinePen.Color = Color.Black;
                outlinePen.DashStyle = DashStyle.Dot;
                pe.Graphics.DrawLine(outlinePen, nubTL.Location, nubTR.Location);
                pe.Graphics.DrawLine(outlinePen, nubTR.Location, nubBR.Location);
                pe.Graphics.DrawLine(outlinePen, nubBR.Location, nubBL.Location);
                pe.Graphics.DrawLine(outlinePen, nubBL.Location, nubTL.Location);
            }

            // Draw Nubs
            using (Pen nubPen = new Pen(Color.White, 4))
            using (Pen nubStatePen = new Pen(Color.Black, 1.6f))
            {
                int radius;

                // Top Left control nub
                radius = (nubTL.Hovered || nubTL.Selected) ? RadiusLarge : RadiusSmall;
                pe.Graphics.DrawEllipse(nubPen, nubTL.X - radius, nubTL.Y - radius, radius * 2 + 1, radius * 2 + 1);
                nubStatePen.Color = (nubTL.Selected) ? Color.DodgerBlue : Color.Black;
                pe.Graphics.DrawEllipse(nubStatePen, nubTL.X - radius, nubTL.Y - radius, radius * 2 + 1, radius * 2 + 1);

                // Top Right control nub
                radius = (nubTR.Hovered || nubTR.Selected) ? RadiusLarge : RadiusSmall;
                pe.Graphics.DrawEllipse(nubPen, nubTR.X - radius - 1, nubTR.Y - radius, radius * 2 + 1, radius * 2 + 1);
                nubStatePen.Color = (nubTR.Selected) ? Color.DodgerBlue : Color.Black;
                pe.Graphics.DrawEllipse(nubStatePen, nubTR.X - radius - 1, nubTR.Y - radius, radius * 2 + 1, radius * 2 + 1);

                // Bottom Right control nub
                radius = (nubBR.Hovered || nubBR.Selected) ? RadiusLarge : RadiusSmall;
                pe.Graphics.DrawEllipse(nubPen, nubBR.X - radius - 1, nubBR.Y - radius - 1, radius * 2 + 1, radius * 2 + 1);
                nubStatePen.Color = (nubBR.Selected) ? Color.DodgerBlue : Color.Black;
                pe.Graphics.DrawEllipse(nubStatePen, nubBR.X - radius - 1, nubBR.Y - radius - 1, radius * 2 + 1, radius * 2 + 1);

                // Bottom Left control nub
                radius = (nubBL.Hovered || nubBL.Selected) ? RadiusLarge : RadiusSmall;
                pe.Graphics.DrawEllipse(nubPen, nubBL.X - radius, nubBL.Y - radius - 1, radius * 2 + 1, radius * 2 + 1);
                nubStatePen.Color = (nubBL.Selected) ? Color.DodgerBlue : Color.Black;
                pe.Graphics.DrawEllipse(nubStatePen, nubBL.X - radius, nubBL.Y - radius - 1, radius * 2 + 1, radius * 2 + 1);
            }

            // Draw local magnified view after the nubs so it stays visible.
            DrawMagnifier(pe.Graphics);
        }

        private void DrawPreviewImage(Graphics graphics)
        {
            Rectangle destination = new Rectangle(
                0,
                0,
                this.ClientSize.Width,
                this.ClientSize.Height);

            if (destination.Width <= 0 || destination.Height <= 0) return;

            // Draw the background first to prevent transparent layers or edges from revealing unpainted areas.
            if (this.BackgroundImage != null)
            {
                using TextureBrush brush = new TextureBrush(this.BackgroundImage, WrapMode.Tile);
                graphics.FillRectangle(brush, destination);
            }
            else
            {
                using SolidBrush brush = new SolidBrush(this.BackColor);
                graphics.FillRectangle(brush, destination);
            }

            if (this.Image == null) return;

            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;

            using ImageAttributes imageAttributes = new ImageAttributes();

            // Prevent sampling transparent/background colors outside the image bounds during scaling.
            imageAttributes.SetWrapMode(WrapMode.TileFlipXY);

            graphics.DrawImage(
                this.Image,
                destination,
                0,
                0,
                this.Image.Width,
                this.Image.Height,
                GraphicsUnit.Pixel,
                imageAttributes);
        }

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

        #region Nub functions
        private void SelectNub(Nub nub)
        {
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

            if (nub.Location == nubTL.Location)
            {
                nubTL.Selected = true;
                nubTL.Hovered = true;
            }
            else if (nub.Location == nubTR.Location)
            {
                nubTR.Selected = true;
                nubTR.Hovered = true;
            }
            else if (nub.Location == nubBR.Location)
            {
                nubBR.Selected = true;
                nubBR.Hovered = true;
            }
            else if (nub.Location == nubBL.Location)
            {
                nubBL.Selected = true;
                nubBL.Hovered = true;
            }

            magnifierMouseLocation = nub.Location;

            this.Cursor = handOpen;
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

        private void DrawMagnifier(Graphics g)
        {
            if (!showMagnifier || this.Image == null || this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
            {
                return;
            }

            Nub activeNub = GetActiveNub();

            if (activeNub == null)
            {
                return;
            }

            Point nubClientPoint = activeNub.Location;
            Point imagePoint = ClientToImagePoint(nubClientPoint);

            int srcSize = Math.Max(1, MagnifierSize / Math.Max(1, MagnifierZoom));
            int halfSrcSize = srcSize / 2;

            Rectangle srcRect = new Rectangle(
                imagePoint.X - halfSrcSize,
                imagePoint.Y - halfSrcSize,
                srcSize,
                srcSize);

            Rectangle imageBounds = new Rectangle(0, 0, this.Image.Width, this.Image.Height);
            srcRect.Intersect(imageBounds);

            if (srcRect.Width <= 0 || srcRect.Height <= 0)
            {
                return;
            }

            Rectangle destRect = GetMagnifierDestinationRectangle(nubClientPoint);

            SmoothingMode oldSmoothingMode = g.SmoothingMode;
            InterpolationMode oldInterpolationMode = g.InterpolationMode;
            PixelOffsetMode oldPixelOffsetMode = g.PixelOffsetMode;
            CompositingQuality oldCompositingQuality = g.CompositingQuality;

            try
            {
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.CompositingQuality = CompositingQuality.HighSpeed;

                using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(230, Color.White)))
                using (Pen borderPen = new Pen(Color.Black, 1))
                using (Pen crossPen = new Pen(Color.Red, 1))
                using (SolidBrush textBackBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
                using (SolidBrush textBrush = new SolidBrush(Color.Black))
                {
                    g.FillRectangle(backgroundBrush, destRect);
                    g.DrawImage(this.Image, destRect, srcRect, GraphicsUnit.Pixel);
                    g.DrawRectangle(borderPen, destRect);

                    int centerX = destRect.Left + destRect.Width / 2;
                    int centerY = destRect.Top + destRect.Height / 2;

                    g.DrawLine(crossPen, centerX, destRect.Top, centerX, destRect.Bottom);
                    g.DrawLine(crossPen, destRect.Left, centerY, destRect.Right, centerY);

                    string text = $"X:{imagePoint.X}, Y:{imagePoint.Y}, {MagnifierZoom}x";
                    SizeF textSize = g.MeasureString(text, this.Font);

                    RectangleF textBackRect = new RectangleF(
                        destRect.Left + 3,
                        destRect.Bottom - textSize.Height - 5,
                        textSize.Width + 4,
                        textSize.Height + 2);

                    g.FillRectangle(textBackBrush, textBackRect);
                    g.DrawString(text, this.Font, textBrush, textBackRect.Left + 2, textBackRect.Top + 1);
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothingMode;
                g.InterpolationMode = oldInterpolationMode;
                g.PixelOffsetMode = oldPixelOffsetMode;
                g.CompositingQuality = oldCompositingQuality;
            }
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
            if (this.Image == null || this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
            {
                return Point.Empty;
            }

            int x = (int)Math.Round((double)clientPoint.X * (this.Image.Width - 1) / Math.Max(1, this.ClientSize.Width - 1));
            int y = (int)Math.Round((double)clientPoint.Y * (this.Image.Height - 1) / Math.Max(1, this.ClientSize.Height - 1));

            x = Math.Max(0, Math.Min(this.Image.Width - 1, x));
            y = Math.Max(0, Math.Min(this.Image.Height - 1, y));

            return new Point(x, y);
        }

        #endregion

        #region Utility routines
        private int ClampToWidth(int x)
        {
            return (x < 0) ? 0 : (x > this.ClientSize.Width - 1) ? this.ClientSize.Width - 1 : x;
        }

        private int ClampToHeight(int y)
        {
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

        private Point SnapToNearestLineIfEnabled(Point controlPoint)
        {
            controlPoint = new Point(
                ClampToWidth(controlPoint.X),
                ClampToHeight(controlPoint.Y));

            if (!LineSnapEnabled ||
                this.Image is not Bitmap bitmap ||
                bitmap.Width < 3 ||
                bitmap.Height < 3 ||
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

            int clientWidth = Math.Max(1, this.ClientSize.Width - 1);
            int clientHeight = Math.Max(1, this.ClientSize.Height - 1);

            for (int dy = -searchRadius; dy <= searchRadius; dy++)
            {
                for (int dx = -searchRadius; dx <= searchRadius; dx++)
                {
                    int distanceSquared = dx * dx + dy * dy;
                    if (distanceSquared > searchRadius * searchRadius)
                    {
                        continue;
                    }

                    Point candidate = new Point(
                        ClampToWidth(controlPoint.X + dx),
                        ClampToHeight(controlPoint.Y + dy));

                    int imageX = (int)Math.Round((double)candidate.X * (bitmap.Size.Width - 1) / clientWidth);
                    int imageY = (int)Math.Round((double)candidate.Y * (bitmap.Size.Height - 1) / clientHeight);

                    // control point to image point
                    Point imagePoint = new Point(
                        Math.Max(0, Math.Min(bitmap.Size.Width - 1, imageX)),
                        Math.Max(0, Math.Min(bitmap.Size.Height - 1, imageY)));

                    // Sobel needs a 1-pixel border.
                    if (imagePoint.X <= 0 ||
                        imagePoint.Y <= 0 ||
                        imagePoint.X >= bitmap.Width - 1 ||
                        imagePoint.Y >= bitmap.Height - 1)
                    {
                        continue;
                    }

                    //get sobel edge strength
                    int tl = GetLuma(bitmap.GetPixel(imagePoint.X - 1, imagePoint.Y - 1));
                    int tc = GetLuma(bitmap.GetPixel(imagePoint.X, imagePoint.Y - 1));
                    int tr = GetLuma(bitmap.GetPixel(imagePoint.X + 1, imagePoint.Y - 1));

                    int ml = GetLuma(bitmap.GetPixel(imagePoint.X - 1, imagePoint.Y));
                    int mr = GetLuma(bitmap.GetPixel(imagePoint.X + 1, imagePoint.Y));

                    int bl = GetLuma(bitmap.GetPixel(imagePoint.X - 1, imagePoint.Y + 1));
                    int bc = GetLuma(bitmap.GetPixel(imagePoint.X, imagePoint.Y + 1));
                    int br = GetLuma(bitmap.GetPixel(imagePoint.X + 1, imagePoint.Y + 1));

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

            return bestPoint;
        }

        private static int GetLuma(Color color)
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
