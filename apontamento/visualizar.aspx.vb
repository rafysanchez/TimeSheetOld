Public Partial Class visualizar
    Inherits System.Web.UI.Page

    Dim colCodigo As String
    Dim proCodigo As String
    Dim dataInicio As DateTime
    Dim dataFim As DateTime
    Dim varData As DateTime
    Dim SQL As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

        If ValidarSessao(Session("colPerfilLogado")) Then Response.Redirect("..\Default.aspx")

        lblResponsavel.Text = ""
        lblCliente.Text = ""
        lblColNome.Text = ""
        lblMesReferencia.Text = ""
        lblProjeto.Text = ""
        lnkVoltar.Visible = False

        Try
            If Not IsNothing(Session("p_colCodigo")) Then
                colCodigo = Session("p_colCodigo")
                proCodigo = Session("p_proCodigo")
                dataInicio = Session("p_dataInicio")
            Else
                colCodigo = Request("Projeto")
                proCodigo = Request("Colaborador")
                dataInicio = Request("Data")
            End If


            If IsNumeric(colCodigo) And IsNumeric(proCodigo) And IsDate(dataInicio) Then
                preencherTabelaApontamento(colCodigo, proCodigo, dataInicio)
            Else
                divCalendario.Visible = False
                lnkVoltar.Visible = True
                lblErro.Text = "Passagem de paramêtros incorretos..."
            End If

        Catch ex As Exception
            divCalendario.Visible = False
            lnkVoltar.Visible = True
            lblErro.Text = ex.Message
        End Try

    End Sub

    '========================================================================================================================
    '   Preenche a tabela de Apontamento necessitando em session os valores Data Inicio e Data Fim
    '========================================================================================================================
    Private Sub preencherTabelaApontamento(ByVal colCodigo As Integer, ByVal proCodigo As Integer, ByVal dataInicio As DateTime)

        dataFim = dataInicio.AddMonths(1).AddDays(-1)
        varData = dataInicio

        lblResponsavel.Text = getResponsavelProjeto(colCodigo, proCodigo, dataInicio, dataFim)

        SQL = "SELECT colNome, colModulo, colNivel FROM v_colaboradores WHERE colCodigo = " & colCodigo
        ' Preenchendo o Label Nome do Colaborador
        If selectSQL(SQL) Then
            dr.Read()
            lblColNome.Text = dr("colNome")
            lblColModulo.Text = dr("colModulo")
            lblColNivel.Text = dr("colNivel")
        Else
            lblErro.Text = sqlErro
            Return
        End If

        ' Preenchendo o Label Mês de referência Ex: "JANEIRO"
        lblMesReferencia.Text = Format(dataFim, "MMMM").ToUpper

        ' Preenchendo o Label Periodo
        lblPeriodo.Text = dataInicio & " a " & dataFim

        SQL = "SELECT proNome, proNomeCliente FROM v_projetos WHERE proCodigo = " & proCodigo
        ' Preenchendo o Label Nome do Projeto
        If selectSQL(SQL) Then
            dr.Read()
            lblProjeto.Text = dr("proNome")
            lblCliente.Text = dr("proNomeCliente")
        Else
            lblErro.Text = sqlErro
        End If

        ' Coleta em uma Array todos os dias do apontamento formatado no padrão Ex:"01/12 DOM"
        Dim count As Integer = 1
        Dim array(50) As String
        Dim arrayDatas(50) As String
        varData = dataInicio
        While varData <> dataFim.AddDays(1)
            arrayDatas(count) = varData
            If varData.Day <= 9 And varData.Month <= 9 Then
                array(count) = "0" & varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day <= 9 And varData.Month > 9 Then
                array(count) = "0" & varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day > 9 And varData.Month <= 9 Then
                array(count) = varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day > 9 And varData.Month > 9 Then
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

        SQL = "SELECT * FROM v_apontamento WHERE colCodigo = " & colCodigo & " and proCodigo = " & proCodigo
        SQL += " and apoData BETWEEN '" & dataInicio & "' and '" & dataFim & "'"

        Dim controle As Control = Me.form1
        Dim entrada As String = ""
        Dim entAlmoco As String = ""
        Dim saiAlmoco As String = ""
        Dim saida As String = ""
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

        If selectSQL(SQL) Then
            ' Restaura novamente a variavel usada anteriormente com a data de inicio
            varData = dataInicio
            For i = 1 To DateDiff(DateInterval.Day, dataInicio, dataFim) + 1
                dr.Read()
                Try
                    For j = 1 To 31
                        Dim dataCompare As DateTime = DateTime.Parse(dr("apoData"))
                        If varData <> dataCompare Then
                            varData = varData.AddDays(1)
                            i = i + 1
                        Else
                            Exit For
                        End If
                    Next j

                    horaNormal = "00:00"
                    horaExtra = "00:00"
                    horaTotal = "00:00"

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
                        CType(controle, Label).Style.Add("align", "left")
                        CType(controle, Label).Style.Add("margin-left", "5px")
                        CType(controle, Label).Text = " " & dr("apoDescricao").ToString
                    End If

                    'Calculos dos totais de horas normais, extras e total 
                    somaNormais = somaHoras(somaNormais, horaNormal)
                    somaExtras = somaHoras(somaExtras, horaExtra)
                    somaTotal = somaHoras(somaTotal, horaTotal)

                Catch ex As Exception
#If DEBUG Then
                    lblErro.Text = "Mensagem de erro interno: " & ex.Message
#End If
                    lblErro.Text = ""
                End Try

                varData = varData.AddDays(1)

            Next i
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

    '==========================================================================================================
    ' Função para auxiliar na marcação dos feriados
    '==========================================================================================================
    Private Function isDiaUtil(ByVal dia As DateTime) As Boolean
        If dia.DayOfWeek = System.DayOfWeek.Saturday Or dia.DayOfWeek = System.DayOfWeek.Sunday Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub limpar(ByVal controleP As Control)

        Dim controle As Control

        ' Limpa todos os Label da página
        For Each controle In controleP.Controls
            If TypeOf controle Is Label Then
                DirectCast(controle, Label).Text = ""
            ElseIf controle.Controls.Count > 0 Then
                limpar(controle)
            End If
        Next

    End Sub

    Private Sub lnkVoltar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkVoltar.Click
        Response.Redirect("apontamentoNovo.aspx")
    End Sub

End Class