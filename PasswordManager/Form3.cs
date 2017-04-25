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

        private void button1_Click(object sender, EventArgs e)
        {
            if (tempPassword == confirm)
            {
                databaseStringBuilder = FileDirectory.Text + "\\" + textBox2.Text + ".db3";
                SQLiteConnection.CreateFile(databaseStringBuilder); 
                sql = new SQLiteDatabase(databaseStringBuilder,tempPassword, "create");

                string cmd = "create table DatabaseTable(id INTEGER PRIMARY KEY AUTOINCREMENT, title VARCHAR2(25) NOT NULL, user VARCHAR2(150) NOT NULL, password VARCHAR2(150) NOT NULL);";
                sql.ExecuteNonQuery(cmd);
                cmd = "create table System(System VARCHAR2(150) PRIMARY KEY);";
                sql.ExecuteNonQuery(cmd);
                cmd = string.Format("insert into System values('{0}')", sql.getHash(tempPassword));
                sql.ExecuteNonQuery(cmd);

                Form2 form = new Form2(sql);
                form.Closed += (s, args) => this.Close();
                form.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Passwords Do Not Match");
            }
        }

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

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            tempPassword = textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            confirm = textBox4.Text;
        }
        private void form_Closed(object sender, EventArgs e)
        {
        }
    }
}
