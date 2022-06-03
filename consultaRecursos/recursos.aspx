<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" 
    CodeBehind="recursos.aspx.vb" Inherits="IntranetVB.recursos" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Consulta de recursos
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    
    <style type="text/css">
        
        .divBloqueio {
            -moz-user-select: none;
            -moz-user-focus:ignore;
            -moz-user-input:disabled; 
        }
        
        .invisivel {
            display:none;	
        }  
        
        #mask {
            position:absolute;
            left:0;
            top:0;
            z-index:9000;
            background-color:#000;
            display:none;
        }
        
        #boxes .window {
            position:absolute;
            left:0;
            top:0;
            width:440px;
            height:200px;
            display:none;
            z-index:9999;
            padding:20px;
        }
        
        #boxes #dialog{
            width:auto; 
            height:auto;
            padding:10px;
            background-color:#ffffff;
        }
        
        .close{display:block; text-align:right;}
              
    </style>
    
    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script>
    <script type="text/javascript" src="../recursos/jQuery/jquery.maskedinput-1.2.2.js"></script>
    <script type="text/javascript" src="../recursos/jQuery/jquery.superbox.js"></script>

    <script type="text/javascript">
                    
        var message="Sinto muito, botão direito do mouse está desabilitado.";
        //this will register click funtion for all the mousedown operations on the page
        document.onmousedown = click;
        function click(e) {
            //for Internet Explore..'2' is for right click of mouse
            if (event.button == 2) {
                alert(message);
                return false;
            }
            //for other browsers like Netscape 4 etc..
            if (e.which == 3){
                alert(message);
                return false;
            }
        };
        
        function limparDados() {
            clipboardData.clearData();
        };
        setInterval(limparDados, 100);         

        function novaJanela() {
            var w = 1000;
            var h = 700;
            var LeftPosition = (screen.width) ? (screen.width - w) / 2 : 0;
            var settings = "width=" + w + ",height=" + h + ",top=0,left=" + LeftPosition +
                                ",toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes";

            //var URL = $("[id$=txtURL]").val();
            var URL = <%=URL %>;

            window.open(URL, "_blank", settings);
        }; 
                          
    </script>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server">

<div style="font-style: italic; color: #808080; text-align:center; padding-bottom:10px;">
    <h2>Recursos</h2>
</div>

