﻿using System;
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
            else if (passBox.TextLength < 8 || passBox.TextLength > 150)
            {
                MessageBox.Show("Password has a minimum of 8 and a maximum of 150");
            }
            else if (!passBox.Text.Any(char.IsUpper))
            {
                MessageBox.Show("Password must contain at least one upper letter");
            }
            else if(!passBox.Text.Any(c => char.IsDigit(c)))
            {
                MessageBox.Show("Password must contain at least one number");
            }
            else
            {
                valid = true;
            }
            if (valid)
            {
                string encryptedPass = db.encryptPass(passBox.Text);
                string encryptedUser = db.encryptPass(userBox.Text);
                
                List<string> columns = new List<string>();
                columns.Add("title");
                columns.Add("user");
                columns.Add("password");
                List<string> values = new List<string>();
                values.Add(titleBox.Text);
                values.Add(encryptedPass);
                values.Add(encryptedUser);

                db.Insert("DatabaseTable", columns, values);
                this.Close();
            }
        }
    }
}