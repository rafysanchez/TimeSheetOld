Partial Public Class CadastroProjeto
    Inherits System.Web.UI.Page

    Dim SQL = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

#If Not Debug Then
        Try
            If Not Session("permissao").ToString.Contains("Cadastro de Projeto") Then
                Response.Redirect("..\Default.aspx")
            End If
        Catch ex As Exception
            Response.Redirect("..\Default.aspx")
        End Try

        If Session("perCodigoLogado") <> 4 And Session("perCodigoLogado") <> 5 Then
            btnSalvar.Enabled = False
            btnCadastrarNovo.Visible = False
        End If

#End If

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

        lblMensagem.Style.Add("font-style", "italic")
        lblMensagem.Style.Add("color", "Red")
        lblMensagem2.Style.Add("font-style", "italic")
        lblMensagem2.Style.Add("color", "Red")

        txtNomeProjeto.Focus()

    End Sub

    Private Sub btnSalvar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSalvar.Click
        lblMensagem.Style.Add("color", "Blue")
        lblMensagem.Text = "Processando..."
        lblMensagem2.Text = ""
        If validaFormulario() Then
            If gravaBancoDados() Then
                lblMensagem.Style.Add("color", "Blue")
                If btnSalvar.Text = "Salvar" Then
                    lblMensagem.Text = "Cadastro salvo com sucesso"
                Else
                    lblMensagem.Text = "Cadastro alterado com sucesso"
                End If
                limparFormulario()
            End If
        Else
            lblMensagem.Style.Add("color", "Red")
            lblMensagem.Text = "Preencha os campos obrigatórios"
        End If
    End Sub

    Function gravaBancoDados() As Boolean

        Dim SQL As String = ""
        Dim colCodigo As Integer = Session("colCodigoLogado")
        Dim possueGP = "N"
        Dim possueGC = "N"
        Dim possueDir = "N"

        lblMensagem.Style.Add("color", "Blue")

        If btnSalvar.Text = "Salvar" Then
            'verifica se existe um nome de projeto igual
            If selectSQL("SELECT proNome FROM v_projetos WHERE proNome ='" & txtNomeProjeto.Text & "'") Then
                If Not dr.Read() Then
                    SQL = "INSERT INTO tblProjetos ( " &
                          "proNome, proCentroCusto, proNomeCliente, proNomeGerenteCliente, proEmailGerenteCliente, " &
                          "proDataInicio, proDataFim, proStatus, codGP, codGC, codDir ) " &
                          "VALUES ( " &
                          "'" & txtNomeProjeto.Text & "', " &
                          "'" & txtCentroCusto.Text & "', " &
                          "'" & txtNomeCliente.Text & "', " &
                          "'" & txtNomeGerenteCliente.Text & "', " &
                          "'" & txtEmailGerenteCliente.Text & "', " &
                          "'" & txtDataInicio.Text & "', " &
                          "'" & txtDataFim.Text & "', " &
                          "'" & ddlStatus.SelectedValue & "', "
                    Select Case Session("perCodigoLogado")
                        Case 2 ' Gerente de projeto
                            SQL += " " & colCodigo & ", NULL, NULL  );"
                        Case 3 ' Gerente de contas
                            SQL += " NULL, " & colCodigo & ", NULL );"
                        Case 4 ' Diretoria
                            SQL += " NULL, NULL, " & colCodigo & " );"
                        Case Else
                            SQL += " NULL, NULL, NULL );"
                    End Select
                Else
                    lblMensagem.Style.Add("color", "Red")
                    lblMensagem.Text = "Já existe um projeto com o mesmo nome"
                    Return False
                End If
            End If
        Else
            SQL = _
             "UPDATE tblProjetos SET" + vbCrLf + _
             "  proNome = '" & txtNomeProjeto.Text & "'" + vbCrLf + _
             "  ,proCentroCusto = '" & txtCentroCusto.Text & "'" + vbCrLf + _
             "  ,proNomeCliente = '" & txtNomeCliente.Text & "'" + vbCrLf + _
             "  ,proNomeGerenteCliente = '" & txtNomeGerenteCliente.Text & "'" + vbCrLf + _
             "  ,proEmailGerenteCliente = '" & txtEmailGerenteCliente.Text & "'" + vbCrLf + _
             "  ,proDataInicio = '" & txtDataInicio.Text & "'" + vbCrLf + _
             "  ,proDataFim = '" & txtDataFim.Text & "'" + vbCrLf + _
             "  ,proStatus = '" & ddlStatus.SelectedValue & "'" + vbCrLf + _
             "WHERE proCodigo = " & Session("proCodigo")
        End If

        If comandoSQL(SQL) Then
            If btnSalvar.Text = "Salvar" Then
                lblMensagem.Text = "Salvo com sucesso"
            Else
                lblMensagem.Text = "Alterado com sucesso"
            End If
            limparFormulario()
        Else
            lblMensagem.Style.Add("color", "Red")
            lblMensagem.Text = sqlErro
            lblMensagem2.Text = SQL
        End If

    End Function

    Function validaFormulario()

        Dim deuErro As Boolean = False

        lblMensagem.Text = ""
        lblMensagem2.Text = ""
        lblCentroCusto.Text = ""

        txtNomeProjeto.BackColor = Drawing.Color.White
        txtNomeCliente.BackColor = Drawing.Color.White
        txtNomeGerenteCliente.BackColor = Drawing.Color.White
        txtEmailGerenteCliente.BackColor = Drawing.Color.White
        txtDataInicio.BackColor = Drawing.Color.White
        txtDataFim.BackColor = Drawing.Color.White

        If txtNomeProjeto.Text.Trim() = "" Then
            txtNomeProjeto.BackColor = Drawing.Color.Yellow
            deuErro = True
        Else
            txtNomeProjeto.BackColor = Drawing.Color.White
        End If

        If txtCentroCusto.Text.Trim = "" Then
            txtCentroCusto.BackColor = Drawing.Color.Yellow
            deuErro = True
        Else
            'pedido pelo jorge em 25/06/2018 para retirada
            ' Se for diferente o centro de custo na alteração, então checo se o valor inserido já existe
            'If Session("proCentroCusto") <> txtCentroCusto.Text Then
            '    SQL = "SELECT * FROM v_projetos WHERE proCentroCusto = " & txtCentroCusto.Text
            '    If Not selectSQL(SQL) Then
            '        lblMensagem.Text = sqlErro
            '        deuErro = True
            '        Return False
            '    End If
            '    If dr.HasRows Then
            '        lblCentroCusto.Text = "Dado duplicado"
            '        txtCentroCusto.BackColor = Drawing.Color.Yellow
            '        deuErro = True
            '        Return False
            '    End If
            'End If
            txtCentroCusto.BackColor = Drawing.Color.White
        End If

        If txtNomeCliente.Text.Trim() = "" Then
            txtNomeCliente.BackColor = Drawing.Color.Yellow
            deuErro = True
        Else
            txtNomeCliente.BackColor = Drawing.Color.White
        End If

        If txtNomeGerenteCliente.Text.Trim() = "" Then
            txtNomeGerenteCliente.BackColor = Drawing.Color.Yellow
            deuErro = True
        Else
            txtNomeGerenteCliente.BackColor = Drawing.Color.White
        End If

        If txtEmailGerenteCliente.Text.Trim() = "" Then
            txtEmailGerenteCliente.BackColor = Drawing.Color.Yellow
            deuErro = True
        Else
            If Not isEmail(txtEmailGerenteCliente.Text) Then
                txtEmailGerenteCliente.BackColor = Drawing.Color.Yellow
                lblMensagem2.Text = "E-mail invalido"
                deuErro = True
            Else
                txtEmailGerenteCliente.BackColor = Drawing.Color.White
            End If
        End If

        ' Validação dos campos Data Inicio e Data Fim, verifica se um é maior que o outro e se estão inseridos corretamente
        If txtDataInicio.Text.Trim() <> "" Then
            If Not IsDate(txtDataInicio.Text) Then
                txtDataInicio.BackColor = Drawing.Color.Yellow
                lblMensagem2.Text = "Campo 'Data Inicio' está com formato incorreto"
                deuErro = True
            Else
                txtDataInicio.BackColor = Drawing.Color.White
            End If
        End If
        If txtDataFim.Text.Trim() <> "" Then
            If Not IsDate(txtDataFim.Text) Then
                txtDataFim.BackColor = Drawing.Color.Yellow
                lblMensagem2.Text = "Campo 'Data Fim' está com formato incorreto"
                deuErro = True
            Else
                txtDataFim.BackColor = Drawing.Color.White
            End If
        End If
        If IsDate(txtDataInicio.Text) And IsDate(txtDataFim.Text) Then
            If DateDiff("d", txtDataInicio.Text, txtDataFim.Text) < 0 Then
                lblMensagem2.Text = "Data Validade onde está 'de' está mais recente que 'até'"
                txtDataInicio.BackColor = Drawing.Color.Yellow
                txtDataFim.BackColor = Drawing.Color.Yellow
                deuErro = True
            Else
                txtDataInicio.BackColor = Drawing.Color.White
                txtDataFim.BackColor = Drawing.Color.White
            End If
        End If
        ' Fim de verificação das Datas  -----------------------------------------------------------------------------

        If txtDataInicio.Text.Trim() = "" Then
            txtDataInicio.BackColor = Drawing.Color.Yellow
            deuErro = True
        Else
            txtDataInicio.BackColor = Drawing.Color.White
        End If
        If txtDataFim.Text.Trim() = "" Then
            txtDataFim.BackColor = Drawing.Color.Yellow
            deuErro = True
        Else
            txtDataFim.BackColor = Drawing.Color.White
        End If

        If deuErro Then
            Return False
        Else
            Return True
        End If

    End Function

    Protected Sub btnLimpar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLimpar.Click
        limparFormulario()
        lblMensagem.Text = ""
        lblMensagem2.Text = ""
    End Sub

    Private Sub limparFormulario()
        txtPesquisar.Text = ""
        lbltxtPesquisar.Text = ""
        txtNomeProjeto.Text = ""
        txtNomeProjeto.BackColor = Drawing.Color.White
        txtCentroCusto.Text = ""
        txtCentroCusto.BackColor = Drawing.Color.White
        txtNomeCliente.Text = ""
        txtNomeCliente.BackColor = Drawing.Color.White
        txtNomeGerenteCliente.Text = ""
        txtNomeGerenteCliente.BackColor = Drawing.Color.White
        txtEmailGerenteCliente.Text = ""
        txtEmailGerenteCliente.BackColor = Drawing.Color.White
        txtDataInicio.Text = ""
        txtDataInicio.BackColor = Drawing.Color.White
        txtDataFim.Text = ""
        txtDataFim.BackColor = Drawing.Color.White
        ddlStatus.SelectedValue = "Status"
        txtNomeProjeto.Focus()
        btnSalvar.Text = "Salvar"
        btnCadastrarNovo.Visible = False
        lblTitulo.Text = "Novo cadastro"
        ddlStatus.SelectedIndex = 0
    End Sub

    '=====================================================================================================================
    '   Quando selecionado um Projeto preenche-se todo formulário para edição
    '=====================================================================================================================   
    Private Sub preencheFormulario()

        txtNomeProjeto.BackColor = Drawing.Color.White
        txtCentroCusto.BackColor = Drawing.Color.White
        txtNomeCliente.BackColor = Drawing.Color.White
        txtNomeGerenteCliente.BackColor = Drawing.Color.White
        txtEmailGerenteCliente.BackColor = Drawing.Color.White
        txtDataInicio.BackColor = Drawing.Color.White
        txtDataFim.BackColor = Drawing.Color.White

        Dim SQL As String = ""
        Dim i As Integer = 0
        Dim j As Integer = 0
        lbltxtPesquisar.Text = ""
        lblMensagem.Text = ""

        If txtPesquisar.Text.Trim <> "" Then
            ' Quando selecionado um item no DropDownList "Projeto" faz um select e preenche todos os campos restantes
            If selectSQL("SELECT * FROM v_projetos WHERE proNome COLLATE Latin1_General_CI_AI LIKE '%" & txtPesquisar.Text & "%'") Then
                If dr.Read() Then
                    Session("proCodigo") = dr("proCodigo")
                    txtNomeProjeto.Text = dr("proNome")
                    If dr("proCentroCusto") IsNot DBNull.Value Then
                        txtCentroCusto.Text = dr("proCentroCusto")
                        ' Gravo em Sessão para depois na hora de alterar algum cadastro possa comparar se ouve mudança
                        Session("proCentroCusto") = dr("proCentroCusto")
                    End If
                    txtNomeCliente.Text = dr("proNomeCliente")
                    txtNomeGerenteCliente.Text = dr("proNomeGerenteCliente")
                    txtEmailGerenteCliente.Text = dr("proEmailGerenteCliente")
                    txtDataInicio.Text = dr("proDataInicio")
                    txtDataFim.Text = dr("proDataFim")
                    ddlStatus.SelectedValue = dr("proStatus")
                    btnSalvar.Text = "Alterar Cadastro"
                    If Session("perCodigoLogado") <> 4 And Session("perCodigoLogado") <> 5 Then
                        btnSalvar.Enabled = False
                        btnCadastrarNovo.Visible = False
                    Else
                        btnCadastrarNovo.Visible = True
                    End If
                    lblTitulo.Text = "Alteração de cadastro - '" & dr("proNome") & "'"
                Else
                    ' Exibir mensagem de erro
                    lbltxtPesquisar.Style.Add("color", "Red")
                    lbltxtPesquisar.Text = "Não foi encontrado registros com este nome de projeto"
                End If
            Else
                ' Exibir mensagem de erro
                lblMensagem.Style.Add("color", "Red")
                lblMensagem.Text = sqlErro
            End If
        Else
            lbltxtPesquisar.Style.Add("color", "Red")
            lbltxtPesquisar.Text = "Campo esta vazio"
        End If

    End Sub

    Private Sub btnPesquisar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPesquisar.Click
        preencheFormulario()
        txtPesquisar.Text = ""
    End Sub

    Private Sub btnCadastrarNovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCadastrarNovo.Click
        limparFormulario()
    End Sub

End Class