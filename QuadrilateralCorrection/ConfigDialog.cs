using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;

namespace QuadrilateralCorrectionEffect
{
    internal partial class QuadrilateralCorrectionConfigDialog : QuadrilateralCorrectionConfigDialogBase
    {
        private Rectangle uiImgBounds;
        private Rectangle selection;
        private Size srcImageSize; // Used for UI preview / QuadControl bounds
        private BitmapRegionUtil.BitmapBgra32Data srcImageData; // Used for perspective warp and pixel-level computations
        private bool updatingDialogFromToken;

        public QuadrilateralCorrectionConfigDialog()
        {
            InitializeComponent();
            this.UseAppThemeColors = true;

            quadControl11.LineSnapEnabled = checkBoxLineSnap.Checked;
            quadControl11.AllowNubsOutsideImage = checkBoxAllowNubsOutsideImage.Checked;

            ApplyThemeColorsToNumericUpDowns(this);
        }

        private void ApplyThemeColorsToNumericUpDowns(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is NumericUpDown numericUpDown)
                {
                    numericUpDown.ForeColor = this.ForeColor;
                    numericUpDown.BackColor = this.BackColor;
                }

                if (control.HasChildren)
                {
                    ApplyThemeColorsToNumericUpDowns(control);
                }
            }
        }

        protected override void OnLoading()
        {
            base.OnLoading();

            ApplyQuadControlImageBounds();
        }

        private bool initialized;
        private void Initializers()
        {
            if (initialized) return;

            initialized = true;

            // Read global information from this.Environment
            selection = new Rectangle(this.Environment.Selection.RenderBounds.X, this.Environment.Selection.RenderBounds.Y, this.Environment.Selection.RenderBounds.Width, this.Environment.Selection.RenderBounds.Height);

            ApplyNubNumericRanges(checkBoxAllowNubsOutsideImage.Checked);

            numericUpDownWidth.Maximum = selection.Width;
            numericUpDownHeight.Maximum = selection.Height;

            // Read the current layer as the UI image
            using IEffectInputBitmap<ColorBgra32> sourceBitmap = this.Environment.GetSourceBitmapBgra32();

            srcImageSize = new Size(sourceBitmap.Size.Width, sourceBitmap.Size.Height);

            using (IBitmapLock<ColorBgra32> sourceLock = sourceBitmap.Lock(new RectInt32(0, 0, sourceBitmap.Size)))
            {
                RegionPtr<ColorBgra32> sourceRegion = sourceLock.AsRegionPtr();

                srcImageData = BitmapRegionUtil.CreateBgra32DataFromSourceRegion(sourceRegion, sourceBitmap.Size.Width, sourceBitmap.Size.Height);

                quadControl11.SetImageFromSourceRegion(sourceRegion, sourceBitmap.Size.Width, sourceBitmap.Size.Height);
            }

            float quadBaseSize = this.AutoScaleDimensions.Width / 96f * 500f;
            float divisor = Math.Max(srcImageSize.Width, srcImageSize.Height) / quadBaseSize;

            uiImgBounds.Width = (int)Math.Round(srcImageSize.Width / divisor);
            uiImgBounds.Height = (int)Math.Round(srcImageSize.Height / divisor);
            uiImgBounds.X = (int)Math.Max(0, (quadBaseSize - uiImgBounds.Width) / 2f);
            uiImgBounds.Y = (int)Math.Max(0, (quadBaseSize - uiImgBounds.Height) / 2f);
        }

        private void ApplyQuadControlImageBounds()
        {
            if (srcImageSize.IsEmpty)
            {
                return;
            }

            Padding padding = splitContainerMain.Panel1.Padding;

            int availableWidth =
                splitContainerMain.Panel1.ClientSize.Width
                - padding.Left
                - padding.Right;

            int availableHeight =
                splitContainerMain.Panel1.ClientSize.Height
                - padding.Top
                - padding.Bottom;

            if (availableWidth <= 0 || availableHeight <= 0)
            {
                return;
            }

            float divisor = Math.Max(
                (float)srcImageSize.Width / availableWidth,
                (float)srcImageSize.Height / availableHeight);

            uiImgBounds.Width = Math.Max(1, (int)Math.Round(srcImageSize.Width / divisor));
            uiImgBounds.Height = Math.Max(1, (int)Math.Round(srcImageSize.Height / divisor));

            uiImgBounds.X = padding.Left + Math.Max(0, (availableWidth - uiImgBounds.Width) / 2);
            uiImgBounds.Y = padding.Top + Math.Max(0, (availableHeight - uiImgBounds.Height) / 2);

            quadControl11.Dock = DockStyle.None;
            quadControl11.ClientSize = new Size(uiImgBounds.Width, uiImgBounds.Height);
            quadControl11.Location = new Point(uiImgBounds.X, uiImgBounds.Y);

            quadControl11.Visible = true;
            quadControl11.Invalidate();
        }

        private void ApplyNubNumericRanges(bool allowOutside)
        {
            decimal minX = allowOutside ? -(selection.Width - 1) : 0;
            decimal minY = allowOutside ? -(selection.Height - 1) : 0;
            // When AllowNubsOutsideImage is enabled, NumericUpDown allows values up to 2 times beyond the original image bounds.
            decimal maxX = allowOutside ? (selection.Width - 1) * 2 : selection.Width - 1;
            decimal maxY = allowOutside ? (selection.Height - 1) * 2 : selection.Height - 1;

            SetNumericRange(numericUpDownTopLeftX, minX, maxX);
            SetNumericRange(numericUpDownTopRightX, minX, maxX);
            SetNumericRange(numericUpDownBottomRightX, minX, maxX);
            SetNumericRange(numericUpDownBottomLeftX, minX, maxX);

            SetNumericRange(numericUpDownTopLeftY, minY, maxY);
            SetNumericRange(numericUpDownTopRightY, minY, maxY);
            SetNumericRange(numericUpDownBottomRightY, minY, maxY);
            SetNumericRange(numericUpDownBottomLeftY, minY, maxY);
        }

        private static void SetNumericRange(NumericUpDown numericUpDown, decimal minimum, decimal maximum)
        {
            if (numericUpDown.Maximum < maximum)
                numericUpDown.Maximum = maximum;

            if (numericUpDown.Minimum > minimum)
                numericUpDown.Minimum = minimum;

            if (numericUpDown.Value < minimum)
                numericUpDown.Value = minimum;
            else if (numericUpDown.Value > maximum)
                numericUpDown.Value = maximum;

            numericUpDown.Minimum = minimum;
            numericUpDown.Maximum = maximum;
        }

        #region Values-Changed events
        private Point ScalePointToUi(decimal x, decimal y)
        {
            int uiWidth = Math.Max(1, quadControl11.ClientSize.Width - 1);
            int uiHeight = Math.Max(1, quadControl11.ClientSize.Height - 1);

            return new Point
            {
                X = (int)Math.Round(x * uiWidth / Math.Max(1, selection.Width - 1)),
                Y = (int)Math.Round(y * uiHeight / Math.Max(1, selection.Height - 1))
            };
        }

        private void numericUpDownTopLeft_ValueChanged(object sender, EventArgs e)
        {
            quadControl11.ValueChanged -= quadControl11_ValueChanged;

            quadControl11.NubTL = ScalePointToUi(numericUpDownTopLeftX.Value, numericUpDownTopLeftY.Value);

            quadControl11.ValueChanged += quadControl11_ValueChanged;

            UpdateTokenFromDialog();
        }

        private void numericUpDownTopRight_ValueChanged(object sender, EventArgs e)
        {
            quadControl11.ValueChanged -= quadControl11_ValueChanged;

            quadControl11.NubTR = ScalePointToUi(numericUpDownTopRightX.Value, numericUpDownTopRightY.Value);

            quadControl11.ValueChanged += quadControl11_ValueChanged;

            UpdateTokenFromDialog();
        }

        private void numericUpDownBottomRight_ValueChanged(object sender, EventArgs e)
        {
            quadControl11.ValueChanged -= quadControl11_ValueChanged;

            quadControl11.NubBR = ScalePointToUi(numericUpDownBottomRightX.Value, numericUpDownBottomRightY.Value);

            quadControl11.ValueChanged += quadControl11_ValueChanged;

            UpdateTokenFromDialog();
        }

        private void numericUpDownBottomLeft_ValueChanged(object sender, EventArgs e)
        {
            quadControl11.ValueChanged -= quadControl11_ValueChanged;

            quadControl11.NubBL = ScalePointToUi(numericUpDownBottomLeftX.Value, numericUpDownBottomLeftY.Value);

            quadControl11.ValueChanged += quadControl11_ValueChanged;

            UpdateTokenFromDialog();
        }

        private void splitContainerMain_Panel1_Resize(object sender, EventArgs e)
        {
            ApplyQuadControlImageBounds();

            quadControl11.ValueChanged -= quadControl11_ValueChanged;

            quadControl11.NubTL = ScalePointToUi(numericUpDownTopLeftX.Value, numericUpDownTopLeftY.Value);
            quadControl11.NubTR = ScalePointToUi(numericUpDownTopRightX.Value, numericUpDownTopRightY.Value);
            quadControl11.NubBR = ScalePointToUi(numericUpDownBottomRightX.Value, numericUpDownBottomRightY.Value);
            quadControl11.NubBL = ScalePointToUi(numericUpDownBottomLeftX.Value, numericUpDownBottomLeftY.Value);

            quadControl11.ValueChanged += quadControl11_ValueChanged;

            quadControl11.Invalidate();
        }

        private decimal ScaleXFromUi(int x)
        {
            int uiWidth = Math.Max(1, quadControl11.ClientSize.Width - 1);

            return Clamp(
                (decimal)x * (selection.Width - 1) / uiWidth,
                numericUpDownTopLeftX.Minimum,
                numericUpDownTopLeftX.Maximum);
        }

        private decimal ScaleYFromUi(int y)
        {
            int uiHeight = Math.Max(1, quadControl11.ClientSize.Height - 1);

            return Clamp(
                (decimal)y * (selection.Height - 1) / uiHeight,
                numericUpDownTopLeftY.Minimum,
                numericUpDownTopLeftY.Maximum);
        }

        private void quadControl11_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownTopLeftX.ValueChanged -= numericUpDownTopLeft_ValueChanged;
            numericUpDownTopLeftY.ValueChanged -= numericUpDownTopLeft_ValueChanged;
            numericUpDownTopRightX.ValueChanged -= numericUpDownTopRight_ValueChanged;
            numericUpDownTopRightY.ValueChanged -= numericUpDownTopRight_ValueChanged;
            numericUpDownBottomRightX.ValueChanged -= numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomRightY.ValueChanged -= numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomLeftX.ValueChanged -= numericUpDownBottomLeft_ValueChanged;
            numericUpDownBottomLeftY.ValueChanged -= numericUpDownBottomLeft_ValueChanged;

            numericUpDownTopLeftX.Value = ScaleXFromUi(quadControl11.NubTL.X);
            numericUpDownTopLeftY.Value = ScaleYFromUi(quadControl11.NubTL.Y);

            numericUpDownTopRightX.Value = ScaleXFromUi(quadControl11.NubTR.X);
            numericUpDownTopRightY.Value = ScaleYFromUi(quadControl11.NubTR.Y);

            numericUpDownBottomRightX.Value = ScaleXFromUi(quadControl11.NubBR.X);
            numericUpDownBottomRightY.Value = ScaleYFromUi(quadControl11.NubBR.Y);

            numericUpDownBottomLeftX.Value = ScaleXFromUi(quadControl11.NubBL.X);
            numericUpDownBottomLeftY.Value = ScaleYFromUi(quadControl11.NubBL.Y);

            numericUpDownTopLeftX.ValueChanged += numericUpDownTopLeft_ValueChanged;
            numericUpDownTopLeftY.ValueChanged += numericUpDownTopLeft_ValueChanged;
            numericUpDownTopRightX.ValueChanged += numericUpDownTopRight_ValueChanged;
            numericUpDownTopRightY.ValueChanged += numericUpDownTopRight_ValueChanged;
            numericUpDownBottomRightX.ValueChanged += numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomRightY.ValueChanged += numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomLeftX.ValueChanged += numericUpDownBottomLeft_ValueChanged;
            numericUpDownBottomLeftY.ValueChanged += numericUpDownBottomLeft_ValueChanged;

            UpdateTokenFromDialog();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoDims.Checked)
            {
                numericUpDownWidth.Enabled = false;
                numericUpDownHeight.Enabled = false;

                numericUpDownWidth.Text = "-";
                numericUpDownHeight.Text = "-";
            }
            else
            {
                numericUpDownWidth.Enabled = true;
                numericUpDownHeight.Enabled = true;
                SetDimensionValues();
            }

            UpdateTokenFromDialog();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UpdateTokenFromDialog();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            UpdateTokenFromDialog();
        }

        private void ComboBoxResampling_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (updatingDialogFromToken)
                return;

            UpdateTokenFromDialog();
        }

        private void checkBoxCenter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTokenFromDialog();
        }

        private void ComboBoxCropMode_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            UpdateTokenFromDialog();

        }

        private void CheckBoxMoveNearestNub_CheckedChanged(object sender, System.EventArgs e)
        {
            quadControl11.MoveNearestNubOnClick = checkBoxMoveNearestNub.Checked;
        }

        private void CheckBoxLineSnap_CheckedChanged(object sender, System.EventArgs e)
        {
            quadControl11.LineSnapEnabled = checkBoxLineSnap.Checked;
        }

        private void CheckBoxAllowNubsOutsideImage_CheckedChanged(object sender, EventArgs e)
        {
            bool allowOutside = checkBoxAllowNubsOutsideImage.Checked;

            quadControl11.AllowNubsOutsideImage = allowOutside;
            ApplyNubNumericRanges(allowOutside);

            quadControl11.NubTL = ScalePointToUi(numericUpDownTopLeftX.Value, numericUpDownTopLeftY.Value);
            quadControl11.NubTR = ScalePointToUi(numericUpDownTopRightX.Value, numericUpDownTopRightY.Value);
            quadControl11.NubBR = ScalePointToUi(numericUpDownBottomRightX.Value, numericUpDownBottomRightY.Value);
            quadControl11.NubBL = ScalePointToUi(numericUpDownBottomLeftX.Value, numericUpDownBottomLeftY.Value);

            quadControl11.Invalidate();

            UpdateTokenFromDialog();
        }
        #endregion

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            if (sender is NumericUpDown upDown)
            {
                upDown.Select(0, upDown.Text.Length);
            }
        }

        #region Token Stuff
        protected override QuadrilateralCorrectionConfigToken OnCreateInitialToken()
        {
            return new QuadrilateralCorrectionConfigToken();
        }

        protected override void OnUpdateDialogFromToken(QuadrilateralCorrectionConfigToken effectTokenCopy)
        {
            updatingDialogFromToken = true;

            try
            {
                Initializers();
                ApplyQuadControlImageBounds();

                checkBoxAllowNubsOutsideImage.Checked = effectTokenCopy.AllowNubsOutsideImage;
                quadControl11.AllowNubsOutsideImage = effectTokenCopy.AllowNubsOutsideImage;
                ApplyNubNumericRanges(effectTokenCopy.AllowNubsOutsideImage);

                numericUpDownTopLeftX.Value = Clamp(effectTokenCopy.TopLeft.X, numericUpDownTopLeftX.Minimum, numericUpDownTopLeftX.Maximum);
                numericUpDownTopLeftY.Value = Clamp(effectTokenCopy.TopLeft.Y, numericUpDownTopLeftY.Minimum, numericUpDownTopLeftY.Maximum);
                numericUpDownTopRightX.Value = Clamp(effectTokenCopy.TopRight.X, numericUpDownTopRightX.Minimum, numericUpDownTopRightX.Maximum);
                numericUpDownTopRightY.Value = Clamp(effectTokenCopy.TopRight.Y, numericUpDownTopRightY.Minimum, numericUpDownTopRightY.Maximum);
                numericUpDownBottomRightX.Value = Clamp(effectTokenCopy.BottomRight.X, numericUpDownBottomRightX.Minimum, numericUpDownBottomRightX.Maximum);
                numericUpDownBottomRightY.Value = Clamp(effectTokenCopy.BottomRight.Y, numericUpDownBottomRightY.Minimum, numericUpDownBottomRightY.Maximum);
                numericUpDownBottomLeftX.Value = Clamp(effectTokenCopy.BottomLeft.X, numericUpDownBottomLeftX.Minimum, numericUpDownBottomLeftX.Maximum);
                numericUpDownBottomLeftY.Value = Clamp(effectTokenCopy.BottomLeft.Y, numericUpDownBottomLeftY.Minimum, numericUpDownBottomLeftY.Maximum);

                checkBoxAutoDims.Checked = effectTokenCopy.AutoDims;
                numericUpDownWidth.Value = Clamp(effectTokenCopy.Width, numericUpDownWidth.Minimum, numericUpDownWidth.Maximum);
                numericUpDownHeight.Value = Clamp(effectTokenCopy.Height, numericUpDownHeight.Minimum, numericUpDownHeight.Maximum);
                if (checkBoxAutoDims.Checked)
                {
                    numericUpDownWidth.Text = "-";
                    numericUpDownHeight.Text = "-";
                }

                int resamplingIndex = FindResamplingModeIndex(comboBoxResampling, effectTokenCopy.ResamplingMode);
                if (resamplingIndex < 0)
                    resamplingIndex = FindResamplingModeIndex(comboBoxResampling, ResamplingMode.Bilinear);

                int cropModeIndex = (int)effectTokenCopy.CropOutsideMode;
                if (cropModeIndex < 0 || cropModeIndex >= comboBoxCropMode.Items.Count)
                    cropModeIndex = 0;

                comboBoxResampling.SelectedIndex = resamplingIndex;
                comboBoxCropMode.SelectedIndex = cropModeIndex;
                checkBoxCenter.Checked = effectTokenCopy.Center;
            }
            finally
            {
                updatingDialogFromToken = false;
            }

            quadControl11.Invalidate();
        }

        protected override void OnUpdateTokenFromDialog(QuadrilateralCorrectionConfigToken writeValuesHere)
        {
            writeValuesHere.TopLeft = new Point((int)numericUpDownTopLeftX.Value, (int)numericUpDownTopLeftY.Value);
            writeValuesHere.TopRight = new Point((int)numericUpDownTopRightX.Value, (int)numericUpDownTopRightY.Value);
            writeValuesHere.BottomRight = new Point((int)numericUpDownBottomRightX.Value, (int)numericUpDownBottomRightY.Value);
            writeValuesHere.BottomLeft = new Point((int)numericUpDownBottomLeftX.Value, (int)numericUpDownBottomLeftY.Value);

            writeValuesHere.AutoDims = checkBoxAutoDims.Checked;
            writeValuesHere.Width = (int)numericUpDownWidth.Value;
            writeValuesHere.Height = (int)numericUpDownHeight.Value;
            writeValuesHere.ResamplingMode = TryParseResamplingModeFromText(comboBoxResampling.Text, out ResamplingMode resamplingMode)
                ? resamplingMode : ResamplingMode.Bilinear;

            writeValuesHere.CropOutsideMode = comboBoxCropMode.SelectedIndex >= 0
                ? (CropOutsideMode)comboBoxCropMode.SelectedIndex : CropOutsideMode.Crop;
            writeValuesHere.Center = checkBoxCenter.Checked;
            writeValuesHere.AllowNubsOutsideImage = checkBoxAllowNubsOutsideImage.Checked;
        }
        #endregion

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private static bool TryParseResamplingModeFromText(string text, out ResamplingMode resamplingMode)
        {
            resamplingMode = ResamplingMode.Bilinear;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            char[] chars = new char[text.Length];
            int length = 0;

            foreach (char c in text)
            {
                if (!char.IsWhiteSpace(c))
                    chars[length++] = c;
            }

            string normalizedText = new string(chars, 0, length);

            return Enum.TryParse(normalizedText, true, out resamplingMode) && Enum.IsDefined(resamplingMode);
        }

        private static int FindResamplingModeIndex(ComboBox comboBox, ResamplingMode resamplingMode)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (TryParseResamplingModeFromText(comboBox.Items[i]?.ToString(), out ResamplingMode parsedMode)
                    && parsedMode == resamplingMode)
                {
                    return i;
                }
            }

            return -1;
        }

        private void SetDimensionValues()
        {
            Size quadTransOutput;
            try
            {
                BitmapRegionUtil.BitmapBgra32Data outputBitmap = PerspectiveWarpUtil.PerspectiveWarp(
                    srcImageData,
                    new Point((int)numericUpDownTopLeftX.Value, (int)numericUpDownTopLeftY.Value),
                    new Point((int)numericUpDownTopRightX.Value, (int)numericUpDownTopRightY.Value),
                    new Point((int)numericUpDownBottomRightX.Value, (int)numericUpDownBottomRightY.Value),
                    new Point((int)numericUpDownBottomLeftX.Value, (int)numericUpDownBottomLeftY.Value),
                    true,
                    int.MaxValue,
                    int.MaxValue,
                    ResamplingMode.Bilinear,
                    CropOutsideMode.Crop,
                    out _,
                    out _);
                quadTransOutput = new Size(outputBitmap.Width, outputBitmap.Height);
            }
            catch
            {
                quadTransOutput = Size.Empty;
            }
            numericUpDownWidth.Value = Clamp(quadTransOutput.Width, numericUpDownWidth.Minimum, numericUpDownWidth.Maximum);
            numericUpDownHeight.Value = Clamp(quadTransOutput.Height, numericUpDownHeight.Minimum, numericUpDownHeight.Maximum);
            numericUpDownWidth.Text = numericUpDownWidth.Value.ToString();
            numericUpDownHeight.Text = numericUpDownHeight.Value.ToString();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (quadControl11.SelectedNub == Nub.None)
            {
                base.OnKeyDown(e);
                return;
            }

            e.Handled = true;

            int horAmount = 0;
            int verAmount = 0;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    verAmount = (e.Modifiers == Keys.Control) ? -5 : -1;
                    break;
                case Keys.Right:
                    horAmount = (e.Modifiers == Keys.Control) ? 5 : 1;
                    break;
                case Keys.Down:
                    verAmount = (e.Modifiers == Keys.Control) ? 5 : 1;
                    break;
                case Keys.Left:
                    horAmount = (e.Modifiers == Keys.Control) ? -5 : -1;
                    break;
                default:
                    base.OnKeyDown(e);
                    return;
            }

            switch (quadControl11.SelectedNub)
            {
                case Nub.TopLeft:
                    numericUpDownTopLeftX.Value = Clamp(numericUpDownTopLeftX.Value + horAmount, numericUpDownTopLeftX.Minimum, numericUpDownTopLeftX.Maximum);
                    numericUpDownTopLeftY.Value = Clamp(numericUpDownTopLeftY.Value + verAmount, numericUpDownTopLeftY.Minimum, numericUpDownTopLeftY.Maximum);
                    break;
                case Nub.TopRight:
                    numericUpDownTopRightX.Value = Clamp(numericUpDownTopRightX.Value + horAmount, numericUpDownTopRightX.Minimum, numericUpDownTopRightX.Maximum);
                    numericUpDownTopRightY.Value = Clamp(numericUpDownTopRightY.Value + verAmount, numericUpDownTopRightY.Minimum, numericUpDownTopRightY.Maximum);
                    break;
                case Nub.BottomRight:
                    numericUpDownBottomRightX.Value = Clamp(numericUpDownBottomRightX.Value + horAmount, numericUpDownBottomRightX.Minimum, numericUpDownBottomRightX.Maximum);
                    numericUpDownBottomRightY.Value = Clamp(numericUpDownBottomRightY.Value + verAmount, numericUpDownBottomRightY.Minimum, numericUpDownBottomRightY.Maximum);
                    break;
                case Nub.BottomLeft:
                    numericUpDownBottomLeftX.Value = Clamp(numericUpDownBottomLeftX.Value + horAmount, numericUpDownBottomLeftX.Minimum, numericUpDownBottomLeftX.Maximum);
                    numericUpDownBottomLeftY.Value = Clamp(numericUpDownBottomLeftY.Value + verAmount, numericUpDownBottomLeftY.Minimum, numericUpDownBottomLeftY.Maximum);
                    break;
            }

            base.OnKeyDown(e);
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnHelpButtonClicked(e);

            const string helpMessage = "The control nubs can be manipulated with the mouse in the following ways:\r\n"
            + "\r\n"
            + "Left Mouse Button — Grab and Drag\r\n"
            + "\r\n"
            + "Middle Mouse Button — Grab and Drag with a Dead Zone\r\n"
            + "\r\n"
            + "Right Mouse Button — Select nub for Keyboard Arrow manipulation\r\n"
            + "    Arrow — 1px\r\n"
            + "    Ctrl + Arrow — 5px\r\n"
            + "\r\n"
            + "When enabled, Move Nearest Nub moves the nub nearest to the mouse pointer.\r\n"
            + "\r\n"
            + "When enabled, Line Snap snaps the nub to the nearest detected line while dragging.\r\n"
            + "\r\n"
            + "Magnifier\r\n"
            + "    + — Increase magnification\r\n"
            + "    - — Decrease magnification\r\n"
            + "    Range — 2x to 12x\r\n";

            MessageBox.Show(helpMessage, "Help");
        }

        private void resetAllButton_Click(object sender, EventArgs e)
        {
            numericUpDownTopLeftX.Value = numericUpDownTopLeftX.Minimum;
            numericUpDownTopLeftY.Value = numericUpDownTopLeftY.Minimum;
            numericUpDownTopRightX.Value = numericUpDownTopRightX.Maximum;
            numericUpDownTopRightY.Value = numericUpDownTopRightY.Minimum;
            numericUpDownBottomRightX.Value = numericUpDownBottomRightX.Maximum;
            numericUpDownBottomRightY.Value = numericUpDownBottomRightY.Maximum;
            numericUpDownBottomLeftX.Value = numericUpDownBottomLeftX.Minimum;
            numericUpDownBottomLeftY.Value = numericUpDownBottomLeftY.Maximum;
        }
    }

    /// <summary>
    /// This non-generic intermediate base class allows the Visual Studio WinForms
    /// Designer to load QuadrilateralCorrectionConfigDialog without directly
    /// instantiating the generic Paint.NET EffectConfigForm base type.
    /// OnCreateInitialToken() must be overridden here because EffectConfigForm
    /// calls it from its base constructor.
    /// </summary>
    internal class QuadrilateralCorrectionConfigDialogBase : EffectConfigForm<QuadrilateralCorrectionEffectPlugin, QuadrilateralCorrectionConfigToken>
    {
        public QuadrilateralCorrectionConfigDialogBase() { }

        protected override QuadrilateralCorrectionConfigToken OnCreateInitialToken() => new QuadrilateralCorrectionConfigToken();
    }
}
