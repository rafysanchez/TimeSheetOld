<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="relatorio.aspx.vb" Inherits="IntranetVB.relatorio" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Relatório</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script>
    <script type="text/javascript">

        function apagarMens() {
            $("[id$=lblMensagem]").html("");
        }

        function Desabilitar() {
            $('#desabilitar *').attr('disabled', true);
            return true;
        };

        function Habilitar() {
            $('#desabilitar *').removeAttr("disabled");                       
        };

    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server">
       
    <div style="font-style:italic; color:#808080; text-align:center; padding-bottom:10px;">
        <h2>Relatório</h2>
    </div>
    
    <form id="form1" runat="server">
                   
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
        </asp:ScriptManager>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                
                <div id="desabilitar">
                    <span style="font-size:small;">Projeto: </span>
                        <asp:DropDownList ID="ddlProjeto" runat="server" Width="300" AutoPostBack="true" onchange="Desabilitar();">
                        </asp:DropDownList> &nbsp; &nbsp;                        
                    <span style="font-size:small;">Competência: </span>
                        <asp:DropDownList ID="ddlMes" runat="server" Width="100" AutoPostBack="true" onchange="Desabilitar();">
                        </asp:DropDownList> &nbsp; &nbsp;
                    <span style="font-size:small;">Dia inicial: </span>
                        <asp:DropDownList ID="ddlDiaInicial" runat="server" Width="50" AutoPostBack="true" onchange="Desabilitar();">
                        </asp:DropDownList> &nbsp; &nbsp;
                    <span style="font-size:small;">Ano: </span>
                        <asp:DropDownList ID="ddlAno" runat="server" Width="60" AutoPostBack="true" onchange="Desabilitar();">
                        </asp:DropDownList>
                    <br /><br /><abbr></abbr>
                    <center>
                        <fieldset style="width:900px; padding-top:10px; padding-bottom:10px;">
                            <legend style="font-size:12px; text-align:left;">Filtros</legend>                    
                            <span style="font-size:small;">Nome do colaborador: </span>
                                <asp:TextBox ID="txtPesquisaNome" runat="server" Width="300" 
                                    ToolTip="Pesquisa pelo nome do colaborador" MaxLength="50" AutoCompleteType="Disabled">
                                </asp:TextBox>
                            <asp:AutoCompleteExtender ID="autoComplete" runat="server" 
                                TargetControlID="txtPesquisaNome" 
                                ServicePath="~/AutoCompletar.asmx" 
                                ServiceMethod="getNomesColaboradores" 
                                MinimumPrefixLength="1" 
                                CompletionInterval="200" 
                                EnableCaching="true" 
                                CompletionSetCount="12" />  &nbsp; &nbsp;
                            <span style="font-size:small;">Tipo de contrato: </span>
                            <asp:DropDownList ID="ddlTipoContrato" runat="server" Width="100">
                                <asp:ListItem>Todos</asp:ListItem>
                                <asp:ListItem>PJ</asp:ListItem>
                                <asp:ListItem>PJ Fechado</asp:ListItem>
                                <asp:ListItem>CLT</asp:ListItem>
                            </asp:DropDownList>
                            <br />
                            <div style="padding-top:10px;">
                                <span style="font-size:small;">Aprovadores: </span> 
                                <asp:DropDownList ID="ddlAprovadores" runat="server" Width="400">
                                </asp:DropDownList>  &nbsp; &nbsp;      
                                <asp:Button ID="btnAplicarFiltros" runat="server" Text="Aplicar filtros" OnClick="btnAplicarFiltros_Click1"/>
                                <br />                                                       
                            </div>
                            <div style="width:auto; height:15px; font-size:12px; color:Gray;">
                                <asp:Label ID="lblAprovadorSelecionado" runat="server" Text=""></asp:Label>
                            </div>
                        </fieldset>
                    </center>
                    <asp:Label ID="lblPeriodoSelecionado" runat="server"></asp:Label>
                </div>
                
            </ContentTemplate>
        </asp:UpdatePanel>
       
        <div style="height:20px;">
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" 
                DisplayAfter="0" DynamicLayout ="true">
                <ProgressTemplate>                        
                        <center>
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
                            <br />  
                        </center>                        
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div><br />
        
       <div>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>                
                <center>

                <!-- Lista com Status de liberação -->                
                <asp:ListView ID="tabelaRelatorio" runat="server" ItemPlaceholderID="lista" 
                    DataKeyNames="statusCriadoPedidoSAP,proNome,nome,GP,GC,Dir,colTipoContrato,colSalario,taxaProjeto,
                    horasTrabalhadas,valorTotal,proCodigo,colCodigo,razaoSocial,empresaCompartilhada">
                    
                    <LayoutTemplate>                    
                        <table id="tabela" runat="server" cellspacing="2" style="font-size:10px;" cellpadding="2">
                            <tr id="itemPlaceHolder" runat="server" style="background-color:Gray;">                                
                                <td style="height:20px; width:7; background-color:White;" valign="middle" rowspan="2">                                    
                                </td>
                                <td style="height:20px; width:auto;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Nome do projeto</strong>
                                </td>
                                <td style="height:20px; width:auto; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Nome do consultor</strong>
                                </td>
                                <td style="height:20px; width:80px; text-align:center;" valign="middle" colspan="3" scope="row">
                                    <strong style="color:White;">Aprovação</strong>                                   
                                </td>
                                <td style="height:20px; width:60px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Contrato</strong>
                                </td>
                                <td style="height:20px; width:50px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Taxa<br />hora</strong>
                                </td>
                                <td style="height:20px; width:60px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Taxa<br />Projeto</strong>
                                </td>
                                <td style="height:20px; width:50px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Horas</strong>
                                </td>
                                <td style="height:20px; width:90px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Valor</strong>
                                </td>
                                <td style="height:20px; width:auto; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Razão Social</strong>
                                </td><td style="height:20px; width:auto;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Empresa Compartilhada</strong>
                                </td>
                            </tr>
                            <tr style="background-color:Gray;">
                                <td style="height:20px; width:30px; text-align:center;" valign="middle" align="center">
                                    <strong style="color:White;">GP</strong>
                                </td>
                                <td style="height:20px; width:30px; text-align:center;" valign="middle" align="center">
                                    <strong style="color:White;">GC</strong>
                                </td>
                                <td style="height:20px; width:30px; text-align:center;" valign="middle" align="center">
                                    <strong style="color:White;">Dir</strong>
                                </td>
                            </tr>
                            <tr runat="server" ID="lista">
                            </tr>
                            <tr>
                                <td style="width:auto; text-align:center;" colspan="9"></td>
                                <td style="width:auto; text-align:center; background-color:Gray;">
                                    <strong style="color:White;">Total:</strong>
                                </td>
                                <td style="width:auto; background-color:Gray; text-align:center;">
                                    <strong style="color:White;">
                                        <asp:Label ID="lblTotal" runat="server" Text="" />
                                    </strong>
                                </td>
                            </tr>
                        </table>

                    </LayoutTemplate>
                    
                    <ItemTemplate>
                        <tr id="trDados" runat="server" style='background-color:#FFF6E3;' onmouseover='javascript:this.style.backgroundColor="#D3D1FF"' 
                            onmouseout='javascript:this.style.backgroundColor="#FFF6E3"'>
                            <td align="left" style="padding-right:4px; background-color:White;">
                                <asp:Label ID="lblStatusCriadoPeidoSAP" runat="server" Text='<%# Eval("statusCriadoPedidoSAP") %>' />
                            </td>
                            <td align="left" style="padding-right:4px;">
                                <asp:Label ID="lblProjeto" runat="server" Text='<%# Eval("proNome") %>' />
                            </td>
                            <td align="left" style="padding-right:4px;">
                                <asp:Label ID="lblNome" runat="server" Text='<%# Eval("nome") %>' />
                            </td>
                            <td align="center" id="linhaGP" runat="server">
                                <asp:Label ID="lblGP" runat="server" Text='<%# Eval("GP") %>' />
                            </td>
                            <td align="center" id="linhaGC" runat="server">
                                <asp:Label ID="lblGC" runat="server" Text='<%# Eval("GC") %>' />
                            </td>
                            <td align="center" id="linhaDir" runat="server">
                                <asp:Label ID="lblDir" runat="server" Text='<%# Eval("Dir") %>' />
                            </td>
                            <td align="center">
                                <asp:Label ID="lblColTipoContrato" runat="server" Text='<%# Eval("colTipoContrato") %>' />
                            </td>
                            <td align="right" style="padding-right:4px;">
                                <asp:Label ID="lblColSalario" runat="server" Text='<%# Eval("colSalario") %>' />
                            </td>
                            <td align="center">
                                <asp:Label ID="lblTaxaProjeto" runat="server" Text='<%# Eval("taxaProjeto") %>' />
                            </td>
                            <td align="center">
                                <asp:Label ID="lblHorasTrabalhadas" runat="server" Text='<%# Eval("horasTrabalhadas") %>' />
                            </td>
                            <td align="right" style="padding-right:4px;">
                                <asp:Label ID="lblValorTotal" runat="server" Text='<%# Eval("valorTotal") %>' />
                            </td>                            
                            <td style="display:none;">
                                <asp:Label ID="lblProCodigo" runat="server" Text='<%# Eval("proCodigo") %>' />
                            </td>
                            <td style="display:none;">
                                <asp:Label ID="lblColCodigo" runat="server" Text='<%# Eval("colCodigo") %>' />
                            </td>
                            <td align="left" style="padding-right:4px;">
                                <asp:Label ID="lblRazaoSocial" runat="server" Text='<%# Eval("razaoSocial") %>' />
                            </td> 
                            <td align="left">
                                <asp:Label ID="lblEmpresaCompartilhada" runat="server" Text='<%# Eval("empresaCompartilhada") %>' />
                            </td>                            
                        </tr>
                    </ItemTemplate>

                    <EmptyDataTemplate>
                        <table id="itemPlaceholderContainer" cellspacing="2" style="font-size:10px;" cellpadding="2">
                            <tr style="background-color:Gray;">                                
                                <td style="height:20px; width:300px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Nome do projeto</strong>
                                </td>
                                <td style="height:20px; width:200px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Nome do consultor</strong>
                                </td>
                                <td style="height:20px; width:80px; text-align:center;" valign="middle" colspan="3">
                                    <strong style="color:White;">Aprovação</strong>
                                </td>
                                <td style="height:20px; width:60px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Contrato</strong>
                                </td>
                                <td style="height:20px; width:50px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Taxa<br />hora</strong>
                                </td>
                                <td style="height:20px; width:60px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Taxa<br />Projeto</strong>
                                </td>
                                <td style="height:20px; width:50px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Horas</strong>
                                </td>
                                <td style="height:20px; width:90px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Valor</strong>
                                </td>
                                <td style="height:20px; width:300px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Razão Social</strong>
                                </td> 
                                <td style="height:20px; width:300px; text-align:center;" valign="middle" rowspan="2">
                                    <strong style="color:White;">Empresa Compartilhada</strong>
                                </td>                                 
                            </tr>
                            <tr style="background-color:Gray;">
                                <td style="height:20px; width:30px; text-align:center;" valign="middle" align="center">
                                    <strong style="color:White;">GP</strong>
                                </td>
                                <td style="height:20px; width:30px; text-align:center;" valign="middle" align="center">
                                    <strong style="color:White;">GC</strong>
                                </td>
                                <td style="height:20px; width:30px; text-align:center;" valign="middle" align="center">
                                    <strong style="color:White;">Dir</strong>
                                </td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                
                </asp:ListView>

                <br />
                <div style="font-family:Consolas; text-align:left; font-size:12px; padding-left:20px;">
                    <strong style="color:Green;">A</strong> = Aprovado <br />
                    <strong style="color:Red;">R</strong> = Reprovado <br />
                    <strong style="color:Yellow;">P</strong> = Pendência <br />
                    <strong style="color:Gray;">N</strong> = Não há gerente associado <br />
                </div>

                </center>
            </ContentTemplate>        
        </asp:UpdatePanel>
        
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <div id="divBotoes" runat="server" style="margin-top:-25px; text-align:right; margin-right:40px; display:none;">
                    <div style="float:right; margin-right:10px;">
                        <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar para Excel" Height="25" 
                         ToolTip="Exporta a tabela de fechamento para Excel" />
                    </div>
                    <div style="float:right; margin-right:10px;">
                        <asp:Button ID="btnCriarPedidoSAP" runat="server" Text="Baixar arquivo CSV" Height="25" 
                         OnClientClick="apagarMens();" 
                         ToolTip="Gera e faz download de um arquivo CSV para importação no SAP do pedido de compra" />
                    </div> 
                    <div style="float:right; margin-right:10px;">
                        <asp:Button ID="btnReabrir" runat="server" Text="Reabrir" Height="25" OnClientClick="apagarMens();"
                         ToolTip="Reabre os apontamentos que nunca foram para o arquivo CSV" />
                    </div>
                    <div style="float:right; margin-right:10px;">
                        <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel3" 
                            DisplayAfter="0" DynamicLayout ="true">
                            <ProgressTemplate>
                                <table>
                                    <tr>
                                        <td valign="middle">
                                            <img id="imgProgress2" src="~/imagens/progress.gif" runat="server" 
                                                alt="carregando" style="height:60%; width:60%;" />
                                        </td>
                                        <td valign="middle" style="font-family:@Microsoft YaHei, Verdana; font-size:small; color:Gray;">
                                            Carregando...
                                        </td>
                                    </tr>
                                </table>
                                <br />                    
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div><br />
                </div>
                <center>
                    &nbsp; &nbsp; &nbsp; &nbsp; 
                    <asp:Label ID="lblMensagem" runat="server" Text="" CssClass="error"></asp:Label>
                </center>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnReabrir" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
       </div>
                 
    </form> 
    
</asp:Content>
