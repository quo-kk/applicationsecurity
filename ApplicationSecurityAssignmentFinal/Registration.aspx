<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="ApplicationSecurityAssignmentFinal.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function validatemail() {
            var string = document.getElementById('<%=email.ClientID%>').value;
            if (string.search(/^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/) == -1) {
                document.getElementById("emailerror").style.visibility = "visible";
                document.getElementById("emailerror").innerHTML = "Please enter a valid email address";
                document.getElementById("emailerror").style.color = "Red";
                return ("not_valid");
            }
            document.getElementById("emailerror").innerHTML = "Pass";
            document.getElementById("emailerror").style.color = "Blue";
        }
        function validatepwd() {
            var string = document.getElementById('<%=password.ClientID%>').value;
            if (string.length < 8)
            {
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
    <form id="registration" runat="server">
        <fieldset>
            <div>
            
            <asp:Label ID="Label1" runat="server" Text="Account Registration"></asp:Label>

            <br />
            <br />

        </div>
        <asp:Label ID="Label2" runat="server" Text="First Name  "></asp:Label>
        <asp:TextBox ID="firstname" runat="server"></asp:TextBox>
        <asp:Label ID="firstnameerror" runat="server" Text="Label" Visible="False"></asp:Label>
        <p>
            <asp:Label ID="Label3" runat="server" Text="Last Name   "></asp:Label>
            <asp:TextBox ID="lastname" runat="server" Height="22px"></asp:TextBox>
            <asp:Label ID="lastnameerror" runat="server" Text="Label" Visible="False"></asp:Label>
        </p>
            <p>
        <asp:Label ID="Label4" runat="server" Text="Credit Card Number  "></asp:Label>
        <asp:TextBox ID="creditcardnum" runat="server"></asp:TextBox>
        <asp:Label ID="creditcarderror" runat="server" Text="Label" Visible="False"></asp:Label>
                </p>
            <p>
        <asp:Label ID="Label9" runat="server" Text="Credit Card CVC  "></asp:Label>
        <asp:TextBox ID="creditcardcvc" runat="server"></asp:TextBox>
        <asp:Label ID="cardnumerror" runat="server" Text="Label" Visible="False"></asp:Label>
                </p>
            <p>
        <asp:Label ID="Label11" runat="server" Text="Credit Card Expiry Date  "></asp:Label>
        <asp:TextBox ID="creditcardexp" runat="server"></asp:TextBox>
        <asp:Label ID="carddateerror" runat="server" Text="Label" Visible="False"></asp:Label>
                </p>
        <p>
            <asp:Label ID="Label5" runat="server" Text="Email Address   "></asp:Label>
            <asp:TextBox ID="email" runat="server" onkeyup="javascript:validatemail()"></asp:TextBox>
            <asp:Label ID="emailerror" runat="server" Text="emailchecker"></asp:Label>
        </p>
        <p>
        <asp:Label ID="Label6" runat="server" Text="Password    "></asp:Label>
        <asp:TextBox ID="password" runat="server" onkeyup="javascript:validatepwd();" TextMode="Password"></asp:TextBox>
            <asp:Label ID="passworderror" runat="server" Text="Label" Visible="False"></asp:Label>
            <asp:Label ID="lbl_pwdchecker" runat="server" Text="passwordchecker" ></asp:Label>
        </p>
                <p>
        <asp:Label ID="Label8" runat="server" Text="Confirm Password    "></asp:Label>
        <asp:TextBox ID="confirmpwd" runat="server" TextMode="Password"></asp:TextBox>
        <asp:Label ID="confirmpwderror" runat="server" Text="Label" Visible="False"></asp:Label>

        </p>
        <p>
            <asp:Label ID="Label7" runat="server" Text="Date of Birth   "></asp:Label>
            <asp:TextBox ID="dateofbirth" runat="server"></asp:TextBox>
            <asp:Label ID="dateofbirtherror" runat="server" Text="Label" Visible="False"></asp:Label>
        </p>
        <asp:Button ID="submitbtn" runat="server" Text="Submit" OnClick="submitbtn_Click" />
    </form>
</fieldset>
</body>
</html>

