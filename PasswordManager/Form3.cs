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
        String databaseStringBuilder;
        String tempPassword;
        String confirm;


        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tempPassword == confirm)
            {
                databaseStringBuilder = FileDirectory.Text + "\\" + textBox2.Text + ".db";
                SQLiteConnection.CreateFile(databaseStringBuilder); 
                sql = new SQLiteDatabase(databaseStringBuilder,tempPassword);

                String cmd = "create table DatabaseTable(id INTEGER PRIMARY KEY AUTOINCREMENT, title VARCHAR2(25) NOT NULL, user VARCHAR2(150) NOT NULL, password VARCHAR2(150) NOT NULL);";
                sql.ExecuteNonQuery(cmd);
                cmd = "create table System(System VARCHAR2(150) PRIMARY KEY);";
                sql.ExecuteNonQuery(cmd);
                cmd = String.Format("insert into System values('{0}')", sql.getHash(tempPassword));
                sql.ExecuteNonQuery(cmd);

                string x = "password";
                byte[] y = sql.encryptPass(x);
                StringBuilder z = new StringBuilder();
                foreach (byte b in y)
                {
                    z.Append(b.ToString("x2"));
                }
                Console.WriteLine(z.ToString());

                string hy = sql.decryptPass(y);
                Console.WriteLine(hy);

                Form2 form = new Form2(sql);
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
    }
}