<form id="formulario" runat="server" style="font-family:Verdana; font-size:12px;">

    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>

    <!-- Este Script deve-se ficar depois do ToolkitScriptManager -->
    <script type="text/javascript">

        $(document).ready(function () {
            executaDepoisPostBack();
        });
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            executaDepoisPostBack();
        });
               
        function soNumeros(str) {
            str = str.toString();
            return str.replace(/\D/g, '');
        }

        function addMaskTelefone(obj) {
            $(obj).unmask();
            var num = soNumeros(obj.value);
            if ( num.length == 10 ) {           // Número de tel sem o 9 na frente
                $(obj).mask("(99) 9999-9999?9");
            }else if ( num.length == 11 ) {     // Número de tel com o 9 na frente
                $(obj).mask("(99) 99999-9999");
            }
        }
                
        function executaDepoisPostBack() {
            $(function () {
                $("[id$=txtTelefone1]").mask("(99) 9999-9999?9");
                //$("[id$=txtTelefone1]").live("blur", function () { addMaskTelefone(this); });

                $("[id$=txtTelefone2]").mask("(99) 9999-9999?9");
                //$("[id$=txtTelefone2]").live("blur", function () { addMaskTelefone(this); });

                $("[id$=txtTelefone3]").mask("(99) 9999-9999?9");
                //$("[id$=txtTelefone3]").live("blur", function () { addMaskTelefone(this); });

                $("[id$=txtDataInicio]").mask("99/99/9999");
                $("[id$=txtDataFim]").mask("99/99/9999");
                $("[id$=txtFiltroDataInicio]").mask("99/99/9999");
                $("[id$=txtFiltroDataFim]").mask("99/99/9999");
            });                     
        }

        function noCopy(teclapress) {
            if (navigator.appName == "Netscape") { tecla = teclapress.which; }
            else { tecla = teclapress.keyCode; }

            var ctrl = teclapress.ctrlKey;

            if (ctrl && tecla == 67) { return false; }
            if (ctrl && tecla == 86) { return false; }
        };

        function toLowerCase(campo) {
            campo.value = campo.value.toLowerCase();
        };

    </script>
                
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
                    
            <div style="text-align:left; margin-bottom:10px;">
                <fieldset style="width:900px;">
                    <legend><b>Pesquisa por filtros</b></legend>
                    <table cellspacing="2">
                        <tr>
                            <td style="width:60px;">
                                Nome:
                            </td>
                            <td colspan="3">
                                <asp:TextBox ID="txtFiltroNome" runat="server" Width="420"></asp:TextBox>
                                <asp:AutoCompleteExtender
                                    runat="server"
                                    ID="autoComplete"
                                    TargetControlID="txtFiltroNome"
                                    ServicePath="~/AutoCompletar.asmx"
                                    ServiceMethod="getNomesRecursos"
                                    MinimumPrefixLength="1"
                                    CompletionInterval="200"
                                    EnableCaching="true"
                                    CompletionSetCount="10" />
                            </td>
                            <td>
                                <asp:CheckBox ID="ckbSemEmail" runat="server" /> Sem e-mail cadastrado                                
                            </td>
                            <td rowspan="3" style="width:40px; text-align:center;">
                                <asp:ImageButton ID="btnPesquisar" runat="server" ImageUrl="~/imagens/lupa.gif" 
                                 Height="30" Width="30" ToolTip="Realiza a pesquisa aplicando os filtros selecionados" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Frente: 
                            </td>
                            <td style="width:200px;" align="left">
                                <asp:DropDownList ID="ddlModulo" runat="server" Width="150">
                                </asp:DropDownList>
                            </td>
                            <td style="width:65px;">
                                Alocado:    
                            </td>
                            <td style="width:160px;" align="left">
                                <asp:DropDownList ID="ddlAlocado" runat="server" Width="150">
                                    <asp:ListItem>Todos</asp:ListItem>
                                    <asp:ListItem>Indefinido</asp:ListItem>
                                    <asp:ListItem>Sim</asp:ListItem>
                                    <asp:ListItem>Não</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:CheckBox ID="ckbSemTelefone" runat="server" /> Sem telefone cadastrado 
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Skill:
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlSkill" runat="server" Width="150">
                                </asp:DropDownList>
                            </td>
                            <td colspan="2">
                                <img alt="Filtro por data" src="../imagens/icone-calendario.jpg" /> de 
                                <asp:TextBox ID="txtFiltroDataInicio" runat="server" Width="60" Font-Size="11px"></asp:TextBox> até 
                                <asp:TextBox ID="txtFiltroDataFim" runat="server" Width="60" Font-Size="11px"></asp:TextBox>
                                <asp:calendarextender ID="CalendarExtender3" runat="server" 
                                    FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" TargetControlID="txtFiltroDataInicio">
                                </asp:calendarextender>
                                <asp:calendarextender ID="CalendarExtender4" runat="server" 
                                    FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" TargetControlID="txtFiltroDataFim">
                                </asp:calendarextender>                            
                            </td>
                            <td>
                                <asp:CheckBox ID="ckbSemDataAlocacao" runat="server" /> Sem data de alocação definida
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </div>   
            
            <%--  Div da GrivView  --%>
            <div style="width:auto;">            
                <asp:GridView 
                    id="gridRecursos" 
                    Runat="Server" 
                    DataSourceID="fonteDados"
                    DataKeyNames="recCodigo"
                    AutoGenerateColumns="False"
                    AllowPaging="True" 
                    AllowSorting="True"
                    CellPadding="4" 
                    ForeColor="#333333" 
                    GridLines="Vertical"
                    PageSize="20">
                    <RowStyle BackColor="#F7F6F3" Font-Size="11px" Height="13px" HorizontalAlign="Left" ForeColor="#333333" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#999999" />
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Justify" Wrap="true" Font-Underline="true" />
                    <PagerSettings Position="TopAndBottom" PageButtonCount="30" />
                                 
                    <Columns>
                        
                        <asp:BoundField DataField="recCodigo">
                            <HeaderStyle CssClass="invisivel" />
                            <ItemStyle CssClass="invisivel" />
                        </asp:BoundField>
                        
                        <asp:CommandField ButtonType="Image" SelectImageUrl="~/imagens/editar.png"
                            SelectText="Selecionar" ShowSelectButton="True" />
                        
                        <asp:BoundField HeaderText="Frente" DataField="recFrente" HtmlEncode="false" 
                          ItemStyle-CssClass="divBloqueio" />
                        <asp:BoundField HeaderText="Skill" DataField="recSkill" HtmlEncode="false"
                          ItemStyle-CssClass="divBloqueio"  />
                        <asp:BoundField HeaderText="Nome" DataField="recNome" HtmlEncode="false" SortExpression="Teste" 
                          ItemStyle-CssClass="divBloqueio" />
                        <asp:BoundField HeaderText="Telefone 1" DataField="recTelefone1" HtmlEncode="false" 
                          ItemStyle-CssClass="divBloqueio" />
                        <asp:BoundField HeaderText="Telefone 2" DataField="recTelefone2" HtmlEncode="false" 
                          ItemStyle-CssClass="divBloqueio" />
                        <asp:BoundField HeaderText="Telefone 3" DataField="recTelefone3" HtmlEncode="false" 
                          ItemStyle-CssClass="divBloqueio" />
                        <asp:TemplateField HeaderText=" E-mail ">
                            <ItemTemplate>
                                <asp:HyperLink ID="linkEmail" runat="server" Text='<%#Eval("recEmail") %>'
                                 NavigateUrl='<%#Eval("recEmail","mailto:{0}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText=" MSN ">
                            <ItemTemplate>
                                <asp:HyperLink ID="linkMSN" runat="server" Text='<%#Eval("recMSN") %>'
                                 NavigateUrl='<%#Eval("recMSN","mailto:{0}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Alocado" DataField="recAlocado" HtmlEncode="false" 
                          ItemStyle-CssClass="divBloqueio" />
                        <asp:BoundField HeaderText="De" DataField="recDataInicio" HtmlEncode="false" 
                          ItemStyle-CssClass="divBloqueio" />
                        <asp:BoundField HeaderText="Até" DataField="recDataFim" HtmlEncode="false" 
                          ItemStyle-CssClass="divBloqueio" />

                        <asp:BoundField DataField="recObservacao">
                            <HeaderStyle CssClass="invisivel" />
                            <ItemStyle CssClass="invisivel" />
                        </asp:BoundField>
                                                
                    </Columns>
                    
                    <EmptyDataTemplate>
                        <div>
                            <center style="width:900px; text-align:center;">
                                <div style="height:40px; padding-top:20px; padding-bottom:70px;" >
                                    <p style="color:Blue; font-style:italic; font-size:16px;" >
                                        A consulta não retornou nenhum dado...
                                    </p>
                                </div>
                            </center>
                        </div>                        
                    </EmptyDataTemplate>
                                        
                </asp:GridView>

                <div style="text-align:left; padding-top:7px;">
                    <a id="hplExibirEmails" runat="server" style="text-align:left;">Selecionar e-mails</a>
                    <a id="hplEnviarEmailCCO" runat="server" style="text-align:left;"> | Enviar e-mail em CCO para todos</a>
                </div>                

            </div>
            
            <!-- ConnectionString é setado através de uma função -->
            <asp:SqlDataSource 
                ID="fonteDados" 
                runat="server"
                SelectCommand="SELECT * FROM [v_recursos] ORDER BY recNome">                
            </asp:SqlDataSource>
            
            <div style="height:20px;">
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" 
                   DisplayAfter="100" DynamicLayout ="true">
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
                            </center>                        
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </div>
                  
            <%--  Campos de Edição  --%>
            <fieldset style="width:900px;">
                <legend><b>Dados</b></legend>
                <table style="padding-top:7px;">
                    <tr>
                        <td style="width:10px;"></td>
                        <td style="width: 100px;" align="right">
                            Código:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtCodigo" runat="server" CssClass="fundoCinza" 
                                ReadOnly="true" Width="80"></asp:TextBox>
                        </td>
                        <td></td>
                        <td>
                            <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="color:Red;">
                            <asp:Label ID="lblAsteriscoFrente" runat="server" Text="*"></asp:Label>
                        </td>
                        <td align="right">
                            Frente:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtFrente" runat="server" CssClass="fundoCinza" MaxLength="30" 
                                ReadOnly="true" Width="200"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="color:Red;">
                            <asp:Label ID="lblAsteriscoSkill" runat="server" Text="*"></asp:Label>
                        </td>
                        <td align="right">
                            Skill:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtSkill" runat="server" MaxLength="30" Width="200"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="color:Red;">
                            <asp:Label ID="lblAsteriscoNome" runat="server" Text="*"></asp:Label>
                        </td>
                        <td align="right">
                            Nome:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNome" runat="server" CssClass="fundoCinza" MaxLength="100" 
                               ReadOnly="true" Width="300"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            Telefone 1:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtTelefone1" runat="server" MaxLength="15" Width="100">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            Telefone 2:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtTelefone2" runat="server" MaxLength="15" Width="100">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            Telefone 3:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtTelefone3" runat="server" MaxLength="15" Width="100">
                        </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            E-mail:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" 
                                onblur="toLowerCase(this)" Width="300">
                        </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            MSN:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtMSN" runat="server" MaxLength="100" 
                                onblur="toLowerCase(this)" Width="300">
                        </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            Alocado:&nbsp;&nbsp;
                        </td>
                        <td align="left">                                                        
                            Sim<input type="radio" id="radioAlocSim" runat="server" name="alocado" /> &nbsp;&nbsp;
                            Não<input type="radio" id="radioAlocNao" runat="server" name="alocado" />                                                       
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            De:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtDataInicio" runat="server" MaxLength="10" Width="80"></asp:TextBox>
                            <asp:calendarextender ID="CalendarExtender1" runat="server" 
                                FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" TargetControlID="txtDataInicio">
                            </asp:calendarextender>
                            <span style="font-size:10px; color:Gray;">dd/mm/aaaa</span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            Até:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtDataFim" runat="server" MaxLength="10" Width="80"></asp:TextBox>
                            <asp:calendarextender ID="CalendarExtender2" runat="server" 
                                FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" TargetControlID="txtDataFim">
                            </asp:calendarextender>
                            <span style="font-size:10px; color:Gray;">dd/mm/aaaa </span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right" valign="top">
                            Observações:&nbsp;&nbsp;
                        </td>
                        <td align="left">
                            <textarea ID="txtObservacao" runat="server" cols="40" name="S1" rows="4"></textarea>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td align="left" 
                            style="vertical-align:middle; padding-bottom:7px; padding-top:10px;">
                            <asp:Button ID="btnAtualizar" runat="server" Text="Atualizar" Width="100" />
                            <asp:Button ID="btnExcluir" runat="server" OnClick="excluirRegistro" 
                                OnClientClick="return confirm('Você tem certeza que deseja apagar este registro?');" 
                                Text="Excluir" Width="100" />
                            <asp:Button ID="btnAdicionarSalvar" runat="server" Text="Salvar" Width="100" />
                        </td>                            
                    </tr>                    
                </table>            
            </fieldset>
            
        </ContentTemplate>    
    </asp:UpdatePanel>

    <div id="boxes">
        <div id="dialog" class="window">
            <a href="#" class="close">Fechar [X]</a><br />
            Teste Janela Modal        
        </div>
    </div>     
                       
    <!-- Máscara para cobrir a tela -->
    <div id="mask"></div>       
                           
</form>

</asp:Content>
