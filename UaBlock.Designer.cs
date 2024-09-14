
namespace UpdAter
{
    partial class UaBlock
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UaBlock));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.gameIcon = new System.Windows.Forms.PictureBox();
            this.txtPercent = new System.Windows.Forms.Label();
            this.txtLastUpd = new UpdAter.TransparentLabel();
            this.txtTitle = new UpdAter.TransparentLabel();
            this.btnEdit = new UpdAter.FlatNoBorder();
            this.btnUpdate = new UpdAter.FlatNoBorder();
            this.btnDell = new UpdAter.FlatNoBorder();
            ((System.ComponentModel.ISupportInitialize)(this.gameIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.progressBar.Name = "progressBar";
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // gameIcon
            // 
            this.gameIcon.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.gameIcon, "gameIcon");
            this.gameIcon.Name = "gameIcon";
            this.gameIcon.TabStop = false;
            // 
            // txtPercent
            // 
            resources.ApplyResources(this.txtPercent, "txtPercent");
            this.txtPercent.BackColor = System.Drawing.Color.Transparent;
            this.txtPercent.ForeColor = System.Drawing.Color.White;
            this.txtPercent.Name = "txtPercent";
            // 
            // txtLastUpd
            // 
            resources.ApplyResources(this.txtLastUpd, "txtLastUpd");
            this.txtLastUpd.BackColor = System.Drawing.Color.Transparent;
            this.txtLastUpd.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.txtLastUpd.BorderRadius = 6;
            this.txtLastUpd.ForeColor = System.Drawing.SystemColors.Control;
            this.txtLastUpd.Name = "txtLastUpd";
            // 
            // txtTitle
            // 
            resources.ApplyResources(this.txtTitle, "txtTitle");
            this.txtTitle.BackColor = System.Drawing.Color.Transparent;
            this.txtTitle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.txtTitle.BorderRadius = 6;
            this.txtTitle.ForeColor = System.Drawing.SystemColors.Control;
            this.txtTitle.Name = "txtTitle";
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.BackColor = System.Drawing.Color.White;
            this.btnEdit.BorderRadius = 4;
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.BackColor = System.Drawing.Color.White;
            this.btnUpdate.BorderRadius = 4;
            this.btnUpdate.FlatAppearance.BorderSize = 0;
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDell
            // 
            resources.ApplyResources(this.btnDell, "btnDell");
            this.btnDell.BackColor = System.Drawing.Color.White;
            this.btnDell.BorderRadius = 4;
            this.btnDell.FlatAppearance.BorderSize = 0;
            this.btnDell.ForeColor = System.Drawing.Color.Red;
            this.btnDell.Name = "btnDell";
            this.btnDell.UseVisualStyleBackColor = false;
            this.btnDell.Click += new System.EventHandler(this.btnDell_Click);
            // 
            // UaBlock
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(27)))), ((int)(((byte)(45)))));
            this.BackgroundImage = global::UpdAter.Properties.Resources.background1;
            this.Controls.Add(this.txtLastUpd);
            this.Controls.Add(this.txtPercent);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.gameIcon);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnDell);
            this.DoubleBuffered = true;
            this.Name = "UaBlock";
            ((System.ComponentModel.ISupportInitialize)(this.gameIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private UpdAter.FlatNoBorder btnEdit;
        private UpdAter.FlatNoBorder btnDell;
        private UpdAter.FlatNoBorder btnUpdate;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.PictureBox gameIcon;
        private UpdAter.TransparentLabel txtTitle;
        private System.Windows.Forms.Label txtPercent;
        private TransparentLabel txtLastUpd;
    }
}
