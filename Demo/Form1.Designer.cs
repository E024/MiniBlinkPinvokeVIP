namespace Demo
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.blinkBrowser1 = new MiniBlinkPinvokeVIP.BlinkBrowser();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.返回ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.前进ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重新加载ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.调用JS方法ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // blinkBrowser1
            // 
            this.blinkBrowser1.ContextMenuStrip = this.contextMenuStrip1;
            this.blinkBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blinkBrowser1.DocumentIsReady = false;
            this.blinkBrowser1.ID = new System.Guid("f9e40a78-bcb6-416c-8664-158b400455b8");
            this.blinkBrowser1.Location = new System.Drawing.Point(0, 0);
            this.blinkBrowser1.Name = "blinkBrowser1";
            this.blinkBrowser1.ResourceGcIntervalSec = 3600;
            this.blinkBrowser1.Size = new System.Drawing.Size(800, 450);
            this.blinkBrowser1.TabIndex = 0;
            this.blinkBrowser1.Text = "blinkBrowser1";
            this.blinkBrowser1.Title = null;
            this.blinkBrowser1.Url = "https://www.baidu.com/";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.返回ToolStripMenuItem,
            this.前进ToolStripMenuItem,
            this.重新加载ToolStripMenuItem,
            this.调用JS方法ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 92);
            // 
            // 返回ToolStripMenuItem
            // 
            this.返回ToolStripMenuItem.Name = "返回ToolStripMenuItem";
            this.返回ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.返回ToolStripMenuItem.Text = "返回";
            // 
            // 前进ToolStripMenuItem
            // 
            this.前进ToolStripMenuItem.Name = "前进ToolStripMenuItem";
            this.前进ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.前进ToolStripMenuItem.Text = "前进";
            // 
            // 重新加载ToolStripMenuItem
            // 
            this.重新加载ToolStripMenuItem.Name = "重新加载ToolStripMenuItem";
            this.重新加载ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.重新加载ToolStripMenuItem.Text = "重新加载";
            // 
            // 调用JS方法ToolStripMenuItem
            // 
            this.调用JS方法ToolStripMenuItem.Name = "调用JS方法ToolStripMenuItem";
            this.调用JS方法ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.调用JS方法ToolStripMenuItem.Text = "调用JS方法";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.blinkBrowser1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MiniBlinkPinvokeVIP.BlinkBrowser blinkBrowser1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 返回ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 前进ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重新加载ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 调用JS方法ToolStripMenuItem;
    }
}

