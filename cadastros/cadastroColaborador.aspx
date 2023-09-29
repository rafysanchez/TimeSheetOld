<%@ Page Title="Cadastro de Consultor" Language="vb" AutoEventWireup="true" MasterPageFile="~/MasterPage.Master" 
  EnableEventValidation="false" CodeBehind="cadastroColaborador.aspx.vb" Inherits="IntranetVB.CadastroConsultor" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Cadastro de Colaborador
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server"> 
    
    <style type="text/css">
        .tabelas {
            font-size: 12px;
            background-color: #FAFAFA;
            margin: 0 auto;
            width: 610px;
            text-align: left;
            border: none;
        }

        .coluna1 {
            width: 15px;
        }

        .coluna2 {
            width: 180px;
        }

        .coluna3 {
            width: 415px;
        }
    </style>
           
    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script>   
    <script type="text/javascript" src="../recursos/jQuery/jquery.maskedinput-1.2.2.js"></script>
    
    <script type="text/javascript" src="../recursos/jQuery/jquery.mascaraDinheiro.js"></script>
    <%--
        Exemplo de uso do plugin mascaraDinheiro
        
        $('#example8').priceFormat();
        var unmask = $('#example8).unmask();
        alert(unmask); // alert 123456
    --%>
            
    <script type="text/javascript">

        function toUpper(campo) {
            campo.value = campo.value.toUpperCase();
        };

        function exibeCamposTipoContrato() {
            if ($("[id$=radioCLT]").is(':checked')) {
                $("[id$=tbClt]").css("display", "block");
                $("[id$=trPJ1]").css("display", "none");
                $("[id$=trPJ2]").css("display", "none");
                $("[id$=divFechado]").css("display", "none");
            } else {
                $("[id$=tbClt]").css("display", "none");
                $("[id$=trPJ1]").css("display", "block");
                $("[id$=trPJ2]").css("display", "block");
                $("[id$=divFechado]").css("display", "block");
            }
        };

        function exibeCamposTipoEmpresa() {
            if ($("[id$=cboTipoEmpresa]").val() == "Compartilhada") {
                $("[id$=trEmpresaCompartilhada]").css("display", "block");
            } else {
                $("[id$=trEmpresaCompartilhada]").css("display", "none");
            }
        };

        function ddlPerfil_OnSelectedIndexChanged() {
            if (($('[id$=ddlPerfil] option:selected').text() == 'Consultores' || $('[id$=ddlPerfil] option:selected').text() == 'Analistas')) {
                $("[id$=trModulo]").css("display", "block");
                $("[id$=trNivel]").css("display", "block");
            } else {
                $("[id$=trModulo]").css("display", "none");
                $("[id$=trNivel]").css("display", "none");
            }
        }

        /***********************************************
        * Textarea Maxlength script- © Dynamic Drive (www.dynamicdrive.com)
        * This notice must stay intact for legal use.
        * Visit http://www.dynamicdrive.com/ for full source code
        ***********************************************/
        function ismaxlength(obj) {
            var mlength = obj.getAttribute ? parseInt(obj.getAttribute("maxlength")) : ""
            if (obj.getAttribute && obj.value.length > mlength)
                obj.value = obj.value.substring(0, mlength)
        };

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        function novaJanela() {
            w = 1200;
            h = 700;
            LeftPosition = (screen.width) ? (screen.width - w) / 2 : 0;
            settings = "width=" + w + ",height=" + h + ",top=0,left=" + LeftPosition + ",toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes";

            var URL = "<%=URL%>";   // Pega o valor da variavel no lado servidor

            window.open(URL, "_blank", settings);
        };
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    </script> 

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="corpo" runat="server">
         
    <form id="form1" runat="server">
                    
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
        </asp:ScriptManager>
        
        <!-- Este Script deve-se ficar depois do ToolkitScriptManager --> 
        <script type="text/javascript">

            $(document).ready(function () {
                $(function () {
                    executaDepoisPostBack();
                });
            });
            // Sempre executa quando há um PostBack 
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                $(function () {
                    executaDepoisPostBack();
                });
            });

            function soNumeros(str) {
                str = str.toString();
                return str.replace(/\D/g, '');
            }

            function addMaskTelefone(obj) {
                $(obj).unmask();
                var num = soNumeros(obj.value);
                if (num.length == 10) {             // Número de tel sem o 9 na frente
                    $(obj).mask("(99) 9999-9999?9");
                } else if (num.length == 11) {      // Número de tel com o 9 na frente
                    $(obj).mask("(99) 99999-9999");
                }
            }

            function executaDepoisPostBack() {
                $(function () {
                    exibeCamposTipoEmpresa();
                    exibeCamposTipoContrato();
                    ddlPerfil_OnSelectedIndexChanged();

                    $("[id$=txtCodigoColaboradorSAP]").mask("999999");
                    $("[id$=txtCodigoEmpresaSAP]").mask("999999");

                    $("[id$=txtTelefone]").mask("(99) 9999-9999?9");
                    //$("[id$=txtTelefone]").live("blur", function () { addMaskTelefone(this); });

                    $("[id$=txtCelular]").mask("(99) 9999-9999?9");
                    //$("[id$=txtCelular]").live("blur", function () { addMaskTelefone(this); });

                    $("[id$=txtCEP]").mask("99999-999");
                    $("[id$=txtDataNascimento]").mask("99/99/9999");
                    $("[id$=txtDataInicio]").mask("99/99/9999");
                    $("[id$=txtDataFim]").mask("99/99/9999");
                    $("[id$=txtCPF]").mask("999.999.999-99");
                    $("[id$=txtCNPJ]").mask("99.999.999/9999-99");
                    $("[id$=txtPis]").mask("9999999999-9");
                    $('[id$=txtValorHoraFixo]').priceFormat();
                });
            }

        </script>
           
        <div style="font-style: italic; color: #808080; text-align:center; padding-top:10px; padding-bottom:10px;">
            <h2>Cadastro de Colaborador</h2>
        </div>
        
        <asp:UpdatePanel ID="uppFormulario" runat="server">
            <ContentTemplate>
                
                <table cellpadding="2" cellspacing="2" class="tabelas">
                    <tr>
                        <td colspan="3" align="left">
                            <div style="font-size:10px;">
                                Pesquisa pelo nome do colaborador para alteração de cadastro: &nbsp <br />
                                <asp:TextBox ID="txtPesquisar" runat="server" Width="350" 
                                    ToolTip="Pesquisa pelo nome do colaborador" MaxLength="50" AutoCompleteType="Disabled">
                                </asp:TextBox>                                
                                
                                <asp:Button ID="btnPesquisar" runat="server" Text="OK" Width="60" 
                                    ToolTip="Digite o nome em seguida selecione-o abaixo." />  
                                
                                <asp:AutoCompleteExtender runat="server" ID="autoComplete" 
                                    TargetControlID="txtPesquisar"
                                    ServicePath="~/AutoCompletar.asmx" 
                                    ServiceMethod="getNomesColaboradores"
                                    MinimumPrefixLength="1" 
                                    CompletionInterval="200"
                                    EnableCaching="true"
                                    CompletionSetCount="12" />
                                    
                                <br />
                                <asp:Label ID="lbltxtPesquisar" runat="server" Text=""></asp:Label>                              
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="height:30px;">
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
                </table>
                
            <!-- Campos Dados pessoais -->                
                <table class="tabelas">                    
                    <tr style="height:20px; background-color:Gray;" valign="middle">
                        <td colspan="3">
                            <font color="White">
                                &nbsp <b><asp:Label ID="lblTitulo" runat="server" Text="Novo cadastro"></asp:Label></b>
                            </font>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1"></td>
                        <td colspan="2" valign="middle" align="right" style="height:22px;">
                            <asp:Button ID="btnCadastrarNovo" runat="server" Text="Cadastrar Novo" 
                                ToolTip="Muda o modo para cadastro de novo colaborador" Enabled="true" Visible="false" />
                            <asp:Button ID="btnExibirColaboradores" runat="server" Text="Exibir todos colaboradores" 
                                OnClientClick="novaJanela();" />
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1"></td>
                        <td class="coluna2">
                            <b style="margin-bottom:0px;">Dados pessoais</b>
                        </td>
                        <td class="coluna3"></td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Nome completo:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtNome" runat="server" autocomplete="off" 
                                CssClass="txtBox" MaxLength="50" ToolTip="Digite nome completo" Width="350px">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Cod. Consultor no SAP:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtCodigoColaboradorSAP" runat="server" autocomplete="off" Width="150px"
                                CssClass="txtBox" MaxLength="10" ToolTip="Código cadastrado no SAP do colaborador">
                            </asp:TextBox>
                            <span style="font-size:10px; color:Red; font-style:italic;">
                                <asp:Label ID="lblCodigoColaboradorSAP" runat="server" Text=""></asp:Label>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Login:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtLogin" runat="server" autocomplete="off" 
                                CssClass="txtBox" MaxLength="50" ToolTip="Digite nome de login" Width="200px">
                            </asp:TextBox>
                            <span style="font-size:10px; color:Red; font-style:italic;">
                                <asp:Label ID="lblLogin" runat="server" Text=""></asp:Label>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px; display:block;" id="asteriscoSenha" runat="server">*</div>                        
                        </td>
                        <td class="coluna2">
                            Senha inicial:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtSenhaInicial" runat="server" autocomplete="off" TextMode="Password"  
                                CssClass="txtBox" MaxLength="50" ToolTip="Digite uma senha inicial" Width="150px">
                            </asp:TextBox><br />
                            <span style="font-size:10px; color:Gray;">
                                No campo senha inicial, mínimo de 6 e máximo de 20 caracteres
                            </span>
                        </td>
                    </tr>                             
                </table>

            <!-- Campos Módulo e Nível -->
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="tabelas">
                            <tr>
                                <td class="coluna1">
                                    <div style="color:Red; font-size:14px;">*</div>
                                </td>
                                <td class="coluna2">
                                    Perfil:
                                </td>
                                <td class="coluna3">
                                    <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="txtBox" 
                                      onchange="ddlPerfil_OnSelectedIndexChanged();">                                                                
                                    </asp:DropDownList>
                                </td>
                            </tr>                             
                        </table>
                        <table class="tabelas">
                            <tr id="trModulo" runat="server" style="display:none;">
                                <td class="coluna1">
                                    <div style="color:Red; font-size:14px;">*</div>
                                </td>
                                <td class="coluna2">
                                    Módulo:
                                </td>
                                <td class="coluna3">
                                    <asp:DropDownList ID="ddlModulo" runat="server" CssClass="txtBox">
                                    </asp:DropDownList>
                                    <!-- Para uma futura implementação, um botão para cadastrar módulos
                                    <asp:Button ID="btnCadastrarModulo" runat="server" Text="Adicionar Módulo" Height="24" Width="120" Font-Size="8" />
                                    -->
                                </td>
                            </tr>
                            <tr id="trNivel" runat="server" style="display:none;">
                                <td class="coluna1">
                                    <div style="color:Red; font-size:14px;">*</div>
                                </td>
                                <td class="coluna2">
                                    Nível:
                                </td>
                                <td class="coluna3">
                                    <asp:DropDownList ID="ddlNivel" runat="server" CssClass="txtBox">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPerfil" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>

            <!-- Continuação dos campos "Dados pessoais" -->
                <table class="tabelas">
                    <tr>
                        <td class="coluna1">                            
                        </td>
                        <td class="coluna2">
                            Data de nascimento:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtDataNascimento" runat="server" class="txtBox" MaxLength="10" 
                                Width="80"></asp:TextBox>
                            <asp:calendarextender ID="CalendarExtender3" runat="server" 
                                FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" TargetControlID="txtDataNascimento">
                            </asp:calendarextender>
                            <span style="font-size:10px; color:Gray;"> dd/mm/aaaa </span>
                        </td>
                    </tr>                    
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            RG:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtRG" runat="server" class="maskRG" MaxLength="13" 
                                onblur="toUpper(this);" style="border:3px double #CCCCCC; width: 150px;">
                            </asp:TextBox>
                            <span style="font-size:10px; color:Red; font-style:italic;">
                                <asp:Label ID="lblRG" runat="server" Text=""></asp:Label>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                        </td>
                        <td class="coluna2">
                            Endereço:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtEndereco" runat="server" CssClass="txtBox" MaxLength="100" 
                                Width="350"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                        </td>
                        <td class="coluna2">
                            CEP:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtCEP" runat="server" class="txtBox" MaxLength="9" Width="150"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                        </td>
                        <td class="coluna2">
                            Bairro:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtBairro" runat="server" CssClass="txtBox" MaxLength="50"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                        </td>
                        <td class="coluna2">
                            Cidade:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtCidade" runat="server" class="txtBox" MaxLength="50"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                        </td>
                        <td class="coluna2">
                            UF:
                        </td>
                        <td class="coluna3">
                            <asp:DropDownList ID="ddlUF" runat="server" CssClass="txtBox" Width="60px">
                                <asp:ListItem>AC</asp:ListItem>
                                <asp:ListItem>AL</asp:ListItem>
                                <asp:ListItem>AP</asp:ListItem>
                                <asp:ListItem>AM</asp:ListItem>
                                <asp:ListItem>BA</asp:ListItem>
                                <asp:ListItem>CE</asp:ListItem>
                                <asp:ListItem>DF</asp:ListItem>
                                <asp:ListItem>ES</asp:ListItem>
                                <asp:ListItem>GO</asp:ListItem>
                                <asp:ListItem>MA</asp:ListItem>
                                <asp:ListItem>MT</asp:ListItem>
                                <asp:ListItem>MS</asp:ListItem>
                                <asp:ListItem>MG</asp:ListItem>
                                <asp:ListItem>PA</asp:ListItem>
                                <asp:ListItem>PB</asp:ListItem>
                                <asp:ListItem>PR</asp:ListItem>
                                <asp:ListItem>PE</asp:ListItem>
                                <asp:ListItem>PI</asp:ListItem>
                                <asp:ListItem>RJ</asp:ListItem>
                                <asp:ListItem>RN</asp:ListItem>
                                <asp:ListItem>RS</asp:ListItem>
                                <asp:ListItem>RO</asp:ListItem>
                                <asp:ListItem>RR</asp:ListItem>
                                <asp:ListItem>SC</asp:ListItem>
                                <asp:ListItem Selected="True">SP</asp:ListItem>
                                <asp:ListItem>SE</asp:ListItem>
                                <asp:ListItem>TO</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Telefone:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtTelefone" runat="server" class="txtBox" MaxLength="15" Width="150"></asp:TextBox>
                            <span style="font-size:10px; color:Gray;">(XX) XXXX-XXXXx </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Celular:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtCelular" runat="server" class="txtBox" MaxLength="15" Width="150"></asp:TextBox>
                            <span style="font-size:10px; color:Gray;">(XX) XXXX-XXXXx </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">                            
                        </td>
                        <td class="coluna2">
                            E-mail Addvisor:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtEmail1" runat="server" class="txtBox" MaxLength="100" 
                              Width="350"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">                            
                        </td>
                        <td class="coluna2">
                            E-mail Pessoal:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtEmail2" runat="server" class="txtBox" MaxLength="100" Width="350"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div ID="divAstericoData" style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Data Validade:
                        </td>
                        <td class="coluna3">
                            <!-- Calendario em Ajax -->
                            <b style="font-size:10px;">de &nbsp; </b>
                            <asp:TextBox ID="txtDataInicio" runat="server" class="txtBox" MaxLength="10" Width="80">
                            </asp:TextBox>
                            <asp:calendarextender ID="CalendarExtender1" runat="server" 
                                FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" TargetControlID="txtDataInicio">
                            </asp:calendarextender>
                            <b style="font-size:10px;">&nbsp; até &nbsp;</b>
                            <asp:TextBox ID="txtDataFim" runat="server" class="txtBox" MaxLength="10" Width="80">
                            </asp:TextBox>
                            <asp:calendarextender ID="CalendarExtender2" runat="server" 
                                FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" TargetControlID="txtDataFim">
                            </asp:calendarextender>
                            <span style="font-size:10px; color:Gray;"> dd/mm/aaaa</span>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                        </td>
                        <td class="coluna2">
                            Data Desligamento:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtDtDesligamento" runat="server" class="txtBox" MaxLength="10" Width="80">
                            </asp:TextBox>
                            <asp:calendarextender ID="CalendarExtender4" runat="server" 
                                FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" TargetControlID="txtDtDesligamento">
                            </asp:calendarextender>
                        </td>        
                    </tr>
                    <tr>
                        <td class="coluna1">
                        </td>
                        <td class="coluna2">
                            Tipo de contrato:
                        </td>
                        <td class="coluna3">
                            <b style="font-size:10px;">
                            CLT &nbsp
                            <asp:RadioButton ID="radioCLT" runat="server" GroupName="radioTipoContrato" 
                                onclick="exibeCamposTipoContrato();" Checked="True" />
                            &nbsp &nbsp
                            Pessoa Jurídica &nbsp
                            <asp:RadioButton ID="radioPJ" runat="server" GroupName="radioTipoContrato" 
                                onclick="exibeCamposTipoContrato();" />
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            CPF:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtCPF" runat="server" class='txtBox' MaxLength="14"></asp:TextBox> 
                            <span style="font-size:10px; color:Red; font-style:italic;">
                                <asp:Label ID="lblCPF" runat="server" Text=""></asp:Label>
                            </span>
                        </td>        
                    </tr>

                </table>
               <!--Tabela CLT-->
                 <table id="tbClt" runat="server" style="display:none" class="tabelas">
                    <tr>
                        <td class="coluna1">
                            <%--<div style="color:Red; font-size:14px;">*</div>--%>
                        </td>
                        <td class="coluna2" style="width:174px">
                            Pis:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtPis" runat="server" class='txtBox' MaxLength="11"
                                Width="109"></asp:TextBox> 
                            <span style="font-size:10px; color:Red; font-style:italic;">
                                <asp:Label ID="lblPis" runat="server" Text=""></asp:Label>
                            </span>
                        </td>                            
                    </tr>
                     </table>
                                          <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
            <!-- Campos "tabelaPJ" por padrão escondida -->
                <table id="trPJ1" runat="server" style="display:none" class="tabelas">
                    <tr>  
                        <td class="coluna1">
                                <div style="color:Red; font-size:14px;">*</div>
                        </td>             
                        <td class="coluna2">
                            Tipo de empresa:          
                        </td>                
                        <td class="coluna3">    
   
                                    <asp:DropDownList runat="server" ID="cboTipoEmpresa"  onchange="exibeCamposTipoEmpresa()"  CssClass="txtBox" >
                                    <asp:ListItem Selected="True" Text="Selecione" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Simples" Value="Simples"></asp:ListItem>
                                    <asp:ListItem  Text="Compartilhada" Value="Compartilhada"></asp:ListItem>
                                    <asp:ListItem  Text="ME" Value="ME"></asp:ListItem>
                                    <asp:ListItem  Text="MEI" Value="MEI"></asp:ListItem>
                                    <asp:ListItem  Text="Simples Nacional" Value="Simples Nacional"></asp:ListItem>
                                    <asp:ListItem  Text="LTDA" Value="LTDA"></asp:ListItem>
                                    <asp:ListItem  Text="Presumido" Value="Presumido"></asp:ListItem>
                                </asp:DropDownList>
                             <span style="font-size:10px; color:Red; font-style:italic;">
                                <asp:Label ID="lblTipoEmpresa" runat="server" Text=""></asp:Label>
                            </span>
                      
                        </td> 
                    </tr>
                </table>
                
                <table id="trEmpresaCompartilhada" runat="server" class="tabelas" style="display:none;">
                    <tr>
                        <td class="coluna1">                                
                        </td>
                        <td class="coluna2">
                            <span>Compartilhada:</span>
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtEmpresaCompartilhada" runat="server" class='txtBox' MaxLength="100"
                                Width="350" ToolTip="Nome da empresa compartilhada"></asp:TextBox> 
                        </td>                            
                    </tr>
                </table>
                  </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cboTipoEmpresa" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <table id="trPJ2" runat="server" style="display:none;" class="tabelas">
                    <tr>
                        <td class="coluna1">
                            <div id="div1" style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Razão Social:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtRazaoSocial" runat="server" class='txtBox' MaxLength="100"
                                Width="350"></asp:TextBox> 
                        </td>                            
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div id="div2" style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Inscr. Municipal: 
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtInscrMunicipal" runat="server" class='txtBox' MaxLength="15"></asp:TextBox> 
                        </td>        
                    </tr>                    
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            CNPJ:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtCNPJ" runat="server" class='txtBox' MaxLength="18"></asp:TextBox> 
                            <span style="font-size:10px; color:Red; font-style:italic;">
                                <asp:Label ID="lblCNPJ" runat="server" Text=""></asp:Label>
                            </span>
                        </td>
                    </tr>                             
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Cod. Empresa no SAP:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtCodigoEmpresaSAP" runat="server" class='txtBox' MaxLength="10" Width="150" 
                              ToolTip="Código cadastrado no SAP da empresa">
                            </asp:TextBox>                            
                        </td>        
                    </tr>
                    </tr>
                </table>

            <!-- Tabela "trValor", quando for PJ o checkBox "Fechado" ficará invisivel -->
                <table id="trValor" runat="server" class="tabelas">                     
                    <tr>
                        <td class="coluna1">
                            <div style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Valor hora/fixo:
                        </td>
                        <td class="coluna3">
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtValorHoraFixo" runat="server" class='txtBox' Width="100" MaxLength="15" >
                                        </asp:TextBox>
                                    </td>
                                    <td>
                                        <div id="divFechado" runat="server" style="display:none;">
                                            <asp:CheckBox ID="cbValorFechado" runat="server" Checked="false" /><span> Fechado</span>
                                            <asp:CheckBox ID="chkHoraExtra" runat="server" Text="Hora Extra" />
                                        </div>
                                       
                                    </td>
                                </tr>
                            </table>            
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">                                        
                        </td>
                        <td class="coluna2" style="vertical-align:text-top;">
                            Observação: 
                        </td>
                        <td class="coluna3">
                            <textarea id="txtObservacao" runat="server" Rows="5" cols="220" 
                                onKeyUp="return ismaxlength(this);" style="border: 3px double #CCCCCC; 
                                font-family:Arial; font-size:12px; width:350px;"></textarea>
                        </td>
                    </tr>
                </table>                                    
            <!-- Tabela Dados bancários -->
                <table class="tabelas">
                    <tr>
                        <td class="coluna1">
                        </td>
                        <td colspan="2">
                            <b>Dados Bancário</b>
                        </td>
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div id="div5" style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Banco:
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtBanco" runat="server" CssClass="txtBox" MaxLength="30"></asp:TextBox>
                        </td>                            
                    </tr>
                    <tr>
                        <td class="coluna1">
                            <div id="div6" style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Tipo de Conta:            
                        </td>
                        <td class="coluna3">
                            <b style="font-size:10px;">
                            Conta Corrente &nbsp
                            <asp:RadioButton ID="radioContaC" runat="server" GroupName="radioTipoConta" Checked="true" />
                            &nbsp &nbsp
                            Conta Poupança &nbsp
                            <asp:RadioButton ID="radioContaP" runat="server" GroupName="radioTipoConta" /></b>                             
                        </td>        
                    </tr>   
                    <tr>
                        <td class="coluna1">
                            <div id="div7" style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Agência:            
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtAgencia" runat="server" CssClass="txtBox" MaxLength="15" Width="150">
                            </asp:TextBox>
                        </td>        
                    </tr>  
                    <tr>
                        <td class="coluna1">
                            <div id="div8" style="color:Red; font-size:14px;">*</div>
                        </td>
                        <td class="coluna2">
                            Conta:            
                        </td>
                        <td class="coluna3">
                            <asp:TextBox ID="txtConta" runat="server" CssClass="txtBox" Width="50"
                                MaxLength="10">
                            </asp:TextBox>                                
                            &nbsp Dígito: &nbsp
                            <asp:TextBox ID="txtDigitoConta" runat="server" CssClass="txtBox" Width="27" 
                                MaxLength="3">
                            </asp:TextBox>
                        </td>        
                    </tr>
                    <tr>
                        <td class="coluna1">                           
                        </td>
                        <td class="coluna2">
                            Status:
                        </td>
                        <td class="coluna3">
                            <asp:DropDownList ID="ddlStatus" runat="server" Width="80px" CssClass="txtBox">
                                <asp:ListItem>Ativo</asp:ListItem>
                                <asp:ListItem>Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="padding-top:10px;">
                        <td class="coluna1">
                        </td>
                        <td class="coluna2">                                                                                     
                        </td>
                        <td class="coluna3">
                            <asp:Button ID="btnSalvar" runat="server" Text="Salvar" Height="30" Width="120" />
                            &nbsp &nbsp &nbsp
                            <asp:Button ID="btnLimpar" runat="server" Text="Limpar" Height="30" Width="120" />
                        </td>
                    </tr>
                    <tr style="height:40;">
                        <td class="coluna1">
                        </td>
                        <td class="coluna2">
                        </td>
                        <td class="coluna3">
                            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="uppFormulario" 
                                DisplayAfter="100" DynamicLayout="true" >
                                <ProgressTemplate>
                                    <center>
                                        <div style="float:left;">
                                            <img src="../imagens/progress.gif" 
                                                alt="carregando" style="height:60%; width:60%;" />
                                        </div>
                                        <div style="float:left;font-family:@Microsoft YaHei, Verdana; 
                                            font-size:small; color:Gray;"">
                                            &nbsp;Carregando...
                                        </div>
                                        <br />   
                                    </center>  
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                            <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                            <br />
                            <asp:Label ID="lblMensagem2" runat="server"></asp:Label>
                            <br />
                        </td>
                    </tr>
                </table>
                
                <asp:Label ID="lblTeste" runat="server" CssClass="MsgTestes"></asp:Label>

            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSalvar" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnPesquisar" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="radioCLT" EventName="CheckedChanged" />
                <asp:AsyncPostBackTrigger ControlID="radioPJ" EventName="CheckedChanged" />
            </Triggers>
        </asp:UpdatePanel>
                
  </form>
  
</asp:Content>

