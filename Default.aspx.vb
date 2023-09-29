Partial Public Class _Default

    Inherits System.Web.UI.Page

    Dim loginUsuario As String
    Dim senhaDigitada As String
    Dim senhaAtual As String
    Dim SQL As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

        ' Retire o comentario do comando a seguir para deixar o site como em manutenção
        'Response.Redirect("paginaManutencao/Default.aspx")

        If Not IsPostBack Then

            txtLogin.Focus()

            tabSenha.Style.Add("display", "block")
            tabNovaSenha.Style.Add("display", "none")
            tabConfirmaSenha.Style.Add("display", "none")
            tabMensagem.Style.Add("display", "none")
            lnkOutroUser.Style.Add("display", "none")

            txtLogin.Enabled = True
            txtSenha.Enabled = True
            btnLogin.Text = "Login"

        End If

#If Not Debug Then

        Try
            If Session("permissao").ToString.Contains("Logado") Then
                Response.Redirect("~/boasVindas.aspx")
            End If
        Catch ex As Exception
        End Try

#End If

        lblMensagem.Text = ""

    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin.Click

        loginUsuario = txtLogin.Text.Trim()
        senhaDigitada = txtSenha.Text.Trim()

        ''''''''''''''''''''''''''''''''''''''''''''''''''
        ' Usuário somente para cadastro de colaboradores
        ''''''''''''''''''''''''''''''''''''''''''''''''''
        If loginUsuario = "master" And senhaDigitada = "senhamaster" Then
            Session("colNomeLogado") = "Master"
            Session("colPerfilLogado") = "Administrador do Sistema"
            Session("permissao") = "Logado, Visualizar Apontamento, Visualizar, " & _
                    "Cadastro de Colaborador, Cadastro de Projeto, Gerencia de Projeto, Aprovacao, Configuracoes, Fechamento, Pesquisa"
            Response.Redirect("boasVindas.aspx")
            Exit Sub
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''
        ''''''''''''''''''''''''''''''''''''''''''''''''''

        If txtSenha.Enabled Then
            If loginUsuario = "" Or senhaDigitada = "" Then
                lblMensagem.Text = "Informar Login e Senha."
                Return
            End If
        Else
            If validarMudancaSenha() Then
                entrar(loginUsuario, txtNovaSenha.Text)
            Else
                Return
            End If
        End If

        Select Case autenticarUsuario(loginUsuario, senhaDigitada)
            Case "0"        'Login e Senha encontrados
                dr.Read()
                If dr("colStatus").ToString.ToLower = "ativo" Then
                    Session("colCodigoLogado") = dr("colCodigo")
                    senhaAtual = dr("colSenha")
                    If dr("colMudarSenha") = 1 Then
                        If tabNovaSenha.Style("display") = "none" Then
                            tabSenha.Style.Add("display", "none")
                            tabNovaSenha.Style.Add("display", "block")
                            tabConfirmaSenha.Style.Add("display", "block")
                            tabMensagem.Style.Add("display", "block")
                            lnkOutroUser.Style.Add("display", "block")

                            btnLogin.Text = "Alterar e entrar"
                            txtLogin.Enabled = False
                            txtSenha.Enabled = False
                            txtNovaSenha.Focus()
                            lblMensagem.Text = ""
                        Else
                            If Not validarMudancaSenha() Then
                                Return
                            End If
                            entrar(loginUsuario, txtNovaSenha.Text)
                        End If
                        Return
                    End If
                    entrar(loginUsuario, senhaDigitada)
                Else
                    lblMensagem.Text = "Usuário bloqueado."
                End If
            Case "1"        'Login e Senha não encontrados.
                lblMensagem.Text = "Usuário e senha inválidos."
                txtLogin.Text = ""
                txtLogin.Focus()
            Case "2"        'Erro com conexão do banco de dados.
                lblMensagem.Text = "Erro: " & sqlErro
        End Select

    End Sub

    Private Function validarMudancaSenha() As Boolean

        If txtNovaSenha.Text = "" Or txtConfirmSenha.Text = "" Then
            lblMensagem.Text = "Deve-se preencher os dois campos."
            txtNovaSenha.Focus()
            Return False
        End If
        If Not txtNovaSenha.Text = txtConfirmSenha.Text Then
            lblMensagem.Text = "Senhas não coincidem."
            txtNovaSenha.Focus()
            Return False
        End If
        If txtNovaSenha.Text.Length < 6 Or txtNovaSenha.Text.Length > 20 Then
            lblMensagem.Text = "Senhas deve conter no mínimo 6 e no máximo 20 caracteres."
            txtNovaSenha.Focus()
            Return False
        End If
        If txtNovaSenha.Text.Contains(" ") Then
            lblMensagem.Text = "Senhas não devem conter espaços."
            txtNovaSenha.Focus()
            Return False
        End If

        ' Senha nova validada, atualiza-a no banco de dados e prossegue com o sistema. 
        SQL = "UPDATE tblColaboradores SET " & _
                "colMudarSenha = 0, colSenha = '" & txtNovaSenha.Text & "' WHERE colCodigo = " & Session("colCodigoLogado")
        If Not comandoSQL(SQL) Then
            lblMensagem.Text = sqlErro
            Return False
        End If

        lblMensagem.Text = "Senha alterada com sucesso."
        Return True

    End Function

    Private Sub entrar(ByVal loginUsuario, ByVal senhaUsuario)




        Select Case autenticarUsuario(loginUsuario, senhaUsuario)
            Case "0"
                dr.Read()

                ' Verifica se usuário logado esta como Ativo, se não, impede entrada no sistema
                If dr("colStatus").ToString.ToLower = "ativo" Then

                    Session("colNomeLogado") = dr("colNome")
                    Session("colPerfilLogado") = dr("colPerfil")
                    Session("perCodigoLogado") = dr("perCodigo")
                    Session("colCodigoLogado") = dr("colCodigo")
                    Session("permissao") = ""

                    ' Atribui as permissões de acesso as páginas conforme o perfil do usuário
                    Select Case Session("perCodigoLogado")

                        Case 1 ' "consultores"
                            Session("permissao") = "Logado, Apontamento Novo"

                        Case 2 ' "gerente de Projetos"
                            Session("permissao") = "Logado, Apontamento Novo, Visualizar Apontamento, Cadastro de Colaborador, " & _
                            "Cadastro de Projeto, Gerencia de Projeto, Aprovacao, Fechamento, Relatorio"

                        Case 3 ' "gerente de contas"
                            Session("permissao") = "Logado, Apontamento Novo, Visualizar Apontamento, Cadastro de Colaborador, " & _
                            "Cadastro de Projeto, Gerencia de Projeto, Aprovacao, Fechamento, Relatorio"

                        Case 4 ' "diretoria"
                            Session("permissao") = "Logado, Visualizar Apontamento, " & _
                            "Cadastro de Colaborador, Cadastro de Projeto, Gerencia de Projeto, Aprovacao, Configuracoes, Fechamento" & _
                            "Relatorio, Pesquisa"

                        Case 5 ' "csc"
                            Session("permissao") = "Logado, Visualizar Apontamento, Cadastro de Colaborador, Cadastro de Projeto, " & _
                            "Gerencia de Projeto, Fechamento, Relatorio"

                        Case 6 ' "recrutamento"
                            Session("permissao") = "Logado, Apontamento Novo, Cadastro de Colaborador, Pesquisa"

                        Case 7 ' "administrador"
                            Session("permissao") = "Logado, Apontamento Novo, Visualizar Apontamento, Visualizar, " & _
                             "Cadastro de Colaborador, Cadastro de Projeto, Gerencia de Projeto, Aprovacao, Configuracoes, Fechamento" & _
                             "Pesquisa"
                        Case 8 ' "funcionários"
                            Session("permissao") = "Logado, Apontamento Novo"

                    End Select

                    '' Redireciona para próxima página.
                    lblMensagem.Text = "Bem-vindo."
                    Session.Timeout = 60
                    Response.Redirect("boasVindas.aspx")
                Else
                    lblMensagem.Text = "Usuário bloqueado."
                End If

            Case "1"        'Login e Senha não encontrados
                lblMensagem.Text = "Usuário e senha inválidos."
                txtLogin.Text = ""
                txtLogin.Focus()
            Case "2"        'Erro com conexão do banco de dados.
                lblMensagem.Text = "Erro: " & sqlErro
        End Select

    End Sub

    Protected Sub lnkOutroUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkOutroUser.Click
        outroUsuario()
    End Sub

    Private Sub outroUsuario()

        tabSenha.Style.Add("display", "block")
        tabNovaSenha.Style.Add("display", "none")
        tabConfirmaSenha.Style.Add("display", "none")
        tabMensagem.Style.Add("display", "none")
        lnkOutroUser.Style.Add("display", "none")

        txtLogin.Enabled = True
        txtSenha.Enabled = True
        btnLogin.Text = "Login"

        txtLogin.Text = ""
        txtLogin.Focus()

        Session.Clear()

    End Sub

End Class