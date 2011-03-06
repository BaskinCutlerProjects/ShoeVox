namespace ShoeVox
{
    partial class Form1
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

                if (null != processWatcher)
                {
                    processWatcher.ProcessEvent -= new System.EventHandler<ProcessEventArgs>(processWatcher_ProcessEvent);
                    processWatcher.Dispose();
                }
                if (null != engine)
                {
                    engine.CommandRecognized -= new System.EventHandler<CommandRecognizedEventArgs>(engine_CommandRecognized);
                    engine.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.listenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runOnStartupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prefixMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipTitle = "ShoeVox";
            this.notifyIcon.ContextMenuStrip = this.trayMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "ShoeVox";
            this.notifyIcon.Visible = true;
            // 
            // trayMenu
            // 
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listenMenuItem,
            this.runOnStartupMenuItem,
            this.prefixMenuItem,
            this.toolStripSeparator1,
            this.aboutMenuItem,
            this.exitMenuItem});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.Size = new System.Drawing.Size(154, 142);
            // 
            // listenMenuItem
            // 
            this.listenMenuItem.Checked = true;
            this.listenMenuItem.CheckOnClick = true;
            this.listenMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.listenMenuItem.Name = "listenMenuItem";
            this.listenMenuItem.Size = new System.Drawing.Size(153, 22);
            this.listenMenuItem.Text = "Listen";
            this.listenMenuItem.Click += new System.EventHandler(this.listenMenuItem_Click);
            // 
            // runOnStartupMenuItem
            // 
            this.runOnStartupMenuItem.CheckOnClick = true;
            this.runOnStartupMenuItem.Name = "runOnStartupMenuItem";
            this.runOnStartupMenuItem.Size = new System.Drawing.Size(153, 22);
            this.runOnStartupMenuItem.Text = "Run on Startup";
            this.runOnStartupMenuItem.Click += new System.EventHandler(this.runOnStartupMenuItem_Click);
            // 
            // prefixMenuItem
            // 
            this.prefixMenuItem.Name = "prefixMenuItem";
            this.prefixMenuItem.Size = new System.Drawing.Size(153, 22);
            this.prefixMenuItem.Text = "Set Prefix";
            this.prefixMenuItem.Click += new System.EventHandler(this.prefixMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(150, 6);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(153, 22);
            this.aboutMenuItem.Text = "About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(153, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 207);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.Text = "ShoeVox";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.trayMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listenMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem prefixMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runOnStartupMenuItem;
    }
}

