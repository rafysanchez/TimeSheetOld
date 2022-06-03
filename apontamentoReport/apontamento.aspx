<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="apontamento.aspx.vb" Inherits="IntranetVB.rvApontamento" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Relatório de Apontamento</title>
    <script type="text/javascript">
        function fechaJanela() {
            window.self.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="font-family:Verdana;">
    
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" ScriptMode="Release">
        </asp:ScriptManager>

        <div align='center' runat='server' id='divRelatorio' visible='true'>
           
            <rsweb:ReportViewer ID="ReportViewerApontamento" runat="server" Font-Names="Verdana" 
                Font-Size="8pt" Width="210mm" Height="297mm" 
                WaitMessageFont-Names="Verdana" WaitMessageFont-Size="12pt"  
                InteractiveDeviceInfos="(Collection)" BackColor="" ClientIDMode="AutoID" HighlightBackgroundColor="" InternalBorderColor="204, 204, 204" InternalBorderStyle="Solid" InternalBorderWidth="1px" LinkActiveColor="" LinkActiveHoverColor="" LinkDisabledColor="" PrimaryButtonBackgroundColor="" PrimaryButtonForegroundColor="" PrimaryButtonHoverBackgroundColor="" PrimaryButtonHoverForegroundColor="" SecondaryButtonBackgroundColor="" SecondaryButtonForegroundColor="" SecondaryButtonHoverBackgroundColor="" SecondaryButtonHoverForegroundColor="" SplitterBackColor="" ToolbarDividerColor="" ToolbarForegroundColor="" ToolbarForegroundDisabledColor="" ToolbarHoverBackgroundColor="" ToolbarHoverForegroundColor="" ToolBarItemBorderColor="" ToolBarItemBorderStyle="Solid" ToolBarItemBorderWidth="1px" ToolBarItemHoverBackColor="" ToolBarItemPressedBorderColor="51, 102, 153" ToolBarItemPressedBorderStyle="Solid" ToolBarItemPressedBorderWidth="1px" ToolBarItemPressedHoverBackColor="153, 187, 226">
            </rsweb:ReportViewer>
        </div>
        
        <div align='center' runat='server' id='divAlerta' visible="false">
            <h4>
                <asp:Label ID="lblMensagem" runat="server" Text=""></asp:Label>      
            </h4>
            <br />
            <p><asp:LinkButton ID="lnkFechar" runat="server">Fechar</asp:LinkButton></p>
        </div>    
    
    </form>
</body>
</html>