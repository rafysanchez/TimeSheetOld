<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="boasVindas.aspx.vb" Inherits="IntranetVB.boasVindas1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Bem-Vindo <%Response.Write(Session("colNomeLogado"))%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    
    <script type="text/javascript" src="recursos/JS/swfobject.js"></script>

	<script type="text/javascript">
	    // A Adobe recomenda que os desenvolvedores usem o SWFObject2 para detectar o Flash Player.
	    // Para obter mais informações, consulte a página SWFObject no código do Google (http://code.google.com/p/swfobject/). 
	    // As informações também são disponíveis na Adobe Developer Connection em "Detectando versões do Flash Player e incorporando os arquivos SWF com o SWFObject 2" 
	    // Definir para a versão mínima exigida do Flash Player ou 0 caso não seja detecada nenhuma versão  
	    var swfVersionStr = "8.0.24";
	    // Pode-se usar xiSwfUrlStr para definir um SWF com instalação rápida. 
	    var xiSwfUrlStr = "";
	    var flashvars = {};
	    var params = {};
	    params.quality = "high";
	    params.bgcolor = "#ffffff";
	    params.play = "true";
	    params.loop = "true";
	    params.wmode = "transparent";
	    params.scale = "showall";
	    params.menu = "true";
	    params.devicefont = "false";
	    params.salign = "";
	    params.allowscriptaccess = "sameDomain";
	    var attributes = {};
	    attributes.id = "logo_addvisor";
	    attributes.name = "logo_addvisor";
	    attributes.align = "middle";
	    swfobject.createCSS("html", "height:100%; background-color: #ffffff;");
	    swfobject.createCSS("body", "margin:0; padding:0; overflow:hidden; height:100%;");
	    swfobject.embedSWF(
			"recursos/flash/logo_addvisor.swf", "flashContent",
			"500", "400",
			swfVersionStr, xiSwfUrlStr,
			flashvars, params, attributes
        );
	</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server"> 

    <form id="form1" runat="server">
        
        <!-- O método incorporado dinâmico do SWFObject substitui este conteúdo HTML alternativo para conteúdo 
            Flash quando já há disponível suporte suficiente ao plug-in JavaScript e Flash. -->
		<div id="flashContent">
			<a href="http://www.adobe.com/go/getflash">
				<img src="http://www.adobe.com/images/shared/download_buttons/get_flash_player.gif" alt="Obter Adobe Flash Player" />
			</a>
			<p>Essa página exige o Flash Player versão 8.0.24 ou superior.</p>
		</div>
                
    </form>

</asp:Content>
