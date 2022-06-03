<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="exibeEmails.aspx.vb" Inherits="IntranetVB.exibeRecursos"
 ViewStateMode="Inherit" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Seleção de e-mails</title>

    <style type="text/css">
        .invisivel
        {
            display:none;	
        }
    </style>

    <script type="text/javascript">

    </script>

</head>
<body>
    <form id="form1" runat="server" style="font-family:Verdana; font-size:12px;">
    
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
    
        <table>
            <tr>
                <td>
                    <a id="voltar1" runat="server">Voltar</a>
                </td>
                <td style="width:400px; text-align:center;">
                    <div style="height:15px;">
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" 
                            DisplayAfter="100">
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
                </td>
            </tr>
        </table>
                       
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                
                <asp:Button ID="btnCarregaPagina" runat="server" Text="" />
                        
                <div style="width:auto;">
            
                    <div style="padding-bottom:12px; font-size:12px; height:12px;">
                        <asp:HyperLink NavigateUrl="" ID="hplEnviarEmails1" Visible="false" 
                          runat="server"> Enviar e-mail em CCO para os e-mails selecionados</asp:HyperLink>
                    </div>                        
                    
                    <div style="font-size:10px; color:Blue; padding-bottom:5px;">
                        <asp:Label ID="lblMensagem" runat="server" Text="Nenhum consultor selecionado e nenhum e-mail."></asp:Label> 
                    </div> 

                    <asp:GridView 
                        id="gridRecursos"
                        Runat="Server"
                        DataSourceID="fonteDados"
                        DataKeyNames="recCodigo"
                        AutoGenerateColumns="False"
                        AllowSorting="True"
                        AllowPaging="True"
                        PageSize="50"
                        CellPadding="4"
                        ForeColor="#333333" 
                        GridLines="Vertical">
                        <RowStyle BackColor="#F7F6F3" Font-Size="11px" Height="13px" HorizontalAlign="Left" ForeColor="#333333" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <EditRowStyle BackColor="#999999" />
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" Wrap="true" />
                        <PagerSettings Position="TopAndBottom" PageButtonCount="30" />
                        
                        <Columns>
                        
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="ckbSelTodos" runat="server" OnCheckedChanged="ckbSelTodos_CheckedChanged"
                                     AutoPostBack="true" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckbSelItem" runat="server" OnCheckedChanged="ckbSelItem_CheckedChanged" 
                                     AutoPostBack="true"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        
                            <asp:BoundField DataField="recCodigo">
                                <HeaderStyle CssClass="invisivel" />
                                <ItemStyle CssClass="invisivel" />
                            </asp:BoundField>
                        
                            <asp:BoundField HeaderText="Frente" DataField="recFrente" HtmlEncode="false" Visible="false" />

                            <asp:BoundField HeaderText="Skill" DataField="recSkill" HtmlEncode="false" Visible="false" />

                            <asp:BoundField HeaderText="Nome" DataField="recNome" HtmlEncode="false" />

                            <asp:BoundField HeaderText="Telefone 1" DataField="recTelefone1" HtmlEncode="false" Visible="false" />

                            <asp:BoundField HeaderText="Telefone 2" DataField="recTelefone2" HtmlEncode="false" Visible="false" />

                            <asp:BoundField HeaderText="Telefone 3" DataField="recTelefone3" HtmlEncode="false" Visible="false" />

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

                            <asp:BoundField HeaderText="Alocado" DataField="recAlocado" HtmlEncode="false" />

                            <asp:BoundField HeaderText="De" DataField="recDataInicio" HtmlEncode="false" />

                            <asp:BoundField HeaderText="Até" DataField="recDataFim" HtmlEncode="false" />

                            <asp:BoundField DataField="recObservacao">
                                <HeaderStyle CssClass="invisivel" />
                                <ItemStyle CssClass="invisivel" />
                            </asp:BoundField>
                                                
                        </Columns>
                    
                        <EmptyDataTemplate>
                            <center>
                                <div style="height:40px; padding-top:20px; padding-bottom:70px;" >
                                    <p style="color:Blue; font-style:italic; font-size:16px;" >
                                        A consulta não retornou nenhum dado...
                                    </p>
                                </div>
                            </center>
                        </EmptyDataTemplate>
                                        
                    </asp:GridView>            
                    
                    <div style="padding-top:12px; font-size:12px; height:12px;">
                        <asp:HyperLink NavigateUrl="" ID="hplEnviarEmails2" Visible="false" 
                          runat="server"> Enviar e-mail em CCO para os e-mails selecionados</asp:HyperLink>
                    </div>
                    
                    <br /><br />
                    <a id="voltar2" runat="server">Voltar</a>

                </div>
        
                <!-- stringConexao - vem do modulo funções, uma variavel publica -->
                <asp:SqlDataSource 
                    ID="fonteDados" 
                    runat="server">
                </asp:SqlDataSource>
                        
            </ContentTemplate>
        </asp:UpdatePanel>

    </form>
</body>
</html>
