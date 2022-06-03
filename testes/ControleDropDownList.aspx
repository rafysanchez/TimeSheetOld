<%@ Page Title="" Language="vb" AutoEventWireup="true" MasterPageFile="~/MasterPage.Master" CodeBehind="ControleDropDownList.aspx.vb" Inherits="IntranetVB.ControleDropDownList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="tig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server">

    <script language="JavaScript" type="text/javascript" src="../recursos/jQuery/jquery-1.6.2.min.js"></script>
    <script type="text/javascript">
        function exibe() {
            alert("Uhuuuu!!!");
        }
        // Função para alternar entre show/hide
        $(function() {
            $('#ComboBox1').click(function() {
                exibe();
                $('#myDiv').toggle();
            });
        });
    </script>

    <form runat="server" ID="frmReport">
        
        <tig:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </tig:ToolkitScriptManager>
                
        <tig:ComboBox ID="ComboBox1" runat="server">
            <asp:ListItem>Opção 1</asp:ListItem>
            <asp:ListItem>Opção 2</asp:ListItem>
        </tig:ComboBox>
        
        <div id="myDiv">Teste</div>
        
        <br />
        
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Label ID="lblMensagem" runat="server" Text=""></asp:Label>
            </ContentTemplate>
        </asp:UpdatePanel>        
        
    </form>

</asp:Content>
