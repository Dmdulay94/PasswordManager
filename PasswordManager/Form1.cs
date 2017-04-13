using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class Form1 : Form
    {
        SQLiteDatabase sql;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string txt2 = textBox2.Text;
            Boolean login = true;
            //Need Login Validation Function Here - Return True if Login Successful, Else declare incorrect password
            string file = FileDirectory.Text.ToString();
            sql = new SQLiteDatabase(file);
            if (login)
            {
                if (file.Length > 1)
                {
                    this.Hide();
                    Form2 f2 = new Form2(sql);
                    f2.Closed += (s, args) => this.Close();
                    f2.Show();
                }
                else
                {
                    MessageBox.Show("No Database Loaded");
                }
            }
            else
            {
                MessageBox.Show("Incorrect Password");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            DialogResult result = of.ShowDialog();
            if (result == DialogResult.OK)
                {
                FileDirectory.Text = of.FileName.ToString();
                }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void FileDirectory_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
