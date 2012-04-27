using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace tournament
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["changePass"] == "true")
            {
                changePass.Visible = true;
                loginPanel.Visible = false;
            }
            else
            {
                changePass.Visible = false;
                loginPanel.Visible = true;
            }
        }

        protected void cmdChangePass_Click(object sender, EventArgs e)
        {
            if (txtNewPass.Text != txtNewPassAgain.Text)
            {
                lblChangePassMsg.Text = "Passwords did not match";
                return;
            }

            if (!authData.authUser(Context.User.Identity.Name, txtOldPass.Text))
            {
                lblChangePassMsg.Text = "'Old password' is incorrect";
                return;                
            }

            authData.setPass(Context.User.Identity.Name, txtNewPass.Text);

            Response.Redirect("/scoreboard.aspx");
        }

        protected void Logon_Click(object sender, EventArgs e)
        {
            if (authData.authUser(UserEmail.Text, UserPass.Text))
            {
                FormsAuthentication.RedirectFromLoginPage(UserEmail.Text, Persist.Checked);
            }
            else
            {
                lblMsg.Text = "Invalid credentials. Please try again.";
            }            
        }
    }
}
