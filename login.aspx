<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="campus_app.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Login - Campus App</title>
    <link href="~/assets/css/login.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-box">
            <h2>Silakan Login</h2>
            
            <div class="form-group">
                <label>Username:</label>
                <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label>Password:</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
            </div>
            
            <asp:Button ID="btnLogin" runat="server" Text="Masuk" CssClass="btn" OnClick="btnLogin_Click" />
            
            <asp:Label ID="lblError" runat="server" CssClass="error" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>