<%@ Page Title="Addvisor - Login" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="Default.aspx.vb" Inherits="IntranetVB._Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
   Apontamento de Horas - Addvisor
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="corpo" runat="server">
    
    <form id="formulario" runat="server" style="text-align:center; height:273px;">
    
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    
    <center style="padding-top:30px;">
    
        <table width='410' style='font-family:Verdana;' cellspacing="5">
            <tr>
                <td align='left' style='background-color:#BF5424; font-weight:bold; height:30px;'>
                    <span style='color:#FFFFFF;'> &nbsp; Login</span>
                </td>
            </tr>
        </table>
        
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
        
                <table width="400" style="font-family:Verdana; font-size:small; text-align:center;" cellspacing="0">
                    <tr style="background-color:#CDCDCD; height:35px;">
                        <td style="width:120px; font-size:smaller;" align='right' valign='middle'>
                            Login:
                        </td>
                        <td align='left' valign='middle'>
                            <asp:TextBox ID="txtLogin" runat="server" CssClass="txtUsuario" Width="230"></asp:TextBox>
                        </td>
                        <td style="width:40px;" align='left' valign='middle'>
                        </td>
                    </tr>
                </table>
                
                <table id="tabSenha" runat="server" width="400" cellspacing="0" style="font-family:Verdana; font-size:small;">
                    <tr style='background-color:#CDCDCD; height:40px;'>
                        <td style="width:120px; font-size:smaller;" align='right' valign='middle'>
                           Senha:
                        </td>
                        <td align='left' valign='middle'>
                            <asp:TextBox ID="txtSenha" runat="server" MaxLength="20" Width="230" TextMode="Password" CssClass="txtSenha">
                            </asp:TextBox>                    
                        </td>
                        <td style="width:40px;" align='left' valign='middle'>                    
                        </td>                   
                    </tr>
                 </table>
                 
                 <table id="tabMensagem" runat="server" width="400" cellspacing="0" style="font-family:Verdana; font-size:small;">   
                    <tr style="background-color:#CDCDCD; height:40px">
                        <td style="width:10px;"></td>               
                        <td style="width:390px; font-size:smaller;" align="center" valign='middle'>
                            Você deve alterar sua senha inicial.<br />
                            Senha de no mínimo 6 caracteres e máximo 20.                                        
                        </td>
                        <td style="width:10px;"></td>
                    </tr>
                 </table>
                 
                 <table id="tabNovaSenha" runat="server" width="400" cellspacing="0" style="font-family:Verdana; font-size:small;">
                    <tr style="background-color:#CDCDCD; height:40px;">
                        <td style="width:120px; font-size:smaller;" align='right' valign='middle'>
                           Nova senha:
                        </td>
                        <td align='left' valign='middle'>
                            <asp:TextBox ID="txtNovaSenha" runat="server" Width="230" TextMode="Password" CssClass="txtSenha">
                            </asp:TextBox>                    
                        </td>
                        <td style="width:40px;" align='left' valign='middle'>                    
                        </td>                   
                    </tr>
                 </table>
                 
                 <table id="tabConfirmaSenha" runat="server" width="400" cellspacing="0" style="font-family:Verdana; font-size:small;">
                    <tr style="background-color:#CDCDCD; height:40px;">
                        <td style="width:120px; font-size:smaller;" align='right' valign='middle'>
                           Confirmar senha:
                        </td>
                        <td align='left' valign='middle'>
                            <asp:TextBox ID="txtConfirmSenha" runat="server" Width="230" 
                                TextMode="Password" CssClass="txtSenha">
                            </asp:TextBox>                    
                        </td>
                        <td style="width:40px;" align='left' valign='middle'>                    
                        </td>                   
                    </tr>
                </table>
                
                <table width="400" cellspacing="0" style="font-family:Verdana; font-size:small;">
                    <tr style='background-color:#CDCDCD; height:45px;'>
                        <td style="font-size:smaller; width:210px;" valign="middle">
                            <asp:LinkButton ID="lnkOutroUser" runat="server">Sou outro usuário</asp:LinkButton>
                            <!--     
                            &nbsp;&nbsp;Esqueceu a senha? Clique
                            <asp:LinkButton ID="lnkEsqueceuSenha" runat="server" OnClientClick="exibeMensagem('Em desenvolvimento, logo estará em funcionanmento.');">aqui</asp:LinkButton>
                            !-->
                        </td>
                        <td valign="middle">
                            <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Text="Login" 
                                Width="120px" Height="32px" />
                        </td>
                    </tr>
                </table> 
                
                <asp:UpdateProgress ID="UpdateProgressTaxas" runat="server" AssociatedUpdatePanelID="UpdatePanel1" 
                    DisplayAfter="500" DynamicLayout="true">
                    <ProgressTemplate>
                        <center>
                            <br />
                            <table>
                                <tr>
                                    <td valign="middle">
                                        <img src="imagens/progress.gif" 
                                            alt="carregando" style="height:60%; width:60%;" />
                                    </td>
                                    <td valign="middle" style="font-family:Microsoft YaHei, Verdana; 
                                            font-size:small; color:Gray;">
                                        Carregando...
                                    </td>
                                </tr>
                            </table>
                            <br />   
                        </center>  
                    </ProgressTemplate>
                </asp:UpdateProgress>
                      
                <div class='error'>
                    <asp:Label ID="lblMensagem" runat="server"></asp:Label><br />                  
                </div> 
                
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnLogin" EventName="Click" />
            </Triggers>            
        </asp:UpdatePanel>
                
        <div>                        
            <object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" width="250" height="200" id="logoAddvisor" align="middle">
                <param name="movie" value="recursos/flash/logo_addvisor.swf"/>
                <!--[if !IE]>-->
                <object type="application/x-shockwave-flash" data="recursos/flash/logo_addvisor.swf" width="250" height="200">
                    <param name="movie" value="recursos/flash/logo_addvisor.swf"/>
                <!--<![endif]-->
                    <a href="http://www.adobe.com/go/getflash">
                        <img src="http://www.adobe.com/images/shared/download_buttons/get_flash_player.gif" alt="Get Adobe Flash player"/>
                    </a>
                <!--[if !IE]>-->
                </object>
                <!--<![endif]-->
            </object>            
        </div>
    
    </center>
                         
</form>

</asp:Content>