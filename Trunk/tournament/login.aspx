<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="tournament.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="style/styles.css" type="text/css" />
    <link rel="stylesheet" href="style/roundedbox.css" type="text/css" />

    <script src="js/json.js" type="text/javascript"></script>
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script src="js/tournament.js" type="text/javascript" ></script>
</head>
<body>
    <form id="form1" runat="server">

        <div class="wideroundbox">
            <div class="title">
                <asp:Label ID="lblTitle" runat="server">Login</asp:Label>
            </div>
        </div>

        <br />
        <br />
        
        <asp:panel runat="server" ID="loginPanel">
            <div class="wideroundbox" >
                <div style="width: 100%" >
                    <table>
                      <tr>
                        <asp:Label ID="lblMsg" runat="server"></asp:Label>
                      </tr>
                      <tr>
                        <td>
                          E-mail address:</td>
                        <td>
                          <asp:TextBox ID="UserEmail" runat="server" />
                        </td>
                      </tr>
                      
                      <tr>
                        <td>
                          Password:</td>
                        <td>
                          <asp:TextBox ID="UserPass" TextMode="Password" 
                             runat="server" />
                        </td>
                      </tr>
                      <tr>
                        <td>
                          Remember me?</td>
                        <td>
                          <asp:CheckBox ID="Persist" runat="server" /></td>
                      </tr>
                    </table>
                    <br />
                    <asp:Button ID="cmdLogon" OnClick="Logon_Click" Text="Log in"  runat="server" />
                </div>
            </div>
        </asp:panel>

        <asp:panel runat="server" ID="changePass">
            <div class="wideroundbox" >
                <div style="width: 100%" >
                    <table>
                      <tr>
                        <asp:Label ID="lblChangePassMsg" runat="server" Font-Bold="True" ></asp:Label>
                      </tr>

                      <tr><td>&nbsp;</td></tr>

                      <tr>
                        <td>
                          Old password:</td>
                        <td>
                          <asp:TextBox ID="txtOldPass" TextMode="Password" runat="server" />
                        </td>
                      </tr>

                      <tr><td>&nbsp;</td></tr>

                      <tr>
                        <td>
                          New password:</td>
                        <td>
                          <asp:TextBox ID="txtNewPass" TextMode="Password"  runat="server" />
                        </td>
                      </tr>

                      <tr><td>&nbsp;</td></tr>

                      <tr>
                        <td>
                          New password (again):</td>
                        <td>
                          <asp:TextBox ID="txtNewPassAgain" TextMode="Password" runat="server" />
                        </td>
                      </tr>

                    </table>
                    <br /> 
                    <div style="text-align: center; width: 100%">
                        <asp:Button ID="cmdChangePass" OnClick="cmdChangePass_Click" Text="Change password"  runat="server" />
                    </div>
                </div>
            </div>
        </asp:panel>

    </form>
</body>
</html>
