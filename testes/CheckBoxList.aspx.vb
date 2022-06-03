Partial Public Class CheckBoxList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        lblMensagem.Text = "Itens Selecionados:"

        If Not IsPostBack Then
            cblProjetos.Items.Add(New ListItem("Teste 1", "1"))
            cblProjetos.Items.Add(New ListItem("Teste 2", "2"))
            cblProjetos.Items.Add(New ListItem("Teste 3", "3"))
            cblProjetos.Items(0).Selected = True
        End If

    End Sub

    Private Sub btnApontar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnApontar.Click
        Dim s As String
        Dim i As Integer
        s = "Itens Selecionados:<br />"
        For i = 0 To cblProjetos.Items.Count - 1
            If cblProjetos.Items(i).Selected Then
                s = s & cblProjetos.Items(i).Value & "<br />"
            End If
        Next i
        lblMensagem.Text = s
    End Sub

End Class