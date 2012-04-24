<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="playerDetail.aspx.cs" Inherits="tournament.playerDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lblplayerName" runat="server">test</asp:Label><BR />
        <asp:Table ID="gameTable" runat="server">
        <asp:TableHeaderRow>
        <asp:TableHeaderCell>Opponent</asp:TableHeaderCell>
        <asp:TableHeaderCell>Result</asp:TableHeaderCell>
        <asp:TableHeaderCell>Move count</asp:TableHeaderCell>
        <asp:TableHeaderCell>Game</asp:TableHeaderCell>
        </asp:TableHeaderRow>
        </asp:Table>
    </div>
    </form>
</body>
</html>
