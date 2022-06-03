<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="listview_com_checkBox.aspx.vb" Inherits="IntranetVB.listview_com_checkBox" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ListView com checBox</title>
    
    <style type="text/css">
        /*
        div.scrollTable table.header, div.scrollTable div.scroller table{
            width: 100%;
            border-style: none;
        }
        div.scrollTable table.header{}
        div.scrollTable table.header th, div.scrollTable div.scroller table td{
            border-style: none;
        }
        div.scrollTable table.header th{
            background: #ddd;
            
        }
        div.scrollTable div.scroller{
            height: 150px;
            overflow: auto;
        }        
        div.scrollTable .colunaCheckBox{width: 20px;}
        div.scrollTable .coluna0{width: 300px; font-size:10px;}
        div.scrollTable .coluna1{width: 60px; font-size:10px;}
        div.scrollTable .coluna2{width: 60px; font-size:10px;}
        div.scrollTable .coluna3{width: 60px; font-size:10px;}
        div.scrollTable .coluna4{width: 100px; font-size:10px;}        
        */
        
        table 
        {
            font: normal 11px "Trebuchet MS", Verdana, Arial;                                              
            background: #fff;                                 
            border:solid 1px #C2EAD6;
        }
        td
        {
            padding: 3px 3px 3px 6px;
            color: #5D829B;
        }
        th 
        {
        	vertical-align: middle;
            font-weight: bold;
            font-size: smaller;
            color: #5D728A;
            padding: 0px 3px 3px 6px;
            background: #CAE8EA;
        }
        
    </style>
        
</head>
<body>
    <form id="form1" runat="server" style="font-family:Microsoft JhengHei;">
                  
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>                  
                              
        <center>                    
        <div class="scrollTable" style="width:640px; text-align:center;">
            
            <div style="margin-left:0px;">
            <table class="header">
                <tr>
                    <th class="colunaCheckBox"></th>
                    <th class="coluna0">Nome</th>
                    <th class="coluna1">Módulo</th>
                    <th class="coluna2">Nível</th>
                    <th class="coluna3">Contrato</th>
                    <th class="coluna4">Taxa Atual</th>
                    <th style="width:13px;"></th>
                </tr>                            
            </table>
            </div>                        
                     
            <!-- Lista de todos os consultores -->
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="scroller">
                        <!-- Tabela "Todos os consultores" -->
                        <asp:ListView ID="lstViewTodosConsultores" runat="server" ItemPlaceholderID="lstConsultores"
                          DataKeyNames="colCodigo,nome,modulo,nivel,contrato,taxaAtual">
                                            
                            <LayoutTemplate>                                        
                                <table>                                            
                                    <tr style="display:none;">
                                        <th>1</th><th>2</th><th>3</th><th>4</th><th>5</th><th>6</th>
                                    </tr>                                            
                                    <tr runat="server" ID="lstConsultores">                                      
                                    </tr>                                           
                                </table>                                       
                            </LayoutTemplate>
                            
                            <ItemTemplate>                    
                                <tr onmouseover="this.style.backgroundColor='#FFBBFF'" 
                                    onmouseout="this.style.backgroundColor='#FFFFFF'">
                                    <td class="colunaCheckBox">                                            
                                        <asp:CheckBox ID="chkConsultores" runat="server" />
                                    </td>
                                    <td style="display:none;">
                                        <asp:Label ID="lblColCodigo" runat="server" Text='<%# Eval("colCodigo") %>' />
                                    </td>
                                    <td class="coluna0" align="left">
                                        <asp:Label ID="lblNome" runat="server" Text='<%# Eval("nome") %>' />
                                    </td>                            
                                    <td class="coluna1" align="left">                                                                       
                                        <asp:Label ID="lblModulo" runat="server" Text='<%# Eval("modulo") %>' />                               
                                    </td>
                                    <td class="coluna2" align="left">                                             
                                        <asp:Label ID="lblNivel" runat="server" Text='<%# Eval("nivel") %>' />
                                    </td>
                                    <td class="coluna3" align="left">                                             
                                        <asp:Label ID="lblContrato" runat="server" Text='<%# Eval("contrato") %>' />
                                    </td>
                                    <td class="coluna4" align="left">                                            
                                        <asp:Label ID="lblTaxaAtual" runat="server" Text='<%# Eval("taxaAtual") %>' />
                                    </td>                                        
                                </tr>
                            </ItemTemplate>   
                            
                            <EmptyDataTemplate>
                                <table border="0" cellpadding="6" cellspacing="0">
                                    <thead style="background-color:#FF5500;">
                                        <tr>
                                            <td><b style="color:White; font-size:12px;">Nome</b></td>
                                            <td><b style="color:White; font-size:12px;">Módulo</b></td>
                                            <td><b style="color:White; font-size:12px;">Nível</b></td>
                                            <td><b style="color:White; font-size:12px;">Contrato</b></td>
                                            <td><b style="color:White; font-size:12px;">Taxa atual</b></td>
                                         </tr>
                                    </thead>
                                    
                                    <tbody>
                                        <tr style="height:100px;"></tr>
                                    </tbody>
                                    
                                    <tfoot>
                                        <tr style="height:5px;">
                                            <td colspan="5" style="background-color:#FF5509"></td>
                                        </tr>
                                    </tfoot>
                                    
                                </table>
                            </EmptyDataTemplate>
                        
                        </asp:ListView>                    
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnTeste" EventName="Click" />
                </Triggers>   
            </asp:UpdatePanel> 
            
            <!-- Lista de consultores no projeto -->
            
        </div>
        </center>
        <br /><br />  
                        
        <center>
            <asp:Button ID="btnTeste" runat="server" Text="Teste" /><br />
            <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
        </center>
            
        <!-- gridView todos os consultores -->
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div align="center">
                    <!-- Tabela "Todos os consultores" -->
                    <asp:GridView ID="gvTodosConsultores" runat="server" AllowPaging="true" PageSize="10" 
                        PagerSettings-Mode="Numeric">
                        <Columns>
                            <asp:CheckBoxField />
                        </Columns>
                        <pagerTemplate>
                            <asp:ImageButton ID="imgFirstPage" runat="server" ImageUrl="~/imagens/firstPage.png"
                                CommandArgument="first" CommandName="Page" OnCommand="paginate" /> &nbsp;
                            <asp:ImageButton ID="imgPrevious" runat="server" ImageUrl="~/imagens/previous.png"
                                CommandArgument="prev" CommandName="Page" OnCommand="paginate" /> &nbsp;
                            Página &nbsp;
                            <asp:DropDownList ID="ddlPages" runat="server" AutoPostBack="True" Width="45"
                                OnSelectedIndexChanged="ddlPages_SelectedIndexChanged">
                            </asp:DropDownList> &nbsp;
                            de &nbsp;
                            <asp:Label ID="lblPageCount" runat="server"></asp:Label> &nbsp;
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/imagens/next.png" 
                                CommandArgument="next" CommandName="Page" OnCommand="paginate" /> &nbsp;
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/imagens/lastPage.png" 
                                CommandArgument="last" CommandName="Page" OnCommand="paginate" />
                        </pagerTemplate>
                    </asp:GridView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnTeste" EventName="Click" />
            </Triggers>   
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
