Imports Microsoft.Reporting.WebForms

Public Class rvApontamento
    Inherits System.Web.UI.Page

    Dim SQL As String
    Dim somaNormais = "00:00"
    Dim somaExtras = "00:00"
    Dim somaTotal = "00:00"
    Dim dataSet As DataSet

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim colCodigo As String = Session("visApoColCodigo")
        Dim proCodigo As String = Session("visApoProCodigo")
        Dim dataInicio As String = Session("visApoDataInicio")

        If Not IsNothing(Session("visApoColCodigo")) Then
            colCodigo = Session("visApoColCodigo")
            proCodigo = Session("visApoProCodigo")
            dataInicio = Session("visApoDataInicio")
        Else
            colCodigo = Request("Colaborador")
            proCodigo = Request("projeto")
            dataInicio = Request("Data")
        End If

        '''''''' Somente deixar estas variaveis setadas com valores para testes
        'colCodigo = 105
        ''proCodigo = 27
        'dataInicio = "26/03/2012"
        'Session("visApoAbaSelecionada") = 1

        ' If que verifica se os dados passados como parametro são validos 
        If Session("visApoAbaSelecionada") = 0 Then
            If Not (IsNumeric(colCodigo) And IsNumeric(proCodigo) And IsDate(dataInicio)) Then
                divRelatorio.Visible = False
                divAlerta.Visible = True
                lblMensagem.Text = "Erro na geração do relatório, tente acessa-lo pela página de visualização de apontamentos."
                Return
            End If
        Else
            If Not (IsNumeric(colCodigo) And IsDate(dataInicio)) Then
                divRelatorio.Visible = False
                divAlerta.Visible = True
                lblMensagem.Text = "Erro na geração do relatório, tente acessa-lo pela página de visualização de apontamentos."
                Return
            End If
        End If

        If Not IsPostBack Then
            Try
                'define o modo Local como o processamneto para o ReportViewer
                ReportViewerApontamento.ProcessingMode = ProcessingMode.Local

                ' Habilito ou desabilito algumas funcionalidade do ReportView de acordo com minha necessidade
                ReportViewerApontamento.ShowBackButton = False
                ReportViewerApontamento.ShowFindControls = False
                ReportViewerApontamento.ShowPageNavigationControls = False
                ReportViewerApontamento.ShowPrintButton = True
                ReportViewerApontamento.ShowZoomControl = True

                Dim rep As LocalReport = ReportViewerApontamento.LocalReport
                '  string path = HttpContext.Current.Server.MapPath("~/Reports/");
                Dim caminhoRpt As String = HttpContext.Current.Server.MapPath("~/apontamentoReport/")
                Select Case Session("visApoAbaSelecionada")
                    Case 0
                        'define o local do relatorio criado
                        rep.ReportPath = caminhoRpt & "rvApontamento1.rdlc"
                        dataSet = getApontamento1(colCodigo, proCodigo, dataInicio)
                    Case 1
                        'define o local do relatorio criado
                        rep.ReportPath = caminhoRpt & "rvApontamento2.rdlc"
                        dataSet = getApontamento2(colCodigo, dataInicio)
                End Select
                'Cria uma fonte de dados para o relatório para o dataset vendas
                Dim dsApontamentoRpt As New ReportDataSource()
                'defino o nome do datasource
                dsApontamentoRpt.Name = "relatorioApontamento"
                'usa primeira tabela do dataset
                dsApontamentoRpt.Value = dataSet.Tables(0)
                'atribui o datasource ao relatorio
                rep.DataSources.Add(dsApontamentoRpt)
            Catch ex As Exception
                divRelatorio.Visible = False
                divAlerta.Visible = True
                lblMensagem.Text = ex.Message
                Return
            End Try
        End If

        Dim clienteScript As String = "window.self.close();"
        lnkFechar.Attributes.Add("Onclick", clienteScript)

    End Sub

    ''' <summary>
    ''' Monta um dataSet para exibição do apontamento pelo periodo, projeto e colaborador selecionados
    ''' </summary>
    ''' <param name="colCodigo"></param>
    ''' <param name="proCodigo"></param>
    ''' <param name="dataInicio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getApontamento1(ByVal colCodigo As String, ByVal proCodigo As String, ByVal dataInicio As DateTime) As DataSet

        Dim dataFim As Date = dataInicio.AddMonths(1).AddDays(-1)

        ' Prepara as variaveis que se repetiram em toda a tabela
        Dim nomeColaborador As String = ""
        Dim modulo As String = ""
        Dim nivel As String = ""
        Dim mesReferencia As String = Format(dataFim, "MMMM").ToUpper
        Dim periodo As String = dataInicio & " até " & dataFim
        Dim nomeCliente As String = ""
        Dim nomeProjeto As String = ""
        Dim NomeResponsavel As String = getResponsavelProjeto(colCodigo, proCodigo, dataInicio, dataFim)
        Dim apoDiaUtil As String

        SQL = "SELECT * FROM v_relatorioApontamento WHERE colCodigo = " & colCodigo & " and proCodigo = " & proCodigo
        SQL += " and apoData BETWEEN '" & dataInicio & "' and '" & dataFim & "'"

        If selectSQL(SQL) Then
            dr.Read()
            If Not dr.HasRows Then
                divRelatorio.Visible = False
                divAlerta.Visible = True
                lblMensagem.Text = "No periodo selecionado não houve apontamentos."
            End If
            If dr("colNome") IsNot DBNull.Value Then
                nomeColaborador = dr("colNome")
            Else
                nomeColaborador = "Não cadastrado"
            End If
            If dr("colModulo") IsNot DBNull.Value Then
                modulo = dr("colModulo")
            Else
                modulo = ""
            End If
            If dr("colNivel") IsNot DBNull.Value Then
                nivel = dr("colNivel")
            Else
                nivel = ""
            End If
            If dr("proNomeCliente") IsNot DBNull.Value Then
                nomeCliente = dr("proNomeCliente")
            Else
                nomeCliente = "Não cadastrado"
            End If
            If dr("proNome") IsNot DBNull.Value Then
                nomeProjeto = dr("proNome")
            Else
                nomeProjeto = "Não cadastrado"
            End If
        Else
            divRelatorio.Visible = False
            divAlerta.Visible = True
            lblMensagem.Text = sqlErro
        End If

        ' Declata variaveis que serão usadas para calcular total das horas apontadas
        Dim horaNormal As String
        Dim horaExtra As String
        Dim horaTotal As String

        Dim apoEntrada As String = ""
        Dim apoEntAlmoco As String = ""
        Dim apoSaiAlmoco As String = ""
        Dim apoSaida As String = ""
        Dim apoDescricao As String = ""

        Dim ds As New DataSet

        ' ''''''' Criando a tabela relatorio
        Dim dtRelatorio As New DataTable("relatorio")

        ' ''''''' Criando as colunas para a tabela
        dtRelatorio.Columns.Add("apoData", GetType(String))
        dtRelatorio.Columns.Add("apoDiaUtil", GetType(String))
        dtRelatorio.Columns.Add("colNome", GetType(String))
        dtRelatorio.Columns.Add("colModulo", GetType(String))
        dtRelatorio.Columns.Add("colNivel", GetType(String))
        dtRelatorio.Columns.Add("mesReferencia", GetType(String))
        dtRelatorio.Columns.Add("responsavel", GetType(String))
        dtRelatorio.Columns.Add("periodo", GetType(String))
        dtRelatorio.Columns.Add("totalNormal", GetType(String))
        dtRelatorio.Columns.Add("totalExtras", GetType(String))
        dtRelatorio.Columns.Add("totalTotal", GetType(String))
        dtRelatorio.Columns.Add("proNome", GetType(String))
        dtRelatorio.Columns.Add("proNomeCliente", GetType(String))
        dtRelatorio.Columns.Add("apoEntrada", GetType(String))
        dtRelatorio.Columns.Add("apoEntAlmoco", GetType(String))
        dtRelatorio.Columns.Add("apoSaiAlmoco", GetType(String))
        dtRelatorio.Columns.Add("apoSaida", GetType(String))
        dtRelatorio.Columns.Add("apoNormais", GetType(String))
        dtRelatorio.Columns.Add("apoExtras", GetType(String))
        dtRelatorio.Columns.Add("apoTotal", GetType(String))
        dtRelatorio.Columns.Add("apoDescricao", GetType(String))

        ' ''''''' Alimentando tabela
        Dim dataFormatada = ""
        Dim colTituloPerfil As String = "Consultor: "
        Dim varData As Date = dataInicio

        For i = 1 To DateDiff(DateInterval.Day, dataInicio, dataFim) + 1

            ' Prepara o campo data no formato "01/01 SEG"
            If varData.Day <= 9 And varData.Month <= 9 Then
                dataFormatada = "0" & varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day <= 9 And varData.Month > 9 Then
                dataFormatada = "0" & varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day > 9 And varData.Month <= 9 Then
                dataFormatada = varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day > 9 And varData.Month > 9 Then
                dataFormatada = varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If

            If isDiaUtil(varData) Then
                apoDiaUtil = "S"
            Else
                apoDiaUtil = "N"
            End If

            Try

                horaNormal = "00:00"
                horaExtra = "00:00"
                horaTotal = "00:00"

                If dr("apoEntrada") IsNot DBNull.Value Then
                    apoEntrada = dr("apoEntrada")
                Else
                    apoEntrada = ""
                End If

                If dr("apoEntAlmoco") IsNot DBNull.Value Then
                    apoEntAlmoco = dr("apoEntAlmoco")
                Else
                    apoEntAlmoco = ""
                End If

                If dr("apoSaiAlmoco") IsNot DBNull.Value Then
                    apoSaiAlmoco = dr("apoSaiAlmoco")
                Else
                    apoSaiAlmoco = ""
                End If

                If dr("apoSaida") IsNot DBNull.Value Then
                    apoSaida = dr("apoSaida")
                Else
                    apoSaida = ""
                End If

                If dr("apoNormais") IsNot DBNull.Value Then
                    horaNormal = dr("apoNormais")
                Else
                    horaNormal = ""
                End If

                If dr("apoExtras") IsNot DBNull.Value Then
                    horaExtra = dr("apoExtras")
                Else
                    horaExtra = ""
                End If

                If dr("apoTotal") IsNot DBNull.Value Then
                    horaTotal = dr("apoTotal")
                Else
                    horaTotal = ""
                End If

                If dr("apoDescricao") IsNot DBNull.Value Then
                    apoDescricao = dr("apoDescricao")
                Else
                    apoDescricao = ""
                End If

                ' Se as datas não forem as mesmas então "pula linha na tabela"
                If varData = dr("apoData") Then
                    'Calculos dos totais de horas normais, extras e total 
                    somaNormais = somaHoras(somaNormais, horaNormal)
                    somaExtras = somaHoras(somaExtras, horaExtra)
                    somaTotal = somaHoras(somaTotal, horaTotal)


                    dtRelatorio.Rows.Add(New Object() {dataFormatada, apoDiaUtil, nomeColaborador, modulo, nivel,
                                                        mesReferencia, NomeResponsavel, periodo,
                                                        somaNormais, somaExtras, somaTotal,
                                                        nomeProjeto, nomeCliente, apoEntrada,
                                                        apoEntAlmoco, apoSaiAlmoco, apoSaida,
                                                        horaNormal, horaExtra, horaTotal,
                                                        apoDescricao})
                    dr.Read()
                Else
                    dtRelatorio.Rows.Add(New Object() {dataFormatada, apoDiaUtil, nomeColaborador, modulo, nivel,
                                                       mesReferencia, NomeResponsavel, periodo,
                                                       somaNormais, somaExtras, somaTotal,
                                                       nomeProjeto, nomeCliente, "", "", "", "",
                                                       "", "", "", ""})
                End If
            Catch ex As Exception
                dtRelatorio.Rows.Add(New Object() {dataFormatada, apoDiaUtil, "", "", "",
                                                    "", "", "",
                                                    somaNormais, somaExtras, somaTotal,
                                                    "", "", "", "", "", "",
                                                    "", "", "", ""})
            End Try

            varData = varData.AddDays(1)

        Next i

        ' Atribuindo as DataSet a tabela relatorio
        ds.Tables.Add(dtRelatorio)

        Return ds

    End Function

    ''' <summary>
    ''' Monta um dataSet para exibição do apontamento pelo periodo e colaborador selecionados
    ''' </summary>
    ''' <param name="colCodigo"></param>
    ''' <param name="dataInicio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getApontamento2(ByVal colCodigo As String, ByVal dataInicio As DateTime) As DataSet

        Dim dataFim As Date = dataInicio.AddMonths(1).AddDays(-1)
        Dim varData As Date = dataInicio

        ' Prepara as variaveis que se repetiram em toda a tabela
        Dim nomeColaborador As String = ""
        Dim modulo As String = ""
        Dim nivel As String = ""
        Dim mesReferencia As String = Format(dataFim, "MMMM").ToUpper
        Dim periodo As String = dataInicio & " até " & dataFim
        Dim nomeCliente As String = ""
        Dim nomeProjeto As String = ""
        Dim NomeResponsavel As String = ""
        Dim apoDiaUtil As String

        ' DataSet que utilizarei como retorno dessa função
        Dim ds As New DataSet

        ' ''''''' Criando a tabela relatorio que será atribuido ao dataSet "ds"
        Dim dtRelatorio As New DataTable("relatorio")

        ' ''''''' Criando as colunas para a tabela
        dtRelatorio.Columns.Add("apoData", GetType(String))
        dtRelatorio.Columns.Add("apoDiaUtil", GetType(String))
        dtRelatorio.Columns.Add("colNome", GetType(String))
        dtRelatorio.Columns.Add("colModulo", GetType(String))
        dtRelatorio.Columns.Add("colNivel", GetType(String))
        dtRelatorio.Columns.Add("mesReferencia", GetType(String))
        dtRelatorio.Columns.Add("responsavel", GetType(String))
        dtRelatorio.Columns.Add("periodo", GetType(String))
        dtRelatorio.Columns.Add("totalNormal", GetType(String))
        dtRelatorio.Columns.Add("totalExtras", GetType(String))
        dtRelatorio.Columns.Add("totalTotal", GetType(String))
        dtRelatorio.Columns.Add("proNome", GetType(String))
        dtRelatorio.Columns.Add("proNomeCliente", GetType(String))
        dtRelatorio.Columns.Add("apoEntrada", GetType(String))
        dtRelatorio.Columns.Add("apoEntAlmoco", GetType(String))
        dtRelatorio.Columns.Add("apoSaiAlmoco", GetType(String))
        dtRelatorio.Columns.Add("apoSaida", GetType(String))
        dtRelatorio.Columns.Add("apoNormais", GetType(String))
        dtRelatorio.Columns.Add("apoExtras", GetType(String))
        dtRelatorio.Columns.Add("apoTotal", GetType(String))
        dtRelatorio.Columns.Add("apoDescricao", GetType(String))

        '*******************************************************************
        ' Parte que preenche o cabeçalho da tabela de apontamento
        '*******************************************************************

        SQL = "SELECT * FROM v_colaboradores WHERE colCodigo = " & colCodigo

        ' Função que se faz um select no banco de dados e retorna dataReader "dr" como um objeto publico
        If selectSQL(SQL) Then
            dr.Read()
        Else
            lblMensagem.Text = sqlErro
            divAlerta.Visible = True
            divRelatorio.Visible = False
        End If

        ' Preenche a primeira linha do relatório
        nomeColaborador = dr("colNome")
        modulo = dr("colModulo")
        nivel = dr("colNivel")

        ' Preenchendo o Label Mês de referência Ex: "JANEIRO"
        mesReferencia = Format(dataFim, "MMMM").ToUpper

        ' Preenchendo o Label Periodo
        periodo = dataInicio & " a " & dataFim

        '******************************************************************************************
        ' Loop para preenchimento do dataTable dtApontamento que será usado para o 
        ' preenchimento da tabela de apontamento
        '******************************************************************************************
        While varData <= dataFim

            Dim entrada = ""
            Dim entAlmoco = ""
            Dim saiAlmoco = ""
            Dim saida = ""
            Dim horasNormal = ""
            Dim horasExtra = ""
            Dim horasTotal = ""
            Dim descricao = ""
            Dim data = ""
            Dim decTotal = 0D
            Dim decSaida = 0D
            Dim decEntrada = 0D

            ' Coloco a data no formato "10/02 SEG"
            If varData.Day <= 9 And varData.Month <= 9 Then
                data = "0" & varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day <= 9 And varData.Month >= 9 Then
                data = "0" & varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day >= 9 And varData.Month <= 9 Then
                data = varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day >= 9 And varData.Month >= 9 Then
                data = varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If

            If isDiaUtil(varData) Then
                apoDiaUtil = "S"
            Else
                apoDiaUtil = "N"
            End If

            SQL = "SELECT * FROM v_relatorioApontamento " &
                  "WHERE colCodigo = " & colCodigo & " AND apoData = '" & varData & "'"

            ' Função que se faz um select no banco de dados e retorna dataReader "dr" como um objeto publico
            If Not selectSQL(SQL) Then
                lblMensagem.Text = sqlErro
                divAlerta.Visible = True
                divRelatorio.Visible = False
                Return ds
            End If

            If dr.HasRows Then

                Dim listEntrada As New List(Of String)

                While dr.Read()

                    If Not IsDBNull(dr("apoEntrada")) Then
                        entrada = dr("apoEntrada")
                        ' Armazeno todos os horarios de entrada para utilizar como entrada na tabela de apontamento, faço 
                        ' isso para que quando tenha mais de um projeto apontado no dia, eu exiba corretamente pelo menos a hora de 
                        ' entrada do colaborador
                        listEntrada.Add(entrada)
                    End If
                    If Not IsDBNull(dr("apoEntAlmoco")) Then
                        entAlmoco = dr("apoEntAlmoco")
                    End If
                    If Not IsDBNull(dr("apoSaiAlmoco")) Then
                        saiAlmoco = dr("apoSaiAlmoco")
                    End If
                    If Not IsDBNull(dr("apoSaida")) Then
                        saida = dr("apoSaida")
                    End If
                    If Not IsDBNull(dr("apoNormais")) Then
                        horasNormal = somaHoras(dr("apoNormais"), horasNormal)
                    End If
                    If Not IsDBNull(dr("apoExtras")) Then
                        horasExtra = somaHoras(dr("apoExtras"), horasExtra)
                    End If
                    If Not IsDBNull(dr("apoTotal")) Then
                        horasTotal = somaHoras(dr("apoTotal"), horasTotal)
                        ' Coloco na descrição o centro de custo e as horas trabalhadas "(100100 - 08:00)"
                        If descricao = "" Then
                            descricao = "(" & dr("proCentroCusto") & " - " & dr("apoTotal") & ")"
                        Else
                            descricao += ", (" & dr("proCentroCusto") & " - " & dr("apoTotal") & ")"
                        End If
                    End If
                End While

                ' Se houver mais do que 1 dado neste list, então houve apontamento em mais de 1 projeto
                If listEntrada.Count > 1 Then

                    listEntrada.Sort()

                    entrada = listEntrada(0)

                    decEntrada = converteHorasDecimal(entrada)
                    decTotal = converteHorasDecimal(horasTotal)
                    decSaida = decEntrada + decTotal
                    saida = converteDecimalHoras(decSaida.ToString)

                    entAlmoco = ""
                    saiAlmoco = ""

                    If decTotal <= 24 Then
                        If decSaida > 24.0 Then
                            saida = "00:00"
                            entrada = converteDecimalHoras(24D - decTotal)
                        End If
                    Else
                        ' Se este valor for maior que 24, significa que o horario apontado é maior que 24 horas, alerto sobre
                        ' erro no apontamento
                        entrada = "*"
                        entAlmoco = "*"
                        saiAlmoco = "*"
                        saida = "*"
                        horasNormal = ""
                        horasExtra = ""
                        horasTotal = converteDecimalHoras(decTotal)
                        descricao = descricao
                    End If

                    'If decTotal > converteHorasDecimal(classParametros.limiteHorasNormal) Then
                    '    horasNormal = classParametros.limiteHorasNormal
                    'End If

                    If decTotal > 8 Then
                        horasNormal = "08:00"
                        horasExtra = converteDecimalHoras(decTotal - 8)
                        horasTotal = converteDecimalHoras(decTotal)
                    End If

                End If

                somaNormais = somaHoras(somaNormais, horasNormal)
                somaExtras = somaHoras(somaExtras, horasExtra)
                somaTotal = somaHoras(somaTotal, horasTotal)

                dtRelatorio.Rows.Add(New Object() {data, apoDiaUtil, nomeColaborador, modulo, nivel,
                                                        mesReferencia, NomeResponsavel, periodo,
                                                        somaNormais, somaExtras, somaTotal,
                                                        "", "", entrada, entAlmoco, saiAlmoco, saida,
                                                        horasNormal, horasExtra, horasTotal, descricao})
            Else
                dtRelatorio.Rows.Add(New Object() {data, apoDiaUtil, nomeColaborador, modulo, nivel,
                                                        mesReferencia, NomeResponsavel, periodo,
                                                        somaNormais, somaExtras, somaTotal,
                                                        "", "", entrada, entAlmoco, saiAlmoco, saida,
                                                        horasNormal, horasExtra, horasTotal, descricao})
            End If

            varData = varData.AddDays(1)

        End While

        ' Atribuindo o dataTable "dtRelatorio" ao dataSet "ds"
        ds.Tables.Add(dtRelatorio)

        Return ds

    End Function


End Class