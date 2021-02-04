using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace ApplicationSecurityAssignmentFinal
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void submitbtn_Click(object sender, EventArgs e)
        {
            bool test = ValidateInput();

            if (test == true)
            {

                string pwd = password.Text.ToString().Trim();
                //Generate random "salt" 
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];

                //Fills array of bytes with a cryptographically strong sequence of random values.
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);

                SHA512Managed hashing = new SHA512Managed();

                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                finalHash = Convert.ToBase64String(hashWithSalt);

                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;

                createAccount();
            }

        }

        protected bool ValidateInput()
        {
            var validated = true;
            if (firstname.Text == "") 
            {
                firstnameerror.Visible = true;
                validated = false;
                firstnameerror.Text = "Field cannot be empty";
            }
            if (lastname.Text == "")
            {
                lastnameerror.Visible = true;
                validated = false;
                lastnameerror.Text = "Field cannot be empty";
            }
            if (creditcardnum.Text == "")
            {
                creditcarderror.Visible = true;
                validated = false;
                creditcarderror.Text = "Field cannot be left empty";
            }
            if (creditcardcvc.Text == "")
            {
                cardnumerror.Visible = true;
                validated = false;
                cardnumerror.Text = "Field cannot be left empty";
            }
            if (creditcardexp.Text == "")
            {
                carddateerror.Visible = true;
                validated = false;
                carddateerror.Text = "Field cannot be left empty";
            }
            if (email.Text == "")
            {
                emailerror.Visible = true;
                validated = false;
                emailerror.Text = "Field cannot be left empty";
            }
            if (password.Text == "")
            {
                passworderror.Visible = true;
                validated = false;
                passworderror.Text = "Field cannot be left empty";
            }
            if (confirmpwd.Text == "")
            {
                confirmpwderror.Visible = true;
                validated = false;
                confirmpwderror.Text = "Field cannot be left empty";
            }
            if (password.Text != confirmpwd.Text)
            {
                passworderror.Visible = true;
                validated = false;
                passworderror.Text = "Passwords do not match!";
            }
            if (dateofbirth.Text == "")
            {
                dateofbirtherror.Visible = true;
                validated = false;
                dateofbirtherror.Text = "Field cannot be left empty";
            }
            if (password.Text.ToString() != confirmpwd.Text.ToString())
            {
                passworderror.Text = "Passwords in both textboxes do not match!";
                validated = false;
                passworderror.Visible = true;
            }
            try
            {
                DateTime.Parse(dateofbirth.Text);
                int passwordstrength = checkPassword(password.Text);
                if (passwordstrength < 5) {
                    passworderror.Text = "Password is too weak, please try again!";
                    passworderror.Visible = true;
                    validated = false;
                }
                bool emailindb = GetEmail();
                if (emailindb == false)
                {
                    emailerror.Text = "Email already in use!";
                    validated = false;
                }
            }
            catch 
            {
                dateofbirtherror.Visible = true;
                dateofbirtherror.Text = "Wrong date format";    
            }

            return validated;
        }

        private int checkPassword(string password)
        {
            int score = 0;
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }
            return score;
        }

        protected void checkpassword_Click(object sender, EventArgs e)
        {
            int scores = checkPassword(password.Text);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
              
            if (scores < 4)
            {
                
                return;
            }
                
        }

        protected void createAccount()
        {

            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@first_name, @last_name,@email,@credit_card_no,@credit_card_cvc,@credit_card_date,@password_hash,@password_salt,@dob,@IV,@Key,@LoginAttempts,@Date,@old_password_hash,@old_password_salt)"))
                   
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@first_name", firstname.Text.Trim());
                            cmd.Parameters.AddWithValue("@last_name", lastname.Text.Trim());
                            cmd.Parameters.AddWithValue("@email", email.Text.Trim());
                            cmd.Parameters.AddWithValue("@credit_card_no", Convert.ToBase64String(encryptData(creditcardnum.Text.Trim())));
                            cmd.Parameters.AddWithValue("@credit_card_cvc", creditcardcvc.Text.Trim());
                            cmd.Parameters.AddWithValue("@credit_card_date", creditcardexp.Text.Trim());
                            cmd.Parameters.AddWithValue("@password_hash", finalHash);
                            cmd.Parameters.AddWithValue("@password_salt", salt);
                            cmd.Parameters.AddWithValue("@dob", DateTime.Parse(dateofbirth.Text.Trim()));
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@LoginAttempts", 0);
                            cmd.Parameters.AddWithValue("@Date",DateTime.Now);
                            cmd.Parameters.AddWithValue("@old_password_hash", "");
                            cmd.Parameters.AddWithValue("@old_password_salt", "");
                            cmd.Connection = con;

                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                var errormsg = "An error has occured! Please confirm your input and try again.";
                                firstnameerror.Text = errormsg;//ex.ToString();
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
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        protected bool GetEmail()
        {
            string a = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Email FROM Account";
            SqlCommand command = new SqlCommand(sql, connection);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != null)
                        {
                            if (reader["Email"] != DBNull.Value)
                            {
                                a = reader["Email"].ToString();
                                if (a.ToLower() == email.Text.ToString().ToLower()) 
                                {
                                    emailerror.Text = "Email is already in use!";
                                    return false;
                                }
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
            return true;
        }

    }
}