//=========================================================================================================
// USO: onKeyUp="somenteNumeros(this);"
//=========================================================================================================
function somenteNumeros(campo) {
    var digits = "0123456789"
    var campo_temp
    for (var i = 0; i < campo.value.length; i++) {
        campo_temp = campo.value.substring(i, i + 1)
        if (digits.indexOf(campo_temp) == -1) {
            campo.value = campo.value.substring(0, i);
        }
    }
}

//=========================================================================================================
// USO: onKeyUp="mascaraRG(this);"
//=========================================================================================================
function mascaraRG(campo) {
    var ascii = event.keyCode;
}  

//=========================================================================================================
// USO: mascara(this,'###.###.###-##');
//=========================================================================================================
function mascara(src, mask) {

    var i = src.value.length;
    var saida = mask.substring(i,i+1);
    var ascii = event.keyCode;

    if (saida == "B") {         // Não aceita número como entrada no teclado

        if ((ascii < 48) && (ascii > 57)) {            
            event.keyCode -= 32;
        } else {
            event.keyCode = 0;            
        }
        
    } else if (saida == "A") {  // Aceita somente letras do alfabeto e maiúsculas como entrada no teclado

        if ( (ascii >= 97) && (ascii <= 122) ) {
            event.keyCode -= 32;
        } else {
            event.keyCode = 0;
        }

    } else if (saida == "0") {  // Aceita somente números como entrada no teclado

        if ((ascii >= 48) && (ascii <= 57)) {
            return;
        } else {
            event.keyCode = 0;
        }
        
    } else if (saida == "#") {  // Aceita qualquer entrada no teclado
        
        return;
    
    } else {
        
        src.value += saida;
        i += 1;
        saida = mask.substring(i,i+1);

        if (saida == "A") {

            if ((ascii >=97) && (ascii <= 122)) {
                event.keyCode -= 32;
            } else {
                event.keyCode = 0;
            }

        } else if (saida == "0") {
            
            if ((ascii >= 48) && (ascii <= 57)) {
                return;
            } else {
                event.keyCode = 0;
            } 
           
        } else {
            return;
        }
    }
} 

// Para usar a função acima tente:
//DATA:
//onkeypress = "mascara(this, '00/00/0000')"

//CEP:
//onkeypress = "mascara(this, '00000-000')" 

//TELEFONE:
//onkeypress = "mascara(this, '(00) 0000-0000')"


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Daqui para baixo é o código que já havia neste arquivo

var popUp;

function SetControlValue(controlID, newDate, isPostBack)
{
    popUp.close();
    document.forms[0].elements[controlID].value=newDate;
    __doPostBack(controlID,'');
}

function OpenPopupPage (pageUrl, controlID, isPostBack)
{
    popUp=window.open(pageUrl+'?controlID='+controlID+'&isPostBack='+ isPostBack,'popupcal', 'width=250,height=300,left=200,top=250'); 
}

function checkFileExtension(elem) {
        var filePath = elem.value;

        if(filePath.indexOf('.') == -1)
            return false;
        
        var validExtensions = new Array();
        var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();
    
        validExtensions[0] = 'txt';
        validExtensions[1] = 'xml';
//        validExtensions[2] = 'bmp';
//        validExtensions[3] = 'png';
//        validExtensions[4] = 'gif';  
//        validExtensions[5] = 'tif';  
//        validExtensions[6] = 'tiff';
//        validExtensions[7] = 'txt';
//        validExtensions[8] = 'doc';
//        validExtensions[9] = 'xls';
//        validExtensions[10] = 'pdf';
    
        for(var i = 0; i < validExtensions.length; i++) {
            if(ext == validExtensions[i])
                return true;
        }

        alert('The file extension ' + ext.toUpperCase() + ' is not allowed!');
        return false;
    }
		                         
