using System.Windows.Forms;

namespace AutoClick
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.StartButton = new System.Windows.Forms.Button();
            this.ButtonStop = new System.Windows.Forms.Button();
            this.checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.HandleGridView = new System.Windows.Forms.DataGridView();
            this.句柄ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.TimeInterval = new System.Windows.Forms.ComboBox();
            this.StatusLable = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.HandleGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(148, 211);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(66, 23);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // ButtonStop
            // 
            this.ButtonStop.Location = new System.Drawing.Point(223, 211);
            this.ButtonStop.Name = "ButtonStop";
            this.ButtonStop.Size = new System.Drawing.Size(66, 23);
            this.ButtonStop.TabIndex = 2;
            this.ButtonStop.Text = "Stop";
            this.ButtonStop.UseVisualStyleBackColor = true;
            this.ButtonStop.Click += new System.EventHandler(this.ButtonStop_Click);
            // 
            // checkedListBox
            // 
            this.checkedListBox.FormattingEnabled = true;
            this.checkedListBox.Items.AddRange(new object[] {
            "按F1",
            "按F2",
            "按F3",
            "按F4",
            "按F5",
            "按F6",
            "按F7",
            "按F8",
            "按空格键"});
            this.checkedListBox.Location = new System.Drawing.Point(148, 12);
            this.checkedListBox.Name = "checkedListBox";
            this.checkedListBox.Size = new System.Drawing.Size(141, 196);
            this.checkedListBox.TabIndex = 3;
            // 
            // HandleGridView
            // 
            this.HandleGridView.AllowUserToAddRows = false;
            this.HandleGridView.AllowUserToDeleteRows = false;
            this.HandleGridView.AllowUserToResizeColumns = false;
            this.HandleGridView.AllowUserToResizeRows = false;
            this.HandleGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.HandleGridView.ColumnHeadersVisible = false;
            this.HandleGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.句柄ID});
            this.HandleGridView.Location = new System.Drawing.Point(12, 12);
            this.HandleGridView.MultiSelect = false;
            this.HandleGridView.Name = "HandleGridView";
            this.HandleGridView.ReadOnly = true;
            this.HandleGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.HandleGridView.RowTemplate.Height = 23;
            this.HandleGridView.Size = new System.Drawing.Size(130, 196);
            this.HandleGridView.TabIndex = 5;
            this.HandleGridView.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.m_gridControl_CellMouseUp);
            // 
            // 句柄ID
            // 
            this.句柄ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.句柄ID.HeaderText = "句柄ID";
            this.句柄ID.Name = "句柄ID";
            this.句柄ID.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 216);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "间隔(毫秒)";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 238);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(210, 34);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "Ctrl+Alt+A获取进程，+B开始定时器，+C取消定时器，同时可直接按键。";
            // 
            // TimeInterval
            // 
            this.TimeInterval.FormattingEnabled = true;
            this.TimeInterval.Items.AddRange(new object[] {
            "100",
            "200",
            "500",
            "1000",
            "1500",
            "2000",
            "2500",
            "3000",
            "5000",
            "10000"});
            this.TimeInterval.Location = new System.Drawing.Point(81, 213);
            this.TimeInterval.Name = "TimeInterval";
            this.TimeInterval.Size = new System.Drawing.Size(61, 20);
            this.TimeInterval.TabIndex = 10;
            this.TimeInterval.Text = "1000";
            // 
            // StatusLable
            // 
            this.StatusLable.AutoSize = true;
            this.StatusLable.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StatusLable.Location = new System.Drawing.Point(239, 238);
            this.StatusLable.Name = "StatusLable";
            this.StatusLable.Size = new System.Drawing.Size(40, 16);
            this.StatusLable.TabIndex = 11;
            this.StatusLable.Text = "闲置";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(239, 260);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "嗨起来";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 277);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.StatusLable);
            this.Controls.Add(this.TimeInterval);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.HandleGridView);
            this.Controls.Add(this.checkedListBox);
            this.Controls.Add(this.ButtonStop);
            this.Controls.Add(this.StartButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "自动点击";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.HandleGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button ButtonStop;
        private System.Windows.Forms.CheckedListBox checkedListBox;
        private System.Windows.Forms.DataGridView HandleGridView;
        private Label label1;
        private TextBox textBox1;
        private ComboBox TimeInterval;
        private DataGridViewTextBoxColumn 句柄ID;
        private Label StatusLable;
        private Label label3;
    }
}

