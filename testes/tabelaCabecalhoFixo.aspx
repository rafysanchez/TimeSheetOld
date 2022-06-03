<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="tabelaCabecalhoFixo.aspx.vb" Inherits="IntranetVB.tabelaCabecalhoFixo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "DTD/xhtml1-strict.dtd">
<html><head>
<title>Tabela com cabeçalho fixo</title>
<style type="text/css">

body { font: normal 11px tahoma,arial,serif; }

.tabela{margin: 0px;}
#lista table {width: 350px;}
#lista th{color: #FFFFFF;background-color: #E92345;text-align: left}
.tabContainer {width: 367px;border: 1px solid #000000;border-right: 0px;border-top: 0px;}
#lista .scrollContainer {width: 367px;height: 100px;overflow: auto;}
#lista .tabela-coluna0{width: 99px;}
#lista .tabela-coluna1{width: 149px;}
#lista .tabela-coluna2{width: 99px;}

</style>
</head>
<body>

<div class="tabContainer" id="lista">
       
        <table border="0px" class="tabela">
            <thead>
                <tr>
                    <th class="tabela-coluna0"><span>CPF</span></th>
                    <th class="tabela-coluna1"><span>Nome</span></th>
                    <th class="tabela-coluna2"><span>Cargo</span></th>
                </tr>
            </thead>
        </table>
       
        <div class="scrollContainer">
            <table border="0" class="tabela">
                <tbody>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                    <tr>
                        <td class="tabela-coluna0"><span>111.111.111-11</span></td>
                        <td class="tabela-coluna1"><span>Jorge Luiz da Silva</span></td>
                        <td class="tabela-coluna2"><span>teste</span></td>
                    </tr>
                </tbody>
            </table>
        </div>
</div>

<br /><br />

<table>
    <tr>
        <td>
            Teste 1
        </td>
        <td>
            Teste 2
        </td>
    </tr>
</table>

</body>
</html>
