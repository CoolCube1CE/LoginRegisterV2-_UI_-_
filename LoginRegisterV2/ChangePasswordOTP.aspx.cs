using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LoginRegisterV2
{
    public partial class ChangePasswordOTP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserEmail"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                SendOtp();
            }
        }

        private void SendOtp()
        {
            Random random = new Random();
            int otp = random.Next(100000, 1000000);
            Session["ChPW_OTP"] = otp;

            bool sent = EmailHelper.SendOtpEmail(
                Session["UserEmail"].ToString(), otp,
                "Password Change Verification",
                "Please enter this code to continue changing your password.");

            if (!sent)
            {
                lblMessage.Text = "Unable to send verification e-mail.";
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            int generatedOtp = (int)Session["ChPW_OTP"];

            if (txtOtp.Text.Trim() == generatedOtp.ToString())
            {
                Response.Redirect("ChangePassword.aspx");
            }
            else
            {
                lblMessage.Text = "Incorrect OTP";
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            string accessType = Session["AccessType"]?.ToString();
            if (accessType == "Super Admin") Response.Redirect("SuperAdmin.aspx");
            else if (accessType == "Admin") Response.Redirect("AdminPage.aspx");
            else Response.Redirect("UserHome.aspx");
        }
    }
}