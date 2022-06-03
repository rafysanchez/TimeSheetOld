<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="IntranetVB.paginaConstrucao" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Página em manutenção</title>
    
    <link rel="stylesheet" type="text/css" href="../recursos/addvisor.css" />
    
</head>
<body>
    <form id="form1" runat="server" style="font-family:Arial;">
    <center>    
        <div>
            <img src="../imagens/Logo.png" style="padding-bottom:50px;" />
            <br />
            <img src="../imagens/manutencao2.gif" style="width:5%; height:5%;" />
            <h1 style="color:Gray;">Página em manutenção</h1>
            <p style="color:Gray;">Estamos passando por uma manutenção, tente novamente mais tarde.</p>
            <br />
            <img src="../imagens/manutencao.gif" />
        </div>
    </center>
    <!-- Esta DIV serve somente para o rodapé não tocar em nenhum componente da tela -->
    <div style="height:30px;"></div>
    <div class="rodape">
        <p>Addvisor © 2011 | Todos os Direitos Reservados</p>
    </div>
    </form>
</body>
</html>
