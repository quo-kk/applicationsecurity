<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="ApplicationSecurityAssignmentFinal.HomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">  
    <title></title>  
    <script lang="javascript" >  
        var tim;
        var seconds = 60;
        var dt = new Date();

        /*function showcurrenttime() { 
        countdown(); 
        document.getElementById("starttime").innerHTML = "The Time Now is " + dt.getHours() + ":" + dt.getMinutes(); 
        }*/
        function countdown() {
            document.getElementById("mydiv").style.visibility = "hidden";
            if (parseInt(seconds) > 0) {
                seconds = parseInt(seconds) - 1;
                document.getElementById("showtime").innerHTML = "Your Left Time  is :" + seconds + " Seconds";
                tim = setTimeout("countdown()", 1000);
            }
            else {
                if (parseInt(seconds) == 0) {
                    if (parseInt(seconds) == 0) {
                        clearTimeout(tim);
                        document.getElementById("showtime").style.visibility = "hidden";
                        document.getElementById("mydiv").style.visibility = "visible";
                        location.href = "Login.aspx";  
                    }
                }
            }
        }
    </script>  
</head>
    <body onload="countdown()">
        <form id="form1" runat="server">
    <fieldset>
       
            <legend>HomePage</legend>

               <br />
            
        
            <asp:Label ID="lblMessage" runat="server" EnabledViewState="False" />
            <br />
            <br />
        
        <asp:Button ID="btnLogout" runat="server" Text="Log Out" OnClick="LogoutMe" Visible="false"/>
        </fieldset>
            </div>


            <div>      
            <div id="showtime"></div>  
              <div id="mydiv">  
                  <h3>Time Up!</h3>  
              </div>  
          
       
   </div> 


    </form>
    
</body>
</html>
