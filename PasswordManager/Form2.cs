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
    public partial class Form2 : Form
    {
        SQLiteDatabase sql;
        DataRow[] x;
        TreeNode selectedNode;

        public Form2(SQLiteDatabase db)
        {
            sql = db;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Close();
            TreeNode treeNode;
            DataTable pw = sql.GetDataTable("select * from DatabaseTable");
            var table = pw.AsEnumerable().ToArray();
            x = table;
            foreach(var x in table)
            {
                RowRepresentation row = new RowRepresentation();
                row.id = Int32.Parse(x[0].ToString());
                row.Title = x[1].ToString();
                row.UserName = x[2].ToString();
                row.Password = x[3].ToString();
                treeNode = new TreeNode((x[1]).ToString());
                treeNode.Tag = row;
                treeView1.Nodes.Add(treeNode);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedNode = treeView1.SelectedNode;
        }


        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TextBox txt = textBox1;
            TextBox txt2 = textBox2;
            txt.Text = ((RowRepresentation)selectedNode.Tag).UserName;
            txt2.Text = ((RowRepresentation)selectedNode.Tag).Password;
        }

        private void addNew_Click(object sender, EventArgs e)
        {
            //open new form with title, user, and password fields
            //when okay is clicked, create new treenode and add to treeview, then update DB with new field
        }

        private void modifySelected_Click(object sender, EventArgs e)
        {
            //open new form and populate title, username, and password fields with current values
            //after fields are edited and OK is pressed, close form and update TreeView node + SQLite DB
        }

        private void deleteSelected_Click(object sender, EventArgs e)
        {
            RowRepresentation row = (RowRepresentation)selectedNode.Tag;
            sql.Delete("DatabaseTable", "id = " + row.id.ToString());
            treeView1.Nodes.Remove(selectedNode);
        }
    }
}
