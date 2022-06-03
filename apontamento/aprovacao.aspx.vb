Imports System.Data.Sql

Partial Public Class aprovacao
    Inherits System.Web.UI.Page

    Dim SQL As String
    Dim dataInicio As DateTime
    Dim dataFim As DateTime
    Dim proCodigo As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

#If DEBUG Then
        ' Select para teste, simula que algum consultor já tenha feito o login
        'Session("colCodigoLogado") = 122
        SQL = "SELECT tblColaboradores.colNome, tblColaboradores.perCodigo, tblPerfil.perDescricao " & _
              "FROM tblColaboradores " & _
              "INNER JOIN tblPerfil " & _
              "ON (tblColaboradores.perCodigo = tblPerfil.perCodigo) " & _
              "WHERE colCodigo = " & Session("colCodigoLogado") & ""
        If selectSQL(SQL) Then
            If dr.HasRows Then
                dr.Read()
                Session("colPerfilLogado") = dr("perDescricao")
                Session("colNomeLogado") = dr("colNome")
                Session("perCodigoLogado") = dr("perCodigo")
            Else
                lblMensagem.Text = "Não há dados com o código de colaborador informado... código = " & Session("colCodigoLogado")
            End If
        Else
            lblMensagem.Text = sqlErro
        End If
#Else
        Try
            If Not Session("permissao").ToString.Contains("Aprovacao") Then
                Session.Clear()
                Session.Abandon()
                Response.Redirect("~/Default.aspx")
            End If
        Catch ex As Exception
            Session.Clear()
            Session.Abandon()
            Response.Redirect("~/Default.aspx")
        End Try
#End If

        If Not IsPostBack Then

            'ddlMes.Enabled = False
            lblTeste.Style.Add("font-size", "small")

            lblCompetencia.Style.Add("color", "Gray")
            lblCompetencia.Style.Add("font-size", "11px")

            lblMensagem.Style.Add("color", "Red")
            lblMensagem.Style.Add("font-size", "11px")
            lblMensagem.Style.Add("font-style", "Italic")

            ' Faz um select dos projetos que estão Ativos e estão associados ao colaborador que podem aprovar. 
            Select Case Session("perCodigoLogado")
                Case 2 ' Gerente de Projetos
                    SQL = "SELECT proCodigo, proNome FROM v_projetos WHERE codGP = " & Session("colCodigoLogado")
                    SQL += " AND UPPER(PROSTATUS) = 'ATIVO' ORDER BY proNome"
                Case 3 ' 'Gerente de Contas
                    SQL = "SELECT proCodigo, proNome FROM v_projetos WHERE codGC = " & Session("colCodigoLogado")
                    SQL += " AND UPPER(PROSTATUS) = 'ATIVO' ORDER BY proNome"
                Case 4 ' Diretoria
                    SQL = "SELECT proCodigo, proNome FROM v_projetos WHERE codDir = " & Session("colCodigoLogado")
                    SQL += " AND UPPER(PROSTATUS) = 'ATIVO' ORDER BY proNome"
            End Select

            If selectSQL(SQL) Then
                ddlProjeto.Items.Add("")
                While dr.Read
                    ddlProjeto.Items.Add(New ListItem(dr("proNome"), dr("proCodigo")))
                End While
            End If

            ddlMes.Items.Add(New ListItem("", ""))
            ddlMes.Items.Add(New ListItem("Janeiro", "01"))
            ddlMes.Items.Add(New ListItem("Fevereiro", "02"))
            ddlMes.Items.Add(New ListItem("Março", "03"))
            ddlMes.Items.Add(New ListItem("Abril", "04"))
            ddlMes.Items.Add(New ListItem("Maio", "05"))
            ddlMes.Items.Add(New ListItem("Junho", "06"))
            ddlMes.Items.Add(New ListItem("Julho", "07"))
            ddlMes.Items.Add(New ListItem("Agosto", "08"))
            ddlMes.Items.Add(New ListItem("Setembro", "09"))
            ddlMes.Items.Add(New ListItem("Outubro", "10"))
            ddlMes.Items.Add(New ListItem("Novembro", "11"))
            ddlMes.Items.Add(New ListItem("Dezembro", "12"))

            ' Preenche o camboBox Ano com ano inicio do sistema 2011 até o ano corrente
            Dim anoInicio = 2011
            While anoInicio <= Date.Now.Year
                ddlAno.Items.Add(anoInicio)
                anoInicio = anoInicio + 1
            End While
            ' Deixa selecionado como padrão o ano corrente
            ddlAno.SelectedValue = Date.Now.Year

            statusAprovacao.DataBind()

        End If

    End Sub

    Private Sub ddlProjeto_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlProjeto.SelectedIndexChanged

        divCalendario.Style.Add("display", "none")
        lblCompetencia.Text = ""

        If ddlProjeto.SelectedIndex > 0 And ddlMes.SelectedIndex > 0 Then
            SQL = "SELECT * FROM v_periodoAprovacao " & _
                  "WHERE competencia = " & ddlMes.SelectedValue & " AND ano = " & ddlAno.SelectedValue
            If selectSQL(SQL) Then
                If dr.HasRows Then
                    dr.Read()
                    dataInicio = dr("dataInicio")
                    dataFim = dr("dataFim")
                    proCodigo = ddlProjeto.SelectedValue
                Else
                    lblCompetencia.Style.Add("color", "Red")
                    lblCompetencia.Text = "Tabela do periodo de aprovação para " & ddlAno.SelectedValue & " esta vazia, " & _
                                          "vá a página de configurações em periodo de aprovação, pressione ""Salvar"" " & _
                                          "para que seja preenchida esta tabela de parametrização."
                    statusAprovacao.DataBind()
                    Return
                End If
            Else
                lblMensagem.Text = sqlErro
                Return
            End If
            carregaTabelaParaAprovacao(proCodigo, dataInicio, dataFim)
        Else
            lblTeste.Text = ""
            lblCompetencia.Text = ""
            statusAprovacao.DataBind()
            divPaginas.Visible = False
        End If

    End Sub

    Private Sub ddlMes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMes.SelectedIndexChanged

        ddlProjeto_SelectedIndexChanged(sender, e)

    End Sub

    Private Sub ddlAno_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAno.SelectedIndexChanged

        ddlProjeto_SelectedIndexChanged(sender, e)

    End Sub

    Private Sub carregaTabelaParaAprovacao(ByVal proCodigo As Integer, ByVal dataInicio As DateTime, ByVal dataFim As DateTime)

        lblCompetencia.Style.Add("color", "Gray")
        lblCompetencia.Text = "Periodo de aprovação: <b style='color:#ED7522;'>" & dataInicio & "</b> até <b style='color:#ED7522;'>" & dataFim & "</b>."
        lblTeste.Text = ""
        divCalendario.Style.Add("display", "none")

        txtDataInicio.Value = dataInicio
        txtDataFim.Value = dataFim

        ' ''''''' Criando uma DataTable
        Dim dtAprovacao As New DataTable("consultores")
        ' ''''''' Criando as colunas para a DataTable
        dtAprovacao.Columns.Add("Nome", GetType(String))
        dtAprovacao.Columns.Add("GP", GetType(String))
        dtAprovacao.Columns.Add("GC", GetType(String))
        dtAprovacao.Columns.Add("Dir", GetType(String))
        dtAprovacao.Columns.Add("colCodigo", GetType(Integer))

        statusAprovacao.DataBind()
        divPaginas.Visible = False

        lblTeste.Style.Add("color", "red")
        lblTeste.Style.Add("font-style", "italic")
