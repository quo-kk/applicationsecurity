<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="ApplicationSecurityAssignmentFinal.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
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
        <asp:TextBox ID="newpassword" runat="server"></asp:TextBox>
        <asp:Label ID="newpassworderror" runat="server" Text="Label" Visible="False"></asp:Label>
                </p>
        <p>
            <asp:Label ID="Label5" runat="server" Text="Email Address   "></asp:Label>
            <asp:TextBox ID="confirmpassword" runat="server"></asp:TextBox>
            <asp:Label ID="confirmpassworderror" runat="server" Text="Label"></asp:Label>
        </p>

            <asp:Button ID="submitbtn" runat="server" Text="Confirm Password" OnClick="submitbtn_Click" />
        </div>
    </form>
</body>
</html>
