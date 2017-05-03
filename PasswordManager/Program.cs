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

//PasswordManager Application
//Created by Dewey Mitchem and Devin Dulay
//CMSC 413 - Introduction to Cybersecurity
//Release Date: 5/3/2017

namespace PasswordManager
{
    //Public Encryption Helper Class - Crypt
    //Class is based of the Cryptography DLL provided by C# using WinForms
    //Class contains methods to encrypt/decrypt strings using AES with SHA-256
    public class Crypt
    {
        //leftPad Method - Pads a given array of bytes with 0's on the left hand side.
        //Takes one parameter (arr) - An array of bytes that lack the necessary space requirements
        //Returns Byte Array with 0's padded to meet length requirements of key.
        private byte[] leftPad(byte[] arr)
        {
            var newArray = new byte[16];
            var startAt = newArray.Length - arr.Length;
            Buffer.BlockCopy(arr, 0, newArray, startAt, arr.Length);
            return newArray;
        }

        //EncryptString Method - Encrypts a string with a given initial key. Key is translated from a string into an array of bytes.
        //Takes two parameters - toEncrypt is the string to be encrypted, key is used for encryption/decryption
        //Returns an encrypted string based off the parameters.
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

        //DecryptString Method - Decrypts a encrypted string with a given initial key.
        //Takes two paramters - encryptedString is the ciphertext to be decrypted, key is used for encryption/decryption
        //Returns the plaintext version of the encryptedString
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
    //Public Database Management Helper Class - SQLiteDatabase
    //Class uses the SQLite DLL to assist with operations concerning SQLite Databases
    //Instance of class will manage a connection, handle queries, and their respective returns
    //Based off a generic Database helping class that update, insert, query tables.
    //Includes custom methods to change and confirm valid passwords as well as ensure a connection has been terminated.
    public class SQLiteDatabase
    {
        
        SQLiteConnection dbConn;
        SQLiteCommand cmd;
        Crypt c;
        string dbConnection;
        string password;
        
        public SQLiteDatabase(string inputFile, string pw, string calledFrom)
        {
            dbConnection = "Data Source="+inputFile+";Version=3;";
            if (calledFrom == "login")
            {
                dbConnection += "Password=" + pw;
            }
            else if (calledFrom == "create")
            {
                dbConn = new SQLiteConnection(dbConnection);
                dbConn.Open();
                dbConn.ChangePassword(pw);
                dbConn.Close();
                dbConnection += "Password=" + pw;
            }
            else if (calledFrom == "changePassword")
            {
                dbConnection += "Password=" + pw;
            }
            else
            {
                throw new ArgumentException();
            }
            dbConn = new SQLiteConnection(dbConnection);
            this.password = pw;
            c = new Crypt();
        }

        public void ChangePassword(string newPassword)
        {
            dbConn.Open();
            dbConn.ChangePassword(newPassword);
            dbConn.Close();
        }

        public string decryptPass(string encryptPass)
        {
            return c.DecryptString(encryptPass, password);
        }
        //Kills connection and relinquishes lock on db file
        public void DisposeSQLite()
        {
            dbConn.Dispose();
            try {
                cmd.Dispose();
            } catch(Exception e)
            {

            }
            
            GC.Collect();
        }
        //Checkpass calls a test query from the database.
        //If it is encrypted the query will fail and return false, elsewise return true;
        public bool checkPass()
        {
            bool isCorrect = false;
            try
            {

                DataTable sample = this.GetDataTable("select * from DatabaseTable");
                isCorrect = true;
            }
            catch
            {
                this.password = "";
                dbConn.Close();
            }
            return isCorrect;
        }

        public string encryptPass(string plaintextPass)
        {
            return c.EncryptString(plaintextPass, password);
        }

        //Generic Database Method GetDataTable
        //Takes one parameter sql - sql is string that represents a query that the database can run, e.g. "Select * from table"
        //Returns a DataTable that contains the results from a SQL query
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

        //Generic Database Method ExecuteNonQuery
        //Takes one parameter sql - sql is string that represents a query that the database can run that updates or deletes from a table promising no return, e.g. "delete * from table where id = 1"
        //Returns an integer that represents the number of rows updated in the database
        public int ExecuteNonQuery(string sql)
        {
            dbConn.Open();
            SQLiteCommand cmd = new SQLiteCommand(sql, dbConn);
            int rowsUpdated = cmd.ExecuteNonQuery();
            dbConn.Close();
            return rowsUpdated;
        }

        //Generic Database Method ExecuteQuery
        //Takes one parameter sql - sql is string that represents a query that the database can run that updates or deletes from a table promising no return, e.g. "delete * from table where id = 1"
        //Returns an integer that represents the number of rows updated in the database
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
        //Custom Database Method Update
        //Takes one parameter valueList - valueList is a list of strings that need to be updated in the database
        //This method is used to update all the contents of the password manager table when a user changes the database (master) password
        //Returns a boolean determining the success of the operation
        public bool Update(List<string> valueList)
        {

            bool returnCode = true;
            cmd = dbConn.CreateCommand();
            cmd.CommandText = "update DatabaseTable set title = @title, user = @user, password = @password where id = @id";
            for (int i = 0; i < valueList.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        cmd.Parameters.Add("@title", DbType.AnsiString).Value = valueList[0];
                        continue;
                    case 1:
                        cmd.Parameters.Add("@user", DbType.AnsiString).Value = valueList[1];
                        continue;
                    case 2:
                        cmd.Parameters.Add("@password", DbType.AnsiString).Value = valueList[2];
                        continue;
                    case 3:
                        cmd.Parameters.Add("@id", DbType.AnsiString).Value = valueList[3];
                        break;
                }
            }
            try
            {
                dbConn.Open();
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                dbConn.Close();
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        //Generic Database Method Delete
        //Takes two parameters: tableName, where - tableName is targetted table, where is the string that represents the filter of the SQL statement
        //Returns a boolean determining the success of the operation
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

        //Custom Database Method Insert
        //Takes one parameter valueList - valueList is a list of strings that need to be inserted into the database
        //Returns a boolean determining the success of the operation
        public bool Insert(List<string> valueList)
        {
            bool returnCode = true;
            cmd = dbConn.CreateCommand();
            cmd.CommandText = "insert into DatabaseTable (title, user, password) VALUES(@title,@user,@password)";
            for(int i = 0; i < valueList.Count; i++)
            {
                switch(i)
                {
                    case 0:
                        cmd.Parameters.Add("@title", DbType.AnsiString).Value = valueList[0];
                        continue;
                    case 1:
                        cmd.Parameters.Add("@user", DbType.AnsiString).Value = valueList[1];
                        continue;
                    case 2:
                        cmd.Parameters.Add("@password", DbType.AnsiString).Value = valueList[2];
                        break;
                }
            }
            try
            {
                dbConn.Open();
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                dbConn.Close();
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }


        //Generic Database Method ClearDB
        //Clears all the contents from the SQLite Database
        //Returns a boolean representing the success of the operation
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


        //Generic Database Method Delete
        //Takes one parameter table - the string table should represent the table that needs to be truncated of all its contents
        //Returns a boolean representing the success of the operation
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

}
