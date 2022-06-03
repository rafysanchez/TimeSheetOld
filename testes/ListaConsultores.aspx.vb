Public Partial Class ListaConsultores
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim SQL = "SELECT colNome FROM v_colaboradores WHERE NOT colPerfil = 'Diretoria' ORDER BY colNome"
        If selectSQL(SQL) Then
            While dr.Read()
                lblConsultores.Text += dr("colNome") & "<br />"
            End While
        Else
            lblConsultores.Text = sqlErro
        End If
    End Sub

End Class