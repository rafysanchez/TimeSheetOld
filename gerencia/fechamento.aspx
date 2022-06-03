<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="fechamento.aspx.vb" Inherits="IntranetVB.relatorioFechamento" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Fechamento Folha
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script>
    <script type="text/javascript">

        function apagarMens() {
            $("[id$=lblMensagem]").html("");
        }

        function Desabilitar() {
            $('#desabilitar *').attr('disabled', 'disabled');
        };

        function novaJanela(link) {
              var URL = $("[id$=txtURL]").val();
            w = 1000;
            h = (screen.height * 0.95); // Altura da tela menos 5%
            LeftPosition = (screen.width) ? (screen.width - w) / 2 : 0;
            settings = "width=" + w + ",height=" + h + ",top=0,left=" + LeftPosition +
                ",toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes";

            URL = URL + "?Projeto=" + $(link).attr("projeto") + "&Colaborador=" + $(link).attr("colaborador") + "&Data=" + $("[id$=txtDtIni]").val() 
            window.open(URL, "_blank", settings);

        };

    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server">
    <!-- Guarda uma URL para uso no código -->
    <input type="hidden" id="txtURL" runat="server" />
    

    <div style="font-style: italic; color: #808080; text-align: center; padding-bottom: 10px;">
        <h2>Fechamento Folha</h2>
    </div>

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" AsyncPostBackTimeout="600">
        </asp:ScriptManager>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                 <input type="hidden" id="txtDtIni" runat="server" />
                <div id="desabilitar">
                    <span style="font-size: small;">Projeto: </span>
                    <asp:DropDownList ID="ddlProjeto" runat="server" Width="300" AutoPostBack="true" onchange="Desabilitar();">
                    </asp:DropDownList>
                    &nbsp; &nbsp;
                    <span style="font-size: small;">Competência: </span>
                    <asp:DropDownList ID="ddlMes" runat="server" Width="100" AutoPostBack="true" onchange="Desabilitar();">
                    </asp:DropDownList>
                    &nbsp; &nbsp;
                    <span style="font-size: small;">Dia inicial: </span>
                    <asp:DropDownList ID="ddlDiaInicial" runat="server" Width="50" AutoPostBack="true" onchange="Desabilitar();">
                    </asp:DropDownList>
                    &nbsp; &nbsp;
                    <span style="font-size: small;">Ano: </span>
                    <asp:DropDownList ID="ddlAno" runat="server" Width="60" AutoPostBack="true" onchange="Desabilitar();">
                    </asp:DropDownList>
                    <br />
                    <br />
                    <span style="font-size: small;">Ordenar por:</span>
                    <asp:DropDownList ID="ddlOrdernar" runat="server" Width="180" AutoPostBack="true" onchange="Desabilitar();">
                        <asp:ListItem>Nome do Projeto</asp:ListItem>
                        <asp:ListItem>Nome do colaborador</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Label ID="lblPeriodoSelecionado" runat="server"></asp:Label>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>

        <div style="height: 20px;">
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1"
                DisplayAfter="0" DynamicLayout="true">
                <ProgressTemplate>
                    <center>
                        <table>
                            <tr>
                                <td valign="middle">
                                    <img id="imgProgress1" src="~/imagens/progress.gif" runat="server"
                                        alt="carregando" style="height: 60%; width: 60%;" />
                                </td>
                                <td valign="middle" style="font-family: @Microsoft YaHei, Verdana; font-size: small; color: Gray;">Carregando...
                                </td>
                            </tr>
                        </table>
                        <br />
                    </center>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
        <br />

        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>

                <div style="padding-left: 100px;">
                    <!-- Lista com Status de liberação -->
                    <asp:ListView ID="tabelaRelatorio" runat="server" ItemPlaceholderID="lista"
                        DataKeyNames="proNome,nome,GP,GC,Dir,colTipoContrato,colSalario,taxaProjeto,horasTrabalhadas,
                    valorTotal,proCodigo,colCodigo,taxaCustoCLT_PJ">

                        <LayoutTemplate>
                            <table id="tabela" runat="server" cellspacing="2" style="font-size: 10px;" cellpadding="2">
                                <tr id="itemPlaceHolder" runat="server" style="background-color: Gray;">
                                    <td style="height: 20px; width: 300px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Nome do projeto</strong>
                                    </td>
                                    <td style="height: 20px; width: 200px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Nome do consultor</strong>
                                    </td>
                                    <td style="height: 20px; width: 80px; text-align: center;" valign="middle" colspan="3" scope="row">
                                        <strong style="color: White;">Aprovação</strong>
                                    </td>
                                    <td style="height: 20px; width: 60px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Contrato</strong>
                                    </td>
                                    <td style="height: 20px; width: 50px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Taxa<br />
                                            hora</strong>
                                    </td>
                                    <td style="height: 20px; width: 60px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Taxa<br />
                                            Projeto</strong>
                                    </td>
                                    <td style="height: 20px; width: 50px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Horas</strong>
                                    </td>
                                    <td style="height: 20px; width: 90px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Valor</strong>
                                    </td>
                                </tr>
                                <tr style="background-color: Gray;">
                                    <td style="height: 20px; width: 30px; text-align: center;" valign="middle" align="center">
                                        <strong style="color: White;">GP</strong>
                                    </td>
                                    <td style="height: 20px; width: 30px; text-align: center;" valign="middle" align="center">
                                        <strong style="color: White;">GC</strong>
                                    </td>
                                    <td style="height: 20px; width: 30px; text-align: center;" valign="middle" align="center">
                                        <strong style="color: White;">Dir</strong>
                                    </td>
                                </tr>
                                <tr runat="server" id="lista">
                                </tr>
                                <tr>
                                    <td style="width: auto; text-align: center;" colspan="8"></td>
                                    <td style="width: auto; background-color: Gray; text-align: center;">
                                        <strong style="color: White;">Total:</strong>
                                    </td>
                                    <td style="width: auto; background-color: Gray; text-align: center;">
                                        <strong style="color: White;">
                                            <asp:Label ID="lblTotal" runat="server" Text="" />
                                        </strong>
                                    </td>
                                </tr>
                            </table>

                            <br />
                            <div style="font-family: Consolas; text-align: left; font-size: 12px;">
                                <strong style="color: Green;">A</strong> = Aprovado
                                <br />
                                <strong style="color: Red;">R</strong> = Reprovado
                                <br />
                                <strong style="color: Yellow;">P</strong> = Pendência
                                <br />
                                <strong style="color: Gray;">N</strong> = Não há gerente associado
                                <br />
                            </div>

                        </LayoutTemplate>

                        <ItemTemplate>
                            <tr runat="server" style='background-color: #FFF6E3;' onmouseover='javascript:this.style.backgroundColor="#D3D1FF"'
                                onmouseout='javascript:this.style.backgroundColor="#FFF6E3"'>
                                <td runat="server" align="left">
                                    <asp:Label ID="lblProjeto" runat="server" Text='<%# Eval("proNome") %>' />
                                </td>
                                <td runat="server" align="left">
                                   <asp:LinkButton runat="server" ID="btnExibir"  OnClientClick='novaJanela(this)'   colaborador='<%# Eval("colCodigo") %>' projeto='<%# Eval("proCodigo") %>' > <asp:Label ID="lblNome" runat="server" Text='<%# Eval("nome") %>' /></asp:LinkButton>
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
                                <td runat="server" id="linhaColTipoContrato" align="center">
                                    <asp:Label ID="lblColTipoContrato" runat="server" Text='<%# Eval("colTipoContrato") %>' />
                                </td>
                                <td runat="server" id="linhaColSalario" align="right">
                                    <asp:Label ID="lblColSalario" runat="server" Text='<%# Eval("colSalario") %>' />
                                </td>
                                <td runat="server" id="linhaTaxaProjeto" align="center">
                                    <asp:Label ID="lblTaxaProjeto" runat="server" Text='<%# Eval("taxaProjeto") %>' />
                                </td>
                                <td runat="server" id="linhaHorasTrabalhadas" align="center">
                                    <asp:Label ID="lblHorasTrabalhadas" runat="server" Text='<%#  Eval("horasTrabalhadas") %>' />
                                </td>
                                <td runat="server" id="linhaValorTotal" align="right">
                                    <asp:Label ID="lblValorTotal" runat="server" Text='<%# Eval("valorTotal") %>' />
                                </td>
                                <td runat="server" id="linhaProCodigo" style="display: none;">
                                    <asp:Label ID="lblProCodigo" runat="server" Text='<%# Eval("proCodigo") %>' />
                                </td>
                                <td runat="server" id="linhaColCodigo" style="display: none;">
                                    <asp:Label ID="lblColCodigo" runat="server" Text='<%# Eval("colCodigo") %>' />
                                </td>
                                <td runat="server" id="linhaTaxaCustoCLT_PJ" style="display: none;">
                                    <asp:Label ID="lblTaxaCustoCLT_PJ" runat="server" Text='<%# Eval("taxaCustoCLT_PJ") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>

                        <EmptyDataTemplate>
                            <table cellspacing="2" style="font-size: 10px;" cellpadding="2">
                                <tr style="background-color: Gray;">
                                    <td style="height: 20px; width: 300px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Nome do projeto</strong>
                                    </td>
                                    <td style="height: 20px; width: 200px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Nome do consultor</strong>
                                    </td>
                                    <td style="height: 20px; width: 80px; text-align: center;" valign="middle" colspan="3">
                                        <strong style="color: White;">Aprovação</strong>
                                    </td>
                                    <td style="height: 20px; width: 60px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Contrato</strong>
                                    </td>
                                    <td style="height: 20px; width: 50px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Taxa<br />
                                            hora</strong>
                                    </td>
                                    <td style="height: 20px; width: 60px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Taxa<br />
                                            Projeto</strong>
                                    </td>
                                    <td style="height: 20px; width: 50px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Horas</strong>
                                    </td>
                                    <td style="height: 20px; width: 90px; text-align: center;" valign="middle" rowspan="2">
                                        <strong style="color: White;">Valor</strong>
                                    </td>
                                </tr>
                                <tr style="background-color: Gray;">
                                    <td style="height: 20px; width: 30px; text-align: center;" valign="middle" align="center">
                                        <strong style="color: White;">GP</strong>
                                    </td>
                                    <td style="height: 20px; width: 30px; text-align: center;" valign="middle" align="center">
                                        <strong style="color: White;">GC</strong>
                                    </td>
                                    <td style="height: 20px; width: 30px; text-align: center;" valign="middle" align="center">
                                        <strong style="color: White;">Dir</strong>
                                    </td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>

                    </asp:ListView>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <br />
                <div id="divBotoes" runat="server" style="padding-right: 200px; text-align: right; margin-right: 20px; display: none;">
                    <div style="float: right; margin-right: 10px;">
                        <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar fechamento" Height="25" OnClientClick="apagarMens();" />
                    </div>
                    <div style="float: right; margin-right: 10px;">
                        <asp:Button ID="btnReabrir" runat="server" Text="Reabrir" Height="25" OnClientClick="apagarMens();" />
                    </div>
                    <div style="float: right; margin-right: 10px;">
                        <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel3"
                            DisplayAfter="0" DynamicLayout="true">
                            <ProgressTemplate>
                                <table>
                                    <tr>
                                        <td valign="middle">
                                            <img id="imgProgress2" src="~/imagens/progress.gif" runat="server"
                                                alt="carregando" style="height: 60%; width: 60%;" />
                                        </td>
                                        <td valign="middle" style="font-family: @Microsoft YaHei, Verdana; font-size: small; color: Gray;">Carregando...
                                        </td>
                                    </tr>
                                </table>
                                <br />
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                    <br />
                </div>
                <center>
                    &nbsp; &nbsp; &nbsp; &nbsp; 
                    <asp:Label ID="lblMensagem" runat="server" Text="" CssClass="error"></asp:Label>
                </center>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnConfirmar" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>

    </form>

</asp:Content>
