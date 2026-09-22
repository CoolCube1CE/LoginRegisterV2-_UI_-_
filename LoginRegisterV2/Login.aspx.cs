using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace LoginRegisterV2
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (email == "" || password == "")
            {
                lblMessage.Text = "Please fill in all the fields";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["LoginRegisterDB"].ConnectionString;

            using (SqlConnection sqlConnect = new SqlConnection(connStr))
            {
                try
                {
                    sqlConnect.Open();
                    string selectAccount = "SELECT password, accesstype FROM admin WHERE email = @email";

                    using (SqlCommand command = new SqlCommand(selectAccount, sqlConnect))
                    {
                        command.Parameters.AddWithValue("@email", email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["password"].ToString();
                                string accessType = reader["accesstype"].ToString();

                                if (PasswordHelper.VerifyPassword(password, storedHash))
                                {
                                    Session["UserEmail"] = email;
                                    Session["AccessType"] = accessType;

                                    if (accessType == "Super Admin")
                                        Response.Redirect("SuperAdmin.aspx");
                                    else if (accessType == "Admin")
                                        Response.Redirect("AdminPage.aspx");
                                    else
                                        Response.Redirect("UserHome.aspx");
                                }
                                else
                                {
                                    lblMessage.Text = "Incorrect Username/Password";
                                }
                            }
                            else
                            {
                                lblMessage.Text = "Incorrect Username/Password";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Connection Error: " + ex.Message;
                }
            }
        }

        protected void lnkRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("Register.aspx");
        }
    }
}