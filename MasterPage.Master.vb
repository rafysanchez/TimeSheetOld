Public Partial Class MasterPage
    Inherits System.Web.UI.MasterPage

    Public Class masterPageTeste

        Shared exibeTitulo As String

        Public Shared Property titulo() As String
            Get
                Return exibeTitulo
            End Get
            Set(ByVal Value As String)
                exibeTitulo = Value
            End Set
        End Property

    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'masterPageTeste.titulo = "Teste de titulo de página"
        'lblBoasVindas.Text = masterPageTeste.titulo

        ' HiperLink do Log-out
        hlLogout.Visible = False

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

        ' Páginas no sistema, desabilitadas inicialmente, logo abaixo habilitadas conforme perfil
        lnkVisualizarApontamento.Visible = False
        lnkApontamento.Visible = False
        lnkCadastroColaborador.Visible = False
        lnkCadastroProjeto.Visible = False
        lnkGerenciaProjeto.Visible = False
        lnkPesquisa.Visible = False
        lnkAprovacao.Visible = False
        lnkRelatorio.Visible = False
        lnkConfig.Visible = False
        lnkDespesas.Visible = False
        menuCadastros.Visible = False
        menuLancamentos.Visible = False
        menuFechamento.Visible = False
        lblBoasVindas.Text = ""

        If Session("colNomeLogado") <> "" Then

            lblBoasVindas.Text = Session("colNomeLogado") & " - " & Session("colPerfilLogado")
            lblBoasVindas.Text += ""

            hlLogout.Visible = True

            Select Case Session("perCodigoLogado")
                Case 1, 9 '"consultores" , Analistas
                    menuLancamentos.Visible = True
                    lnkApontamento.Visible = True
                    'lnkDespesas.Visible = True
                Case 2 '"gerente de projeto"
                    lnkApontamento.Visible = True
                    'lnkDespesas.Visible = True
                    lnkVisualizarApontamento.Visible = True
                    lnkCadastroColaborador.Visible = True
                    lnkCadastroProjeto.Visible = True
                    lnkGerenciaProjeto.Visible = True
                    menuFechamento.Visible = True
                    lnkAprovacao.Visible = True
                    lnkRelatorio.Visible = True
                    menuCadastros.Visible = True
                    menuLancamentos.Visible = True
                Case 3 '"gerente de contas"
                    lnkApontamento.Visible = True
                    'lnkDespesas.Visible = True
                    lnkVisualizarApontamento.Visible = True
                    lnkCadastroColaborador.Visible = True
                    lnkCadastroProjeto.Visible = True
                    lnkGerenciaProjeto.Visible = True
                    lnkAprovacao.Visible = True
                    menuFechamento.Visible = True
                    lnkRelatorio.Visible = True
                    menuCadastros.Visible = True
                    menuLancamentos.Visible = True
                Case 4 '"diretoria"
                    lnkApontamento.Visible = True
                    lnkVisualizarApontamento.Visible = True
                    lnkCadastroColaborador.Visible = True
                    lnkCadastroProjeto.Visible = True
                    lnkGerenciaProjeto.Visible = True
                    lnkPesquisa.Visible = True
                    lnkAprovacao.Visible = True
                    menuFechamento.Visible = True
                    lnkRelatorio.Visible = True
                    lnkConfig.Visible = True
                    menuCadastros.Visible = True
                    menuLancamentos.Visible = True
                Case 5 '"csc"
                    lnkVisualizarApontamento.Visible = True
                    lnkCadastroColaborador.Visible = True
                    lnkCadastroProjeto.Visible = True
                    lnkGerenciaProjeto.Visible = True
                    menuFechamento.Visible = True
                    lnkRelatorio.Visible = True
                    menuCadastros.Visible = True
                    menuLancamentos.Visible = True
                Case 6 '"recrutamento"
                    lnkApontamento.Visible = True
                    'lnkDespesas.Visible = True
                    lnkPesquisa.Visible = True
                    lnkCadastroColaborador.Visible = True
                    menuCadastros.Visible = True
                    menuLancamentos.Visible = True
                Case 7 '"administrador"
                    lnkApontamento.Visible = True
                    'lnkDespesas.Visible = True
                    lnkVisualizarApontamento.Visible = True
                    lnkCadastroColaborador.Visible = True
                    lnkCadastroProjeto.Visible = True
                    lnkGerenciaProjeto.Visible = True
                    lnkPesquisa.Visible = True
                    lnkAprovacao.Visible = True
                    menuFechamento.Visible = True
                    lnkConfig.Visible = True
                    menuCadastros.Visible = True
                    menuLancamentos.Visible = True
                Case 8 '"funcionário"
                    menuLancamentos.Visible = True
                    lnkApontamento.Visible = True
                    'lnkDespesas.Visible = True
            End Select
        Else
            'Response.Redirect("Default.aspx")
        End If

    End Sub

End Class