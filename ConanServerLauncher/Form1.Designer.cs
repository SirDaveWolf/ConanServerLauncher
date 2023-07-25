namespace ConanServerLauncher
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonSaveConfig = new Button();
            buttonRefresh = new Button();
            SuspendLayout();
            // 
            // buttonSaveConfig
            // 
            buttonSaveConfig.Location = new Point(676, 526);
            buttonSaveConfig.Name = "buttonSaveConfig";
            buttonSaveConfig.Size = new Size(100, 23);
            buttonSaveConfig.TabIndex = 1;
            buttonSaveConfig.Text = "Save Config";
            buttonSaveConfig.UseVisualStyleBackColor = true;
            buttonSaveConfig.Click += buttonSaveConfig_Click;
            // 
            // buttonRefresh
            // 
            buttonRefresh.Location = new Point(12, 526);
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Size = new Size(75, 23);
            buttonRefresh.TabIndex = 2;
            buttonRefresh.Text = "Refresh List";
            buttonRefresh.UseVisualStyleBackColor = true;
            buttonRefresh.Click += buttonRefresh_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(buttonRefresh);
            Controls.Add(buttonSaveConfig);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Form1";
            Text = "ConanServerLauncher";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button buttonSaveConfig;
        private Button buttonRefresh;
    }
}