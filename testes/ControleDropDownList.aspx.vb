Imports System.Data.SqlClient

Partial Public Class ControleDropDownList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'atualizaModCodigo()
        atualizarPerCodigo()

    End Sub

    ' Atualiza o campo recem criado modCodigo da tabela tblColaboradores
    Private Sub atualizaModCodigo()

        Dim SQL As String
        Dim vetorModulo As ArrayList = New ArrayList
        Dim vetorCodigo As ArrayList = New ArrayList

        SQL = "SELECT colCodigo, colModulo FROM v_colaboradores"

        If selectSQL(SQL) Then
            While dr.Read()
                vetorModulo.Add(dr("colModulo"))
                vetorCodigo.Add(dr("colCodigo"))
            End While
            For i = 0 To (vetorModulo.Count - 1)
                SQL = "UPDATE tblColaboradores SET " & _
                    "modCodigo = (SELECT modCodigo FROM v_modulos WHERE modDescricao = '" & vetorModulo(i) & "') " & _
                    "WHERE colCodigo = " & vetorCodigo(i) & ";"
                If Not comandoSQL(SQL) Then
                    lblMensagem.Text = sqlErro
                    Exit For
                End If
            Next i
        Else
            lblMensagem.Text = sqlErro
        End If

    End Sub

    ' Atualiza o campo recem criado perCodigo da tabela tblColaboradores_Projetos
    Private Sub atualizarPerCodigo()

        Dim SQL As String
        Dim vetor As ArrayList = New ArrayList

        SQL = "SELECT colCodigo FROM v_colaboradores_Projeto"

        If selectSQL(SQL) Then
            While dr.Read()
                vetor.Add(dr("colCodigo"))
            End While
            For i = 0 To (vetor.Count - 1)
                SQL = "UPDATE tblColaboradores_Projetos SET " & _
                    "perCodigoNesteProjeto = (SELECT perCodigo FROM v_colaboradores WHERE colCodigo = " & vetor(i) & ") " & _
                    "WHERE colCodigo = " & vetor(i) & ";"
                If Not comandoSQL(SQL) Then
                    lblMensagem.Text = sqlErro
                    Exit For
                End If
            Next i
        Else
            lblMensagem.Text = sqlErro
        End If

    End Sub

End Class