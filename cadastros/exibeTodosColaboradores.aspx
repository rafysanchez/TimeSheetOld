<%@ Page Language="vb" EnableEventValidation ="false" AutoEventWireup="false" CodeBehind="exibeTodosColaboradores.aspx.vb" Inherits="IntranetVB.exibeTodosColaboradores" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lista de colaboradores cadastrados</title>
</head>
<body>
    <form id="form1" runat="server">
        <p>
            <asp:Button id="btnExportar" onclick="exportar" Runat="server" Text="Exportar para o Excel"></asp:Button>
        </p>
        <p>
            <asp:Datagrid id="dg" Font-Names="Arial" Height="148px" 
                GridLines="None" cellpadding="4" Headerstyle-BackColor="#0000FF" 
                Headerstyle-Forecolor="#FFFFFF" Headerstyle-Font-Name="Arial" 
                Headerstyle-Font-Size="8" Font-Name="Arial" Font-Size="8pt" Runat="server" 
                ForeColor="#333333">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <EditItemStyle BackColor="#2461BF" />
                <SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <AlternatingItemStyle BackColor="White" />
                <ItemStyle BackColor="#EFF3FB" />
                <HeaderStyle font-size="9pt" font-names="Arial" forecolor="White" 
                    backcolor="#507CD1" Height="20" Font-Bold="True"></HeaderStyle>
            </asp:Datagrid>
            <br />
            <font face="Trebuchet MS" size="2"><strong></strong></font>
        </p>
        <center>
            <asp:Label ID="lblMensagem" runat="server"></asp:Label>
        </center>
    </form>
</body>
</html>
