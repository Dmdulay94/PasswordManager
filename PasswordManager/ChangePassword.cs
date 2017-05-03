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
    public partial class ChangePassword : Form
    {
        SQLiteDatabase sql;
        string fileName;
        public ChangePassword(string file)
        {
            fileName = file;
            InitializeComponent();
        }

        private void changeButton_Click(object sender, EventArgs e)
        {
            if(newPassword.Text == newPasswordConfirm.Text)
            {
                sql = new SQLiteDatabase(fileName, textBox1.Text, "changePassword");
                if (sql.checkPass())
                {
                    DataTable pw = sql.GetDataTable("select * from DatabaseTable");
                    var table = pw.AsEnumerable().ToArray();
                    List<RowRepresentation> rows = new List<RowRepresentation>();
                    foreach (var x in table)
                    {
                        RowRepresentation row = new RowRepresentation();
                        row.id = Int32.Parse(x[0].ToString());
                        row.Title = x[1].ToString();
                        row.UserName = sql.decryptPass(x[2].ToString());
                        row.Password = sql.decryptPass(x[3].ToString());
                        rows.Add(row);
                    }
                    sql.ChangePassword(newPassword.Text);
                    sql = new SQLiteDatabase(fileName, newPassword.Text, "changePassword");
                    foreach(RowRepresentation row in rows)
                    {
                        List<string> values = new List<string>();
                        values.Add(row.Title);
                        values.Add(sql.encryptPass(row.UserName));
                        values.Add(sql.encryptPass(row.Password));
                        values.Add(row.id.ToString());
                        sql.Update(values);
                    }
                    this.Close();
                } else
                {
                    MessageBox.Show("Incorrect password for the database selected.");
                }
            }
        }
    }
}
