<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript">
    function Selecionar(elemento,cordefundo)
    {
        var Inputs = elemento.getElementsByTagName("input");
        var cor = elemento.style.backgroundColor;
        for(var i = 0; i < Inputs.length; ++i)
        {
            if(Inputs[i].type == 'checkbox')
            {
                Inputs[i].checked = !Inputs[i].checked;
                elemento.style.backgroundColor = cordefundo;
                elemento.onclick = function()
                {
                    Selecionar(this,cor);
                };         
            }         
        }       
    }
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:GridView ID="gvSelecao" runat="server" AutoGenerateColumns="False" OnPreRender="gvSelecao_PreRender" CellPadding="3" GridLines="Horizontal" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px">
            <Columns>
                <asp:BoundField HeaderText="Codigo" DataField="Id">
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                </asp:BoundField>
                
                <asp:BoundField HeaderText="Letra" DataField="LETRA">
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                </asp:BoundField>
                
                
                <asp:BoundField HeaderText="Guid" DataField="CODIGO">
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                </asp:BoundField>
                
                
                <asp:TemplateField HeaderText="Select">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkBxSelect" runat="server"   Enabled="false" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                    <HeaderTemplate>
                    </HeaderTemplate>
                </asp:TemplateField>
            </Columns>
            <RowStyle Font-Names="Verdana" Font-Size="10pt" BackColor="#E7E7FF" ForeColor="#4A3C8C" />
            <HeaderStyle BackColor="#4A3C8C" BorderColor="White" BorderStyle="None" BorderWidth="0px"
                Font-Bold="True" Font-Names="Verdana" Font-Size="10pt"
                ForeColor="#F7F7F7" />
            <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
            <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
            <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
            <AlternatingRowStyle BackColor="#F7F7F7" />
        </asp:GridView>
    </form>
</body>
</html>
