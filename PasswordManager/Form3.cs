using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class Form3 : Form
    {
        SQLiteDatabase sql;
        string databaseStringBuilder;
        string tempPassword;
        string confirm;


        public Form3()
        {
            InitializeComponent();
        }

        //Button1 - Create Database Button
        //Provided a location has been chosen and passwords match, create the file with extension .db3 and database for storage
        //Return to Form2 to allow user to create passwords to be saved.
        private void button1_Click(object sender, EventArgs e)
        {
            if (tempPassword == confirm)
            {
                try
                {
                    databaseStringBuilder = FileDirectory.Text + "\\" + textBox2.Text + ".db3";
                    SQLiteConnection.CreateFile(databaseStringBuilder);
                    sql = new SQLiteDatabase(databaseStringBuilder, tempPassword, "create");

                    string cmd = "create table DatabaseTable(id INTEGER PRIMARY KEY AUTOINCREMENT, title VARCHAR2(25) NOT NULL, user VARCHAR2(150) NOT NULL, password VARCHAR2(150) NOT NULL);";
                    sql.ExecuteNonQuery(cmd);

                    Form2 form = new Form2(sql);
                    form.Closed += (s, args) => this.Close();
                    form.Show();
                    this.Hide();
                }
                catch (System.UnauthorizedAccessException)
                {
                    MessageBox.Show("Access is Denied, Choose another location");
                }
            }
            else
            {
                MessageBox.Show("Passwords Do Not Match");
            }
        }

        //Button2 - Location Button
        //When clicked, the File Directory prompt appears for the user to decide where to save the file
        //On "OK", the neighboring Textbox, FileDirectory, is filled with the path of the directory
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog o = new FolderBrowserDialog();
            DialogResult result = o.ShowDialog();
            OpenFileDialog of = new OpenFileDialog();
            if (result == DialogResult.OK)
            {
                of.InitialDirectory = o.SelectedPath.ToString();
                databaseStringBuilder = of.InitialDirectory.ToString();
                FileDirectory.Text = databaseStringBuilder;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        //Textbox3 - Database Password Textbox
        //Password to be set for the Database File
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            tempPassword = textBox3.Text;
        }
        //Textbox4 - Database Confirmation Password Textbox
        //Allows the system to validate that the passwords are the same prior to changing the database password
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            confirm = textBox4.Text;
        }
        private void form_Closed(object sender, EventArgs e)
        {
        }
    }
}
