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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuadrilateralCorrectionConfigDialog));
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
            numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            checkBoxAutoDims = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            checkBoxCenter = new System.Windows.Forms.CheckBox();
            panelDivider = new System.Windows.Forms.Panel();
            resetAllButton = new System.Windows.Forms.Button();
            cropOutsideCheckBox = new System.Windows.Forms.CheckBox();
            checkBoxMoveNearestNub = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopLeftX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopLeftY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopRightX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopRightY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomRightX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomRightY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomLeftX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomLeftY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)quadControl11).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            SuspendLayout();
            // 
            // numericUpDownTopLeftX
            // 
            numericUpDownTopLeftX.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownTopLeftX.Location = new System.Drawing.Point(632, 12);
            numericUpDownTopLeftX.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownTopLeftX.Name = "numericUpDownTopLeftX";
            numericUpDownTopLeftX.Size = new System.Drawing.Size(60, 23);
            numericUpDownTopLeftX.TabIndex = 0;
            numericUpDownTopLeftX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownTopLeftX.ValueChanged += numericUpDownTopLeft_ValueChanged;
            numericUpDownTopLeftX.Enter += numericUpDown_Enter;
            // 
            // numericUpDownTopLeftY
            // 
            numericUpDownTopLeftY.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownTopLeftY.Location = new System.Drawing.Point(632, 41);
            numericUpDownTopLeftY.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownTopLeftY.Name = "numericUpDownTopLeftY";
            numericUpDownTopLeftY.Size = new System.Drawing.Size(60, 23);
            numericUpDownTopLeftY.TabIndex = 1;
            numericUpDownTopLeftY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownTopLeftY.ValueChanged += numericUpDownTopLeft_ValueChanged;
            numericUpDownTopLeftY.Enter += numericUpDown_Enter;
            // 
            // numericUpDownTopRightX
            // 
            numericUpDownTopRightX.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownTopRightX.Location = new System.Drawing.Point(632, 84);
            numericUpDownTopRightX.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownTopRightX.Name = "numericUpDownTopRightX";
            numericUpDownTopRightX.Size = new System.Drawing.Size(60, 23);
            numericUpDownTopRightX.TabIndex = 2;
            numericUpDownTopRightX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownTopRightX.ValueChanged += numericUpDownTopRight_ValueChanged;
            numericUpDownTopRightX.Enter += numericUpDown_Enter;
            // 
            // numericUpDownTopRightY
            // 
            numericUpDownTopRightY.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownTopRightY.Location = new System.Drawing.Point(632, 113);
            numericUpDownTopRightY.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownTopRightY.Name = "numericUpDownTopRightY";
            numericUpDownTopRightY.Size = new System.Drawing.Size(60, 23);
            numericUpDownTopRightY.TabIndex = 3;
            numericUpDownTopRightY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownTopRightY.ValueChanged += numericUpDownTopRight_ValueChanged;
            numericUpDownTopRightY.Enter += numericUpDown_Enter;
            // 
            // numericUpDownBottomRightX
            // 
            numericUpDownBottomRightX.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownBottomRightX.Location = new System.Drawing.Point(632, 156);
            numericUpDownBottomRightX.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownBottomRightX.Name = "numericUpDownBottomRightX";
            numericUpDownBottomRightX.Size = new System.Drawing.Size(60, 23);
            numericUpDownBottomRightX.TabIndex = 4;
            numericUpDownBottomRightX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownBottomRightX.ValueChanged += numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomRightX.Enter += numericUpDown_Enter;
            // 
            // numericUpDownBottomRightY
            // 
            numericUpDownBottomRightY.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownBottomRightY.Location = new System.Drawing.Point(632, 185);
            numericUpDownBottomRightY.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownBottomRightY.Name = "numericUpDownBottomRightY";
            numericUpDownBottomRightY.Size = new System.Drawing.Size(60, 23);
            numericUpDownBottomRightY.TabIndex = 5;
            numericUpDownBottomRightY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownBottomRightY.ValueChanged += numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomRightY.Enter += numericUpDown_Enter;
            // 
            // numericUpDownBottomLeftX
            // 
            numericUpDownBottomLeftX.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownBottomLeftX.Location = new System.Drawing.Point(632, 228);
            numericUpDownBottomLeftX.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownBottomLeftX.Name = "numericUpDownBottomLeftX";
            numericUpDownBottomLeftX.Size = new System.Drawing.Size(60, 23);
            numericUpDownBottomLeftX.TabIndex = 6;
            numericUpDownBottomLeftX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownBottomLeftX.ValueChanged += numericUpDownBottomLeft_ValueChanged;
            numericUpDownBottomLeftX.Enter += numericUpDown_Enter;
            // 
            // numericUpDownBottomLeftY
            // 
            numericUpDownBottomLeftY.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownBottomLeftY.Location = new System.Drawing.Point(632, 257);
            numericUpDownBottomLeftY.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numericUpDownBottomLeftY.Name = "numericUpDownBottomLeftY";
            numericUpDownBottomLeftY.Size = new System.Drawing.Size(60, 23);
            numericUpDownBottomLeftY.TabIndex = 7;
            numericUpDownBottomLeftY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDownBottomLeftY.ValueChanged += numericUpDownBottomLeft_ValueChanged;
            numericUpDownBottomLeftY.Enter += numericUpDown_Enter;
            // 
            // buttonOK
            // 
            buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            buttonOK.Location = new System.Drawing.Point(536, 491);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(75, 23);
            buttonOK.TabIndex = 12;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            buttonCancel.Location = new System.Drawing.Point(617, 491);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 13;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // quadControl11
            // 
            quadControl11.BackgroundImage = (System.Drawing.Image)resources.GetObject("quadControl11.BackgroundImage");
            quadControl11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            quadControl11.Location = new System.Drawing.Point(12, 12);
            quadControl11.Name = "quadControl11";
            quadControl11.MoveNearestNubOnClick = true;
            quadControl11.Size = new System.Drawing.Size(502, 502);
            quadControl11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            quadControl11.TabIndex = 14;
            quadControl11.TabStop = false;
            quadControl11.ValueChanged += quadControl11_ValueChanged;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDown1.Location = new System.Drawing.Point(632, 348);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new System.Drawing.Size(60, 23);
            numericUpDown1.TabIndex = 9;
            numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            numericUpDown1.Enter += numericUpDown_Enter;
            // 
            // numericUpDown2
            // 
            numericUpDown2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDown2.Location = new System.Drawing.Point(632, 377);
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new System.Drawing.Size(60, 23);
            numericUpDown2.TabIndex = 10;
            numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            numericUpDown2.ValueChanged += numericUpDown2_ValueChanged;
            numericUpDown2.Enter += numericUpDown_Enter;
            // 
            // checkBoxAutoDims
            // 
            checkBoxAutoDims.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBoxAutoDims.AutoSize = true;
            checkBoxAutoDims.Location = new System.Drawing.Point(538, 314);
            checkBoxAutoDims.Name = "checkBoxAutoDims";
            checkBoxAutoDims.Size = new System.Drawing.Size(121, 19);
            checkBoxAutoDims.TabIndex = 8;
            checkBoxAutoDims.Text = "Auto Dimensions";
            checkBoxAutoDims.UseVisualStyleBackColor = true;
            checkBoxAutoDims.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(538, 14);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(65, 15);
            label1.TabIndex = 15;
            label1.Text = "Top Left X";
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(538, 43);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(64, 15);
            label2.TabIndex = 16;
            label2.Text = "Top Left Y";
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(538, 86);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(74, 15);
            label3.TabIndex = 17;
            label3.Text = "Top Right X";
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(538, 115);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(73, 15);
            label4.TabIndex = 18;
            label4.Text = "Top Right Y";
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(538, 158);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(93, 15);
            label5.TabIndex = 19;
            label5.Text = "Bottom Right X";
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(538, 187);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(92, 15);
            label6.TabIndex = 20;
            label6.Text = "Bottom Right Y";
            // 
            // label7
            // 
            label7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(538, 230);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(84, 15);
            label7.TabIndex = 21;
            label7.Text = "Bottom Left X";
            // 
            // label8
            // 
            label8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(538, 259);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(83, 15);
            label8.TabIndex = 22;
            label8.Text = "Bottom Left Y";
            // 
            // label9
            // 
            label9.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(538, 350);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(41, 15);
            label9.TabIndex = 23;
            label9.Text = "Width";
            // 
            // label10
            // 
            label10.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(538, 379);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(45, 15);
            label10.TabIndex = 24;
            label10.Text = "Height";
            // 
            // checkBoxCenter
            // 
            checkBoxCenter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBoxCenter.AutoSize = true;
            checkBoxCenter.Location = new System.Drawing.Point(538, 406);
            checkBoxCenter.Name = "checkBoxCenter";
            checkBoxCenter.Size = new System.Drawing.Size(63, 19);
            checkBoxCenter.TabIndex = 11;
            checkBoxCenter.Text = "Center";
            checkBoxCenter.UseVisualStyleBackColor = true;
            checkBoxCenter.CheckedChanged += checkBoxCenter_CheckedChanged;
            // 
            // panelDivider
            // 
            panelDivider.BackColor = System.Drawing.SystemColors.ControlDark;
            panelDivider.Location = new System.Drawing.Point(527, 12);
            panelDivider.Name = "panelDivider";
            panelDivider.Size = new System.Drawing.Size(1, 501);
            panelDivider.TabIndex = 25;
            // 
            // resetAllButton
            // 
            resetAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            resetAllButton.Location = new System.Drawing.Point(632, 285);
            resetAllButton.Name = "resetAllButton";
            resetAllButton.Size = new System.Drawing.Size(60, 23);
            resetAllButton.TabIndex = 26;
            resetAllButton.Text = "Reset All";
            resetAllButton.UseVisualStyleBackColor = true;
            resetAllButton.Click += resetAllButton_Click;
            // 
            // cropOutsideCheckBox
            // 
            cropOutsideCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cropOutsideCheckBox.AutoSize = true;
            cropOutsideCheckBox.Checked = true;
            cropOutsideCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            cropOutsideCheckBox.Location = new System.Drawing.Point(538, 431);
            cropOutsideCheckBox.Name = "cropOutsideCheckBox";
            cropOutsideCheckBox.Size = new System.Drawing.Size(174, 19);
            cropOutsideCheckBox.TabIndex = 27;
            cropOutsideCheckBox.Text = "Crop outside quadrilateral";
            cropOutsideCheckBox.UseVisualStyleBackColor = true;
            cropOutsideCheckBox.CheckedChanged += CropOutsideCheckBox_CheckedChanged;
            // 
            // checkBoxMoveNearestNub
            // 
            checkBoxMoveNearestNub.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBoxMoveNearestNub.AutoSize = true;
            checkBoxMoveNearestNub.Checked = true;
            checkBoxMoveNearestNub.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxMoveNearestNub.Location = new System.Drawing.Point(538, 456);
            checkBoxMoveNearestNub.Name = "checkBoxMoveNearestNub";
            checkBoxMoveNearestNub.Size = new System.Drawing.Size(173, 19);
            checkBoxMoveNearestNub.TabIndex = 28;
            checkBoxMoveNearestNub.Text = "Move nearest nub on click";
            checkBoxMoveNearestNub.UseVisualStyleBackColor = true;
            checkBoxMoveNearestNub.CheckedChanged += CheckBoxMoveNearestNub_CheckedChanged;
            // 
            // QuadrilateralCorrectionConfigDialog
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(704, 526);
            Controls.Add(resetAllButton);
            Controls.Add(panelDivider);
            Controls.Add(checkBoxCenter);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(checkBoxAutoDims);
            Controls.Add(numericUpDown2);
            Controls.Add(numericUpDown1);
            Controls.Add(quadControl11);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(numericUpDownBottomLeftY);
            Controls.Add(numericUpDownBottomLeftX);
            Controls.Add(numericUpDownBottomRightY);
            Controls.Add(numericUpDownBottomRightX);
            Controls.Add(numericUpDownTopRightY);
            Controls.Add(numericUpDownTopRightX);
            Controls.Add(numericUpDownTopLeftY);
            Controls.Add(numericUpDownTopLeftX);
            Controls.Add(cropOutsideCheckBox);
            Controls.Add(checkBoxMoveNearestNub);
            DoubleBuffered = true;
            HelpButton = true;
            KeyPreview = true;
            Location = new System.Drawing.Point(0, 0);
            Name = "QuadrilateralCorrectionConfigDialog";
            Text = "Quadrilateral Correction";
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopLeftX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopLeftY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopRightX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTopRightY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomRightX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomRightY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomLeftX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBottomLeftY).EndInit();
            ((System.ComponentModel.ISupportInitialize)quadControl11).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.CheckBox checkBoxAutoDims;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox checkBoxCenter;
        private System.Windows.Forms.Panel panelDivider;
        private System.Windows.Forms.Button resetAllButton;
        private System.Windows.Forms.CheckBox cropOutsideCheckBox;
        private System.Windows.Forms.CheckBox checkBoxMoveNearestNub;
    }
}