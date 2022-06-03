<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="menuHsubmenuV.aspx.vb" Inherits="IntranetVB.menuHsubmenuV" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="pt-br" lang="pt-br"> 
<head> 
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"> 
<title>Menu horizontal e vertical</title> 

<script type="text/javascript">
    
    function vertical() {

        var navItems = document.getElementById("nav").getElementsByTagName("li");

        for (var i = 0; i < navItems.length; i++) {
            if (navItems[i].className == "submenu") {
                navItems[i].onmouseover = function () { this.getElementsByTagName('ul')[0].style.display = "block"; this.style.backgroundColor = "#f9f9f9"; }
                navItems[i].onmouseout = function () { this.getElementsByTagName('ul')[0].style.display = "none"; this.style.backgroundColor = "#FFFFFF"; }
            }
        }

    }

    function horizontal() {

        var navItems = document.getElementById("barra").getElementsByTagName("li");

        for (var i = 0; i < navItems.length; i++) {
            if ((navItems[i].className == "menuvertical") || (navItems[i].className == "submenu")) {
                if (navItems[i].getElementsByTagName('ul')[0] != null) {
                    navItems[i].onmouseover = function () { this.getElementsByTagName('ul')[0].style.display = "block"; this.style.backgroundColor = "#f9f9f9"; }
                    navItems[i].onmouseout = function () { this.getElementsByTagName('ul')[0].style.display = "none"; this.style.backgroundColor = "#FFFFFF"; }
                }
            }
        }

    } 

</script> 

<style type="text/css"> 

body { font: normal 62.5% verdana; } 

ul.menubar 
{ 
   margin: 0px; 
   padding: 0px; 
   background-color: #FFFFFF; /* IE6 Bug */ 
   font-size: 100%; 
   } 

ul.menubar .menuvertical 
{ 
   margin: 0px; 
	 padding: 0px; 
	 list-style: none; 
	 background-color: #FFFFFF; 
   border: 1px solid #ccc; 
   float:left; 
} 

ul.menubar ul.menu 
{ 
   display: none; 
   position: absolute; 
   margin: 0px; 
} 

ul.menubar a 
{ 
   padding: 5px; 
   display:block; 
   text-decoration: none; 
   color: #777; 
   padding: 5px; 
} 


ul.menu, 
ul.menu ul 
{ 
   margin: 0; 
   padding: 0; 
   border-bottom: 1px solid #ccc; 
   width: 150px; /* Width of Menu Items */ 
   background-color: #FFFFFF; /* IE6 Bug */ 
} 

ul.menu li 
{ 
   position: relative; 
   list-style: none; 
   border: 0px; 
} 

ul.menu li a 
{ 
   display: block; 
   text-decoration: none; 
   border: 1px solid #ccc; 
   border-bottom: 0px; 
   color: #777; 
   padding: 5px 10px 5px 5px; 
} 

/* Fix IE. Hide from IE Mac \*/ 
* html ul.menu li { float: left; height: 1%; } 
* html ul.menu li a { height: 1%; } 
/* End */ 

ul.menu ul 
{ 
   position: absolute; 
   display: none; 
   left: 149px; /* Set 1px less than menu width */ 
   top: 0px; 
} 

ul.menu li.submenu ul { display: none; } /* Hide sub-menus initially */ 

ul.menu li.submenu { background: transparent url(arrow.gif) right center no-repeat; } 

ul.menu li a:hover { color: #E2144A; } 

</style> 

</head> 

<body onload="vertical();horizontal();"> 
 
<br /> 
<br /> 
<br /> 
<br />
 
<ul id="barra" class="menubar"> 
    <li class="menuvertical"><a href="#">Apontamento</a> 
	    <ul id="nav" class="menu"> 

		    <li><a href="#">Apontamento</a></li> 
	     
		    <li class="submenu"><a href="#">About</a>
		        <ul>
			        <li><a href="#">History</a></li>
			        <li><a href="#">Team</a></li>
			        <li><a href="#">Offices</a></li>
		        </ul>
		    </li>
	   
		    <li class="submenu"><a href="#">Services</a> 
		        <ul> 
			        <li><a href="#">Web Design</a></li> 
			        <li><a href="#">Internet Marketing</a></li> 
			        <li class="submenu"><a href="#">Hosting</a> 
			            <ul> 
				            <li><a href="#">Dedicated</a></li>	   
				            <li class="submenu"><a href="#">Virtual</a> 
			                <ul> 
				                <li><a href="#">United Kingdom</a></li> 
				                <li><a href="#">France</a></li> 
				                <li><a href="#">USA</a></li> 
			 
				                <li><a href="#">Australia</a></li> 
				            </ul> 
			                </li> 
				            <li><a href="#">Shared</a></li> 
				            <li><a href="#">Managed</a></li> 
			            </ul> 
			        </li> 
			        <li><a href="#">Domain Names</a></li> 
			        <li><a href="#">Broadband</a></li>	   
		        </ul> 
		    </li> 
		<li class="submenu"><a href="#">Contact Us</a> 
		  <ul> 
			<li><a href="#">United Kingdom</a></li> 
			<li><a href="#">France</a></li> 
			<li><a href="#">USA</a></li> 
	   
			<li><a href="#">Australia</a></li> 
		  </ul> 
		</li> 
	  </ul> 
   </li> 
   <li class="menuvertical"><a href="#">Menu 2</a></li> 
   <li class="menuvertical"><a href="#">Menu 3</a> 
	  <ul id="nav" class="menu"> 
		<li><a href="#">Home</a></li> 
	   
		<li class="submenu"><a href="#">About</a> 
		  <ul> 
			<li><a href="#">History</a></li> 
			<li><a href="#">Team</a></li> 
			<li><a href="#">Offices</a></li> 
		  </ul> 
		</li> 
	   
		<li class="submenu"><a href="#">Services</a> 
		  <ul> 
			<li><a href="#">Web Design</a></li> 
			<li><a href="#">Internet Marketing</a></li> 
			<li class="submenu"><a href="#">Hosting</a> 
			  <ul> 
				<li><a href="#">Dedicated</a></li> 
	   
				<li class="submenu"><a href="#">Virtual</a> 
			   <ul> 
				  <li><a href="#">United Kingdom</a></li> 
				  <li><a href="#">France</a></li> 
				  <li><a href="#">USA</a></li> 
			 
				  <li><a href="#">Australia</a></li> 
				</ul> 
			  </li> 
				<li><a href="#">Shared</a></li> 
				<li><a href="#">Managed</a></li> 
			  </ul> 
			</li> 
			<li><a href="#">Domain Names</a></li> 
			<li><a href="#">Broadband</a></li> 
	   
		  </ul> 
		</li> 
		<li class="submenu"><a href="#">Contact Us</a> 
		  <ul> 
			<li><a href="#">United Kingdom</a></li> 
			<li><a href="#">France</a></li> 
			<li><a href="#">USA</a></li> 
	   
			<li><a href="#">Australia</a></li> 
		  </ul> 
		</li> 
	  </ul> 
   </li> 
</ul> 
	
</body> 
</html>
