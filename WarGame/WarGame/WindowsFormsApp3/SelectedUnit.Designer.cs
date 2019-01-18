namespace WindowsFormsApp3
{
	partial class SelectedUnitHUD
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.nameLbl = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.BackColor = System.Drawing.SystemColors.Control;
			this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(3, 100);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(164, 67);
			this.listBox1.TabIndex = 0;
			// 
			// nameLbl
			// 
			this.nameLbl.AutoSize = true;
			this.nameLbl.Location = new System.Drawing.Point(63, 2);
			this.nameLbl.Name = "nameLbl";
			this.nameLbl.Size = new System.Drawing.Size(35, 13);
			this.nameLbl.TabIndex = 1;
			this.nameLbl.Text = "label1";
			this.nameLbl.Click += new System.EventHandler(this.nameLbl_Click);
			// 
			// SelectedUnitHUD
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this.nameLbl);
			this.Controls.Add(this.listBox1);
			this.Name = "SelectedUnitHUD";
			this.Size = new System.Drawing.Size(170, 170);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Label nameLbl;
	}
}
