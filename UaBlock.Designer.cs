
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UaBlock));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.gameIcon = new System.Windows.Forms.PictureBox();
            this.txtPercent = new System.Windows.Forms.Label();
            this.moreMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddToList = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPin = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGuide = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDell = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMore = new UpdAter.FlatNoBorder();
            this.txtLastUpd = new UpdAter.TransparentLabel();
            this.txtTitle = new UpdAter.TransparentLabel();
            this.btnUpdate = new UpdAter.FlatNoBorder();
            ((System.ComponentModel.ISupportInitialize)(this.gameIcon)).BeginInit();
            this.moreMenu.SuspendLayout();
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
            resources.ApplyResources(this.gameIcon, "gameIcon");
            this.gameIcon.BackColor = System.Drawing.Color.Transparent;
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
            // moreMenu
            // 
            resources.ApplyResources(this.moreMenu, "moreMenu");
            this.moreMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.moreMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEdit,
            this.menuAddToList,
            this.menuPin,
            this.menuOpen,
            this.menuGuide,
            this.menuDell});
            this.moreMenu.Name = "moreMenu";
            // 
            // menuEdit
            // 
            resources.ApplyResources(this.menuEdit, "menuEdit");
            this.menuEdit.Name = "menuEdit";
            this.menuEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // menuAddToList
            // 
            resources.ApplyResources(this.menuAddToList, "menuAddToList");
            this.menuAddToList.CheckOnClick = true;
            this.menuAddToList.Name = "menuAddToList";
            this.menuAddToList.CheckedChanged += new System.EventHandler(this.menuButton_CheckedChanged);
            // 
            // menuPin
            // 
            resources.ApplyResources(this.menuPin, "menuPin");
            this.menuPin.CheckOnClick = true;
            this.menuPin.Name = "menuPin";
            this.menuPin.CheckedChanged += new System.EventHandler(this.menuButton_CheckedChanged);
            // 
            // menuOpen
            // 
            resources.ApplyResources(this.menuOpen, "menuOpen");
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // menuGuide
            // 
            resources.ApplyResources(this.menuGuide, "menuGuide");
            this.menuGuide.Name = "menuGuide";
            this.menuGuide.Click += new System.EventHandler(this.menuGuide_Click);
            // 
            // menuDell
            // 
            resources.ApplyResources(this.menuDell, "menuDell");
            this.menuDell.ForeColor = System.Drawing.Color.Red;
            this.menuDell.Name = "menuDell";
            this.menuDell.Click += new System.EventHandler(this.btnDell_Click);
            // 
            // btnMore
            // 
            resources.ApplyResources(this.btnMore, "btnMore");
            this.btnMore.BackColor = System.Drawing.Color.White;
            this.btnMore.BorderRadius = 4;
            this.btnMore.FlatAppearance.BorderSize = 0;
            this.btnMore.Name = "btnMore";
            this.btnMore.UseVisualStyleBackColor = false;
            this.btnMore.Click += new System.EventHandler(this.btnMore_Click);
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
            // UaBlock
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(29)))), ((int)(((byte)(37)))));
            this.BackgroundImage = global::UpdAter.Properties.Resources.background;
            this.Controls.Add(this.btnMore);
            this.Controls.Add(this.txtLastUpd);
            this.Controls.Add(this.txtPercent);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.gameIcon);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnUpdate);
            this.DoubleBuffered = true;
            this.Name = "UaBlock";
            ((System.ComponentModel.ISupportInitialize)(this.gameIcon)).EndInit();
            this.moreMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private UpdAter.FlatNoBorder btnUpdate;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.PictureBox gameIcon;
        private UpdAter.TransparentLabel txtTitle;
        private System.Windows.Forms.Label txtPercent;
        private TransparentLabel txtLastUpd;
        private System.Windows.Forms.ContextMenuStrip moreMenu;
        private System.Windows.Forms.ToolStripMenuItem menuDell;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripMenuItem menuEdit;
        private FlatNoBorder btnMore;
        private System.Windows.Forms.ToolStripMenuItem menuAddToList;
        private System.Windows.Forms.ToolStripMenuItem menuPin;
        private System.Windows.Forms.ToolStripMenuItem menuGuide;
    }
}
