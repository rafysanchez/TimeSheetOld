<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="visualizarApontamento.aspx.vb" Inherits="IntranetVB.visualizarApontamento" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Visualizar Apontamento
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">    

    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script> 
    <script type="text/javascript" src="../recursos/jQuery/jquery.maskedinput-1.2.2.js"></script>
    <style type="text/css">
        
        h1      {font: bold 16px Arial, Helvetica, sans-serif;}
        p       {font: 11px Arial, Helvetica, sans-serif;}
        a       {color: #900; text-decoration: none;}
        a:hover {text-decoration:underline;}
         .promptText
        {            
            text-align:right;
            color:Gray;	
        }

                    
    </style>

         
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server">
<center>   
 
    <div style="font-style: italic; color: #808080; text-align:center; padding-bottom:10px;">
        <h2>Visualização de Apontamentos</h2> 
    </div>
   
    <form id="form1" runat="server">
   
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" />
        <!-- Guarda uma URL para uso no código -->
        <input type="hidden" id="txtURL" runat="server" />

        <!-- Este Script deve-se ficar depois do ToolkitScriptManager -->
        <script type="text/javascript">
            $(document).ready(function () {
                $(function () {
                    $("[id$=txtDataInicio]").mask("99/99/9999");
                    $("[id$=txtDataInicio2]").mask("99/99/9999");
                });
            });
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                $(function () {
                    $("[id$=txtDataInicio]").mask("99/99/9999");
                    $("[id$=txtDataInicio2]").mask("99/99/9999");
                });
            }); 

            function novaJanela() {
            w = 1000;
            h = (screen.height * 0.95); // Altura da tela menos 5%
            LeftPosition = (screen.width) ? (screen.width - w) / 2 : 0;
            settings = "width=" + w + ",height=" + h + ",top=0,left=" + LeftPosition + 
                ",toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes";
                        
            var URL = $("[id$=txtURL]").val();

//            var colCodigo = $("[id$=txtColCodigo]").val();
//            var proCodigo = $("[id$=txtProCodigo]").val();
//            var dataInicio = $("[id$=txtData]").val();

//            URL = URL + "?colCodigo=" + colCodigo + "&proCodigo=" + proCodigo + "&dataInicio=" + dataInicio;

            window.open(URL, "_blank", settings);
        };
        </script>
                        
        <asp:Label ID="lblDebug" runat="server" Text="" />
                               
        <asp:TabContainer ID="TabContainer1" runat="server" VerticalStripWidth="50"
            Width="850" Height="250" ActiveTabIndex="0" ScrollBars="Auto" style="text-align: left;">
                    
            <asp:TabPanel HeaderText="&nbsp; Apontamento por projeto &nbsp;" ID="tabPainel1" runat="server">
                <ContentTemplate>                            
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>

                            <div style="margin-left:10px; margin-top:5px;"> 
                                <table style="font-size:12px;">
                                    <tr>
                                        <td align="left" style="width:400px;">
                                            <h5>Projeto:</h5> 
                                            <asp:DropDownList ID="ddlProjetos" runat="server" Width="380px" CssClass="txtBox" 
                                                AutoPostBack="True">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>  
                                        <td valign="top" align="left" style="width:100px;">
                                            <h5>Data inicial:</h5>                           
                                            <asp:TextBox ID="txtDataInicio" runat="server" Width="80px" class='txtBox' 
                                                MaxLength="10" />
                                            <asp:CalendarExtender ID="CalendarExtender1" runat="server" Enabled="True"
                                                FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" TargetControlID="txtDataInicio">
                                            </asp:CalendarExtender>
                                        </td>
                                        <td valign="bottom">                           
                                            <asp:Button ID="btnVisualizar" runat="server" Text="Visualizar" Height="25px" 
                                              Width="90px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top" align="left">
                                            <h5>Colaborador:</h5>
                                            <asp:ListBox ID="lstColaboradores" runat="server" Width="380px" Height="100px" 
                                             CssClass="txtBox" />                                            
                                        </td>
                                        <td colspan="2">
                                            <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0" 
                                               AssociatedUpdatePanelID="UpdatePanel1">                                                                                                                                                                        <ProgressTemplate>                        
                                                <center>
                                                    <table>
                                                        <tr>
                                                            <td valign="middle">
                                                                <img id="imgProgress" src="~/imagens/progress.gif" runat="server" 
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
                                        </td>                        
                                    </tr>
                                    <tr style="height:50px;">
                                        <td colspan="2" valign="bottom">
                                            <asp:Label ID="lblMensagem1" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>

                        </ContentTemplate>        
                    </asp:UpdatePanel>
                </ContentTemplate>
            </asp:TabPanel>
                    
            <asp:TabPanel HeaderText="&nbsp; Consolidar apontamentos &nbsp;" ID="tabPanel2" runat="server">
                <ContentTemplate>                            
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div id="divApontamento" style="text-align:left" runat="server">
                    <asp:Label ID="lblTitulo1" runat="server" Text=""></asp:Label>
                </div>
                            <div style="margin-left:10px; margin-top:5px;">
                                <table style="font-size:12px;" cellpadding="0" cellspacing="0"> 
                                    <tr>
                                        <td rowspan="2" valign="top" align="left" style="padding-right:18px;">
                                            <h5>Colaborador:</h5>                                            
                                            <asp:ListBox ID="lstColaboradores2" runat="server" Height="120" Width="380" 
                                                CssClass="txtBox" />
                                            <asp:ListSearchExtender ID="ListSearchExtender1" runat="server"
                                                TargetControlID="lstColaboradores2"
                                                PromptText="Digite para procurar"
                                                PromptCssClass="promptText"
                                                PromptPosition="Top" />
                                        </td>
                                        <td valign="top" align="left" style="padding-right:18px;">
                                            <h5>Data inicial:</h5>
                                            <asp:TextBox ID="txtDataInicio2" runat="server" CssClass="txtBox" Width="80" 
                                            MaxLength="10" />
                                            <asp:CalendarExtender ID="CalendarExtender2" runat="server" Enabled="True"
                                                FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" TargetControlID="txtDataInicio2">
                                            </asp:CalendarExtender>
                                        </td>
                                        <td valign="top" align="left" style="padding-right:15px; padding-top:17px;">
                                            <asp:Button ID="btnVisualizar2" runat="server" Text="Visualizar" Height="25px" 
                                              Width="90px" />
                                        </td>
                                    </tr>
                                    <tr>                                    
                                        <td colspan="2" valign="middle" align="center">
                                            <asp:UpdateProgress ID="UpdateProgress2" runat="server" DisplayAfter="0"
                                              AssociatedUpdatePanelID="UpdatePanel2">
                                                <ProgressTemplate>                        
                                                        <center>
                                                            <table>
                                                                <tr>
                                                                    <td valign="middle">
                                                                        <img id="imgProgress" src="~/imagens/progress.gif" runat="server" 
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
                                        </td>
                                    </tr>
                                    <tr style="height:50px;">
                                        <td colspan="2" valign="bottom">
                                            <asp:Label ID="lblMensagem2" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>

                        </ContentTemplate>        
                    </asp:UpdatePanel>
                </ContentTemplate>
            </asp:TabPanel>

        </asp:TabContainer>
                       
        <div class="h2" style="text-align:left; padding-left:140px; padding-right:140px; padding-top:30px;">
            <strong>Visualização do Apontamento</strong>
            <hr />
        </div>
                
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                                       
                <!-- Tabela de exibição do Apontamento -->                
                <div id="divTabelaApontamento" runat="server" style="font-family:Verdana; font-size:12px;">
                    <div align="left" style="padding-left:200px;">
                        <input type='hidden' id='txtColCodigo' runat='server' />
                        <input type='hidden' id='txtProCodigo' runat='server' />
                        <input type='hidden' id='txtData' runat='server' />
                        <input type='button' id='btnExportar' runat='server' value='Exportar para PDF/Excel'
                          onclick='novaJanela();' />
                        <br /><br />                                                   
                    </div>
                    <table border="1" cellspacing="0" style="border-color:Gray;">
                        <tr>
                            <td colspan="9" align="left" valign="middle" height="67">
                                <div style="float:left;">
                                    &nbsp &nbsp &nbsp &nbsp
                                    <img alt="Logo Addvisor" src="../Imagens/Logo.png" width="114" height="58"/>
                                </div>
                                <div style="float:left; vertical-align:middle;">
                                    <br />
                                    <font size="5">
                                        &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp
                                        <b>Apontamento de Horas</b>
                                    </font>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="9" align="left" style="height:17px;">
                                <font size="1">
                                    &nbsp;
                                    <asp:Label ID="lblTituloPerfil" runat="server" Text="Consultor:"></asp:Label> &nbsp; 
                                    <b><asp:Label ID="lblColNome" runat="server" Text="[lblColNome]"></asp:Label></b>
                                    &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                                    Módulo: &nbsp; 
                                    <b><asp:Label ID="lblColModulo" runat="server" Text="[lblColModulo]"></asp:Label></b>
                                    &nbsp; &nbsp; &nbsp; &nbsp; 
                                    Nivel: &nbsp; 
                                    <b><asp:Label ID="lblColNivel" runat="server" Text="[lblColNivel]"></asp:Label></b>
                                </font>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="9" align="left" style="height:17px;">
                                <font size="1">
                                    &nbsp Mês de referência: &nbsp
                                    <b><asp:Label ID="lblMesReferencia" runat="server" Text="[lblMesReferencia]"></asp:Label></b>
                                    &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp  &nbsp &nbsp &nbsp &nbsp &nbsp
                                    &nbsp &nbsp &nbsp &nbsp  &nbsp &nbsp
                                    Período: &nbsp
                                    <b><asp:Label ID="lblPeriodo" runat="server" Text="[lblPeriodo]"></asp:Label></b>
                                </font>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="9" align="left" style="height:17px;">
                                <font size="1">
                                    &nbsp Cliente: &nbsp
                                    <b><asp:Label ID="lblCliente" runat="server" Text="[lblCliente]"></asp:Label></b>
                                </font>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="9" align="left" style="height:17px;">
                                <font size="1">
                                    &nbsp Projeto: &nbsp
                                    <b><asp:Label ID="lblProjeto" runat="server" Text="[lblProjeto]"></asp:Label></b>
                                </font>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="9" align="left" style="height:17px;">
                                <font size="1">
                                    &nbsp Responsável: &nbsp
                                    <b><asp:Label ID="lblResponsavel" runat="server" Text="[lblResponsavel]"></asp:Label></b>
                                </font>
                            </td>
                        </tr>
                        <tr valign="middle" style="height:20px;">
                            <td width="70">
                                <font size="1"><b>
                                    Data
                                </b></font>
                            </td>
                            <td width="50" align="center">
                                <font size="1"><b>
                                    Entrada
                                </b></font>
                            </td>
                            <td colspan="2" width="90" align="center">
                                <font size="1"><b>
                                    Almoço
                                </b></font>
                            </td>
                            <td width="50" align="center">
                                <font size="1"><b>
                                    Saída
                                </b></font>
                            </td>
                            <td width="50" align="center">
                                <font size="1"><b>
                                    Normal
                                </b></font>
                            </td>
                            <td width="50" align="center">
                                <font size="1"><b>
                                    Extra
                                </b></font>
                            </td>
                            <td width="50" align="center">
                                <font size="1"><b>
                                    Total
                                </b></font>
                            </td>
                            <td width="400" align="center">
                                <font size="1"><b>
                                    Descrição
                                </b></font>
                            </td>
                        </tr>
                        <tr id="linha1" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia1" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada1" runat="server" Width="33" MaxLength="5"
                                    AutoPostBack="true" Text="">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco1" runat="server" Width="33" MaxLength="5"
                                    AutoPostBack="true" Text="">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco1" runat="server" Width="33" MaxLength="5"
                                    AutoPostBack="true" Text="">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida1" runat="server" Width="33" MaxLength="5"
                                    AutoPostBack="true" Text="">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal1" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra1" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal1" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades1" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha2" runat="server" >
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia2" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada2" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco2" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco2" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label> 
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida2" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal2" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra2" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal2" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades2" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha3" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia3" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada3" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco3" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco3" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida3" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center"> 
                                <asp:Label ID="lblNormal3" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra3" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal3" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades3" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha4" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia4" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada4" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco4" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco4" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida4" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal4" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra4" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal4" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades4" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha5" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia5" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada5" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco5" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco5" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida5" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal5" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra5" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal5" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left"> 
                                <asp:Label ID="lblAtividades5" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha6" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia6" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada6" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco6" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco6" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida6" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal6" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra6" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal6" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades6" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td> 
                        </tr>
                        <tr id="linha7" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia7" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada7" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco7" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco7" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida7" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                           <td height="17" align="center">
                                <asp:Label ID="lblNormal7" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra7" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal7" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades7" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha8" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia8" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada8" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco8" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco8" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida8" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal8" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra8" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal8" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades8" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha9" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia9" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada9" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco9" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco9" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida9" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal9" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra9" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal9" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades9" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha10" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia10" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada10" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco10" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco10" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida10" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal10" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra10" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal10" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades10" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha11" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia11" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada11" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco11" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco11" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida11" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal11" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra11" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal11" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades11" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha12" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia12" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada12" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco12" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco12" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida12" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal12" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra12" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal12" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades12" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha13" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia13" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada13" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco13" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco13" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida13" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal13" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra13" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal13" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades13" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha14" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia14" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada14" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco14" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco14" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida14" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal14" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra14" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal14" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades14" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha15" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia15" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada15" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco15" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco15" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida15" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal15" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra15" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal15" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades15" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha16" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia16" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada16" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco16" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco16" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida16" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal16" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra16" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal16" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades16" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha17" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia17" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada17" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco17" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco17" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida17" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal17" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra17" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal17" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades17" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha18" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia18" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada18" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco18" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco18" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida18" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal18" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra18" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal18" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades18" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha19" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia19" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada19" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco19" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco19" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida19" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal19" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra19" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal19" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades19" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha20" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia20" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada20" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco20" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco20" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida20" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal20" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra20" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal20" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades20" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha21" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia21" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada21" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco21" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco21" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida21" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal21" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra21" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal21" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades21" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha22" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia22" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada22" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco22" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco22" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida22" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal22" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra22" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal22" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades22" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha23" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia23" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada23" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco23" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco23" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida23" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal23" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra23" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal23" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades23" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha24" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia24" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada24" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco24" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco24" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida24" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal24" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra24" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal24" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades24" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha25" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia25" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada25" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco25" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco25" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida25" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal25" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra25" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal25" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades25" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha26" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia26" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada26" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco26" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco26" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida26" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal26" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra26" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal26" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades26" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha27" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia27" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada27" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco27" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco27" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida27" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal27" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra27" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal27" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades27" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha28" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia28" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada28" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco28" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco28" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida28" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal28" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra28" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal28" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades28" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha29" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia29" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada29" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco29" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco29" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida29" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal29" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra29" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal29" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades29" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha30" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia30" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada30" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco30" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco30" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida30" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal30" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra30" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal30" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades30" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr id="linha31" runat="server">
                            <td height="17" align="center">
                                <font size="1">
                                    <asp:Label ID="lblDia31" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntrada31" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblEntAlmoco31" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaiAlmoco31" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblSaida31" runat="server" Width="33" MaxLength="5" AutoPostBack="true">
                                </asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblNormal31" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblExtra31" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="center">
                                <asp:Label ID="lblTotal31" runat="server" Text=""></asp:Label>
                            </td>
                            <td height="17" align="left">
                                <asp:Label ID="lblAtividades31" runat="server" Width="440" MaxLength="500">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr align="right">
                            <td colspan="5" bgcolor="#E4E4E4" height="17" align="right">
                                <font size="2">
                                    <b>Total:</b> &nbsp                           
                                </font>
                            </td>
                            <td bgcolor="#E4E4E4" align="center">
                                <b><asp:Label ID="lblTotalNormais" runat="server" Text="0:00"></asp:Label></b>
                            </td>
                            <td bgcolor="#E4E4E4" align="center">
                                <b><asp:Label ID="lblTotalExtras" runat="server" Text="0:00"></asp:Label></b>
                            </td>
                            <td bgcolor="#E4E4E4" align="center">
                                <b><asp:Label ID="lblTotalTotal" runat="server" Text="0:00"></asp:Label></b>
                            </td>                    
                        </tr>
                    </table>
                </div>       
                
                <br />     
                <asp:Label ID="lblErro" runat="server" Text=""></asp:Label>
                
          </ContentTemplate>
        </asp:UpdatePanel>       
                
    </form>

</center>     
</asp:Content>
