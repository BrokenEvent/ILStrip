namespace ILStripTest
{
  partial class UnusedForm
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
      this.controlOfUnusedForm1 = new ILStripTest.ControlOfUnusedForm();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // controlOfUnusedForm1
      // 
      this.controlOfUnusedForm1.Location = new System.Drawing.Point(143, 69);
      this.controlOfUnusedForm1.Name = "controlOfUnusedForm1";
      this.controlOfUnusedForm1.Size = new System.Drawing.Size(75, 23);
      this.controlOfUnusedForm1.TabIndex = 0;
      this.controlOfUnusedForm1.Text = "controlOfUnusedForm1";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::ILStripTest.Properties.Resources.BrokenEventLogo;
      this.pictureBox1.Location = new System.Drawing.Point(90, 187);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(160, 160);
      this.pictureBox1.TabIndex = 1;
      this.pictureBox1.TabStop = false;
      // 
      // UnusedForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.controlOfUnusedForm1);
      this.Name = "UnusedForm";
      this.Text = "UnusedForm";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private ControlOfUnusedForm controlOfUnusedForm1;
    private System.Windows.Forms.PictureBox pictureBox1;
  }
}