namespace PasswordManager
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
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.FileLookupButton = new System.Windows.Forms.Button();
            this.FileDirectory = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 65);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(250, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Password:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(77, 39);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new System.Drawing.Size(185, 20);
            this.textBox2.TabIndex = 6;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // FileLookupButton
            // 
            this.FileLookupButton.Location = new System.Drawing.Point(16, 12);
            this.FileLookupButton.Name = "FileLookupButton";
            this.FileLookupButton.Size = new System.Drawing.Size(23, 23);
            this.FileLookupButton.TabIndex = 7;
            this.FileLookupButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.FileLookupButton.UseVisualStyleBackColor = true;
            this.FileLookupButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // FileDirectory
            // 
            this.FileDirectory.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.FileDirectory.Location = new System.Drawing.Point(45, 13);
            this.FileDirectory.Name = "FileDirectory";
            this.FileDirectory.Size = new System.Drawing.Size(217, 20);
            this.FileDirectory.TabIndex = 8;
            this.FileDirectory.TextChanged += new System.EventHandler(this.FileDirectory_TextChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 104);
            this.Controls.Add(this.FileDirectory);
            this.Controls.Add(this.FileLookupButton);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button FileLookupButton;
        private System.Windows.Forms.TextBox FileDirectory;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

