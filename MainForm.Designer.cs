
namespace UpdAter
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.iconList = new System.Windows.Forms.ImageList(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.uaList = new System.Windows.Forms.TableLayoutPanel();
            this.addNew = new System.Windows.Forms.Button();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.updateAll = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.listPanel = new System.Windows.Forms.Panel();
            this.controlPanel.SuspendLayout();
            this.listPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // iconList
            // 
            this.iconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iconList.ImageStream")));
            this.iconList.TransparentColor = System.Drawing.Color.Transparent;
            this.iconList.Images.SetKeyName(0, "icon-calendar-clock.png");
            this.iconList.Images.SetKeyName(1, "icon-coins.png");
            this.iconList.Images.SetKeyName(2, "icon-info.png");
            this.iconList.Images.SetKeyName(3, "icon-list.png");
            this.iconList.Images.SetKeyName(4, "icon-list-check.png");
            this.iconList.Images.SetKeyName(5, "icon-pencil.png");
            this.iconList.Images.SetKeyName(6, "icon-settings-sliders.png");
            this.iconList.Images.SetKeyName(7, "icon-star.png");
            this.iconList.Images.SetKeyName(8, "icon-trash.png");
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            resources.ApplyResources(this.notifyIcon, "notifyIcon");
            // 
            // uaList
            // 
            resources.ApplyResources(this.uaList, "uaList");
            this.uaList.Name = "uaList";
            // 
            // addNew
            // 
            this.addNew.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.addNew, "addNew");
            this.addNew.Name = "addNew";
            this.addNew.UseVisualStyleBackColor = true;
            this.addNew.Click += new System.EventHandler(this.btn_addNew_Click);
            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(54)))));
            this.controlPanel.Controls.Add(this.updateAll);
            this.controlPanel.Controls.Add(this.exitButton);
            this.controlPanel.Controls.Add(this.addNew);
            this.controlPanel.Cursor = System.Windows.Forms.Cursors.SizeAll;
            resources.ApplyResources(this.controlPanel, "controlPanel");
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            // 
            // updateAll
            // 
            this.updateAll.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.updateAll, "updateAll");
            this.updateAll.Name = "updateAll";
            this.updateAll.UseVisualStyleBackColor = true;
            // 
            // exitButton
            // 
            this.exitButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.exitButton.FlatAppearance.BorderSize = 0;
            this.exitButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.exitButton, "exitButton");
            this.exitButton.ForeColor = System.Drawing.Color.White;
            this.exitButton.Name = "exitButton";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // listPanel
            // 
            resources.ApplyResources(this.listPanel, "listPanel");
            this.listPanel.Controls.Add(this.uaList);
            this.listPanel.Name = "listPanel";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.Controls.Add(this.listPanel);
            this.Controls.Add(this.controlPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.controlPanel.ResumeLayout(false);
            this.listPanel.ResumeLayout(false);
            this.listPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList iconList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel37;
        private System.Windows.Forms.Panel panel31;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Panel panel38;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel25;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel39;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel32;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel40;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel19;
        private System.Windows.Forms.Panel panel18;
        private System.Windows.Forms.Panel panel41;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel33;
        private System.Windows.Forms.Panel panel17;
        private System.Windows.Forms.Panel panel42;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel26;
        private System.Windows.Forms.Panel panel16;
        private System.Windows.Forms.Panel panel34;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Panel panel24;
        private System.Windows.Forms.Panel panel35;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Panel panel27;
        private System.Windows.Forms.Panel panel23;
        private System.Windows.Forms.Panel panel36;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel20;
        private System.Windows.Forms.Panel panel22;
        private System.Windows.Forms.Panel panel28;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel30;
        private System.Windows.Forms.Panel panel29;
        private System.Windows.Forms.Panel panel21;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.TableLayoutPanel uaList;
        private System.Windows.Forms.Button addNew;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button updateAll;
        private System.Windows.Forms.Panel listPanel;
    }
}

