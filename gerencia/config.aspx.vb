Imports System.Drawing
Imports System.Globalization

Partial Public Class config
    Inherits System.Web.UI.Page
    Dim SQL As String
    Dim primeiroAnoSistema As Integer = 2011

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Try
            If Not Session("permissao").ToString.Contains("Configuracoes") Then
                If Session("colNomeLogado") <> "" Then
                    Response.Redirect("..\boasVindas.aspx")
                Else
                    Response.Redirect("..\Default.aspx")
                End If
            End If

        Catch ex As Exception
            Response.Redirect("..\Default.aspx")
        End Try

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

        lblPeriodo.Style.Add("font-style", "italic")
        lblPeriodo.Style.Add("color", "Blue")

        lblTaxas.Style.Add("font-style", "italic")
        lblTaxas.Style.Add("color", "Blue")

        lblDiasHorasMes.Style.Add("font-style", "italic")
        lblDiasHorasMes.Style.Add("color", "Blue")

        lblPeriodoAprovacao.Style.Add("font-style", "italic")
        lblPeriodoAprovacao.Style.Add("color", "Blue")


        If Not IsPostBack Then

            preencheAFPeriodo()
            preencheTaxa()
            preencheDiasHorasMes(Date.Now.Year)
            ddlAno.Items.Add(New ListItem("Selecione", ""))

            ' Inicialização do Panel "Periodo de aprovação"
            '''''''''''''''''''''''''''''''''''''''''''''''
            ' Preenche o camboBox Ano com ano inicio 2011 até o ano corrente
            While primeiroAnoSistema <= Date.Now.Year
                ddlAno_PA.Items.Add(primeiroAnoSistema)
                ddlAno.Items.Add(primeiroAnoSistema)
                primeiroAnoSistema = primeiroAnoSistema + 1

            End While
            ' Deixa selecionado como padrão o ano corrente
            ddlAno_PA.SelectedValue = Date.Now.Year

            ddlAno.SelectedValue = ""

            primeiroAnoSistema = primeiroAnoSistema + 1

            Dim anoCorrente = Date.Now.Year

            preparaPanelPeriodoAprovacao(anoCorrente)
            ddlAno_PA.AutoPostBack = True

            'Preenche os calendarios salvos
            PreencherCalendSalvos(ddlCalendSalvo)

        End If

    End Sub
    Private Sub CriarCalendario()
        Try

            Calendario.Visible = True
            Dim Dia As DateTime = DateTime.Parse("01/01/" + ddlAno.SelectedValue)
            Dim DiaContador As Int16 = 1, MesContador As Int16 = 1
            Dim Linhas As New List(Of TableRow), Celulas As New List(Of TableCell)
            Dim Feriados As List(Of Feriados) = RetornarFeriados()

            'Contador dos meses
            For MesContador = 1 To 12
                'Pega a tabela de acordo com o mês
                Dim Table As Table = updtMeses.FindControl(String.Concat("Tb", MesContador))

                If Not Table Is Nothing Then
                    'Contador dos dias do mês
                    For DiaContador = 1 To Date.DaysInMonth(ddlAno.SelectedValue, MesContador)
                        Dim Row As TableRow
                        Dim LabelSemana As New Label, LabelDia As New Label
                        Dim TextBox As New TextBox

                        'Cria a variavel com o dia para manipulação de semana
                        Dim DiaMes As New DateTime(ddlAno.SelectedValue, MesContador, DiaContador)

                        Row = Tb1.FindControl(String.Concat(DiaMes.ToString("MMMM", New CultureInfo("pt-BR")).ToLower, DiaContador.ToString))

                        LabelSemana = Tb1.FindControl(String.Concat("lblSemana_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", DiaContador.ToString))
                        LabelDia = Tb1.FindControl(String.Concat("lblDia_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", DiaContador.ToString))
                        TextBox = Tb1.FindControl(String.Concat("txt_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", DiaContador.ToString))

                        LabelSemana.Font.Name = "Verdana"
                        LabelSemana.Font.Size = 8

                        LabelDia.Font.Name = "Verdana"
                        LabelDia.Font.Size = 8

                        TextBox.Font.Name = "Verdana"
                        TextBox.Font.Size = 8
                        TextBox.Attributes.Add("onkeypress", "return validaHoras(this, event);")
                        TextBox.Width = "35"

                        LabelSemana.Text = DiaMes.ToString("ddd", New CultureInfo("pt-BR")).ToUpper()
                        LabelDia.Text = DiaContador
                        TextBox.Text = ConfigurationManager.AppSettings("HorasDia")
                        'verifica na lista de feriados se a data é dia util ou não
                        Dim VerFer As Feriados = (From dados In Feriados Where (dados.Dia.Equals(DiaContador) AndAlso (dados.Mes.Equals(MesContador))) Select dados).FirstOrDefault()

                        'Verifica se é feriado ou final de semana
                        If Not IsNothing(VerFer) Then
                            Row.BackColor = Color.Yellow
                            Row.ToolTip = VerFer.Descricao
                            TextBox.Text = String.Empty
                        Else
                            If LabelSemana.Text.Contains("SÁB") Or LabelSemana.Text.Contains("DOM") Then
                                Row.BackColor = ColorTranslator.FromHtml("#F05000")
                                TextBox.Text = String.Empty
                            Else
                                Row.BackColor = ColorTranslator.FromHtml("#8A7DFF")
                            End If
                        End If

                    Next

                    'verifica se o mes tem 31 dia *somente para deixar as tabelas alinhadas
                    If Date.DaysInMonth(ddlAno.SelectedValue, MesContador) < 31 Then
                        Dim LinhasFaltantes = 31 - Date.DaysInMonth(ddlAno.SelectedValue, MesContador)
                        Dim Contador As Integer = 1, ContadorCampo As Integer = ((LinhasFaltantes - 31) * -1) + 1
                        Dim LabelSemana As New Label, LabelDia As New Label
                        Dim TextBox As New TextBox
                        'Adicona as as linhas faltantes
                        While Contador <= LinhasFaltantes
                            Dim DiaMes As New DateTime(ddlAno.SelectedValue, MesContador, 1)

                            LabelSemana = Tb1.FindControl(String.Concat("lblSemana_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", ContadorCampo.ToString))
                            LabelDia = Tb1.FindControl(String.Concat("lblDia_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", ContadorCampo.ToString))
                            TextBox = Tb1.FindControl(String.Concat("txt_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", ContadorCampo.ToString))

                            LabelSemana.Font.Name = "Verdana"
                            LabelSemana.Font.Size = 8

                            LabelDia.Font.Name = "Verdana"
                            LabelDia.Font.Size = 8

                            TextBox.Font.Name = "Verdana"
                            TextBox.Font.Size = 8
                            TextBox.Width = "35"

                            LabelSemana.Visible = False
                            LabelDia.Visible = False
                            TextBox.BorderStyle = BorderStyle.None

                            Contador += 1
                            ContadorCampo += 1
                        End While
                    End If
                End If
            Next
            updtMeses.Update()
        Catch ex As Exception
            Calendario.Visible = False
            lblMensagem.Text = String.Format("Ocorreu um erro ao montar o calendário com descrição: {0}", ex.Message)
        End Try

    End Sub

    Private Sub CriarCalendarioSalvo()
        Try

            Calendario.Visible = True
            Dim DsCalendario As String = ddlCalendSalvo.SelectedValue.Split("|")(0)
            Dim Ano As Integer = ddlCalendSalvo.SelectedValue.Split("|")(1)
            Dim Dia As DateTime = DateTime.Parse("01/01/" & Ano)
            Dim DiaContador As Int16 = 1, MesContador As Int16 = 1
            Dim Linhas As New List(Of TableRow), Celulas As New List(Of TableCell)
            Dim ListaDados As New List(Of Calendario)
            Dim Feriados As New List(Of Feriados)


            'Cria a lista com os feriados
            SQL = "select dia,mes,Descricao from tblFeriados with(nolock) order by mes,dia"

            If selectSQL(SQL) Then
                While dr.Read
                    Dim Fer As New Feriados()
                    Fer.Dia = Integer.Parse(dr("dia"))
                    Fer.Mes = Integer.Parse(dr("mes"))
                    Fer.Descricao = dr("Descricao")
                    Feriados.Add(Fer)
                End While
            End If

            'Cria a lista com os dados do calendario
            SQL = "select DsCalendario,Dia,Mes,Ano,Hora from tblcalendario with(nolock,index=IX_tblCalendario_1)  where upper(dscalendario) ='" & DsCalendario & "' and ano =" & Ano & " order by id"

            If selectSQL(SQL) Then
                While dr.Read
                    Dim Calen As New Calendario()
                    Calen.Dia = Integer.Parse(dr("dia"))
                    Calen.Mes = Integer.Parse(dr("mes"))
                    Calen.Ano = Integer.Parse(dr("dia"))
                    Calen.Hora = dr("Hora")
                    ListaDados.Add(Calen)
                End While
            End If

            'Contador dos meses
            For MesContador = 1 To 12
                'Pega a tabela de acordo com o mês
                Dim Table As Table = updtMeses.FindControl(String.Concat("Tb", MesContador))

                If Not Table Is Nothing Then
                    'Contador dos dias do mês
                    For DiaContador = 1 To Date.DaysInMonth(Ano, MesContador)
                        Dim Row As TableRow
                        Dim LabelSemana As New Label, LabelDia As New Label
                        Dim TextBox As New TextBox

                        'Cria a variavel com o dia para manipulação de semana
                        Dim DiaMes As New DateTime(Ano, MesContador, DiaContador)

                        Row = Tb1.FindControl(String.Concat(DiaMes.ToString("MMMM", New CultureInfo("pt-BR")).ToLower, DiaContador.ToString))

                        LabelSemana = Tb1.FindControl(String.Concat("lblSemana_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", DiaContador.ToString))
                        LabelDia = Tb1.FindControl(String.Concat("lblDia_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", DiaContador.ToString))
                        TextBox = Tb1.FindControl(String.Concat("txt_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", DiaContador.ToString))

                        LabelSemana.Font.Name = "Verdana"
                        LabelSemana.Font.Size = 8

                        LabelDia.Font.Name = "Verdana"
                        LabelDia.Font.Size = 8

                        TextBox.Font.Name = "Verdana"
                        TextBox.Font.Size = 8
                        TextBox.Attributes.Add("onkeypress", "return validaHoras(this, event);")
                        TextBox.Width = "35"

                        LabelSemana.Text = DiaMes.ToString("ddd", New CultureInfo("pt-BR")).ToUpper()
                        LabelDia.Text = DiaContador

                        'verifica se existe a linha com valor e atribui
                        Dim Valor = (From dados In ListaDados Where dados.Dia.Equals(DiaContador) AndAlso (dados.Mes.Equals(MesContador)) Select dados.Hora).FirstOrDefault()
                        If Not IsNothing(Valor) Then TextBox.Text = Valor.ToString

                        'verifica na lista de feriados se a data é dia util ou não
                        Dim VerFer As Feriados = (From dados In Feriados Where (dados.Dia.Equals(DiaContador) AndAlso (dados.Mes.Equals(MesContador))) Select dados).FirstOrDefault()

                        'Verifica se é feriado ou final de semana
                        If Not IsNothing(VerFer) Then
                            Row.BackColor = Color.Yellow
                            Row.ToolTip = VerFer.Descricao
                        Else
                            If LabelSemana.Text.Contains("SÁB") Or LabelSemana.Text.Contains("DOM") Then
                                Row.BackColor = ColorTranslator.FromHtml("#F05000")
                            Else
                                Row.BackColor = ColorTranslator.FromHtml("#8A7DFF")
                            End If
                        End If

                    Next

                    'verifica se o mes tem 31 dia *somente para deixar as tabelas alinhadas
                    If Date.DaysInMonth(Ano, MesContador) < 31 Then
                        Dim LinhasFaltantes = 31 - Date.DaysInMonth(Ano, MesContador)
                        Dim Contador As Integer = 1, ContadorCampo As Integer = ((LinhasFaltantes - 31) * -1) + 1
                        Dim LabelSemana As New Label, LabelDia As New Label
                        Dim TextBox As New TextBox
                        'Adicona as as linhas faltantes
                        While Contador <= LinhasFaltantes
                            Dim DiaMes As New DateTime(Ano, MesContador, 1)

                            LabelSemana = Tb1.FindControl(String.Concat("lblSemana_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", ContadorCampo.ToString))
                            LabelDia = Tb1.FindControl(String.Concat("lblDia_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", ContadorCampo.ToString))
                            TextBox = Tb1.FindControl(String.Concat("txt_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", ContadorCampo.ToString))

                            LabelSemana.Font.Name = "Verdana"
                            LabelSemana.Font.Size = 8

                            LabelDia.Font.Name = "Verdana"
                            LabelDia.Font.Size = 8

                            TextBox.Font.Name = "Verdana"
                            TextBox.Font.Size = 8
                            TextBox.Width = "35"

                            LabelSemana.Visible = False
                            LabelDia.Visible = False
                            TextBox.BorderStyle = BorderStyle.None

                            Contador += 1
                            ContadorCampo += 1
                        End While
                    End If
                End If
            Next
            updtMeses.Update()
        Catch ex As Exception
            Calendario.Visible = False
            lblMensagem.Text = String.Format("Ocorreu um erro ao montar o calendário com descrição: {0}", ex.Message)
        End Try

    End Sub

    '=============================================================================================
    '=======    Div Abertura e fechamentos    ====================================================
    '=============================================================================================

    Private Sub preencheAFPeriodo()

        If DateTime.IsLeapYear(DateTime.Now.Year) Then
            slideFevereiro.Maximum = 29
        End If

        SQL = "SELECT * FROM v_AFPeriodo ORDER BY afpMes"

        If selectSQL(SQL) Then
            While dr.Read
                Select Case dr("afpMes")
                    Case "1"
                        If dr("afpAberto") = "A" Then
                            chbJaneiro.Checked = True
                            txtJaneiro.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "2"
                        If dr("afpAberto") = "A" Then
                            chbFevereiro.Checked = True
                            txtFevereiro.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "3"
                        If dr("afpAberto") = "A" Then
                            chbMarco.Checked = True
                            txtMarco.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "4"
                        If dr("afpAberto") = "A" Then
                            chbAbril.Checked = True
                            txtAbril.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "5"
                        If dr("afpAberto") = "A" Then
                            chbMaio.Checked = True
                            txtMaio.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "6"
                        If dr("afpAberto") = "A" Then
                            chbJunho.Checked = True
                            txtJunho.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "7"
                        If dr("afpAberto") = "A" Then
                            chbJulho.Checked = True
                            txtJulho.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "8"
                        If dr("afpAberto") = "A" Then
                            chbAgosto.Checked = True
                            txtAgosto.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "9"
                        If dr("afpAberto") = "A" Then
                            chbSetembro.Checked = True
                            txtSetembro.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "10"
                        If dr("afpAberto") = "A" Then
                            chbOutubro.Checked = True
                            txtOutubro.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "11"
                        If dr("afpAberto") = "A" Then
                            chbNovembro.Checked = True
                            txtNovembro.Text = dr("afpDiaEmDiante").ToString
                        End If
                    Case "12"
                        If dr("afpAberto") = "A" Then
                            chbDezembro.Checked = True
                            txtDezembro.Text = dr("afpDiaEmDiante").ToString
                        End If
                End Select
            End While
        Else
            lblPeriodo.Style.Add("color", "Red")
            lblPeriodo.Text = sqlErro + " :("
        End If

    End Sub

    Private Sub btnSalvarAFPeriodo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSalvarAFPeriodo.Click

        salvarAFPeriodo()

    End Sub

    Private Sub salvarAFPeriodo()

        Dim arraySQLString As New ArrayList
        lblPeriodo.Style.Add("color", "Blue")
        lblPeriodo.Text = ""
        Dim afpAberto = "A"
        Dim i = 0

        SQL = "DELETE tblAFPeriodo"

        If comandoSQL(SQL) Then

            If chbJaneiro.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (1, '" & afpAberto & "'," & txtJaneiro.Text.Trim & ")")

            If chbFevereiro.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (2, '" & afpAberto & "'," & txtFevereiro.Text.Trim & ")")

            If chbMarco.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (3, '" & afpAberto & "'," & txtMarco.Text.Trim & ")")

            If chbAbril.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (4, '" & afpAberto & "'," & txtAbril.Text.Trim & ")")

            If chbMaio.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (5, '" & afpAberto & "'," & txtMaio.Text.Trim & ")")

            If chbJunho.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (6, '" & afpAberto & "'," & txtJunho.Text.Trim & ")")

            If chbJulho.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (7, '" & afpAberto & "'," & txtJulho.Text.Trim & ")")

            If chbAgosto.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (8, '" & afpAberto & "'," & txtAgosto.Text.Trim & ")")

            If chbSetembro.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (9, '" & afpAberto & "'," & txtSetembro.Text.Trim & ")")

            If chbOutubro.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (10, '" & afpAberto & "'," & txtOutubro.Text.Trim & ")")

            If chbNovembro.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (11, '" & afpAberto & "'," & txtNovembro.Text.Trim & ")")

            If chbDezembro.Checked Then
                afpAberto = "A"
            Else
                afpAberto = "F"
            End If

            arraySQLString.Add("INSERT INTO tblAFPeriodo (afpMes, afpAberto, afpDiaEmDiante)" &
                               " VALUES (12, '" & afpAberto & "'," & txtDezembro.Text.Trim & ")")

            ' Loop para fazer a atualização como o que foi selecionado
            For Each SQL As String In arraySQLString
                If Not comandoSQL(SQL) Then
                    lblPeriodo.Style.Add("color", "Red")
                    lblPeriodo.Text = sqlErro
                    Return
                End If
            Next
        Else
            lblPeriodo.Text = sqlErro
        End If



        lblPeriodo.Text = "Alteração com sucesso.  "

    End Sub

    '=============================================================================================
    '=======    Div Periodo de aprovação    =====================================================
    '=============================================================================================

    Private Sub preparaPanelPeriodoAprovacao(ByVal ano As Integer)

        Dim varData As DateTime
        Dim strData As String
        Dim dataLimite As DateTime
        Dim controle As Control = Me.accPeriodoAprovacao

        ' Reseta/limpa todos os campos
        For i = 1 To 12

            controle = controle.FindControl("ddlPA_de_" & i)
            CType(controle, DropDownList).Items.Clear()

            controle = controle.FindControl("ddlPA_ate_" & i)
            CType(controle, DropDownList).Items.Clear()

            controle = controle.FindControl("ckbPA_Mes" & i)
            CType(controle, HtmlInputCheckBox).Checked = False

        Next i

        ' Carrega os campos de acordo com o ano seelcionado
        For i = 1 To 12

            controle = controle.FindControl("ddlPA_de_" & i)

            ' Trecho que preenche os DropDownList "DE" ************************
            If i = 1 Then
                varData = "02/12/" & (ano - 1)
            Else
                varData = "02" & "/" & (i - 1) & "/" & ano
            End If

            dataLimite = varData.AddMonths(1).AddDays(-1)

            While varData <= dataLimite
                strData = varData.ToString.Replace(" 00:00:00", "")
                CType(controle, DropDownList).Items.Add(varData)
                varData = varData.AddDays(1)
            End While
            '*******************************************************************

            ' Trecho que preenche os DropDownList "ATÉ" ************************ 
            controle = controle.FindControl("ddlPA_ate_" & i)

            varData = "01" & "/" & i & "/" & ano
            dataLimite = varData.AddMonths(1).AddDays(-1)

            While varData <= dataLimite
                strData = varData.ToString.Replace(" 00:00:00", "")
                CType(controle, DropDownList).Items.Add(varData)
                varData = varData.AddDays(1)
            End While

        Next i

        carregaPanelPeriodoAprovacao(ano)

    End Sub

    Private Sub carregaPanelPeriodoAprovacao(ByVal ano As Integer)

        Dim ddl_de As Control = Me.accPeriodoAprovacao ' DropDownList data inicio
        Dim ddl_ate As Control = Me.accPeriodoAprovacao ' DropDownList data fim
        Dim ckb As Control = Me.accPeriodoAprovacao ' CheckBox

        Dim checado As Boolean
        Dim competencia As Integer

        SQL = "SELECT * FROM v_periodoAprovacao WHERE ano = " & ano & " ORDER BY competencia"

        If selectSQL(SQL) Then

            If dr.HasRows Then

                While dr.Read

                    Try
                        competencia = dr("competencia")

                        ddl_de = ddl_de.FindControl("ddlPA_de_" & competencia)
                        ddl_ate = ddl_ate.FindControl("ddlPA_ate_" & competencia)
                        ckb = ckb.FindControl("ckbPA_Mes" & competencia)

                        If dr("aberto") = 1 Then
                            checado = True
                        Else
                            checado = False
                        End If

                        CType(ckb, HtmlInputCheckBox).Checked = checado
                        CType(ddl_de, DropDownList).SelectedValue = dr("dataInicio")
                        CType(ddl_ate, DropDownList).SelectedValue = dr("dataFim")

                    Catch ex As Exception
                    End Try

                End While

            End If

        End If

    End Sub

    Public Sub salvarPeriodoAprovacao()

        Dim ddl_de As Control = Me.accPeriodoAprovacao ' DropDownList data inicio
        Dim ddl_ate As Control = Me.accPeriodoAprovacao ' DropDownList data fim
        Dim ckb As Control = Me.accPeriodoAprovacao ' CheckBox

        Dim dataInicio As DateTime
        Dim dataFim As DateTime
        Dim checado As Integer
        Dim ano As Integer = ddlAno_PA.SelectedValue

        For i = 1 To 12

            ddl_de = ddl_de.FindControl("ddlPA_de_" & i)
            ddl_ate = ddl_ate.FindControl("ddlPA_ate_" & i)
            ckb = ckb.FindControl("ckbPA_Mes" & i)

            dataInicio = CType(ddl_de, DropDownList).SelectedItem.Text
            dataFim = CType(ddl_ate, DropDownList).SelectedItem.Text

            If CType(ckb, HtmlInputCheckBox).Checked Then
                checado = 1
            Else
                checado = 0
            End If

            SQL = "IF EXISTS (SELECT * FROM v_periodoAprovacao WHERE ano = " & ano & " AND competencia = " & i & ")" &
              "  UPDATE tblPeriodoAprovacao SET aberto = " & checado & ", dataInicio = '" & dataInicio & "', dataFim = '" & dataFim & "'" &
              "  WHERE competencia = " & i & " AND ano = " & ano & "; " &
              "ELSE " &
              "  INSERT INTO tblPeriodoAprovacao (aberto, competencia, ano, dataInicio, dataFim)" &
              "  VALUES (" & checado & ", " & i & ", " & ano & ", '" & dataInicio & "', '" & dataFim & "');"

            If Not comandoSQL(SQL) Then
                lblPeriodoAprovacao.Text = sqlErro
            Else
                lblPeriodoAprovacao.Text = "Salvo com sucesso..."
            End If

        Next i

    End Sub

    Private Sub btnSalvarPeriodoAprovacao_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSalvarPeriodoAprovacao.Click
        salvarPeriodoAprovacao()
    End Sub
    Private Sub ddlAno_PA_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAno_PA.SelectedIndexChanged
        lblPeriodoAprovacao.Text = ""
        preparaPanelPeriodoAprovacao(ddlAno_PA.SelectedValue)
    End Sub

    '=============================================================================================
    '=======    Div Taxas    =====================================================================
    '=============================================================================================

    Private Sub preencheTaxa()

        SQL = "SELECT * FROM v_taxas"

        If selectSQL(SQL) Then
            While dr.Read
                Select Case dr("taxTipo")
                    Case "CLT"
                        txtTaxaCLT.Text = dr("taxPct")
                    Case "PJ"
                        txtTaxaPJ.Text = dr("taxPct")
                End Select
            End While
        Else
            lblTaxas.Style.Add("color", "Red")
            lblTaxas.Text = sqlErro
        End If

    End Sub

    Private Sub btnSalvarTaxa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSalvarTaxa.Click

        SQL = "UPDATE tblTaxas SET taxPct = '" & txtTaxaCLT.Text & "' WHERE taxTipo = 'CLT' "
        SQL += "UPDATE tblTaxas SET taxPct = '" & txtTaxaPJ.Text & "' WHERE taxTipo = 'PJ'"

        If comandoSQL(SQL) Then
            lblTaxas.Style.Add("color", "Blue")
            lblTaxas.Text = "Alteração com sucesso.  "
        Else
            lblTaxas.Style.Add("color", "Red")
            lblTaxas.Text = sqlErro
        End If

    End Sub

    '=============================================================================================
    '=======    Div Meses/dias/horas    ==========================================================
    '=============================================================================================

    Private Sub preencheDiasHorasMes(ByVal ano As Integer)

        SQL = "SELECT * FROM v_diasHorasMes WHERE ano = " & ano

        If selectSQL(SQL) Then
            While dr.Read
                Select Case dr("mes")
                    Case "Janeiro"
                        txtDias1.Text = dr("qtdDiasUteis")
                        txtHoras1.Text = dr("qtdHorasPorMes")
                    Case "Fevereiro"
                        txtDias2.Text = dr("qtdDiasUteis")
                        txtHoras2.Text = dr("qtdHorasPorMes")
                    Case "Março"
                        txtDias3.Text = dr("qtdDiasUteis")
                        txtHoras3.Text = dr("qtdHorasPorMes")
                    Case "Abril"
                        txtDias4.Text = dr("qtdDiasUteis")
                        txtHoras4.Text = dr("qtdHorasPorMes")
                    Case "Maio"
                        txtDias5.Text = dr("qtdDiasUteis")
                        txtHoras5.Text = dr("qtdHorasPorMes")
                    Case "Junho"
                        txtDias6.Text = dr("qtdDiasUteis")
                        txtHoras6.Text = dr("qtdHorasPorMes")
                    Case "Julho"
                        txtDias7.Text = dr("qtdDiasUteis")
                        txtHoras7.Text = dr("qtdHorasPorMes")
                    Case "Agosto"
                        txtDias8.Text = dr("qtdDiasUteis")
                        txtHoras8.Text = dr("qtdHorasPorMes")
                    Case "Setembro"
                        txtDias9.Text = dr("qtdDiasUteis")
                        txtHoras9.Text = dr("qtdHorasPorMes")
                    Case "Outubro"
                        txtDias10.Text = dr("qtdDiasUteis")
                        txtHoras10.Text = dr("qtdHorasPorMes")
                    Case "Novembro"
                        txtDias11.Text = dr("qtdDiasUteis")
                        txtHoras11.Text = dr("qtdHorasPorMes")
                    Case "Dezembro"
                        txtDias12.Text = dr("qtdDiasUteis")
                        txtHoras12.Text = dr("qtdHorasPorMes")
                End Select
            End While
        Else
            Dim erro = sqlErro
        End If

    End Sub

    Private Sub btnDiasHorasMeses_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDiasHorasMeses.Click

        Dim anoCorrente As Integer = Date.Now.Year

        SQL = "SELECT ano FROM v_diasHorasMes WHERE ano = " & anoCorrente

        Dim qtd As Integer = selectSQLQtdRegistros(SQL)

        If qtd = -1 Then
            lblDiasHorasMes.Style.Add("color", "Red")
            lblDiasHorasMes.Text = sqlErro
            Return
        End If

        SQL = "DELETE tblDiasHorasMes WHERE ano =" & anoCorrente
        comandoSQL(SQL)

        If txtDias1.Text.Trim = "" Then
            txtDias1.Text = "0"
        End If
        If txtDias2.Text.Trim = "" Then
            txtDias2.Text = "0"
        End If
        If txtDias3.Text.Trim = "" Then
            txtDias3.Text = "0"
        End If
        If txtDias4.Text.Trim = "" Then
            txtDias4.Text = "0"
        End If
        If txtDias5.Text.Trim = "" Then
            txtDias5.Text = "0"
        End If
        If txtDias6.Text.Trim = "" Then
            txtDias6.Text = "0"
        End If
        If txtDias7.Text.Trim = "" Then
            txtDias7.Text = "0"
        End If
        If txtDias8.Text.Trim = "" Then
            txtDias8.Text = "0"
        End If
        If txtDias9.Text.Trim = "" Then
            txtDias9.Text = "0"
        End If
        If txtDias10.Text.Trim = "" Then
            txtDias10.Text = "0"
        End If
        If txtDias11.Text.Trim = "" Then
            txtDias11.Text = "0"
        End If
        If txtDias12.Text.Trim = "" Then
            txtDias12.Text = "0"
        End If

        If txtHoras1.Text.Trim = "" Then
            txtHoras1.Text = "0"
        End If
        If txtHoras2.Text.Trim = "" Then
            txtHoras2.Text = "0"
        End If
        If txtHoras3.Text.Trim = "" Then
            txtHoras3.Text = "0"
        End If
        If txtHoras4.Text.Trim = "" Then
            txtHoras4.Text = "0"
        End If
        If txtHoras5.Text.Trim = "" Then
            txtHoras5.Text = "0"
        End If
        If txtHoras6.Text.Trim = "" Then
            txtHoras6.Text = "0"
        End If
        If txtHoras7.Text.Trim = "" Then
            txtHoras7.Text = "0"
        End If
        If txtHoras8.Text.Trim = "" Then
            txtHoras8.Text = "0"
        End If
        If txtHoras9.Text.Trim = "" Then
            txtHoras9.Text = "0"
        End If
        If txtHoras10.Text.Trim = "" Then
            txtHoras10.Text = "0"
        End If
        If txtHoras11.Text.Trim = "" Then
            txtHoras11.Text = "0"
        End If
        If txtHoras12.Text.Trim = "" Then
            txtHoras12.Text = "0"
        End If

        SQL = " INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Janeiro'," & txtDias1.Text & "," & txtHoras1.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Fevereiro'," & txtDias2.Text & "," & txtHoras2.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Março'," & txtDias3.Text & "," & txtHoras3.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Abril'," & txtDias4.Text & "," & txtHoras4.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Maio'," & txtDias5.Text & "," & txtHoras5.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Junho'," & txtDias6.Text & "," & txtHoras6.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Julho'," & txtDias7.Text & "," & txtHoras7.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Agosto'," & txtDias8.Text & "," & txtHoras8.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Setembro'," & txtDias9.Text & "," & txtHoras9.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Outubro'," & txtDias10.Text & "," & txtHoras10.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Novembro'," & txtDias11.Text & "," & txtHoras11.Text & ")" &
            "   INSERT INTO tblDiasHorasMes (ano, mes, qtdDiasUteis, qtdHorasPorMes) VALUES (" & anoCorrente & ",'Dezembro'," & txtDias12.Text & "," & txtHoras12.Text & ")"
        If Not comandoSQL(SQL) Then
            lblDiasHorasMes.Style.Add("color", "Red")
            lblDiasHorasMes.Text = sqlErro
            Return
        Else
            lblDiasHorasMes.Style.Add("color", "Blue")
            lblDiasHorasMes.Text = "Alteração com sucesso."
        End If

    End Sub

    Protected Sub ddlAno_SelectedIndexChanged(sender As Object, e As EventArgs)
        CriarCalendario()
    End Sub
    Protected Sub ddlCalendSalvo_SelectedIndexChanged(sender As Object, e As EventArgs)
        lblMensagem.Text = String.Empty
        Calendario.Visible = False
        txtNomeCalendario.Text = String.Empty
        ddlAno.SelectedValue = ""

        'Verifica se é para mostrar o calendário salvo
        If Not String.IsNullOrWhiteSpace(ddlCalendSalvo.SelectedValue) Then
            txtNomeCalendario.Enabled = False
            ddlAno.Enabled = False
            CriarCalendarioSalvo()
        Else
            txtNomeCalendario.Enabled = True
            ddlAno.Enabled = True
        End If
    End Sub
    Protected Sub btnSalvar_Click(sender As Object, e As EventArgs)
        'valida os campos antes de salvar
        If ValidarCalendario() Then
            SalvarCalendario()
        End If

    End Sub
    Private Sub LimparDados()
        PreencherCalendSalvos(ddlCalendSalvo)
        txtNomeCalendario.Text = String.Empty
        ddlAno.SelectedValue = ""
        txtNomeCalendario.Enabled = True
        ddlAno.Enabled = True
        Calendario.Visible = False
    End Sub
    Private Function ValidarCalendario() As Boolean
        Try
            Dim Lista As List(Of Calendario) = RetornarDadosCalendario()
            Dim Reg As New Regex("(^([0-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])$)|(^([0-9]|[1][0-9]|[2][0-3])$)")

            'valida o campo de nome do calendario
            If txtNomeCalendario.Enabled Then
                If String.IsNullOrWhiteSpace(txtNomeCalendario.Text) Then
                    lblMensagem.Text = "Preencha um nome para o calendário."
                    txtNomeCalendario.Focus()
                    Return False
                Else
                    'verifica se ja existe um nome de calendario neste ano
                    SQL = "select id from tblcalendario with(nolock) where dscalendario='" & txtNomeCalendario.Text & "' and ano=" & ddlAno.SelectedValue

                    If selectSQL(SQL) Then
                        If dr.HasRows Then
                            lblMensagem.Text = "Calendário ja existente com este nome para este ano."
                            Return False
                        End If
                    End If

                End If
            End If

            'verifica se existe alguma hora invalida
            For Each Inv In Lista
                If Not Reg.IsMatch(Inv.Hora) Then
                    lblMensagem.Text += String.Format("A data:{0} esta com o horário inválido.", String.Concat(Inv.Dia, "/", Inv.Mes, "/", Inv.Ano)) & vbCrLf
                End If

            Next
        Catch ex As Exception
            lblMensagem.Text = String.Format("Ocorreu um erro ao validar o calendário com descrição: {0}", ex.Message)
        End Try
        Return True
    End Function
    Private Function RetornarDadosCalendario() As List(Of Calendario)
        Dim ListaDados As New List(Of Calendario)
        Try
            Dim Dia As DateTime

            If ddlAno.Enabled Then
                Dia = DateTime.Parse("01/01/" & ddlAno.SelectedValue)
            Else
                Dia = DateTime.Parse("01/01/" & ddlCalendSalvo.SelectedValue.Split("|")(1))
            End If

            Dim DiaContador As Int16 = 1, MesContador As Int16 = 1

            'Pega os dados dos calendarios
            For MesContador = 1 To 12
                'Pega a tabela de acordo com o mês
                Dim Table As Table = updtMeses.FindControl(String.Concat("Tb", MesContador))
                Dim Calend As Calendario

                If Not Table Is Nothing Then
                    'Contador dos dias do mês
                    For DiaContador = 1 To Date.DaysInMonth(Dia.ToString("yyyy"), MesContador)
                        Dim TextBox As New TextBox
                        'Cria a variavel com o dia para manipulação de semana
                        Dim DiaMes As New DateTime(Dia.ToString("yyyy"), MesContador, DiaContador)

                        'Pega os controles coms os devidos valores
                        TextBox = Table.FindControl(String.Concat("txt_", DiaMes.ToString("MMMM", New CultureInfo("pt-BR")), "_", DiaContador.ToString))

                        If Not IsNothing(TextBox) Then
                            If Not String.IsNullOrWhiteSpace(TextBox.Text) Then
                                Calend = New Calendario
                                Calend.Dia = DiaContador
                                Calend.Mes = MesContador
                                Calend.Hora = TextBox.Text
                                Calend.Ano = Dia.ToString("yyyy")
                                ListaDados.Add(Calend)
                            End If
                        End If

                    Next
                End If
            Next

        Catch ex As Exception
            lblMensagem.Text = String.Format("Ocorreu um erro ao buscar os dados do calendário com descrição: {0}", ex.Message)
        End Try
        Return ListaDados
    End Function
    Private Sub SalvarCalendario()
        Try
            Dim Lista As List(Of Calendario) = RetornarDadosCalendario()

            If txtNomeCalendario.Enabled Then
                IncluirCalendario(Lista)
            Else
                AlterarCalendario(Lista)
            End If

            LimparDados()
        Catch ex As Exception
            lblMensagem.Text = String.Format("Ocorreu um erro ao salvar o calendário com descrição: {0}", ex.Message)
        End Try
    End Sub
    Private Sub IncluirCalendario(ByVal Lista As List(Of Calendario))
        For Each Calend In Lista
            SQL = String.Format("insert into tblcalendario (DsCalendario,Dia,Mes,Ano,Hora) values ('{0}',{1},{2},{3},'{4}')", txtNomeCalendario.Text, Calend.Dia, Calend.Mes, Calend.Ano, Calend.Hora)
            If comandoSQL(SQL) Then
                lblMensagem.Text = "Dados salvo com sucesso."
            End If
        Next
    End Sub
    Private Sub AlterarCalendario(ByVal Lista As List(Of Calendario))
        Dim DsCalendario As String = ddlCalendSalvo.SelectedValue.Split("|")(0)
        Dim Ano As Integer = ddlCalendSalvo.SelectedValue.Split("|")(1)

        SQL = String.Format("delete tblcalendario where dscalendario='{0}' and ano={1}", DsCalendario, Ano)
        If comandoSQL(SQL) Then
            lblMensagem.Text = "Dados salvo com sucesso."
        End If

        For Each Calend In Lista
            SQL = String.Format("insert into tblcalendario (DsCalendario,Dia,Mes,Ano,Hora) values ('{0}',{1},{2},{3},'{4}')", DsCalendario, Calend.Dia, Calend.Mes, Ano, Calend.Hora)
            If comandoSQL(SQL) Then
                lblMensagem.Text = "Dados atualizados com sucesso."
            End If
        Next
    End Sub

End Class