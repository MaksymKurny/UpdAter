
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
            this.updateAll = new UpdAter.FlatNoBorder();
            this.addNew = new UpdAter.FlatNoBorder();
            this.credits = new UpdAter.FlatNoBorder();
            this.exitButton = new System.Windows.Forms.Button();
            this.listPanel = new UpdAter.ImagePanel();
            this.uaList = new System.Windows.Forms.TableLayoutPanel();
            this.controlPanel.SuspendLayout();
            this.listPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(29)))), ((int)(((byte)(37)))));
            this.controlPanel.Controls.Add(this.updateAll);
            this.controlPanel.Controls.Add(this.addNew);
            this.controlPanel.Controls.Add(this.credits);
            this.controlPanel.Controls.Add(this.exitButton);
            this.controlPanel.Cursor = System.Windows.Forms.Cursors.SizeAll;
            resources.ApplyResources(this.controlPanel, "controlPanel");
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            // 
            // updateAll
            // 
            this.updateAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(74)))));
            this.updateAll.BorderRadius = 4;
            this.updateAll.Cursor = System.Windows.Forms.Cursors.Default;
            this.updateAll.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.updateAll, "updateAll");
            this.updateAll.Name = "updateAll";
            this.updateAll.UseVisualStyleBackColor = false;
            // 
            // addNew
            // 
            this.addNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(74)))));
            this.addNew.BorderRadius = 4;
            this.addNew.Cursor = System.Windows.Forms.Cursors.Default;
            this.addNew.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.addNew, "addNew");
            this.addNew.Name = "addNew";
            this.addNew.UseVisualStyleBackColor = false;
            this.addNew.Click += new System.EventHandler(this.btn_addNew_Click);
            // 
            // credits
            // 
            this.credits.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(74)))));
            this.credits.BorderRadius = 4;
            this.credits.Cursor = System.Windows.Forms.Cursors.Default;
            this.credits.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.credits, "credits");
            this.credits.Name = "credits";
            this.credits.UseVisualStyleBackColor = false;
            this.credits.Click += new System.EventHandler(this.credits_Click);
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
            this.listPanel.BackColor = System.Drawing.Color.Transparent;
            this.listPanel.Controls.Add(this.uaList);
            this.listPanel.Name = "listPanel";
            // 
            // uaList
            // 
            resources.ApplyResources(this.uaList, "uaList");
            this.uaList.Name = "uaList";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.listPanel);
            this.Controls.Add(this.controlPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.controlPanel.ResumeLayout(false);
            this.listPanel.ResumeLayout(false);
            this.listPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel uaList;
        private UpdAter.FlatNoBorder addNew;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Button exitButton;
        private UpdAter.FlatNoBorder updateAll;
        private UpdAter.ImagePanel listPanel;
        private UpdAter.FlatNoBorder credits;
    }
}

