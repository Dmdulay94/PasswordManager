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
    //Modify Form
    //Prompts the user to modify the contents of a selected node
    //Once completed, the "Save" Button will send the user back to Form2 and update the contents of the Tree View.
    public partial class Modify : Form
    {
        TreeNode node;
        
        public Modify(TreeNode nodeToModify)
        {
            node = nodeToModify;
            RowRepresentation temp = (RowRepresentation)node.Tag;
            
            InitializeComponent();
            titleBox.Text = temp.Title;
            userBox.Text = temp.UserName;
            passBox.Text = temp.Password;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            RowRepresentation temp = (RowRepresentation)node.Tag;
            temp.Title = titleBox.Text;
            temp.UserName = userBox.Text;
            temp.Password = passBox.Text;
            node.Tag = temp;
            this.Close();
        }

        private void titleBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
