Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.Office.Interop
Imports System.Threading.Thread
Imports System.Globalization

Partial Public Class exibeTodosColaboradores

    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ' IF que checa se o colaborador logado tem permissão de acesso a esta página ===================================
#If Not Debug Then
        ' Se caso a Session expirar volta para tela de Login
        Try
            If Not Session("permissao").ToString.ToLower.Contains("cadastro de colaborador") Then
                Response.Redirect("..\Default.aspx")
            End If
        Catch ex As Exception
            Response.Redirect("..\Default.aspx")
        End Try
#Else
        Session("colCodigoLogado") = 39
#End If
        ' ==============================================================================================================

        Dim strConn As String = getConnectionString()
        Dim SQL As String

        Dim dtConsultores As New DataTable("consultores")

        dtConsultores.Columns.Add(" Cod. no SAP ", GetType(String))
        dtConsultores.Columns.Add("Cod. Empresa no SAP", GetType(String))
        dtConsultores.Columns.Add("Nome", GetType(String))
        dtConsultores.Columns.Add("Perfil", GetType(String))
        dtConsultores.Columns.Add("Módulo", GetType(String))
        dtConsultores.Columns.Add("Nível", GetType(String))
        dtConsultores.Columns.Add("Celular", GetType(String))
        dtConsultores.Columns.Add("E-mail Addvisor", GetType(String))
        dtConsultores.Columns.Add("Contrato", GetType(String))
        dtConsultores.Columns.Add("Valor Hora/Fixo", GetType(String))
        dtConsultores.Columns.Add("Status", GetType(String))

        SQL = "SELECT *, convert(varchar(30), colSalario) AS 'salario' FROM v_colaboradores ORDER BY colNome"

        If Not selectSQL(SQL) Then
            lblMensagem.Text = sqlErro
            Return
        End If

        dr.Read()

        While dr.Read

            Dim tipoContrato As String = dr("colTipoContrato")
            Dim salario As String = ""

            If dr("colSalario") IsNot DBNull.Value Then
                salario = dr("colSalario")
                salario = Decimal.Parse(salario).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                salario = salario.Replace("R$ ", "")
            End If

            If dr("colTipoContrato").ToString.ToUpper = "PJ" Then
                If dr("colValorFechado") = "S" Then
                    tipoContrato = "PJ - F"
                Else
                    tipoContrato = "PJ"
                End If
            End If

            dtConsultores.Rows.Add(New Object() { _
               dr("colCodigoSAP"), dr("colCodigoEmpresaSAP"), dr("colNome"), dr("colPerfil"), dr("colModulo"), dr("colNivel"), dr("colCel"), _
                dr("colEmail1"), tipoContrato, salario, dr("colStatus")})

        End While

        dg.DataSource = dtConsultores
        dg.DataBind()

        '' Cria uma nova conexao OLedb
        'Dim con As New SqlConnection(strConn)
        '' Cria um dataset
        'Dim ds As DataSet = New DataSet()
        '' Cria um commando usando o DataAdapter
        'Dim Cmd As New SqlDataAdapter(SQL, con)
        '' Preenche o dataset com os dados da tabela
        'Cmd.Fill(ds, "colaboradores")
        '' Exibe os dados no datagrid
        'dg.DataSource = ds.Tables("colaboradores").DefaultView
        'dg.DataBind()

    End Sub

    Enum xlsOption
        xlsSaveAs
        xlsOpen
    End Enum

    Sub exportar(ByVal Source As Object, ByVal E As EventArgs)
        exportarExcel(dg, "colaboradores")
    End Sub

    Sub exportarExcel(ByVal grid As DataGrid, ByVal nomeArquivo As String)

        ' O limite de linhas do Excel é 65536
        If grid.Items.Count.ToString + 1 < 65536 Then
            HttpContext.Current.Response.Clear()
            'Excel 2003 : "application/vnd.ms-excel"
            'Excel 2007 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel"
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" & nomeArquivo & ".xls")
            ' Remover caracteres do header - Content-Type
            HttpContext.Current.Response.Charset = ""
            ' HttpContext.Current.Response.WriteFile("style.txt")
            ' desabilita o view state.
            grid.EnableViewState = False
            Dim tw As New System.IO.StringWriter()
            Dim hw As New System.Web.UI.HtmlTextWriter(tw)
            grid.RenderControl(hw)
            ' Escrever o html no navegador
            HttpContext.Current.Response.Write(tw.ToString())
            ' Termina o response
            HttpContext.Current.Response.End()
        Else
            HttpContext.Current.Response.Write("Muitas linhas para exportar para o Excel !!!")
        End If

    End Sub

    Private Sub teste()
        'limpo a resposta do HTTP
        Response.Clear()
        'adiciono o cabeçalho que faz o .Net entender que se trata de um xls
        Response.AddHeader("content-disposition", "attachment;filename=todosColaboradores.xls")
        'zero o charset
        Response.Charset = ""
        'desabilito o cache do browser
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        'Seto o content type para xls
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        'Crio um String Writer
        Dim sWr As New System.IO.StringWriter()
        'Transformo em HTML String Writer
        Dim hWr As System.Web.UI.HtmlTextWriter = New HtmlTextWriter(sWr)
        'Mando o datagrid (que deve ser criado antes de renderizar o HtmlTextWriter criado...
        dg.RenderControl(hWr)
        'mando escrever na tela. Com todos os cabeçalhos setados, é afpAberto para salvar o XML
        Response.Write(sWr.ToString())
        'Fecho a resposta.
        Response.End()
    End Sub

End Class