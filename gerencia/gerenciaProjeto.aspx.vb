Imports System.Globalization

Partial Public Class GerenciaProjeto
    Inherits System.Web.UI.Page

    Dim SQL As String
    Dim arrayConsultoresProjeto As New ArrayList
    Dim arrayTodosConsultores As New ArrayList
    Dim proCodigo As String
    Dim colNome As String
    Dim colModulo As String
    Dim colNivel As String
    Dim colCodigo As String
    Dim perCodigo As String
    Dim colPerfil As String
    Dim colProTaxa As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Try
            If Not Session("permissao").ToString.Contains("Gerencia de Projeto") Then
                Response.Redirect("..\Default.aspx")
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

        ' Este if serve para que carregue estes comandos apenas uma vez, mesmo depois de um PostBack
        If Not IsPostBack Then

            ddlProjeto.Focus()
            'atualizarDDLProjeto()

            lblMensagem.Style.Add("font-style", "italic")

            lblMensagem2.Style.Add("font-style", "italic")
            lblMensagem2.Style.Add("color", "Red")

            txtGerenteProjeto.Visible = False
            txtGerenteConta.Visible = False
            txtDiretor.Visible = False

            txtNomeGerenteCliente.Visible = False
            txtEmailGerenteCliente.Visible = False

            ddlProjeto.Items.Add("")
            btnExcluir.Enabled = False

            '   Carrega a comboBox com todos os projetos ativos de acordo com o perfil logado
            Select Case Session("perCodigoLogado")
                Case 1 ' Consultores
                Case 2 ' Gerente de Projeto
                    SQL = "SELECT proCodigo, proNome FROM v_projetos WHERE UPPER(proStatus) = 'ATIVO' AND codGP = " & Session("colCodigoLogado")
                Case 3 ' Gerente de contas
                    SQL = "SELECT proCodigo, proNome FROM v_projetos WHERE UPPER(proStatus) = 'ATIVO' AND codGC = " & Session("colCodigoLogado")
                Case 4 ' Diretoria
                    SQL = "SELECT proCodigo, proNome FROM v_projetos WHERE UPPER(proStatus) = 'ATIVO'"
                Case 5 ' CSC
                    SQL = "SELECT proCodigo, proNome FROM v_projetos WHERE UPPER(proStatus) = 'ATIVO'"
                Case 6 ' Recrutamento
                Case 7 ' Adminitrador
                    SQL = "SELECT proCodigo, proNome FROM v_projetos WHERE UPPER(proStatus) = 'ATIVO'"
            End Select
            If SQL <> "" Then
                SQL += " ORDER BY proNome"
                If selectSQL(SQL) Then
                    While dr.Read()
                        ddlProjeto.Items.Add(New ListItem(dr("proNome"), dr("proCodigo")))
                    End While
                End If
            End If

            ' Carrega a comboBox "Gerente de Projetos"
            ddlGerenteProjeto.Items.Add("Não possue")
            If selectSQL("SELECT colCodigo, colNome FROM v_colaboradores WHERE perCodigo = 2 and upper(colStatus)='ATIVO'  ORDER BY colNome") Then
                While dr.Read()
                    ddlGerenteProjeto.Items.Add(New ListItem(dr("colNome"), dr("colCodigo")))
                End While
            Else
                lblMensagem.Text = sqlErro
            End If

            ' Carrega a comboBox "Gerente de Contas"
            ddlGerenteConta.Items.Add("Não possue")
            If selectSQL("SELECT colCodigo, colNome FROM v_colaboradores WHERE perCodigo = 3 and upper(colStatus)='ATIVO' ORDER BY colNome") Then
                While dr.Read()
                    ddlGerenteConta.Items.Add(New ListItem(dr("colNome"), dr("colCodigo")))
                End While
            Else
                lblMensagem.Text = sqlErro
            End If

            ' Carrega a comboBox "Diretor"
            ddlDiretor.Items.Add("Não possue")
            If selectSQL("SELECT colCodigo, colNome FROM v_colaboradores WHERE perCodigo = 4 and upper(colStatus)='ATIVO' ORDER BY colNome") Then
                While dr.Read()
                    ddlDiretor.Items.Add(New ListItem(dr("colNome"), dr("colCodigo")))
                End While
            Else
                lblMensagem.Text = sqlErro
            End If

            'Preenche o combo de calendario
            PreencherCalendSalvos(ddlCalendario)

            ' Quando carregado a página desabilita todos os itens do formulários menos o comboBox Projetos
            habilitaDesabilitaForm()

        End If

    End Sub

    Protected Sub gridTodosColaboradores_PreRender(sender As Object, e As EventArgs) Handles gridTodosColaboradores.PreRender
        Dim gv As GridView = DirectCast(sender, GridView)
        For Each row As GridViewRow In gv.Rows
            row.Attributes.Add("onclick", "javascript:Selecionar(this,'#FCE2B1');") '#FCE2B1 = Laranjado
        Next
    End Sub

    Protected Sub gridColaboradoresNoProjeto_PreRender(sender As Object, e As EventArgs) Handles gridColaboradoresNoProjeto.PreRender
        For Each row As GridViewRow In CType(sender, GridView).Rows
            ' CType(row.FindControl("txtTaxaProjeto"), HtmlGenericControl).Attributes.Add("onclick", "javascript:Sel(this,'#FCE2B1');")
            'row.Cells(3).Attributes.Add("onclick", "javascript:Selecionar(" & row.ClientID & ",'#FCE2B1');") '#FCE2B1 = Laranjado
            row.Attributes.Add("onclick", "javascript:Selecionar(this,'#FCE2B1');") '#FCE2B1 = Laranjado
        Next
    End Sub

    Private Sub btnAdicionarCol_Click(sender As Object, e As System.EventArgs) Handles btnAdicionarCol.Click

        For Each row As GridViewRow In gridTodosColaboradores.Rows

            If CType(row.FindControl("ckbSel"), CheckBox).Checked Then
                Dim proCodigo = CType(row.Cells(1).FindControl("lblProCodigo"), Label).Text
                Dim colCodigo = CType(row.Cells(2).FindControl("lblColCodigo"), Label).Text
                Dim nome = CType(row.Cells(3).FindControl("lblColNome"), Label).Text

                CType(ViewState("gridColaboradoresProjeto"), DataTable).Rows.Add(New Object() {proCodigo, colCodigo, nome})
                CType(ViewState("gridTodosColaboradores"), DataTable).Rows.RemoveAt(FindRow(CType(ViewState("gridTodosColaboradores"), DataTable), colCodigo))


            End If

        Next

        'coloca em ordem alfabetica
        Dim DataTable As DataTable = CType(ViewState("gridTodosColaboradores"), DataTable)
        DataTable.DefaultView.Sort = "colNome asc"
        ViewState("gridTodosColaboradores") = DataTable.DefaultView.ToTable

        gridTodosColaboradores.DataSource = ViewState("gridTodosColaboradores")
        gridTodosColaboradores.DataBind()

        DataTable = CType(ViewState("gridColaboradoresProjeto"), DataTable)
        DataTable.DefaultView.Sort = "colNome asc"
        ViewState("gridColaboradoresProjeto") = DataTable.DefaultView.ToTable

        gridColaboradoresNoProjeto.DataSource = ViewState("gridColaboradoresProjeto")
        gridColaboradoresNoProjeto.DataBind()

    End Sub

    Private Sub btnRemoverCol_Click(sender As Object, e As System.EventArgs) Handles btnRemoverCol.Click

        For Each row As GridViewRow In gridColaboradoresNoProjeto.Rows

            If CType(row.FindControl("ckbSel"), CheckBox).Checked Then
                Dim proCodigo = CType(row.Cells(1).FindControl("lblProCodigo"), Label).Text
                Dim colCodigo = CType(row.Cells(2).FindControl("lblColCodigo"), Label).Text
                Dim nome = CType(row.Cells(3).FindControl("lblColNome"), Label).Text
                CType(ViewState("gridColaboradoresProjeto"), DataTable).Rows.RemoveAt(FindRow(CType(ViewState("gridColaboradoresProjeto"), DataTable), colCodigo))
                CType(ViewState("gridTodosColaboradores"), DataTable).Rows.Add(New Object() {proCodigo, colCodigo, nome})
            End If

        Next

        'coloca em ordem alfabetica
        Dim DataTable As DataTable = CType(ViewState("gridTodosColaboradores"), DataTable)
        DataTable.DefaultView.Sort = "colNome asc"
        ViewState("gridTodosColaboradores") = DataTable.DefaultView.ToTable

        gridTodosColaboradores.DataSource = ViewState("gridTodosColaboradores")
        gridTodosColaboradores.DataBind()

        DataTable.Dispose()


        DataTable = CType(ViewState("gridColaboradoresProjeto"), DataTable)
        DataTable.DefaultView.Sort = "colNome asc"
        ViewState("gridColaboradoresProjeto") = DataTable.DefaultView.ToTable

        gridColaboradoresNoProjeto.DataSource = ViewState("gridColaboradoresProjeto")
        gridColaboradoresNoProjeto.DataBind()

    End Sub

    Function FindRow(ByVal dt As DataTable, ByVal colCodigo As String) As Integer
        For i As Integer = 0 To dt.Rows.Count
            If dt.Rows(i)("colCodigo") = colCodigo Then
                Return i
            End If
        Next
        Return -1
    End Function

    Private Sub preencheFormulario(ByVal proCodigo As Integer)

        limparFormulario()
        lblMensagem.Text = ""

        SQL = "SELECT * FROM v_projetos WHERE proCodigo = " & proCodigo

        If Not selectSQL(SQL) Then
            lblMensagem.Text = sqlErro
            Return
        End If

        ' Preenche os comboBox GP, GC e Diretoria
        dr.Read()

        If dr("proNomeGerenteCliente") IsNot DBNull.Value Then
            lblNomeGerenteCliente.Text = dr("proNomeGerenteCliente")
        Else
            lblNomeGerenteCliente.Text = ""
        End If
        If dr("proEmailGerenteCliente") IsNot DBNull.Value Then
            lblEmailGerenteCliente.Text = dr("proEmailGerenteCliente")
        Else
            lblEmailGerenteCliente.Text = ""
        End If
        If dr("proDataInicio") IsNot DBNull.Value Then
            txtDataInicio.Text = dr("proDataInicio")
        Else
            txtDataInicio.Text = ""
        End If
        If dr("proDataFim") IsNot DBNull.Value Then
            txtDataFim.Text = dr("proDataFim")
        Else
            txtDataFim.Text = ""
        End If

        If dr("proNomeCalendario") IsNot DBNull.Value And dr("proAnoCalendario") IsNot DBNull.Value Then
            ddlCalendario.SelectedValue = dr("proNomeCalendario") & "|" & dr("proAnoCalendario")
            ddlCalendario.Enabled = False
        End If

        ddlStatus.SelectedValue = dr("proStatus")

        If dr("codGP") IsNot DBNull.Value Then
            ddlGerenteProjeto.SelectedValue = dr("codGP")
        Else
            ddlGerenteProjeto.SelectedIndex = 0
        End If
        If dr("codGC") IsNot DBNull.Value Then
            ddlGerenteConta.SelectedValue = dr("codGC")
        Else
            ddlGerenteConta.SelectedIndex = 0
        End If
        If dr("codDir") IsNot DBNull.Value Then
            ddlDiretor.SelectedValue = dr("codDir")
        Else
            ddlDiretor.SelectedIndex = 0
        End If

    End Sub

    Private Function preencheGrids(proCodigo As Integer) As Boolean

        ' Carrega gridView "Todos os Colaboradores", colaboradores com perfis
        ' 1 = Consultores
        ' 2 = Gerente de Projeto
        ' 3 = Gerente de Contas
        ' 6 = Recrutamento
        ' 7 = Administrador
        ' 8 = Funcionários

        Dim listaTodosColaboradores As New List(Of String)
        Dim listaColaboradoresProjeto As New List(Of String)

        Dim dtTodosColaboradores As New DataTable("tblTodosColaboradores")
        ' Crio os campos para o dataTable para ser usado no gridView
        dtTodosColaboradores.Columns.Add("proCodigo")
        dtTodosColaboradores.Columns.Add("colCodigo")
        dtTodosColaboradores.Columns.Add("colNome")

        Dim dtColaboradoresProjeto As New DataTable("tblColaboradoresProjeto")
        ' Crio os campos para o dataTable para ser usado no gridView
        dtColaboradoresProjeto.Columns.Add("proCodigo")
        dtColaboradoresProjeto.Columns.Add("colCodigo")
        dtColaboradoresProjeto.Columns.Add("colNome")
        dtColaboradoresProjeto.Columns.Add("colProTaxa")

        SQL = " SELECT colCodigo, colNome, colModulo, colNivel, perCodigo, colPerfil FROM v_colaboradores "
        SQL += "WHERE upper(colStatus) = 'ATIVO' AND perCodigo = 1 OR perCodigo = 2 OR perCodigo = 3 OR perCodigo = 6 OR perCodigo = 7  OR perCodigo = 4"
        SQL += "OR perCodigo = 8 OR perCodigo = 9 ORDER BY colNome"

        '
        ' Cria Lista de todos colaboradores
        '
        If selectSQL(SQL) Then
            ' Preencho uma lista com os códigos dos colaboradores para comparar com o resultado do select de quais colaboradores
            ' estão associados ao projeto selecionado, para que preencha os dois grids corretamente.
            While dr.Read
                listaTodosColaboradores.Add(dr("colCodigo"))
            End While
        Else
            lblMensagem.Text = sqlErro
            Return False
        End If

        '
        ' Cria Lista de todos colaboradores associados ao projeto selecionado 
        '
        SQL = " SELECT * FROM v_colaboradores_projeto WHERE proCodigo = " & proCodigo & " ORDER BY colNome"

        If selectSQL(SQL) Then
            ' Preencho uma lista com os códigos dos colaboradores para comparar com o resultado do select de quais colaboradores
            ' estão associados ao projeto selecionado, para que preencha os dois grids corretamente.
            While dr.Read
                listaColaboradoresProjeto.Add(dr("colCodigo"))
            End While
        Else
            lblMensagem.Text = sqlErro
            Return False
        End If

        '
        ' Apagando da lista listaTodosColaboradores o que tem na lista listaColaboradoresProjeto
        '
        For Each codigo In listaColaboradoresProjeto
            If listaTodosColaboradores.Contains(codigo) Then
                listaTodosColaboradores.Remove(codigo)
            End If
        Next

        '
        ' Agora com as duas listas criadas preenchidas, utilizo-as para preenchimento das 
        ' duas grids (gridTodosColaboradores e gridColaboradoresProjeto)
        '

        ' Preenchendo dtTodosColaboradores
        For Each codigo In listaTodosColaboradores

            SQL = " SELECT colCodigo, colNome, colModulo, colNivel, perCodigo, colPerfil FROM v_colaboradores "
            SQL += "WHERE colCodigo = " & codigo & " and upper(colStatus)='ATIVO'"

            If selectSQL(SQL) Then

                While dr.Read

                    Try
                        colCodigo = dr("colCodigo")
                        colNome = dr("colNome")
                        colModulo = dr("colModulo")
                        colNivel = dr("colNivel")
                        perCodigo = dr("perCodigo")
                        colPerfil = dr("colPerfil")

                    Catch ex As Exception
                        lblMensagem.Text = ex.Message
                        Return False
                    End Try

                    ' Somente exibirá os colaboradores com status 'Ativo' 
                    ' Se for perfil "Consultores"
                    'If perCodigo = 1 Then
                    '    dtTodosColaboradores.Rows.Add(New Object() {proCodigo, colCodigo, colNome & " - " & colModulo & " - " & colNivel})
                    'End If
                    ' Se for perfil "Gerente de Projeto", "Gerente de Contas", "Recrutamento", "Administrador" ou "Funcionário"
                    'If perCodigo = 2 Or perCodigo = 3 Or perCodigo = 6 Or perCodigo = 7 Or perCodigo = 8 Or perCodigo = 4 Then
                    dtTodosColaboradores.Rows.Add(New Object() {proCodigo, colCodigo, colNome & " - " & colPerfil})
                    'End If

                End While

            Else
                lblMensagem.Text = sqlErro
                Return False
            End If

        Next

        ' Preenchendo dtColaboradoresProjeto
        For Each codigo In listaColaboradoresProjeto

            SQL = " SELECT * FROM v_colaboradores_projeto WHERE colCodigo = " & codigo &
                  " AND proCodigo = " & proCodigo

            If selectSQL(SQL) Then

                While dr.Read

                    colProTaxa = ""

                    Try
                        colCodigo = dr("colCodigo")
                        colNome = dr("colNome")
                        colModulo = dr("colModulo")
                        colNivel = dr("colNivel")
                        perCodigo = dr("perCodigo")
                        colPerfil = dr("colPerfil")
                        If Not IsDBNull(dr("colProTaxa")) Then
                            If dr("colProTaxa") > 0D Then
                                colProTaxa = Decimal.Parse(dr("colProTaxa")).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                colProTaxa = colProTaxa.Replace("R$ ", "")
                            End If
                        End If
                    Catch ex As Exception
                        lblMensagem.Text = ex.Message
                        Return False
                    End Try

                    ' Somente exibirá os colaboradores com status 'Ativo' 
                    ' Se for perfil "Consultores"
                    'If perCodigo = 1 Then
                    '    dtColaboradoresProjeto.Rows.Add(New Object() {proCodigo, colCodigo, _
                    '                                     colNome & " - " & colModulo & " - " & colNivel, colProTaxa})
                    'End If
                    ' Se for perfil "Gerente de Projeto", "Gerente de Contas", "Recrutamento", "Administrador" ou "Funcionário"
                    'If perCodigo = 2 Or perCodigo = 3 Or perCodigo = 6 Or perCodigo = 7 Or perCodigo = 8 Or perCodigo = 4 Then
                    dtColaboradoresProjeto.Rows.Add(New Object() {proCodigo, colCodigo, colNome & " - " & colPerfil, colProTaxa})
                    'End If

                End While

            Else
                lblMensagem.Text = sqlErro
                Return False
            End If

        Next

        gridTodosColaboradores.DataSource = dtTodosColaboradores
        gridTodosColaboradores.DataBind()

        gridColaboradoresNoProjeto.DataSource = dtColaboradoresProjeto
        gridColaboradoresNoProjeto.DataBind()

        Try
            CType(ViewState("gridTodosColaboradores"), DataTable).Clear()
            CType(ViewState("gridColaboradoresProjeto"), DataTable).Clear()
        Catch ex As Exception
        End Try

        ViewState("gridTodosColaboradores") = dtTodosColaboradores
        ViewState("gridColaboradoresProjeto") = dtColaboradoresProjeto


        Return True

    End Function

    Private Sub ddlProjeto_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlProjeto.SelectedIndexChanged
        habilitaDesabilitaForm()
    End Sub

    Private Sub habilitaDesabilitaForm()

        If ddlProjeto.SelectedIndex > 0 Then

            ddlGerenteProjeto.Enabled = True
            ddlGerenteConta.Enabled = True
            ddlCalendario.Enabled = True
            ddlDiretor.Enabled = True
            txtDataInicio.Enabled = True
            txtDataFim.Enabled = True
            ddlStatus.Enabled = True
            btnSalvar.Enabled = True
            btnExcluir.Enabled = True
            preencheFormulario(ddlProjeto.SelectedValue)
            preencheGrids(ddlProjeto.SelectedValue)

            ddlProjeto.Focus()

        Else

            txtNomeGerenteCliente.Enabled = False
            txtEmailGerenteCliente.Enabled = False
            ddlGerenteProjeto.Enabled = False
            ddlGerenteConta.Enabled = False
            ddlCalendario.Enabled = False
            ddlDiretor.Enabled = False
            txtDataInicio.Enabled = False
            txtDataFim.Enabled = False
            ddlStatus.Enabled = False
            btnSalvar.Enabled = False
            btnExcluir.Enabled = False
            limparFormulario()
            ddlProjeto.Focus()

        End If

    End Sub

    Private Sub limparFormulario()

        lblNomeGerenteCliente.Text = "_________________________________"
        lblEmailGerenteCliente.Text = "_________________________________"
        ddlGerenteProjeto.SelectedIndex = 0
        ddlGerenteConta.SelectedIndex = 0
        ddlDiretor.SelectedIndex = 0
        txtDataInicio.Text = ""
        txtDataFim.Text = ""
        ddlStatus.SelectedIndex = 0
        ddlCalendario.SelectedValue = ""
        gridTodosColaboradores.DataBind()
        gridColaboradoresNoProjeto.DataBind()

    End Sub
    Private Sub btnExcluir_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExcluir.Click
        Try
            SQL = "select isnull(count(*),0) as existe from tblColaboradores_Projetos clp with(nolock)" & vbCrLf &
                   "inner join tblApontamento tla with(nolock) on (clp.colCodigo = tla.colCodigo)" & vbCrLf &
                   "where clp.proCodigo=" & ddlProjeto.SelectedValue

            If selectSQL(SQL) Then
                If dr.HasRows Then
                    dr.Read()
                    If CInt(dr("existe")) > 0 Then
                        lblMensagem.Text = "Não é possivel excluir os dados devido ja existirem apontamentos."
                        Exit Sub
                    Else
                        SQL = "update proNomeCalendario=null,proAnoCalendario=null where proCodigo=" & ddlProjeto.SelectedValue
                        If comandoSQL(SQL) Then
                            lblMensagem.Text = "Projeto atualizado com sucesso."

                            SQL = "delete tblColaboradores_Projetos where proCodigo=" & ddlProjeto.SelectedValue
                            If comandoSQL(SQL) Then
                                lblMensagem.Text = "Dados excluidos com sucesso."
                            Else
                                lblMensagem.Text = String.Concat("Ocorreu um erro ao excluir os dados erro:", sqlErro)
                            End If
                        Else
                            lblMensagem.Text = String.Concat("Ocorreu um erro ao atualizar o projeto erro:", sqlErro)
                        End If

                    End If
                End If
            Else
                lblMensagem.Text = String.Concat("Ocorreu um erro ao buscar os dados erro:", sqlErro)
            End If
        Catch ex As Exception
            lblMensagem.Text = String.Concat("Ocorreu um erro ao tentar excluir os dados erro:", ex.Message)
        End Try

        limparFormulario()
        ddlProjeto.SelectedIndex = 0
        habilitaDesabilitaForm()

    End Sub

    Private Sub btnSalvar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSalvar.Click

        Dim proCodigo As Integer = ddlProjeto.SelectedValue
        Dim codGP As String
        Dim codGC As String
        Dim codDir As String
        Dim possueGP As String = "N"
        Dim possueGC As String = "N"
        Dim possueDir As String = "N"
        Dim taxa As String = "NULL"
        Dim NomCalendario As String, AnoCalendario As Integer

        ' Validação dos campos Data Inicio e Data término, verifica se um é maior que o outro e se estão inseridos corretamente
        lblMensagem.Text = ""
        lblMensagem2.Text = ""
        txtDataInicio.BackColor = Drawing.Color.White
        txtDataFim.BackColor = Drawing.Color.White
        Dim erroData As Boolean = False

        If String.IsNullOrWhiteSpace(ddlCalendario.SelectedValue) Then
            lblMensagem2.Text = "Selecione um calendário."
            erroData = True
            Return
        End If

        If txtDataInicio.Text.Trim() <> "" Then
            If Not IsDate(txtDataInicio.Text) Then
                lblMensagem2.Text = "Campo 'Data inicio' está com formato incorreto"
                erroData = True
            End If
        Else
            erroData = True
        End If
        If txtDataFim.Text.Trim() <> "" Then
            If Not IsDate(txtDataFim.Text) Then
                lblMensagem2.Text = "Campo 'Data término' está com formato incorreto"
                erroData = True
            End If
        Else
            erroData = True
        End If

        If Not erroData Then
            If DateDiff("d", txtDataInicio.Text, txtDataFim.Text) < 0 Then
                lblMensagem2.Text = "Data validade onde está 'de' está mais recente que 'até'"
                txtDataInicio.BackColor = Drawing.Color.Yellow
                txtDataFim.BackColor = Drawing.Color.Yellow
                Return
            End If
        Else
            txtDataInicio.BackColor = Drawing.Color.Yellow
            txtDataFim.BackColor = Drawing.Color.Yellow
        End If
        ' Fim de verificação das Datas    ----------------------------------------------------------------------------------

        lblMensagem.Style.Add("color", "Gray")
        lblMensagem.Text = "Processando..."

        If ddlGerenteProjeto.SelectedIndex <> 0 Then
            codGP = ddlGerenteProjeto.SelectedValue
            possueGP = "S"
        Else
            codGP = "NULL"
        End If
        If ddlGerenteConta.SelectedIndex <> 0 Then
            codGC = ddlGerenteConta.SelectedValue
            possueGC = "S"
        Else
            codGC = "NULL"
        End If
        If ddlDiretor.SelectedIndex <> 0 Then
            codDir = ddlDiretor.SelectedValue
            possueDir = "S"
        Else
            codDir = "NULL"
        End If

        'retira os valores do combo de calendario
        NomCalendario = ddlCalendario.SelectedValue.Split("|")(0)
        AnoCalendario = Integer.Parse(ddlCalendario.SelectedValue.Split("|")(1))

        ' Primeiro atualizo a tabela de projetos 
        SQL = " UPDATE tblProjetos SET "
        SQL += "proDataInicio = '" & txtDataInicio.Text & "', "
        SQL += "proDataFim = '" & txtDataFim.Text & "', "
        SQL += "possueGP = '" & possueGP & "', "
        SQL += "possueGC = '" & possueGC & "', "
        SQL += "possueDir = '" & possueDir & "', "
        SQL += "codGP = " & codGP & ", "
        SQL += "codGC = " & codGC & ", "
        SQL += "codDir = " & codDir & ", "
        SQL += "proNomeCalendario = '" & NomCalendario & "', "
        SQL += "proAnoCalendario = '" & AnoCalendario & "', "
        SQL += "proStatus = '" & ddlStatus.SelectedValue() & "' "
        SQL += "WHERE proCodigo = " & proCodigo

        If comandoSQL(SQL) Then

            ' Apago todos os colaboradores envolvidos no projeto para em seguida inserir novamente 
            ' com o que foi selecionado
            SQL = "DELETE FROM tblColaboradores_Projetos WHERE proCodigo = " & proCodigo
            If Not comandoSQL(SQL) Then
                lblMensagem.Style.Add("color", "Red")
                lblMensagem.Text = sqlErro
                Return
            End If

            ' Agora gravamos o que foi selecionado em "Consultores no Projeto" se houver algo lá
            For Each row As GridViewRow In gridColaboradoresNoProjeto.Rows

                colCodigo = CType(row.Cells(2).FindControl("lblColCodigo"), Label).Text
                taxa = CType(row.Cells(2).FindControl("txtTaxaProjeto"), TextBox).Text

                If taxa.Trim = "" Then
                    taxa = "NULL"
                Else
                    taxa = taxa.Replace(".", "").Replace(",", ".")
                End If

                SQL = " INSERT INTO tblColaboradores_Projetos "
                SQL += "VALUES(" & proCodigo & "," & colCodigo & "," & taxa & ")"

                If Not comandoSQL(SQL) Then
                    lblMensagem.Style.Add("color", "Red")
                    lblMensagem.Text = sqlErro
                    Return
                End If

            Next


            'If lstConsultoresProjeto.Items.Count > 0 Then

            '    For i = 0 To lstConsultoresProjeto.Items.Count - 1

            '        ' Vai inserindo os colaboradores da comboBox "Colaboradores liberados para apontamento" na
            '        '  tabela tblColaboradores_Projetos
            '        lstConsultoresProjeto.SelectedIndex = i
            '        colCodigo = lstConsultoresProjeto.SelectedValue

            '        SQL = " INSERT INTO tblColaboradores_Projetos "
            '        SQL += "VALUES(" & proCodigo & "," & colCodigo & "," & taxa & ")"

            '        If Not comandoSQL(SQL) Then
            '            lblMensagem.Style.Add("color", "Red")
            '            lblMensagem.Text = sqlErro
            '            Return
            '        End If

            '    Next i

            'End If
        Else
            lblMensagem.Style.Add("color", "Red")
            lblMensagem.Text = sqlErro
            Return
        End If

        limparFormulario()
        lblMensagem.Text = "Alterações salvas com sucesso"

        ddlProjeto.SelectedIndex = 0
        habilitaDesabilitaForm()

    End Sub

End Class