namespace MSR.Tools.Visualizer
{
	partial class VisualizerView
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
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cleanUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.automaticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cleanUpNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.visualizationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.statusBar.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusBar
			// 
			this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText});
			this.statusBar.Location = new System.Drawing.Point(0, 440);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(784, 22);
			this.statusBar.TabIndex = 0;
			// 
			// statusText
			// 
			this.statusText.Name = "statusText";
			this.statusText.Size = new System.Drawing.Size(0, 17);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.visualizationsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(784, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openConfigToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openConfigToolStripMenuItem
			// 
			this.openConfigToolStripMenuItem.Name = "openConfigToolStripMenuItem";
			this.openConfigToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.openConfigToolStripMenuItem.Text = "Open config...";
			this.openConfigToolStripMenuItem.Click += new System.EventHandler(this.openConfigToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scaleToolStripMenuItem,
            this.cleanUpToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// scaleToolStripMenuItem
			// 
			this.scaleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logXToolStripMenuItem,
            this.logYToolStripMenuItem});
			this.scaleToolStripMenuItem.Name = "scaleToolStripMenuItem";
			this.scaleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.scaleToolStripMenuItem.Text = "Scale";
			// 
			// logXToolStripMenuItem
			// 
			this.logXToolStripMenuItem.Name = "logXToolStripMenuItem";
			this.logXToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.logXToolStripMenuItem.Text = "Log X";
			this.logXToolStripMenuItem.Click += new System.EventHandler(this.logXToolStripMenuItem_Click);
			// 
			// logYToolStripMenuItem
			// 
			this.logYToolStripMenuItem.Name = "logYToolStripMenuItem";
			this.logYToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.logYToolStripMenuItem.Text = "Log Y";
			this.logYToolStripMenuItem.Click += new System.EventHandler(this.logYToolStripMenuItem_Click);
			// 
			// cleanUpToolStripMenuItem
			// 
			this.cleanUpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.automaticallyToolStripMenuItem,
            this.cleanUpNowToolStripMenuItem});
			this.cleanUpToolStripMenuItem.Name = "cleanUpToolStripMenuItem";
			this.cleanUpToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.cleanUpToolStripMenuItem.Text = "Clean up";
			// 
			// automaticallyToolStripMenuItem
			// 
			this.automaticallyToolStripMenuItem.Name = "automaticallyToolStripMenuItem";
			this.automaticallyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.automaticallyToolStripMenuItem.Text = "Automatically";
			this.automaticallyToolStripMenuItem.Click += new System.EventHandler(this.automaticallyToolStripMenuItem_Click);
			// 
			// cleanUpNowToolStripMenuItem
			// 
			this.cleanUpNowToolStripMenuItem.Name = "cleanUpNowToolStripMenuItem";
			this.cleanUpNowToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.cleanUpNowToolStripMenuItem.Text = "Clean up now";
			this.cleanUpNowToolStripMenuItem.Click += new System.EventHandler(this.cleanUpNowToolStripMenuItem_Click);
			// 
			// visualizationsToolStripMenuItem
			// 
			this.visualizationsToolStripMenuItem.Name = "visualizationsToolStripMenuItem";
			this.visualizationsToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
			this.visualizationsToolStripMenuItem.Text = "Visualizations";
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(784, 416);
			this.panel1.TabIndex = 2;
			// 
			// VisualizerView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 462);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "VisualizerView";
			this.Text = "Visualizer";
			this.statusBar.ResumeLayout(false);
			this.statusBar.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusBar;
		private System.Windows.Forms.ToolStripStatusLabel statusText;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openConfigToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem scaleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem logXToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem logYToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cleanUpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem automaticallyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cleanUpNowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem visualizationsToolStripMenuItem;
		private System.Windows.Forms.Panel panel1;
	}
}