#If Not Debug Then
        lblTeste.Text = ""
#End If

        SQL = "SELECT colCodigo, colNome FROM v_relatorioApontamento " & _
              "WHERE proCodigo = " & proCodigo & " and " & _
              "apoData between '" & dataInicio & "' and '" & dataFim & "' GROUP BY colCodigo, colNome ORDER BY colNome"

        If selectSQL(SQL) Then

            If dr.HasRows Then

                ' Preenche em um array com os nomes dos consultores associados ao projeto e que fizeram apontamento
                ' no periodo selecionado
                Dim arrayColNome As New ArrayList()
                Dim arrayColCodigo As New ArrayList()
                While dr.Read()
                    arrayColNome.Add(dr("colNome"))
                    arrayColCodigo.Add(dr("colCodigo"))
                End While

                ' Depois de coletado os nomes e códigos, verifico o apontamento de cada colaborador se tem alguma hora apontada
                ' e se existe alguma descrição registrada pelo mesmo.
                For i = 0 To (arrayColNome.Count - 1)

                    If arrayColCodigo(i) IsNot Nothing And arrayColNome(i) IsNot Nothing Then

                        SQL = "SELECT * FROM v_relatorioApontamento " & _
                              "WHERE proCodigo = " & proCodigo & " AND colCodigo = " & arrayColCodigo(i) & " AND " & _
                              "apoData between '" & dataInicio & "' AND '" & dataFim & "'"

                        If Not selectSQL(SQL) Then
                            lblTeste.Text = sqlErro
                            Return
                        End If

                        Dim existeApontamento As Boolean = False

                        If dr.HasRows Then
                            While dr.Read()
                                ' Verifico se existe algum apontamento realizado no periodo selecionado
                                If dr("apoEntrada") IsNot DBNull.Value Or dr("apoEntrada").ToString.Trim <> "" And _
                                   dr("apoEntAlmoco") IsNot DBNull.Value Or dr("apoEntAlmoco").ToString.Trim <> "" And _
                                   dr("apoSaiAlmoco") IsNot DBNull.Value Or dr("apoSaiAlmoco").ToString.Trim <> "" And _
                                   dr("apoSaida") IsNot DBNull.Value Or dr("apoSaida").ToString.Trim <> "" And _
                                   dr("apoDescricao") IsNot DBNull.Value Or dr("apoDescricao").ToString.Trim <> "" _
                                   Then

                                    existeApontamento = True

                                End If
                            End While
                        End If

                        If Not existeApontamento Then
                            arrayColCodigo(i) = "Vazio"
                            arrayColNome(i) = "Vazio"
                            existeApontamento = False
                        End If

                    End If

                Next i

                Dim arrayStatusAprovacao As New ArrayList
                Dim GP As String = ""
                Dim GC As String = ""
                Dim Dir As String = ""

                ' Preenche o dataTable com os status de aprovação dos consultores
                For i = 0 To (arrayColNome.Count - 1)

                    If arrayColCodigo(i).ToString <> "Vazio" Then

                        arrayStatusAprovacao = getStatusAprovacao(arrayColCodigo(i), ddlProjeto.SelectedValue, dataInicio, dataFim)

                        GP = arrayStatusAprovacao(0)
                        GC = arrayStatusAprovacao(1)
                        Dir = arrayStatusAprovacao(2)

                        Select Case GP
                            Case "A"
                                GP = "<b style='color:Green;'  >A</b>"
                            Case "R"
                                GP = "<b style='color:Red;'    >R</b>"
                            Case "N"
                                GP = "<b style='color:Gray;'   >N</b>"
                            Case "P"
                                GP = "<b style='color:#E8C500;'>P</b>"
                        End Select

                        Select Case GC
                            Case "A"
                                GC = "<b style='color:Green;'  >A</b>"
                            Case "R"
                                GC = "<b style='color:Red;'    >R</b>"
                            Case "N"
                                GC = "<b style='color:Gray;'   >N</b>"
                            Case "P"
                                GC = "<b style='color:#E8C500;'>P</b>"
                        End Select

                        Select Case Dir
                            Case "A"
                                Dir = "<b style='color:Green;'  >A</b>"
                            Case "R"
                                Dir = "<b style='color:Red;'    >R</b>"
                            Case "N"
                                Dir = "<b style='color:Gray;'   >N</b>"
                            Case "P"
                                Dir = "<b style='color:#E8C500;'>P</b>"
                        End Select

                        dtAprovacao.Rows.Add(New Object() {arrayColNome(i), GP, GC, Dir, arrayColCodigo(i)})

                    End If

                Next i

                divPaginas.Visible = True

                ' Acionando a DataTable ao statusAprovacao
                statusAprovacao.DataSource = dtAprovacao
                statusAprovacao.DataBind()

                ' Guarda o dataTable numa sessão para ser usado depois na páginação
                Session("dtAprovacao") = dtAprovacao

            End If

        Else
            lblTeste.Text = sqlErro
        End If

    End Sub

    Private Function getStatusAprovacao(ByVal colCodigo As String, ByVal proCodigo As String, ByVal dataInicio As DateTime, ByVal dataFim As DateTime) As ArrayList

        Dim array As New ArrayList
        Dim linha As DataRow
        Dim aprovacaoGP As String = ""
        Dim aprovacaoGC As String = ""
        Dim aprovacaoDir As String = ""

        SQL = "SELECT codGP, codGC, codDir FROM v_projetos WHERE proCodigo = " & proCodigo

        If selectSQL(SQL) Then
            dr.Read()
            If dr.HasRows Then
                If dr("codGP") IsNot DBNull.Value Then
                    aprovacaoGP = dr("codGP")
                Else
                    aprovacaoGP = "N"
                End If
                If dr("codGC") IsNot DBNull.Value Then
                    aprovacaoGC = dr("codGC")
                Else
                    aprovacaoGC = "N"
                End If
                If dr("codDir") IsNot DBNull.Value Then
                    aprovacaoDir = dr("codDir")
                Else
                    aprovacaoDir = "N"
                End If
            End If
        End If

        SQL = "SELECT apoAprovacaoGP, apoAprovacaoGC, apoAprovacaoDir FROM v_apontamento WHERE colCodigo = " & colCodigo & " " & _
              "AND proCodigo = " & proCodigo & " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

        ' Verifica se em todas as colunas de aprovação esta como A de aprovado, R de reprovado, ou vazia que 
        ' significa que ainda não houve aprovação/reprovação
        If selectSQL(SQL) Then

            Dim dtStatusAprovacao As New DataTable("tblStatusAprovacao")

            dtStatusAprovacao.Columns.Add("aprovacaoGP", GetType(String))
            dtStatusAprovacao.Columns.Add("aprovacaoGC", GetType(String))
            dtStatusAprovacao.Columns.Add("aprovacaoDir", GetType(String))

            Dim numLinhas = 0
            While dr.Read
                dtStatusAprovacao.Rows.Add(New Object() {dr("apoAprovacaoGP"), dr("apoAprovacaoGC"), dr("apoAprovacaoDir")})
                numLinhas += 1
            End While

            Dim qtdA = 0
            Dim qtdR = 0
            Dim qtdPendencia = 0
            Dim qtd = 0

            If aprovacaoGP <> "N" Then

                For i = 0 To numLinhas - 1
                    linha = dtStatusAprovacao.Rows(i)
                    Select Case linha("aprovacaoGP").ToString.Trim
                        Case "A"
                            qtdA += 1
                        Case "R"
                            qtdR += 1
                        Case "P"
                            qtdPendencia += 1
                    End Select
                    qtd += 1
                Next i

                If qtd = qtdA Then
                    aprovacaoGP = "A"
                End If
                If qtd = qtdR Then
                    aprovacaoGP = "R"
                End If
                If qtd <> qtdA And qtd <> qtdR Then
                    aprovacaoGP = "P"
                End If

            End If

            ' Adiciona ao array no primeiro indice o Status de Aprovação do Gerente de projetos
            array.Add(aprovacaoGP)

            ' Resetando variaveis
            qtdA = 0
            qtdR = 0
            qtdPendencia = 0
            qtd = 0

            If aprovacaoGC <> "N" Then

                For i = 0 To numLinhas - 1
                    linha = dtStatusAprovacao.Rows(i)
                    Select Case linha("aprovacaoGC").ToString.Trim
                        Case "A"
                            qtdA += 1
                        Case "R"
                            qtdR += 1
                        Case "P"
                            qtdPendencia += 1
                    End Select
                    qtd += 1
                Next i

                If qtd = qtdA Then
                    aprovacaoGC = "A"
                End If
                If qtd = qtdR Then
                    aprovacaoGC = "R"
                End If
                If qtd <> qtdA And qtd <> qtdR Then
                    aprovacaoGC = "P"
                End If

            End If

            ' Adiciona ao array no segundo indice o Status de Aprovação do Gerente de contas
            array.Add(aprovacaoGC)

            ' Resetando variaveis
            qtdA = 0
            qtdR = 0
            qtdPendencia = 0
            qtd = 0

            If aprovacaoDir <> "N" Then

                For i = 0 To numLinhas - 1
                    linha = dtStatusAprovacao.Rows(i)
                    Select Case linha("aprovacaoDir").ToString.Trim
                        Case "A"
                            qtdA += 1
                        Case "R"
                            qtdR += 1
                        Case "P"
                            qtdPendencia += 1
                    End Select
                    qtd += 1
                Next i

                If qtd = qtdA Then
                    aprovacaoDir = "A"
                End If
                If qtd = qtdR Then
                    aprovacaoDir = "R"
                End If
                If qtd <> qtdA And qtd <> qtdR Then
                    aprovacaoDir = "P"
                End If

            End If

            ' Adiciona ao array no terceiro indice o Status de Aprovação da Diretoria
            array.Add(aprovacaoDir)

        Else
            ' Exibe o erro causado ao se conectar no banco de dados
            lblTeste.Text = sqlErro
        End If

        ' Retorna o array com os três status de aporvação do consultor selecionado
        Return array

    End Function

    Private Sub statusAprovacao_PagePropertiesChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.PagePropertiesChangingEventArgs) Handles statusAprovacao.PagePropertiesChanging

        paginas.SetPageProperties(e.StartRowIndex, e.MaximumRows, False)

        If Not Session("dtAprovacao") Is Nothing Then

            Dim dtAprovacao As DataTable = CType(Session("dtAprovacao"), DataTable)

            statusAprovacao.DataSource = dtAprovacao
            statusAprovacao.DataBind()

        End If

    End Sub

    Private Sub statusAprovacao_SelectedIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ListViewSelectEventArgs) Handles statusAprovacao.SelectedIndexChanging

        Dim colCodigo = statusAprovacao.DataKeys.Item(e.NewSelectedIndex).Values.Item("colCodigo").ToString
        Dim proCodigo = ddlProjeto.SelectedValue
        Dim linhaIndex As Integer = e.NewSelectedIndex

        dataInicio = Session("dataInicio")
        dataFim = dataInicio.AddMonths(1).AddDays(-1)

        lblMensagem.Text = ""

        txtColCodigo.Value = colCodigo
        txtProCodigo.Value = proCodigo

