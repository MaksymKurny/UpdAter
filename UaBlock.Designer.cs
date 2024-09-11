
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
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDell = new System.Windows.Forms.Button();
            this.txtTitle = new System.Windows.Forms.GroupBox();
            this.gameIcon = new System.Windows.Forms.PictureBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.txtTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gameIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDell
            // 
            resources.ApplyResources(this.btnDell, "btnDell");
            this.btnDell.ForeColor = System.Drawing.Color.Red;
            this.btnDell.Name = "btnDell";
            this.btnDell.UseVisualStyleBackColor = true;
            this.btnDell.Click += new System.EventHandler(this.btnDell_Click);
            // 
            // txtTitle
            // 
            resources.ApplyResources(this.txtTitle, "txtTitle");
            this.txtTitle.Controls.Add(this.gameIcon);
            this.txtTitle.Controls.Add(this.progressBar);
            this.txtTitle.Controls.Add(this.btnEdit);
            this.txtTitle.Controls.Add(this.btnUpdate);
            this.txtTitle.Controls.Add(this.btnDell);
            this.txtTitle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.TabStop = false;
            // 
            // gameIcon
            // 
            resources.ApplyResources(this.gameIcon, "gameIcon");
            this.gameIcon.Name = "gameIcon";
            this.gameIcon.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.Name = "progressBar";
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // UaBlock
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtTitle);
            this.Name = "UaBlock";
            this.txtTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gameIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDell;
        private System.Windows.Forms.GroupBox txtTitle;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.PictureBox gameIcon;
    }
}
