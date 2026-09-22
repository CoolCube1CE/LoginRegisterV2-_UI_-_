using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace LoginRegisterV2
{
    public partial class Register : System.Web.UI.Page
    {
        private const int MinLength = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private bool Is18(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age >= 18;
        }

        private bool IsPasswordStrong(string password)
        {
            bool length = password.Length >= 8 && password.Length <= 16;
            bool upper = Regex.IsMatch(password, @"[A-Z]");
            bool lower = Regex.IsMatch(password, @"[a-z]");
            bool number = Regex.IsMatch(password, @"[0-9]");
            bool special = Regex.IsMatch(password, @"[^a-zA-Z0-9]");
            return length && upper && lower && number && special;
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            bool valid = true;
            lblGeneralError.Text = "";

            // Full name — letters, spaces, periods, commas only
            if (txtFullName.Text == "")
            { lblFullNameError.Text = "Full Name is required"; valid = false; }
            else if (!Regex.IsMatch(txtFullName.Text, @"^[a-zA-Z ,.]+$"))
            { lblFullNameError.Text = "Invalid, must only contain letters, periods, and commas"; valid = false; }
            else if (txtFullName.Text.Length < MinLength)
            { lblFullNameError.Text = "Full Name is too short"; valid = false; }
            else { lblFullNameError.Text = ""; }

            // Address
            if (txtAddress.Text == "")
            { lblAddressError.Text = "Address is required"; valid = false; }
            else if (!Regex.IsMatch(txtAddress.Text, @"^[a-zA-Z0-9 !#$%&'*+\-/=?^_`{|}~,]+$"))
            { lblAddressError.Text = "Invalid address"; valid = false; }
            else if (txtAddress.Text.Length < MinLength)
            { lblAddressError.Text = "Address is too short"; valid = false; }
            else { lblAddressError.Text = ""; }

            // Email
            if (txtEmail.Text == "")
            { lblEmailError.Text = "E-mail is required"; valid = false; }
            else if (!Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z0-9.!#$%&'*+\-/=?^_`{|}~]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            { lblEmailError.Text = "Invalid E-mail"; valid = false; }
            else { lblEmailError.Text = ""; }

            // Phone number
            if (txtNumber.Text == "")
            { lblNumberError.Text = "Phone Number is required"; valid = false; }
            else if (!Regex.IsMatch(txtNumber.Text, @"^09[0-9]{9}$") && !Regex.IsMatch(txtNumber.Text, @"^\+639[0-9]{9}$"))
            { lblNumberError.Text = "Invalid Phone Number. Must be 09XXXXXXXXX or +639XXXXXXXXX"; valid = false; }
            else { lblNumberError.Text = ""; }

            // Birth date — either the calendar textbox or the 3-part manual entry
            DateTime birthDate = DateTime.MinValue;
            bool birthDateValid;

            if (hdnBirthdateMode.Value == "manual")
            {
                int year, month, day;
                if (int.TryParse(txtYear.Text, out year) && int.TryParse(txtMonth.Text, out month) && int.TryParse(txtDay.Text, out day))
                {
                    try { birthDate = new DateTime(year, month, day); birthDateValid = true; }
                    catch { birthDateValid = false; }
                }
                else
                {
                    birthDateValid = false;
                }
            }
            else
            {
                birthDateValid = DateTime.TryParse(txtBirthDate.Text, out birthDate);
            }

            if (!birthDateValid)
            { lblBirthDateError.Text = "Birth date is required"; valid = false; }
            else if (!Is18(birthDate))
            { lblBirthDateError.Text = "You must be at least 18 years old."; valid = false; }
            else { lblBirthDateError.Text = ""; }

            // Password
            if (txtPassword.Text == "")
            { lblPasswordError.Text = "Password is required"; valid = false; }
            else if (!IsPasswordStrong(txtPassword.Text))
            { lblPasswordError.Text = "Password does not meet the requirements above"; valid = false; }
            else { lblPasswordError.Text = ""; }

            // Confirm password
            if (txtConfirmPassword.Text == "")
            { lblConfirmPasswordError.Text = "Confirm Password Required"; valid = false; }
            else if (txtPassword.Text != txtConfirmPassword.Text)
            { lblConfirmPasswordError.Text = "Password does not match"; valid = false; }
            else { lblConfirmPasswordError.Text = ""; }

            // Access type
            if (ddlAccessType.SelectedValue == "")
            { lblAccessTypeError.Text = "Access Type is required"; valid = false; }
            else { lblAccessTypeError.Text = ""; }

            // Data Privacy agreement (server-side check — client-side disabling can be bypassed)
            if (!chkDataPrivacy.Checked)
            {
                lblGeneralError.Text = "You must agree to the Data Privacy Terms and Agreement before registering.";
                valid = false;
            }

            if (!valid) return;

            string connStr = ConfigurationManager.ConnectionStrings["LoginRegisterDB"].ConnectionString;

            using (SqlConnection sqlConnect = new SqlConnection(connStr))
            {
                try
                {
                    sqlConnect.Open();
                    string validateEmail = "SELECT COUNT(*) FROM admin WHERE email = @email";

                    using (SqlCommand cmd = new SqlCommand(validateEmail, sqlConnect))
                    {
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            lblEmailError.Text = "Email is registered already, please use a different e-mail address.";
                            return;
                        }
                    }

                    Random random = new Random();
                    int otp = random.Next(100000, 1000000);

                    bool emailSent = EmailHelper.SendOtpEmail(
                        txtEmail.Text.Trim(), otp,
                        "Registration Verification",
                        "Please enter this code in the registration form to verify your e-mail address.");

                    if (!emailSent)
                    {
                        lblGeneralError.Text = "Registration cannot continue because the verification e-mail could not be sent.";
                        return;
                    }

                    Session["Reg_FullName"] = txtFullName.Text.Trim();
                    Session["Reg_BirthDate"] = birthDate;
                    Session["Reg_Address"] = txtAddress.Text.Trim();
                    Session["Reg_Email"] = txtEmail.Text.Trim();
                    Session["Reg_Number"] = txtNumber.Text.Trim();
                    Session["Reg_AccessType"] = ddlAccessType.SelectedValue;
                    Session["Reg_PasswordHash"] = PasswordHelper.HashPassword(txtPassword.Text);
                    Session["Reg_OTP"] = otp;

                    Response.Redirect("RegisterOTP.aspx");
                }
                catch (Exception ex)
                {
                    lblGeneralError.Text = "Error: " + ex.Message;
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }

}