Public Class despesas
    Inherits System.Web.UI.Page

    Dim SQL As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

#If DEBUG Then
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        ' Select para teste, simula que algum consultor já tenha feito o login
        'Session("colCodigoLogado") = 65
        SQL = "SELECT colNome, colPerfil, perCodigo FROM v_colaboradores WHERE colCodigo = " & Session("colCodigoLogado")
        If selectSQL(Sql) Then
            If dr.HasRows Then
                dr.Read()
                Session("colPerfilLogado") = dr("colPerfil")
                Session("colNomeLogado") = dr("colNome")
                Session("perCodigoLogado") = dr("perCodigo")
            Else
                lblMensagem.Text = "Não há dados com o código de colaborador informado... código = " & Session("colCodigoLogado")
            End If
        Else
            lblMensagem.Text = sqlErro
        End If
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#Else
        Try
            If Not Session("permissao").ToString.Contains("Apontamento Novo") Then
                Response.Redirect("..\Default.aspx")
            End If
        Catch ex As Exception
            Response.Redirect("..\Default.aspx")
        End Try
#End If

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

    End Sub

End Class