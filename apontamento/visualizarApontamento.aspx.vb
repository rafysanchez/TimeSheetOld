Imports System
Imports System.IO

Partial Public Class visualizarApontamento
    Inherits System.Web.UI.Page

    Dim colCodigo As String
    Dim proCodigo As String
    Dim dataInicio As DateTime
    Dim dataFim As DateTime
    Dim varData As DateTime
    Dim SQL As String
    Dim anoCorrente As Integer = Date.Now.Year

    Const primeiroAnoSistema As Integer = 2011

    '===================================================================================================================
    ' Page_Load
    '===================================================================================================================
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim URL = Request.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~/")
        txtURL.Value = URL + "apontamentoReport/apontamento.aspx"

        Try
            If (Not Me.Session("permissao").ToString().Contains("Visualizar Apontamento")) Then
                Session.Clear()
                Session.Abandon()
                Response.Redirect("~/Default.aspx")
            End If
        Catch exception As System.Exception
            Session.Clear()
            Session.Abandon()
            Response.Redirect("~/Default.aspx")
        End Try

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

        If Not IsPostBack Then

            lblTitulo1.Style.Add("font-style", "italic")
            lblTitulo1.Style.Add("color", "Gray")

            divTabelaApontamento.Style.Add("display", "none")

            ' Carrega comboBox com todos os projetos quando perfil é igual a Diretoria ou Administrador
            Select Case Session("perCodigoLogado")
                Case 2 ' Gerente de projetos
                    SQL = "SELECT proCodigo, proNome FROM v_projetos WHERE codGP = " & Session("colCodigoLogado") & " and upper(proStatus)='ATIVO'"
                Case 3 ' Gerente de contas
                    SQL = "Select proCodigo, proNome FROM v_projetos WHERE codGC = " & Session("colCodigoLogado") & " and upper(proStatus)='ATIVO'"
                Case 4 ' Diretoria
                    SQL = "Select proNome, proCodigo FROM v_projetos where upper(proStatus)='ATIVO'"
                Case 5 ' CSC
                    SQL = "Select proNome, proCodigo FROM v_projetos where upper(proStatus)='ATIVO'"
                Case 7 ' Administrador
                    SQL = "Select proNome, proCodigo FROM v_projetos where upper(proStatus)='ATIVO'"
                Case Else
                    Return
            End Select

            SQL += " ORDER BY proNome"

            Dim SQLCondicoes As String = ""
            Dim i As Integer = 0

            If selectSQL(SQL) Then
                While dr.Read
                    SQLCondicoes = If(i <> 0, String.Concat(SQLCondicoes, "Or tblColaboradores_Projetos.proCodigo = ", dr("proCodigo"), " "), String.Concat(SQLCondicoes, "WHERE tblColaboradores_Projetos.proCodigo = ", dr("proCodigo"), " "))

                    ddlProjetos.Items.Add(New System.Web.UI.WebControls.ListItem(dr("proNome"), dr("proCodigo")))
                    i = i + 1
                End While
            End If

            If (i <> 0) Then
                SQL = "Select tblColaboradores.colNome, tblColaboradores.colCodigo FROM tblColaboradores INNER JOIN tblColaboradores_projetos On tblColaboradores.colCodigo = tblColaboradores_Projetos.colCodigo "
                SQL = String.Concat(SQL, SQLCondicoes)
                SQL = String.Concat(SQL, "GROUP BY tblColaboradores.colNome, tblColaboradores.colCodigo ORDER BY tblColaboradores.colNome")

                If (Not selectSQL(SQL)) Then
                    lblErro.Text = sqlErro
                    Return
                End If

                While dr.Read()
                    lstColaboradores2.Items.Add(New ListItem(dr("colNome"), dr("colCodigo")))
                End While
            End If
        End If

    End Sub

    '========================================================================================================================
    '   Sub quando pressionado o botão Apontar Hora
    '========================================================================================================================
    Private Sub btnVisualizar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVisualizar.Click

        limpar()

        lblTitulo1.Style.Add("color", "red")

        If ddlProjetos.SelectedIndex = 0 Then
            lblTitulo1.Text = "Selecione um projeto, um colaborador, e uma data inicial para o Apontamento!"
            Return
        End If
        If lstColaboradores.SelectedIndex < 0 Then
            lblTitulo1.Text = "Selecione um projeto, um colaborador, e uma data inicial para o Apontamento!"
            Return
        End If
        If txtDataInicio.Text.Trim = "" Then
            lblTitulo1.Text = "Selecione um projeto, um colaborador, e uma data inicial para o Apontamento!"
            Return
        End If
        If Not IsDate(txtDataInicio.Text) Then
            lblTitulo1.Text = "Data inserida inválida!"
            Return
        End If

        lblTitulo1.Style.Add("color", "gray")
        lblTitulo1.Text = ""

        lblMensagem1.Style.Add("color", "gray")
        lblMensagem1.Text = ""
        colCodigo = lstColaboradores.SelectedValue
        proCodigo = ddlProjetos.SelectedValue
        dataInicio = DateTime.Parse(txtDataInicio.Text)
        Session("visApoColCodigo") = colCodigo
        Session("visApoProCodigo") = proCodigo
        Session("visApoDataInicio") = dataInicio
        Session("visApoAbaSelecionada") = 0

        preencherTabelaApontamento(colCodigo, proCodigo, dataInicio)

    End Sub

    '========================================================================================================================
    '   Preenche a tabela de Apontamento necessitando em session os valores Data Inicio e Data Fim
    '========================================================================================================================
    Private Sub preencherTabelaApontamento(ByVal colCodigo As String, ByVal proCodigo As String, ByVal dataInicio As DateTime)

        ' Preparando as variaveis de datas
        varData = dataInicio
        dataFim = dataInicio.AddMonths(1).AddDays(-1)

        ' Verifica se o projeto tem GC associado para exbição no campo responsavel, se caso não existir, exibe o Diretor.
        SQL = "If EXISTS(Select codGC FROM v_projetos WHERE proCodigo = " & proCodigo & " And codGC Is Not NULL ) " &
              "BEGIN " &
              " Select colNome FROM v_colaboradores WHERE colCodigo = (Select codGC FROM v_projetos WHERE proCodigo = " & proCodigo & ") " &
              "End " &
              "Else BEGIN " &
              " Select colNome FROM v_colaboradores WHERE colCodigo = (Select codDir FROM v_projetos WHERE proCodigo = " & proCodigo & ") " &
              "End"

        If Not selectSQL(SQL) Then
            lblTitulo1.Text = sqlErro
            Return
        Else
            dr.Read()
            lblResponsavel.Text = dr("colNome")
        End If
        '==================================================================================================================

        SQL = "Select * FROM v_relatorioApontamento WHERE " &
              "colCodigo = " & colCodigo & " And proCodigo = " & proCodigo & " And " &
              "apoData BETWEEN '" & dataInicio & "' and '" & dataFim & "'"

        If selectSQL(SQL) Then
            dr.Read()
            ' Se DataReader existe linhas
            If dr.HasRows = True Then
                ' Exibe a tabela de apontamento
                divTabelaApontamento.Style.Add("display", "block")
            Else
                lblTitulo1.Style.Add("color", "red")
                lblTitulo1.Text = "No periodo selecionado não há apontamentos registrados."
                ' Esconde a tabela de apontamento
                divTabelaApontamento.Style.Add("display", "none")
                Return
            End If
        Else
            lblTitulo1.Text = sqlErro
            Return
        End If

        ' Dependendo do perfil muda o titulo do primeiro campo
        Select Case dr("perCodigo")
            Case 1 ' Consultores
                lblTituloPerfil.Text = "Consultor:"
            Case 2 ' Gerente de projeto
                lblTituloPerfil.Text = "Gerente de projeto:"
            Case 3 ' Gerente de contas
                lblTituloPerfil.Text = "Gerente de contas:"
            Case 4  ' Diretoria
                lblTituloPerfil.Text = "Diretor:"
            Case 5 ' CSC
                lblTituloPerfil.Text = "CSC:"
            Case 6 ' Recrutamento
                lblTituloPerfil.Text = "Recrutamento:"
            Case 7 ' Administrador
                lblTituloPerfil.Text = "Administrador:"
        End Select

        ' Preenche a primeira linha do relatório
        lblColNome.Text = dr("colNome")
        lblColModulo.Text = dr("colModulo")
        lblColNivel.Text = dr("colNivel")

        ' Preenchendo o Label Mês de referência Ex: "JANEIRO"
        lblMesReferencia.Text = Format(dataFim, "MMMM").ToUpper

        ' Preenchendo o Label Periodo
        lblPeriodo.Text = dataInicio & " a " & dataFim

        lblProjeto.Text = dr("proNome")
        lblCliente.Text = dr("proNomeCliente")

        ' Coleta em uma Array todos os dias do apontamento formatado no padrão Ex:"01/12 DOM"
        Dim count As Integer = 1
        Dim array(50) As String
        Dim arrayDatas(50) As String

        While count <= DateDiff(DateInterval.Day, dataInicio, dataFim) + 1
            arrayDatas(count) = varData
            If varData.Day <= 9 And varData.Month <= 9 Then
                array(count) = "0" & varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day <= 9 And varData.Month >= 9 Then
                array(count) = "0" & varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day >= 9 And varData.Month <= 9 Then
                array(count) = varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day >= 9 And varData.Month >= 9 Then
                array(count) = varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            varData = varData.AddDays(1)
            count = count + 1
        End While

        lblDia1.Text = array(1)
        lblDia2.Text = array(2)
        lblDia3.Text = array(3)
        lblDia4.Text = array(4)
        lblDia5.Text = array(5)
        lblDia6.Text = array(6)
        lblDia7.Text = array(7)
        lblDia8.Text = array(8)
        lblDia9.Text = array(9)
        lblDia10.Text = array(10)
        lblDia11.Text = array(11)
        lblDia12.Text = array(12)
        lblDia13.Text = array(13)
        lblDia14.Text = array(14)
        lblDia15.Text = array(15)
        lblDia16.Text = array(16)
        lblDia17.Text = array(17)
        lblDia18.Text = array(18)
        lblDia19.Text = array(19)
        lblDia20.Text = array(20)
        lblDia21.Text = array(21)
        lblDia22.Text = array(22)
        lblDia23.Text = array(23)
        lblDia24.Text = array(24)
        lblDia25.Text = array(25)
        lblDia26.Text = array(26)
        lblDia27.Text = array(27)
        lblDia28.Text = array(28)
        linha29.Visible = True
        linha30.Visible = True
        linha31.Visible = True

        'Verifica total de dias para apontamento, para exibir ou não as últimas linhas
        Select Case count - 1
            Case 28
                linha29.Visible = False
                linha30.Visible = False
                linha31.Visible = False
            Case 29
                lblDia29.Text = array(29)
                linha30.Visible = False
                linha31.Visible = False
            Case 30
                lblDia29.Text = array(29)
                lblDia30.Text = array(30)
                linha31.Visible = False
            Case 31
                lblDia29.Text = array(29)
                lblDia30.Text = array(30)
                lblDia31.Text = array(31)
        End Select

        Dim controle As Control = Me.form1
        Dim entrada As String = ""
        Dim entAlmoco As String = ""
        Dim saiAlmoco As String = ""
        Dim saida As String = ""
        Dim atividades As String = ""
        Dim horaNormal As String = ""
        Dim horaExtra As String = ""
        Dim horaTotal As String = ""
        Dim somaNormais As String = "00:00"
        Dim somaExtras As String = "00:00"
        Dim somaTotal As String = "00:00"

        ' Restaura novamente a variavel usada anteriormente com a data de inicio
        varData = dataInicio

        ' Loop para pintar as linhas que pertence a dias de feriados
        For i = 1 To 31
            If Not isDiaUtil(varData) Then
                controle = controle.FindControl("linha" & i)
                CType(controle, HtmlTableRow).BgColor = "#E8E8E8"
            End If
            varData = varData.AddDays(1)
        Next i

        ' Restaura novamente a variavel usada anteriormente com a data de inicio
        varData = dataInicio
        ' Criei esta variavel para comparar quantas linhas sem dados existe e se for igual ao numero de linhas da tabela
        ' significa que temos uma tabela que esta em branco.
        Dim numLinhasSemDados = 0

        For i = 1 To DateDiff(DateInterval.Day, dataInicio, dataFim) + 1

            horaNormal = "00:00"
            horaExtra = "00:00"
            horaTotal = "00:00"

            entrada = ""
            entAlmoco = ""
            saiAlmoco = ""
            saida = ""
            atividades = ""

            Try
                For j = 1 To 31
                    Dim dataCompare As DateTime = DateTime.Parse(dr("apoData"))
                    If varData <> dataCompare Then
                        varData = varData.AddDays(1)
                        i = i + 1
                        numLinhasSemDados = numLinhasSemDados + 1
                    Else
                        Exit For
                    End If
                Next j

                If dr("apoEntrada") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblEntrada" & i)
                    entrada = FormatDateTime(dr("apoEntrada").ToString, DateFormat.ShortTime)
                    CType(controle, Label).Text = entrada
                End If

                If dr("apoEntAlmoco") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblEntAlmoco" & i)
                    entAlmoco = FormatDateTime(dr("apoEntAlmoco").ToString, DateFormat.ShortTime)
                    CType(controle, Label).Text = entAlmoco
                End If

                If dr("apoSaiAlmoco") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblSaiAlmoco" & i)
                    saiAlmoco = FormatDateTime(dr("apoSaiAlmoco").ToString, DateFormat.ShortTime)
                    CType(controle, Label).Text = saiAlmoco
                End If

                If dr("apoSaida") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblSaida" & i)
                    saida = FormatDateTime(dr("apoSaida").ToString, DateFormat.ShortTime)
                    CType(controle, Label).Text = saida
                End If

                If dr("apoNormais") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblNormal" & i)
                    horaNormal = FormatDateTime(dr("apoNormais").ToString, DateFormat.ShortTime)
                    CType(controle, Label).Text = horaNormal
                End If

                If dr("apoExtras") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblExtra" & i)
                    horaExtra = FormatDateTime(dr("apoExtras").ToString, DateFormat.ShortTime)
                    CType(controle, Label).Text = horaExtra
                End If

                If dr("apoTotal") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblTotal" & i)
                    horaTotal = FormatDateTime(dr("apoTotal").ToString, DateFormat.ShortTime)
                    CType(controle, Label).Text = horaTotal
                End If

                If dr("apoDescricao") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblAtividades" & i)
                    atividades = dr("apoDescricao")
                    CType(controle, Label).Style.Add("align", "left")
                    CType(controle, Label).Style.Add("margin-left", "5px")
                    CType(controle, Label).Text = " " & dr("apoDescricao")
                End If

                ' Verifico se todos os campos estão em branco para acrescentar +1 na variavel numLinhasSemDados
                If entrada.Trim = "" And entAlmoco.Trim = "" And saiAlmoco.Trim = "" And saida.Trim = "" And atividades.Trim = "" Then
                    numLinhasSemDados = numLinhasSemDados + 1
                End If

                'Calculos dos totais de horas normais, extras e total das duas
                somaNormais = somaHoras(somaNormais, horaNormal)
                somaExtras = somaHoras(somaExtras, horaExtra)
                somaTotal = somaHoras(somaTotal, horaTotal)

                varData = varData.AddDays(1)

            Catch ex As Exception
