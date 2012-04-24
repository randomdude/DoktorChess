<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scoreboard.aspx.cs" Inherits="tournament.scoreboard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <asp:Table ID="Leaderboard" runat="server" BorderStyle="Inset" BorderWidth="1px">
        <asp:TableRow ID="TableRow1" runat="server" BorderStyle=Dotted BorderColor="Black"> 
            <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">Rank</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="TableHeaderCell4" runat="server">Name</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="TableHeaderCell2" runat="server">Score</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="TableHeaderCell5" runat="server">Wins</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="TableHeaderCell6" runat="server">Losses</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="TableHeaderCell7" runat="server">Draws</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="TableHeaderCell3" runat="server">Error count</asp:TableHeaderCell>
        </asp:TableRow>
    </asp:Table>

    <asp:Table ID="resultGrid" runat="server" BorderStyle="Inset" BorderWidth="1px"></asp:Table>
    </form>
</body>
</html>
