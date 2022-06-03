<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="ShowHide.aspx.vb" Inherits="IntranetVB.WebForm1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Teste com AJAX
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="menuTopo" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server">

    <!-- Exemplo de utilização do show/Hide usando o updatePanel -->

    <script type="text/jscript">

        function apareceTexto1() {
            divTexto1.style.display = "block";
            divTexto2.style.display = "none";
        }

        function apareceTexto2() {
            divTexto1.style.display = "none";
            divTexto2.style.display = "block";
        }
    
    </script>

    <form runat="server" ID="frmReport">
    
        <ajax:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </ajax:ToolkitScriptManager>
        
        <br />    
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div id="myDiv" runat="server">
                    <table>
                        <tr>
                            <td>
                                Caixa de texto 1:
                            </td>
                            <td>
                                <asp:TextBox ID="txt1" runat="server"></asp:TextBox>
                            </td>                            
                        </tr>                        
                    </table>                    
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btn1" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <br />
        Caixa de texto 2:<asp:TextBox ID="txt2" runat="server"></asp:TextBox> 
        <br />
        <br />
        <asp:Button ID="btn1" runat="server" Text="Inserir Data" Height="32px" Width="112px" />
        <hr />
        <br />
        <br />
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
        Aparece<asp:RadioButton ID="radioAparece" GroupName="teste" Checked="true" runat="server" /> 
        Desaparece<asp:RadioButton ID="radioDesaparece" GroupName="teste" runat="server" /> 
        </ContentTemplate>            
        </asp:UpdatePanel>
        <br />
        <br />
        
        <div id="divTexto1">
            <strong> Texto Primeira Seleção </strong>
        </div>
        <div id="divTexto2" style="display:none;">
            <strong> Texto Segunda Seleção </strong>
        </div>             
    
              
    </form>

</asp:Content>
