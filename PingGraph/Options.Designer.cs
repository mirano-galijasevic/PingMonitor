namespace PingGraph
{
    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            this.numDelay = new System.Windows.Forms.NumericUpDown();
            this.numRoundtripTreshold = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoundtripTreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(388, 231);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 34);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Address or IP:";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(15, 43);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(130, 20);
            this.txtAddress.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(151, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Address or IP to ping";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(151, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Max time to wait for request to return";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Maroon;
            this.label4.Location = new System.Drawing.Point(12, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Ping timeout (ms):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Gray;
            this.label5.Location = new System.Drawing.Point(151, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(202, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Delay between subsequent ping requests";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Maroon;
            this.label6.Location = new System.Drawing.Point(12, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Ping delay (sec):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Gray;
            this.label7.Location = new System.Drawing.Point(151, 194);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(301, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "If roundtrip is above this treshold, then it is considered too slow";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Maroon;
            this.label8.Location = new System.Drawing.Point(12, 174);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(118, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Roundtrip treshold (ms):";
            // 
            // numTimeout
            // 
            this.numTimeout.Location = new System.Drawing.Point(15, 92);
            this.numTimeout.Maximum = new decimal(new int[] {
            12000,
            0,
            0,
            0});
            this.numTimeout.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(130, 20);
            this.numTimeout.TabIndex = 13;
            this.numTimeout.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // numDelay
            // 
            this.numDelay.Location = new System.Drawing.Point(15, 141);
            this.numDelay.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numDelay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDelay.Name = "numDelay";
            this.numDelay.Size = new System.Drawing.Size(130, 20);
            this.numDelay.TabIndex = 14;
            this.numDelay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numRoundtripTreshold
            // 
            this.numRoundtripTreshold.Location = new System.Drawing.Point(15, 192);
            this.numRoundtripTreshold.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numRoundtripTreshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRoundtripTreshold.Name = "numRoundtripTreshold";
            this.numRoundtripTreshold.Size = new System.Drawing.Size(130, 20);
            this.numRoundtripTreshold.TabIndex = 15;
            this.numRoundtripTreshold.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 277);
            this.Controls.Add(this.numRoundtripTreshold);
            this.Controls.Add(this.numDelay);
            this.Controls.Add(this.numTimeout);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoundtripTreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numTimeout;
        private System.Windows.Forms.NumericUpDown numDelay;
        private System.Windows.Forms.NumericUpDown numRoundtripTreshold;
    }
}