Public Partial Class exemplo
    Inherits System.Web.UI.Page

    Dim SQL As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'atualizaAssociacaoColaboradoresProjetos()

    End Sub



    Private Sub atualizaAssociacaoColaboradoresProjetos()

        Dim arrayProCodigo As ArrayList = New ArrayList
        Dim arrayColCodigo As ArrayList = New ArrayList
        Dim arrayPerCodigo As ArrayList = New ArrayList

        SQL = "SELECT tblColaboradores.perCodigo, tblColaboradores_Projetos.proCodigo, tblColaboradores_Projetos.colCodigo " & _
              "FROM dbIntranet.dbo.tblColaboradores_Projetos tblColaboradores_Projetos " & _
              "INNER JOIN dbIntranet.dbo.tblColaboradores tblColaboradores " & _
              "ON (tblColaboradores_Projetos.colCodigo = tblColaboradores.colCodigo)"

        If Not selectSQL(SQL) Then
            lblMensagem.Text = sqlErro
            Return
        End If

        While dr.Read
            arrayColCodigo.Add(dr("colCodigo"))
            arrayProCodigo.Add(dr("proCodigo"))
            arrayPerCodigo.Add(dr("perCodigo"))
        End While

        For i = 0 To (arrayColCodigo.Count - 1)
            Select Case arrayPerCodigo(i)
                Case 2 ' Gerente de Projeto
                    ' deleta a linha e adiciona no campo codGP da tabela projetos
                    'SQL = "DELETE tblColaboradores_projetos WHERE colCodigo = " & arrayColCodigo(i) & " AND " & _
                    '      "proCodigo = " & arrayProCodigo(i) & "; "
                    SQL += "UPDATE tblProjetos SET " & _
                          "codGP = " & arrayColCodigo(i) & " " & _
                          "WHERE proCodigo = " & arrayProCodigo(i) & "; "
                    If Not comandoSQL(SQL) Then
                        lblMensagem.Text = sqlErro
                    End If
                Case 3 ' Gerente de Contas
                    ' deleta a linha e adiciona no campo codGP da tabela projetos
                    'SQL = "DELETE tblColaboradores_projetos WHERE colCodigo = " & arrayColCodigo(i) & " AND " & _
                    '      "proCodigo = " & arrayProCodigo(i) & "; "
                    SQL += "UPDATE tblProjetos SET " & _
                          "codGC = " & arrayColCodigo(i) & " " & _
                          "WHERE proCodigo = " & arrayProCodigo(i) & "; "
                    If Not comandoSQL(SQL) Then
                        lblMensagem.Text = sqlErro
                    End If
                Case 4 ' Diretoria
                    ' deleta a linha e adiciona no campo codGP da tabela projetos
                    'SQL = "DELETE tblColaboradores_projetos WHERE colCodigo = " & arrayColCodigo(i) & " AND " & _
                    '      "proCodigo = " & arrayProCodigo(i) & "; "
                    SQL += "UPDATE tblProjetos SET " & _
                          "codDir = " & arrayColCodigo(i) & " " & _
                          "WHERE proCodigo = " & arrayProCodigo(i) & "; "
                    If Not comandoSQL(SQL) Then
                        lblMensagem.Text = sqlErro
                    End If
            End Select
        Next i

    End Sub

End Class