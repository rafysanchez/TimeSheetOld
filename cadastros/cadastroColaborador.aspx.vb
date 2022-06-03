Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient

Partial Public Class CadastroConsultor

    Inherits System.Web.UI.Page

    Dim sqlstring As String
    Dim SQL As String
    Dim script = ""

    Protected URL As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        URL = Request.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~/") _
              & "cadastros/exibeTodosColaboradores.aspx"

        If ValidarSessao(Session("colPerfilLogado")) Then Response.Redirect("..\Default.aspx")

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

        If Not IsPostBack Then
            lblMensagem.Style.Add("font-style", "italic")
            lblMensagem2.Style.Add("font-style", "italic")
            lbltxtPesquisar.Style.Add("font-style", "italic")
            lbltxtPesquisar.Style.Add("color", "Red")

            txtNome.Focus()

            ' Select banco de dados para preencher o DropDownList ddlPerfil
            If selectSQL("SELECT * FROM v_perfil ORDER BY perDescricao") Then
                ddlPerfil.Items.Add("")
                While dr.Read()
                    ddlPerfil.Items.Add(New ListItem(dr("perDescricao"), dr("perCodigo")))
                End While
            Else
                lblMensagem.Text = sqlErro
            End If

            ' Select banco de dados para preencher o DropDownList ddlModulo
            If selectSQL("SELECT * FROM v_modulos ORDER BY modDescricao") Then
                ddlModulo.Items.Clear()
                ddlModulo.Items.Add("")
                While dr.Read()
                    ddlModulo.Items.Add(dr("modDescricao"))
                End While
            Else
                lblMensagem.Text = sqlErro
            End If

            ' Select banco de dados para preencher o DropDownList ddlNivel
            If selectSQL("SELECT * FROM v_nivel") Then
                ddlNivel.Items.Clear()
                ddlNivel.Items.Add("")
                While dr.Read()
                    ddlNivel.Items.Add(dr("nivDescricao"))
                End While
            Else
                lblMensagem.Text = sqlErro
            End If

            Session("senha") = ""

            radioCLT.Checked = True

        End If

        ' Dependendo do perfil logado habilita o botão de salvar para inclusão
        ' Somente se for Diretoria ou CSC
        If Session("perCodigoLogado") = 4 Or Session("perCodigoLogado") = 5 Then
            btnSalvar.Enabled = True
            asteriscoSenha.Style.Add("display", "block")
            btnExibirColaboradores.Style.Add("display", "block")
        Else
            btnSalvar.Enabled = False
            asteriscoSenha.Style.Add("display", "none")
            btnExibirColaboradores.Style.Add("display", "none")

            showHideTrValor()

        End If

    End Sub

    Sub showHideTrValor()

        If Session("perCodigoLogado") = 4 Or Session("perCodigoLogado") = 5 Then
            trValor.Visible = True
        Else
            trValor.Visible = False
        End If

        Return

        ' Somente Diretoria e CSC podem visualizar/modificar o campo Salario
        If Session("perCodigoLogado") = 4 Or Session("perCodigoLogado") = 5 Then
            script += "$('[id$=trValor]').css('display', 'block');"
        Else
            script += "$('[id$=trValor]').css('display', 'none');"
        End If

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "JavaScriptBlock", script, True)

    End Sub

    Private Sub btnLimpar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLimpar.Click
        lblMensagem.Text = ""
        limparFormulario()
    End Sub

    '=======================================================================================================
    '   Limpa formulário inteiro ou deixando valores padrões
    '=======================================================================================================
    Private Sub limparFormulario()
        txtPesquisar.Text = ""
        txtNome.Text = ""
        txtCodigoColaboradorSAP.Text = ""
        txtLogin.Text = ""
        txtSenhaInicial.Text = ""
        lbltxtPesquisar.Text = ""
        ddlPerfil.SelectedValue = ""
        txtDataNascimento.Text = ""
        ddlModulo.SelectedValue = ""
        ddlNivel.SelectedValue = ""
        txtCPF.Text = ""
        txtRG.Text = ""
        txtEndereco.Text = ""
        txtCEP.Text = ""
        txtBairro.Text = ""
        txtCidade.Text = ""
        ddlUF.SelectedValue = "SP"
        radioPJ.Checked = False
        radioCLT.Checked = True
        trPJ1.Style.Add("display", "none")
        trPJ2.Style.Add("display", "none")
        trEmpresaCompartilhada.Style.Add("display", "none")
        divFechado.Style.Add("display", "none")
        txtTelefone.Text = ""
        txtCelular.Text = ""
        txtEmail1.Text = ""
        txtEmail2.Text = ""
        txtDataInicio.Text = ""
        txtDataFim.Text = ""
        txtEmpresaCompartilhada.Text = ""
        txtRazaoSocial.Text = ""
        txtInscrMunicipal.Text = ""
        txtCNPJ.Text = ""
        lblCNPJ.Text = ""
        txtCodigoEmpresaSAP.Text = ""
        cboTipoEmpresa.SelectedValue = ""
        txtBanco.Text = ""
        radioContaP.Checked = False
        radioContaC.Checked = True
        txtAgencia.Text = ""
        txtConta.Text = ""
        txtDigitoConta.Text = ""
        txtValorHoraFixo.Text = ""
        cbValorFechado.Checked = False
        txtObservacao.Value = ""
        txtNome.Focus()
        btnSalvar.Text = "Salvar"
        btnCadastrarNovo.Visible = False
        lblTitulo.Text = "Novo cadastro"
        txtPis.Text = String.Empty
        lblPis.Text = String.Empty
        txtDtDesligamento.Text = String.Empty
        camposEmBranco()
    End Sub

    Sub camposEmBranco()
        txtNome.BackColor = Drawing.Color.White
        txtCodigoColaboradorSAP.BackColor = Drawing.Color.White
        txtLogin.BackColor = Drawing.Color.White
        txtSenhaInicial.BackColor = Drawing.Color.White
        txtNome.BackColor = Drawing.Color.White
        ddlPerfil.BackColor = Drawing.Color.White
        txtDataNascimento.BackColor = Drawing.Color.White
        ddlModulo.BackColor = Drawing.Color.White
        ddlNivel.BackColor = Drawing.Color.White
        txtCPF.BackColor = Drawing.Color.White
        txtRG.BackColor = Drawing.Color.White
        txtEndereco.BackColor = Drawing.Color.White
        txtCEP.BackColor = Drawing.Color.White
        txtBairro.BackColor = Drawing.Color.White
        txtCidade.BackColor = Drawing.Color.White
        txtTelefone.BackColor = Drawing.Color.White
        txtCelular.BackColor = Drawing.Color.White
        txtEmail1.BackColor = Drawing.Color.White
        txtEmail2.BackColor = Drawing.Color.White
        txtDataInicio.BackColor = Drawing.Color.White
        txtDataFim.BackColor = Drawing.Color.White
        txtEmpresaCompartilhada.BackColor = Drawing.Color.White
        txtRazaoSocial.BackColor = Drawing.Color.White
        txtInscrMunicipal.BackColor = Drawing.Color.White
        txtCNPJ.BackColor = Drawing.Color.White
        txtCodigoEmpresaSAP.BackColor = Drawing.Color.White
        txtBanco.BackColor = Drawing.Color.White
        txtAgencia.BackColor = Drawing.Color.White
        txtConta.BackColor = Drawing.Color.White
        txtDigitoConta.BackColor = Drawing.Color.White
        txtValorHoraFixo.BackColor = Drawing.Color.White
        txtPis.BackColor = Drawing.Color.White
        cboTipoEmpresa.BackColor = Drawing.Color.White
        lblCodigoColaboradorSAP.Text = ""
        lblRG.Text = ""
        lblLogin.Text = ""
        txtDtDesligamento.BackColor = Drawing.Color.White
    End Sub

    Private Sub btnSalvar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSalvar.Click

        lblMensagem.Style.Add("color", "Blue")
        lblMensagem.Text = "Processando..."
        txtEmail1.Text = txtEmail1.Text.ToLower
        txtEmail2.Text = txtEmail2.Text.ToLower

        If validarCampos() Then
            If validarCPF_CNPJ() Then
                lblMensagem.Text = ""
                If salvarBancoDados() Then
                    If btnSalvar.Text = "Salvar" Then
                        lblMensagem.Style.Add("color", "Blue")
                        lblMensagem.Text = "Cadastro Salvo com Sucesso"
                        limparFormulario()
                    Else
                        lblMensagem.Style.Add("color", "Blue")
                        lblMensagem.Text = "Cadastro Alterado com Sucesso"
                        btnSalvar.Text = "Salvar"
                        limparFormulario()
                    End If
                End If
            End If
        Else
            lblMensagem.Style.Add("color", "Red")
            lblMensagem.Text = "Os campos em amarelo, ou estão vazios ou preenchidos incorretamente"
        End If

    End Sub

    '=======================================================================================================
    '   Salva no Banco de dados depois de validado o formulário
    '=======================================================================================================
    Private Function salvarBancoDados() As Boolean

        Dim login As String = txtLogin.Text.Trim
        Dim tipoContrato As String
        Dim tipoEmpresa As String
        Dim empresaCompartilhada = "NULL"
        Dim tipoConta As String
        Dim numConta As String = txtConta.Text & "-" & txtDigitoConta.Text
        Dim cpf As String = String.Format("{0:00\.000\.000\-0}", txtCPF.Text.Trim)
        Dim cnpj As String = String.Format("{0:00\.000\.000\/0000\-00}", txtCNPJ.Text.Trim)
        Dim SQL As String
        Dim valor As String = "NULL"
        Dim valorFechado = "N"
        Dim observacao = "NULL"
        Dim codigoEmpresaSAP = "0"
        Dim DataDesli As String = "NULL"

        lblMensagem.Style.Add("color", "Blue")

        ' Prepara o conteudo do campo txtValorHoraFixo para ser usado no insert/update sql
        valor = txtValorHoraFixo.Text.Replace("R$", "").Replace(".", "")
        valor = valor.Replace(",", ".")

        If valor = "" Then
            valor = "NULL"
        End If

        If radioCLT.Checked Then
            tipoContrato = "CLT"
        Else
            tipoContrato = "PJ"
            If cbValorFechado.Checked Then
                valorFechado = "S"
            End If
        End If

        If radioPJ.Checked Then
            tipoEmpresa = cboTipoEmpresa.SelectedValue

            If tipoEmpresa = "Compartilhada" Then
                If txtEmpresaCompartilhada.Text.Trim <> "" Then
                    empresaCompartilhada = "'" + txtEmpresaCompartilhada.Text + "'"
                End If
            End If
            codigoEmpresaSAP = txtCodigoEmpresaSAP.Text
        Else
            txtCNPJ.Text = ""   ' Limpa o que esta preenchido no campo CNPJ
            cnpj = "NULL"       ' Como aqui foi escolhido CLT, então o campo cnpj no banco grava-se nulo
            tipoEmpresa = "NULL"
        End If

        SQL = "SELECT colCPF FROM v_colaboradores WHERE colCPF = '" & cpf & "'"

        ' Verifica se esta adicionando ou alterando algum cadastro para depois verificar a duplicidade de cpf no sistema
        If btnSalvar.Text = "Salvar" Then
            Select Case existeDados(SQL)    ' Verifico se há algum registro conforme o select feito
                Case 1
                    lblMensagem.Style.Add("color", "Red")
                    lblMensagem.Text = "CPF já existente."
                    txtCPF.BackColor = Drawing.Color.Yellow
                    Return False
                Case -1
                    lblTitulo.Text = sqlErro
                    Return False
            End Select
        Else
            If Session("colCPF") <> cpf Then    ' Se caso for diferente, então houve mudança do CPF e checará se já existe nos dados.
                If existeDados(SQL) = 1 Then    ' Verifico se há algum registro conforme o select feito
                    lblMensagem.Style.Add("color", "Red")
                    lblMensagem.Text = "CPF já existente."
                    txtCPF.BackColor = Drawing.Color.Yellow
                    Return False
                End If
            End If
        End If

        If radioContaC.Checked Then
            tipoConta = "Conta Corrente"
        Else
            tipoConta = "Conta Poupança"
        End If

        Dim senha = txtSenhaInicial.Text

        If txtObservacao.Value.Trim <> "" Then
            observacao = txtObservacao.Value
            observacao = observacao.Replace("'", "''")
            observacao = "'" & observacao & "'"
        End If

        If Not txtDtDesligamento.Text = String.Empty Then
            DataDesli = "'" & txtDtDesligamento.Text & "'"

        End If

        If btnSalvar.Text = "Salvar" Then
            SQL =
             "INSERT INTO tblColaboradores (" + vbCrLf +
             "  colLogin,colSenha,colNome,colCodigoSAP,colPerfil,colDataNascimento,colModulo,colNivel,colRG" + vbCrLf +
             "  ,colEndereco,colCEP,colBairro,colCidade,colUF,colTel,colCel,colEmail1,colEmail2" + vbCrLf +
             "  ,colDataInicio,colDataFim,colTipoContrato,colEmpresaCompartilhada,colRazaoSocial,colInscrMunicipal" + vbCrLf +
             "  ,colCPF,colCNPJ,colCodigoEmpresaSAP,colSalario,colValorFechado,colTipoEmpresa" + vbCrLf +
             "  ,colBanco,colTipoConta,colAgencia,colConta,colStatus,colObservacao,perCodigo,colPis,colDataDesli" + vbCrLf +
             ") VALUES (" + vbCrLf +
             "  '" & login & "'" &
             "  ,'" & senha & "'" &
             "  ,'" & txtNome.Text & "'" &
             "  ," & txtCodigoColaboradorSAP.Text & "" &
             "  ,'" & ddlPerfil.SelectedItem.Text & "'" &
             "  ,'" & txtDataNascimento.Text & "'" &
             "  ,'" & ddlModulo.SelectedValue & "'" &
             "  ,'" & ddlNivel.SelectedValue & "'" &
             "  ,'" & txtRG.Text & "'" &
             "  ,'" & txtEndereco.Text & "'" &
             "  ,'" & txtCEP.Text & "'" &
             "  ,'" & txtBairro.Text & "'" &
             "  ,'" & txtCidade.Text & "'" &
             "  ,'" & ddlUF.SelectedValue & "'" &
             "  ,'" & txtTelefone.Text & "'" &
             "  ,'" & txtCelular.Text & "'" &
             "  ,'" & txtEmail1.Text & "'" &
             "  ,'" & txtEmail2.Text & "'" &
             "  ,'" & txtDataInicio.Text & "'" &
             "  ,'" & txtDataFim.Text & "'" &
             "  ,'" & tipoContrato & "'" &
             "  ," & empresaCompartilhada & "" &
             "  ,'" & txtRazaoSocial.Text & "'" &
             "  ,'" & txtInscrMunicipal.Text & "'" &
             "  ,'" & cpf & "'" &
             "  ,'" & cnpj & "'" &
             "  ," & codigoEmpresaSAP & "" &
             "  ," & valor & "" &
             "  ,'" & valorFechado & "'" &
             "  ,'" & tipoEmpresa & "'" &
             "  ,'" & txtBanco.Text & "'" &
             "  ,'" & tipoConta & "'" &
             "  ,'" & txtAgencia.Text & "'" &
             "  ,'" & numConta & "'" &
             "  ,'" & ddlStatus.SelectedValue & "'" &
             "  ," & observacao & "" &
             "  ," & ddlPerfil.SelectedValue & "" &
             "  ,'" & txtPis.Text & "'," &
             DataDesli &
             ")"
        Else
            Dim perCodigo As Integer
            perCodigo = ddlPerfil.SelectedValue

            SQL =
             "UPDATE tblColaboradores SET" + vbCrLf +
             "   colLogin = '" & login & "'" + vbCrLf +
             "  ,colNome = '" & txtNome.Text & "'" + vbCrLf +
             "  ,colCodigoSAP = " & txtCodigoColaboradorSAP.Text & "" + vbCrLf +
             "  ,perCodigo = " & perCodigo & "" + vbCrLf +
             "  ,colPerfil = '" & ddlPerfil.SelectedItem.Text & "'" + vbCrLf +
             "  ,colDataNascimento = '" & txtDataNascimento.Text & "'" + vbCrLf +
             "  ,colModulo = '" & ddlModulo.SelectedValue & "'" + vbCrLf +
             "  ,colNivel = '" & ddlNivel.SelectedValue & "'" + vbCrLf +
             "  ,colRG = '" & txtRG.Text & "'" + vbCrLf +
             "  ,colEndereco = '" & txtEndereco.Text & "'" + vbCrLf +
             "  ,colCEP = '" & txtCEP.Text & "'" + vbCrLf +
             "  ,colBairro = '" & txtBairro.Text & "'" + vbCrLf +
             "  ,colCidade = '" & txtCidade.Text & "'" + vbCrLf +
             "  ,colUF = '" & ddlUF.SelectedValue & "'" + vbCrLf +
             "  ,colTel = '" & txtTelefone.Text & "'" + vbCrLf +
             "  ,colCel = '" & txtCelular.Text & "'" + vbCrLf +
             "  ,colEmail1 = '" & txtEmail1.Text & "'" + vbCrLf +
             "  ,colEmail2 = '" & txtEmail2.Text & "'" + vbCrLf +
             "  ,colDataInicio = '" & txtDataInicio.Text & "'" + vbCrLf +
             "  ,colDataFim = '" & txtDataFim.Text & "'" + vbCrLf +
             "  ,colTipoContrato = '" & tipoContrato & "'" + vbCrLf +
             "  ,colEmpresaCompartilhada = " & empresaCompartilhada & "" + vbCrLf +
             "  ,colRazaoSocial = '" & txtRazaoSocial.Text & "'" + vbCrLf +
             "  ,colInscrMunicipal = '" & txtInscrMunicipal.Text & "'" + vbCrLf +
             "  ,colCPF = '" & cpf & "'" + vbCrLf +
             "  ,colCNPJ = '" & cnpj & "'" + vbCrLf +
             "  ,colCodigoEmpresaSAP = " & codigoEmpresaSAP & "" + vbCrLf +
             "  ,colSalario = " & valor & "" + vbCrLf +
             "  ,colValorFechado = '" & valorFechado & "'" + vbCrLf +
             "  ,colTipoEmpresa = '" & tipoEmpresa & "'" + vbCrLf +
             "  ,colBanco = '" & txtBanco.Text & "'" + vbCrLf +
             "  ,colTipoConta = '" & tipoConta & "'" + vbCrLf +
             "  ,colAgencia = '" & txtAgencia.Text & "'" + vbCrLf +
             "  ,colConta = '" & numConta & "'" + vbCrLf +
             "  ,colStatus = '" & ddlStatus.SelectedValue & "'" + vbCrLf +
             "  ,colPis = '" & txtPis.Text & "'" + vbCrLf +
              "  ,colDataDesli = " & DataDesli & vbCrLf +
             "  ,colObservacao = " & observacao & ""

            ' Se foi preenchido uma senha inicial na alteração de cadastro então é preciso alterar no banco de dados e
            ' atualizar o campo colMudarSenha para com valor = 1
            If btnSalvar.Text = "Alterar Cadastro" Then
                If txtSenhaInicial.Text.Trim <> "" Then
                    SQL += ",colSenha = '" & txtSenhaInicial.Text & "', colMudarSenha = 1"
                End If
            End If

            SQL += " WHERE colCodigo = " & Session("colCodigoPesquisado")

        End If

        If comandoSQL(SQL) Then
            Return True
        Else
            lblMensagem.Style.Add("color", "red")
            lblMensagem.Text = sqlErro
            Return False
        End If

    End Function

    '=======================================================================================================
    '   Verifica se todos os campos obrigatórios estão preenchidos ou se tem algum erro
    '=======================================================================================================
    Private Function validarCampos() As Boolean

        camposEmBranco()

        Session("senha") = txtSenhaInicial.Text

        ' O que estiver como comentário é os campos não obrigatórios 
        Dim erro As Boolean = False

        ' Valida campo nome
        If txtNome.Text.Trim() = "" Then
            erro = True
            txtNome.BackColor = Drawing.Color.Yellow
        Else
            If Session("colNome") <> txtNome.Text Or btnSalvar.Text = "Salvar" Then
                sqlstring = "SELECT colNome FROM v_colaboradores WHERE colNome ='" & txtNome.Text.Trim & "'"
                If selectSQL(sqlstring) Then
                    If dr.HasRows() Then
                        erro = True
                        lbltxtPesquisar.Text = "Este nome já existe, se deseja alterar algum cadastro utilize a caixa de texto de pesquisa"
                        txtNome.BackColor = Drawing.Color.Yellow
                        Return False
                    End If
                Else
                    lblMensagem.Text = sqlErro
                    Return False
                End If
            End If

        End If

        ' Valida o campo Cod. Colaborador
        If Not IsNumeric(txtCodigoColaboradorSAP.Text) Then
            erro = True
            txtCodigoColaboradorSAP.BackColor = Drawing.Color.Yellow
        Else
            If Session("colCodigoSAP") <> txtCodigoColaboradorSAP.Text Or btnSalvar.Text = "Salvar" Then
                sqlstring = "SELECT * FROM v_colaboradores WHERE colCodigoSAP = " & txtCodigoColaboradorSAP.Text.Trim
                If selectSQL(sqlstring) Then
                    If dr.HasRows() Then
                        erro = True
                        lblCodigoColaboradorSAP.Text = "Não é permitido código duplicado."
                        txtCodigoColaboradorSAP.BackColor = Drawing.Color.Yellow
                        Return False
                    End If
                Else
                    lblMensagem.Text = sqlErro
                    Return False
                End If
            End If
        End If

        ' Valida o campo Cod. Empresa no SAP se for PJ somente
        If radioPJ.Checked Then
            If Not IsNumeric(txtCodigoEmpresaSAP.Text) Then
                erro = True
                txtCodigoEmpresaSAP.BackColor = Drawing.Color.Yellow
            End If
        End If

        ' Valida campo txtLogin
        If txtLogin.Text.Trim() = "" Or txtLogin.Text.Trim.Contains(" ") Then
            erro = True
            txtLogin.BackColor = Drawing.Color.Yellow
        Else
            If Session("colNome") <> txtNome.Text Or btnSalvar.Text = "Salvar" Then
                If selectSQL("SELECT * FROM v_colaboradores WHERE colLogin = '" & txtLogin.Text & "'") Then
                    If dr.HasRows Then
                        erro = True
                        txtLogin.BackColor = Drawing.Color.Yellow
                        lblLogin.Text = "Login já existente."
                        Return False
                    End If
                Else
                    lblMensagem.Text = sqlErro
                    Return False
                End If
            End If
        End If

        ' Valida campo txtSenhaInicial
        If btnSalvar.Text = "Salvar" Then
            If txtSenhaInicial.Text.Trim = "" Then
                erro = True
                txtSenhaInicial.BackColor = Drawing.Color.Yellow
            Else
                If txtSenhaInicial.Text.Length < 6 Or txtSenhaInicial.Text.Length > 20 Then
                    erro = True
                    txtSenhaInicial.BackColor = Drawing.Color.Yellow
                End If
            End If
        Else
            If txtSenhaInicial.Text.Trim <> "" Then
                If txtSenhaInicial.Text.Length < 6 Or txtSenhaInicial.Text.Length > 20 Then
                    erro = True
                    txtSenhaInicial.BackColor = Drawing.Color.Yellow
                End If
            End If
        End If

        ' Valida campo Perfil
        If ddlPerfil.SelectedValue = "" Then
            erro = True
            ddlPerfil.BackColor = Drawing.Color.Yellow
        End If

        ' Valida campo Data Aniversario
        If txtDataNascimento.Text <> "" Then
            If Not IsDate(txtDataNascimento.Text) Then
                txtDataNascimento.BackColor = Drawing.Color.Yellow
                erro = True
            End If
        End If

        If ddlPerfil.SelectedIndex = 0 Or ddlPerfil.SelectedValue = "Consultores" Then
            If ddlModulo.SelectedValue = "" Then
                erro = True
                ddlModulo.BackColor = Drawing.Color.Yellow
            End If

            If ddlNivel.SelectedValue = "" Then
                erro = True
                ddlNivel.BackColor = Drawing.Color.Yellow
            End If
        End If

        ' Valida campo RG
        If txtRG.Text.Trim() = "" Then
            erro = True
            txtRG.BackColor = Drawing.Color.Yellow
            If Session("colRG") <> txtRG.Text Or btnSalvar.Text = "Salvar" Then
                If selectSQL("SELECT * FROM v_colaboradores WHERE colRG = '" & txtRG.Text & "'") Then
                    If dr.HasRows Then
                        erro = True
                        txtRG.BackColor = Drawing.Color.Yellow
                        lblRG.Text = "RG já existente."
                        Return False
                    End If
                Else
                    lblMensagem.Text = sqlErro
                    Return False
                End If
            End If
        End If

        If txtTelefone.Text.Trim() = "" Then
            erro = True
            txtTelefone.BackColor = Drawing.Color.Yellow
        End If

        If txtCelular.Text.Trim() = "" Then
            erro = True
            txtCelular.BackColor = Drawing.Color.Yellow
        End If

        ' Validação dos campos Data Inicio e Data Fim, verifica se um é maior que o outro e se estão 
        ' inseridos(corretamente)
        lblMensagem2.Text = ""
        Dim erroData As Boolean = False
        If txtDataInicio.Text.Trim() <> "" Then
            If Not IsDate(txtDataInicio.Text) Then
                lblMensagem2.Text = "Campo 'Data Inicio' está com formato incorreto"
                erroData = True
            End If
        Else
            erroData = True
        End If
        If txtDataFim.Text.Trim() <> "" Then
            If Not IsDate(txtDataFim.Text) Then
                lblMensagem2.Text = "Campo 'Data Fim' está com formato incorreto"
                erroData = True
            End If
        Else
            erroData = True
        End If
        If Not erroData Then
            If DateDiff("d", txtDataInicio.Text, txtDataFim.Text) < 0 Then
                lblMensagem2.Text = "Na data, campo 'de' está mais recente que o campo 'até'"
                erro = True
                txtDataInicio.BackColor = Drawing.Color.Yellow
                txtDataFim.BackColor = Drawing.Color.Yellow
            End If
        Else
            txtDataInicio.BackColor = Drawing.Color.Yellow
            txtDataFim.BackColor = Drawing.Color.Yellow
        End If
        ' Fim de verificação das Datas    ------------------------------------------------------------------------------

        If txtCPF.Text.Trim() = "" Then
            erro = True
            txtCPF.BackColor = Drawing.Color.Yellow
            lblCPF.Text = ""
        Else
            If isCPF(txtCPF.Text) Then
                If Session("colCPF") <> txtCPF.Text Or btnSalvar.Text = "Salvar" Then
                    If selectSQL("SELECT * FROM v_colaboradores WHERE colCPF = '" & txtCPF.Text & "'") Then
                        If dr.HasRows Then
                            erro = True
                            txtCPF.BackColor = Drawing.Color.Yellow
                            lblCPF.Text = "CPF já existente."
                            Return False
                        End If
                    Else
                        lblMensagem.Text = sqlErro
                        Return False
                    End If
                End If
            Else
                erro = True
                txtCPF.BackColor = Drawing.Color.Yellow
                lblCPF.Text = "CPF inválido"
            End If
        End If

        If radioPJ.Checked = True Then
            If txtRazaoSocial.Text.Trim() = "" Then
                erro = True
                txtRazaoSocial.BackColor = Drawing.Color.Yellow
            End If
            If txtInscrMunicipal.Text.Trim() = "" Then
                erro = True
                txtInscrMunicipal.BackColor = Drawing.Color.Yellow
            End If
            If txtCNPJ.Text.Trim() = "" Then
                erro = True
                txtCNPJ.BackColor = Drawing.Color.Yellow
                lblCNPJ.Text = ""
            Else
                If Not isCNPJ(txtCNPJ.Text) Then
                    erro = True
                    txtCNPJ.BackColor = Drawing.Color.Yellow
                    lblCNPJ.Text = "CNPJ inválido"
                End If
            End If

            If cboTipoEmpresa.SelectedValue = "" Then
                erro = True
                cboTipoEmpresa.BackColor = Drawing.Color.Yellow
                lblTipoEmpresa.Text = "Selecione um tipo de empresa."
            End If
        ElseIf radioCLT.Checked Then
            'If String.IsNullOrWhiteSpace(txtPis.Text) Then
            '    erro = True
            '    txtPis.BackColor = Drawing.Color.Yellow
            '    lblPis.Text = String.Empty
            'ElseIf Not ValidarPIS(txtPis.Text) Then
            '    erro = True
            '    txtPis.BackColor = Drawing.Color.Yellow
            '    lblPis.Text = "Pis inválido."

            'End If
            If Not String.IsNullOrWhiteSpace(txtPis.Text) Then
                If Not ValidarPIS(txtPis.Text) Then
                    erro = True
                    txtPis.BackColor = Drawing.Color.Yellow
                    lblPis.Text = "Pis inválido."
                End If
            End If
        Else
            txtRazaoSocial.Text = ""
            txtInscrMunicipal.Text = ""
            txtCNPJ.Text = ""
            txtPis.Text = String.Empty
            cboTipoEmpresa.SelectedValue = ""
        End If

        ' Prepara o valor hora/fixo que foi inserido
        Try
            If txtValorHoraFixo.Text.Trim = "" Then
                erro = True
                txtValorHoraFixo.BackColor = Drawing.Color.Yellow
                Exit Try
            End If
            txtValorHoraFixo.Text = txtValorHoraFixo.Text.Replace("R$", "")
            txtValorHoraFixo.Text = Convert.ToDouble(txtValorHoraFixo.Text).ToString("C2")
        Catch ex As Exception
            erro = True
            txtValorHoraFixo.BackColor = Drawing.Color.Yellow
        End Try

        If txtBanco.Text.Trim() = "" Then
            erro = True
            txtBanco.BackColor = Drawing.Color.Yellow
        End If

        If txtAgencia.Text.Trim() = "" Then
            erro = True
            txtAgencia.BackColor = Drawing.Color.Yellow
        End If

        If txtConta.Text.Trim() = "" Or txtDigitoConta.Text = "" Then
            erro = True
            txtConta.BackColor = Drawing.Color.Yellow
            txtDigitoConta.BackColor = Drawing.Color.Yellow
        End If

        If Not erro Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function validarCPF_CNPJ()

        If isCPF(txtCPF.Text) Then
            lblMensagem.Text = ""
            Return True
        Else
            lblMensagem.Style.Add("color", "Red")
            lblMensagem.Text = "CPF invalido."
            txtCPF.BackColor = Drawing.Color.Yellow
            Return False
        End If

        If radioPJ.Checked Then
            ' Valida se o CNPJ é valido
            If isCNPJ(txtCNPJ.Text) Then
                lblMensagem.Text = ""
                Return True
            Else
                lblMensagem.Style.Add("color", "Red")
                lblMensagem.Text = "CNPJ invalido."
                txtCNPJ.BackColor = Drawing.Color.Yellow
                Return False
            End If
        End If

    End Function

    '=================================================================================================================
    '  Faz um select e preenche todo o formulario
    '=================================================================================================================
    Private Sub btnPesquisar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPesquisar.Click

        Dim strPesquisar As String = txtPesquisar.Text

        limparFormulario()
        lblMensagem.Text = ""

        If strPesquisar.Trim <> "" Then
            sqlstring = "SELECT * FROM v_colaboradores WHERE colNome COLLATE Latin1_General_CI_AI LIKE '%" & strPesquisar & "%'"
            If selectSQL(sqlstring) Then
                If dr.Read() Then
                    Try
                        Session("colCodigoPesquisado") = dr("colCodigo")
                        'Gravo em sessão o perfil do usuário para depois comparar se foi alterado o perfil na alteração do cadastro
                        Session("colPerfil") = dr("colPerfil")

                        txtNome.Text = dr("colNome")
                        txtCodigoColaboradorSAP.Text = dr("colCodigoSAP")
                        txtLogin.Text = dr("colLogin")

                        Dim perfil As Integer = dr("perCodigo")
                        ddlPerfil.SelectedValue = perfil
                        'ddlPerfil_indiceTrocado(perfil)

                        If dr("colDataNascimento") IsNot DBNull.Value Then
                            txtDataNascimento.Text = dr("colDataNascimento")
                        End If

                        ddlModulo.SelectedValue = dr("colModulo")
                        ddlNivel.Text = dr("colNivel")
                        txtRG.Text = dr("colRG")
                        txtEndereco.Text = dr("colEndereco")
                        txtCEP.Text = dr("colCEP")
                        txtBairro.Text = dr("colBairro")
                        txtCidade.Text = dr("colCidade")
                        ddlUF.Text = dr("colUF")
                        txtTelefone.Text = dr("colTel")
                        txtCelular.Text = dr("colCel")

                        If dr("colEmail1") IsNot DBNull.Value Then
                            txtEmail1.Text = dr("colEmail1")
                        End If

                        If dr("colEmail2") IsNot DBNull.Value Then
                            txtEmail2.Text = dr("colEmail2")
                        End If

                        txtDataInicio.Text = dr("colDataInicio")
                        txtDataFim.Text = dr("colDataFim")

                        Try
                            If dr("colCPF").ToString.Length < 11 Then
                                txtCPF.Text = "0" & dr("colCPF")
                            End If
                            txtCPF.Text = dr("colCPF")
                        Catch ex As Exception
                            txtCPF.Text = ""
                        End Try

                        Try
                            Dim valor = Convert.ToDouble(dr("colSalario")).ToString("C2")
                            txtValorHoraFixo.Text = valor
                        Catch ex As Exception
                            txtValorHoraFixo.Text = ""
                        End Try

                        If dr("colTipoContrato") = "CLT" Then
                            radioCLT.Checked = True
                            trPJ1.Style.Add("display", "none")
                            trPJ2.Style.Add("display", "none")
                            divFechado.Style.Add("display", "none")
                            tbClt.Style.Add("display", "block")
                            If dr("colPis") IsNot DBNull.Value Then txtPis.Text = dr("colPis")
                        Else
                            radioPJ.Checked = True
                            tbClt.Style.Add("display", "none")
                            trPJ1.Style.Add("display", "block")
                            trPJ2.Style.Add("display", "block")
                            divFechado.Style.Add("display", "block")
                            txtRazaoSocial.Text = dr("colRazaoSocial")
                            txtInscrMunicipal.Text = dr("colInscrMunicipal")

                            If dr("colCNPJ") IsNot DBNull.Value Then
                                If dr("colCNPJ").ToString.Length < 14 Then
                                    Dim cnpj As String = dr("colCNPJ").ToString
                                    cnpj = "0" & cnpj
                                    txtCNPJ.Text = cnpj
                                End If
                                txtCNPJ.Text = dr("colCNPJ")
                            End If

                            txtCodigoEmpresaSAP.Text = dr("coLCodigoEmpresaSAP")

                            cboTipoEmpresa.SelectedValue = dr("colTipoEmpresa")

                            If dr("colTipoEmpresa") = "Compartilhada" Then
                                trEmpresaCompartilhada.Style.Add("display", "block")
                                If dr("colEmpresaCompartilhada") IsNot DBNull.Value Then
                                    txtEmpresaCompartilhada.Text = dr("colEmpresaCompartilhada")
                                End If
                            End If
                        End If

                        If dr("colSalario") IsNot DBNull.Value Then
                            If dr("colValorFechado") = "S" Then
                                cbValorFechado.Checked = True
                            Else
                                cbValorFechado.Checked = False
                            End If
                        End If

                        If dr("colObservacao") IsNot DBNull.Value Then
                            txtObservacao.Value = dr("colObservacao")
                        End If

                        If dr("colDataDesli") IsNot DBNull.Value Then
                            txtDtDesligamento.Text = dr("colDataDesli")
                        End If

                        txtBanco.Text = dr("colBanco")
                        If dr("colTipoConta") = "Conta Corrente" Then
                            radioContaC.Checked = True
                        Else
                            radioContaP.Checked = True
                        End If
                        txtAgencia.Text = dr("colAgencia")
                        Dim conta As String = dr("colConta")
                        txtConta.Text = conta.Remove(conta.IndexOf("-"))
                        txtDigitoConta.Text = conta.Remove(0, conta.IndexOf("-") + 1)
                        ddlStatus.SelectedValue = dr("colStatus")
                        btnSalvar.Text = "Alterar Cadastro"
                        btnCadastrarNovo.Visible = True
                        asteriscoSenha.Visible = False
                        lblTitulo.Text = "Alteração de cadastro - '" & dr("colNome") & "'"
                        txtNome.Focus()
                    Catch ex As Exception
                        lbltxtPesquisar.Text = ex.Message
                        Return
                    End Try
                Else
                    Dim nome As String = txtNome.Text
                    lbltxtPesquisar.Text = "Não foi encontrado registros com este nome"
                    Return
                End If
            End If
        Else
            lbltxtPesquisar.Text = "Campo está vazio"
        End If

        ' Gravo em Sessão dados que utilizo para comparar se houve mudanças, se houver preciso checar duplicidade
        Session("colCPF") = txtCPF.Text
        Session("colNome") = txtNome.Text
        Session("colLogin") = txtLogin
        Session("colCodigoSAP") = txtCodigoColaboradorSAP.Text
        Session("colRG") = txtRG.Text

        showHideTrValor()

    End Sub

    Private Sub btnCadastrarNovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCadastrarNovo.Click
        asteriscoSenha.Style.Add("display", "block")
        limparFormulario()
    End Sub

    Private Sub txtValorFixo_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtValorHoraFixo.TextChanged
        Try
            txtValorHoraFixo.Text = Convert.ToDouble(txtValorHoraFixo.Text).ToString("C2")
            txtValorHoraFixo.BackColor = Drawing.Color.White
        Catch ex As Exception
            txtValorHoraFixo.BackColor = Drawing.Color.Yellow
        End Try
    End Sub

    Protected Sub cboTipoEmpresa_SelectedIndexChanged(sender As Object, e As EventArgs)
        If cboTipoEmpresa.SelectedValue = "Compartilhada" Then
            trEmpresaCompartilhada.Style.Add("display", "block")
        Else
            trEmpresaCompartilhada.Style.Add("display", "none")
        End If

    End Sub
End Class