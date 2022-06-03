<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="CheckBoxList.aspx.vb" Inherits="IntranetVB.CheckBoxList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Exemplo: CheckBoxList
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="menuTopo" runat="server">
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server">
    
    <div style="font-style: italic; color: #808080; text-align:center; padding-top:15px; padding-bottom:15px;">
        <h2>CheckBoxList</h2>
    </div>
    
    <form id="form1" runat="server">
    
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        
        <div id="divSelecaoProjeto" style="text-align:left">
        
            <asp:CheckBoxList ID="cblProjetos" runat="server" TextAlign="Left">                
            </asp:CheckBoxList>
            <br />
        
            <div style="float:left">
                <asp:Button ID="btnApontar" runat="server" Text="Exibir código dos Selecionados" />
            </div>
            <br /><br />
        
            <asp:UpdatePanel ID="UpdatePanel1" runat="server"> 
                <ContentTemplate>
                    <asp:Label ID="lblMensagem" runat="server" Text=""></asp:Label>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnApontar" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>  
            
        </div>   
        
    </form>
    
    
    
    
</asp:Content>
