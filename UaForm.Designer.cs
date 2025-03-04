
namespace UpdAter
{
    partial class UaForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UaForm));
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.gamePathTextBox = new System.Windows.Forms.TextBox();
            this.btnPath = new System.Windows.Forms.Button();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.iconTextBox = new System.Windows.Forms.TextBox();
            this.btnIcon = new System.Windows.Forms.Button();
            this.btnBaner = new System.Windows.Forms.Button();
            this.bannerTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.helpUrl = new System.Windows.Forms.Label();
            this.helpToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.helpPath = new System.Windows.Forms.Label();
            this.btnFillInfo = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.guideTextBox = new System.Windows.Forms.TextBox();
            this.helpGuide = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // titleTextBox
            // 
            resources.ApplyResources(this.titleTextBox, "titleTextBox");
            this.titleTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.titleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.titleTextBox.ForeColor = System.Drawing.Color.Black;
            this.titleTextBox.Name = "titleTextBox";
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // gamePathTextBox
            // 
            resources.ApplyResources(this.gamePathTextBox, "gamePathTextBox");
            this.gamePathTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.gamePathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gamePathTextBox.ForeColor = System.Drawing.Color.Black;
            this.gamePathTextBox.Name = "gamePathTextBox";
            // 
            // btnPath
            // 
            resources.ApplyResources(this.btnPath, "btnPath");
            this.btnPath.Name = "btnPath";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.OpenFolderBrowser);
            // 
            // urlTextBox
            // 
            resources.ApplyResources(this.urlTextBox, "urlTextBox");
            this.urlTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.urlTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.urlTextBox.ForeColor = System.Drawing.Color.Black;
            this.urlTextBox.Name = "urlTextBox";
            // 
            // iconTextBox
            // 
            resources.ApplyResources(this.iconTextBox, "iconTextBox");
            this.iconTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.iconTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.iconTextBox.ForeColor = System.Drawing.Color.Black;
            this.iconTextBox.Name = "iconTextBox";
            // 
            // btnIcon
            // 
            resources.ApplyResources(this.btnIcon, "btnIcon");
            this.btnIcon.Name = "btnIcon";
            this.btnIcon.UseVisualStyleBackColor = true;
            this.btnIcon.Click += new System.EventHandler(this.btnIcon_Click);
            // 
            // btnBaner
            // 
            resources.ApplyResources(this.btnBaner, "btnBaner");
            this.btnBaner.Name = "btnBaner";
            this.btnBaner.UseVisualStyleBackColor = true;
            this.btnBaner.Click += new System.EventHandler(this.btnBaner_Click);
            // 
            // bannerTextBox
            // 
            resources.ApplyResources(this.bannerTextBox, "bannerTextBox");
            this.bannerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.bannerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bannerTextBox.ForeColor = System.Drawing.Color.Black;
            this.bannerTextBox.Name = "bannerTextBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Name = "label5";
            // 
            // helpUrl
            // 
            resources.ApplyResources(this.helpUrl, "helpUrl");
            this.helpUrl.BackColor = System.Drawing.Color.Transparent;
            this.helpUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(195)))), ((int)(((byte)(78)))));
            this.helpUrl.Name = "helpUrl";
            // 
            // helpPath
            // 
            resources.ApplyResources(this.helpPath, "helpPath");
            this.helpPath.BackColor = System.Drawing.Color.Transparent;
            this.helpPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(195)))), ((int)(((byte)(78)))));
            this.helpPath.Name = "helpPath";
            // 
            // btnFillInfo
            // 
            resources.ApplyResources(this.btnFillInfo, "btnFillInfo");
            this.btnFillInfo.Name = "btnFillInfo";
            this.btnFillInfo.UseVisualStyleBackColor = true;
            this.btnFillInfo.Click += new System.EventHandler(this.btnFillInfo_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(29)))), ((int)(((byte)(37)))));
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(195)))), ((int)(((byte)(78)))));
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Name = "label6";
            // 
            // guideTextBox
            // 
            resources.ApplyResources(this.guideTextBox, "guideTextBox");
            this.guideTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.guideTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.guideTextBox.ForeColor = System.Drawing.Color.Black;
            this.guideTextBox.Name = "guideTextBox";
            // 
            // helpGuide
            // 
            resources.ApplyResources(this.helpGuide, "helpGuide");
            this.helpGuide.BackColor = System.Drawing.Color.Transparent;
            this.helpGuide.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(195)))), ((int)(((byte)(78)))));
            this.helpGuide.Name = "helpGuide";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.panel1.Name = "panel1";
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UaForm_MouseDown);
            // 
            // UaForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::UpdAter.Properties.Resources.forn_bg;
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.helpGuide);
            this.Controls.Add(this.guideTextBox);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnFillInfo);
            this.Controls.Add(this.helpPath);
            this.Controls.Add(this.helpUrl);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBaner);
            this.Controls.Add(this.bannerTextBox);
            this.Controls.Add(this.btnIcon);
            this.Controls.Add(this.iconTextBox);
            this.Controls.Add(this.urlTextBox);
            this.Controls.Add(this.btnPath);
            this.Controls.Add(this.gamePathTextBox);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.titleTextBox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "UaForm";
            this.ShowInTaskbar = false;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UaForm_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.TextBox gamePathTextBox;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.TextBox iconTextBox;
        private System.Windows.Forms.Button btnIcon;
        private System.Windows.Forms.Button btnBaner;
        private System.Windows.Forms.TextBox bannerTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label helpUrl;
        private System.Windows.Forms.ToolTip helpToolTip;
        private System.Windows.Forms.Label helpPath;
        private System.Windows.Forms.Button btnFillInfo;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox guideTextBox;
        private System.Windows.Forms.Label helpGuide;
        private System.Windows.Forms.Panel panel1;
    }
}