<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Web.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="Button1" runat="server" Text="Application Insightsログ出力試験" OnClick="Button1_Click" />
            <asp:Button ID="Button2" runat="server" Text="App Service再起動試験" OnClick="Button2_Click" />
            <asp:Button ID="Button3" runat="server" Text="App Service再起動試験2" OnClick="Button3_Click" />
        </div>
    </form>
</body>
</html>
