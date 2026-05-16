namespace QuadrilateralCorrectionEffect
{
    partial class QuadrilateralCorrectionConfigDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void OnDispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.OnDispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            numericUpDownTopLeftX = new System.Windows.Forms.NumericUpDown();
            numericUpDownTopLeftY = new System.Windows.Forms.NumericUpDown();
            numericUpDownTopRightX = new System.Windows.Forms.NumericUpDown();
            numericUpDownTopRightY = new System.Windows.Forms.NumericUpDown();
            numericUpDownBottomRightX = new System.Windows.Forms.NumericUpDown();
            numericUpDownBottomRightY = new System.Windows.Forms.NumericUpDown();
            numericUpDownBottomLeftX = new System.Windows.Forms.NumericUpDown();
            numericUpDownBottomLeftY = new System.Windows.Forms.NumericUpDown();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            quadControl11 = new QuadControl();
            numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            checkBoxAutoDims = new System.Windows.Forms.CheckBox();
            labelTopLeftX = new System.Windows.Forms.Label();
            labelTopLeftY = new System.Windows.Forms.Label();
            labelTopRightX = new System.Windows.Forms.Label();
            labelTopRightY = new System.Windows.Forms.Label();
            labelBottomRightX = new System.Windows.Forms.Label();
            labelBottomRightY = new System.Windows.Forms.Label();
            labelBottomLeftX = new System.Windows.Forms.Label();
            labelBottomLeftY = new System.Windows.Forms.Label();
            labelWidth = new System.Windows.Forms.Label();
            labelHeight = new System.Windows.Forms.Label();
            labelResampling = new System.Windows.Forms.Label();
            comboBoxResampling = new System.Windows.Forms.ComboBox();
            labelCropMode = new System.Windows.Forms.Label();
            comboBoxCropMode = new System.Windows.Forms.ComboBox();
            checkBoxCenter = new System.Windows.Forms.CheckBox();
            panelDivider = new System.Windows.Forms.Panel();
            resetAllButton = new System.Windows.Forms.Button();
            checkBoxMoveNearestNub = new System.Windows.Forms.CheckBox();
            checkBoxLineSnap = new System.Windows.Forms.CheckBox();
            splitContainerMain = new System.Windows.Forms.SplitContainer();
            rightRootTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            rightTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopLeftX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopLeftY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopRightX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopRightY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomRightX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomRightY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomLeftX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomLeftY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            rightRootTableLayoutPanel.SuspendLayout();
            rightTableLayoutPanel.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // numericUpDownTopLeftX
            // 
            numericUpDownTopLeftX.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownTopLeftX.Location = new System.Drawing.Point(112, 3);
            numericUpDownTopLeftX.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownTopLeftX.Name = "numericUpDownTopLeftX";
            numericUpDownTopLeftX.Size = new System.Drawing.Size(67, 23);
            numericUpDownTopLeftX.TabIndex = 0;
            numericUpDownTopLeftX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownTopLeftX.ValueChanged += numericUpDownTopLeft_ValueChanged;
            numericUpDownTopLeftX.Enter += numericUpDown_Enter;
            // 
            // numericUpDownTopLeftY
            // 
            numericUpDownTopLeftY.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownTopLeftY.Location = new System.Drawing.Point(112, 32);
            numericUpDownTopLeftY.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownTopLeftY.Name = "numericUpDownTopLeftY";
            numericUpDownTopLeftY.Size = new System.Drawing.Size(67, 23);
            numericUpDownTopLeftY.TabIndex = 1;
            numericUpDownTopLeftY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownTopLeftY.ValueChanged += numericUpDownTopLeft_ValueChanged;
            numericUpDownTopLeftY.Enter += numericUpDown_Enter;
            // 
            // numericUpDownTopRightX
            // 
            numericUpDownTopRightX.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownTopRightX.Location = new System.Drawing.Point(112, 71);
            numericUpDownTopRightX.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownTopRightX.Name = "numericUpDownTopRightX";
            numericUpDownTopRightX.Size = new System.Drawing.Size(67, 23);
            numericUpDownTopRightX.TabIndex = 2;
            numericUpDownTopRightX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownTopRightX.ValueChanged += numericUpDownTopRight_ValueChanged;
            numericUpDownTopRightX.Enter += numericUpDown_Enter;
            // 
            // numericUpDownTopRightY
            // 
            numericUpDownTopRightY.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownTopRightY.Location = new System.Drawing.Point(112, 100);
            numericUpDownTopRightY.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownTopRightY.Name = "numericUpDownTopRightY";
            numericUpDownTopRightY.Size = new System.Drawing.Size(67, 23);
            numericUpDownTopRightY.TabIndex = 3;
            numericUpDownTopRightY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownTopRightY.ValueChanged += numericUpDownTopRight_ValueChanged;
            numericUpDownTopRightY.Enter += numericUpDown_Enter;
            // 
            // numericUpDownBottomRightX
            // 
            numericUpDownBottomRightX.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownBottomRightX.Location = new System.Drawing.Point(112, 139);
            numericUpDownBottomRightX.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownBottomRightX.Name = "numericUpDownBottomRightX";
            numericUpDownBottomRightX.Size = new System.Drawing.Size(67, 23);
            numericUpDownBottomRightX.TabIndex = 4;
            numericUpDownBottomRightX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownBottomRightX.ValueChanged += numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomRightX.Enter += numericUpDown_Enter;
            // 
            // numericUpDownBottomRightY
            // 
            numericUpDownBottomRightY.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownBottomRightY.Location = new System.Drawing.Point(112, 168);
            numericUpDownBottomRightY.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownBottomRightY.Name = "numericUpDownBottomRightY";
            numericUpDownBottomRightY.Size = new System.Drawing.Size(67, 23);
            numericUpDownBottomRightY.TabIndex = 5;
            numericUpDownBottomRightY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownBottomRightY.ValueChanged += numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomRightY.Enter += numericUpDown_Enter;
            // 
            // numericUpDownBottomLeftX
            // 
            numericUpDownBottomLeftX.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownBottomLeftX.Location = new System.Drawing.Point(112, 207);
            numericUpDownBottomLeftX.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownBottomLeftX.Name = "numericUpDownBottomLeftX";
            numericUpDownBottomLeftX.Size = new System.Drawing.Size(67, 23);
            numericUpDownBottomLeftX.TabIndex = 6;
            numericUpDownBottomLeftX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownBottomLeftX.ValueChanged += numericUpDownBottomLeft_ValueChanged;
            numericUpDownBottomLeftX.Enter += numericUpDown_Enter;
            // 
            // numericUpDownBottomLeftY
            // 
            numericUpDownBottomLeftY.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownBottomLeftY.Location = new System.Drawing.Point(112, 236);
            numericUpDownBottomLeftY.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownBottomLeftY.Name = "numericUpDownBottomLeftY";
            numericUpDownBottomLeftY.Size = new System.Drawing.Size(67, 23);
            numericUpDownBottomLeftY.TabIndex = 7;
            numericUpDownBottomLeftY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownBottomLeftY.ValueChanged += numericUpDownBottomLeft_ValueChanged;
            numericUpDownBottomLeftY.Enter += numericUpDown_Enter;
            // 
            // buttonOK
            // 
            buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            buttonOK.Location = new System.Drawing.Point(3, 27);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(76, 23);
            buttonOK.TabIndex = 12;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            buttonCancel.Location = new System.Drawing.Point(94, 27);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(76, 23);
            buttonCancel.TabIndex = 13;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // quadControl11
            // 
            quadControl11.Anchor = System.Windows.Forms.AnchorStyles.None;
            quadControl11.Location = new System.Drawing.Point(1, 35);
            quadControl11.Margin = new System.Windows.Forms.Padding(0);
            quadControl11.Name = "quadControl11";
            quadControl11.Size = new System.Drawing.Size(506, 520);
            quadControl11.SizeMode = PaintDotNet.Controls.Direct2DPictureBoxSizeMode.StretchBitmap;
            quadControl11.TabIndex = 14;
            quadControl11.TabStop = false;
            quadControl11.ValueChanged += quadControl11_ValueChanged;
            // 
            // numericUpDownWidth
            // 
            numericUpDownWidth.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownWidth.Location = new System.Drawing.Point(112, 329);
            numericUpDownWidth.Name = "numericUpDownWidth";
            numericUpDownWidth.Size = new System.Drawing.Size(67, 23);
            numericUpDownWidth.TabIndex = 9;
            numericUpDownWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownWidth.ValueChanged += numericUpDown1_ValueChanged;
            numericUpDownWidth.Enter += numericUpDown_Enter;
            // 
            // numericUpDownHeight
            // 
            numericUpDownHeight.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownHeight.Location = new System.Drawing.Point(112, 358);
            numericUpDownHeight.Name = "numericUpDownHeight";
            numericUpDownHeight.Size = new System.Drawing.Size(67, 23);
            numericUpDownHeight.TabIndex = 10;
            numericUpDownHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownHeight.ValueChanged += numericUpDown2_ValueChanged;
            numericUpDownHeight.Enter += numericUpDown_Enter;
            // 
            // checkBoxAutoDims
            // 
            checkBoxAutoDims.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            checkBoxAutoDims.AutoSize = true;
            rightTableLayoutPanel.SetColumnSpan(checkBoxAutoDims, 2);
            checkBoxAutoDims.Location = new System.Drawing.Point(3, 304);
            checkBoxAutoDims.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            checkBoxAutoDims.Name = "checkBoxAutoDims";
            checkBoxAutoDims.Size = new System.Drawing.Size(179, 19);
            checkBoxAutoDims.TabIndex = 8;
            checkBoxAutoDims.Text = "Auto Dimensions";
            checkBoxAutoDims.UseVisualStyleBackColor = true;
            checkBoxAutoDims.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // labelTopLeftX
            // 
            labelTopLeftX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelTopLeftX.AutoSize = true;
            labelTopLeftX.Location = new System.Drawing.Point(3, 7);
            labelTopLeftX.Name = "labelTopLeftX";
            labelTopLeftX.Size = new System.Drawing.Size(65, 15);
            labelTopLeftX.TabIndex = 15;
            labelTopLeftX.Text = "Top Left X";
            // 
            // labelTopLeftY
            // 
            labelTopLeftY.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelTopLeftY.AutoSize = true;
            labelTopLeftY.Location = new System.Drawing.Point(3, 36);
            labelTopLeftY.Name = "labelTopLeftY";
            labelTopLeftY.Size = new System.Drawing.Size(64, 15);
            labelTopLeftY.TabIndex = 16;
            labelTopLeftY.Text = "Top Left Y";
            // 
            // labelTopRightX
            // 
            labelTopRightX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelTopRightX.AutoSize = true;
            labelTopRightX.Location = new System.Drawing.Point(3, 75);
            labelTopRightX.Name = "labelTopRightX";
            labelTopRightX.Size = new System.Drawing.Size(74, 15);
            labelTopRightX.TabIndex = 17;
            labelTopRightX.Text = "Top Right X";
            // 
            // labelTopRightY
            // 
            labelTopRightY.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelTopRightY.AutoSize = true;
            labelTopRightY.Location = new System.Drawing.Point(3, 104);
            labelTopRightY.Name = "labelTopRightY";
            labelTopRightY.Size = new System.Drawing.Size(73, 15);
            labelTopRightY.TabIndex = 18;
            labelTopRightY.Text = "Top Right Y";
            // 
            // labelBottomRightX
            // 
            labelBottomRightX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelBottomRightX.AutoSize = true;
            labelBottomRightX.Location = new System.Drawing.Point(3, 143);
            labelBottomRightX.Name = "labelBottomRightX";
            labelBottomRightX.Size = new System.Drawing.Size(93, 15);
            labelBottomRightX.TabIndex = 19;
            labelBottomRightX.Text = "Bottom Right X";
            // 
            // labelBottomRightY
            // 
            labelBottomRightY.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelBottomRightY.AutoSize = true;
            labelBottomRightY.Location = new System.Drawing.Point(3, 172);
            labelBottomRightY.Name = "labelBottomRightY";
            labelBottomRightY.Size = new System.Drawing.Size(92, 15);
            labelBottomRightY.TabIndex = 20;
            labelBottomRightY.Text = "Bottom Right Y";
            // 
            // labelBottomLeftX
            // 
            labelBottomLeftX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelBottomLeftX.AutoSize = true;
            labelBottomLeftX.Location = new System.Drawing.Point(3, 211);
            labelBottomLeftX.Name = "labelBottomLeftX";
            labelBottomLeftX.Size = new System.Drawing.Size(84, 15);
            labelBottomLeftX.TabIndex = 21;
            labelBottomLeftX.Text = "Bottom Left X";
            // 
            // labelBottomLeftY
            // 
            labelBottomLeftY.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelBottomLeftY.AutoSize = true;
            labelBottomLeftY.Location = new System.Drawing.Point(3, 240);
            labelBottomLeftY.Name = "labelBottomLeftY";
            labelBottomLeftY.Size = new System.Drawing.Size(83, 15);
            labelBottomLeftY.TabIndex = 22;
            labelBottomLeftY.Text = "Bottom Left Y";
            // 
            // labelWidth
            // 
            labelWidth.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelWidth.AutoSize = true;
            labelWidth.Location = new System.Drawing.Point(3, 333);
            labelWidth.Name = "labelWidth";
            labelWidth.Size = new System.Drawing.Size(41, 15);
            labelWidth.TabIndex = 23;
            labelWidth.Text = "Width";
            // 
            // labelHeight
            // 
            labelHeight.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelHeight.AutoSize = true;
            labelHeight.Location = new System.Drawing.Point(3, 362);
            labelHeight.Name = "labelHeight";
            labelHeight.Size = new System.Drawing.Size(45, 15);
            labelHeight.TabIndex = 24;
            labelHeight.Text = "Height";
            // 
            // labelResampling
            // 
            labelResampling.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelResampling.AutoSize = true;
            labelResampling.Location = new System.Drawing.Point(3, 401);
            labelResampling.Name = "labelResampling";
            labelResampling.Size = new System.Drawing.Size(74, 15);
            labelResampling.TabIndex = 27;
            labelResampling.Text = "Resampling";
            // 
            // comboBoxResampling
            // 
            comboBoxResampling.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboBoxResampling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxResampling.DropDownWidth = 150;
            comboBoxResampling.Items.AddRange(new object[] { "Nearest Neighbor", "Bilinear", "Bicubic", "Lanczos 3", "High Quality Supersampling" });
            comboBoxResampling.Location = new System.Drawing.Point(112, 397);
            comboBoxResampling.Name = "comboBoxResampling";
            comboBoxResampling.Size = new System.Drawing.Size(67, 23);
            comboBoxResampling.TabIndex = 28;
            comboBoxResampling.SelectedIndexChanged += ComboBoxResampling_SelectedIndexChanged;
            // 
            // labelCropMode
            // 
            labelCropMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelCropMode.AutoSize = true;
            labelCropMode.Location = new System.Drawing.Point(3, 430);
            labelCropMode.Name = "labelCropMode";
            labelCropMode.Size = new System.Drawing.Size(73, 15);
            labelCropMode.TabIndex = 27;
            labelCropMode.Text = "Crop Mode";
            // 
            // comboBoxCropMode
            // 
            comboBoxCropMode.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboBoxCropMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxCropMode.DropDownWidth = 150;
            comboBoxCropMode.Items.AddRange(new object[] { "Crop", "Do not crop - Preserve", "Do not crop - Repeat", "Do not crop - Mirror" });
            comboBoxCropMode.Location = new System.Drawing.Point(112, 426);
            comboBoxCropMode.Name = "comboBoxCropMode";
            comboBoxCropMode.Size = new System.Drawing.Size(67, 23);
            comboBoxCropMode.TabIndex = 27;
            comboBoxCropMode.SelectedIndexChanged += ComboBoxCropMode_SelectedIndexChanged;
            // 
            // checkBoxCenter
            // 
            checkBoxCenter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            checkBoxCenter.AutoSize = true;
            rightTableLayoutPanel.SetColumnSpan(checkBoxCenter, 2);
            checkBoxCenter.Location = new System.Drawing.Point(3, 455);
            checkBoxCenter.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            checkBoxCenter.Name = "checkBoxCenter";
            checkBoxCenter.Size = new System.Drawing.Size(179, 19);
            checkBoxCenter.TabIndex = 11;
            checkBoxCenter.Text = "Center";
            checkBoxCenter.UseVisualStyleBackColor = true;
            checkBoxCenter.CheckedChanged += checkBoxCenter_CheckedChanged;
            // 
            // panelDivider
            // 
            panelDivider.BackColor = System.Drawing.SystemColors.ControlDark;
            panelDivider.Dock = System.Windows.Forms.DockStyle.Fill;
            panelDivider.Location = new System.Drawing.Point(3, 6);
            panelDivider.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            panelDivider.Name = "panelDivider";
            panelDivider.Size = new System.Drawing.Size(1, 573);
            panelDivider.TabIndex = 25;
            // 
            // resetAllButton
            // 
            resetAllButton.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            resetAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            resetAllButton.Location = new System.Drawing.Point(112, 265);
            resetAllButton.Name = "resetAllButton";
            resetAllButton.Size = new System.Drawing.Size(67, 23);
            resetAllButton.TabIndex = 26;
            resetAllButton.Text = "Reset All";
            resetAllButton.UseVisualStyleBackColor = true;
            resetAllButton.Click += resetAllButton_Click;
            // 
            // checkBoxMoveNearestNub
            // 
            checkBoxMoveNearestNub.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            checkBoxMoveNearestNub.AutoSize = true;
            checkBoxMoveNearestNub.Checked = true;
            checkBoxMoveNearestNub.CheckState = System.Windows.Forms.CheckState.Checked;
            rightTableLayoutPanel.SetColumnSpan(checkBoxMoveNearestNub, 2);
            checkBoxMoveNearestNub.Location = new System.Drawing.Point(3, 480);
            checkBoxMoveNearestNub.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            checkBoxMoveNearestNub.Name = "checkBoxMoveNearestNub";
            checkBoxMoveNearestNub.Size = new System.Drawing.Size(179, 21);
            checkBoxMoveNearestNub.TabIndex = 28;
            checkBoxMoveNearestNub.Text = "Move nearest nub on click";
            checkBoxMoveNearestNub.UseCompatibleTextRendering = true;
            checkBoxMoveNearestNub.UseVisualStyleBackColor = true;
            checkBoxMoveNearestNub.CheckedChanged += CheckBoxMoveNearestNub_CheckedChanged;
            // 
            // checkBoxLineSnap
            // 
            checkBoxLineSnap.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            checkBoxLineSnap.AutoSize = true;
            checkBoxLineSnap.Checked = true;
            checkBoxLineSnap.CheckState = System.Windows.Forms.CheckState.Checked;
            rightTableLayoutPanel.SetColumnSpan(checkBoxLineSnap, 2);
            checkBoxLineSnap.Location = new System.Drawing.Point(3, 507);
            checkBoxLineSnap.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            checkBoxLineSnap.Name = "checkBoxLineSnap";
            checkBoxLineSnap.Size = new System.Drawing.Size(179, 21);
            checkBoxLineSnap.TabIndex = 29;
            checkBoxLineSnap.Text = "Snap nubs to lines";
            checkBoxLineSnap.UseCompatibleTextRendering = true;
            checkBoxLineSnap.UseVisualStyleBackColor = true;
            checkBoxLineSnap.CheckedChanged += CheckBoxLineSnap_CheckedChanged;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            splitContainerMain.IsSplitterFixed = true;
            splitContainerMain.Location = new System.Drawing.Point(0, 0);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(quadControl11);
            splitContainerMain.Panel1.Padding = new System.Windows.Forms.Padding(3);
            splitContainerMain.Panel1.Resize += splitContainerMain_Panel1_Resize;
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(rightRootTableLayoutPanel);
            splitContainerMain.Panel2.ForeColor = System.Drawing.Color.Transparent;
            splitContainerMain.Panel2.Padding = new System.Windows.Forms.Padding(3);
            splitContainerMain.Size = new System.Drawing.Size(704, 591);
            splitContainerMain.SplitterDistance = 510;
            splitContainerMain.SplitterWidth = 1;
            splitContainerMain.TabIndex = 29;
            // 
            // rightRootTableLayoutPanel
            // 
            rightRootTableLayoutPanel.ColumnCount = 2;
            rightRootTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            rightRootTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            rightRootTableLayoutPanel.Controls.Add(panelDivider, 0, 0);
            rightRootTableLayoutPanel.Controls.Add(rightTableLayoutPanel, 1, 0);
            rightRootTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            rightRootTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            rightRootTableLayoutPanel.Name = "rightRootTableLayoutPanel";
            rightRootTableLayoutPanel.RowCount = 1;
            rightRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            rightRootTableLayoutPanel.Size = new System.Drawing.Size(187, 585);
            rightRootTableLayoutPanel.TabIndex = 15;
            // 
            // rightTableLayoutPanel
            // 
            rightTableLayoutPanel.ColumnCount = 2;
            rightTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            rightTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            rightTableLayoutPanel.Controls.Add(labelTopLeftX, 0, 0);
            rightTableLayoutPanel.Controls.Add(numericUpDownTopLeftX, 1, 0);
            rightTableLayoutPanel.Controls.Add(labelTopLeftY, 0, 1);
            rightTableLayoutPanel.Controls.Add(numericUpDownTopLeftY, 1, 1);
            rightTableLayoutPanel.Controls.Add(labelTopRightX, 0, 3);
            rightTableLayoutPanel.Controls.Add(numericUpDownTopRightX, 1, 3);
            rightTableLayoutPanel.Controls.Add(labelTopRightY, 0, 4);
            rightTableLayoutPanel.Controls.Add(numericUpDownTopRightY, 1, 4);
            rightTableLayoutPanel.Controls.Add(labelBottomRightX, 0, 6);
            rightTableLayoutPanel.Controls.Add(numericUpDownBottomRightX, 1, 6);
            rightTableLayoutPanel.Controls.Add(labelBottomRightY, 0, 7);
            rightTableLayoutPanel.Controls.Add(numericUpDownBottomRightY, 1, 7);
            rightTableLayoutPanel.Controls.Add(labelBottomLeftX, 0, 9);
            rightTableLayoutPanel.Controls.Add(numericUpDownBottomLeftX, 1, 9);
            rightTableLayoutPanel.Controls.Add(labelBottomLeftY, 0, 10);
            rightTableLayoutPanel.Controls.Add(numericUpDownBottomLeftY, 1, 10);
            rightTableLayoutPanel.Controls.Add(resetAllButton, 1, 11);
            rightTableLayoutPanel.Controls.Add(checkBoxAutoDims, 0, 13);
            rightTableLayoutPanel.Controls.Add(labelWidth, 0, 14);
            rightTableLayoutPanel.Controls.Add(numericUpDownWidth, 1, 14);
            rightTableLayoutPanel.Controls.Add(labelHeight, 0, 15);
            rightTableLayoutPanel.Controls.Add(numericUpDownHeight, 1, 15);
            rightTableLayoutPanel.Controls.Add(labelResampling, 0, 17);
            rightTableLayoutPanel.Controls.Add(comboBoxResampling, 1, 17);
            rightTableLayoutPanel.Controls.Add(labelCropMode, 0, 18);
            rightTableLayoutPanel.Controls.Add(comboBoxCropMode, 1, 18);
            rightTableLayoutPanel.Controls.Add(checkBoxCenter, 0, 19);
            rightTableLayoutPanel.Controls.Add(checkBoxMoveNearestNub, 0, 20);
            rightTableLayoutPanel.Controls.Add(checkBoxLineSnap, 0, 21);
            rightTableLayoutPanel.Controls.Add(tableLayoutPanel1, 0, 22);
            rightTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            rightTableLayoutPanel.Location = new System.Drawing.Point(5, 0);
            rightTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            rightTableLayoutPanel.Name = "rightTableLayoutPanel";
            rightTableLayoutPanel.RowCount = 23;
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            rightTableLayoutPanel.Size = new System.Drawing.Size(182, 585);
            rightTableLayoutPanel.TabIndex = 15;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            rightTableLayoutPanel.SetColumnSpan(tableLayoutPanel1, 2);
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(buttonOK, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonCancel, 1, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 531);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(182, 58);
            tableLayoutPanel1.TabIndex = 15;
            // 
            // QuadrilateralCorrectionConfigDialog
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(704, 591);
            Controls.Add(splitContainerMain);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            HelpButton = true;
            KeyPreview = true;
            Location = new System.Drawing.Point(0, 0);
            Name = "QuadrilateralCorrectionConfigDialog";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            Text = "Quadrilateral Correction";
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopLeftX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopLeftY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopRightX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopRightY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomRightX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomRightY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomLeftX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomLeftY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownHeight).EndInit();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            rightRootTableLayoutPanel.ResumeLayout(false);
            rightTableLayoutPanel.ResumeLayout(false);
            rightTableLayoutPanel.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownTopLeftX;
        private System.Windows.Forms.NumericUpDown numericUpDownTopLeftY;
        private System.Windows.Forms.NumericUpDown numericUpDownTopRightX;
        private System.Windows.Forms.NumericUpDown numericUpDownTopRightY;
        private System.Windows.Forms.NumericUpDown numericUpDownBottomRightX;
        private System.Windows.Forms.NumericUpDown numericUpDownBottomRightY;
        private System.Windows.Forms.NumericUpDown numericUpDownBottomLeftX;
        private System.Windows.Forms.NumericUpDown numericUpDownBottomLeftY;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private QuadrilateralCorrectionEffect.QuadControl quadControl11;
        private System.Windows.Forms.NumericUpDown numericUpDownWidth;
        private System.Windows.Forms.NumericUpDown numericUpDownHeight;
        private System.Windows.Forms.CheckBox checkBoxAutoDims;
        private System.Windows.Forms.Label labelTopLeftX;
        private System.Windows.Forms.Label labelTopLeftY;
        private System.Windows.Forms.Label labelTopRightX;
        private System.Windows.Forms.Label labelTopRightY;
        private System.Windows.Forms.Label labelBottomRightX;
        private System.Windows.Forms.Label labelBottomRightY;
        private System.Windows.Forms.Label labelBottomLeftX;
        private System.Windows.Forms.Label labelBottomLeftY;
        private System.Windows.Forms.Label labelWidth;
        private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.Label labelResampling;
        private System.Windows.Forms.ComboBox comboBoxResampling;
        private System.Windows.Forms.Label labelCropMode;
        private System.Windows.Forms.ComboBox comboBoxCropMode;
        private System.Windows.Forms.CheckBox checkBoxCenter;
        private System.Windows.Forms.Panel panelDivider;
        private System.Windows.Forms.Button resetAllButton;
        private System.Windows.Forms.CheckBox checkBoxMoveNearestNub;
        private System.Windows.Forms.CheckBox checkBoxLineSnap;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TableLayoutPanel rightTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel rightRootTableLayoutPanel;
    }
}