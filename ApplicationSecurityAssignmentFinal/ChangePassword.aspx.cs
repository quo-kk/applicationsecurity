using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;

namespace ApplicationSecurityAssignmentFinal
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        static string newfinalHash;
        static string newsalt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void submitbtn_Click(object sender, EventArgs e)
        {
            string userid = email.Text.ToString().Trim();
            string pwd = oldpassword.Text.ToString().Trim();
            string newpwd = newpassword.Text.ToString().Trim();
            //Generate random "salt" 
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltByte = new byte[8];

            //Fills array of bytes with a cryptographically strong sequence of random values.
            rng.GetBytes(saltByte);
            salt = Convert.ToBase64String(saltByte);
            newsalt = Convert.ToBase64String(saltByte);

            SHA512Managed hashing = new SHA512Managed();

            string pwdWithSalt = pwd + salt;
            string newpwdWithSalt = newpwd + salt;
            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
            byte[] newplainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(newpwd));
            byte[] newhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newpwdWithSalt));

            newfinalHash = Convert.ToBase64String(newhashWithSalt);
            finalHash = Convert.ToBase64String(hashWithSalt);


            RijndaelManaged cipher = new RijndaelManaged();
            cipher.GenerateKey();
            Key = cipher.Key;
            IV = cipher.IV;

            LoginMe();

            //if (newpassword.Text == confirmpassword.Text)
            //{ PasswordChange(userid); }
            //else
            //{ confirmpassworderror.Text = "Both Passwords do not match!"; }

        }

        protected void LoginMe()
        {
            string pwd = oldpassword.Text.ToString().Trim();
            string newpwd = newpassword.Text.ToString().Trim();
            string userid = email.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);
            string olddbhash = getoldDBHash(userid);
            string olddbsalt = getoldDBSalt(userid);
            var entrydate = GetDate(userid);
            var dateentry = DateTime.Parse(entrydate);
            var minutes = (DateTime.Now - dateentry).TotalMinutes;

            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = pwd + dbSalt;
                    string newpwdwitholdsalt = newpwd + olddbsalt;
                    
                    byte[] newhashwitholdsalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newpwdwitholdsalt));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string newuserhash = Convert.ToBase64String(newhashwitholdsalt);

                    string userHash = Convert.ToBase64String(hashWithSalt);

                    if (olddbhash != newuserhash)
                    {
                        if (userHash == dbHash)
                        {
                            if (newpassword.Text.ToString() == confirmpassword.Text.ToString())
                            {
                                if (minutes > 5)
                                {
                                    oldpassworderror.Visible = true;
                                    oldpassworderror.Text = "works";
                                    bool changed = PasswordChange(userid);
                                }
                                else
                                {
                                    oldpassworderror.Visible = true;
                                    oldpassworderror.Text = "Can't change password so soon!";
                                }
                            }
                            else {
                                
                                    confirmpassworderror.Text = "Passwords in both textboxes do not match!";
                                    confirmpassworderror.Visible = true;
                                
                            }
                        }
                        else if (userHash != dbHash)
                        {
                            oldpassworderror.Visible = true;
                            oldpassworderror.Text = "Incorrect Username or Password";
                        }
                    }
                    else {
                        oldpassworderror.Text = "Password history error!";
                        oldpassworderror.Visible = true;
                    }
                    

                }
                else if (dbHash == null)
                {
                    emailerror.Visible = true;
                    emailerror.Text = "Incorrect Username or Password";
                }
                else if (dbHash == null && pwd == "")
                {
                    emailerror.Visible = true;
                    emailerror.Text = "Error";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }

        }

        protected bool PasswordChange(string userid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET password_hash = @password_hash, password_salt = @password_salt, Date = @date, old_password_hash = @old_password_hash, old_password_salt = @old_password_salt WHERE email = @Id"))

                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", userid);
                            cmd.Parameters.AddWithValue("@password_hash", newfinalHash);
                            cmd.Parameters.AddWithValue("@password_salt", newsalt);
                            cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                            cmd.Parameters.AddWithValue("@old_password_hash", finalHash);
                            cmd.Parameters.AddWithValue("@old_password_salt", salt);
                            cmd.Connection = con;

                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                var errormsg = "An error has occured! Please confirm your input and try again.";
                            }
                            finally
                            {
                                con.Close();
                                Response.Redirect("Login.aspx", false);
                            }

                            //con.Open();
                            //cmd.ExecuteNonQuery();
                            //con.Close();


                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return true;
        }

        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select password_hash FROM Account WHERE email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["password_hash"] != null)
                        {
                            if (reader["password_hash"] != DBNull.Value)
                            {
                                h = reader["password_hash"].ToString();
                            }
                        }
                        if (reader["password_hash"] == null)
                        {
                            Label1.Text = "Invalid";
                            h = "1";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getoldDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select old_password_hash FROM Account WHERE email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["old_password_hash"] != null)
                        {
                            if (reader["old_password_hash"] != DBNull.Value)
                            {
                                h = reader["old_password_hash"].ToString();
                            }
                        }
                        if (reader["old_password_hash"] == null)
                        {
                            Label1.Text = "Invalid";
                            h = "1";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getoldDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select old_password_salt FROM ACCOUNT WHERE email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {


                    while (reader.Read())
                    {
                        if (reader["old_password_salt"] != null)
                        {
                            if (reader["old_password_salt"] != DBNull.Value)
                            {
                                s = reader["old_password_salt"].ToString();
                            }
                        }
                        if (reader["old_password_salt"] == null)
                        {
                            Label1.Text = "Invalid";
                            s = "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select password_salt FROM ACCOUNT WHERE email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {


                    while (reader.Read())
                    {
                        if (reader["password_salt"] != null)
                        {
                            if (reader["password_salt"] != DBNull.Value)
                            {
                                s = reader["password_salt"].ToString();
                            }
                        }
                        if (reader["password_salt"] == null)
                        {
                            Label1.Text = "Invalid";
                            s = "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected string decryptData(byte[] cipherText)
        {

            string decryptedString = null;
            //byte[] cipherText = Convert.FromBase64String(cipherString);

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                //Decrypt
                //byte[] decryptedText = decryptTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);
                //decryptedString = Encoding.UTF8.GetString(decryptedText);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { }
            return decryptedString;
        }

        protected string GetDate(string userid)
        {
            string a = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Date FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Date"] != null)
                        {
                            if (reader["Date"] != DBNull.Value)
                            {
                                a = reader["Date"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return a;
        }
    }
}