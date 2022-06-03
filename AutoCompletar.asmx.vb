Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<System.Web.Script.Services.ScriptService()> _
<ToolboxItem(False)> _
Public Class AutoCompletar
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function getNomesColaboradores(ByVal prefixText As String, ByVal count As Integer) As String()

        Dim SQL As String
        Dim itens As New List(Of String)

        SQL = "SELECT TOP " & count & " colCodigo, colNome FROM v_colaboradores " & _
              "WHERE colNome COLLATE Latin1_General_CI_AI LIKE '%" & prefixText & "%' " & _
              "ORDER BY colNome"

        If selectSQL(SQL) Then
            If dr.HasRows Then
                While dr.Read()
                    itens.Add(dr("colNome"))
                End While
            End If
        Else
            itens.Add(sqlErro)
        End If

        Return itens.ToArray()

    End Function

    <WebMethod()> _
    Public Function getNomesProjetos(ByVal prefixText As String, ByVal count As Integer) As String()

        Dim SQL As String
        Dim itens As New List(Of String)

        SQL = "SELECT TOP " & count & " proCodigo, proNome FROM v_projetos " & _
              "WHERE proNome COLLATE Latin1_General_CI_AI LIKE '%" & prefixText & "%' " & _
              "ORDER BY proNome"

        If selectSQL(SQL) Then
            If dr.HasRows Then
                While dr.Read()
                    itens.Add(dr("proNome"))
                End While
            End If
        Else
            itens.Add(sqlErro)
        End If

        Return itens.ToArray()

    End Function

    <WebMethod()> _
    Public Function getNomesRecursos(ByVal prefixText As String, ByVal count As Integer) As String()

        Dim SQL As String
        Dim itens As List(Of String) = New List(Of String)(count)

        SQL = "SELECT TOP " & count & " recNome FROM v_recursos " & _
              "WHERE recNome COLLATE Latin1_General_CI_AI LIKE '%" & prefixText & "%' " & _
              "GROUP BY recNome ORDER BY recNome"

        dr.Close()

        If selectSQL(SQL) Then
            If dr.HasRows Then
                While dr.Read()
                    itens.Add(dr("recNome"))
                End While
            End If
        Else
            itens.Add(sqlErro)
        End If

        Return itens.ToArray()

    End Function

End Class