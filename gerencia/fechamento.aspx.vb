Imports System.Globalization

Partial Public Class relatorioFechamento
    Inherits System.Web.UI.Page

    Dim SQL As String
    Dim dataInicio As DateTime
    Dim dataFim As DateTime
    Dim primeiroDia As String = "01"
    Dim totalTudo As Decimal = 0
    ' ''''''' Criando uma DataTable
    Dim dtRelatorio As New DataTable("consultores")
    Dim arrayProCodigo As New ArrayList()
    Dim arrayColCodigo As New ArrayList()
    Dim proCodigo As String
    Dim colCodigo As String
    Dim periodoSelecionado As Date
    Dim primeiroAnoSistema As Integer = 2011
    'Variavel utilizada na função "getAprovador"
    Public aprovador As String
    Protected URL

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Prepara URL para ser usada na chamada de função janelaNova (javascript), janela que mostra o apontamento selecionado
        URL = Request.Url.GetLeftPart(UriPartial.Authority) & VirtualPathUtility.ToAbsolute("~/")
        URL += "apontamentoReport/apontamento.aspx"
        txtURL.Value = URL

        If ValidarSessao(Session("colPerfilLogado")) Then Response.Redirect("..\Default.aspx")

        lblMensagem.Style.Add("color", "Red")
        lblMensagem.Text = ""

        If Not IsPostBack Then

            lblPeriodoSelecionado.Style.Add("color", "Gray")
            lblPeriodoSelecionado.Style.Add("font-size", "smaller")
            lblPeriodoSelecionado.Text = ""

            ' Preenche o comboBox Mes
            ddlMes.Items.Add(New ListItem("Janeiro", "01"))
            ddlMes.Items.Add(New ListItem("Fevereiro", "02"))
            ddlMes.Items.Add(New ListItem("Março", "03"))
            ddlMes.Items.Add(New ListItem("Abril", "04"))
            ddlMes.Items.Add(New ListItem("Maio", "05"))
            ddlMes.Items.Add(New ListItem("Junho", "06"))
            ddlMes.Items.Add(New ListItem("Julho", "07"))
            ddlMes.Items.Add(New ListItem("Agosto", "08"))
            ddlMes.Items.Add(New ListItem("Setembro", "09"))
            ddlMes.Items.Add(New ListItem("Outubro", "10"))
            ddlMes.Items.Add(New ListItem("Novembro", "11"))
            ddlMes.Items.Add(New ListItem("Dezembro", "12"))

            ' Preenche o comboBox projetos de acordo com o perfil logado
            '   Diretoria e CSC exibe todos
            '   GC e GP, exibe somente os projetos associados a ele            
            Select Case Session("perCodigoLogado")
                Case 2 ' Gerente de Projetos
                    SQL = "SELECT proNome, proCodigo FROM v_Projetos WHERE UPPER(PROSTATUS) = 'ATIVO' " &
                          "AND codGP = " & Session("colCodigoLogado") & " ORDER BY proNome"
                Case 3 ' Gerente de Contas
                    SQL = "SELECT proNome, proCodigo FROM v_Projetos WHERE UPPER(PROSTATUS) = 'ATIVO' " &
                          "AND codGC = " & Session("colCodigoLogado") & " ORDER BY proNome"
                Case 4 ' Diretoria 
                    SQL = "SELECT proNome, proCodigo FROM v_projetos WHERE UPPER(PROSTATUS) = 'ATIVO' ORDER BY proNome"
                Case 5 ' CSC
                    SQL = "SELECT proNome, proCodigo FROM v_projetos WHERE UPPER(PROSTATUS) = 'ATIVO' ORDER BY proNome"
            End Select

            If selectSQL(SQL) Then
                ddlProjeto.Items.Add("")
                ddlProjeto.Items.Add("Exibir todos projetos desta lista")
                While dr.Read
                    ddlProjeto.Items.Add(New ListItem(dr("proNome"), dr("proCodigo")))
                End While
            End If

            ' Deixa selecionado como padrão o mês corrente
            ddlMes.SelectedIndex = Date.Now.Month - 1

            ' Preenche o camboBox Ano com ano inicio 2011 até o ano corrente
            While primeiroAnoSistema <= Date.Now.Year
                ddlAno.Items.Add(primeiroAnoSistema)
                primeiroAnoSistema = primeiroAnoSistema + 1
            End While
            ' Deixa selecionado como padrão o ano corrente
            ddlAno.SelectedValue = Date.Now.Year

            ' =========== Preenche o ddlDiaInicial com os dias do mês anterior ao mês selecionado
            Dim mes As Integer = ddlMes.SelectedValue
            Dim ano As Integer = ddlAno.SelectedValue
            If mes = 1 Then
                mes = 12
                ano = ano - 1
            Else
                mes = mes - 1
            End If

            ddlDiaInicial.Items.Clear()
            For i = 1 To System.DateTime.DaysInMonth(ano, mes)
                If i < 10 Then
                    ddlDiaInicial.Items.Add("0" & i)
                Else
                    ddlDiaInicial.Items.Add(i)
                End If
            Next i

            ' Seto por padrão o dia 26 no ddlDia
            ddlDiaInicial.SelectedValue = 26
            '========================================================================================

            preencheTabela(getCompetenciaSelecionada())

            divBotoes.Style.Add("display", "none")
            btnReabrir.Visible = False

        End If

    End Sub

    Private Function getCompetenciaSelecionada() As Date

        Dim mes = ddlMes.SelectedValue
        Dim dia = ddlDiaInicial.SelectedValue
        Dim ano = ddlAno.SelectedValue
        Dim data As Date

        ' Passando para a variavel data a data selecionada
        Date.TryParse(dia & "/" & mes & "/" & ano, data)

        If dia > 1 Then
            data = data.AddMonths(-1)
        End If

        lblPeriodoSelecionado.Text = "Período selecionado: <b style='color:#ED7522;'>" & data & "</b>" &
                                 " a <b style='color:#ED7522;'>" & data.AddMonths(1).AddDays(-1) & "</b>."

        txtDtIni.Value = data

        Return data

    End Function

    Private Sub ddlProjeto_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlProjeto.SelectedIndexChanged

        preencheTabela(getCompetenciaSelecionada())

    End Sub

    Private Sub ddlMes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMes.SelectedIndexChanged

        Dim diaSelecionado As Integer = ddlDiaInicial.SelectedValue
        Dim mesSelecionado As Integer = ddlMes.SelectedValue
        Dim anoSelecionado As Integer = ddlAno.SelectedValue

        '============ Preenchendo o DropDownList dos dias
        If mesSelecionado = 1 Then
            mesSelecionado = 12
            anoSelecionado = anoSelecionado - 1
        Else
            mesSelecionado = mesSelecionado - 1
        End If

        ddlDiaInicial.Items.Clear()
        For i = 1 To System.DateTime.DaysInMonth(anoSelecionado, mesSelecionado)
            If i < 10 Then
                ddlDiaInicial.Items.Add("0" & i)
            Else
                ddlDiaInicial.Items.Add(i)
            End If
        Next i

        Dim j As Integer = diaSelecionado

        If j >= 28 Then
            While j >= 28
                Try
                    ddlDiaInicial.SelectedValue = j
                    Exit While
                Catch ex As Exception
                    j = j - 1
                End Try
            End While
        Else
            ddlDiaInicial.SelectedValue = diaSelecionado
        End If

        preencheTabela(getCompetenciaSelecionada())

    End Sub

    Private Sub ddlDiaInicial_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDiaInicial.SelectedIndexChanged

        preencheTabela(getCompetenciaSelecionada())

    End Sub

    Private Sub ddlAno_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAno.SelectedIndexChanged

        preencheTabela(getCompetenciaSelecionada())

    End Sub

    Private Sub ddlOrdernar_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlOrdernar.SelectedIndexChanged

        preencheTabela(getCompetenciaSelecionada())

    End Sub

    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '   Subrotina para preencher a tabela do relatório
    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Private Sub preencheTabela(ByVal dataInicio As Date)

        primeiroDia = ddlDiaInicial.SelectedValue
        Dim taxaCustoCLT As Decimal = 0D
        Dim taxaCustoPJ As Decimal = 0D
        Dim cancelaExibicaoLinha = False
        Dim cancelaCalculo = False
        Dim ListaFeriados As List(Of Feriados) = RetornarFeriados()

        ' Coleta o que foi configurado em taxa de custo para CLT
        SQL = "SELECT taxPct FROM v_taxas WHERE taxTipo = 'CLT'"
        If selectSQL(SQL) Then
            If dr.HasRows Then
                dr.Read()
                If dr("taxPct").ToString.Trim <> "" Or dr("taxPct") IsNot DBNull.Value Then
                    Decimal.TryParse(dr("taxPct").ToString, taxaCustoCLT)
                End If
            End If
        End If

        ' Coleta o que foi configurado em taxa de custo para PJ
        SQL = "SELECT taxPct FROM v_taxas WHERE taxTipo = 'PJ'"
        If selectSQL(SQL) Then
            If dr.HasRows Then
                dr.Read()
                If dr("taxPct").ToString.Trim <> "" Or dr("taxPct") IsNot DBNull.Value Then
                    Decimal.TryParse(dr("taxPct").ToString, taxaCustoPJ)
                End If
            End If
        End If

        dtRelatorio.Clear()

        ' ''''''' Criando as colunas para a DataTable
        dtRelatorio.Columns.Add("proNome", GetType(String))
        dtRelatorio.Columns.Add("nome", GetType(String))
        dtRelatorio.Columns.Add("GP", GetType(String))
        dtRelatorio.Columns.Add("GC", GetType(String))
        dtRelatorio.Columns.Add("Dir", GetType(String))
        dtRelatorio.Columns.Add("colTipoContrato", GetType(String))
        dtRelatorio.Columns.Add("colSalario", GetType(String))
        dtRelatorio.Columns.Add("taxaProjeto", GetType(String))
        dtRelatorio.Columns.Add("horasTrabalhadas", GetType(String))
        dtRelatorio.Columns.Add("valorTotal", GetType(String))
        dtRelatorio.Columns.Add("proCodigo", GetType(String))
        dtRelatorio.Columns.Add("colCodigo", GetType(String))
        dtRelatorio.Columns.Add("taxaCustoCLT_PJ", GetType(String))

        ' Só continua se no comboBox ddlMes estiver selecionado algo
        If ddlProjeto.SelectedIndex > 0 Then

            Dim arrayStatusAprovacao As New ArrayList

            ' Prepara as datas para consulta no banco de dados
            Session("dataInicio") = dataInicio
            dataFim = dataInicio.AddMonths(1).AddDays(-1)

            ' Se selecionado "Exibir Todos"
            If ddlProjeto.SelectedIndex = 1 Then
                ' Foi selecionado para exibir todos os projetos
                SQL = "SELECT " &
                     "  tblApontamento.colCodigo, " &
                     "  tblApontamento.proCodigo, " &
                     "  tblProjetos.proNome, " &
                     "  tblColaboradores.colNome " &
                     "FROM tblApontamento " &
                     "INNER JOIN tblProjetos " &
                     "ON (tblApontamento.proCodigo = tblProjetos.proCodigo) " &
                     "INNER JOIN tblColaboradores " &
                     "ON tblColaboradores.colCodigo = tblApontamento.colCodigo " &
                     "WHERE apoData BETWEEN '" & dataInicio & "' and '" & dataFim & "' " &
                     "GROUP BY tblApontamento.colCodigo, tblApontamento.proCodigo, tblProjetos.proNome, tblColaboradores.colNome "
            Else
                ' Select para coletar todos os consultores que fizeram apontamento no projeto e mês selecionado
                SQL = "SELECT " &
                      "  tblApontamento.colCodigo, " &
                      "  tblApontamento.proCodigo, " &
                      "  tblProjetos.proNome, " &
                      "  tblColaboradores.colNome " &
                      "FROM tblApontamento " &
                      "INNER JOIN tblProjetos " &
                      "ON (tblApontamento.proCodigo = tblProjetos.proCodigo) " &
                      "INNER JOIN tblColaboradores " &
                      "ON tblColaboradores.colCodigo = tblApontamento.colCodigo " &
                      "WHERE tblApontamento.proCodigo = " & ddlProjeto.SelectedValue &
                      " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "' " &
                      "GROUP BY tblApontamento.colCodigo, tblApontamento.proCodigo, tblProjetos.proNome, tblColaboradores.colNome "
            End If

            Select Case ddlOrdernar.SelectedIndex
                Case 0 ' Escolhida a opção de ordernar por nome de projeto
                    SQL += "ORDER BY proNome "
                Case 1 ' Escolhida a opção de ordernar por nome do colaborador
                    SQL += "ORDER BY colNome "
            End Select

            ' Loop para coletar todos os códigos dos consultores e projeto através do que o usuário selecionou
            If selectSQL(SQL) Then
                While dr.Read()
                    ' Se for "Diretoria" e "CSC" exibe todos os projetos
                    If Session("colPerfilLogado").ToString.ToLower = "diretoria" Or Session("colPerfilLogado").ToString.ToLower = "csc" Then
                        arrayColCodigo.Add(dr("colCodigo"))
                        arrayProCodigo.Add(dr("proCodigo"))
                    Else ' Se não for "Diretoria" ou "CSC" exibe somente os projetos associados ao colaborador logado 
                        Dim lista As ListItem = New ListItem
                        lista.Text = dr("proNome")
                        lista.Value = dr("proCodigo")
                        If ddlProjeto.Items.Contains(lista) Then
                            arrayColCodigo.Add(dr("colCodigo"))
                            arrayProCodigo.Add(dr("proCodigo"))
                        End If
                    End If
                End While
            Else
                lblMensagem.Text = sqlErro
            End If

            For i = 0 To arrayColCodigo.Count - 1
                Dim proCodigo As String = ""
                Dim colCodigo As String = ""
                Dim proNome As String = ""
                Dim colNome As String = ""
                Dim taxaSalario
                Dim taxaProjeto = 0D
                Dim colunaTipoContrato As String = ""
                Dim colunaValor As String = "0,00"
                Dim valorProporcional As Decimal = 0
                Dim horasTrabalhadas = "00:00"
                Dim colunaGP As String = ""
                Dim colunaGC As String = ""
                Dim colunaDir As String = ""
                Dim colunaTaxaHora As String = "0,00"
                Dim fechado = ""
                Dim taxaCusto As Decimal = 0D
                Dim horasEmTodosProjetos As Decimal = "0"
                Dim temTaxaNoProjeto = False
                Dim QuantidadeDias As Integer = 0
                Dim MaisProjeto As Boolean = False

                ' Verifica se já existe registros na tabela tblFechamento para então não exibir nesta página, somente os
                ' que ainda estão como afpAbertos
                SQL = "SELECT * FROM v_fechamento WHERE " &
                       "proCodigo = " & arrayProCodigo(i) & " AND " &
                       "colCodigo = " & arrayColCodigo(i) & " AND " &
                       "fecDataInicio = '" & dataInicio & "' AND " &
                       "fecDataFim = '" & dataFim & "'"

                SQL = "IF EXISTS (" & SQL & ")" &
                      "  SELECT 'True' as 'existeDado' " &
                      "ELSE" &
                      "  SELECT 'False' as 'existeDado'"

                If Not selectSQL(SQL) Then
                    lblMensagem.Text = sqlErro
                    Return
                End If

                dr.Read()

                If arrayColCodigo(i) = 101 Then Console.Write("OI")

                ' Se não existir dados na tabela de fechamento então exiba a linha, ou seja, os apontamentos fechados
                ' somente exibirão na página de relatórios
                If dr("existeDado") = "False" Then

                    ' Coleta os nomes, salario e tipo de contrato, calcula também as horas trabalhadas
                    SQL = "SELECT proCodigo, colCodigo, proNome, colNome, colSalario, colTipoContrato, colValorFechado " &
                          "FROM v_relatorioApontamento " &
                          "WHERE proCodigo = " & arrayProCodigo(i) & " AND " &
                          "colCodigo = " & arrayColCodigo(i) & " AND " &
                          "apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

                    If selectSQL(SQL) Then
                        dr.Read()
                        If Not dr.HasRows Then Continue For
                        ' Coleta nome do projeto, nome, tipo de contrato, salario e o total de horas da primeira linha
                        ' e adiciona em arrays
                        proCodigo = dr("proCodigo")
                        proNome = dr("proNome")
                        colNome = dr("colNome")
                        colCodigo = dr("colCodigo")

                        If dr("colSalario") IsNot DBNull.Value Then
                            ' Coleto o salario/taxa do colaborador e considera a taxa do projeto a mesma
                            taxaSalario = dr("colSalario")
                            taxaProjeto = taxaSalario
                        Else
                            taxaSalario = "N"
                        End If

                        colunaTipoContrato = dr("colTipoContrato")

                        If dr("colTipoContrato") = "PJ" Then

                            taxaCusto = taxaCustoPJ

                            If dr("colValorFechado") = "S" Then
                                colunaTipoContrato = dr("colTipoContrato") & " - " & "F"
                            End If

                            ' Verifico se foi cadastrado uma taxa do projeto
                            SQL = "SELECT colProTaxa FROM v_colaboradores_projeto WHERE " &
                              "colCodigo = " & colCodigo & " AND proCodigo = " & proCodigo

                            If selectSQL(SQL) Then
                                If dr.Read() Then
                                    If Not IsDBNull(dr("colProTaxa")) Then
                                        If dr("colProTaxa") > 0D Then
                                            ' Se houver taxa cadastrada para o projeto, então considera esta para calculos
                                            taxaProjeto = dr("colProTaxa")
                                            temTaxaNoProjeto = True
                                        End If
                                    End If
                                End If
                            Else
                                lblMensagem.Text = sqlErro
                                Return
                            End If

                        Else

                            taxaCusto = taxaCustoCLT

                        End If

                    Else
                        lblMensagem.Text = sqlErro
                        Exit For
                    End If

                    Dim DiasUteis As Long = DateAndTime.DateDiff(DateInterval.Day, dataInicio, dataFim.AddDays(1))
                    Dim DiasFeriados As Integer = 0
                    Dim DiasUteisMes As Integer = 0

                    'Caso seja PJ -F busca a quantidade de dias apontada
                    If colunaTipoContrato = "PJ - F" Then
                        SQL = "SELECT count(proCodigo) as Dias " &
                                 "FROM v_relatorioApontamento " &
                                 "WHERE proCodigo = " & arrayProCodigo(i) & " AND " &
                                 "colCodigo = " & arrayColCodigo(i) & " AND " &
                                 "apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "' and apoEntrada is not null"

                        If selectSQL(SQL) Then
                            If dr.HasRows Then
                                dr.Read()
                                QuantidadeDias = Integer.Parse(dr("Dias"))
                            End If
                        End If

                        'Pega os feriados no intervalo caso haja
                        SQL = "Select count(*) as QtdFeriados from tblFeriados where cast(cast(dia As varchar(02)) + '/' + cast(mes as varchar(02)) + '/" & dataInicio.Year & "'  as date) BETWEEN '" & dataInicio & "' and '" & dataFim & "' and  Datepart (weekday,cast(cast(dia As varchar(02)) + '/' + cast(mes as varchar(02)) + '/2018'  as date)) not in (1,7)"
                        If selectSQL(SQL) Then
                            If dr.HasRows Then
                                dr.Read()
                                DiasFeriados = Integer.Parse(dr("QtdFeriados"))
                            End If
                        End If

                        Dim DataVer As Date = dataInicio

                        While DataVer <= dataFim.AddDays(1)
                            If DataVer.DayOfWeek = System.DayOfWeek.Saturday Or DataVer.DayOfWeek = System.DayOfWeek.Sunday Then
                                DiasUteisMes += 1
                            End If
                            DataVer = DataVer.AddDays(1)
                        End While
                        DiasUteis = (DiasUteis - DiasFeriados - DiasUteisMes)

                        MaisProjeto = False

                        'verifica se o consultor esta em mais de um projeto
                        SQL = "select count(clp.proCodigo) as QTD   from tblColaboradores_Projetos clp with(nolock)" & vbCrLf &
                               " inner join tblProjetos pro with(nolock) on (clp.proCodigo = pro.proCodigo and upper(pro.proStatus)='ATIVO')" & vbCrLf &
                               " where clp.colCodigo=" & colCodigo

                        If selectSQL(SQL) Then
                            If dr.HasRows Then
                                dr.Read()
                                If Integer.Parse(dr("QTD")) > 1 Then MaisProjeto = True
                            End If
                        End If
                    End If

                    '===============================================    Fim do IF que verifica dados na tabela tblFechamento 

                    ' Verifica se o conteudo da variavel colSalario é do tipo Decimal
                    If TypeOf taxaSalario Is Decimal Then

                        If Not cancelaCalculo Then

                            ' Se for PJ multiplica o valor/hora com o total de horas trabalhadas
                            If colunaTipoContrato = "PJ" Then
                                horasTrabalhadas = converteHorasDecimal(FomatarDataHora(getHorasApontadasEmUmProjeto(colCodigo, proCodigo, dataInicio)))

                                If horasTrabalhadas <> "0" Then
                                    '  Atualiza valorTotal multiplicando horasDecimal pelo valor taxa/hora ex.: 180.5 * 50 
                                    colunaValor = horasTrabalhadas * taxaSalario
                                    colunaValor = Decimal.Parse(colunaValor).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                    colunaTaxaHora = Decimal.Parse(taxaSalario).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                Else
                                    cancelaExibicaoLinha = True
                                End If

                            End If

                            ' Se for PJ-F divide o valor pelas horas trabalhadas e se acha o valor/hora
                            If colunaTipoContrato = "PJ - F" Then



                                horasEmTodosProjetos = converteHorasDecimal(getHorasApontadasTodosProjeto(colCodigo, dataInicio))
                                horasTrabalhadas = converteHorasDecimal(FomatarDataHora(getHorasApontadasEmUmProjeto(colCodigo, proCodigo, dataInicio)))

                                ' Verifico se tem taxa no projeto cadastrada
                                If temTaxaNoProjeto Then
                                    If horasTrabalhadas > 0 Then
                                        '  Atualiza valorTotal multiplicando horas em Decimal pela taxa no Projeto ex.: 180.5 * 50 
                                        colunaValor = horasTrabalhadas * taxaProjeto
                                        colunaValor = Decimal.Parse(colunaValor).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                        colunaTaxaHora = Decimal.Parse(taxaProjeto).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                    Else
                                        cancelaExibicaoLinha = True
                                    End If
                                Else
                                    If horasTrabalhadas > 0 Then
                                        'verifica se o consultor esta em mais deu m projeto
                                        If MaisProjeto Then
                                            '  Divido seu salario com as horas trabalhadas para achar sua taxa/hora
                                            valorProporcional = taxaSalario * (((horasTrabalhadas * 100) / horasEmTodosProjetos) / 100)
                                            colunaValor = valorProporcional.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                            colunaTaxaHora = (valorProporcional / horasTrabalhadas).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                        Else
                                            If (horasTrabalhadas < 168) Then
                                                Dim ValorHora As Decimal = (taxaSalario / DiasUteis / 8)
                                                valorProporcional = (taxaSalario / DiasUteis)
                                                colunaValor = (valorProporcional * QuantidadeDias).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                                colunaTaxaHora = ValorHora.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                            Else
                                                colunaValor = Decimal.Parse(taxaSalario).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                                colunaTaxaHora = Decimal.Parse(taxaSalario).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))

                                            End If
                                        End If


                                    Else
                                        cancelaExibicaoLinha = True
                                    End If
                                End If

                            End If

                            ' Se for CLT = (salario * (1 + (custoPercentual / 100))) / horasTrabalhadas 
                            '  Ex.: (1000 * (1 + (72 / 100))) / 168 = 10,23
                            If colunaTipoContrato = "CLT" Then
                                horasEmTodosProjetos = converteHorasDecimal(getHorasApontadasTodosProjeto(colCodigo, dataInicio))
                                horasTrabalhadas = converteHorasDecimal(FomatarDataHora(getHorasApontadasEmUmProjeto(colCodigo, proCodigo, dataInicio)))

                                If horasEmTodosProjetos > 0 And horasTrabalhadas <> "0" Then
                                    '  Multiplico salario com a porcentual para obter o custo
                                    taxaSalario = taxaSalario * (1 + (taxaCustoCLT / 100))
                                    '  Divido seu salario com as horas trabalhadas para achar sua taxa/hora
                                    valorProporcional = taxaSalario * (((horasTrabalhadas * 100) / horasEmTodosProjetos) / 100)
                                    colunaValor = valorProporcional.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                    colunaTaxaHora = (valorProporcional / horasTrabalhadas).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                Else
                                    cancelaExibicaoLinha = True
                                End If

                            End If

                        End If

                    Else
                        ' Se caso o campo colSalario estiver vazio ou nulo, seta a a variavel com um valor conpativel para ser
                        ' utilizado mais a frente com a função converteDecimalHoras
                        horasTrabalhadas = "0"
                    End If

                    colunaValor = colunaValor.ToString.Replace("R$ ", "")
                    colunaTaxaHora = colunaTaxaHora.ToString.Replace("R$ ", "")
                    ' Variavel exibida no rodapé da tabela
                    totalTudo = totalTudo + colunaValor

                    ' Preenche o dataTable com os status de aprovação dos consultores
                    arrayStatusAprovacao = getStatusAprovacao(colCodigo, arrayProCodigo(i), dataInicio)

                    colunaGP = arrayStatusAprovacao(0)
                    colunaGC = arrayStatusAprovacao(1)
                    colunaDir = arrayStatusAprovacao(2)

                    Select Case colunaGP
                        Case "A"
                            colunaGP = "<b style='color:Green;'  >A</b>"
                        Case "R"
                            colunaGP = "<b style='color:Red;'    >R</b>"
                        Case "N"
                            colunaGP = "<b style='color:Gray;'   >N</b>"
                        Case "P"
                            colunaGP = "<b style='color:#E8C500;'>P</b>"
                    End Select
                    Select Case colunaGC
                        Case "A"
                            colunaGC = "<b style='color:Green;'  >A</b>"
                        Case "R"
                            colunaGC = "<b style='color:Red;'    >R</b>"
                        Case "N"
                            colunaGC = "<b style='color:Gray;'   >N</b>"
                        Case "P"
                            colunaGC = "<b style='color:#E8C500;'>P</b>"
                    End Select
                    Select Case colunaDir
                        Case "A"
                            colunaDir = "<b style='color:Green;'  >A</b>"
                        Case "R"
                            colunaDir = "<b style='color:Red;'    >R</b>"
                        Case "N"
                            colunaDir = "<b style='color:Gray;'   >N</b>"
                        Case "P"
                            colunaDir = "<b style='color:#E8C500;'>P</b>"
                    End Select

                    Dim expRegular = "([0-9]+|\d):[0-5]\d" ' Aceita no formato de horas, por exemplo "999:59"

                    'If Not Regex.IsMatch(horasTrabalhadas, expRegular) Then
                    '    ' Converte o valor que vem em decimal para o formato de horas Ex.: 18.5 = 18:00
                    '    horasTrabalhadas = converteDecimalHoras(horasTrabalhadas)
                    'End If

                    ' Cancela Exibição da Linha quando não foi encontrado nenhum apontamento realizado
                    If Not cancelaExibicaoLinha Then
                        dtRelatorio.Rows.Add(New Object() {
                           proNome, colNome, colunaGP, colunaGC, colunaDir,
                           colunaTipoContrato, colunaTaxaHora, "",
                           FomatarDataHora(getHorasApontadasEmUmProjeto(colCodigo, proCodigo, dataInicio)), colunaValor,
                           proCodigo, colCodigo, taxaCusto
                        })
                    End If

                    cancelaCalculo = False
                        cancelaExibicaoLinha = False

                    End If

            Next

            If dtRelatorio.Rows.Count > 0 Then
                ' Se for Diretoria e CSC poderá realizar fechamentos e aberturas
                If Session("colPerfilLogado").ToString.ToLower = "diretoria" Or Session("colPerfilLogado").ToString.ToLower = "csc" Then
                    divBotoes.Style.Add("display", "block")
                End If
            Else
                divBotoes.Style.Add("display", "none")
            End If

            ' Acionando a DataTable ao dtRelatorio
            tabelaRelatorio.DataSource = dtRelatorio
            tabelaRelatorio.DataBind()

        Else
            tabelaRelatorio.DataBind()
            divBotoes.Style.Add("display", "none")
        End If

    End Sub

    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '   Salva o relatório na tabela tblFechamento
    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Private Sub btnConfirmar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmar.Click

        Dim aprovacaoGP As String
        Dim aprovacaoGC As String
        Dim aprovacaoDir As String
        Dim aprovadorGP = ""
        Dim aprovadorGC = ""
        Dim aprovadorDir = ""
        Dim tipoContrato As String
        Dim taxaCusto As String
        Dim taxaHora As String = "NULL"
        Dim taxaProjeto As String = "NULL"
        Dim horasTrabalhadas As String
        Dim valorTotal As String
        Dim tipoEmpresa As String
        Dim razaoSocial As String
        Dim empresaCompartilhada As String
        Dim cont As Integer = 0

        ' Loop para verificar se todos os apontamentos estão como aprovados
        For i = 0 To (tabelaRelatorio.DataKeys.Count - 1)

            aprovadorGP = "NULL"
            aprovadorGC = "NULL"
            aprovadorDir = "NULL"
            tipoEmpresa = "NULL"
            razaoSocial = "NULL"
            empresaCompartilhada = "NULL"

            ' Coleta o que foi preenchido nas colunas GP, GC e Dir
            aprovacaoGP = tabelaRelatorio.DataKeys(i).Item(2).ToString
            aprovacaoGC = tabelaRelatorio.DataKeys(i).Item(3).ToString
            aprovacaoDir = tabelaRelatorio.DataKeys(i).Item(4).ToString
            aprovacaoGP = aprovacaoGP.Substring(26, 1)
            aprovacaoGC = aprovacaoGC.Substring(26, 1)
            aprovacaoDir = aprovacaoDir.Substring(26, 1)
            ' Verifica se a linha selecionada no loop esta como aprovado, não havendo pendencia ou reprovado
            If aprovacaoGP = "P" Or aprovacaoGP = "R" Or
              aprovacaoGC = "P" Or aprovacaoGC = "R" Or
              aprovacaoDir = "P" Or aprovacaoDir = "R" Then
                'lblMensagem.Text = "Para confirmar o fechamento todos os apontamentos devem estar como Aprovados."
                'Return
            Else

                proCodigo = tabelaRelatorio.DataKeys(i).Item(10).ToString
                colCodigo = tabelaRelatorio.DataKeys(i).Item(11).ToString
                aprovacaoGP = tabelaRelatorio.DataKeys(i).Item(2).ToString
                aprovacaoGC = tabelaRelatorio.DataKeys(i).Item(3).ToString
                aprovacaoDir = tabelaRelatorio.DataKeys(i).Item(4).ToString
                tipoContrato = tabelaRelatorio.DataKeys(i).Item(5).ToString
                taxaHora = tabelaRelatorio.DataKeys(i).Item(6).ToString
                taxaProjeto = "NULL"
                horasTrabalhadas = tabelaRelatorio.DataKeys(i).Item(8).ToString
                valorTotal = tabelaRelatorio.DataKeys(i).Item(9).ToString
                taxaCusto = tabelaRelatorio.DataKeys(i).Item(12).ToString

                aprovacaoGP = aprovacaoGP.Substring(26, 1)
                aprovacaoGC = aprovacaoGC.Substring(26, 1)
                aprovacaoDir = aprovacaoDir.Substring(26, 1)
                taxaHora = taxaHora.Replace("R$", "").Replace(".", "")
                taxaHora = taxaHora.Replace(",", ".")
                valorTotal = valorTotal.Replace("R$", "").Replace(".", "")
                valorTotal = valorTotal.Replace(",", ".")
                dataInicio = Session("dataInicio")
                dataFim = dataInicio.AddMonths(1).AddDays(-1)

                If aprovacaoGP = "A" Then
                    If getAprovador(colCodigo, proCodigo, dataInicio, "gp") Then
                        aprovadorGP = aprovador
                    Else
                        lblMensagem.Text = "Não foi possivel obter a informação de quem é o GP  do projeto " &
                            tabelaRelatorio.DataKeys(i).Item(0).ToString
                        Return
                    End If
                End If

                If aprovacaoGC = "A" Then
                    If getAprovador(colCodigo, proCodigo, dataInicio, "gc") Then
                        aprovadorGC = aprovador
                    Else
                        lblMensagem.Text = "Não foi possivel obter a informação de quem é o GC do projeto " &
                            tabelaRelatorio.DataKeys(i).Item(0).ToString
                        Return
                    End If
                End If

                If aprovacaoDir = "A" Then
                    If getAprovador(colCodigo, proCodigo, dataInicio, "dir") Then
                        aprovadorDir = aprovador
                    Else
                        lblMensagem.Text = "Não foi possivel obter a informação de quem é o Diretor do projeto " &
                            tabelaRelatorio.DataKeys(i).Item(0).ToString
                        Return
                    End If
                End If

                ' Coletando o tipo de empresa, Razao Social e a empresa compartilhada se caso tiver
                SQL = "SELECT colTipoEmpresa, colRazaoSocial, colEmpresaCompartilhada FROM v_colaboradores WHERE colCodigo = " & colCodigo
                If Not selectSQL(SQL) Then
                    lblMensagem.Text = SQL & "<br />" & sqlErro
                    Return
                End If
                dr.Read()
                If Not IsDBNull(dr("colTipoEmpresa")) And dr("colTipoEmpresa").ToString.Trim <> "" Then
                    tipoEmpresa = "'" & dr("colTipoEmpresa") & "'"
                End If
                If Not IsDBNull(dr("colRazaoSocial")) And dr("colRazaoSocial").ToString.Trim <> "" Then
                    razaoSocial = "'" & dr("colRazaoSocial") & "'"
                End If
                If Not IsDBNull(dr("colEmpresaCompartilhada")) And dr("colEmpresaCompartilhada").ToString.Trim <> "" Then
                    empresaCompartilhada = "'" & dr("colEmpresaCompartilhada") & "'"
                End If

                SQL = "SELECT proCodigo FROM v_fechamento " &
                      "WHERE procodigo = " & proCodigo & " AND " &
                      "colCodigo = " & colCodigo & " AND " &
                      "fecDataInicio = '" & dataInicio & "' AND " &
                      "fecDataFim = '" & dataFim & "'"

                If selectSQL(SQL) Then
                    If Not dr.HasRows Then
                        SQL = "INSERT INTO tblFechamento (" &
                            " proCodigo, colCodigo, fecDataInicio, fecDataFim, fecAprovacaoGP, fecAprovacaoGC, fecAprovacaoDir," &
                            " fecAprovadorGP, fecAprovadorGC, fecAprovadorDir, fecTipoContrato, fecTaxaCustoCLT_PJ, fecTaxaHora," &
                            " fecTaxaProjeto, fecHorasTrabalhadas, fecValorTotal, fecTipoEmpresa, fecRazaoSocial, fecEmpresaCompartilhada" &
                            " ) VALUES ( " &
                            "" & proCodigo & "" &
                            "," & colCodigo & "" &
                            ",'" & dataInicio & "'" &
                            ",'" & dataFim & "'" &
                            ",'" & aprovacaoGP & "'" &
                            ",'" & aprovacaoGC & "'" &
                            ",'" & aprovacaoDir & "'" &
                            "," & aprovadorGP & "" &
                            "," & aprovadorGC & "" &
                            "," & aprovadorDir & "" &
                            ",'" & tipoContrato & "'" &
                            "," & taxaCusto & "" &
                            "," & taxaHora & "" &
                            ",null" &
                            ",'" & horasTrabalhadas & "'" &
                            "," & valorTotal & "" &
                            "," & tipoEmpresa & "" &
                            "," & razaoSocial & "" &
                            "," & empresaCompartilhada & "" &
                            " )"
                    End If
                Else
                    lblMensagem.Text = SQL & "<br />" & sqlErro
                    Return
                End If

                ' Verifica se houve algum erro na gravação no banco de dados e exibi 
                If Not comandoSQL(SQL) Then
                    lblMensagem.Text = SQL & "<br />" & sqlErro
                    Return
                Else
                    ' Serve para ver quantos linhas foram gravadas na tabela de fechamento
                    cont = cont + 1
                End If

            End If

        Next i

        If cont = 0 Then
            lblMensagem.Style.Add("color", "Blue")
            lblMensagem.Text = "Nenhum registro foi gravado, pois todos estão pendentes de aprovação."
        Else
            lblMensagem.Style.Add("color", "Blue")
            If cont = 1 Then
                lblMensagem.Text = "Fechamento realizado com sucesso. " & cont & " linha foi gravada na tabela de fechamento."
            Else
                lblMensagem.Text = "Fechamento realizado com sucesso. " & cont & " linhas foram gravadas na tabela de fechamento."
            End If

            preencheTabela(getCompetenciaSelecionada())

        End If

    End Sub

    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '   Reabre o projeto apagando o seu registro na tabela tblFechamento
    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Private Sub btnReabrir_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReabrir.Click

        dataInicio = Session("dataInicio")
        dataFim = dataInicio.AddMonths(1).AddDays(-1)

        For i = 0 To (tabelaRelatorio.DataKeys.Count - 1)

            proCodigo = tabelaRelatorio.DataKeys(i).Item(10).ToString
            colCodigo = tabelaRelatorio.DataKeys(i).Item(11).ToString

            SQL = "DELETE tblFechamento WHERE colCodigo = " & colCodigo & " " &
              "AND proCodigo = " & proCodigo & " " &
              "AND fecDataInicio = '" & dataInicio & "' " &
              "AND fecDataFim = '" & dataFim & "'"

            If Not comandoSQL(SQL) Then
                lblMensagem.Text = sqlErro
                Return
            End If

        Next i

        lblMensagem.Style.Add("color", "Blue")
        lblMensagem.Text = "Reabertura realizada com sucesso."

        preencheTabela(getCompetenciaSelecionada())

    End Sub

    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '   Função para coletar os status de aprovação do colaborador
    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Private Function getStatusAprovacao(ByVal colCodigo As String, ByVal proCodigo As String, ByVal dataInicio As DateTime) As ArrayList

        Dim array As New ArrayList
        Dim linha As DataRow
        Dim aprovacaoGP As String = "N"
        Dim aprovacaoGC As String = "N"
        Dim aprovacaoDir As String = "N"

        dataFim = dataInicio.AddMonths(1).AddDays(-1)

        SQL = "SELECT codGP, codGC, codDir FROM v_projetos WHERE proCodigo = " & proCodigo

        If selectSQL(SQL) Then
            dr.Read()
            If dr.HasRows Then
                If dr("codGP") IsNot DBNull.Value Or dr("codGP").ToString.Trim <> "" Then
                    aprovacaoGP = dr("codGP")
                End If
                If dr("codGC") IsNot DBNull.Value Or dr("codGC").ToString.Trim <> "" Then
                    aprovacaoGC = dr("codGC")
                End If
                If dr("codDir") IsNot DBNull.Value Or dr("codDir").ToString.Trim <> "" Then
                    aprovacaoDir = dr("codDir")
                End If
            End If
        End If

        SQL = "SELECT apoAprovacaoGP, apoAprovacaoGC, apoAprovacaoDir FROM v_apontamento WHERE colCodigo = " & colCodigo &
              " AND proCodigo = " & proCodigo & " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "' and apoTotal is not null"

        ' Verifica se em todas as colunas de aprovação esta como A de aprovado, R de reprovado, ou vazia que 
        ' significa que ainda não houve aprovação/reprovação
        If selectSQL(SQL) Then

            Dim dtStatusAprovacao As New DataTable("tblStatusAprovacao")

            dtStatusAprovacao.Columns.Add("aprovacaoGP", GetType(String))
            dtStatusAprovacao.Columns.Add("aprovacaoGC", GetType(String))
            dtStatusAprovacao.Columns.Add("aprovacaoDir", GetType(String))

            Dim numLinhas = 0
            While dr.Read
                dtStatusAprovacao.Rows.Add(New Object() {dr("apoAprovacaoGP").ToString.Trim, dr("apoAprovacaoGC").ToString.Trim,
                                                         dr("apoAprovacaoDir").ToString.Trim})
                numLinhas += 1
            End While

            Dim qtdA = 0
            Dim qtdR = 0
            Dim qtdPendencia = 0
            Dim qtd = 0

            If aprovacaoGP <> "N" Then

                For i = 0 To numLinhas - 1
                    linha = dtStatusAprovacao.Rows(i)
                    Select Case linha("aprovacaoGP").ToString.Trim
                        Case "A"
                            qtdA += 1
                        Case "R"
                            qtdR += 1
                        Case "P"
                            qtdPendencia += 1
                        Case ""
                            qtdPendencia += 1
                    End Select
                    qtd += 1
                Next i

                If qtd = qtdA Then
                    aprovacaoGP = "A"
                End If
                If qtd = qtdR Then
                    aprovacaoGP = "R"
                End If
                If qtd <> qtdA And qtd <> qtdR Then
                    aprovacaoGP = "P"
                End If

            End If

            ' Adiciona ao array no primeiro indice o Status de Aprovação do Gerente de projetos
            array.Add(aprovacaoGP)

            ' Resetando variaveis
            qtdA = 0
            qtdR = 0
            qtdPendencia = 0
            qtd = 0

            If aprovacaoGC <> "N" Then

                For i = 0 To numLinhas - 1
                    linha = dtStatusAprovacao.Rows(i)
                    Select Case linha("aprovacaoGC").ToString.Trim
                        Case "A"
                            qtdA += 1
                        Case "R"
                            qtdR += 1
                        Case "P"
                            qtdPendencia += 1
                        Case ""
                            qtdPendencia += 1
                    End Select
                    qtd += 1
                Next i

                If qtd = qtdA Then
                    aprovacaoGC = "A"
                End If
                If qtd = qtdR Then
                    aprovacaoGC = "R"
                End If
                If qtd <> qtdA And qtd <> qtdR Then
                    aprovacaoGC = "P"
                End If

            End If

            ' Adiciona ao array no segundo indice o Status de Aprovação do Gerente de contas
            array.Add(aprovacaoGC)

            ' Resetando variaveis
            qtdA = 0
            qtdR = 0
            qtdPendencia = 0
            qtd = 0

            If aprovacaoDir <> "N" Then

                For i = 0 To numLinhas - 1
                    linha = dtStatusAprovacao.Rows(i)
                    Select Case linha("aprovacaoDir").ToString.Trim
                        Case "A"
                            qtdA += 1
                        Case "R"
                            qtdR += 1
                        Case "P"
                            qtdPendencia += 1
                        Case ""
                            qtdPendencia += 1
                    End Select
                    qtd += 1
                Next i

                If qtd = qtdA Then
                    aprovacaoDir = "A"
                End If
                If qtd = qtdR Then
                    aprovacaoDir = "R"
                End If
                If qtd <> qtdA And qtd <> qtdR Then
                    aprovacaoDir = "P"
                End If

            End If

            ' Adiciona ao array no terceiro indice o Status de Aprovação da Diretoria
            array.Add(aprovacaoDir)

        Else
            ' Exibe o erro causado ao se conectar no banco de dados
            lblMensagem.Text = sqlErro
        End If

        ' Retorna o array com os três status de aporvação do consultor selecionado
        Return array

    End Function

    Private Sub tabelaRelatorio_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabelaRelatorio.DataBinding
        Dim lblTotal As Label = TryCast(Me.tabelaRelatorio.FindControl("lblTotal"), Label)
        lblTotal.Text = totalTudo.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
    End Sub

    Private Sub tabelaRelatorio_LayoutCreated(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabelaRelatorio.LayoutCreated
        Dim lblTotal As Label = TryCast(Me.tabelaRelatorio.FindControl("lblTotal"), Label)
        lblTotal.Text = totalTudo.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
    End Sub

    ' Função que retorna o total de horas apontadas durante o periodo passado como parametro
    Private Function getHorasApontadasTodosProjeto(ByVal colCodigo As Integer, ByVal dataInicio As Date) As String

        Dim strHorasApontadas As String = "00:00"
        Dim strHoras As String = "00:00"
        Dim arrayProCodigo As ArrayList = New ArrayList
        Dim dataFim As Date = dataInicio.AddMonths(1).AddDays(-1)
        Dim proCodigo As Integer
        Dim colTipoContrato = ""
        Dim Soma As Double = 0


        ' Select de todos os projetos apontados pelo colaborador e periodo dado
        SQL = "SELECT tblApontamento.colCodigo, tblApontamento.proCodigo, tblProjetos.proNome " &
              "FROM   tblApontamento tblApontamento " &
              "INNER JOIN  dbIntranet.dbo.tblProjetos tblProjetos " &
              "ON (tblApontamento.proCodigo = tblProjetos.proCodigo) " &
              "WHERE apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "' " &
              "AND tblApontamento.colCodigo = " & colCodigo & " " &
              "GROUP BY tblApontamento.colCodigo, tblApontamento.proCodigo, tblProjetos.proNome " &
              "ORDER BY tblProjetos.proNome "

        ' Grava em um array todos os códigos dos projetos que o consultor apontou no periodo selecionado
        If selectSQL(SQL) Then
            While dr.Read
                arrayProCodigo.Add(dr("proCodigo"))
            End While
        Else
            lblMensagem.Text = sqlErro
            Return "erro"
        End If

        ' Verifico qual é o tipo de contrato (CLT, PJ ou PJ Fixo)
        SQL = "SELECT colTipoContrato, colValorFechado FROM v_colaboradores WHERE colCodigo = " & colCodigo
        If selectSQL(SQL) Then
            dr.Read()
            If dr("colTipoContrato") = "PJ" Then
                If dr("colValorFechado") = "S" Then
                    colTipoContrato = "PJ - F"
                End If
            End If
        Else
            lblMensagem.Text = sqlErro
            Return "erro"
        End If

        For Each proCodigo In arrayProCodigo

            ' Se for PJ Fixo e se houver taxa por projeto, o calculo é feito assim com se fosse PJ, (taxa Projeto * horas trabalhadas)
            If colTipoContrato = "PJ - F" Then
                SQL = "SELECT colProTaxa FROM v_colaboradores_projeto WHERE proCodigo = " & proCodigo &
                      " AND colCodigo = " & colCodigo
                If selectSQL(SQL) Then
                    dr.Read()
                    If dr.HasRows Then
                        If IsDBNull(dr("colProTaxa")) Then
                            Soma += getHorasApontadasEmUmProjeto(colCodigo, proCodigo, dataInicio)
                            'strHoras = FomatarDataHora(getHorasApontadasEmUmProjeto(colCodigo, proCodigo, dataInicio))
                            'strHorasApontadas = somaHoras(strHorasApontadas, strHoras)
                            strHorasApontadas = strHoras
                        End If
                    End If
                Else
                    lblMensagem.Text = sqlErro
                    Return "erro"
                End If
            Else
                Soma += getHorasApontadasEmUmProjeto(colCodigo, proCodigo, dataInicio)
                'strHoras = FomatarDataHora(getHorasApontadasEmUmProjeto(colCodigo, proCodigo, dataInicio))
                'strHorasApontadas = somaHoras(strHorasApontadas, strHoras)
                strHorasApontadas = strHoras
            End If

        Next

        ' Retorna o total de horas trabalhadas em todos os projetos que o colaborador apontou pelo periodo informado 
        Return FomatarDataHora(Soma) ' strHorasApontadas

    End Function

    Private Function getHorasApontadasEmUmProjeto(ByVal colCodigo As Integer, ByVal proCodigo As Integer, ByVal dataInicio As Date) As Double


        Dim strHorasTrabalhadas As Double = 0
        Dim dataFim As Date = dataInicio.AddMonths(1).AddDays(-1)

        ' Coleta as horas trabalhadas por projeto
        SQL = "SELECT convert(varchar(5), apoTotal, 14) as 'apoTotal' FROM tblApontamento " &
              "WHERE apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "' AND colCodigo = " & colCodigo & " " &
              "AND apoTotal IS NOT NULL AND proCodigo = " & proCodigo

        If Not selectSQL(SQL) Then
            lblMensagem.Text = sqlErro
            Return "erro"
        End If

        ' Soma todos os campos apoTotal, ou seja, soma horas trabalhadas por projeto
        While dr.Read()
            strHorasTrabalhadas += somaHorasnew(dr("apoTotal"))
        End While

        ' Retorna o total de horas trabalhadas em todos os projetos que o colaborador apontou pelo periodo informado 
        Return strHorasTrabalhadas

    End Function

    ''' <summary>
    ''' qualAprovador = gp, gc ou dir
    ''' </summary>
    ''' <param name="colCodigo"></param>
    ''' <param name="proCodigo"></param>
    ''' <param name="dataInicio"></param>
    ''' <param name="qualAprovador"></param>
    ''' <remarks></remarks>
    Private Function getAprovador(colCodigo As Integer, proCodigo As Integer, dataInicio As Date, qualAprovador As String) As Boolean

        SQL = "SELECT apoAprovadorGP, apoAprovadorGC, apoAprovadorDir " &
              "FROM tblApontamento WHERE " &
              "colCodigo = " & colCodigo & " AND " &
              "proCodigo = " & proCodigo & " " &
              "AND apoData = '" & dataInicio & "' "

        If selectSQL(SQL) Then
            dr.Read()
            Dim i = 1
            ' Executo um loop caso não retorne nenhum registro do banco, pois significa que é uma data que não foi
            'realizado nenhum apontamento, então passo para proxima data até encontrar esta informação
            While Not dr.HasRows And i <= 31
                SQL = "SELECT apoAprovadorGP, apoAprovadorGC, apoAprovadorDir " &
                      "FROM tblApontamento WHERE " &
                      "colCodigo = " & colCodigo & " AND " &
                      "proCodigo = " & proCodigo & " " &
                      "AND apoData = '" & dataInicio.AddDays(i) & "' "
                If selectSQL(SQL) Then
                    dr.Read()
                Else
                    Return False
                End If
                i = i + 1
            End While
            ' If dr.HasRows Then
            Select Case qualAprovador
                Case "gp"
                    If dr("apoAprovadorGP") IsNot DBNull.Value Then
                        aprovador = dr("apoAprovadorGP")
                        Return True
                    Else
                        Return False
                    End If
                Case "gc"
                    If dr("apoAprovadorGC") IsNot DBNull.Value Then
                        aprovador = dr("apoAprovadorGC")
                        Return True
                    Else
                        Return False
                    End If
                Case "dir"
                    If dr("apoAprovadorDir") IsNot DBNull.Value Then
                        aprovador = dr("apoAprovadorDir")
                        Return True
                    Else
                        Return False
                    End If
                Case Else
                    Return False
            End Select
        Else
            Return False
            lblMensagem.Text = SQL & "<br />" & sqlErro
        End If

        Return False

    End Function
End Class