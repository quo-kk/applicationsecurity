using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;

namespace ApplicationSecurityAssignmentFinal
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=6LctcOUZAAAAAFHbie_xJ9rOaAhktOl7QzV5MTjb &response=" + captchaResponse);

            try
            {

                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        lblMessage.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = true; //Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected void LoginMe(object sender, EventArgs e)
        {
            string pwd = tb_pwd.Text.ToString().Trim();
            string userid = tb_userid.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);

            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash == dbHash)
                    {
                        if (ValidateCaptcha())
                        {

                            Session["LoggedIn"] = tb_userid.Text.Trim();

                            string guid = Guid.NewGuid().ToString();
                            Session["AuthToken"] = guid;

                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                            string attempts = GetLoginAttempts(userid);

                            if (Int64.Parse(attempts) < 3)
                            {
                                Response.Redirect("HomePage.aspx", false);
                            }
                            else if (Int64.Parse(attempts) >= 3)
                            {
                                lblMessage.Text = "Exceeded Login Attempts";
                            }
                        }
                    }
                    else if (userHash != dbHash)
                    {
                        ChangeLoginAttempts(userid);
                        
                        lblMessage.Text = "Incorrect Password";
                        
                        string attempts = GetLoginAttempts(userid);

                        attemptslabel.Text = attempts;

                        if (Int64.Parse(attempts) >= 3)
                        {
                            lblMessage.Text = "Exceeded login attempts";
                            attemptslabel.Text = "3";
                        }
                    }
                }
                else if (dbHash == null)
                {
                    lblMessage.Text = "Incorrect Username";
                }
                else if (dbHash == null && pwd == "") 
                {
                    lblMessage.Text = "Error";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
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

        protected bool ChangeLoginAttempts(string userid)
        {
            string attempts = GetLoginAttempts(userid);
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LoginAttempts = @LoginAttempts WHERE email = @Id"))

                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", userid);
                            cmd.Parameters.AddWithValue("@LoginAttempts", Int64.Parse(attempts) + 1);
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

        protected bool ResetLoginAttempts(string userid)
        {
            string attempts = GetLoginAttempts(userid);
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LoginAttempts = @LoginAttempts WHERE email = @Id"))

                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", userid);
                            cmd.Parameters.AddWithValue("@LoginAttempts", 0);
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
        protected string GetLoginAttempts(string userid)
        {
            string a = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LoginAttempts FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LoginAttempts"] != null)
                        {
                            if (reader["LoginAttempts"] != DBNull.Value)
                            {
                                a = reader["LoginAttempts"].ToString();
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

        protected void ResetAttempts(object sender, EventArgs e)
        {
            string userid = tb_userid.Text.ToString().Trim();
            ResetLoginAttempts(userid);
        }
    }
}