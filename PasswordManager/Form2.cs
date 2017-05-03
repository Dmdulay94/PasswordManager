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
        TreeNode selectedNode;

        //Form2 accepts the authenticated connection from Form1
        //sql will be used to generated nodes in Tree View that contain all the password information in the database
        public Form2(SQLiteDatabase db)
        {
            sql = db;
            InitializeComponent();
        }

        //On the creation of Form2 - Create the nodes necessary for Tree View
        //Queries the database for every entry in the DatabaseTable, inserts data into a DataTable
        //table parses the DataTable into an array of DataRows
        //Then each node of the Tree View is generated in a foreach function call where each node's properties are appropriately set and added to the tree.
        private void Form2_Load(object sender, EventArgs e)
        {
            TreeNode treeNode;
            DataTable pw = sql.GetDataTable("select * from DatabaseTable");
            var table = pw.AsEnumerable().ToArray();
            foreach(var x in table)
            {
                RowRepresentation row = new RowRepresentation();
                row.id = Int32.Parse(x[0].ToString());
                row.Title = x[1].ToString();
                row.UserName = sql.decryptPass(x[2].ToString());
                row.Password = sql.decryptPass(x[3].ToString());
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
            selectedNode = e.Node;
            textBox1.Text = ((RowRepresentation)selectedNode.Tag).UserName;
            textBox2.Text = ((RowRepresentation)selectedNode.Tag).Password;
        }

        //Queries the database for every entry in the DatabaseTable, inserts data into a DataTable
        //table parses the DataTable into an array of DataRows
        //Only adds nodes that have not previously been entered into the Tree View, other nodes remain untouched and duplicates avoided.
        private void UpdateTreeView()
        {
            DataTable pw = sql.GetDataTable("select * from DatabaseTable");
            var table = pw.AsEnumerable().ToArray();
            IEnumerable<TreeNode> nodes = treeView1.Nodes.Cast<TreeNode>();
            foreach(var row in table)
            {
                var match = nodes.Where(node => ((RowRepresentation)node.Tag).id.ToString() == row[0].ToString());
                if (!match.Any())
                {
                    RowRepresentation newNodeTag = new RowRepresentation();
                    newNodeTag.id = Int32.Parse(row[0].ToString());
                    newNodeTag.Title = row[1].ToString();
                    newNodeTag.UserName = sql.decryptPass(row[2].ToString());
                    newNodeTag.Password = sql.decryptPass(row[3].ToString());
                    TreeNode newNode = new TreeNode(row[1].ToString());
                    newNode.Tag = newNodeTag;

                    treeView1.Nodes.Add(newNode);
                }
            }
        }

        //Clicking a node on the Tree View will update the Username and Password textboxes with their respective decrypted version
        //All elements of the Tree View are stored as nodes of the tree to be accessed as the user needs them. Nodes have properties which contain the username, password, etc.
        //Textbox1 is the Username Textbox
        //Textbox2 is the Password Textbox
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TextBox txt = textBox1;
            TextBox txt2 = textBox2;
            txt.Text = ((RowRepresentation)selectedNode.Tag).UserName;
            txt2.Text = ((RowRepresentation)selectedNode.Tag).Password;
        }

        //Calls the Add Form to allow the user to create a new password
        private void addNew_Click(object sender, EventArgs e)
        {
            //open new form with title, user, and password fields
            //when okay is clicked, create new treenode and add to treeview, then update DB with new field
            Add add = new Add(sql);
            add.Closed += (s, args) => this.UpdateTreeView();
            add.Show();
        }

        //Calls the Modify Form to allow modification of entries in the database
        private void modifySelected_Click(object sender, EventArgs e)
        {
            //open new form and populate title, username, and password fields with current values
            //after fields are edited and OK is pressed, close form and update TreeView node + SQLite DB
            Modify modify = new Modify(selectedNode);
            modify.Closed += (s, args) => this.modifyAndUpdate();
            modify.Show();
        }

        //Updates the database and nodes with the newly modified contents of a particular chosen node
        //This is called in the modifySelected_Click Function when the Modify Form is closed
        private void modifyAndUpdate()
        {
            RowRepresentation modified = (RowRepresentation)selectedNode.Tag;
            foreach (TreeNode node in treeView1.Nodes)
            {
                
                RowRepresentation row = (RowRepresentation)node.Tag;
                if (modified.id == row.id)
                {
                    if (row.Title != node.Text)
                    {
                        node.Text = row.Title;
                    }
                    if (textBox1.Text != row.UserName)
                    {
                        textBox1.Text = row.UserName;
                    }
                    if (textBox2.Text != row.Password)
                    {
                        textBox2.Text = row.Password;
                    }
                    break;
                }
            }
            List<string> updateList = new List<string>();
            updateList.Add(modified.Title);
            updateList.Add(sql.encryptPass(modified.UserName));
            updateList.Add(sql.encryptPass(modified.Password));
            updateList.Add(modified.id.ToString());
            sql.Update(updateList);

        }

        //Deletes a node from the database and the Tree View when the "Delete" button is clicked
        private void deleteSelected_Click(object sender, EventArgs e)
        {
            RowRepresentation row = (RowRepresentation)selectedNode.Tag;
            sql.Delete("DatabaseTable", "id = " + row.id.ToString());
            treeView1.Nodes.Remove(selectedNode);
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        //When Form2 is being closed, the database connection will be terminated
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.DisposeSQLite();
        }
    }
}
