Public Partial Class boasVindas1
    Inherits System.Web.UI.Page

    Dim SQL As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

#If Not DEBUG Then
        Try
            If Session("perCodigoLogado").ToString = "" Then
                Response.Redirect("sair.aspx")
            End If
        Catch ex As Exception
            Response.Redirect("sair.aspx")
        End Try
#Else
        ' Select para teste, simula que algum consultor já tenha feito o login
        'Session("colCodigoLogado") = 122
        SQL = "SELECT tblColaboradores.colNome, tblPerfil.perDescricao " & _
              "FROM tblColaboradores " & _
              "INNER JOIN tblPerfil " & _
              "ON (tblColaboradores.perCodigo = tblPerfil.perCodigo) " & _
              "WHERE colCodigo = " & Session("colCodigoLogado") & ""
        If selectSQL(SQL) Then
            If dr.HasRows Then
                dr.Read()
                Session("colPerfilLogado") = dr("perDescricao")
                Session("colNomeLogado") = dr("colNome")
            End If
        End If
#End If

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

    End Sub

End Class