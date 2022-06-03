<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="gerenciaProjeto.aspx.vb" Inherits="IntranetVB.GerenciaProjeto" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Gerenciamento de Projeto
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
 
    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script>   
    <script type="text/javascript" src="../recursos/jQuery/jquery.maskedinput-1.2.2.js"></script>
    <script type="text/javascript" src="../recursos/jQuery/jquery.maskMoney.js"></script>
        
    <style type="text/css">
        .invisivel          { display:none; }
        .negrito            { font-weight:bold; }
        .setaMouse:hover    { cursor:default; }
        .txtTaxa            { text-align:right; padding:0; padding-right:3px; font-size:11px; }
    </style>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server">
 
    <form id="formulario" runat="server" style="text-align:left;">
    
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    
    <!-- Este Script deve-se ficar depois do ToolkitScriptManager -->
    <script type="text/javascript">
        $(document).ready(function () {
            $(function () {
                $("[id$=txtDataInicio]").mask("99/99/9999");
                $("[id$=txtDataFim]").mask("99/99/9999");
                $('[id$=txtTaxaProjeto]').maskMoney({ thousands: '.', decimal: ',' });
            });
        });
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            $(function () {
                $("[id$=txtDataInicio]").mask("99/99/9999");
                $("[id$=txtDataFim]").mask("99/99/9999");
                $('[id$=txtTaxaProjeto]').maskMoney({ thousands: '.', decimal: ',' });
            });
        });
    </script>
        
    <div style="font-style: italic; color: #808080; text-align:center; padding-bottom:0px;">
        <h2>Gerência de projetos</h2>
    </div>
        
    <asp:UpdatePanel ID="UpdatePanelFormulario" runat="server">
        <ContentTemplate>
        
            <table style="font-size:12px; margin: 0 auto;" cellpadding="1" cellspacing="2" width="600">
                <tr style="height:15px;">
                    <td colspan="2" style="font-size:11px; color:Red; text-align:center;">
                        <asp:Label ID="lblAvisoMudancaPerfil" runat="server" Text="" />
                    </td>
                </tr>
                <tr>
                    <td width="220" style="font-size: small">
                        Escolha um projeto: 
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProjeto" runat="server" Width="350" CssClass="txtBox" AutoPostBack="True">
                        </asp:DropDownList>                        
                    </td>
                </tr>
                <tr>
                     <td width="220" style="font-size: small">
                        Escolha um calendário: 
                    </td>
                     <td>
                        <asp:DropDownList ID="ddlCalendario" runat="server" Width="350" CssClass="txtBox" AutoPostBack="True">
                        </asp:DropDownList>                        
                    </td>
                </tr>
                <tr>
                    <td width="220" style="font-size:small">
                        Nome do Gerente (Cliente):
                    </td>
                    <td>
                        <asp:TextBox ID="txtNomeGerenteCliente" runat="server" class='txtBox'></asp:TextBox>
                        <b style="color:#315585;">
                        <asp:Label ID="lblNomeGerenteCliente" runat="server" Text=""></asp:Label>
                        </b>
                    </td>
                </tr>
                <tr>
                    <td width="220" style="font-size: small">
                        E-mail do Gerente (Cliente):
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmailGerenteCliente" runat="server" class='txtBox'></asp:TextBox>
                        <b style="color:#315585;">
                        <asp:Label ID="lblEmailGerenteCliente" runat="server" Text=""></asp:Label>
                        </b>
                    </td>
                </tr>
                <tr>
                    <td width="220" style="font-size: small">
                        Gerente do Projeto:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGerenteProjeto" runat="server" Width="230" CssClass="txtBox">
                        </asp:DropDownList>
                        <asp:TextBox ID="txtGerenteProjeto" Enabled="false" runat="server" CssClass="txtBox"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td width="220" style="font-size: small">
                        Gerente de Conta:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGerenteConta" runat="server" Width="230" CssClass="txtBox">                                
                        </asp:DropDownList>
                        <asp:TextBox ID="txtGerenteConta" Enabled="false" runat="server" CssClass="txtBox"></asp:TextBox>  
                    </td>        
                </tr>
                <tr>
                    <td width="220" style="font-size: small">                            
                        Diretor:
                    </td>
                    <td>                            
                        <asp:DropDownList ID="ddlDiretor" runat="server" Width="230" CssClass="txtBox">                                
                        </asp:DropDownList> 
                        <asp:TextBox ID="txtDiretor" Enabled="false" runat="server" CssClass="txtBox"></asp:TextBox>
                    </td>        
                </tr>
                <tr>
                    <td width="220" style="font-size: small">                            
                        Data Validade:
                    </td>
                    <td>            
                        de&nbsp;
                        <asp:TextBox ID="txtDataInicio" runat="server" Width="80" class='txtBox' MaxLength="10" />
                        &nbsp;até&nbsp;
                        <asp:TextBox ID="txtDataFim" runat="server" Width="80" class='txtBox' MaxLength="10" />
                        <asp:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtDataInicio">                        
                        </asp:CalendarExtender>
                        <asp:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDataFim">                        
                        </asp:CalendarExtender>
                    </td>        
                </tr>
                <tr style="display:none;">
                    <td width="220" style="font-size: small">                            
                        Status:            
                     </td>
                    <td>                            
                        <asp:DropDownList ID="ddlStatus" runat="server" Width="100px" class='txtBox'>
                            <asp:ListItem>Ativo</asp:ListItem>
                            <asp:ListItem>Inativo</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            
            <script type="text/javascript">
                function Selecionar(elemento, cordefundo) {
                    var Inputs = elemento.getElementsByTagName("input");
                    var cor = elemento.style.backgroundColor; //manter a cor default do elemento
                    for (var i = 0; i < Inputs.length; ++i) {
                        
                        if (Inputs[i].type == 'checkbox') {
                            Inputs[i].checked = !Inputs[i].checked;
                            elemento.style.backgroundColor = cordefundo;
                            elemento.onclick = function () {
                                Selecionar(this, cor);
                            };
                        }
                    }
                }
                
            </script>

            <!-- Tabela de inclusão/remoção de "Colaboradores no projeto" -->            
            <asp:UpdatePanel ID="UpdatePanelLista" runat="server">
                <ContentTemplate>
                                        
                    <table style="margin-top:15px; margin: 0 auto; padding-bottom:10px; margin-top:12px;">                        
                        <tr>
                            <td align='left' style="font-size:11px; padding-bottom:5px;">
                                <b style="color:#FF5500;">Todos os Colaboradores</b>
                            </td>
                            <td></td>
                            <td></td>
                            <td align='left' style="font-size:11px; padding-bottom:5px;">
                                <b style="color:#FF5500;">Colaboradores no projeto</b>
                            </td>
                            <td align='right' style="font-size:11px; padding-bottom:5px; padding-right:24px;">
                                <b style="color:#FF5500;">Taxa no projeto</b>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div style="overflow-y:scroll; width:400px; height:215px; border:1px solid Gray;">
                                    <!-- Grid com todos os Colaboradores Ativos -->
                                    <asp:GridView ID="gridTodosColaboradores" runat="server" CellPadding="4" RowStyle-Font-Size="11px"
                                        ForeColor="#333333" GridLines="None" DataKeyNames="proCodigo,colCodigo" 
                                        AutoGenerateColumns="False">
                                        <EditRowStyle BackColor="#999999" />
                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                       
                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                       
                                        <Columns>

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="ckbSel" runat="server"></asp:CheckBox>
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle CssClass="invisivel" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="proCodigo">
                                                <ItemTemplate>
                                                    <asp:Label id="lblProCodigo" runat="server" text='<%# Eval("proCodigo") %>' />
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle CssClass="invisivel" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="colCodigo">
                                                <ItemTemplate>
                                                    <asp:Label id="lblColCodigo" runat="server" text='<%# Eval("colCodigo") %>' />
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle CssClass="invisivel" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="colNome">
                                                <ItemTemplate>
                                                    <asp:Label id="lblColNome" runat="server" text='<%# Eval("colNome") %>' />
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle HorizontalAlign="Left" Width="380px" CssClass="setaMouse" />
                                            </asp:TemplateField> 

                                        </Columns>
                        
                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                                
                                    </asp:GridView>
                                </div>
                            </td>
                            <td style="width:50px; text-align:center; vertical-align:middle;">
                                  <asp:Button ID="btnAdicionarCol" runat="server" Text=">" Width="30" Height="30" CssClass="negrito"
                                    ToolTip="Adiciona consultor ao projeto selecionado" />
                                <br /><br />
                                <asp:Button ID="btnRemoverCol" runat="server" Text="<" Width="30" Height="30" CssClass="negrito"
                                    ToolTip="Remove consultor do projeto" />
                            </td>
                            <td colspan="2">
                                <div style="overflow-y:scroll; width:450px; height:215px; border:1px solid Gray;">
                                    <!-- Grid com todos os Colaboradores associados ao projeto -->
                                    <asp:GridView ID="gridColaboradoresNoProjeto" runat="server" CellPadding="4" 
                                        RowStyle-Font-Size="11px" ForeColor="#333333" GridLines="None" 
                                        DataKeyNames="proCodigo,colCodigo" AutoGenerateColumns="False">
                                        <EditRowStyle BackColor="#999999" />
                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                       
                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                       
                                        <Columns>

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="ckbSel" runat="server"></asp:CheckBox>
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle CssClass="invisivel" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="proCodigo">
                                                <ItemTemplate>
                                                    <asp:Label id="lblProCodigo" runat="server" text='<%# Eval("proCodigo") %>' />
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle CssClass="invisivel" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="colCodigo">
                                                <ItemTemplate>
                                                    <asp:Label id="lblColCodigo" runat="server" text='<%# Eval("colCodigo") %>' />                                                    
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle CssClass="invisivel" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="colNome">
                                                <ItemTemplate>
                                                    <div id="divSel" runat="server" style="width:auto;">
                                                        <asp:Label id="lblColNome" runat="server" text='<%# Eval("colNome") %>' />
                                                    </div>
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle HorizontalAlign="Left" Width="430px" CssClass="setaMouse" />                                            
                                            </asp:TemplateField>

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtTaxaProjeto" runat="server" Width="60" CssClass="txtTaxa"
                                                     AutoCompleteType="None" text='<%# Eval("colProTaxa") %>' />
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle HorizontalAlign="Right" CssClass="txtTaxa" />
                                            </asp:TemplateField>

                                        </Columns>
                        
                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                                
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>                            
                    </table>               
                                       
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlProjeto" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
            
            <table style="margin: 0 auto;" width="500">
                <tr>
                    <td align="center">
                        <asp:Button ID="btnSalvar" runat="server" Text=" Salvar Alteração " Width="150" Height="25" />
                    </td>
                    <td align="center">
                        <asp:Button ID="btnExcluir" runat="server" Text=" Excluir " Width="150" Height="25" />
                    </td>
                </tr>
                <tr style="height:15px; font-size:11px;">
                    <td align="center">
                        <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                        <br />
                        <asp:Label ID="lblMensagem2" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanelFormulario" 
                DisplayAfter="100" DynamicLayout ="true">
                <ProgressTemplate>                        
                    <center style="margin-top:-15px;">
                        <table>
                            <tr>
                                <td valign="middle">
                                    <img id="imgProgress1" src="~/imagens/progress.gif" runat="server" 
                                        alt="carregando" style="height:60%; width:60%;" />
                                </td>
                                <td valign="middle" style="font-family:@Microsoft YaHei, Verdana; font-size:small; color:Gray;">
                                    Carregando...
                                </td>
                            </tr>
                        </table>                          
                    </center>                        
                </ProgressTemplate>
            </asp:UpdateProgress>
    
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlProjeto" EventName="TextChanged" />
        </Triggers>
    </asp:UpdatePanel>
    
</form>

<!-- Esta DIV serve somente para o rodapé não tocar em nenhum componente da tela -->
<div style="height:20px;"></div>   
    
</asp:Content>
