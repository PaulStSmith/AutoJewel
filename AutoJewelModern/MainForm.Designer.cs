namespace AutoJewelModern
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            gameModeGroupBox = new GroupBox();
            balanceRadioButton = new RadioButton();
            iceStormRadioButton = new RadioButton();
            lightningRadioButton = new RadioButton();
            zenRadioButton = new RadioButton();
            classicRadioButton = new RadioButton();
            processGroupBox = new GroupBox();
            trialRadioButton = new RadioButton();
            standardRadioButton = new RadioButton();
            controlGroupBox = new GroupBox();
            stopButton = new Button();
            startButton = new Button();
            debugGroupBox = new GroupBox();
            debugCheckBox = new CheckBox();
            aboutButton = new Button();
            exitButton = new Button();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            instructionLabel = new Label();
            gameModeGroupBox.SuspendLayout();
            processGroupBox.SuspendLayout();
            controlGroupBox.SuspendLayout();
            debugGroupBox.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // gameModeGroupBox
            // 
            gameModeGroupBox.Controls.Add(balanceRadioButton);
            gameModeGroupBox.Controls.Add(iceStormRadioButton);
            gameModeGroupBox.Controls.Add(lightningRadioButton);
            gameModeGroupBox.Controls.Add(zenRadioButton);
            gameModeGroupBox.Controls.Add(classicRadioButton);
            gameModeGroupBox.Location = new Point(12, 12);
            gameModeGroupBox.Name = "gameModeGroupBox";
            gameModeGroupBox.Size = new Size(120, 160);
            gameModeGroupBox.TabIndex = 0;
            gameModeGroupBox.TabStop = false;
            gameModeGroupBox.Text = "Game Mode";
            // 
            // balanceRadioButton
            // 
            balanceRadioButton.AutoSize = true;
            balanceRadioButton.Location = new Point(6, 122);
            balanceRadioButton.Name = "balanceRadioButton";
            balanceRadioButton.Size = new Size(71, 19);
            balanceRadioButton.TabIndex = 4;
            balanceRadioButton.Text = "Balance";
            balanceRadioButton.UseVisualStyleBackColor = true;
            // 
            // iceStormRadioButton
            // 
            iceStormRadioButton.AutoSize = true;
            iceStormRadioButton.Checked = true;
            iceStormRadioButton.Location = new Point(6, 97);
            iceStormRadioButton.Name = "iceStormRadioButton";
            iceStormRadioButton.Size = new Size(78, 19);
            iceStormRadioButton.TabIndex = 3;
            iceStormRadioButton.TabStop = true;
            iceStormRadioButton.Text = "Ice Storm";
            iceStormRadioButton.UseVisualStyleBackColor = true;
            // 
            // lightningRadioButton
            // 
            lightningRadioButton.AutoSize = true;
            lightningRadioButton.Location = new Point(6, 72);
            lightningRadioButton.Name = "lightningRadioButton";
            lightningRadioButton.Size = new Size(78, 19);
            lightningRadioButton.TabIndex = 2;
            lightningRadioButton.Text = "Lightning";
            lightningRadioButton.UseVisualStyleBackColor = true;
            // 
            // zenRadioButton
            // 
            zenRadioButton.AutoSize = true;
            zenRadioButton.Location = new Point(6, 47);
            zenRadioButton.Name = "zenRadioButton";
            zenRadioButton.Size = new Size(46, 19);
            zenRadioButton.TabIndex = 1;
            zenRadioButton.Text = "Zen";
            zenRadioButton.UseVisualStyleBackColor = true;
            // 
            // classicRadioButton
            // 
            classicRadioButton.AutoSize = true;
            classicRadioButton.Location = new Point(6, 22);
            classicRadioButton.Name = "classicRadioButton";
            classicRadioButton.Size = new Size(62, 19);
            classicRadioButton.TabIndex = 0;
            classicRadioButton.Text = "Classic";
            classicRadioButton.UseVisualStyleBackColor = true;
            // 
            // processGroupBox
            // 
            processGroupBox.Controls.Add(trialRadioButton);
            processGroupBox.Controls.Add(standardRadioButton);
            processGroupBox.Location = new Point(150, 12);
            processGroupBox.Name = "processGroupBox";
            processGroupBox.Size = new Size(120, 80);
            processGroupBox.TabIndex = 1;
            processGroupBox.TabStop = false;
            processGroupBox.Text = "Process Type";
            // 
            // trialRadioButton
            // 
            trialRadioButton.AutoSize = true;
            trialRadioButton.Location = new Point(6, 47);
            trialRadioButton.Name = "trialRadioButton";
            trialRadioButton.Size = new Size(48, 19);
            trialRadioButton.TabIndex = 1;
            trialRadioButton.Text = "Trial";
            trialRadioButton.UseVisualStyleBackColor = true;
            // 
            // standardRadioButton
            // 
            standardRadioButton.AutoSize = true;
            standardRadioButton.Checked = true;
            standardRadioButton.Location = new Point(6, 22);
            standardRadioButton.Name = "standardRadioButton";
            standardRadioButton.Size = new Size(74, 19);
            standardRadioButton.TabIndex = 0;
            standardRadioButton.TabStop = true;
            standardRadioButton.Text = "Standard";
            standardRadioButton.UseVisualStyleBackColor = true;
            // 
            // controlGroupBox
            // 
            controlGroupBox.Controls.Add(stopButton);
            controlGroupBox.Controls.Add(startButton);
            controlGroupBox.Location = new Point(150, 110);
            controlGroupBox.Name = "controlGroupBox";
            controlGroupBox.Size = new Size(120, 62);
            controlGroupBox.TabIndex = 2;
            controlGroupBox.TabStop = false;
            controlGroupBox.Text = "Control";
            // 
            // debugGroupBox
            // 
            debugGroupBox.Controls.Add(debugCheckBox);
            debugGroupBox.Location = new Point(288, 12);
            debugGroupBox.Name = "debugGroupBox";
            debugGroupBox.Size = new Size(100, 50);
            debugGroupBox.TabIndex = 7;
            debugGroupBox.TabStop = false;
            debugGroupBox.Text = "Debug";
            // 
            // debugCheckBox
            // 
            debugCheckBox.AutoSize = true;
            debugCheckBox.Location = new Point(6, 22);
            debugCheckBox.Name = "debugCheckBox";
            debugCheckBox.Size = new Size(87, 19);
            debugCheckBox.TabIndex = 0;
            debugCheckBox.Text = "Log && Bitmaps";
            debugCheckBox.UseVisualStyleBackColor = true;
            debugCheckBox.CheckedChanged += debugCheckBox_CheckedChanged;
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new Point(62, 22);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(50, 23);
            stopButton.TabIndex = 1;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += stopButton_Click;
            // 
            // startButton
            // 
            startButton.Location = new Point(6, 22);
            startButton.Name = "startButton";
            startButton.Size = new Size(50, 23);
            startButton.TabIndex = 0;
            startButton.Text = "Start";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += startButton_Click;
            // 
            // aboutButton
            // 
            aboutButton.Location = new Point(12, 190);
            aboutButton.Name = "aboutButton";
            aboutButton.Size = new Size(75, 23);
            aboutButton.TabIndex = 3;
            aboutButton.Text = "About";
            aboutButton.UseVisualStyleBackColor = true;
            aboutButton.Click += aboutButton_Click;
            // 
            // exitButton
            // 
            exitButton.Location = new Point(195, 190);
            exitButton.Name = "exitButton";
            exitButton.Size = new Size(75, 23);
            exitButton.TabIndex = 4;
            exitButton.Text = "Exit";
            exitButton.UseVisualStyleBackColor = true;
            exitButton.Click += exitButton_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 268);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(400, 22);
            statusStrip.TabIndex = 5;
            statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(39, 17);
            statusLabel.Text = "Ready";
            // 
            // instructionLabel
            // 
            instructionLabel.Location = new Point(12, 225);
            instructionLabel.Name = "instructionLabel";
            instructionLabel.Size = new Size(376, 35);
            instructionLabel.TabIndex = 6;
            instructionLabel.Text = "Press Ctrl+F8 to start/stop automation.\r\nWorks best in windowed mode at 1024x768.";
            instructionLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 290);
            Controls.Add(instructionLabel);
            Controls.Add(statusStrip);
            Controls.Add(exitButton);
            Controls.Add(aboutButton);
            Controls.Add(debugGroupBox);
            Controls.Add(controlGroupBox);
            Controls.Add(processGroupBox);
            Controls.Add(gameModeGroupBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AutoJewel - Modernized";
            Load += MainForm_Load;
            gameModeGroupBox.ResumeLayout(false);
            gameModeGroupBox.PerformLayout();
            processGroupBox.ResumeLayout(false);
            processGroupBox.PerformLayout();
            controlGroupBox.ResumeLayout(false);
            debugGroupBox.ResumeLayout(false);
            debugGroupBox.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private GroupBox gameModeGroupBox;
        private RadioButton balanceRadioButton;
        private RadioButton iceStormRadioButton;
        private RadioButton lightningRadioButton;
        private RadioButton zenRadioButton;
        private RadioButton classicRadioButton;
        private GroupBox processGroupBox;
        private RadioButton trialRadioButton;
        private RadioButton standardRadioButton;
        private GroupBox controlGroupBox;
        private Button stopButton;
        private Button startButton;
        private GroupBox debugGroupBox;
        private CheckBox debugCheckBox;
        private Button aboutButton;
        private Button exitButton;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private Label instructionLabel;
    }
}