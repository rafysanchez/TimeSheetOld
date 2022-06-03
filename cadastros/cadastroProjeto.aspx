<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="cadastroProjeto.aspx.vb" Inherits="IntranetVB.CadastroProjeto" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Cadastro de Projeto
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
   
    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script> 
    <script type="text/javascript" src="../recursos/jQuery/jquery.maskedinput-1.2.2.js"></script>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="corpo" runat="server">

    <div style="font-style: italic; color: #808080; text-align:center; padding-top:10px; padding-bottom:5px;">
        <h2>Cadastro de projeto</h2>
    </div>

    <form id="form1" runat="server" style="text-align:left;">

        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" EnableScriptGlobalization="true">
        </asp:ToolkitScriptManager>
    
        <!-- Este Script deve-se ficar depois do ToolkitScriptManager -->
        <script type="text/javascript">
            $(document).ready(function () {
                $(function () {
                    $("[id$=txtDataInicio]").mask("99/99/9999");
                    $("[id$=txtDataFim]").mask("99/99/9999");
                    $("[id$=txtCentroCusto]").mask("999999");
                });
            });
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                $(function () {
                    $("[id$=txtDataInicio]").mask("99/99/9999");
                    $("[id$=txtDataFim]").mask("99/99/9999");
                    $("[id$=txtCentroCusto]").mask("999999");
                });
            }); 
        </script>

        <asp:UpdatePanel ID="uppFormulario" runat="server">
            <ContentTemplate>
               
                <div id="Cadastro">
                    <table style="font-size:12px; margin: 0 auto;" cellpadding="2" cellspacing="2" width="500">
                          <tr>
                            <td colspan="3" align="center">
                                <div style="font-size:10px;">
                                    Pesquisa pelo nome do Projeto para alteração de cadastro: &nbsp &nbsp &nbsp <br />
                                    <asp:TextBox ID="txtPesquisar" runat="server" Width="430" CssClass='txtBox'
                                        ToolTip="" MaxLength="50" autocomplete="off">
                                    </asp:TextBox>
                                    <asp:Button ID="btnPesquisar" runat="server" Text=" OK "
                                        ToolTip="Pesquisa se existe e preenche o formulário" />
                                    <asp:AutoCompleteExtender
                                        runat="server"
                                        ID="autoComplete"
                                        TargetControlID="txtPesquisar"
                                        ServicePath="~/AutoCompletar.asmx"
                                        ServiceMethod="getNomesProjetos"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="200"
                                        EnableCaching="true"
                                        CompletionSetCount="12" />
                                    <br />
                                    <asp:Label ID="lbltxtPesquisar" runat="server" Text=""></asp:Label>                              
                                </div>
                            </td>
                        <tr>
                            <td colspan="3" style="height:20px;">
                                <asp:UpdateProgress ID="UpdateProgressTaxas" runat="server" AssociatedUpdatePanelID="uppFormulario" 
                                    DisplayAfter="100" DynamicLayout="true" >
                                    <ProgressTemplate>
                                        <center>
                                            <table>
                                                <tr>
                                                    <td valign="middle">
                                                        <img src="../imagens/progress.gif" 
                                                            alt="carregando" style="height:60%; width:60%;" />
                                                    </td>
                                                    <td valign="middle" style="font-family:@Microsoft YaHei, Verdana; 
                                                            font-size:small; color:Gray;">
                                                        &nbsp; Carregando...
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />   
                                        </center>  
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </td>
                        </tr>                                                    
                        </tr>
                        <tr style="background-color:Gray; height:20px;" valign="middle">
                            <td colspan="3">                            
                                <font color="White">
                                    &nbsp <b><asp:Label ID="lblTitulo" runat="server" Text="Novo cadastro"></asp:Label></b>
                                </font> 
                            </td>
                        </tr>
                    </table>
                        
                    <table style="font-size:12px; margin: 0 auto;" cellpadding="2" cellspacing="2">     
                        <tr style="height:24px;">
                            <td colspan="3" valign="middle" align="right">
                                <asp:Button ID="btnCadastrarNovo" runat="server" Text=" Cadastrar Novo " 
                                   ToolTip="Muda o modo para cadastro de novo colaborador" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <div id="div3" style="color:Red; font-size:12px;">*</div>
                            </td>
                            <td style="width:auto;">
                                Nome do Projeto: 
                            </td>
                            <td style="width:300px;">
                                <asp:TextBox ID="txtNomeProjeto" runat="server" Width="350px" autocomplete="off"
                                    class='txtBox' MaxLength="50"></asp:TextBox>                                
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <div id="div5" style="color:Red; font-size:12px;">*</div>
                            </td>
                            <td style="width:auto;">
                                Centro de custo: 
                            </td>
                            <td style="width:300px;">
                                <asp:TextBox ID="txtCentroCusto" runat="server" Width="230px" autocomplete="off"
                                    class='txtBox' MaxLength="10"></asp:TextBox>
                                <span style="font-size:10px; color:Red; font-style:italic;">
                                    <asp:Label ID="lblCentroCusto" runat="server" Text=""></asp:Label>
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <div id="div4" style="color:Red; font-size:12px;">*</div>
                            </td>
                            <td>
                                Nome do Cliente:
                            </td>
                            <td>
                                <asp:TextBox ID="txtNomeCliente" runat="server" 
                                    MaxLength="50" class='txtBox'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <div id="div1" style="color:Red; font-size:12px;">*</div>
                            </td>
                            <td>
                                Nome do Gerente (Cliente):
                            </td>
                            <td>
                                <asp:TextBox ID="txtNomeGerenteCliente" runat="server"
                                    MaxLength="50" class='txtBox'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <div id="div2" style="color:Red; font-size:12px;">*</div>
                            </td>
                            <td>
                                E-mail do Gerente (Cliente):
                            </td>
                            <td>
                                <asp:TextBox ID="txtEmailGerenteCliente" runat="server" Width="350px"
                                    MaxLength="100" class='txtBox'></asp:TextBox>
                            </td>        
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                Data Validade:
                             </td>
                            <td>
                                de
                                <asp:TextBox ID="txtDataInicio" runat="server" Width="80" class='txtBox' MaxLength="10"></asp:TextBox> 
                                &nbsp 
                                até &nbsp 
                                <asp:TextBox ID="txtDataFim" runat="server" Width="80" class='txtBox' MaxLength="10"></asp:TextBox>
                                
                                <asp:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtDataInicio"
                                    FirstDayOfWeek="Sunday" Format="dd/MM/yyyy">
                                </asp:CalendarExtender>
                                
                                <asp:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDataFim"
                                    FirstDayOfWeek="Sunday" Format="dd/MM/yyyy">
                                </asp:CalendarExtender>
                                
                            </td>        
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                Status:            
                             </td>
                            <td>
                                <asp:DropDownList ID="ddlStatus" runat="server" Width="100px" class='txtBox'>
                                    <asp:ListItem Selected="True">Ativo</asp:ListItem>
                                    <asp:ListItem>Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </td>          
                        </tr>
                        <tr style="height:50px; text-align:right;">
                            <td>
                            </td>
                            <td colspan="2"> 
                                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" Height="30px" Width="150" />
                                &nbsp
                                <asp:Button ID="btnLimpar" runat="server" Text="Limpar" Height="30px" Width="150" />          
                            </td>                                     
                        </tr>
                        <tr style="height:20px; text-align:right;">
                            <td>
                            </td>                            
                            <td>
                            </td>
                            <td align="left">                                
                            </td>            
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="uppFormulario" 
                                    DisplayAfter="100" DynamicLayout="true" >
                                    <ProgressTemplate>
                                        <center>
                                            <table>
                                                <tr>
                                                    <td valign="middle">
                                                        <img src="../imagens/progress.gif" 
                                                            alt="carregando" style="height:60%; width:60%;" />
                                                    </td>
                                                    <td valign="middle" style="font-family:@Microsoft YaHei, Verdana; 
                                                            font-size:small; color:Gray;">
                                                        &nbsp; Carregando...
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />   
                                        </center>  
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </td>
                        </tr>      
                    </table>
                </div>
                
                <div style="text-align:center; font-size:12px;">
                    <asp:Label ID="lblMensagem" runat="server" /><br />
                    <asp:Label ID="lblMensagem2" runat="server" />
                </div>
            
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSalvar" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnLimpar" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>

    </form>
    
    <!-- Esta DIV serve somente para o rodapé não tocar em nenhum componente da tela -->
    <div style="height:20px;"></div>

</asp:Content>
