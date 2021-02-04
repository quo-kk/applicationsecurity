<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="ApplicationSecurityAssignmentFinal.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function validatepwd() {
            var string = document.getElementById('<%=newpassword.ClientID%>').value;
            if (string.length < 8) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password length must be longer than 8 characters";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("too_short");
            }
            else if (string.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password must contain at least one number";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_number");
            }
            else if (string.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 capital letter";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_capital");
            }
            else if (string.search(/[^A-Za-z0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 special character";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_special");
            }
            document.getElementById("lbl_pwdchecker").innerHTML = "Pass";
            document.getElementById("lbl_pwdchecker").style.color = "Blue";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <fieldset>
            <legend>Change Password</legend>
            <div>
            <p>
        <asp:Label ID="Label2" runat="server" Text="Enter Email: "></asp:Label>
        <asp:TextBox ID="email" runat="server"></asp:TextBox>
        <asp:Label ID="emailerror" runat="server" Text="Label" Visible="False"></asp:Label>
                </p>
            <p>
        <asp:Label ID="Label1" runat="server" Text="Enter Current Password: "></asp:Label>
        <asp:TextBox ID="oldpassword" runat="server"></asp:TextBox>
        <asp:Label ID="oldpassworderror" runat="server" Text="Label" Visible="False"></asp:Label>
                </p>
            <p>
        <asp:Label ID="Label11" runat="server" Text="Enter New Password: "></asp:Label>
        <asp:TextBox ID="newpassword" runat="server" onkeyup="javascript:validatepwd();" TextMode="Password"></asp:TextBox>
        <asp:Label ID="newpassworderror" runat="server" Text="Label" Visible="False"></asp:Label>
                <asp:Label ID="lbl_pwdchecker" runat="server" Text="passwordchecker" ></asp:Label>
                </p>
        <p>
            <asp:Label ID="Label5" runat="server" Text="Confirm New Password   "></asp:Label>
            <asp:TextBox ID="confirmpassword" runat="server" TextMode="Password"></asp:TextBox>
            <asp:Label ID="confirmpassworderror" runat="server" Visible="False" Text="Label"></asp:Label>
        </p>

            <asp:Button ID="submitbtn" runat="server" Text="Confirm Password" OnClick="submitbtn_Click" />
        </div>
        </fieldset>
        
    </form>
</body>
</html>
