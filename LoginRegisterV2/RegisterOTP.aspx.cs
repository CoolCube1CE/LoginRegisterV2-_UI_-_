using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace LoginRegisterV2
{
    public partial class RegisterOTP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Reg_Email"] == null)
            {
                Response.Redirect("Register.aspx");
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            int generatedOtp = (int)Session["Reg_OTP"];

            if (txtOtp.Text.Trim() != generatedOtp.ToString())
            {
                lblMessage.Text = "Incorrect OTP";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["LoginRegisterDB"].ConnectionString;

            using (SqlConnection sqlConnect = new SqlConnection(connStr))
            {
                try
                {
                    sqlConnect.Open();
                    string insertInfo = "INSERT INTO admin (fullname, birthdate, address, email, number, accesstype, password) " +
                                         "VALUES (@fullname, @birthdate, @address, @email, @number, @accesstype, @password)";

                    using (SqlCommand cmd = new SqlCommand(insertInfo, sqlConnect))
                    {
                        cmd.Parameters.AddWithValue("@fullname", Session["Reg_FullName"]);
                        cmd.Parameters.AddWithValue("@birthdate", Session["Reg_BirthDate"]);
                        cmd.Parameters.AddWithValue("@address", Session["Reg_Address"]);
                        cmd.Parameters.AddWithValue("@email", Session["Reg_Email"]);
                        cmd.Parameters.AddWithValue("@number", Session["Reg_Number"]);
                        cmd.Parameters.AddWithValue("@accesstype", Session["Reg_AccessType"]);
                        cmd.Parameters.AddWithValue("@password", Session["Reg_PasswordHash"]);

                        cmd.ExecuteNonQuery();
                    }

                    // Clear pending registration data
                    Session.Remove("Reg_FullName");
                    Session.Remove("Reg_BirthDate");
                    Session.Remove("Reg_Address");
                    Session.Remove("Reg_Email");
                    Session.Remove("Reg_Number");
                    Session.Remove("Reg_AccessType");
                    Session.Remove("Reg_PasswordHash");
                    Session.Remove("Reg_OTP");

                    Response.Redirect("Login.aspx");
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error saving account: " + ex.Message;
                }
            }
        }
    }
}