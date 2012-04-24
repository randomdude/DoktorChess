<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="overview.aspx.cs" Inherits="tournament._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="style/styles.css" type="text/css" />
    <link rel="stylesheet" href="style/roundedbox.css" type="text/css" />

    <script src="js/json.js" type="text/javascript"></script>
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script src="js/doktorChess.js" type="text/javascript" ></script>        
</head>
<body>
    <form id="form1" runat="server">
        <div>            
            <br />
            <asp:Button ID="Button1" runat="server" Text="Start new tournament" onclick="Button1_Click" /><br />

            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label><br />
            <asp:Label ID="Label1" runat="server" Text="Games waiting in queue:"></asp:Label><br />
            <asp:Table ID="tblQueuedGames" runat="server">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>White</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Black</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table><br />
            <asp:Label ID="Label2" runat="server" Text="Currently playing games:"></asp:Label><br />
            <asp:Label ID="lblCurrentGameSummary" runat="server" Text="Currently playing games:"></asp:Label><br />
            <asp:Label ID="lblCurrentGame" runat="server" Text=""></asp:Label><br />
        </div>
    </form>
</body>
</html>
