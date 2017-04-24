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
            DataTable pw = sql.GetDataTable("select * from DatabaseTable");
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
