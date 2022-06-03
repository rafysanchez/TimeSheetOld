Imports System.Globalization
Public Class FechamentoBanco
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


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
                     "ON tblColaboradores.colCodigo = tblApontamento.colCodigo and  upper(tblColaboradores.colTipoContrato) in('CLT','PJ') and upper(tblColaboradores.colValorFechado)='S' " &
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
                      "ON tblColaboradores.colCodigo = tblApontamento.colCodigo and  upper(tblColaboradores.colTipoContrato) in('CLT','PJ') and upper(tblColaboradores.colValorFechado)='S' " &
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

            For i = 0 To (arrayColCodigo.Count - 1)

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

                ' Se não existir dados na tabela de fechamento então exiba a linha, ou seja, os apontamentos fechados
                ' somente exibirão na página de relatórios
                If dr("existeDado") = "False" Then

                    ' Coleta os nomes, salario e tipo de contrato, calcula também as horas trabalhadas
                    SQL = "SELECT proCodigo, colCodigo, proNome, colNome, colSalario, colTipoContrato, colValorFechado,apoBanco " &
                          "FROM v_relatorioApontamento " &
                          "WHERE proCodigo = " & arrayProCodigo(i) & " AND " &
                          "colCodigo = " & arrayColCodigo(i) & " AND " &
                          "apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

                    If selectSQL(SQL) Then
                        dr.Read()
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

                    '===============================================    Fim do IF que verifica dados na tabela tblFechamento 

                    ' Verifica se o conteudo da variavel colSalario é do tipo Decimal
                    If TypeOf taxaSalario Is Decimal Then

                        If Not cancelaCalculo Then

                            ' Se for PJ multiplica o valor/hora com o total de horas trabalhadas
                            If colunaTipoContrato = "PJ" Then
                                horasTrabalhadas = ApontadasEmUmProjetoBanco(colCodigo, proCodigo, dataInicio)

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

                                horasEmTodosProjetos = converteHorasDecimal(HorasApontadasTodosProjetoBanco(colCodigo, dataInicio))
                                horasTrabalhadas = ApontadasEmUmProjetoBanco(colCodigo, proCodigo, dataInicio)

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
                                        '  Divido seu salario com as horas trabalhadas para achar sua taxa/hora
                                        valorProporcional = taxaSalario * (((horasTrabalhadas * 100) / horasEmTodosProjetos) / 100)
                                        colunaValor = valorProporcional.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                        colunaTaxaHora = (valorProporcional / horasTrabalhadas).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                    Else
                                        cancelaExibicaoLinha = True
                                    End If
                                End If

                            End If

                            ' Se for CLT = (salario * (1 + (custoPercentual / 100))) / horasTrabalhadas 
                            '  Ex.: (1000 * (1 + (72 / 100))) / 168 = 10,23
                            If colunaTipoContrato = "CLT" Then
                                horasEmTodosProjetos = converteHorasDecimal(HorasApontadasTodosProjetoBanco(colCodigo, dataInicio))
                                horasTrabalhadas = ApontadasEmUmProjetoBanco(colCodigo, proCodigo, dataInicio)

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

                    ' Cancela Exibição da Linha quando não foi encontrado nenhum apontamento realizado
                    'If Not cancelaExibicaoLinha Then
                    '    dtRelatorio.Rows.Add(New Object() {
                    '       proNome, colNome, colunaGP, colunaGC, colunaDir,
                    '       colunaTipoContrato, colunaTaxaHora, "",
                    '       FomatarDataHora(horasTrabalhadas), colunaValor,
                    '       proCodigo, colCodigo, taxaCusto
                    '    })
                    'End If
                    dtRelatorio.Rows.Add(New Object() {
                           proNome, colNome, colunaGP, colunaGC, colunaDir,
                           colunaTipoContrato, colunaTaxaHora, "",
                           FomatarDataHora(horasTrabalhadas), colunaValor,
                           proCodigo, colCodigo, taxaCusto
                        })


                    cancelaCalculo = False
                    cancelaExibicaoLinha = False

                End If

            Next i

            ' Acionando a DataTable ao dtRelatorio
            tabelaRelatorio.DataSource = dtRelatorio
            tabelaRelatorio.DataBind()

        Else
            tabelaRelatorio.DataBind()
        End If

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
              " AND proCodigo = " & proCodigo & " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

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
        'Dim lblTotal As Label = TryCast(Me.tabelaRelatorio.FindControl("lblTotal"), Label)
        'lblTotal.Text = totalTudo.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
    End Sub

    Private Sub tabelaRelatorio_LayoutCreated(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabelaRelatorio.LayoutCreated
        'Dim lblTotal As Label = TryCast(Me.tabelaRelatorio.FindControl("lblTotal"), Label)
        'lblTotal.Text = totalTudo.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
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
                            strHorasApontadas = strHoras
                        End If
                    End If
                Else
                    lblMensagem.Text = sqlErro
                    Return "erro"
                End If
            Else
                Soma += getHorasApontadasEmUmProjeto(colCodigo, proCodigo, dataInicio)
                strHorasApontadas = strHoras
            End If

        Next

        ' Retorna o total de horas trabalhadas em todos os projetos que o colaborador apontou pelo periodo informado 
        Return FomatarDataHora(Soma) ' strHorasApontadas

    End Function
    Private Function HorasApontadasTodosProjetoBanco(ByVal colCodigo As Integer, ByVal dataInicio As Date) As String

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
                            Soma += ApontadasEmUmProjetoBanco(colCodigo, proCodigo, dataInicio)
                            strHorasApontadas = strHoras
                        End If
                    End If
                Else
                    lblMensagem.Text = sqlErro
                    Return "erro"
                End If
            Else
                Soma += ApontadasEmUmProjetoBanco(colCodigo, proCodigo, dataInicio)
                strHorasApontadas = strHoras
            End If

        Next

        Return FomatarDataHora(Soma)

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
    Private Function ApontadasEmUmProjetoBanco(ByVal colCodigo As Integer, ByVal proCodigo As Integer, ByVal dataInicio As Date) As Double


        Dim strHorasTrabalhadas As Double = 0
        Dim HorasSubtrair As Double = 0
        Dim dataFim As Date = dataInicio.AddMonths(1).AddDays(-1)

        ' Coleta as horas trabalhadas por projeto
        SQL = "SELECT apoBanco FROM tblApontamento " &
              "WHERE apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "' AND colCodigo = " & colCodigo & " " &
              "AND apoBanco IS NOT NULL AND proCodigo = " & proCodigo & " and apoBanco is not null"

        If Not selectSQL(SQL) Then
            lblMensagem.Text = sqlErro
            Return "erro"
        End If

        Dim Horas = New List(Of String)

        ' soma e 
        While dr.Read()
            Horas.Add(dr("apoBanco").ToString())
        End While

        'separa as horas negativas das positivas
        Dim HorasNegativas = New List(Of String)
        Dim HorasPositivas = New List(Of String)
        Dim Hora As String = String.Empty

        HorasNegativas = (From dados In Horas Where dados.IndexOf("-") >= 0).ToList()
        HorasPositivas = Horas.Except(HorasNegativas).ToList()

        If HorasNegativas.Count > 0 Then
            For Each Hora In HorasNegativas
                strHorasTrabalhadas += somaHorasnew(Hora.Replace("-", ""))
            Next

        End If

        If HorasPositivas.Count > 0 Then
            For Each Hora In HorasPositivas
                HorasSubtrair += somaHorasnew(Hora)
            Next
        End If

        If strHorasTrabalhadas > HorasSubtrair Then
            strHorasTrabalhadas = (strHorasTrabalhadas - HorasSubtrair)
            strHorasTrabalhadas = strHorasTrabalhadas * -1
        ElseIf HorasSubtrair > strHorasTrabalhadas Then
            strHorasTrabalhadas = (HorasSubtrair - strHorasTrabalhadas)

        End If

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