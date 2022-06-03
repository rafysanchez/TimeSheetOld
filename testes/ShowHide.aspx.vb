Partial Public Class WebForm1

    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        radioAparece.Attributes.Add("onclick", "this.value=apareceTexto1()")
        radioDesaparece.Attributes.Add("onclick", "this.value=apareceTexto2()")

    End Sub

    
    Protected Sub btn1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btn1.Click
        If myDiv.Visible Then
            myDiv.Visible = False
        Else
            myDiv.Visible = True
        End If

        txt2.Text = DateTime.Now()
    End Sub

End Class