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
    //Add Form
    //Form used to create new passwords to be stored into the database
    public partial class Add : Form
    {
        SQLiteDatabase db;
        public Add(SQLiteDatabase db)
        {
            this.db = db;
            InitializeComponent();
        }

        private void Add_Load(object sender, EventArgs e)
        {

        }
        
        //Add Button
        //Performs text checks before inserting the encrypted version of the username/password into the database
        //If all the user plaintext meets criteria, the strings will be encrypted and submitted into the database
        //Otherwise a Messagebox with the appropriate error will be shown
        private void addButton_Click(object sender, EventArgs e)
        {
            bool valid = false;
            if(titleBox.TextLength == 0 || titleBox.TextLength > 25)
            {
                MessageBox.Show("Title must be not empty and shorter than 25");
            }
            else if(userBox.TextLength == 0 || userBox.TextLength > 150)
            {
                MessageBox.Show("User must not be empty and shorter than 150");
            }
            else if (passBox.TextLength > 150)
            {
                MessageBox.Show("Password has a maximum of 150");
            }
            else
            {
                valid = true;
            }
            if (valid)
            {
                string encryptedPass = db.encryptPass(passBox.Text);
                string encryptedUser = db.encryptPass(userBox.Text);
                List<string> values = new List<string>();
                values.Add(titleBox.Text);
                values.Add(encryptedUser);
                values.Add(encryptedPass);

                db.Insert(values);
                this.Close();
            }
        }
    }
}
