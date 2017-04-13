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
            /*string[] x = new string[3];
            x[0] = "Cat1";
            x[1] = "Cat2";
            x[2] = "Cat3";
            foreach (string y in x)
            {
              treeNode = new TreeNode(y);
              treeView1.Nodes.Add(treeNode);
            }
            TreeNode node2 = new TreeNode("C#");
            TreeNode node3 = new TreeNode("VB.NET");
            TreeNode[] array = new TreeNode[] { node2, node3 };
            treeNode = new TreeNode("Dot Net Perls", array);
            treeView1.Nodes.Add(treeNode);*/
            DataTable pw = sql.GetDataTable("select * from t");
            var table = pw.AsEnumerable().ToArray();
            x = table;
            foreach(var x in table)
            {
                treeNode = new TreeNode((x[0]).ToString());
                treeView1.Nodes.Add(treeNode);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }


        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            var table = x.AsEnumerable().ToArray();
            string u = " ", p = " ";
            foreach (var x in table)
            {
                if ((x[0]).ToString() == node.Text)
                {
                    u = (x[0]).ToString();
                    p = (x[1]).ToString();
                }
            }
            TextBox txt = textBox1;
            TextBox txt2 = textBox2;
            txt.Text = u;
            txt2.Text = p;
        }
    }
}
