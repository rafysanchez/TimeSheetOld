<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="exemplo.aspx.vb" Inherits="IntranetVB.exemplo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Exemplo de página utilizando ASP.NET com VB.NET</title>
            
    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script>
   
    <script type="text/javascript">
        var timeout = 500;
        var closetimer = 0;
        var ddmenuitem = 0;

        function jsddm_open() {
            jsddm_canceltimer();
            jsddm_close();
            ddmenuitem = $(this).find('ul').eq(0).css('visibility', 'visible');
        }

        function jsddm_close()
        { if (ddmenuitem) ddmenuitem.css('visibility', 'hidden'); }

        function jsddm_timer()
        { closetimer = window.setTimeout(jsddm_close, timeout); }

        function jsddm_canceltimer() {
            if (closetimer) {
                window.clearTimeout(closetimer);
                closetimer = null;
            } 
        }

        $(document).ready(function() {
            $('#jsddm > li').bind('mouseover', jsddm_open);
            $('#jsddm > li').bind('mouseout', jsddm_timer);
        });

        document.onclick = jsddm_close;
    </script>
    
    <style type="text/css">
        /* menu styles */
        #jsddm
        {	
        	margin: 0;
	        padding: 0
	    }
	    #jsddm li
	    {	
	    	float: left;
		    list-style: none;
		    font: 12px Tahoma, Arial
		}
	    #jsddm li a
	    {	
	    	display: block;
		    background: #324143;
		    padding: 5px 12px;
		    text-decoration: none;
		    border-right: 1px solid white;
		    width: 70px;
		    color: #EAFFED;
		    white-space: nowrap
		}
	    #jsddm li a:hover
	    {	
	    	background: #24313C
	    }    		
		#jsddm li ul
		{	margin: 0;
			padding: 0;
			position: absolute;
			visibility: hidden;
			border-top: 1px solid white
		}		
		#jsddm li ul li
		{
			float: none;
			display: inline
		}			
		#jsddm li ul li a	
		{
			width: auto;
			background: #A9C251;
			color: #24313C
		}			
	    #jsddm li ul li a:hover
	    {
	    	background: #8EA344
	    }
    </style>
    
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="TextBox1" runat="server" Width="200">Texto no campo</asp:TextBox>
            <br />
            <asp:Button ID="Button1" runat="server" Text="Limpar" />
        </div>
        <asp:Label ID="lblMensagem" runat="server" Text=""></asp:Label>
        
        <br /><br />
        
        <ul id="jsddm">
	        <li><a href="#">JavaScript</a>
		        <ul>
			        <li><a href="#">Drop Down Menu</a></li>
			        <li><a href="#">jQuery Plugin</a></li>
			        <li><a href="#">Ajax Navigation</a></li>
		        </ul>
	        </li>
	        <li><a href="#">Effect</a>
		        <ul>
			        <li><a href="#">Slide Effect</a></li>
			        <li><a href="#">Fade Effect</a></li>
			        <li><a href="#">Opacity Mode</a></li>
			        <li><a href="#">Drop Shadow</a></li>
			        <li><a href="#">Semitransparent</a></li>
		        </ul>
	        </li>
	        <li><a href="#">Navigation</a></li>
	        <li><a href="#">HTML/CSS</a></li>
	        <li><a href="#">Help</a></li>
        </ul>
        
    </form>
</body> 
</html>
