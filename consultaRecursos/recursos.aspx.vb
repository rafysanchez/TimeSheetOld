Imports System.Data.OleDb
Imports System.Data.SqlClient

Partial Public Class recursos
    Inherits System.Web.UI.Page

    Private da As OleDbDataAdapter
    Private dt As New DataTable
    Private sqlComando As OleDbCommand
    Private sqlConn As New OleDbConnection
    Private drExcel As OleDbDataReader
    Dim caminhoArquivo As String = Server.MapPath("Relação de Consultores SAP - v2.xlsx")
    Dim connString_Excel As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + caminhoArquivo + "';Extended Properties=""Excel 12.0"""
    Dim SQL As String
    Protected URL As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

#If DEBUG Then
        Session("colPerfilLogado") = "Gerente de contas"
        Session("colCodigoLogado") = 89
#Else
        Try
            If Not Session("permissao").ToString.Contains("Pesquisa") Then
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

            formulario.Attributes.Add("oncopy", "Javascript: limparDados();")

            Dim body As HtmlGenericControl
            Dim divCorpo As HtmlGenericControl
            body = Page.Master.FindControl("body")
            divCorpo = Page.Master.FindControl("divCorpo")
            body.Attributes.Add("onkeydown", "Javascript: return noCopy(event)")
            divCorpo.Style.Add("width", "auto")
            divCorpo.Style.Add("margin-left", "20px")

            limparCamposEdicao()

            btnAtualizar.Enabled = False
            btnExcluir.Enabled = False

            carregaFiltros()
            preparaLinkParaEnvioEmail(getSQLComFiltros())

        End If

        URL = Request.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~/") _
              & "consultaRecursos/exibeEmails.aspx"

        hplExibirEmails.HRef = URL

        lblMensagem.Style.Add("color", "Red")
        lblMensagem.Text = ""

        fonteDados.ConnectionString = getConnectionString()

        'Session("connString_Excel") = connString_Excel
        'fonteDados.ConnectionString = Session("connString_Excel")
        'carregaExcelParaBD()

    End Sub

    Private Sub carregaFiltros()

        ddlModulo.Items.Clear()
        ddlSkill.Items.Clear()

        ddlModulo.Items.Add("Todos")
        ddlSkill.Items.Add("Todos")

        ' Carrega os comboBox Modulos de acordo com os valores da coluna recFrente
        SQL = "SELECT [recFrente] FROM [v_recursos] GROUP BY [recFrente] ORDER BY [recFrente]"
        If Not selectSQL(SQL) Then
            sqlErro = sqlErro
        End If
        While dr.Read()
            ddlModulo.Items.Add(dr("recFrente"))
        End While

        ' Carrega os comboBox Skill de acordo com os valores da coluna recSkill
        SQL = "SELECT [recSkill] FROM [v_recursos] GROUP BY [recSkill] ORDER BY [recSkill]"
        If Not selectSQL(SQL) Then
            sqlErro = sqlErro
        End If
        While dr.Read()
            ddlSkill.Items.Add(dr("recSkill"))
        End While

        Try
            If Session("ddlModulo").ToString.Trim <> "" Then
                ddlModulo.SelectedValue = Session("ddlModulo")
            End If
            If Session("ddlSkill").ToString.Trim <> "" Then
                ddlSkill.SelectedValue = Session("ddlSkill")
            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Sub preparaLinkParaEnvioEmail(SQL As String)

        Try
            ' Prepara o link para enviar emails CCO
            selectSQL(SQL)
            If dr.HasRows Then

                hplEnviarEmailCCO.Visible = True

                Dim emails = "mailto:?bcc="

                While dr.Read

                    If Not IsDBNull(dr("recEmail")) Then
                        If isEmail(dr("recEmail")) Then
                            If Not emails.Contains(dr("recEmail")) Then
                                emails += dr("recEmail") + ";"
                            End If
                        ElseIf Not IsDBNull(dr("recMSN")) Then
                            If isEmail(dr("recMSN")) Then
                                If Not emails.Contains(dr("recMSN")) Then
                                    emails += dr("recMSN") + ";"
                                End If
                            End If
                        End If
                    End If

                End While

                hplEnviarEmailCCO.HRef = emails
                Dim tamanho = emails.Length()

                ' Se foi adicionado algum e-mail então removo o ponto e virgula do final
                If tamanho > 12 Then
                    hplEnviarEmailCCO.HRef = emails.Substring(0, tamanho - 1)
                Else
                    hplEnviarEmailCCO.Visible = False
                End If

            Else
                hplEnviarEmailCCO.Visible = False
            End If
        Catch ex As Exception
            lblMensagem.Text = ex.Message
        End Try

    End Sub

    ' Sub-rotina que é executada antes de carregar o Grid, é nela que é preparada a Query para a sub "carregaGrid"
    Private Function getSQLComFiltros() As String

        Dim filtroAdicionado As Boolean = False

        SQL = "SELECT * FROM [v_recursos] "

        If ddlModulo.SelectedIndex > 0 Then
            SQL += "WHERE [recFrente]='" & ddlModulo.SelectedValue & "' COLLATE Latin1_General_CI_AI "
            filtroAdicionado = True
        End If

        If ddlSkill.SelectedIndex > 0 Then
            If filtroAdicionado Then
                SQL += "AND [recSkill]='" & ddlSkill.SelectedValue & "' COLLATE Latin1_General_CI_AI "
            Else
                SQL += "WHERE [recSkill]='" & ddlSkill.SelectedValue & "' COLLATE Latin1_General_CI_AI "
                filtroAdicionado = True
            End If
        End If

        If ddlAlocado.SelectedIndex > 0 Then
            If ddlAlocado.SelectedIndex = 1 Then ' Quando selecionado para exibir campos que estão vazios (indefinidos)
                If filtroAdicionado Then
                    SQL += "AND ([recAlocado] = '' OR [recAlocado] is NULL) "
                Else
                    SQL += "WHERE ([recAlocado] = '' OR [recAlocado] is NULL) "
                    filtroAdicionado = True
                End If
            Else
                If filtroAdicionado Then
                    SQL += "AND [recAlocado]='" & ddlAlocado.SelectedValue & "' "
                Else
                    SQL += "WHERE [recAlocado]='" & ddlAlocado.SelectedValue & "' "
                    filtroAdicionado = True
                End If
            End If
        End If

        If txtFiltroNome.Text.Trim <> "" Then
            If filtroAdicionado Then
                SQL += "AND [recNome] COLLATE Latin1_General_CI_AI LIKE '%" & txtFiltroNome.Text & "%' "
            Else
                SQL += "WHERE [recNome] COLLATE Latin1_General_CI_AI LIKE '%" & txtFiltroNome.Text & "%' "
                filtroAdicionado = True
            End If
        End If

        If ckbSemEmail.Checked Then
            If filtroAdicionado Then
                SQL += "AND (([recEmail] = '' OR [recEmail] is NULL) AND ([recMSN] = '' OR [recMSN] is NULL)) "
            Else
                SQL += "WHERE (([recEmail] = '' OR [recEmail] is NULL) AND ([recMSN] = '' OR [recMSN] is NULL))  "
                filtroAdicionado = True
            End If
        End If

        If ckbSemTelefone.Checked Then
            If filtroAdicionado Then
                SQL += "AND (([recTelefone1] = '' OR [recTelefone1] is NULL)" & _
                       " AND ([recTelefone2] = '' OR [recTelefone2] is NULL)" & _
                       " AND ([recTelefone3] = '' OR [recTelefone3] is NULL)) "
            Else
                SQL += "WHERE (([recTelefone1] = '' OR [recTelefone1] is NULL)" & _
                       " AND ([recTelefone2] = '' OR [recTelefone2] is NULL)" & _
                       " AND ([recTelefone3] = '' OR [recTelefone3] is NULL))  "
                filtroAdicionado = True
            End If
        End If

        If ckbSemDataAlocacao.Checked Then
            If filtroAdicionado Then
                SQL += "AND ((recDataInicio is NULL OR recDataInicio = '') OR (recDataFim is NULL OR recDataFim = '')) "
            Else
                SQL += "WHERE ((recDataInicio is NULL OR recDataInicio = '') OR (recDataFim is NULL OR recDataFim = '')) "
                filtroAdicionado = True
            End If
        End If

        If txtFiltroDataInicio.Text.Trim <> "" Or txtFiltroDataFim.Text.Trim <> "" Then
            If txtFiltroDataInicio.Text.Trim <> "" And txtFiltroDataFim.Text.Trim <> "" Then
                If filtroAdicionado Then
                    SQL += "AND (('" & txtFiltroDataInicio.Text & "' < recDataInicio OR '" & txtFiltroDataInicio.Text & "' > recDataFim) " & _
                           "AND ('" & txtFiltroDataFim.Text & "' < recDataFim OR '" & txtFiltroDataFim.Text & "' > recDataFim)) "
                Else
                    SQL += "WHERE (('" & txtFiltroDataInicio.Text & "' < recDataInicio OR '" & txtFiltroDataInicio.Text & "' > recDataFim) " & _
                           "AND ('" & txtFiltroDataFim.Text & "' < recDataFim OR '" & txtFiltroDataFim.Text & "' > recDataFim)) "
                    filtroAdicionado = True
                End If
            Else
                ' Os dois campos precisam estar preenchidos
            End If
        End If

        SQL += "ORDER BY [recNome] "

        Session("sqlFiltros") = SQL

        Return SQL

    End Function

    ' Carrega "GridView" de acordo com o a Query que a sub "aplicarFiltros" criou e também a página corrente
    Private Sub carregaGrid(ByVal pagina As Integer, ByVal SQL As String)

        preparaLinkParaEnvioEmail(getSQLComFiltros())

        Try
            fonteDados.SelectCommand = SQL
            gridRecursos.PageIndex = pagina
            gridRecursos.DataBind()

            ' Estas Sessions serve para manter o valor dos filtros após um postback
            Session("ddlModulo") = ddlModulo.SelectedItem.ToString
            Session("ddlSkill") = ddlSkill.SelectedItem.ToString

        Catch ex As Exception
            lblMensagem.Text = ex.Message
        End Try

    End Sub

    Function conectaSQLExcel() As Boolean
        sqlConn.Close()
        sqlConn.ConnectionString = connString_Excel
        Try
            sqlConn.Open()
            Return True
        Catch ex As Exception
            sqlErro = ex.Message()
            Return False
        End Try
    End Function

    Function selectSQLExcel(ByVal sqlString As String) As Boolean
        If conectaSQLExcel() Then
            sqlComando = New OleDbCommand(sqlString, sqlConn)
            Try
                drExcel = sqlComando.ExecuteReader()
                Return True
            Catch ex As Exception
                sqlErro = ex.Message()
                Return False
            End Try
        End If
    End Function

    Private Sub gridRecursos_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gridRecursos.PageIndexChanging
        gridRecursos.SelectedIndex = -1
        carregaGrid(e.NewPageIndex, getSQLComFiltros())
    End Sub

    Private Sub carregaExcelParaBD()

        SQL = "SELECT * FROM [Consultores$]"
        If selectSQLExcel(SQL) Then
            If Not drExcel.HasRows Then
                lblMensagem.Text = "Tabela vazia, não é possivel fazer a carga nova"
                Return
            End If
        Else
            lblMensagem.Text = "Não foi possivel conectar ao arquivo, talvez ele esteja em uso, ou não esta no endereço especifico."
            lblMensagem.Text = sqlErro
            Return
        End If

        SQL = "DELETE tblRecursos"
        If Not comandoSQL(SQL) Then
            lblMensagem.Text = sqlErro
            Return
        End If

        While drExcel.Read

            SQL = _
             "INSERT INTO tblRecursos (frente,skill,nome,[telefone 1],[telefone 2],[telefone 3],[e-mail],MSN) " + vbCrLf + _
             "VALUES (" + vbCrLf + _
             "   '" & drExcel("Frente") & "'  -- frente" + vbCrLf + _
             "  ,'" & drExcel("Skill") & "'  -- skill" + vbCrLf + _
             "  ,'" & drExcel("Nome") & "'  -- nome" + vbCrLf + _
             "  ,'" & drExcel("Telefone 1") & "'  -- telefone 1" + vbCrLf + _
             "  ,'" & drExcel("Telefone 2") & "'  -- telefone 2" + vbCrLf + _
             "  ,'" & drExcel("Telefone 3") & "'  -- telefone 3" + vbCrLf + _
             "  ,'" & drExcel("E-mail") & "'  -- e-mail" + vbCrLf + _
             "  ,'" & drExcel("MSN") & "'  -- MSN" + vbCrLf + _
             ")"

            If Not comandoSQL(SQL) Then
                lblMensagem.Text = sqlErro
                Return
            End If

        End While

        lblMensagem.Style.Add("color", "Blue")
        lblMensagem.Text = "Carga de dados concluída com sucesso."

    End Sub

    Protected Sub gridRecursos_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles gridRecursos.SelectedIndexChanged

        If gridRecursos.SelectedIndex >= 0 Then

            limparCamposEdicao()
            carregaCamposDeEdicao()

        End If

    End Sub

    Private Sub carregaCamposDeEdicao()

        btnAdicionarSalvar.Text = "Novo"
        txtCodigo.Text = gridRecursos.SelectedRow.Cells(0).Text
        txtFrente.Text = Replace(gridRecursos.SelectedRow.Cells(2).Text, "&nbsp;", "")
        txtSkill.Text = Replace(gridRecursos.SelectedRow.Cells(3).Text, "&nbsp;", "")
        txtNome.Text = Replace(gridRecursos.SelectedRow.Cells(4).Text, "&nbsp;", "")
        txtTelefone1.Text = Replace(gridRecursos.SelectedRow.Cells(5).Text, "&nbsp;", "")
        txtTelefone2.Text = Replace(gridRecursos.SelectedRow.Cells(6).Text, "&nbsp;", "")
        txtTelefone3.Text = Replace(gridRecursos.SelectedRow.Cells(7).Text, "&nbsp;", "")
        txtEmail.Text = CType(gridRecursos.SelectedRow.Cells(8).FindControl("linkEmail"), HyperLink).Text
        txtMSN.Text = CType(gridRecursos.SelectedRow.Cells(9).FindControl("linkMSN"), HyperLink).Text
        If gridRecursos.SelectedRow.Cells(10).Text.ToLower = "sim" Then
            radioAlocNao.Checked = False
            radioAlocSim.Attributes.Add("checked", "checked")
        Else
            radioAlocSim.Checked = False
            radioAlocNao.Attributes.Add("checked", "checked")
        End If
        txtDataInicio.Text = Replace(gridRecursos.SelectedRow.Cells(11).Text, "&nbsp;", "")
        txtDataFim.Text = Replace(gridRecursos.SelectedRow.Cells(12).Text, "&nbsp;", "")
        txtObservacao.InnerText = gridRecursos.SelectedRow.Cells(13).Text.Replace("&nbsp;", "")

        btnAtualizar.Enabled = True
        btnExcluir.Enabled = True
        lblAsteriscoFrente.Visible = False
        lblAsteriscoSkill.Visible = False
        lblAsteriscoNome.Visible = False
        txtFrente.ReadOnly = True
        txtNome.ReadOnly = True
        txtFrente.Style.Add("background-color", "#DCDCDC")
        txtNome.Style.Add("background-color", "#DCDCDC")

    End Sub

    Private Sub limparCamposEdicao()

        txtSkill.Style.Add("background-color", "White")
        txtDataInicio.Style.Add("background-color", "White")
        txtDataFim.Style.Add("background-color", "White")
        txtFrente.Style.Add("background-color", "White")
        txtNome.Style.Add("background-color", "White")

        btnAdicionarSalvar.Text = "Salvar"
        txtFrente.ReadOnly = False
        txtNome.ReadOnly = False
        lblAsteriscoFrente.Visible = True
        lblAsteriscoSkill.Visible = True
        lblAsteriscoNome.Visible = True

        txtCodigo.Text = ""
        txtFrente.Text = ""
        txtSkill.Text = ""
        txtNome.Text = ""
        txtTelefone1.Text = ""
        txtTelefone2.Text = ""
        txtTelefone3.Text = ""
        txtEmail.Text = ""
        txtMSN.Text = ""
        txtDataInicio.Text = ""
        txtDataFim.Text = ""
        txtObservacao.InnerText = ""
        btnAtualizar.Enabled = False
        btnExcluir.Enabled = False

    End Sub

    Private Sub btnExportarExcel()

        Dim dtParaExcel As New DataTable("excel")
        ' ''''''' Criando as colunas para a DataTable 
        With dtParaExcel
            .Columns.Add("Nome do Projeto", GetType(String))
            .Columns.Add("Nome do Colaborador", GetType(String))
            .Columns.Add("Contrato", GetType(String))
            .Columns.Add("Taxa", GetType(String))
            .Columns.Add("Taxa Projeto", GetType(String))
            .Columns.Add("Horas", GetType(String))
            .Columns.Add("Valor", GetType(String))
            .Columns.Add("Razão Social", GetType(String))
            .Columns.Add("Empresa Compartilhada", GetType(String))
        End With



    End Sub

    '****** Filtros de pesquisa

    Sub btnPesquisar_Click() Handles btnPesquisar.Click

        Dim pagina = 0
        Dim linhaSelecionada = -1

        gridRecursos.SelectedIndex = linhaSelecionada
        carregaGrid(pagina, getSQLComFiltros())
        limparCamposEdicao()

    End Sub

    '****** Ações dos botões

    Private Sub btnAtualizar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAtualizar.Click

        atualizarRecurso()

    End Sub

    Private Sub btnAdicionarSalvar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdicionarSalvar.Click

        gridRecursos.SelectedIndex = -1

        ' Se o botão estiver com o texto "Adicionar", então habilita todos os campos para inserção de novos registros
        If btnAdicionarSalvar.Text.ToLower = "novo" Then
            btnAdicionarSalvar.Text = "Salvar"
            txtFrente.Style.Add("background-color", "White")
            txtNome.Style.Add("background-color", "White")
            txtFrente.ReadOnly = False
            txtNome.ReadOnly = False
            lblAsteriscoFrente.Visible = True
            lblAsteriscoSkill.Visible = True
            lblAsteriscoNome.Visible = True
            limparCamposEdicao()
        Else
            btnAtualizar.Enabled = False
            ' Tento gravar se conseguir passo a diante
            If incluirRecurso() Then
                ' Deixo o botão como estava antes e os campos também
                btnAdicionarSalvar.Text = "Novo"
                txtFrente.Style.Add("background-color", "#DCDCDC")
                txtNome.Style.Add("background-color", "#DCDCDC")
                txtFrente.ReadOnly = True
                txtNome.ReadOnly = True
                lblAsteriscoFrente.Visible = False
                lblAsteriscoSkill.Visible = False
                lblAsteriscoNome.Visible = False
            End If
        End If

    End Sub

    '****** Sub-rotinas de inclusão, exclusão, alteração e validação

    Function validarCampos() As Boolean

        txtFrente.Style.Add("background-color", "White")
        txtSkill.Style.Add("background-color", "White")
        txtNome.Style.Add("background-color", "White")
        txtDataInicio.Style.Add("background-color", "White")
        txtDataFim.Style.Add("background-color", "White")
        txtEmail.Style.Add("background-color", "White")
        txtMSN.Style.Add("background-color", "White")

        Dim campoObrigatorioPreenchido As Boolean = True

        '   Validação dos campos Data Inicio e Data Fim, verifica se um é maior que o outro e se estão inseridos(corretamente)
        ' Verifico se os dois campos de datas estão preenchidos
        If txtDataInicio.Text.Trim() <> "" And txtDataFim.Text.Trim() <> "" Then
            ' Verifico se a data inserida é uma data valida
            If Not IsDate(txtDataInicio.Text) Then
                lblMensagem.Text = "Campo 'Data Inicio' está com formato incorreto"
                txtDataInicio.Style.Add("background-color", "Yellow")
                Return False
            End If
            ' Verifico se a data inserida é uma data valida
            If Not IsDate(txtDataFim.Text) Then
                lblMensagem.Text = "Campo 'Data Fim' está com formato incorreto"
                txtDataFim.Style.Add("background-color", "Yellow")
                Return False
            End If
            ' Verifico se as datas estão em ordem cronológica correta
            If DateDiff("d", txtDataInicio.Text, txtDataFim.Text) < 0 Then
                lblMensagem.Text = "Na data, campo 'de' está mais recente que o campo 'até'"
                txtDataInicio.Style.Add("background-color", "Yellow")
                txtDataFim.Style.Add("background-color", "Yellow")
                Return False
            End If

        Else
            If txtDataInicio.Text.Trim() <> "" Then
                ' Verifico se a data inserida é uma data valida
                If Not IsDate(txtDataInicio.Text) Then
                    lblMensagem.Text = "Campo 'Data Inicio' está com formato incorreto"
                    txtDataInicio.Style.Add("background-color", "Yellow")
                    Return False
                End If
            End If
            If txtDataFim.Text.Trim() <> "" Then
                ' Verifico se a data inserida é uma data valida
                If Not IsDate(txtDataFim.Text) Then
                    lblMensagem.Text = "Campo 'Data Fim' está com formato incorreto"
                    txtDataFim.Style.Add("background-color", "Yellow")
                    Return False
                End If
            End If
        End If
        ' Fim de verificação das Datas    ----------------------------------------------------------------------------

        If txtFrente.Text.Trim() = "" Then
            txtFrente.Style.Add("background-color", "Yellow")
            campoObrigatorioPreenchido = False
        End If

        If txtSkill.Text.Trim() = "" Then
            txtSkill.Style.Add("background-color", "Yellow")
            campoObrigatorioPreenchido = False
        End If

        If txtNome.Text.Trim() = "" Then
            txtNome.Style.Add("background-color", "Yellow")
            campoObrigatorioPreenchido = False
        End If

        ' Se algum campo obrigatório não foi preenchido
        If Not campoObrigatorioPreenchido Then
            lblMensagem.Text = "Campo(s) obrigatório(s) em branco ou incorreto(s)."
            Return False
        End If

        If txtEmail.Text.Trim <> "" Then
            If Not isEmail(txtEmail.Text) Then
                txtEmail.Style.Add("background-color", "Yellow")
                lblMensagem.Text = "E-mail invalido."
                Return False
            End If
        End If

        If txtMSN.Text.Trim <> "" Then
            If Not isEmail(txtMSN.Text) Then
                txtMSN.Style.Add("background-color", "Yellow")
                lblMensagem.Text = "MSN invalido."
                Return False
            End If
        End If

        Return True

    End Function

    Function incluirRecurso() As Boolean

        Dim sqlFiltros = Session("sqlFiltros")

        If validarCampos() Then

            Dim varFrente = "NULL"
            Dim varSkill = "NULL"
            Dim varNome = "NULL"
            Dim varTelefone1 = "NULL"
            Dim varTelefone2 = "NULL"
            Dim varTelefone3 = "NULL"
            Dim varEmail = "NULL"
            Dim varMSN = "NULL"
            Dim varAlocado = "NULL"
            Dim varDataInicio = "NULL"
            Dim varDataFim = "NULL"
            Dim varObservacao = "NULL"

            ' Campos que são obrigatórios e já passaram por validação
            varFrente = "'" & txtFrente.Text & "'"
            varSkill = "'" & txtSkill.Text & "'"
            varNome = "'" & txtNome.Text & "'"

            ' Campos que não são obrigatórios
            If txtTelefone1.Text.Trim() <> "" Then
                varTelefone1 = "'" & txtTelefone1.Text & "'"
            End If

            If txtTelefone2.Text.Trim() <> "" Then
                varTelefone2 = "'" & txtTelefone2.Text & "'"
            End If

            If txtTelefone3.Text.Trim() <> "" Then
                varTelefone3 = "'" & txtTelefone3.Text & "'"
            End If

            If txtEmail.Text.Trim() <> "" Then
                varEmail = "'" & txtEmail.Text & "'"
            End If

            If txtMSN.Text.Trim() <> "" Then
                varMSN = "'" & txtMSN.Text & "'"
            End If

            If radioAlocSim.Checked Then
                varAlocado = "'Sim'"
            ElseIf radioAlocNao.Checked Then
                varAlocado = "'Não'"
            End If

            If txtDataInicio.Text.Trim() <> "" Then
                varDataInicio = "'" & txtDataInicio.Text & "'"
            End If

            If txtDataFim.Text.Trim() <> "" Then
                varDataFim = "'" & txtDataFim.Text & "'"
            End If

            If txtObservacao.InnerText.Trim() <> "" Then
                varObservacao = "'" & txtObservacao.InnerText & "'"
            End If

            ' Grava o novo registro
            SQL = "INSERT INTO tblRecursos ( recFrente, recSkill, recNome, recTelefone1, recTelefone2, recTelefone3, " & _
                  "  recEmail, recMSN, recAlocado, recDataInicio, recDataFim, recObservacao) " & _
                  "VALUES ( " & _
                  "   " & varFrente & " " & _
                  "  ," & varSkill & " " & _
                  "  ," & varNome & " " & _
                  "  ," & varTelefone1 & " " & _
                  "  ," & varTelefone2 & " " & _
                  "  ," & varTelefone3 & " " & _
                  "  ," & varEmail & " " & _
                  "  ," & varMSN & " " & _
                  "  ," & varAlocado & " " & _
                  "  ," & varDataInicio & " " & _
                  "  ," & varDataFim & _
                  "  ," & varObservacao & _
                  " )"

            If Not comandoSQL(SQL) Then
                lblMensagem.Text = sqlErro
                Return False
            Else
                lblMensagem.Style.Add("color", "Blue")
                lblMensagem.Text = "Registro incluido com sucesso."
                carregaFiltros()

                gridRecursos.SelectedIndex = -1
                carregaGrid(gridRecursos.PageIndex(), sqlFiltros)
                limparCamposEdicao()

            End If

            Return True

        End If

    End Function

    Private Sub atualizarRecurso()

        Dim sqlFiltros = Session("sqlFiltros")

        If validarCampos() Then

            Dim varFrente = "NULL"
            Dim varSkill = "NULL"
            Dim varNome = "NULL"
            Dim varTelefone1 = "NULL"
            Dim varTelefone2 = "NULL"
            Dim varTelefone3 = "NULL"
            Dim varEmail = "NULL"
            Dim varMSN = "NULL"
            Dim varAlocado = "NULL"
            Dim varDataInicio = "NULL"
            Dim varDataFim = "NULL"
            Dim varObservacao = "NULL"

            ' Campos que são obrigatórios e já passaram por validação
            varFrente = "'" & txtFrente.Text & "'"
            varSkill = "'" & txtSkill.Text & "'"
            varNome = "'" & txtNome.Text & "'"

            ' Campos que não são obrigatórios
            If txtTelefone1.Text.Trim() <> "" Then
                varTelefone1 = "'" & txtTelefone1.Text & "'"
            End If

            If txtTelefone2.Text.Trim() <> "" Then
                varTelefone2 = "'" & txtTelefone2.Text & "'"
            End If

            If txtTelefone3.Text.Trim() <> "" Then
                varTelefone3 = "'" & txtTelefone3.Text & "'"
            End If

            If txtEmail.Text.Trim() <> "" Then
                varEmail = "'" & txtEmail.Text & "'"
            End If

            If txtMSN.Text.Trim() <> "" Then
                varMSN = "'" & txtMSN.Text & "'"
            End If

            If radioAlocSim.Checked Then
                varAlocado = "'Sim'"
            ElseIf radioAlocNao.Checked Then
                varAlocado = "'Não'"
            End If

            If txtDataInicio.Text.Trim() <> "" Then
                varDataInicio = "'" & txtDataInicio.Text & "'"
            End If

            If txtDataFim.Text.Trim() <> "" Then
                varDataFim = "'" & txtDataFim.Text & "'"
            End If

            If txtObservacao.InnerText.Trim() <> "" Then
                varObservacao = "'" & txtObservacao.InnerText & "'"
            End If

            ' Prepara a query UPDATE 
            SQL = "UPDATE tblRecursos SET " & _
                  "recSkill = " & varSkill & ", " & _
                  "recTelefone1 = " & varTelefone1 & ", " & _
                  "recTelefone2 = " & varTelefone2 & ", " & _
                  "recTelefone3 = " & varTelefone3 & ", " & _
                  "recEmail = " & varEmail & ", " & _
                  "recMSN = " & varMSN & ", " & _
                  "recAlocado = " & varAlocado & ",  " & _
                  "recDataInicio = " & varDataInicio & ", " & _
                  "recDataFim = " & varDataFim & ", " & _
                  "recObservacao = " & varObservacao & " " & _
                  "WHERE recCodigo = " & txtCodigo.Text

            If Not comandoSQL(SQL) Then
                lblMensagem.Text = sqlErro
            Else
                lblMensagem.Style.Add("color", "Blue")
                lblMensagem.Text = "Atualizado com sucesso..."
                carregaFiltros()
                btnAtualizar.Enabled = False

                gridRecursos.SelectedIndex = -1
                carregaGrid(gridRecursos.PageIndex(), sqlFiltros)
                limparCamposEdicao()

            End If

        End If

    End Sub

    Protected Sub excluirRegistro(ByVal sender As Object, ByVal e As System.EventArgs)

        SQL = "DELETE tblRecursos WHERE recCodigo = " & txtCodigo.Text

        Dim sqlFiltros = Session("sqlFiltros")

        If comandoSQL(SQL) Then
            lblMensagem.Style.Add("color", "Blue")
            lblMensagem.Text = "Registro excluido com sucesso."
            carregaFiltros()
            btnAtualizar.Enabled = False

            gridRecursos.SelectedIndex = -1
            carregaGrid(gridRecursos.PageIndex(), sqlFiltros)
            limparCamposEdicao()
        Else
            lblMensagem.Text = sqlErro
        End If

    End Sub

End Class