<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="despesas.aspx.vb" Inherits="IntranetVB.despesas" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Despesas
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script>
    <script type="text/javascript" src="../recursos/jQuery/jquery.maskedinput-1.2.2.js"></script>
    
    <script type="text/javascript">

        $(document).ready(function () {
            $(function () {
                $("[id$=txtDataInicio]").mask("99/99/9999");
                $("[id$=txtDataFim]").mask("99/99/9999");                
            });
        });      
    
    </script>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server">

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
        </asp:ScriptManager>
                
        <div style="font-style: italic; color: #808080; text-align:center; padding-bottom:10px;">
            <h2>Lançamento de despesas<br /></h2>
        </div>

        <div class="h2" style="text-align:left;">
            <strong>Despesas por projeto</strong>
            <hr />
            <br />
        </div>

        <div style="text-align:left; width:900px;">
            <table border="0" cellpadding="3">
                <tr>
                    <td colspan="2">
                        <b style="font-size:12px; color:Gray;">Projeto</b><br />                        
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:DropDownList ID="ddlProjetos" runat="server" Width="440">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width:220px;">
                        <b style="font-size:12px; color:Gray;">Tipo</b>
                    </td>
                    <td>
                        <b style="font-size:12px; color:Gray;">Período</b>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddlTipoDespesas" runat="server" Width="200">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <b style="font-size:10px; color:Gray;">de</b>
                        <asp:TextBox ID="txtDataInicio" runat="server" Width="75" MaxLength="10"></asp:TextBox>
                        <asp:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtDataInicio">
                        </asp:CalendarExtender>
                        <b style="font-size:10px; color:Gray;">até</b>
                        <asp:TextBox ID="txtDataFim" runat="server" Width="75" MaxLength="10"></asp:TextBox>
                        <asp:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDataFim">
                        </asp:CalendarExtender>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <b style="font-size:12px; color:Gray;">Valor R$</b><br />                        
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="txtValor" runat="server" Width="185"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="btnAdicionar" runat="server" Text="Adicionar" Width="100" />
                    </td>                    
                </tr>
            </table>
                       
        </div>
    
        <asp:Label ID="lblMensagem" runat="server" Text=""></asp:Label>
    
    </form>    
        
</asp:Content>