<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="calculaHoras.aspx.vb" Inherits="IntranetVB.calculaHoras" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Calcula Horas com JavaScript</title>
         
    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script>
    <script type="text/javascript" src="../recursos/jQuery/jquery.animate-colors-min.js"></script>
    <script type="text/javascript" src="../recursos/jQuery/meiomask.js"></script>
    <script type="text/javascript"></script>
    <script type="text/javascript">

        function calculaHoras(linha) {

            var entrada = $("[id$=txtEntrada" + linha + "]").val();
            var entAlmoco = $("[id$=txtEntAlmoco" + linha + "]").val();
            var saiAlmoco = $("[id$=txtSaiAlmoco" + linha + "]").val();
            var saida = $("[id$=txtSaida" + linha + "]").val();

            $("[id$=txtEntrada" + linha + "]").animate({ backgroundColor: "#FFFFFF" }, "slow");
            $("[id$=txtEntAlmoco" + linha + "]").animate({ backgroundColor: "#FFFFFF" }, "slow");
            $("[id$=txtSaiAlmoco" + linha + "]").animate({ backgroundColor: "#FFFFFF" }, "slow");
            $("[id$=txtSaida" + linha + "]").animate({ backgroundColor: "#FFFFFF" }, "slow");

            if (entrada.length != 5) {
                if (entrada.length != 0) {
                    // Destaca o campo em amarelo
                    $("[id$=txtEntrada" + linha + "]").animate({ backgroundColor: "#FFED80" }, "slow");
                    return;
                }
                // Se chegou aqui, é que o campo txtEntrada esta vazio
                //return;
            }
            if (entAlmoco.length != 5) {
                if (entAlmoco.length != 0) {
                    // Destaca o campo em amarelo
                    $("[id$=txtEntAlmoco" + linha + "]").animate({ backgroundColor: "#FFED80" }, "slow");
                    return;
                }
                // Se chegou aqui, é que o campo txtEntrada esta vazio
                //return;
            }
            if (saiAlmoco.length != 5) {
                if (saiAlmoco.length != 0) {
                    // Destaca o campo em amarelo
                    $("[id$=txtSaiAlmoco" + linha + "]").animate({ backgroundColor: "#FFED80" }, "slow");
                    return;
                }
                // Se chegou aqui, é que o campo txtEntrada esta vazio
                //return;
            }
            if (saida.length != 5) {
                if (saida.length != 0) {
                    // Destaca o campo em amarelo
                    $("[id$=txtSaida" + linha + "]").animate({ backgroundColor: "#FFED80" }, "slow");
                    return;
                }
                // Se chegou aqui, é que o campo txtEntrada esta vazio
                //return;
            }

            if (entrada.length == 0 || entAlmoco.length == 0 || saiAlmoco.length == 0 || saida.length == 0) {
                return;               
            }

            // Se chegou aqui siginifica que todos os campos estão preenchidos corretamento para calculo das horas

            var diferenca1 = subtraiHoras(entrada, entAlmoco, linha);
            var diferenca2 = subtraiHoras(saiAlmoco, saida, linha);
            var resultado = somaHoras(diferenca1, diferenca2, true);

            $("[id$=lblTotal" + linha + "]").val(resultado);

            if (eval(nova_h) > 8) {
                $("[id$=lblNormal" + linha + "]").val("08:00");
                nova_h = eval(nova_h) - 8;
                if (eval(nova_h) < 10) {
                    nova_h = "0" + nova_h;
                }
                $("[id$=lblExtra" + linha + "]").val(nova_h + ":" + novo_m);
            } else {
                $("[id$=lblNormal" + linha + "]").val(resultado);
                $("[id$=lblExtra" + linha + "]").val("00:00");
            }

            calculaTotais();

//            if (entrada.length != 5) {
//                if (entrada.length != 0) {
//                    // Destaca o campo em amarelo
//                    $("[id$=txtEntrada" + linha + "]").animate({ backgroundColor: "#FFED80" }, "slow");
//                }
//                // Se chegou aqui, é que o campo txtEntrada esta vazio
//                return;
//            }
//            if (entAlmoco.length != 5) {
//                if (entAlmoco.length != 0) {
//                    // Destaca o campo em amarelo
//                    $("[id$=txtEntAlmoco" + linha + "]").animate({ backgroundColor: "#FFED80" }, "slow");
//                }
//                // Se chegou aqui, é que o campo txtEntAlmoco esta vazio                
//                if (saida.length != 5) {
//                    if (saida.length != 0) {
//                        // Destaca o campo em amarelo
//                        $("[id$=txtSaida" + linha + "]").animate({ backgroundColor: "#FFED80" }, "slow");
//                    }
//                    // Se chegou aqui, é que o campo txtSaida esta vazio
//                    return;
//                }
//                
//                subtraiHoras(entrada, saida, linha);
//                return;
//            }     
//            if (saiAlmoco != 5) {
//                if (saiAlmoco != 0) {
//                    // Destaca o campo em amarelo
//                    //$("[id$=txtSaiAlmoco" + linha + "]").animate({ backgroundColor: "#FFED80" }, "slow");
//                }
//                // Se chegou aqui, é que o campo txtSaida esta vazio
//                // Destaca o campo em amarelo
//                $("[id$=txtSaiAlmoco" + linha + "]").animate({ backgroundColor: "#FFED80" }, "slow");
//                return;
//            }       
        
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////       
        function somaHoras(hrA, hrB, zerarHora) {

            if (hrA.length != 5 || hrB.length != 5) return "00:00";
                    
            temp = 0;
            nova_h = 0;
            novo_m = 0;

            var horario = hrA.split(":");
            
            var hora1 = horario[0];
            var minu1 = horario[1];
             
            horario = hrB.split(":");

            var hora2 = horario[0];
            var minu2 = horario[1];            
            
//          Função de soma de Horas
           
            temp = eval(minu1) + eval(minu2);
            while(eval(temp) > 59) {
                    nova_h++;
                    temp = eval(temp) - 60;
                }
            
            novo_m = temp.toString().length == 2 ? temp : ("0" + temp);
            temp = eval(hora1) + eval(hora2) + eval(nova_h);
            while(temp > 23 && zerarHora) {
                    temp = temp - 24;
            }
            nova_h = temp.toString().length == 2 ? temp : ("0" + temp);
                      
            return nova_h + ":" + novo_m;
        }
        // Como Usar
//        novaHora = somaHoras("12:00", "02:27", false);
//        novaHora -> "14:27"
// 
//        novaHora = somaHoras("22:50", "05:10", true);
//        novaHora -> "04:00"
// 
//        novaHora = somaHoras("22:50", "05:10", false);
//        novaHora -> "28:00"
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        function subtraiHoras(hrA, hrB, linha) {

            if (hrA.length != 5 || hrB.length != 5) return "00:00";
            
            zerarHora = true;

            temp = 0;
            nova_h = 0;
            novo_m = 0;

            var horario = hrA.split(":");

            var hora1 = horario[0];
            var minu1 = horario[1];

            horario = hrB.split(":");

            var hora2 = horario[0];
            var minu2 = horario[1];

            temp = eval(minu1) + eval(minu2);
            while (eval(temp) > 59) {
                nova_h++;
                temp = eval(temp) - 60;
            }

            novo_m = temp.toString().length == 2 ? temp : ("0" + temp);
            temp = eval(hora1) - eval(hora2) - eval(nova_h);
            if ( eval(temp) < 0 ) {
                temp = eval(temp)*(-1);
            }
            while (temp > 23 && zerarHora) {
                temp = temp - 24;
            }
            nova_h = temp.toString().length == 2 ? temp : ("0" + temp);

            var resultado = nova_h + ":" + novo_m;

            return resultado;           
            
        }              
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        function calculaTotais() {
            
            var i = 1;
            var somaTotal = "00:00";
            var somaExtras = "00:00";
            var somaNormal = "00:00";

            for (i = 1; i <= 2; i += 1) {
                var horaTotal = $("[id$=lblTotal" + i + "]").val();
                var horaExtra = $("[id$=lblExtra" + i + "]").val();
                var horaNormal = $("[id$=lblNormal" + i + "]").val();

                if (horaTotal.length != 5) {
                    horaTotal = "00:00";
                }
                if (horaExtra.length != 5) {
                    horaExtra = "00:00";
                }
                if (horaNormal.length != 5) {
                    horaNormal = "00:00";
                }

                somaTotal = somaHoras(somaTotal, horaTotal, false);
                somaExtras = somaHoras(somaExtras, horaExtra, false);
                somaNormal = somaHoras(somaNormal, horaNormal, false);
            }

            $("[id$=lblTotalTotal]").val(somaTotal);
            $("[id$=lblTotalExtras]").val(somaExtras);
            $("[id$=lblTotalNormais]").val(somaNormal);
            
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        function validaHoras(campo, e) {
            
            var key = window.event ? e.keyCode : e.which;
            var keychar = String.fromCharCode(key);
            //alert(e.keyCode);
            
            // Permitindo a tecla "BACKSPACE" - "Firefox"
            if (key == 8){
                return true;
            }
            // Permitindo a tecla "TAB" - "Firefox"
            if (key == 9) {
                return true;
            }
                        
            // Expressão regular para validar somente números
            regDigitos = /^\d+$/;
            // Expressão regular para validar horario HH:MM 24h
            regHorario = /^([0-1]\d|2[0-3]):[0-5]\d$/;
            
            if (campo.value.length == 0 ) {
                // Testa se o que o usuário digitou é realmente número.
                if (!regDigitos.test(keychar)) {
                    return false;
                } else {
                // Testa se o primeiro digito pertence a um horario válido.
                if ( !regHorario.test( keychar + "0:00") ) {
                        return false;
                    }
                }
            }            
            // Se chegou até aqui, significa que temos um digito de 0 a 2
            if (campo.value.length == 1) {
                // Testa se o caracter digitado é número e diferente de ":"
                if (!regDigitos.test(keychar) && keychar != ":") {
                    return false;
                } else {
                    if (keychar == ":") {
                        campo.value = "0" + campo.value + ":";
                    } else {
                        // Testa se os dois primeiros digitos pertence a um horario válido.
                        if (!regHorario.test(campo.value + keychar + ":00")) {
                            return false;
                        } else {
                            campo.value = campo.value + keychar + ":";
                            return false;
                        }
                    }
                }
            }            
            // Se chegou até aqui, significa que temos dois digito de 00 a 23
            if (campo.value.length == 2) {
                // Testa se o caracter digitado é número e diferente de ":"
                if (!regDigitos.test(keychar) && keychar != ":") {
                    return false;
                } else {
                    if (keychar != ":") {
                        campo.value = campo.value + ":"; // Deixando no formato HH:
                        // Testa se os três primeiros digitos pertence a um horario válido.
                        if (!regHorario.test(campo.value + keychar + "0")) {
                            return false;
                        }
                    }
                }
                return;
            }
            // Se chegou até aqui, significa que temos o seguinte formato "HH:"
            if (campo.value.length == 3 ) {
                // Testa se o caracter digitado é número
                if (!regDigitos.test(keychar)) {
                    return false;
                }
                // Testa se os três primeiros digitos pertence a um horario válido.
                var teste = campo.value + keychar + "0";
                if (!regHorario.test(campo.value + keychar + "0")) {
                    return false;
                }
            }
            // Se chegou até aqui, significa que temos o seguinte formato "HH:M"
            if (campo.value.length == 4) {
                // Testa se o caracter digitado é número
                if (!regDigitos.test(keychar)) {
                    return false;
                }
                // Testa se os três primeiros digitos pertence a um horario válido.
                if (!regHorario.test(campo.value + keychar)) {
                    return false;
                }
            }
            // Se chegou até aqui, significa que temos um digito de 0 a 2
            if (campo.value.length == 5) {
                return false;                         
            }

        }        
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        // Substitui o ENTER por TAB
        $('input').live("keypress", function(e) {
            /* ENTER PRESSED*/
            if (e.keyCode == 13 || e.keyCode == 9) {
                /* FOCUS ELEMENT */
                var inputs = $(this).parents("form").eq(0).find(":input");
                var idx = inputs.index(this);

                if (idx == inputs.length - 1) {
                    inputs[0].select()
                } else {
                    inputs[idx + 1].focus(); //  handles submit buttons 
                    inputs[idx + 1].select();
                }
                return false;
            }
        });
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    </script>
    
</head>

<body>
<form action="" method="post" id="formulario">

<table width="150">
    <tr>
        <td>Entrada</td>
        <td>Ent. Almoço</td>
        <td>Sai. Almoço</td>
        <td>Saída</td>
        <td>Hr. Normal</td>
        <td>Hr. Extra</td>
        <td>Hr. Total</td>
        <td>Atividades</td>
    </tr>
    <tr>
        <td><input type="text" id="txtEntrada1" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(1);" /></td>
        <td><input type="text" id="txtEntAlmoco1" value="" maxlength="5" style="width:50px;"  
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(1);" /></td>
        <td><input type="text" id="txtSaiAlmoco1" value="" maxlength="5" style="width:50px;" 
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(1);" /></td>
        <td><input type="text" id="txtSaida1" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(1);" /></td>    
        <td><input type="text" id="lblNormal1" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(1);" /></td>
        <td><input type="text" id="lblExtra1" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(1);" /></td>
        <td><input type="text" id="lblTotal1" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(1);" /></td>
        <td><input type="text" id="txtAtividades1" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(1);" /></td>
    </tr>
    <tr>
        <td><input type="text" id="txtEntrada2" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(2);" /></td>
        <td><input type="text" id="txtEntAlmoco2" value="" maxlength="5" style="width:50px;"  
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(2);" /></td>
        <td><input type="text" id="txtSaiAlmoco2" value="" maxlength="5" style="width:50px;" 
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(2);" /></td>
        <td><input type="text" id="txtSaida2" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(2);" /></td>  
        <td><input type="text" id="lblNormal2" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(2);" /></td>
        <td><input type="text" id="lblExtra2" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(2);" /></td>
        <td><input type="text" id="lblTotal2" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(2);" /></td>
        <td><input type="text" id="txtAtividades2" value="" maxlength="5" style="width:50px;"
            onkeypress="return validaHoras(this, event);" onblur="calculaHoras(2);" /></td>
    </tr>
    <tr>
        <td></td>
        <td></td>
        <td></td>
        <td>Totais:</td>
        <td><input type="text" id="lblTotalNormais" value="" maxlength="5" style="width:50px;" /></td>
        <td><input type="text" id="lblTotalExtras" value="" maxlength="5" style="width:50px;" /></td>
        <td><input type="text" id="lblTotalTotal" value="" maxlength="5" style="width:50px;" /></td>
        <td></td>
    </tr>
</table>

</form>
</body>
</html>