#If Not Debug Then
        lblResponsavel.Text = ""
        lblCliente.Text = ""
        lblColNome.Text = ""
        lblColModulo.Text = ""
        lblColNivel.Text = ""
        lblMesReferencia.Text = ""
        lblProjeto.Text = ""
#End If

        Dim GP = statusAprovacao.DataKeys.Item(linhaIndex).Values.Item("GP").ToString
        Dim GC = statusAprovacao.DataKeys.Item(linhaIndex).Values.Item("GC").ToString
        Dim Dir = statusAprovacao.DataKeys.Item(linhaIndex).Values.Item("Dir").ToString

        ' Só debugando pra entender  ''''''''''''''''
        If GP.Length > 1 Then
            GP = GP.Substring(26, 1)
        End If

        If GC.Length > 1 Then
            GC = GC.Substring(26, 1)
        End If

        If Dir.Length > 1 Then
            Dir = Dir.Substring(26, 1)
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''

        lblTeste.Text = ""

        ' Situações onde habilita ou não os botões para aprovação
        Select Case Session("perCodigoLogado")
            Case 2 ' "Gerente de Projeto"
                ' Se o Gerente de Contas tiver aprovado então não se pode mais aprovar/reprovar, a não ser que,
                ' o Gerente de contas coloque como Reprovado.

                If GC <> "N" Then
                    If GC = "A" Then
                        ' Não permite aprovar/reprovar
                        btnAprovar.Enabled = False
                        btnReprovar.Enabled = False
                        btnAprovar.Style.Add("background-color", "Gray")
                        btnReprovar.Style.Add("background-color", "Gray")
                        lblTeste.Text = "Você não pode aprovar/reprovar devido o gerente de contas já " & _
                                    "ter aprovado este apontamento.  <br />"
                    Else
                        ' Permite aprovar/reprovar
                        btnAprovar.Enabled = True
                        btnReprovar.Enabled = True
                        btnAprovar.Style.Add("background-color", "Green")
                        btnReprovar.Style.Add("background-color", "Red")
                    End If
                ElseIf Dir <> "N" Then
                    If Dir = "A" Then
                        ' Não permite aprovar/reprovar
                        btnAprovar.Enabled = False
                        btnReprovar.Enabled = False
                        btnAprovar.Style.Add("background-color", "Gray")
                        btnReprovar.Style.Add("background-color", "Gray")
                        lblTeste.Text = "Você não pode aprovar/reprovar devido o diretor(a) já " & _
                                    "ter aprovado este apontamento.  <br />"
                    Else
                        ' Permite aprovar/reprovar
                        btnAprovar.Enabled = True
                        btnReprovar.Enabled = True
                        btnAprovar.Style.Add("background-color", "Green")
                        btnReprovar.Style.Add("background-color", "Red")
                    End If
                Else
                    ' Permite aprovar/reprovar
                    btnAprovar.Enabled = True
                    btnReprovar.Enabled = True
                    btnAprovar.Style.Add("background-color", "Green")
                    btnReprovar.Style.Add("background-color", "Red")
                End If

            Case 3 ' "Gerente de Contas"
                If GP <> "N" Then
                    If GP <> "A" Then
                        ' Não permite aprovar/reprovar
                        btnAprovar.Enabled = False
                        btnReprovar.Enabled = False
                        btnAprovar.Style.Add("background-color", "Gray")
                        btnReprovar.Style.Add("background-color", "Gray")
                        lblTeste.Text = "Você não pode aprovar/reprovar devido o gerente de projeto não " & _
                                    "ter aprovado este apontamento.  <br />"
                    Else
                        ' Permite aprovar/reprovar
                        btnAprovar.Enabled = True
                        btnReprovar.Enabled = True
                        btnAprovar.Style.Add("background-color", "Green")
                        btnReprovar.Style.Add("background-color", "Red")
                    End If
                ElseIf Dir <> "N" Then
                    If Dir = "A" Then
                        ' Não permite aprovar/reprovar
                        btnAprovar.Enabled = False
                        btnReprovar.Enabled = False
                        btnAprovar.Style.Add("background-color", "Gray")
                        btnReprovar.Style.Add("background-color", "Gray")
                        lblTeste.Text = "Você não pode aprovar/reprovar devido o diretor(a) já " & _
                                    "ter aprovado este apontamento.  <br />"
                    Else
                        ' Permite aprovar/reprovar
                        btnAprovar.Enabled = True
                        btnReprovar.Enabled = True
                        btnAprovar.Style.Add("background-color", "Green")
                        btnReprovar.Style.Add("background-color", "Red")
                    End If
                Else
                    ' Permite aprovar/reprovar
                    btnAprovar.Enabled = True
                    btnReprovar.Enabled = True
                    btnAprovar.Style.Add("background-color", "Green")
                    btnReprovar.Style.Add("background-color", "Red")
                End If

            Case 4 ' "Diretoria"
                If GC <> "N" Then
                    If GC <> "A" Then
                        ' Não permite aprovar/reprovar
                        btnAprovar.Enabled = False
                        btnReprovar.Enabled = False
                        btnAprovar.Style.Add("background-color", "Gray")
                        btnReprovar.Style.Add("background-color", "Gray")
                        lblTeste.Text = "Você não pode aprovar/reprovar devido o gerente de contas não " & _
                                    "ter aprovado este apontamento.  <br />"
                    Else
                        ' Permite aprovar/reprovar
                        btnAprovar.Enabled = True
                        btnReprovar.Enabled = True
                        btnAprovar.Style.Add("background-color", "Green")
                        btnReprovar.Style.Add("background-color", "Red")
                    End If
                ElseIf GP <> "N" Then
                    If GP <> "A" Then
                        ' Não permite aprovar/reprovar
                        btnAprovar.Enabled = False
                        btnReprovar.Enabled = False
                        btnAprovar.Style.Add("background-color", "Gray")
                        btnReprovar.Style.Add("background-color", "Gray")
                        lblTeste.Text = "Você não pode aprovar/reprovar devido o gerente de projeto não " & _
                                    "ter aprovado este apontamento.  <br />"
                    Else
                        ' Permite aprovar/reprovar
                        btnAprovar.Enabled = True
                        btnReprovar.Enabled = True
                        btnAprovar.Style.Add("background-color", "Green")
                        btnReprovar.Style.Add("background-color", "Red")
                    End If
                Else
                    ' Permite aprovar/reprovar
                    btnAprovar.Enabled = True
                    btnReprovar.Enabled = True
                    btnAprovar.Style.Add("background-color", "Green")
                    btnReprovar.Style.Add("background-color", "Red")
                End If

        End Select

        ' Verifica se o periodo de aprovação esta afpAberto para aprovações
        SQL = "SELECT aberto FROM v_periodoAprovacao WHERE competencia = " & ddlMes.SelectedValue & " AND " & _
              "ano = " & ddlAno.SelectedValue
        If selectSQL(SQL) Then
            dr.Read()
            If dr("aberto") = 0 Then
                btnAprovar.Enabled = False
                btnReprovar.Enabled = False
                btnAprovar.Style.Add("background-color", "Gray")
                btnReprovar.Style.Add("background-color", "Gray")
                lblMensagem.Text = "A competência de " & ddlMes.SelectedItem.Text & " esta fechado para aprovação."
            End If
        Else
            btnAprovar.Enabled = False
            btnReprovar.Enabled = False
            btnAprovar.Style.Add("background-color", "Gray")
            btnReprovar.Style.Add("background-color", "Gray")
            lblMensagem.Text = sqlErro
            Return
        End If

        Session("linhaIndex") = linhaIndex
        dataInicio = txtDataInicio.Value
        preencherTabelaApontamento(colCodigo, proCodigo, dataInicio)

    End Sub

    Private Sub btnAprovar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAprovar.Click

        Dim colCodigo = txtColCodigo.Value
        Dim proCodigo = txtProCodigo.Value
        Dim dataInicio As DateTime = txtDataInicio.Value
        Dim dataFim As DateTime = txtDataFim.Value
        Dim linhaIndex As Integer = Session("linhaIndex")

        Dim GP = statusAprovacao.DataKeys.Item(linhaIndex).Values.Item("GP").ToString
        Dim GC = statusAprovacao.DataKeys.Item(linhaIndex).Values.Item("GC").ToString
        Dim Dir = statusAprovacao.DataKeys.Item(linhaIndex).Values.Item("Dir").ToString

        GP = GP.Substring(26, 1)
        GC = GC.Substring(26, 1)
        Dir = Dir.Substring(26, 1)

        Select Case Session("perCodigoLogado").ToString
            Case 2 '"Gerente de Projeto"

                SQL = "UPDATE tblApontamento SET" & _
                    "  apoAprovacaoGP = 'A'" & _
                    " ,apoAprovacaoGC = '" & GC & "'" & _
                    " ,apoAprovacaoDir = '" & Dir & "'" & _
                    " ,apoAprovadorGP = " & Session("colCodigoLogado") & "" & _
                    " WHERE colCodigo = '" & colCodigo & "'" & _
                    " AND proCodigo = " & proCodigo & "" & _
                    " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'; "

            Case 3 '"Gerente de Contas"

                SQL = "UPDATE tblApontamento SET" & _
                    " apoAprovacaoGP = '" & GP & "'" & _
                    " ,apoAprovacaoGC = 'A'" & _
                    " ,apoAprovacaoDir = '" & Dir & "'" & _
                    " ,apoAprovadorGC = " & Session("colCodigoLogado") & "" & _
                    " WHERE colCodigo = '" & colCodigo & "'" & _
                    " AND proCodigo = " & proCodigo & "" & _
                    " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

            Case 4 '"Diretoria"

                SQL = "UPDATE tblApontamento SET" & _
                    " apoAprovacaoGP = '" & GP & "'" & _
                    " ,apoAprovacaoGC = '" & GC & "'" & _
                    " ,apoAprovacaoDir = 'A'" & _
                    " ,apoAprovadorDir = " & Session("colCodigoLogado") & "" & _
                    " WHERE colCodigo = '" & colCodigo & "'" & _
                    " AND proCodigo = " & proCodigo & "" & _
                    " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

        End Select

        If SQL = Nothing Then
            lblMensagem.Text = "Erro: Variavel SQL está vazia."
            Return
        End If

        If comandoSQL(SQL) Then
            divCalendario.Style.Add("display", "none")
            ddlMes_SelectedIndexChanged(sender, e)
        Else
            divCalendario.Style.Add("display", "none")
            lblTeste.Text = sqlErro
        End If
        
    End Sub

    Private Sub btnReprovar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReprovar.Click

        Dim colCodigo = txtColCodigo.Value
        Dim proCodigo = txtProCodigo.Value
        Dim dataInicio As DateTime = txtDataInicio.Value
        Dim dataFim As DateTime = txtDataFim.Value

        Select Case Session("colPerfilLogado")
            Case "Gerente de Projeto"

                SQL = "UPDATE tblApontamento SET" & _
                    " apoAprovacaoGP = 'R'," & _
                    " apoAprovadorGP = NULL" & _
                    " WHERE colCodigo = '" & colCodigo & "'" & _
                    " AND proCodigo = " & proCodigo & "" & _
                    " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

            Case "Gerente de Contas"

                SQL = "UPDATE tblApontamento SET" & _
                    " apoAprovacaoGC = 'R'," & _
                    " apoAprovadorGC = NULL" & _
                    " WHERE colCodigo = '" & colCodigo & "'" & _
                    " AND proCodigo = " & proCodigo & "" & _
                    " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

            Case "Diretoria"

                SQL = "UPDATE tblApontamento SET" & _
                    " apoAprovacaoDir = 'R'," & _
                    " apoAprovadorDir = NULL" & _
                    " WHERE colCodigo = '" & colCodigo & "'" & _
                    " AND proCodigo = " & proCodigo & "" & _
                    " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

        End Select

        If SQL = Nothing Then
            lblMensagem.Text = "Erro: Variavel SQL está vazia."
            Return
        End If

        If comandoSQL(SQL) Then
            divCalendario.Style.Add("display", "none")
            ddlMes_SelectedIndexChanged(sender, e)
        Else
            divCalendario.Style.Add("display", "none")
            lblTeste.Text = sqlErro
        End If

    End Sub

    '========================================================================================================================
    '   Preenche a tabela de Apontamento necessitando em session os valores Data Inicio e Data Fim
    '========================================================================================================================
    Private Sub preencherTabelaApontamento(ByVal colCodigo As String, ByVal proCodigo As String, ByVal dataInicio As DateTime)

        limpar()

        ' Preparando as variaveis de datas
        Dim varData As DateTime = dataInicio
        dataFim = dataInicio.AddMonths(1).AddDays(-1)

        ' Verifica se o projeto tem GC associado para exbição no campo responsavel, se caso não existir, exibe o Diretor.
        SQL = "IF EXISTS(SELECT codGC FROM v_projetos WHERE proCodigo = " & proCodigo & " AND codGC IS NOT NULL ) " & _
              "BEGIN " & _
              "  SELECT colNome FROM v_colaboradores WHERE colCodigo = (SELECT codGC FROM v_projetos WHERE proCodigo = " & proCodigo & ") " & _
              "END " & _
              "ELSE BEGIN " & _
              "  SELECT colNome FROM v_colaboradores WHERE colCodigo = (SELECT codDir FROM v_projetos WHERE proCodigo = " & proCodigo & ") " & _
              "END"

        If Not selectSQL(SQL) Then
            lblMensagem.Text = sqlErro
            Return
        Else
            dr.Read()
            lblResponsavel.Text = dr("colNome")
        End If
        '==================================================================================================================

        SQL = "SELECT * FROM v_relatorioApontamento WHERE " & _
              "colCodigo = '" & colCodigo & "' and proCodigo = " & proCodigo & " and " & _
              "apoData BETWEEN '" & dataInicio & "' and '" & dataFim & "'"

        ' Preenchendo o Label Nome do Colaborador
        If selectSQL(SQL) Then
            dr.Read()
            ' Se DataReader existe linhas
            If dr.HasRows = True Then
                ' Exibe a tabela de apontamento
                divCalendario.Style.Add("display", "block")
            Else
                lblTeste.Style.Add("color", "red")
                lblTeste.Text = "No periodo selecionado não há apontamentos registrados."
                ' Esconde a tabela de apontamento
                divCalendario.Style.Add("display", "none")
                Return
            End If
        Else
            lblTeste.Text = sqlErro
        End If

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

        Dim controle As Control = Me.formulario
        Dim entrada As String = ""
        Dim entAlmoco As String = ""
        Dim saiAlmoco As String = ""
        Dim saida As String = ""
        Dim horaNormal As String
        Dim horaExtra As String
        Dim horaTotal As String
        Dim somaNormais As Double = 0
        Dim somaExtras As Double = 0
        Dim somaTotal As Double = 0


        ' Restaura novamente a variavel usada anteriormente com a data de inicio
        varData = dataInicio

        ' Loop para pintar as linhas que pertence a dias de feriados
        For i = 1 To 31
            If Not isDiaUtil(varData) Then
                ' Se não for dia util, aqui destaco de cinza a linha
                controle = controle.FindControl("linha" & i)
                CType(controle, HtmlTableRow).BgColor = "#E8E8E8"
            Else
                ' Se for dia util, aqui deixo em branco a linha
                controle = controle.FindControl("linha" & i)
                CType(controle, HtmlTableRow).BgColor = "White"
            End If
            varData = varData.AddDays(1)
        Next i

        ' Restaura novamente a variavel usada anteriormente com a data de inicio
        varData = dataInicio

        For i = 1 To DateDiff(DateInterval.Day, dataInicio, dataFim) + 1
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
                    CType(controle, Label).Text = dr("apoEntrada")
                End If

                If dr("apoEntAlmoco") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblEntAlmoco" & i)
                    CType(controle, Label).Text = dr("apoEntAlmoco")
                End If

                If dr("apoSaiAlmoco") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblSaiAlmoco" & i)
                    CType(controle, Label).Text = dr("apoSaiAlmoco")
                End If

                If dr("apoSaida") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblSaida" & i)
                    CType(controle, Label).Text = dr("apoSaida")
                End If

                If dr("apoNormais") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblNormal" & i)
                    horaNormal = dr("apoNormais")
                    CType(controle, Label).Text = dr("apoNormais")
                End If

                If dr("apoExtras") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblExtra" & i)
                    horaExtra = dr("apoExtras")
                    CType(controle, Label).Text = dr("apoExtras")
                End If

                If dr("apoTotal") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblTotal" & i)
                    horaTotal = dr("apoTotal")
                    CType(controle, Label).Text = dr("apoTotal")
                End If

                If dr("apoDescricao") IsNot DBNull.Value Then
                    controle = controle.FindControl("lblAtividades" & i)
                    CType(controle, Label).Text = dr("apoDescricao").ToString
                End If

                'Calculos dos totais de horas normais, extras e total das duas
                somaNormais += somaHorasnew(horaNormal)
                somaExtras += somaHorasnew(horaExtra)
                somaTotal += somaHorasnew(horaTotal)

                varData = varData.AddDays(1)

            Catch ex As Exception
#If DEBUG Then
                'lblErro.Text = "Mensagem de erro interno: " & ex.Message
#End If
            End Try
            dr.Read()
        Next i

        lblTotalNormais.Text = FomatarDataHora(somaNormais)
        lblTotalExtras.Text = FomatarDataHora(somaExtras)
        lblTotalTotal.Text = FomatarDataHora(somaTotal)

    End Sub

    Private Sub limpar()

        Dim controle As Control = Me.formulario

        For i = 1 To 31
            controle = controle.FindControl("lblEntrada" & i)
            CType(controle, Label).Text = ""
            controle = controle.FindControl("lblEntAlmoco" & i)
            CType(controle, Label).Text = ""
            controle = controle.FindControl("lblSaiAlmoco" & i)
            CType(controle, Label).Text = ""
            controle = controle.FindControl("lblSaida" & i)
            CType(controle, Label).Text = ""
            controle = controle.FindControl("lblNormal" & i)
            CType(controle, Label).Text = ""
            controle = controle.FindControl("lblExtra" & i)
            CType(controle, Label).Text = ""
            controle = controle.FindControl("lblTotal" & i)
            CType(controle, Label).Text = ""
            controle = controle.FindControl("lblAtividades" & i)
            CType(controle, Label).Text = ""
        Next

        lblColNome.Text = ""
        lblColModulo.Text = ""
        lblColNivel.Text = ""
        lblMesReferencia.Text = ""
        lblPeriodo.Text = ""
        lblCliente.Text = ""
        lblProjeto.Text = ""
        lblResponsavel.Text = ""

    End Sub

End Class