#If DEBUG Then
                lblErro.Text = "Mensagem de erro interno: " & ex.Message
#End If
                lblErro.Text = ""
                ' Se gerou uma exeção siginifica que neste periodo não ha nada gravado, então é contabilizado como uma linha
                ' sem dados
                numLinhasSemDados = numLinhasSemDados + 1
            End Try

            dr.Read()

        Next i

        Dim numTotalDias = DateDiff(DateInterval.Day, dataInicio, dataFim) + 1

        ' Verifico se o número de linhas sem dados é igual ao número de dias da tabela
        If numLinhasSemDados = numTotalDias Then
            lblTitulo1.Style.Add("color", "red")
            lblTitulo1.Text = "No periodo selecionado não há apontamentos registrados."
            ' Esconde a tabela de apontamento
            divTabelaApontamento.Style.Add("display", "none")
        End If

        If somaNormais.Length = 4 Then
            somaNormais = "0" & somaNormais
        End If
        If somaExtras.Length = 4 Then
            somaExtras = "0" & somaExtras
        End If
        If somaTotal.Length = 4 Then
            somaTotal = "0" & somaTotal
        End If

        lblTotalNormais.Text = somaNormais
        lblTotalExtras.Text = somaExtras
        lblTotalTotal.Text = somaTotal

    End Sub

    Private Sub ddlProjetos_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlProjetos.SelectedIndexChanged
        If ddlProjetos.SelectedIndex > 0 Then
            preencheListaColaboradores(ddlProjetos.SelectedValue)
        Else
            lstColaboradores.Items.Clear()
        End If
    End Sub

    Private Sub preencheListaColaboradores(ByVal proCodigo As Integer)

        lstColaboradores.Items.Clear()
        Dim arrayColCodigos As New ArrayList
        lstColaboradores.Enabled = True

        Dim colNome
        Dim colStatus
        Dim colNivel
        Dim colModulo
        Dim colPerfil
        Dim perCodigo

        SQL = "SELECT tblColaboradores_Projetos.*, tblColaboradores.* " &
                "FROM tblColaboradores INNER JOIN " &
                "tblColaboradores_Projetos ON tblColaboradores.colCodigo = tblColaboradores_Projetos.colCodigo INNER JOIN " &
                "tblProjetos ON tblColaboradores_Projetos.proCodigo = tblProjetos.proCodigo " &
                "WHERE tblProjetos.proCodigo = " & proCodigo & " ORDER BY colNome"

        If selectSQL(SQL) Then
            While dr.Read
                ' Lista os seguintes colaboradores associados ao projeto selecionado e que realizam apontamento no sistema
                ' 1 = Consultores
                ' 2 = Gerente de Projeto
                ' 3 = Gerente de Contas
                ' 4 = Diretoria
                ' 5 = CSC
                ' 6 = Recrutamento
                ' 7 = Administrador
                ' 8 = Funcionários
                'Select Case dr("perCodigo")
                '    Case 4
                '        ' Se caso for Diretoria não adiciona na lista pois diretoria não realiza apontamento
                '    Case Else
                arrayColCodigos.Add(dr("colCodigo"))
                'End Select
            End While
        End If

        If arrayColCodigos.Count > 0 Then
            For i = 0 To (arrayColCodigos.Count - 1)
                SQL = "SELECT * FROM v_colaboradores WHERE colCodigo = " & arrayColCodigos(i)
                If selectSQL(SQL) Then
                    dr.Read()
                    colNome = dr("colNome")
                    colPerfil = dr("colPerfil")
                    colModulo = dr("colModulo")
                    colStatus = dr("colStatus")
                    colNivel = dr("colNivel")
                    colCodigo = dr("colCodigo")
                    perCodigo = dr("perCodigo")

                    ' Se perfil for Consultores, adiciono Módulo e Nivel a frente do nome 
                    'If perCodigo = 1 Then
                    '    lstColaboradores.Items.Add(New ListItem(colNome & " - " & colModulo & " - " & colNivel, colCodigo))
                    'ElseIf perCodigo <> 4 Then
                    'Se for perfil for diferente de "Diretoria" não adiciona na lista pois diretoria não realiza apontamento
                    lstColaboradores.Items.Add(New ListItem(colNome & " - " & colPerfil, colCodigo))
                    'End If

                Else
                    'lblTeste.Text = sqlErro
                End If
            Next i
        Else
            lstColaboradores.Enabled = False
            lstColaboradores.Items.Add("Não há consultores associados à este projeto")
        End If

    End Sub

    '================================================================================================================
    ' Rotina para limpar os Label entrada, entAlmoco, saiAlmoco, saida, atividades e também deixar as linhas em branco
    '================================================================================================================
    Private Sub limpar()

        Dim controle As Control = Me.form1

        For i = 1 To 31
            Try
                controle = controle.FindControl("lblEntrada" & i)
                CType(controle, Label).Text = ""

                controle = controle.FindControl("lblEntAlmoco" & i)
                CType(controle, Label).Text = ""

                controle = controle.FindControl("lblSaiAlmoco" & i)
                CType(controle, Label).Text = ""

                controle = controle.FindControl("lblSaida" & i)
                CType(controle, Label).Text = ""

                controle = controle.FindControl("lblAtividades" & i)
                CType(controle, Label).Text = ""

                controle = controle.FindControl("lblNormal" & i)
                CType(controle, Label).Text = ""

                controle = controle.FindControl("lblExtra" & i)
                CType(controle, Label).Text = ""

                controle = controle.FindControl("lblTotal" & i)
                CType(controle, Label).Text = ""

                controle = controle.FindControl("linha" & i)
                CType(controle, HtmlTableRow).BgColor = "#FFFFFF"

            Catch ex As Exception
                'lblTeste.Text = "Erro"
            End Try

        Next i

        lblTotalNormais.Text = ""
        lblTotalExtras.Text = ""
        lblTotalTotal.Text = ""

    End Sub

    Protected Sub btnVisualizar2_Click(sender As Object, e As EventArgs) Handles btnVisualizar2.Click
        limpar()
        lblMensagem2.Style.Add("color", "red")

        If (lstColaboradores2.SelectedIndex < 0) Then
            lblMensagem2.Text = "Selecione um projeto, um colaborador, e uma data inicial para o Apontamento!"
            Return
        End If

        If txtDataInicio2.Text.Trim() = "" Then
            lblMensagem2.Text = "Selecione um projeto, um colaborador, e uma data inicial para o Apontamento!"
            Return
        End If

        If (Not IsDate(txtDataInicio2.Text)) Then
            lblMensagem2.Text = "Data inserida inválida!"
            Return
        End If
        lblMensagem2.Style.Add("color", "gray")
        lblMensagem2.Text = ""
        colCodigo = lstColaboradores2.SelectedValue
        dataInicio = DateTime.Parse(txtDataInicio2.Text)
        Session("visApoColCodigo") = colCodigo
        Session("visApoProCodigo") = proCodigo
        Session("visApoDataInicio") = dataInicio
        Session("visApoAbaSelecionada") = 1
        preencherTabelaApontamento2(colCodigo, dataInicio)

    End Sub
    Private Sub preencherTabelaApontamento2(ByVal colCodigo As String, ByVal dataInicio As DateTime)
        Dim str As String()
        Dim objArray As Object()
        varData = dataInicio
        Dim dateTime As System.DateTime = dataInicio.AddMonths(1)
        dataFim = dateTime.AddDays(-1)
        Dim numDias As Int16 = CInt((DateAndTime.DateDiff(DateInterval.Day, dataInicio, dataFim, Microsoft.VisualBasic.FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1) + CLng(1)))
        Dim controle As Control = form1
        Dim data As String = ""
        Dim entrada As String = ""
        Dim entAlmoco As String = ""
        Dim saiAlmoco As String = ""
        Dim saida As String = ""
        Dim horasNormal As String = ""
        Dim horasExtra As String = ""
        Dim horasTotal As String = ""
        Dim descricao As String = ""
        Dim somaNormais As String = "00:00"
        Dim somaExtras As String = "00:00"
        Dim somaTotal As String = "00:00"
        Dim exibeErroDeApontamento As Boolean = False
        Dim dtApontamento As DataTable = New DataTable()
        dtApontamento.Columns.Add("data")
        dtApontamento.Columns.Add("entrada")
        dtApontamento.Columns.Add("entAlmoco")
        dtApontamento.Columns.Add("saiAlmoco")
        dtApontamento.Columns.Add("saida")
        dtApontamento.Columns.Add("normais")
        dtApontamento.Columns.Add("extras")
        dtApontamento.Columns.Add("total")
        dtApontamento.Columns.Add("descricao")
        SQL = String.Concat("SELECT * FROM v_colaboradores WHERE colCodigo = ", colCodigo)
        If (Not selectSQL(SQL)) Then
            lblErro.Text = sqlErro
            Return
        End If
        dr.Read()
        lblColNome.Text = dr("colNome")
        lblColModulo.Text = dr("colModulo")
        lblColNivel.Text = dr("colNivel")
        lblMesReferencia.Text = Strings.Format(dataFim, "MMMM").ToUpper()
        lblPeriodo.Text = String.Concat(dataInicio.ToShortDateString(), " a ", dataFim.ToShortDateString())
        lblCliente.Text = String.Empty
        lblProjeto.Text = String.Empty
        lblResponsavel.Text = String.Empty
        While DateTime.Compare(Me.varData, Me.dataFim) <= 0
            entrada = ""
            Dim decEntrada As Decimal = New Decimal()
            Dim decTotal As Decimal = New Decimal()
            Dim decSaida As Decimal = New Decimal()
            entAlmoco = ""
            saiAlmoco = ""
            saida = ""
            horasNormal = ""
            horasExtra = ""
            horasTotal = ""
            descricao = ""
            If (varData.Day <= 9 And varData.Month <= 9) Then
                str = New String() {"0", varData.Day, "/0", varData.Month, " ", Strings.Format(varData, "ddd").ToUpper()}
                data = String.Concat(str)
            End If
            If (varData.Day <= 9 And varData.Month >= 9) Then
                str = New String() {"0", varData.Day, "/", varData.Month, " ", Strings.Format(varData, "ddd").ToUpper()}
                data = String.Concat(str)
            End If
            If (varData.Day >= 9 And varData.Month <= 9) Then
                str = New String() {varData.Day, "/0", varData.Month, " ", Strings.Format(varData, "ddd").ToUpper()}
                data = String.Concat(str)
            End If
            If (varData.Day >= 9 And varData.Month >= 9) Then
                str = New String() {varData.Day, "/", varData.Month, " ", Strings.Format(varData, "ddd").ToUpper()}
                data = String.Concat(str)
            End If
            SQL = "SELECT * FROM v_relatorioApontamento WHERE colCodigo = " & colCodigo & " AND apoData = '" & varData & "'"
            If (Not selectSQL(SQL)) Then
                lblErro.Text = funcoesGerais.sqlErro
                Return
            End If
            If (Not dr.HasRows()) Then
                Dim rows As System.Data.DataRowCollection = dtApontamento.Rows
                objArray = New Object() {data, "", "", "", "", "", "", "", ""}
                rows.Add(objArray)
            Else
                Dim listEntrada As List(Of String) = New List(Of String)()
                While dr.Read()
                    If (Not Information.IsDBNull(dr("apoEntrada"))) Then
                        entrada = dr("apoEntrada")
                        listEntrada.Add(entrada)
                    End If

                    If (Not Information.IsDBNull(dr("apoEntAlmoco"))) Then
                        entAlmoco = dr("apoEntAlmoco")
                    End If

                    If (Not Information.IsDBNull(dr("apoSaiAlmoco"))) Then
                        saiAlmoco = dr("apoSaiAlmoco")
                    End If

                    If (Not Information.IsDBNull(dr("apoSaida"))) Then
                        saida = dr("apoSaida")
                    End If

                    If (Not Information.IsDBNull(dr("apoNormais"))) Then
                        horasNormal = somaHoras(dr("apoNormais"), horasNormal)
                    End If

                    If (Not Information.IsDBNull(dr("apoExtras"))) Then
                        horasExtra = somaHoras(dr("apoExtras"), horasExtra)
                    End If

                    If (Information.IsDBNull(dr("apoTotal"))) Then
                        Continue While
                    End If

                    horasTotal = somaHoras(dr("apoTotal"), horasTotal)
                    descricao += "(" & dr("proCentroCusto") & " - <b> " & dr("apoTotal") & "</b>)"
                End While
                If (listEntrada.Count > 1) Then
                    listEntrada.Sort()
                    entrada = listEntrada(0)
                    decEntrada = converteHorasDecimal(entrada)
                    decTotal = converteHorasDecimal(horasTotal)
                    decSaida = Decimal.Add(decEntrada, decTotal)
                    saida = converteDecimalHoras(decSaida)
                    entAlmoco = ""
                    saiAlmoco = ""
                    If (Decimal.Compare(decTotal, New Decimal(CLng(24))) > 0) Then
                        entrada = "<span style='color:Red;'>*</span>"
                        entAlmoco = "<span style='color:Red;'>*</span>"
                        saiAlmoco = "<span style='color:Red;'>*</span>"
                        saida = "<span style='color:Red;'>*</span>"
                        horasNormal = ""
                        horasExtra = ""
                        horasTotal = converteDecimalHoras(decTotal)
                        descricao = String.Concat("<span style='color:Red; font-size:11px;'>", descricao, "</span>")
                        exibeErroDeApontamento = True
                        lblMensagem2.Style.Add("color", "red")
                        lblMensagem2.Text = "Existe dias onde foram apontamentos mais de 24 horas."
                    ElseIf (Convert.ToDouble(decSaida) > 24) Then
                        saida = "00:00"
                        entrada = converteDecimalHoras((Decimal.Subtract(New Decimal(CLng(24)), decTotal)))
                    End If
                    If (Decimal.Compare(decTotal, New Decimal(CLng(8))) > 0) Then
                        horasNormal = "08:00"
                        horasExtra = converteDecimalHoras(Decimal.Subtract(decTotal, New Decimal(CLng(8))))
                        horasTotal = converteDecimalHoras(decTotal)
                    End If
                End If
                somaNormais = somaHoras(somaNormais, horasNormal)
                somaExtras = somaHoras(somaExtras, horasExtra)
                somaTotal = somaHoras(somaTotal, horasTotal)
                Dim dataRowCollection As System.Data.DataRowCollection = dtApontamento.Rows
                objArray = New Object() {data, entrada, entAlmoco, saiAlmoco, saida, horasNormal, horasExtra, horasTotal, descricao}
                dataRowCollection.Add(objArray)
            End If
            varData = varData.AddDays(1)
        End While
        If (somaTotal = "00:00" And Not exibeErroDeApontamento) Then
            lblMensagem2.Style.Add("color", "red")
            lblMensagem2.Text = "No periodo selecionado não há apontamentos registrados."
            divTabelaApontamento.Style.Add("display", "none")
            Return
        End If
        divTabelaApontamento.Style.Add("display", "block")
        Dim num As Integer = numDias
        Dim num1 As Integer = 1
        Do
            controle = controle.FindControl(String.Concat("lblDia", num1))
            DirectCast(controle, Label).Text = dtApontamento.Rows(num1 - 1)(0).ToString()
            controle = controle.FindControl(String.Concat("lblEntrada", num1))
            DirectCast(controle, Label).Text = dtApontamento.Rows(num1 - 1)(1).ToString()
            controle = controle.FindControl(String.Concat("lblEntAlmoco", num1))
            DirectCast(controle, Label).Text = dtApontamento.Rows(num1 - 1)(2).ToString()
            controle = controle.FindControl(String.Concat("lblSaiAlmoco", num1))
            DirectCast(controle, Label).Text = dtApontamento.Rows(num1 - 1)(3).ToString()
            controle = controle.FindControl(String.Concat("lblSaida", num1))
            DirectCast(controle, Label).Text = dtApontamento.Rows(num1 - 1)(4).ToString()
            controle = controle.FindControl(String.Concat("lblNormal", num1))
            DirectCast(controle, Label).Text = dtApontamento.Rows(num1 - 1)(5).ToString()
            controle = controle.FindControl(String.Concat("lblExtra", num1))
            DirectCast(controle, Label).Text = dtApontamento.Rows(num1 - 1)(6).ToString()
            controle = controle.FindControl(String.Concat("lblTotal", num1))
            DirectCast(controle, Label).Text = dtApontamento.Rows(num1 - 1)(7).ToString()
            controle = controle.FindControl(String.Concat("lblAtividades", num1))
            DirectCast(controle, Label).Text = dtApontamento.Rows(num1 - 1)(8).ToString()
            num1 = num1 + 1
        Loop While num1 <= num
        varData = dataInicio
        Dim num2 As Integer = numDias
        Dim i As Integer = 1
        Do
            If (Not funcoesGerais.isDiaUtil(varData)) Then
                controle = controle.FindControl(String.Concat("linha", i))
                DirectCast(controle, HtmlTableRow).BgColor = "#E8E8E8"
            End If
            varData = varData.AddDays(1)
            i = i + 1
        Loop While i <= num2
        linha29.Visible = True
        linha30.Visible = True
        linha31.Visible = True
        Select Case numDias
            Case 28
                linha29.Visible = False
                linha30.Visible = False
                linha31.Visible = False
                Exit Select
            Case 29
                linha30.Visible = False
                linha31.Visible = False
                Exit Select
            Case 30
                linha31.Visible = False
                Exit Select
        End Select
        If (somaNormais.Length = 4) Then
            somaNormais = String.Concat("0", somaNormais)
        End If
        If (somaExtras.Length = 4) Then
            somaExtras = String.Concat("0", somaExtras)
        End If
        If (somaTotal.Length = 4) Then
            somaTotal = String.Concat("0", somaTotal)
        End If
        lblTotalNormais.Text = somaNormais
        lblTotalExtras.Text = somaExtras
        lblTotalTotal.Text = somaTotal
    End Sub


End Class

