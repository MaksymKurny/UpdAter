
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.controlPanel = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnMin = new System.Windows.Forms.Button();
            this.listPanel = new UpdAter.ImagePanel();
            this.uaList = new System.Windows.Forms.TableLayoutPanel();
            this.btnUpdateList = new UpdAter.FlatNoBorder();
            this.btnAddNew = new UpdAter.FlatNoBorder();
            this.btnMoreInfo = new UpdAter.FlatNoBorder();
            this.controlPanel.SuspendLayout();
            this.listPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(29)))), ((int)(((byte)(37)))));
            this.controlPanel.Controls.Add(this.btnMin);
            this.controlPanel.Controls.Add(this.btnUpdateList);
            this.controlPanel.Controls.Add(this.btnAddNew);
            this.controlPanel.Controls.Add(this.btnMoreInfo);
            this.controlPanel.Controls.Add(this.btnExit);
            this.controlPanel.Cursor = System.Windows.Forms.Cursors.SizeAll;
            resources.ApplyResources(this.controlPanel, "controlPanel");
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            // 
            // btnExit
            // 
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(195)))), ((int)(((byte)(78)))));
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // btnMin
            // 
            this.btnMin.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnMin.FlatAppearance.BorderSize = 0;
            this.btnMin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(29)))), ((int)(((byte)(37)))));
            resources.ApplyResources(this.btnMin, "btnMin");
            this.btnMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(195)))), ((int)(((byte)(78)))));
            this.btnMin.Name = "btnMin";
            this.btnMin.UseVisualStyleBackColor = true;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // listPanel
            // 
            resources.ApplyResources(this.listPanel, "listPanel");
            this.listPanel.BackColor = System.Drawing.Color.Transparent;
            this.listPanel.Controls.Add(this.uaList);
            this.listPanel.Name = "listPanel";
            // 
            // uaList
            // 
            resources.ApplyResources(this.uaList, "uaList");
            this.uaList.Name = "uaList";
            // 
            // btnUpdateList
            // 
            this.btnUpdateList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(74)))));
            this.btnUpdateList.BorderRadius = 4;
            this.btnUpdateList.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnUpdateList.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnUpdateList, "btnUpdateList");
            this.btnUpdateList.Name = "btnUpdateList";
            this.btnUpdateList.UseVisualStyleBackColor = false;
            this.btnUpdateList.Click += new System.EventHandler(this.AllUpdate);
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(74)))));
            this.btnAddNew.BorderRadius = 4;
            this.btnAddNew.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnAddNew.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnAddNew, "btnAddNew");
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btn_addNew_Click);
            // 
            // btnMoreInfo
            // 
            this.btnMoreInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(74)))));
            this.btnMoreInfo.BorderRadius = 4;
            this.btnMoreInfo.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnMoreInfo.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnMoreInfo, "btnMoreInfo");
            this.btnMoreInfo.Name = "btnMoreInfo";
            this.btnMoreInfo.UseVisualStyleBackColor = false;
            this.btnMoreInfo.Click += new System.EventHandler(this.credits_Click);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.listPanel);
            this.Controls.Add(this.controlPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.controlPanel.ResumeLayout(false);
            this.listPanel.ResumeLayout(false);
            this.listPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel uaList;
        private UpdAter.FlatNoBorder btnAddNew;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Button btnExit;
        private UpdAter.FlatNoBorder btnUpdateList;
        private UpdAter.ImagePanel listPanel;
        private UpdAter.FlatNoBorder btnMoreInfo;
        private System.Windows.Forms.Button btnMin;
    }
}

