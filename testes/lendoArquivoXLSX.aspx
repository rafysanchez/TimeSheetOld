<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="lendoArquivoXLSX.aspx.vb" Inherits="IntranetVB.lendoArquivoXLSX" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="font-family:Arial;">

<form id="form1" runat="server">
    
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>    
    
    <div style="padding-left:40px;">
                        
        <p>
           <asp:Label runat="server" id="lblMensagem" Text=""></asp:Label>
        </p>
        
        <br />  

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gridCEP" runat="server">
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>       
                       
    </div>

</form>

</body>
</html>
