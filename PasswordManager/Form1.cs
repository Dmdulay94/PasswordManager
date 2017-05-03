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
        String password;

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
            string file = FileDirectory.Text.ToString();
            try
            {
                sql = new SQLiteDatabase(file,textBox2.Text, "login");
                if (sql.checkPass())
                {
                    FileDirectory.Text = "";
                    textBox2.Text = "";
                    this.Hide();
                    Form2 f2 = new Form2(sql);
                    f2.Closed += (s, args) => this.Show();
                    f2.Show();
                }
                else
                {
                    MessageBox.Show("Incorrect Password, Please Try Again.");
                }
            }
            catch
            {
                MessageBox.Show("Bad File");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            password = textBox2.Text;
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

        private void button2_Click_1(object sender, EventArgs e)
        {
            Form3 form = new Form3();
            form.Closed += (s, args) => this.Show();
            this.Hide();
            form.Show();
        }

        private void modifyPassword_Click(object sender, EventArgs e)
        {
            if(FileDirectory.Text == "")
            {
                MessageBox.Show("Please select a database file.");
            } else
            {
                ChangePassword form = new ChangePassword(FileDirectory.Text);
                form.Closed += (s, args) => this.Show();
                this.Hide();
                form.Show();
            }
            //open a modify dbpassword form with current password, new password, confirm password
            //get all usernames and passwords, unencrypt them with the current password, reencrypt them
            //with new password, and update the table
        }
    }
}
