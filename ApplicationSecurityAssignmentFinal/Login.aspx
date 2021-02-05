<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ApplicationSecurityAssignmentFinal.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LctcOUZAAAAAN0BbTIC_HC1EJvzsJCWDEDyFSzL"></script>
</head>
<body>
        <script>
            grecaptcha.ready(function () {
                grecaptcha.execute('6LctcOUZAAAAAN0BbTIC_HC1EJvzsJCWDEDyFSzL', { action: 'Login' }).then(function (token) {
                    document.getElementById("g-captcha-response").value = token;
                });
            });
        </script>
    <form id="form1" runat="server">
        <div>

            <fieldset>
                <legend>Login</legend>
                
                <p>Login Attempts: <asp:Label ID="attemptslabel" runat="server" Text="0"></asp:Label></p>
                
                <p>Username: <asp:TextBox ID="tb_userid" runat="server"></asp:TextBox></p>
                <p>Password: <asp:TextBox ID="tb_pwd" runat="server" TextMode="Password"></asp:TextBox></p>
                <p>
                    <asp:Button ID="btnSubmit" runat="server" Text="Login" OnClick="LoginMe" />
                    <asp:Button ID="resetattempts" runat="server" Text="Reset" OnClick="ResetAttempts" />
                    <asp:Button ID="changepassword" runat="server" Text="Change Password" OnClick="passwordchangeredirect" />
                    <asp:Button ID="register" runat="server" Text="Register Account" OnClick="registerredirect" />
                <br />
                <br />
                <asp:Label ID="lblMessage" runat="server" Visible="false">Error message here</asp:Label>
                    
            </p>
                    </fieldset>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
        </div>
    </form>

</body>
</html>
