Imports System.Data.OleDb
Imports System.Data.SqlClient

Public Class lendoArquivoXLSX
    Inherits System.Web.UI.Page

    Private sqlComando As OleDbCommand
    Private drExcel As OleDbDataReader
    Private sqlConn As New OleDbConnection
    Dim caminhoArquivo As String = "c:\BD\lista_cep.xls"
    Protected connString_Excel As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + caminhoArquivo + "';Extended Properties=""Excel 12.0"""
    Dim SQL As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim dt As New DataTable("listaCEP")
        dt.Columns.Add("cep")
        dt.Columns.Add("endereco")

        SQL = "SELECT [cep], [logradouro], [rua], [ruaDescricao], [bairro] FROM [Plan1$] WHERE [codLocalidade] <> 4"

        If selectSQLExcel(SQL) Then
            If drExcel.HasRows Then

                While drExcel.Read

                    Dim cep = ""
                    Dim logradouro = ""
                    Dim rua = ""
                    Dim ruaDescricao = ""
                    Dim bairro = ""
                    Dim endereco = ""

                    cep = drExcel("cep")
                    cep = "0" & cep

                    logradouro = drExcel("logradouro") & " " & drExcel("rua") & ", "

                    If drExcel("ruaDescricao").ToString.ToUpper <> "NULL" Then
                        ruaDescricao = drExcel("ruaDescricao") & ", "
                    End If

                    bairro = drExcel("bairro")

                    endereco = logradouro & ruaDescricao & bairro

                    dt.Rows.Add(New Object() {cep, endereco})

                End While

                gridCEP.DataSource = dt
                gridCEP.DataBind()

            End If
        Else
            lblMensagem.Text = sqlErro
        End If

    End Sub

    Function conectaSQLExcel() As Boolean
        sqlConn.Close()
        sqlConn.ConnectionString = connString_Excel
        Try
            sqlConn.Open()
            Return True
        Catch ex As Exception
            sqlErro = ex.Message()
            Return False
        End Try
    End Function

    Function selectSQLExcel(ByVal sqlString As String) As Boolean
        If conectaSQLExcel() Then
            sqlComando = New OleDbCommand(sqlString, sqlConn)
            Try
                drExcel = sqlComando.ExecuteReader()
                Return True
            Catch ex As Exception
                sqlErro = ex.Message()
                Return False
            End Try
        End If
    End Function



End Class