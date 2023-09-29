<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="apontamentoNovo.aspx.vb" Inherits="IntranetVB.ApontamentoNovo" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tituloPagina" runat="server">
    Apontamento novo
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript" src="../recursos/jQuery/jquery-1.7.min.js"></script>

    <script type="text/javascript">

        //alert(window.navigator.appVersion);

        function calculaTudo() {
            for (i = 1; i <= $("[id$=qtdDiasMes]").val(); i++) {
                calculaLinhaDepoisTotais(i);
            }
            calculaTotais();
        }

        function calculaLinha(linha) {

            var qtdDiasMes = $("[id$=qtdDiasMes]").val();

            $("[id$=lblExtra" + linha + "]").val("");
            $("[id$=lblNormal" + linha + "]").val("");
            $("[id$=lblTotal" + linha + "]").val("");

            var entrada = $("[id$=txtEntrada" + linha + "]").val();
            var entAlmoco = $("[id$=txtEntAlmoco" + linha + "]").val();
            var saiAlmoco = $("[id$=txtSaiAlmoco" + linha + "]").val();
            var saida = $("[id$=txtSaida" + linha + "]").val();

            var sEntrada = "";
            var sEntAlmoco = "";
            var sSaiAlmoco = "";
            var sSaida = "";

            var camposVazios = 0;

            $("[id$=txtEntrada" + linha + "]").css("backgroundColor", "#FFFFFF");
            $("[id$=txtEntAlmoco" + linha + "]").css("backgroundColor", "#FFFFFF");
            $("[id$=txtSaiAlmoco" + linha + "]").css("backgroundColor", "#FFFFFF");
            $("[id$=txtSaida" + linha + "]").css("backgroundColor", "#FFFFFF");

            // Expressão regular para validar horario HH:MM 24h
            regHorario = /^([0-1]\d|2[0-3]):[0-5]\d$/;

            // Valida campo entrada

            if (entrada.length == 5) {
                if (regHorario.test(entrada)) {
                    sEntrada = "correto";
                } else {
                    sEntrada = "incorreto";
                }
            }
            if (entrada == "") {
                sEntrada = "vazio";
                camposVazios++;
            } else {
                if (entrada.length != 5 && entrada.length != 0) {
                    sEntrada = "incorreto";
                }
            }

            // Valida campo entAlmoco
            if (entAlmoco.length == 5) {
                if (regHorario.test(entAlmoco)) {
                    sEntAlmoco = "correto";
                } else {
                    sEntAlmoco = "incorreto";
                }
            }
            if (entAlmoco == "") {
                sEntAlmoco = "vazio";
                camposVazios++;
            } else {
                if (entAlmoco.length != 5 && entrada.length != 0) {
                    sEntAlmoco = "incorreto";
                }
            }

            // Valida campo saiAlmoco
            if (saiAlmoco.length == 5) {
                if (regHorario.test(saiAlmoco)) {
                    sSaiAlmoco = "correto";
                } else {
                    sSaiAlmoco = "incorreto";
                }
            }
            if (saiAlmoco == "") {
                sSaiAlmoco = "vazio";
                camposVazios++;
            } else {
                if (saiAlmoco.length != 5 && entrada.length != 0) {
                    sSaiAlmoco = "incorreto";
                }
            }

            // Valida campo saida
            if (saida.length == 5) {
                if (regHorario.test(saida)) {
                    sSaida = "correto";
                } else {
                    sSaida = "incorreto";
                }
            }
            if (saida == "") {
                sSaida = "vazio";
                camposVazios++;
            } else {
                if (saida.length != 5 && entrada.length != 0) {
                    sSaida = "incorreto";
                }
            }

            // Se 3 ou todos os campos estão vazios então não se calcula nada        
            if (camposVazios >= 3) {
                $("[id$=lblExtra" + linha + "]").val("");
                $("[id$=lblNormal" + linha + "]").val("");
                $("[id$=lblTotal" + linha + "]").val("");
                calculaTotais();
                return;
            }

            // Se algum campo contém erro
            if (sEntrada == "incorreto" || sEntAlmoco == "incorreto" || sSaiAlmoco == "incorreto" || sSaida == "incorreto") {
                $("[id$=lblExtra" + linha + "]").val("");
                $("[id$=lblNormal" + linha + "]").val("");
                $("[id$=lblTotal" + linha + "]").val("");
                $("[id$=txtEntrada" + linha + "]").css("backgroundColor", "#FFED80");
                $("[id$=txtEntAlmoco" + linha + "]").css("backgroundColor", "#FFED80");
                $("[id$=txtSaiAlmoco" + linha + "]").css("backgroundColor", "#FFED80");
                $("[id$=txtSaida" + linha + "]").css("backgroundColor", "#FFED80");
                calculaTotais();
                return;
            }

            // Se "entrada" e "saida" estão corretos, e "entrada almoço" ou "saida almoço" estão vazios
            if (sEntrada == "correto" && sSaida == "correto") {
                if (sEntAlmoco == "vazio" && sSaiAlmoco == "vazio") {
                    var resultado = subtraiHoras(entrada, saida, true);

                    $("[id$=lblTotal" + linha + "]").val(resultado);
                    var horario = resultado.split(":");

                    var resulHora = horario[0];
                    var resulMin = horario[1];

                    if (eval(resulHora) > 8) {

                        $("[id$=lblNormal" + linha + "]").val("08:00");
                        resulHora = eval(resulHora) - 8;
                        if (eval(resulHora) < 10) {
                            resulHora = "0" + resulHora;
                        }
                        $("[id$=lblExtra" + linha + "]").val(resulHora + ":" + resulMin);

                    } else {
                        if (eval(resulHora) == 8 && eval(resulMin) > 0) {
                            $("[id$=lblNormal" + linha + "]").val("08:00");
                            resulHora = eval(resulHora) - 8;
                            if (eval(resulHora) < 10) {
                                resulHora = "0" + resulHora;
                            }
                            $("[id$=lblExtra" + linha + "]").val(resulHora + ":" + resulMin);
                        } else {
                            $("[id$=lblNormal" + linha + "]").val(resultado);
                            $("[id$=lblExtra" + linha + "]").val("00:00");
                        }
                    }
                    calculaTotais();
                    return;
                } else {
                    if (sEntAlmoco == "vazio") {
                        $("[id$=txtEntAlmoco" + linha + "]").css("backgroundColor", "#FFED80");
                        $("[id$=lblExtra" + linha + "]").val("");
                        $("[id$=lblNormal" + linha + "]").val("");
                        $("[id$=lblTotal" + linha + "]").val("");
                        calculaTotais();
                        return;
                    }
                    if (sSaiAlmoco == "vazio") {
                        $("[id$=txtSaiAlmoco" + linha + "]").css("backgroundColor", "#FFED80");
                        $("[id$=lblExtra" + linha + "]").val("");
                        $("[id$=lblNormal" + linha + "]").val("");
                        $("[id$=lblTotal" + linha + "]").val("");
                        calculaTotais();
                        return;
                    }
                }
            } else {
                calculaTotais();
                return;
            }

            // Se chegou aqui siginifica que todos os campos estão preenchidos corretamente

            var diferenca1 = subtraiHoras(entrada, saida, linha);
            var diferenca2 = subtraiHoras(entAlmoco, saiAlmoco, linha);
            var resultado = subtraiHoras(diferenca1, diferenca2, false);

            // Abaixo: pega o resultado e exibe nos campos "Normal", "Extras" e "Total"           

            $("[id$=lblTotal" + linha + "]").val(resultado);

            var horario = resultado.split(":");

            var resulHora = horario[0];
            var resulMin = horario[1];

            if (eval(resulHora) > 8) {

                $("[id$=lblNormal" + linha + "]").val("08:00");
                resulHora = eval(resulHora) - 8;
                if (eval(resulHora) < 10) {
                    resulHora = "0" + resulHora;
                }
                $("[id$=lblExtra" + linha + "]").val(resulHora + ":" + resulMin);

            } else {
                if (eval(resulHora) == 8 && eval(resulMin) > 0) {
                    $("[id$=lblNormal" + linha + "]").val("08:00");
                    resulHora = eval(resulHora) - 8;
                    if (eval(resulHora) < 10) {
                        resulHora = "0" + resulHora;
                    }
                    $("[id$=lblExtra" + linha + "]").val(resulHora + ":" + resulMin);
                } else {
                    $("[id$=lblNormal" + linha + "]").val(resultado);
                    $("[id$=lblExtra" + linha + "]").val("00:00");
                }

            }

            calculaTotais();

        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        function calculaLinhaDepoisTotais(linha) {

            var qtdDiasMes = $("[id$=qtdDiasMes]").val();

            $("[id$=lblExtra" + linha + "]").val("");
            $("[id$=lblNormal" + linha + "]").val("");
            $("[id$=lblTotal" + linha + "]").val("");

            var entrada = $("[id$=txtEntrada" + linha + "]").val();
            var entAlmoco = $("[id$=txtEntAlmoco" + linha + "]").val();
            var saiAlmoco = $("[id$=txtSaiAlmoco" + linha + "]").val();
            var saida = $("[id$=txtSaida" + linha + "]").val();

            var sEntrada = "";
            var sEntAlmoco = "";
            var sSaiAlmoco = "";
            var sSaida = "";

            var camposVazios = 0;

            $("[id$=txtEntrada" + linha + "]").css("backgroundColor", "#FFFFFF");
            $("[id$=txtEntAlmoco" + linha + "]").css("backgroundColor", "#FFFFFF");
            $("[id$=txtSaiAlmoco" + linha + "]").css("backgroundColor", "#FFFFFF");
            $("[id$=txtSaida" + linha + "]").css("backgroundColor", "#FFFFFF");

            // Expressão regular para validar horario HH:MM 24h
            regHorario = /^([0-1]\d|2[0-3]):[0-5]\d$/;

            // Valida campo entrada

            if (entrada.length == 5) {
                if (regHorario.test(entrada)) {
                    sEntrada = "correto";
                } else {
                    sEntrada = "incorreto";
                }
            }
            if (entrada == "") {
                sEntrada = "vazio";
                camposVazios++;
            } else {
                if (entrada.length != 5 && entrada.length != 0) {
                    sEntrada = "incorreto";
                }
            }

            // Valida campo entAlmoco
            if (entAlmoco.length == 5) {
                if (regHorario.test(entAlmoco)) {
                    sEntAlmoco = "correto";
                } else {
                    sEntAlmoco = "incorreto";
                }
            }
            if (entAlmoco == "") {
                sEntAlmoco = "vazio";
                camposVazios++;
            } else {
                if (entAlmoco.length != 5 && entrada.length != 0) {
                    sEntAlmoco = "incorreto";
                }
            }

            // Valida campo saiAlmoco
            if (saiAlmoco.length == 5) {
                if (regHorario.test(saiAlmoco)) {
                    sSaiAlmoco = "correto";
                } else {
                    sSaiAlmoco = "incorreto";
                }
            }
            if (saiAlmoco == "") {
                sSaiAlmoco = "vazio";
                camposVazios++;
            } else {
                if (saiAlmoco.length != 5 && entrada.length != 0) {
                    sSaiAlmoco = "incorreto";
                }
            }

            // Valida campo saida
            if (saida.length == 5) {
                if (regHorario.test(saida)) {
                    sSaida = "correto";
                } else {
                    sSaida = "incorreto";
                }
            }
            if (saida == "") {
                sSaida = "vazio";
                camposVazios++;
            } else {
                if (saida.length != 5 && entrada.length != 0) {
                    sSaida = "incorreto";
                }
            }

            // Se 3 ou todos os campos estão vazios então não se calcula nada        
            if (camposVazios >= 3) {
                $("[id$=lblExtra" + linha + "]").val("");
                $("[id$=lblNormal" + linha + "]").val("");
                $("[id$=lblTotal" + linha + "]").val("");
                return;
            }

            // Se algum campo contém erro
            if (sEntrada == "incorreto" || sEntAlmoco == "incorreto" || sSaiAlmoco == "incorreto" || sSaida == "incorreto") {
                $("[id$=lblExtra" + linha + "]").val("");
                $("[id$=lblNormal" + linha + "]").val("");
                $("[id$=lblTotal" + linha + "]").val("");
                $("[id$=txtEntrada" + linha + "]").css("backgroundColor", "#FFED80");
                $("[id$=txtEntAlmoco" + linha + "]").css("backgroundColor", "#FFED80");
                $("[id$=txtSaiAlmoco" + linha + "]").css("backgroundColor", "#FFED80");
                $("[id$=txtSaida" + linha + "]").css("backgroundColor", "#FFED80");
                return;
            }

            // Se "entrada" e "saida" estão corretos, e "entrada almoço" ou "saida almoço" estão vazios
            if (sEntrada == "correto" && sSaida == "correto") {
                if (sEntAlmoco == "vazio" && sSaiAlmoco == "vazio") {
                    var resultado = subtraiHoras(entrada, saida, true);

                    $("[id$=lblTotal" + linha + "]").val(resultado);
                    var horario = resultado.split(":");

                    var resulHora = horario[0];
                    var resulMin = horario[1];

                    if (eval(resulHora) > 8) {

                        $("[id$=lblNormal" + linha + "]").val("08:00");
                        resulHora = eval(resulHora) - 8;
                        if (eval(resulHora) < 10) {
                            resulHora = "0" + resulHora;
                        }
                        $("[id$=lblExtra" + linha + "]").val(resulHora + ":" + resulMin);

                    } else {
                        if (eval(resulHora) == 8 && eval(resulMin) > 0) {
                            $("[id$=lblNormal" + linha + "]").val("08:00");
                            resulHora = eval(resulHora) - 8;
                            if (eval(resulHora) < 10) {
                                resulHora = "0" + resulHora;
                            }
                            $("[id$=lblExtra" + linha + "]").val(resulHora + ":" + resulMin);
                        } else {
                            $("[id$=lblNormal" + linha + "]").val(resultado);
                            $("[id$=lblExtra" + linha + "]").val("00:00");
                        }
                    }
                    return;
                } else {
                    if (sEntAlmoco == "vazio") {
                        $("[id$=txtEntAlmoco" + linha + "]").css("backgroundColor", "#FFED80");
                        $("[id$=lblExtra" + linha + "]").val("");
                        $("[id$=lblNormal" + linha + "]").val("");
                        $("[id$=lblTotal" + linha + "]").val("");
                        return;
                    }
                    if (sSaiAlmoco == "vazio") {
                        $("[id$=txtSaiAlmoco" + linha + "]").css("backgroundColor", "#FFED80");
                        $("[id$=lblExtra" + linha + "]").val("");
                        $("[id$=lblNormal" + linha + "]").val("");
                        $("[id$=lblTotal" + linha + "]").val("");
                        return;
                    }
                }
            } else {
                return;
            }

            // Se chegou aqui siginifica que todos os campos estão preenchidos corretamente

            var diferenca1 = subtraiHoras(entrada, saida, linha);
            var diferenca2 = subtraiHoras(entAlmoco, saiAlmoco, linha);
            var resultado = subtraiHoras(diferenca1, diferenca2, false);

            // Abaixo: pega o resultado e exibe nos campos "Normal", "Extras" e "Total"           

            $("[id$=lblTotal" + linha + "]").val(resultado);

            var horario = resultado.split(":");

            var resulHora = horario[0];
            var resulMin = horario[1];

            if (eval(resulHora) > 8) {

                $("[id$=lblNormal" + linha + "]").val("08:00");
                resulHora = eval(resulHora) - 8;
                if (eval(resulHora) < 10) {
                    resulHora = "0" + resulHora;
                }
                $("[id$=lblExtra" + linha + "]").val(resulHora + ":" + resulMin);

            } else {
                if (eval(resulHora) == 8 && eval(resulMin) > 0) {
                    $("[id$=lblNormal" + linha + "]").val("08:00");
                    resulHora = eval(resulHora) - 8;
                    if (eval(resulHora) < 10) {
                        resulHora = "0" + resulHora;
                    }
                    $("[id$=lblExtra" + linha + "]").val(resulHora + ":" + resulMin);
                } else {
                    $("[id$=lblNormal" + linha + "]").val(resultado);
                    $("[id$=lblExtra" + linha + "]").val("00:00");
                }

            }

        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////        
        function somaHoras(hrA, hrB, zerarHora) {

            zerarHora = false;

            var temp = 0;
            var nova_h = 0;
            var novo_m = 0;

            var horario = hrA.split(":");

            var hora1 = horario[0];
            var minu1 = horario[1];

            if (hora1.length < 2) {
                return;
            }
            if (minu1.length < 2) {
                return;
            }

            var minutos1 = (eval(hora1) * 60) + eval(minu1);

            horario = hrB.split(":");

            var hora2 = horario[0];
            var minu2 = horario[1];

            if (hora2.length < 2) {
                return;
            }
            if (minu2.length < 2) {
                return;
            }

            var minutos2 = (eval(hora2) * 60) + eval(minu2);

            temp = eval(minutos1) + eval(minutos2);
            if (eval(temp) < 0) {
                temp = eval(temp) * (-1);
            }

            while (eval(temp) > 59) {
                nova_h++;
                temp = eval(temp) - 60;
            }

            if (eval(nova_h) < 10) {
                nova_h = "0" + nova_h;
            }
            if (eval(temp) < 10) {
                temp = "0" + temp;
            }

            var resultado = nova_h + ":" + temp;

            return resultado;
        }
        //    Exemplos de utilização
        //        novaHora = somaHoras("12:00", "02:27", false);
        //        novaHora -> "14:27"
        // 
        //        novaHora = somaHoras("23:00", "05:00", true);
        //        novaHora -> "04:00"
        // 
        //        novaHora = somaHoras("23:00", "05:00", false);
        //        novaHora -> "28:00"

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        function subtraiHoras(hrA, hrB, linha) {

            if (hrA.length != 5 || hrB.length != 5) return "00:00";

            zerarHora = false;

            var temp = 0;
            var nova_h = 0;
            var novo_m = 0;

            var horario = hrA.split(":");

            var hora1 = horario[0];
            var minu1 = horario[1];

            var minutos1 = (eval(hora1) * 60) + eval(minu1);

            horario = hrB.split(":");

            var hora2 = horario[0];
            var minu2 = horario[1];

            var minutos2 = (eval(hora2) * 60) + eval(minu2);

            temp = eval(minutos1) - eval(minutos2);
            if (eval(temp) < 0) {
                temp = eval(temp) * (-1);
            }

            while (eval(temp) > 59) {
                nova_h++;
                temp = eval(temp) - 60;
            }

            if (eval(nova_h) < 10) {
                nova_h = "0" + nova_h;
            }
            if (eval(temp) < 10) {
                temp = "0" + temp;
            }

            var resultado = nova_h + ":" + temp;

            return resultado;

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        function calculaTotais() {

            var i = 1;
            var somaTotal = "00:00";
            var somaExtras = "00:00";
            var somaNormal = "00:00";

            var qtdDiasMes = $("[id$=qtdDiasMes]").val();


            for (i = 1; i <= qtdDiasMes; i += 1) {
                var horaTotal = $("[id$=lblTotal" + i + "]").val();
                var horaExtra = $("[id$=lblExtra" + i + "]").val();
                var horaNormal = $("[id$=lblNormal" + i + "]").val();

                if (horaTotal == null) {
                    horaTotal = "00:00";
                } else if (horaTotal.length != 5) {
                    horaTotal = "00:00";
                }
                if (horaExtra == null) {
                    horaExtra = "00:00";
                } else if (horaExtra.length != 5) {
                    horaExtra = "00:00";
                }
                if (horaNormal == null) {
                    horaNormal = "00:00";
                } else if (horaNormal.length != 5) {
                    horaNormal = "00:00";
                }

                somaTotal = somaHoras(somaTotal, horaTotal, false);
                somaExtras = somaHoras(somaExtras, horaExtra, false);
                somaNormal = somaHoras(somaNormal, horaNormal, false);
            }

            $("[id$=lblTotalTotal]").val(somaTotal);
            $("[id$=lblTotalExtras]").val(somaExtras);
            $("[id$=lblTotalNormais]").val(somaNormal);


            // ajuste para horas do mes
            var valHoraTotalMes = $("[id$=valHoraTotalMes]").val();
            debugger;
            var somaLimpa = somaTotal.toString();
            let arr = somaLimpa.split(":");
            console.log(somaLimpa);
            console.log(arr[0]);
            somaLimpa = arr[0];
            if (somaLimpa > valHoraTotalMes) {

                console.log('entrou no if'); console.log(parseInt(somaLimpa));
                $("[id$=btnSalvar]").css("backgroundColor", "#d3d3d3");
                $("[id$=btnSalvar]").prop("disabled", true);
                //$("[id$=lblHorasExcesso]").html = "Suas horas excederam o total permitido";
                //document.getElementById("#ctl00_corpo_lblHorasExcesso").innerHTML = "Suas horas excederam o total permitido";
                $("[id$=lblHorasExcesso2]").val('Suas horas excederam o total permitido:' + valHoraTotalMes + ' hrs.');
         
                console.log('saiu no if');
               
            }
            else if (somaLimpa == valHoraTotalMes) {
                $("[id$=btnSalvar]").prop('disabled', false);
                $("[id$=btnSalvar]").css("backgroundColor", "#ffffff");
                $("[id$=lblHorasExcesso2]").val('...');
            }
            else if (somaLimpa < valHoraTotalMes)  {
                //alert(somaTotal + '-' + valHoraTotalMes)
                console.log('entrou no else');
                $("[id$=btnSalvar]").prop('disabled', false);
                $("[id$=btnSalvar]").css("backgroundColor", "#ffffff");
                $("[id$=lblHorasExcesso2]").val('...');
                // $("#btnSalvar").attr("disabled", true);
                console.log('saiu do else');
            }

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        function validaHoras(campo, e) {

            var key = window.event ? e.keyCode : e.which;
            var keychar = String.fromCharCode(key);
            //alert(e.keyCode);

            // Permitindo a tecla "BACKSPACE" - "Firefox"
            if (key == 8) {
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

            if (campo.value.length == 0) {
                // Testa se o que o usuário digitou é realmente número.
                if (!regDigitos.test(keychar)) {
                    return false;
                } else {
                    // Testa se o primeiro digito pertence a um horario válido.
                    if (!regHorario.test(keychar + "0:00")) {
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
            if (campo.value.length == 3) {
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
        $('input').live("keypress", function (e) {
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

        $("[id$=txtEntrada1]").live("blur", function () { calculaLinha(1); }); $("[id$=txtEntAlmoco1]").live("blur", function () { calculaLinha(1); }); $("[id$=txtSaiAlmoco1]").live("blur", function () { calculaLinha(1); }); $("[id$=txtSaida1]").live("blur", function () { calculaLinha(1); });
        $("[id$=txtEntrada2]").live("blur", function () { calculaLinha(2); }); $("[id$=txtEntAlmoco2]").live("blur", function () { calculaLinha(2); }); $("[id$=txtSaiAlmoco2]").live("blur", function () { calculaLinha(2); }); $("[id$=txtSaida2]").live("blur", function () { calculaLinha(2); });
        $("[id$=txtEntrada3]").live("blur", function () { calculaLinha(3); }); $("[id$=txtEntAlmoco3]").live("blur", function () { calculaLinha(3); }); $("[id$=txtSaiAlmoco3]").live("blur", function () { calculaLinha(3); }); $("[id$=txtSaida3]").live("blur", function () { calculaLinha(3); });
        $("[id$=txtEntrada4]").live("blur", function () { calculaLinha(4); }); $("[id$=txtEntAlmoco4]").live("blur", function () { calculaLinha(4); }); $("[id$=txtSaiAlmoco4]").live("blur", function () { calculaLinha(4); }); $("[id$=txtSaida4]").live("blur", function () { calculaLinha(4); });
        $("[id$=txtEntrada5]").live("blur", function () { calculaLinha(5); }); $("[id$=txtEntAlmoco5]").live("blur", function () { calculaLinha(5); }); $("[id$=txtSaiAlmoco5]").live("blur", function () { calculaLinha(5); }); $("[id$=txtSaida5]").live("blur", function () { calculaLinha(5); });
        $("[id$=txtEntrada6]").live("blur", function () { calculaLinha(6); }); $("[id$=txtEntAlmoco6]").live("blur", function () { calculaLinha(6); }); $("[id$=txtSaiAlmoco6]").live("blur", function () { calculaLinha(6); }); $("[id$=txtSaida6]").live("blur", function () { calculaLinha(6); });
        $("[id$=txtEntrada7]").live("blur", function () { calculaLinha(7); }); $("[id$=txtEntAlmoco7]").live("blur", function () { calculaLinha(7); }); $("[id$=txtSaiAlmoco7]").live("blur", function () { calculaLinha(7); }); $("[id$=txtSaida7]").live("blur", function () { calculaLinha(7); });
        $("[id$=txtEntrada8]").live("blur", function () { calculaLinha(8); }); $("[id$=txtEntAlmoco8]").live("blur", function () { calculaLinha(8); }); $("[id$=txtSaiAlmoco8]").live("blur", function () { calculaLinha(8); }); $("[id$=txtSaida8]").live("blur", function () { calculaLinha(8); });
        $("[id$=txtEntrada9]").live("blur", function () { calculaLinha(9); }); $("[id$=txtEntAlmoco9]").live("blur", function () { calculaLinha(9); }); $("[id$=txtSaiAlmoco9]").live("blur", function () { calculaLinha(9); }); $("[id$=txtSaida9]").live("blur", function () { calculaLinha(9); });
        $("[id$=txtEntrada10]").live("blur", function () { calculaLinha(10); }); $("[id$=txtEntAlmoco10]").live("blur", function () { calculaLinha(10); }); $("[id$=txtSaiAlmoco10]").live("blur", function () { calculaLinha(10); }); $("[id$=txtSaida10]").live("blur", function () { calculaLinha(10); });
        $("[id$=txtEntrada11]").live("blur", function () { calculaLinha(11); }); $("[id$=txtEntAlmoco11]").live("blur", function () { calculaLinha(11); }); $("[id$=txtSaiAlmoco11]").live("blur", function () { calculaLinha(11); }); $("[id$=txtSaida11]").live("blur", function () { calculaLinha(11); });
        $("[id$=txtEntrada12]").live("blur", function () { calculaLinha(12); }); $("[id$=txtEntAlmoco12]").live("blur", function () { calculaLinha(12); }); $("[id$=txtSaiAlmoco12]").live("blur", function () { calculaLinha(12); }); $("[id$=txtSaida12]").live("blur", function () { calculaLinha(12); });
        $("[id$=txtEntrada13]").live("blur", function () { calculaLinha(13); }); $("[id$=txtEntAlmoco13]").live("blur", function () { calculaLinha(13); }); $("[id$=txtSaiAlmoco13]").live("blur", function () { calculaLinha(13); }); $("[id$=txtSaida13]").live("blur", function () { calculaLinha(13); });
        $("[id$=txtEntrada14]").live("blur", function () { calculaLinha(14); }); $("[id$=txtEntAlmoco14]").live("blur", function () { calculaLinha(14); }); $("[id$=txtSaiAlmoco14]").live("blur", function () { calculaLinha(14); }); $("[id$=txtSaida14]").live("blur", function () { calculaLinha(14); });
        $("[id$=txtEntrada15]").live("blur", function () { calculaLinha(15); }); $("[id$=txtEntAlmoco15]").live("blur", function () { calculaLinha(15); }); $("[id$=txtSaiAlmoco15]").live("blur", function () { calculaLinha(15); }); $("[id$=txtSaida15]").live("blur", function () { calculaLinha(15); });
        $("[id$=txtEntrada16]").live("blur", function () { calculaLinha(16); }); $("[id$=txtEntAlmoco16]").live("blur", function () { calculaLinha(16); }); $("[id$=txtSaiAlmoco16]").live("blur", function () { calculaLinha(16); }); $("[id$=txtSaida16]").live("blur", function () { calculaLinha(16); });
        $("[id$=txtEntrada17]").live("blur", function () { calculaLinha(17); }); $("[id$=txtEntAlmoco17]").live("blur", function () { calculaLinha(17); }); $("[id$=txtSaiAlmoco17]").live("blur", function () { calculaLinha(17); }); $("[id$=txtSaida17]").live("blur", function () { calculaLinha(17); });
        $("[id$=txtEntrada18]").live("blur", function () { calculaLinha(18); }); $("[id$=txtEntAlmoco18]").live("blur", function () { calculaLinha(18); }); $("[id$=txtSaiAlmoco18]").live("blur", function () { calculaLinha(18); }); $("[id$=txtSaida18]").live("blur", function () { calculaLinha(18); });
        $("[id$=txtEntrada19]").live("blur", function () { calculaLinha(19); }); $("[id$=txtEntAlmoco19]").live("blur", function () { calculaLinha(19); }); $("[id$=txtSaiAlmoco19]").live("blur", function () { calculaLinha(19); }); $("[id$=txtSaida19]").live("blur", function () { calculaLinha(19); });
        $("[id$=txtEntrada20]").live("blur", function () { calculaLinha(20); }); $("[id$=txtEntAlmoco20]").live("blur", function () { calculaLinha(20); }); $("[id$=txtSaiAlmoco20]").live("blur", function () { calculaLinha(20); }); $("[id$=txtSaida20]").live("blur", function () { calculaLinha(20); });
        $("[id$=txtEntrada21]").live("blur", function () { calculaLinha(21); }); $("[id$=txtEntAlmoco21]").live("blur", function () { calculaLinha(21); }); $("[id$=txtSaiAlmoco21]").live("blur", function () { calculaLinha(21); }); $("[id$=txtSaida21]").live("blur", function () { calculaLinha(21); });
        $("[id$=txtEntrada22]").live("blur", function () { calculaLinha(22); }); $("[id$=txtEntAlmoco22]").live("blur", function () { calculaLinha(22); }); $("[id$=txtSaiAlmoco22]").live("blur", function () { calculaLinha(22); }); $("[id$=txtSaida22]").live("blur", function () { calculaLinha(22); });
        $("[id$=txtEntrada23]").live("blur", function () { calculaLinha(23); }); $("[id$=txtEntAlmoco23]").live("blur", function () { calculaLinha(23); }); $("[id$=txtSaiAlmoco23]").live("blur", function () { calculaLinha(23); }); $("[id$=txtSaida23]").live("blur", function () { calculaLinha(23); });
        $("[id$=txtEntrada24]").live("blur", function () { calculaLinha(24); }); $("[id$=txtEntAlmoco24]").live("blur", function () { calculaLinha(24); }); $("[id$=txtSaiAlmoco24]").live("blur", function () { calculaLinha(24); }); $("[id$=txtSaida24]").live("blur", function () { calculaLinha(24); });
        $("[id$=txtEntrada25]").live("blur", function () { calculaLinha(25); }); $("[id$=txtEntAlmoco25]").live("blur", function () { calculaLinha(25); }); $("[id$=txtSaiAlmoco25]").live("blur", function () { calculaLinha(25); }); $("[id$=txtSaida25]").live("blur", function () { calculaLinha(25); });
        $("[id$=txtEntrada26]").live("blur", function () { calculaLinha(26); }); $("[id$=txtEntAlmoco26]").live("blur", function () { calculaLinha(26); }); $("[id$=txtSaiAlmoco26]").live("blur", function () { calculaLinha(26); }); $("[id$=txtSaida26]").live("blur", function () { calculaLinha(26); });
        $("[id$=txtEntrada27]").live("blur", function () { calculaLinha(27); }); $("[id$=txtEntAlmoco27]").live("blur", function () { calculaLinha(27); }); $("[id$=txtSaiAlmoco27]").live("blur", function () { calculaLinha(27); }); $("[id$=txtSaida27]").live("blur", function () { calculaLinha(27); });
        $("[id$=txtEntrada28]").live("blur", function () { calculaLinha(28); }); $("[id$=txtEntAlmoco28]").live("blur", function () { calculaLinha(28); }); $("[id$=txtSaiAlmoco28]").live("blur", function () { calculaLinha(28); }); $("[id$=txtSaida28]").live("blur", function () { calculaLinha(28); });
        $("[id$=txtEntrada29]").live("blur", function () { calculaLinha(29); }); $("[id$=txtEntAlmoco29]").live("blur", function () { calculaLinha(29); }); $("[id$=txtSaiAlmoco29]").live("blur", function () { calculaLinha(29); }); $("[id$=txtSaida29]").live("blur", function () { calculaLinha(29); });
        $("[id$=txtEntrada30]").live("blur", function () { calculaLinha(30); }); $("[id$=txtEntAlmoco30]").live("blur", function () { calculaLinha(30); }); $("[id$=txtSaiAlmoco30]").live("blur", function () { calculaLinha(30); }); $("[id$=txtSaida30]").live("blur", function () { calculaLinha(30); });
        $("[id$=txtEntrada31]").live("blur", function () { calculaLinha(31); }); $("[id$=txtEntAlmoco31]").live("blur", function () { calculaLinha(31); }); $("[id$=txtSaiAlmoco31]").live("blur", function () { calculaLinha(31); }); $("[id$=txtSaida31]").live("blur", function () { calculaLinha(31); });

        function novaJanela() {
            var w = 1000;
            var h = 700;
            var LeftPosition = (screen.width) ? (screen.width - w) / 2 : 0;
            var settings = "width=" + w + ",height=" + h + ",top=0,left=" + LeftPosition +
                ",toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes";

            //var URL = $("[id$=txtURL]").val();
            var URL = "<%=URL %>";

            window.open(URL, "_blank", settings);
        };

    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="corpo" runat="server">

    <div style="font-style: italic; color: #808080; text-align: center; padding-bottom: 10px;">
        <h2>Apontamento de Horas
            <br />
        </h2>
    </div>

    <form id="form1" runat="server" name="aspnetForm">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
        </asp:ScriptManager>

        <!-- Este Script deve-se ficar depois do ToolkitScriptManager -->
        <script type="text/javascript">
            // Sempre executa quando há um PostBack 
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                $(function () {
                    // Coloque seu código aqui
                });
            });

        </script>

        <div>
            <input type="hidden" id="txtURL" runat="server" />
        </div>

        <div style="padding-left: 170px; padding-right: 170px;">
            <div class="h2" style="text-align: left;">
                <strong>Projeto e período</strong>
                <hr />
                <br />
            </div>

            <asp:UpdatePanel ID="UpdatePanel" runat="server">
                <ContentTemplate>

                    <input type="hidden" id="varDataInicio" runat="server" />
                    <input type="hidden" id="varProCodigo" runat="server" />
                    <input type="hidden" id="varafpDiaEmDiante" runat="server" />
                    <input type="hidden" id="qtdDiasMes" runat="server" />
                    <input type="hidden" id="valHoraTotalMes" runat="server" />

                    <input type="hidden" id="txtColCodigo" runat="server" />
                    <input type="hidden" id="txtProCodigo" runat="server" />
                    <input type="hidden" id="txtDataInicio" runat="server" />
                    <input type="hidden" id="txtDataFim" runat="server" />

                    <table>
                        <tr>
                            <td valign="middle">
                                <span style="font-size: 12px;">Projeto:</span>
                                <asp:DropDownList ID="ddlProjetos" runat="server" Width="350">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td valign="middle">&nbsp;&nbsp;<span style="font-size: 12px;">Mês</span>
                                <asp:DropDownList ID="ddlMes" runat="server" Width="110">
                                </asp:DropDownList>
                            </td>
                            <td valign="middle">&nbsp;&nbsp;<span style="font-size: 12px;">Ano:</span>
                                <asp:DropDownList ID="ddlAno" runat="server" Width="90">
                                </asp:DropDownList>
                            </td>
                            <td valign="middle">&nbsp;&nbsp;&nbsp;<asp:Button ID="btnApontarHora" runat="server" Text="Apontar Hora" />
                            </td>
                        </tr>
                    </table>

                    <br />
                    <br />

                    <div class="h2" style="text-align: left;">
                        <strong>Apontamento</strong>
                        <hr />
                    </div>

                    <div id="divApontamento" style="text-align: left" runat="server">
                        <asp:Label ID="lblTitulo1" runat="server" Text=""></asp:Label>
                        <br />
                        <asp:Label ID="lblAviso" runat="server" Text=""></asp:Label>
                    </div>
                    <br />

                    <asp:UpdateProgress ID="UpdateProgress" runat="server" AssociatedUpdatePanelID="UpdatePanel"
                        DisplayAfter="250" DynamicLayout="true">
                        <ProgressTemplate>
                            <center>
                                <table>
                                    <tr>
                                        <td valign="middle">
                                            <img src="../imagens/progress.gif"
                                                alt="carregando" style="height: 60%; width: 60%;" />
                                        </td>
                                        <td valign="middle" style="font-family: @Microsoft YaHei, Verdana; font-size: small; color: Gray;">Carrregando...
                                        </td>
                                    </tr>
                                </table>
                            </center>
                        </ProgressTemplate>
                    </asp:UpdateProgress>

                </ContentTemplate>

            </asp:UpdatePanel>

            <asp:UpdatePanel ID="UpdatePanelTabelaApontamento" runat="server">
                <ContentTemplate>

                    <!-- Tabela de Apontamento -->

                    <div id="divTabelaApontamento" runat="server" style="font-family: Verdana; font-size: 12px;">

                        <div style='font-family: @Arial Unicode MS; font-style: italic; font-size: medium; display: none; color: Red;'
                            align='left'>
                            <asp:Label ID="lblAlerta" runat="server"></asp:Label>
                            <br />
                            <br />
                        </div>

                        <!-- Tabela de Aprovadores -->

                        <table id="tblAprovadores" runat="server" style="display: none;">
                            <tr>
                                <th style="width: 100px;">Aprovadores
                                </th>
                                <th style="width: 300px;"></th>
                            </tr>
                            <tr>
                                <td>Gerente de projeto: &nbsp;
                                </td>
                                <td>
                                    <asp:Label ID="lblGP" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>Gerente de contas: &nbsp;
                                </td>
                                <td>
                                    <asp:Label ID="lblGC" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>Diretor: &nbsp;
                                </td>
                                <td>
                                    <asp:Label ID="lblDir" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>

                        <div style="font-family: @Arial Unicode MS; font-style: italic; font-size: smaller; color: Gray;" align='left'>
                            Observações:<br />
                            * Os campos "Entrada", "Almoço" e "Saída" devem ser preenchidos no formato hh:mm;<br />
                            * No mínimo deve-se preencher os campos de "Entrada" e "Saída", caso não, a linha correspondente não será salva.                           
                        </div>

                        <table>
                            <tr>
                                <td style="width: 7px;"></td>
                                <td colspan="9" style="background-color: #0B258C; height: 25px;">
                                    <font color="white" size="2"><b>
                                        <asp:Label ID="lblTituloCalendario" runat="server"></asp:Label>
                                    </font>
                                </td>
                            </tr>
                            <tr style="height: 25px;" valign="middle">
                                <td></td>
                                <td style="background-color: #0B258C; width: 70px;">
                                    <font color="white" size="1"><b>Data
                                    </b></font>
                                </td>
                                <td style="background-color: #0B258C; width: 50px;">
                                    <font color="white" size="1"><b>Entrada
                                    </b></font>
                                </td>
                                <td colspan="2" style="background-color: #0B258C; width: 90px;">
                                    <font color="white" size="1"><b>Almoço
                                    </b></font>
                                </td>
                                <td style="background-color: #0B258C; width: 50px;">
                                    <font color="white" size="1"><b>Saída
                                    </b></font>
                                </td>
                                <td style="background-color: #0B258C; width: 50px;">
                                    <font color="white" size="1"><b>Normais
                                    </b></font>
                                </td>
                                <td style="background-color: #0B258C; width: 50px;">
                                    <font color="white" size="1"><b>Extras
                                    </b></font>
                                </td>
                                <td style="background-color: #0B258C; width: 50px;">
                                    <font color="white" size="1"><b>Total
                                    </b></font>
                                </td>
                                <td style="background-color: #0B258C; width: 400px;">
                                    <font color="white" size="1"><b>Atividades
                                    </b></font>
                                </td>
                            </tr>
                            <tr id="linha1" runat="server">
                                <td></td>
                                <td id="dia1" runat="server" title="Teste">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia1" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" name="txtEntrada1" id="txtEntrada1" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" name="txtEntAlmoco1" id="txtEntAlmoco1" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" name="txtSaiAlmoco1" id="txtSaiAlmoco1" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" name="txtSaida1" id="txtSaida1" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal1" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra1" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal1" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades1" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha2" runat="server">
                                <td></td>
                                <td id="dia2" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia2" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada2" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco2" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco2" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida2" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal2" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra2" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal2" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades2" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha3" runat="server">
                                <td></td>
                                <td id="dia3" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia3" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada3" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco3" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco3" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida3" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal3" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra3" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal3" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades3" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha4" runat="server">
                                <td></td>
                                <td id="dia4" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia4" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada4" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco4" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco4" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida4" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal4" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra4" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal4" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades4" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha5" runat="server">
                                <td></td>
                                <td id="dia5" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia5" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada5" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco5" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco5" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida5" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal5" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra5" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal5" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades5" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha6" runat="server">
                                <td></td>
                                <td id="dia6" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia6" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada6" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco6" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco6" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida6" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal6" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra6" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal6" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades6" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha7" runat="server">
                                <td></td>
                                <td id="dia7" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia7" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada7" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco7" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco7" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida7" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal7" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra7" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal7" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades7" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha8" runat="server">
                                <td></td>
                                <td id="dia8" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia8" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada8" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco8" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco8" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida8" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal8" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra8" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal8" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades8" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha9" runat="server">
                                <td></td>
                                <td id="dia9" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia9" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada9" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco9" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco9" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida9" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal9" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra9" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal9" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades9" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha10" runat="server">
                                <td></td>
                                <td id="dia10" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia10" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada10" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco10" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco10" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida10" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal10" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra10" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal10" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades10" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha11" runat="server">
                                <td></td>
                                <td id="dia11" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia11" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada11" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco11" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco11" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida11" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal11" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra11" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal11" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades11" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha12" runat="server">
                                <td></td>
                                <td id="dia12" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia12" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada12" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco12" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco12" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida12" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal12" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra12" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal12" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades12" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha13" runat="server">
                                <td></td>
                                <td id="dia13" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia13" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada13" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco13" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco13" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida13" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal13" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra13" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal13" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades13" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha14" runat="server">
                                <td></td>
                                <td id="dia14" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia14" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada14" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco14" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco14" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida14" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal14" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra14" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal14" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades14" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha15" runat="server">
                                <td></td>
                                <td id="dia15" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia15" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada15" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco15" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco15" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida15" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal15" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra15" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal15" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades15" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha16" runat="server">
                                <td></td>
                                <td id="dia16" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia16" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada16" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco16" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco16" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida16" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal16" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra16" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal16" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades16" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha17" runat="server">
                                <td></td>
                                <td id="dia17" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia17" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada17" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco17" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco17" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida17" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal17" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra17" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal17" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades17" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha18" runat="server">
                                <td></td>
                                <td id="dia18" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia18" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada18" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco18" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco18" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida18" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal18" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra18" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal18" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades18" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha19" runat="server">
                                <td></td>
                                <td id="dia19" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia19" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada19" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco19" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco19" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida19" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal19" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra19" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal19" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades19" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha20" runat="server">
                                <td></td>
                                <td id="dia20" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia20" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada20" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco20" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco20" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida20" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal20" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra20" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal20" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades20" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha21" runat="server">
                                <td></td>
                                <td id="dia21" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia21" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada21" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco21" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco21" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida21" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal21" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra21" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal21" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades21" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha22" runat="server">
                                <td></td>
                                <td id="dia22" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia22" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada22" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco22" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco22" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida22" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal22" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra22" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal22" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades22" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha23" runat="server">
                                <td></td>
                                <td id="dia23" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia23" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada23" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco23" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco23" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida23" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;"
                                        readonly="readonly" id="lblNormal23" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;"
                                        readonly="readonly" id="lblExtra23" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;"
                                        readonly="readonly" id="lblTotal23" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades23" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha24" runat="server">
                                <td></td>
                                <td id="dia24" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia24" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada24" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco24" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco24" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida24" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal24" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra24" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal24" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades24" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha25" runat="server">
                                <td></td>
                                <td id="dia25" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia25" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada25" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco25" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco25" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida25" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal25" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra25" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal25" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades25" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha26" runat="server">
                                <td></td>
                                <td id="dia26" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia26" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada26" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco26" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco26" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida26" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal26" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra26" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal26" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades26" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha27" runat="server">
                                <td></td>
                                <td id="dia27" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia27" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada27" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco27" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco27" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida27" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal27" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra27" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal27" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades27" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha28" runat="server">
                                <td></td>
                                <td id="dia28" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia28" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada28" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco28" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco28" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida28" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal28" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra28" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal28" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades28" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha29" runat="server">
                                <td></td>
                                <td id="dia29" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia29" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada29" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco29" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco29" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida29" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal29" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra29" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal29" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades29" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha30" runat="server">
                                <td></td>
                                <td id="dia30" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia30" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada30" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco30" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco30" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida30" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal30" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra30" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal30" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades30" runat="server" Width="440" MaxLength="199"
                                        CssClass="txtApo">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr id="linha31" runat="server">
                                <td></td>
                                <td id="dia31" runat="server">
                                    <font color="white" size="1">
                                        <asp:Label ID="lblDia31" runat="server" Text=""></asp:Label>
                                    </font>
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntrada31" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtEntAlmoco31" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaiAlmoco31" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" id="txtSaida31" runat="server" maxlength="5" style="width: 33px;"
                                        onkeypress="return validaHoras(this, event);" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblNormal31" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblExtra31" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <input type="text" style="background-color: #EDEFF7; border-style: none; width: 33px;" readonly="readonly" id="lblTotal31" runat="server" />
                                </td>
                                <td style="background-color: #EDEFF7;">
                                    <asp:TextBox ID="txtAtividades31" runat="server" Width="440" MaxLength="199"
                                        CssClass='txtApo'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr align="right">
                                <td></td>
                                <td colspan="5" style="background-color: #E4E4E4; height: 17px;">
                                    <font size="2">
                                        <b>Total:</b> &nbsp
                                    </font>
                                </td>
                                <td style="background-color: #E4E4E4;" align="center">
                                    <input type="text" style="background-color: #E4E4E4; border-style: none; width: 50px; font-weight: bold; font-family: Verdana; font-size: 12px;"
                                        readonly="readonly" id="lblTotalNormais" runat="server" value="00:00" />
                                </td>
                                <td style="background-color: #E4E4E4;" align="center">
                                    <input type="text" style="background-color: #E4E4E4; border-style: none; width: 50px; font-weight: bold; font-family: Verdana; font-size: 12px;"
                                        readonly="readonly" id="lblTotalExtras" runat="server" value="00:00" />
                                </td>
                                <td style="background-color: #E4E4E4;" align="center">
                                    <input type="text" style="background-color: #E4E4E4; border-style: none; width: 50px; font-weight: bold; font-family: Verdana; font-size: 12px;"
                                        readonly="readonly" id="lblTotalTotal" runat="server" value="00:00" />
                                </td>
                                <td style="background-color: #E4E4E4;" align="left">
                                 <input type="text" style="background-color: #E4E4E4; border-style: none; width: 366px; font-weight: bold; font-family: Verdana; font-size: 12px;"
                                        readonly="readonly" id="lblHorasExcesso2" runat="server" value="..." />
                                </td>
                            </tr>

                        </table>
                        <table>
                            <tr style="height: 40px;">
                                <td style="width: 500px;" align="right">
                                    <asp:Label ID="lblMensagem" runat="server" Text="..."></asp:Label>
                                    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanelTabelaApontamento"
                                        DisplayAfter="0" DynamicLayout="true">
                                        <ProgressTemplate>
                                            <table>
                                                <tr>
                                                    <td valign="middle">
                                                        <img src="../imagens/progress.gif"
                                                            alt="carregando" style="height: 60%; width: 60%;" />
                                                    </td>
                                                    <td valign="middle" style="font-family: @Microsoft YaHei, Verdana; font-size: small; color: Gray;">
                                                        <label id="lblCarregando" runat="server"></label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnSalvar" runat="server" Text="Salvar" Width="100" Height="30"
                                        OnClientClick="calculaTudo();" />
                                    &nbsp;
                                    <asp:Button ID="btnLimpar" runat="server" Text="Limpar" Width="100" Height="30" />
                                    &nbsp;
                                    <input type='button' id='btnVisualizar' runat='server' value='Visualizar'
                                        onclick="novaJanela();" style="width: 100px; height: 30px;" />
                                </td>
                            </tr>
                        </table>

                    </div>

                    <asp:Label ID="lblTeste" runat="server" Text="" CssClass="MsgTestes"></asp:Label>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnApontarHora" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </form>

</asp:Content>

