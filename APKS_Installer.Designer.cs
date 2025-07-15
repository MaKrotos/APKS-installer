namespace APKS_installer
{
    partial class APKS_Installer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button buttonInstallAll;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ComboBox comboBoxDevices;
        private System.Windows.Forms.Button buttonRefreshDevices;
        private System.Windows.Forms.ComboBox comboBoxLanguage;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(APKS_Installer));
            textBoxFilePath = new TextBox();
            buttonInstallAll = new Button();
            textBoxLog = new TextBox();
            progressBar = new ProgressBar();
            comboBoxDevices = new ComboBox();
            buttonRefreshDevices = new Button();
            comboBoxLanguage = new ComboBox();
            SuspendLayout();
            // 
            // textBoxFilePath
            // 
            textBoxFilePath.Location = new Point(236, 41);
            textBoxFilePath.Name = "textBoxFilePath";
            textBoxFilePath.ReadOnly = true;
            textBoxFilePath.Size = new Size(420, 23);
            textBoxFilePath.TabIndex = 1;
            // 
            // buttonInstallAll
            // 
            buttonInstallAll.Location = new Point(30, 39);
            buttonInstallAll.Name = "buttonInstallAll";
            buttonInstallAll.Size = new Size(200, 30);
            buttonInstallAll.TabIndex = 2;
            buttonInstallAll.Text = "Выбрать и установить .apks";
            buttonInstallAll.UseVisualStyleBackColor = true;
            buttonInstallAll.Click += buttonInstallAll_Click;
            // 
            // textBoxLog
            // 
            textBoxLog.Location = new Point(30, 77);
            textBoxLog.Multiline = true;
            textBoxLog.Name = "textBoxLog";
            textBoxLog.ReadOnly = true;
            textBoxLog.ScrollBars = ScrollBars.Vertical;
            textBoxLog.Size = new Size(740, 317);
            textBoxLog.TabIndex = 3;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(30, 400);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(740, 23);
            progressBar.TabIndex = 4;
            // 
            // comboBoxDevices
            // 
            comboBoxDevices.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxDevices.Location = new Point(30, 10);
            comboBoxDevices.Name = "comboBoxDevices";
            comboBoxDevices.Size = new Size(300, 23);
            comboBoxDevices.TabIndex = 5;
            // 
            // buttonRefreshDevices
            // 
            buttonRefreshDevices.Location = new Point(340, 10);
            buttonRefreshDevices.Name = "buttonRefreshDevices";
            buttonRefreshDevices.Size = new Size(120, 23);
            buttonRefreshDevices.TabIndex = 6;
            buttonRefreshDevices.Text = "Обновить список устройств";
            buttonRefreshDevices.UseVisualStyleBackColor = true;
            buttonRefreshDevices.Click += buttonRefreshDevices_Click;
            // 
            // comboBoxLanguage
            // 
            comboBoxLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxLanguage.Location = new Point(480, 10);
            comboBoxLanguage.Name = "comboBoxLanguage";
            comboBoxLanguage.Size = new Size(120, 23);
            comboBoxLanguage.TabIndex = 7;
            // 
            // APKS_Installer
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 435);
            Controls.Add(comboBoxLanguage);
            Controls.Add(buttonRefreshDevices);
            Controls.Add(comboBoxDevices);
            Controls.Add(progressBar);
            Controls.Add(buttonInstallAll);
            Controls.Add(textBoxFilePath);
            Controls.Add(textBoxLog);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(816, 474);
            MinimumSize = new Size(816, 474);
            Name = "APKS_Installer";
            Text = "Установщик APK/APKS для Android (через ADB)";
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
