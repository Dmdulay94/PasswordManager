using System;
using System.Data.SQLite;
using System.Data;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace PasswordManager
{
    public class Crypt
    {

        public string getHash(string key)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            StringBuilder hash = new StringBuilder();
            byte[] hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(key));
            foreach (byte b in hashValue)
            {
                hash.Append(b.ToString("x2"));
            }
            return hash.ToString();
        }

        private byte[] leftPad(byte[] arr)
        {
            var newArray = new byte[16];
            var startAt = newArray.Length - arr.Length;
            Buffer.BlockCopy(arr, 0, newArray, startAt, arr.Length);
            return newArray;
        }

        public string EncryptString(string toEncrypt, string key)
        {
            byte[] encryptionKey = Encoding.ASCII.GetBytes(key);
            if (encryptionKey.Length < 16)
            {
                encryptionKey = leftPad(encryptionKey);   
            }
            if (string.IsNullOrEmpty(toEncrypt)) throw new ArgumentException("toEncrypt");
            if (encryptionKey == null || encryptionKey.Length == 0) throw new ArgumentException("encryptionKey");
            var toEncryptBytes = Encoding.UTF8.GetBytes(toEncrypt);
            using (var provider = new AesCryptoServiceProvider())
            {
                provider.Key = encryptionKey;
                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;
                using (var encryptor = provider.CreateEncryptor(provider.Key, provider.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(provider.IV, 0, 16);
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(toEncryptBytes, 0, toEncryptBytes.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        public string DecryptString(string encryptedString, string key)
        {
            byte[] encryptionKey = Encoding.ASCII.GetBytes(key);
            if (encryptionKey.Length < 16)
            {
                encryptionKey = leftPad(encryptionKey);
            }
            using (var provider = new AesCryptoServiceProvider())
            {
                provider.Key = encryptionKey;
                using (var ms = new MemoryStream(Convert.FromBase64String(encryptedString)))
                {
                    // Read the first 16 bytes which is the IV.
                    byte[] iv = new byte[16];
                    ms.Read(iv, 0, 16);
                    provider.IV = iv;
                    provider.Padding = PaddingMode.PKCS7;

                    using (var decryptor = provider.CreateDecryptor())
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }


    }

    public class SQLiteDatabase
    {
        //String Variable for Database Connection
        SQLiteConnection dbConn;
        Crypt c;
        string dbConnection;
        string password;
        //Basic Constructor
        public SQLiteDatabase()
        {
            dbConnection = "Data Source = pw.s3db";
        }
        //SQLite Constructor (@params = (String) InputFile)
        public SQLiteDatabase(string inputFile, string pw, string calledFrom)
        {
            //dbConnection = String.Format("Data Source = {0}", inputFile);
            dbConnection = "Data Source="+inputFile+";Version=3;";
            if(calledFrom == "login")
            {
                dbConnection += "Password=" + pw;
            } else if (calledFrom == "create")
            {
                dbConn = new SQLiteConnection(dbConnection);
                dbConn.Open();
                dbConn.ChangePassword(pw);
                dbConn.Close();
                dbConnection += "Password=" + pw;
            } else
            {
                throw new ArgumentException();
            }
            dbConn = new SQLiteConnection(dbConnection);
            this.password = pw;
            c = new Crypt();
        }

        //SQLite Constructor (@params = (Dictionary) Connection Options)
        public SQLiteDatabase(Dictionary<string, string> connectionsOpts)
        {
            string str = "";
            foreach (KeyValuePair<string, string> row in connectionsOpts)
            {
                str += string.Format("{0}={1};", row.Key, row.Value);
            }
            str = str.Trim().Substring(0, str.Length - 1);
            dbConnection = str;
        }

        public string getHash(string password)
        {
            return c.getHash(password);
        }

        public string decryptPass(string encryptPass)
        {
            return c.DecryptString(encryptPass, password);
        }

        public bool checkPass()
        {
            bool isCorrect = false;
            try
            {
                dbConn.Open();
                isCorrect = true;
                dbConn.Close();
            }
            catch
            {
            }
            /*DataTable pw = this.GetDataTable("select * from System");
            var table = pw.AsEnumerable().ToArray();
            DataRow[] x = table;
            if ((table[0][0]).ToString().Equals(c.getHash(this.password)))
            {
                isCorrect = true;
            }
            */
            return isCorrect;
        }

        public string encryptPass(string plaintextPass)
        {
            return c.EncryptString(plaintextPass, password);
        }

        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                dbConn.Open();
                SQLiteCommand cmd = new SQLiteCommand(dbConn);
                cmd.CommandText = sql;
                SQLiteDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                dbConn.Close();
            }
            catch (Exception e)
            {
               throw new Exception(e.Message);
            }

            return dt;
        }

        public int ExecuteNonQuery(string sql)
        {
            dbConn.Open();
            SQLiteCommand cmd = new SQLiteCommand(sql, dbConn);
            //cmd.CommandText = sql;
            int rowsUpdated = cmd.ExecuteNonQuery();
            dbConn.Close();
            return rowsUpdated;
        }

        public string ExecuteScalar(string sql)
        {
            SQLiteConnection cnn = dbConn;
            cnn.Open();
            SQLiteCommand cmd = new SQLiteCommand(cnn);
            cmd.CommandText = sql;
            object value = cmd.ExecuteScalar();
            cnn.Close();
            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }

        public bool Update(String tableName, Dictionary<String, String> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
                this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }
            return returnCode;
        }

        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        public bool Insert(String tableName, Dictionary<String, String> data)
        {
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        public bool Insert(string tableName, List<string> columnList ,List<string> valueList)
        {
            string columns = "";
            string values = "";
            bool returnCode = true;
            foreach (string val in columnList)
            {
                columns += String.Format(" '{0}',", val.ToString());
            }
            foreach (string val in valueList)
            {
                values += String.Format(" '{0}',", val.ToString());
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /*public bool changePassword(string str)
        {
            bool result = false;
            try
            {
                String tempString = dbConnection + "Password=" + str +";";
                dbConn = new SQLiteConnection(tempString);
                dbConn.ChangePassword(str);
                result = true;
                this.password = str;
                dbConn.Open();
            }
            catch
            {
                result = false;
            }
            finally
            {
                dbConn.Close();
            }
            return result;
        }

        public bool setPassword(string str)
        {
            bool result = false;
            dbConn.SetPassword(str);
            this.password = str;
            try
            {
                dbConn.Open();
                using (SQLiteCommand command = new SQLiteCommand("PRAGMA schema_version;", dbConn))
                {
                    var ret = command.ExecuteScalar();
                }
                result = true;
            }
            catch(SQLiteException)
            {
                MessageBox.Show("Password Incorrect");
                result = false;
            }
            finally
            {
                dbConn.Close();
            }
            return result;
        }

        public bool setPassword(string str, SQLiteConnection s)
        {
            try
            {
                s.SetPassword(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
        */
        public bool ClearDB()
        {
            DataTable tables;
            try
            {
                tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    this.ClearTable(table["NAME"].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ClearTable(String table)
        {
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Crypt c = new Crypt();
            string password = "p";
            Console.WriteLine(password);
            Console.WriteLine(c.getHash(password));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

}
