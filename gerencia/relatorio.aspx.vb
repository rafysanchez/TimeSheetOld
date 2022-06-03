Imports System.Globalization
Imports System.IO
Imports System.Web

Partial Public Class relatorio
    Inherits System.Web.UI.Page

    Dim SQL As String
    Dim dataInicio As DateTime
    Dim dataFim As DateTime
    Dim primeiroDia As String = "01"
    Dim totalTudo As Decimal = 0
    ' ''''''' Criando uma DataTable
    Dim dtRelatorio As New DataTable("consultores")
    Dim array As New ArrayList()
    Dim arrayProCodigo As New ArrayList()
    Dim arrayColCodigo As New ArrayList()
    ' Array para guardar quais são os aprovadores presente no select feito
    Dim arrayAprovadores As ArrayList = New ArrayList
    Dim proCodigo As String
    Dim colCodigo As String
    Dim periodoSelecionado As Date
    Dim primeiroAnoSistema As Integer = 2011

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

#If DEBUG Then
        ' Select para teste, simula que algum consultor já tenha feito o login
        'Session("colCodigoLogado") = 122
        SQL = "SELECT colNome, colPerfil FROM v_colaboradores WHERE colCodigo = " & Session("colCodigoLogado")
        If selectSQL(SQL) Then
            If dr.HasRows Then
                dr.Read()
                Session("colPerfilLogado") = dr("colPerfil")
                Session("colNomeLogado") = dr("colNome")
            Else
                lblMensagem.Text = "Não há dados com o código de colaborador informado... código = " & Session("colCodigoLogado")
            End If
        Else
            lblMensagem.Text = SQL & "<br />" & sqlErro
        End If
#Else
        Try
            If Not Session("permissao").ToString.Contains("Relatorio") Then
                Response.Redirect("..\Default.aspx")
            End If
        Catch ex As Exception
            Response.Redirect("..\Default.aspx")
        End Try
#End If

        'Dim controle As Control
        'controle = ("corpo")
        'CType(controle, HtmlGenericControl).Style.Add("width", "auto")

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
            If Session("colPerfilLogado").ToString.ToLower = "diretoria" _
                                            Or Session("colPerfilLogado").ToString.ToLower = "csc" Then
                SQL = "SELECT proNome, proCodigo FROM v_projetos WHERE upper(proStatus)='ATIVO'  ORDER BY proNome"
            Else
                SQL = "SELECT proNome, proCodigo FROM v_Projetos WHERE upper(proStatus)='ATIVO' AND  codGC = " & Session("colCodigoLogado")
            End If

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

            ddlAprovadores.Enabled = False  ' Desabilita o DropDownList "Aprovadores"
            ddlAprovadores.Items.Clear()    ' Limpa o DropDownList "Aprovadores"
            ddlAprovadores.Items.Add("")    ' Adiciona uma linha em branco o DropDownList "Aprovadores"

            divBotoes.Style.Add("display", "none")

            preencheTabela(getCompetenciaSelecionada())

        End If

        lblMensagem.Style.Add("color", "Red")
        lblMensagem.Text = ""

        ' Esta linha resolve o problema da mensagem de erro quando download de arquivos
        ScriptManager.GetCurrent(Me).RegisterPostBackControl(btnCriarPedidoSAP)
        ScriptManager.GetCurrent(Me).RegisterPostBackControl(btnExportarExcel)

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

        lblPeriodoSelecionado.Text = "Período selecionado: <b style='color:#ED7522;'>" & data & "</b>" & _
                                 " a <b style='color:#ED7522;'>" & data.AddMonths(1).AddDays(-1) & "</b>."

        Return data

    End Function

    Private Sub ddlProjeto_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlProjeto.SelectedIndexChanged

        preencheTabela(getCompetenciaSelecionada())

    End Sub

    Private Sub ddlMes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMes.SelectedIndexChanged

        Dim diaSelecionado As String = ddlDiaInicial.Text

        ddlDiaInicial.Items.Clear()
        For i = 1 To System.DateTime.DaysInMonth(ddlAno.SelectedValue, ddlMes.SelectedValue)
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
                    ddlDiaInicial.Text = j
                    Exit While
                Catch ex As Exception
                    j = j - 1
                End Try
            End While
        Else
            ddlDiaInicial.Text = diaSelecionado
        End If

        atualizaLabelPeriodoSelecionado()
        'preencheTabela()

    End Sub

    Private Sub ddlDiaInicial_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDiaInicial.SelectedIndexChanged
        atualizaLabelPeriodoSelecionado()
    End Sub

    Private Sub ddlAno_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAno.SelectedIndexChanged
        atualizaLabelPeriodoSelecionado()
    End Sub

    Private Sub atualizaLabelPeriodoSelecionado()

        preencheTabela(getCompetenciaSelecionada())

    End Sub

    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '   Subrotina para preencher a tabela do relatório
    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Private Sub preencheTabela(ByVal dataInicio As Date)

        primeiroDia = ddlDiaInicial.SelectedValue
        Dim colCodigoSelecionado As Integer = 0

        ' Guarda em sessão para ser usada em sub rotinas 
        Session("dataInicio") = dataInicio
        dataFim = dataInicio.AddMonths(1).AddDays(-1)

        dtRelatorio.Clear()

        ' ''''''' Criando as colunas para a DataTable 
        With dtRelatorio
            .Columns.Add("statusCriadoPedidoSAP", GetType(String))
            .Columns.Add("proNome", GetType(String))
            .Columns.Add("nome", GetType(String))
            .Columns.Add("GP", GetType(String))
            .Columns.Add("GC", GetType(String))
            .Columns.Add("Dir", GetType(String))
            .Columns.Add("colTipoContrato", GetType(String))
            .Columns.Add("colSalario", GetType(String))
            .Columns.Add("taxaProjeto", GetType(String))
            .Columns.Add("horasTrabalhadas", GetType(String))
            .Columns.Add("valorTotal", GetType(String))
            .Columns.Add("proCodigo", GetType(String))
            .Columns.Add("colCodigo", GetType(String))
            .Columns.Add("razaoSocial", GetType(String))
            .Columns.Add("empresaCompartilhada", GetType(String))
        End With

        Dim dtParaExcel As New DataTable("excel")
        ' ''''''' Criando as colunas para a DataTable 
        With dtParaExcel
            .Columns.Add("Nome do Projeto", GetType(String))
            .Columns.Add("Nome do Colaborador", GetType(String))
            .Columns.Add("Contrato", GetType(String))
            .Columns.Add("Taxa", GetType(String))
            .Columns.Add("Taxa Projeto", GetType(String))
            .Columns.Add("Horas", GetType(String))
            .Columns.Add("Valor", GetType(String))
            .Columns.Add("Razão Social", GetType(String))
            .Columns.Add("Empresa Compartilhada", GetType(String))
        End With

        ' Só continua se no comboBox ddlMes estiver selecionado algo
        If ddlProjeto.SelectedIndex > 0 Then

            ' Se selecionado "Exibir Todos"
            If ddlProjeto.SelectedIndex = 1 Then
                ' Foi selecionado para exibir todos os projetos
                SQL = "SELECT * FROM v_relatorioFechamento WHERE " & _
                      "fecDataInicio = '" & dataInicio & "' AND " & _
                      "fecDataFim = '" & dataFim & "' "
            Else
                ' Select para coletar todos os consultores que fizeram apontamento no projeto e mês selecionado
                SQL = "SELECT * FROM v_relatorioFechamento WHERE " & _
                      "proCodigo = " & ddlProjeto.SelectedValue & " AND " & _
                      "fecDataInicio = '" & dataInicio & "' AND " & _
                      "fecDataFim = '" & dataFim & "' "
            End If

            ' ======== Verifica os filtros

            ' Se tem algo prenchido no campo de pesquisa então exibe o primeiro nome dado
            If txtPesquisaNome.Text.Trim <> "" Then
                Dim SQL2 = "SELECT TOP 1 colCodigo FROM v_colaboradores " & _
                      "WHERE colNome COLLATE Latin1_General_CI_AI LIKE '%" & txtPesquisaNome.Text & "%'"
                If selectSQL(SQL2) Then
                    ' Se houver retorno na pesquisa adiciona mais um filtro no SELECT
                    If dr.HasRows Then
                        dr.Read()
                        colCodigoSelecionado = dr("colCodigo")
                        SQL += "AND colCodigo = " & colCodigoSelecionado & " "
                    Else
                        lblMensagem.Text = "Nome de colaborador não encontrado..."
                        tabelaRelatorio.DataBind()
                        Return
                    End If
                Else
                    lblMensagem.Text = SQL & "<br />" & sqlErro
                    Return
                End If
            End If

            ' Verifica o que foi selecionado em ddlTipoContrato
            If ddlTipoContrato.SelectedIndex > 0 Then
                Select Case ddlTipoContrato.SelectedIndex
                    Case 1 ' Selecionado PJ
                        SQL += "AND fecTipoContrato = 'PJ' "
                    Case 2 ' Selecionado PJ Fechado
                        SQL += "AND fecTipoContrato = 'PJ - F' "
                    Case 3 ' Selecionado CLT
                        SQL += "AND fecTipoContrato = 'CLT' "
                End Select
            End If

            ' Verifica o que foi selecionado em ddlAprovadores
            If ddlAprovadores.SelectedIndex > 0 Then
                If Not selectSQL("SELECT perCodigo, colNome FROM v_colaboradores WHERE colCodigo = " & ddlAprovadores.SelectedValue) Then
                    lblMensagem.Text = SQL & "<br />" & sqlErro
                    Return
                End If
                dr.Read()
                Select Case dr("perCodigo")
                    Case 2 ' GP
                        SQL += "AND fecAprovadorGP = " & ddlAprovadores.SelectedValue & " "
                    Case 3 ' GC
                        SQL += "AND fecAprovadorGC = " & ddlAprovadores.SelectedValue & " "
                    Case 4 ' Dir
                        SQL += "AND fecAprovadorDir = " & ddlAprovadores.SelectedValue & " "
                End Select
            End If

            SQL += "ORDER BY proNome "

            ' ======== Fim de verifica filtros

            ' Loop para coletar todos os códigos dos consultores e projeto através do que o usuário selecionou
            If selectSQL(SQL) Then
                While dr.Read()
                    ' Se for "Diretoria" e "CSC" exibe todos os projetos
                    If Session("colPerfilLogado").ToString.ToLower = "diretoria" Or _
                       Session("colPerfilLogado").ToString.ToLower = "csc" Then
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
                lblMensagem.Text = SQL & "<br />" & sqlErro
            End If

            For i = 0 To (arrayColCodigo.Count - 1)

                Dim proCodigo As String = ""
                Dim colCodigo As String = ""
                Dim proNome As String = ""
                Dim colNome As String = ""
                Dim colSalario
                Dim colunaTipoContrato As String = ""
                Dim colunaValor As String = "0,00"
                Dim valorProporcional As Decimal = 0
                Dim horasTrabalhadas = "00:00"
                Dim colunaGP As String = ""
                Dim colunaGC As String = ""
                Dim colunaDir As String = ""
                Dim aprovadorGP = "0"
                Dim aprovadorGC = "0"
                Dim aprovadorDir = "0"
                Dim colunaTaxaHora As String = "0,00"
                Dim razaoSocial = ""
                Dim empresaCompartilhada = ""
                Dim horasEmTodosProjetos As Decimal = "0"
                Dim criadoPedidoSAP = ""

                '   Verifico se através do proCodigo, colCodigo e periodo selecionado, já exista dados na tabela tblFechamento,
                ' se caso exista somente preenche o dataTable com os resultados
                SQL = "SELECT * FROM v_fechamento WHERE " & _
                      "proCodigo = " & arrayProCodigo(i) & " AND " & _
                      "colCodigo = " & arrayColCodigo(i) & " AND " & _
                      "fecDataInicio = '" & dataInicio & "' AND " & _
                      "fecDataFim = '" & dataFim & "' "

                If selectSQL(SQL) Then
                    '====================================  IF que verifica se já existe dados na tabela tblFechamento 
                    If dr.HasRows Then
                        SQL = "SELECT * FROM v_relatorioFechamento WHERE " & _
                              "proCodigo = " & arrayProCodigo(i) & " AND " & _
                              "colCodigo = " & arrayColCodigo(i) & " AND " & _
                              "fecDataInicio = '" & dataInicio & "' AND " & _
                              "fecDataFim = '" & dataFim & "'"

                        If selectSQL(SQL) Then
                            dr.Read()

                            If Not IsDBNull(dr("fecRazaoSocial")) Then
                                razaoSocial = dr("fecRazaoSocial")
                            End If
                            If Not IsDBNull(dr("fecEmpresaCompartilhada")) Then
                                empresaCompartilhada = dr("fecEmpresaCompartilhada")
                            End If

                            ' Campos que tenho certeza que não estarão nulos
                            proCodigo = dr("proCodigo")
                            colCodigo = dr("colCodigo")

                            ' IFs que fazem a verificação de campos nulos
                            If dr("proNome") IsNot DBNull.Value Then
                                proNome = dr("proNome")
                            End If
                            If dr("colNome") IsNot DBNull.Value Then
                                colNome = dr("colNome")
                            End If
                            If dr("fecAprovacaoGP") IsNot DBNull.Value Then
                                colunaGP = dr("fecAprovacaoGP")
                                If colunaGP <> "N" Then
                                    If dr("fecAprovadorGP") IsNot DBNull.Value Then
                                        arrayAprovadores.Add(dr("fecAprovadorGP"))
                                        aprovadorGP = dr("fecAprovadorGP")
                                    End If
                                End If
                            End If
                            If dr("fecAprovacaoGC") IsNot DBNull.Value Then
                                colunaGC = dr("fecAprovacaoGC")
                                If colunaGC <> "N" Then
                                    If dr("fecAprovadorGC") IsNot DBNull.Value Then
                                        arrayAprovadores.Add(dr("fecAprovadorGC"))
                                        aprovadorGC = dr("fecAprovadorGC")
                                    End If
                                End If
                            End If
                            If dr("fecAprovacaoDir") IsNot DBNull.Value Then
                                colunaDir = dr("fecAprovacaoDir")
                                If colunaDir <> "N" Then
                                    If dr("fecAprovadorDir") IsNot DBNull.Value Then
                                        arrayAprovadores.Add(dr("fecAprovadorDir"))
                                        aprovadorDir = dr("fecAprovadorDir")
                                    End If
                                End If
                            End If
                            If dr("fecTipoContrato") IsNot DBNull.Value Then
                                colunaTipoContrato = dr("fecTipoContrato")
                            End If
                            If dr("fecTaxaHora") IsNot DBNull.Value Then
                                colSalario = Decimal.Parse(dr("fecTaxaHora")).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                                colunaTaxaHora = colSalario
                            Else
                                colSalario = "N"
                            End If
                            If dr("fecHorasTrabalhadas") IsNot DBNull.Value Then
                                horasTrabalhadas = dr("fecHorasTrabalhadas")
                            End If
                            If dr("fecValorTotal") IsNot DBNull.Value Then
                                colunaValor = Decimal.Parse(dr("fecValorTotal")).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                            End If

                            ' Verifica se tem todos os dados necessarios para fazer o pedido de compra no SAP
                            ' Ainda não foi especificado se ira fazer ou não esta funcionalidade

                            criadoPedidoSAP = dr("fecCriadoPedidoSAP")

                            colunaValor = colunaValor.ToString.Replace("R$ ", "")
                            colunaTaxaHora = colunaTaxaHora.ToString.Replace("R$ ", "")
                            ' Variavel exibida no rodapé da tabela
                            totalTudo = totalTudo + colunaValor

                            Select Case colunaGP
                                Case "A"
                                    colunaGP = "<b style='color:Green;'   title='" & getNomeColaborador(aprovadorGP) & "' >A</b>"
                                Case "R"
                                    colunaGP = "<b style='color:Red;'     title='" & getNomeColaborador(aprovadorGP) & "' >R</b>"
                                Case "N"
                                    colunaGP = "<b style='color:Gray;'    title='" & getNomeColaborador(aprovadorGP) & "' >N</b>"
                                Case "P"
                                    colunaGP = "<b style='color:#E8C500;' title='" & getNomeColaborador(aprovadorGP) & "' >P</b>"
                            End Select
                            Select Case colunaGC
                                Case "A"
                                    colunaGC = "<b style='color:Green;'   title='" & getNomeColaborador(aprovadorGC) & "' >A</b>"
                                Case "R"
                                    colunaGC = "<b style='color:Red;'     title='" & getNomeColaborador(aprovadorGC) & "' >R</b>"
                                Case "N"
                                    colunaGC = "<b style='color:Gray;'    title='" & getNomeColaborador(aprovadorGC) & "' >N</b>"
                                Case "P"
                                    colunaGC = "<b style='color:#E8C500;' title='" & getNomeColaborador(aprovadorGC) & "' >P</b>"
                            End Select
                            Select Case colunaDir
                                Case "A"
                                    colunaDir = "<b style='color:Green;'   title='" & getNomeColaborador(aprovadorDir) & "'>A</b>"
                                Case "R"
                                    colunaDir = "<b style='color:Red;'     title='" & getNomeColaborador(aprovadorDir) & "'>R</b>"
                                Case "N"
                                    colunaDir = "<b style='color:Gray;'    title='" & getNomeColaborador(aprovadorDir) & "'>N</b>"
                                Case "P"
                                    colunaDir = "<b style='color:#E8C500;' title='" & getNomeColaborador(aprovadorDir) & "'>P</b>"
                            End Select

                            Dim expRegular = "([0-9]+|\d):[0-5]\d" ' Aceita no formato de horas, por exemplo "999:59"

                            If Not Regex.IsMatch(horasTrabalhadas, expRegular) Then
                                ' Converte o valor que vem em decimal para o formato de horas Ex.: 18.5 = 18:00
                                horasTrabalhadas = converteDecimalHoras(horasTrabalhadas)
                            End If

                            dtRelatorio.Rows.Add(New Object() { _
                               criadoPedidoSAP, proNome, colNome, colunaGP, colunaGC, colunaDir, _
                               colunaTipoContrato, colunaTaxaHora, "", _
                               horasTrabalhadas, colunaValor, _
                               proCodigo, colCodigo, razaoSocial, empresaCompartilhada _
                            })

                            dtParaExcel.Rows.Add(New Object() { _
                               proNome, colNome, _
                               colunaTipoContrato, colunaTaxaHora, "", _
                               horasTrabalhadas, colunaValor, _
                               razaoSocial, empresaCompartilhada _
                            })

                        Else
                            lblMensagem.Text = SQL & "<br />" & sqlErro
                            Exit For
                        End If

                    End If
                Else
                    lblMensagem.Text = SQL & "<br />" & sqlErro
                    Exit For
                End If
                '===============================================    Fim do IF que verifica dados na tabela tblFechamento 

            Next i

            sqlConn.Close()

            ' Acionando a DataTable ao dtRelatorio
            tabelaRelatorio.DataSource = dtRelatorio
            tabelaRelatorio.DataBind()

            Session("dtParaExcel") = dtParaExcel

        Else
            tabelaRelatorio.DataBind()
        End If

        ' Habilito o botão de reabrir fechamento somente para perfis "diretoria" ou "csc"
        If dtRelatorio.Rows.Count > 0 Then
            ' Se for Diretoria e CSC poderá realizar fechamentos e aberturas
            If Session("colPerfilLogado").ToString.ToLower = "diretoria" Or Session("colPerfilLogado").ToString.ToLower = "csc" Then
                divBotoes.Style.Add("display", "block")
            End If
        Else
            divBotoes.Style.Add("display", "none")
        End If

        preencheDDLAprovadores(dataInicio)

    End Sub

    Private Sub preencheDDLAprovadores(ByVal dataInicio As Date)

        Dim dataFim As Date = dataInicio.AddMonths(1).AddDays(-1)

        ' Só continua se no comboBox ddlMes estiver selecionado algo
        If ddlProjeto.SelectedIndex > 0 Then

            ' Se selecionado "Exibir Todos"
            If ddlProjeto.SelectedIndex = 1 Then
                ' Foi selecionado para exibir todos os projetos
                SQL = "SELECT * FROM v_relatorioFechamento WHERE " & _
                      "fecDataInicio = '" & dataInicio & "' AND " & _
                      "fecDataFim = '" & dataFim & "' "
            Else
                ' Select para coletar todos os consultores que fizeram apontamento no projeto e mês selecionado
                SQL = "SELECT * FROM v_relatorioFechamento WHERE " & _
                      "proCodigo = " & ddlProjeto.SelectedValue & " AND " & _
                      "fecDataInicio = '" & dataInicio & "' AND " & _
                      "fecDataFim = '" & dataFim & "' "
            End If

            SQL += "ORDER BY proNome "

            ' Loop para coletar todos os códigos dos consultores e projeto através do que o usuário selecionou
            If selectSQL(SQL) Then
                While dr.Read()
                    ' Se for "Diretoria" e "CSC" exibe todos os projetos
                    If Session("colPerfilLogado").ToString.ToLower = "diretoria" Or _
                       Session("colPerfilLogado").ToString.ToLower = "csc" Then
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
                lblMensagem.Text = SQL & "<br />" & sqlErro
            End If

            For i = 0 To (arrayColCodigo.Count - 1)

                '   Verifico se através do proCodigo, colCodigo e periodo selecionado, já exista dados na tabela tblFechamento,
                ' se caso exista somente preenche o dataTable com os resultados
                SQL = "SELECT * FROM v_fechamento WHERE " & _
                      "proCodigo = " & arrayProCodigo(i) & " AND " & _
                      "colCodigo = " & arrayColCodigo(i) & " AND " & _
                      "fecDataInicio = '" & dataInicio & "' AND " & _
                      "fecDataFim = '" & dataFim & "' "

                If selectSQL(SQL) Then
                    '====================================  IF que verifica se já existe dados na tabela tblFechamento 
                    If dr.HasRows Then
                        SQL = "SELECT * FROM v_relatorioFechamento WHERE " & _
                              "proCodigo = " & arrayProCodigo(i) & " AND " & _
                              "colCodigo = " & arrayColCodigo(i) & " AND " & _
                              "fecDataInicio = '" & dataInicio & "' AND " & _
                              "fecDataFim = '" & dataFim & "'"

                        If selectSQL(SQL) Then
                            dr.Read()

                            If dr("fecAprovacaoGP") IsNot DBNull.Value Then
                                If dr("fecAprovacaoGP") <> "N" Then
                                    arrayAprovadores.Add(dr("fecAprovadorGP"))
                                End If
                            End If
                            If dr("fecAprovacaoGC") IsNot DBNull.Value Then
                                If dr("fecAprovacaoGC") <> "N" Then
                                    arrayAprovadores.Add(dr("fecAprovadorGC"))
                                End If
                            End If
                            If dr("fecAprovacaoDir") IsNot DBNull.Value Then
                                If dr("fecAprovacaoDir") <> "N" Then
                                    arrayAprovadores.Add(dr("fecAprovadorDir"))
                                End If
                            End If

                        Else
                            lblMensagem.Text = SQL & "<br />" & sqlErro
                            Exit For
                        End If

                    End If
                Else
                    lblMensagem.Text = SQL & "<br />" & sqlErro
                    Exit For
                End If
                '===============================================    Fim do IF que verifica dados na tabela tblFechamento 

            Next i

            Dim aprovadorSelecionado = ddlAprovadores.SelectedValue

            ddlAprovadores.Enabled = False
            ddlAprovadores.Items.Clear()    ' Limpa o DropDownList "Aprovadores"
            ddlAprovadores.Items.Add("Todos")    ' Adiciona uma linha em branco o DropDownList "Aprovadores"

            ' Preenche o DropDownList "Aprovadores" para adicionar como filtro na pesquisa
            For i = 0 To (arrayAprovadores.Count - 1)

                SQL = "SELECT colNome, colCodigo FROM v_colaboradores WHERE colCodigo = " & arrayAprovadores(i)

                If SQL = "SELECT colNome, colCodigo FROM v_colaboradores WHERE colCodigo = " Then
                    Dim teste = ""
                End If

                If selectSQL(SQL) Then
                    dr.Read()
                    If Not ddlAprovadores.Items.Contains(New ListItem(dr("colNome"), dr("colCodigo"))) Then
                        ddlAprovadores.Enabled = True
                        ddlAprovadores.Items.Add(New ListItem(dr("colNome"), dr("colCodigo")))
                    End If
                Else
                    lblMensagem.Text = SQL & "<br />" & sqlErro
                    Return
                End If

            Next i

            Try
                ddlAprovadores.SelectedValue = aprovadorSelecionado
            Catch ex As Exception
            End Try

        Else

            ddlAprovadores.Enabled = False
            ddlAprovadores.Items.Clear()    ' Limpa o DropDownList "Aprovadores"

        End If

    End Sub

    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '   Cria Pedido para o SAP salvando um arquivo em CSV para o SAP
    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Private Sub btnCriarPedidoSAP_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCriarPedidoSAP.Click

        Dim strCSV = ""

        ' Com esta array, coleto todos os codigos do campo fecCodidoSequencial para depois atualizar a informação se foi feito o
        ' pedido no SAP ou não no campo fecCriadoPedidoSAP
        Dim arrayCodSequencial As New ArrayList()

        dataInicio = getCompetenciaSelecionada()
        dataFim = dataInicio.AddMonths(1).AddDays(-1)

        ' Loop que percorre todas as linhas da tabela de relatório coletando todos os dados para depois
        For i = 0 To (tabelaRelatorio.DataKeys.Count - 1)
            Try
                ' Somente os PJ e PJ - F que irão para pedido do SAP
                If tabelaRelatorio.DataKeys(i).Item(6).ToString.ToUpper <> "CLT" Then

                    ' Dados que precisarei para geração do CSV
                    Dim codSequencial = ""
                    Dim proNome = ""
                    Dim centroCusto = ""
                    Dim centroLucro = ""
                    Dim colNome = ""
                    Dim colCodigoSAP = ""
                    Dim taxaHora = ""
                    Dim horasTrabalhadas = ""
                    Dim valorTotal = ""
                    Dim colRazaoSocial = ""
                    Dim colCodigoEmpresaSAP = ""
                    Dim colEmpresaCompartilhada = ""

                    proCodigo = tabelaRelatorio.DataKeys(i).Item(11).ToString
                    colCodigo = tabelaRelatorio.DataKeys(i).Item(12).ToString

                    SQL = "SELECT * FROM v_relatorioFechamento WHERE " & _
                          "colCodigo = " & colCodigo & " AND proCodigo = " & proCodigo & " AND fecDataInicio = '" & dataInicio & "'"

                    If Not selectSQL(SQL) Then
                        lblMensagem.Text = sqlErro
                        Return
                    Else
                        dr.Read()
                    End If

                    codSequencial = dr("fecCodigoSequencial")
                    proNome = dr("proNome")
                    centroCusto = dr("proCentroCusto")
                    centroLucro = centroCusto
                    colNome = dr("colNome")
                    colCodigoSAP = dr("colCodigoSAP")
                    taxaHora = dr("fecTaxaHora")
                    horasTrabalhadas = dr("fecHorasTrabalhadas")
                    valorTotal = dr("fecValorTotal")
                    colRazaoSocial = dr("colRazaoSocial")
                    colCodigoEmpresaSAP = dr("colCodigoEmpresaSAP")
                    If Not IsDBNull(dr("colEmpresaCompartilhada")) Then
                        If dr("colEmpresaCompartilhada").ToString.Trim <> "" Then
                            colEmpresaCompartilhada = dr("colEmpresaCompartilhada")
                        End If
                    End If

                    strCSV += codSequencial & ";" & proNome & ";" & centroCusto & ";" & centroLucro & ";" & _
                              colNome & ";" & colCodigoSAP & ";" & taxaHora & ";" & horasTrabalhadas & ";" & _
                              valorTotal & ";" & colRazaoSocial & ";" & colCodigoEmpresaSAP & ";" & _
                              colEmpresaCompartilhada & vbCrLf

                    arrayCodSequencial.Add(codSequencial)

                End If

            Catch ex As Exception
                lblMensagem.Text = ex.Message()
                Return
            End Try

        Next i

        Dim diretorio As String
        Dim nomeArquivo As String

        diretorio = "c:\_PedidosSAP"
        nomeArquivo = "ped_compra_sap_competencia_" & DateAndTime.MonthName(dataFim.Month, True).ToUpper & _
                      "_" & DateAndTime.Year(dataFim) & ".csv"

        ' Para copiar o arquivo com acentuação correta
        Dim encoding As Encoding = encoding.GetEncoding("ISO-8859-1")

        Try
            ' Cria o diretorio caso ele não exista
            If Not Directory.Exists(diretorio) Then
                Directory.CreateDirectory(diretorio)
            End If

            ' Copia o arquivo de texto no diretorio especificado
            Dim objStream As New FileStream(diretorio & "\" & nomeArquivo, IO.FileMode.OpenOrCreate)
            Dim Arq As New StreamWriter(objStream, encoding)

            ' Copia o arquivo
            Arq.Write(strCSV)
            Arq.Close()

            lblMensagem.Style.Add("color", "blue")
            lblMensagem.Text = "Arquivo " & nomeArquivo & " gravado com sucesso..."

            ' Atualiza o banco de dados com a informação de quais linhas foram para o arquivo CSV, assim não será permitido para 
            ' estas linhas o reabrimento
            For i = 0 To (arrayCodSequencial.Count - 1)

                SQL = "UPDATE tblFechamento SET fecCriadoPedidoSAP = 'S' WHERE fecCodigoSequencial = " & arrayCodSequencial(i)

                If Not comandoSQL(SQL) Then
                    lblMensagem.Text = sqlErro
                    Return
                End If

            Next i

            downloadArquivo(diretorio & "\" & nomeArquivo)

            ' Preenche novamente para ficar atualizado o status de envio para pedido de compra no SAP
            preencheTabela(dataInicio)

        Catch ex As Exception
            lblMensagem.Text = "Não foi possivel gerar o arquivo, ou ele esta afpAberto, ou o endereço de destino não existe."
            lblMensagem.Text = ex.Message()
        End Try

    End Sub

    ''' <summary>
    ''' Rotina para forçar o download de arquivos
    ''' </summary>
    ''' <param name="caminhoArquivo">Caminho para o arquivo no sistema de arquivos</param>
    ''' <param name="contentType">Content-Type do arquivo (opcional)</param>
    Private Sub downloadArquivo(ByVal caminhoArquivo As String, _
                                  Optional ByVal contentType As String = "application/octet-stream")

        Dim arquivo As FileInfo = New FileInfo(caminhoArquivo)
        Response.Clear()
        Response.AddHeader("Content-Disposition", "attachment; filename=" + arquivo.Name)
        Response.AddHeader("Content-Length", arquivo.Length.ToString())
        Response.ContentType = contentType
        Response.WriteFile(arquivo.FullName)
        Response.End()
    End Sub

    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '   Reabre o projeto apagando o seu registro na tabela tblFechamento
    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Private Sub btnReabrir_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReabrir.Click

        dataInicio = Session("dataInicio")
        dataFim = dataInicio.AddMonths(1).AddDays(-1)

        ' For i = 0 To (tabelaRelatorio.DataKeys.Count - 1)
        For Each linhaTabela As DataKey In tabelaRelatorio.DataKeys

            Dim teste = linhaTabela.Item(0)

            If linhaTabela.Item(0) = "N" Then

                proCodigo = linhaTabela.Item(11)
                colCodigo = linhaTabela.Item(12)

                SQL = "DELETE tblFechamento WHERE colCodigo = " & colCodigo & " " & _
                  "AND proCodigo = " & proCodigo & " " & _
                  "AND fecDataInicio = '" & dataInicio & "' " & _
                  "AND fecDataFim = '" & dataFim & "'"

                If Not comandoSQL(SQL) Then
                    lblMensagem.Text = SQL & "<br />" & sqlErro
                    Return
                End If

            End If

        Next

        lblMensagem.Style.Add("color", "Blue")
        lblMensagem.Text = "Reabertura dos que ainda não foram feito Pedido de Compra no SAP realizada com sucesso."

        preencheTabela(getCompetenciaSelecionada())

    End Sub

    Private Sub btnExportarExcel_Click(sender As Object, e As System.EventArgs) Handles btnExportarExcel.Click

        ' Limpando o Response
        Response.Clear()
        ' Conteúdo do Response
        Response.AddHeader("content-disposition", "attachment; filename=relatorio.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.xls"
        Dim stringWrite As StringWriter = New System.IO.StringWriter
        Dim htmlWrite As HtmlTextWriter = New HtmlTextWriter(stringWrite)
        Dim dgDados As DataGrid = New DataGrid
        ' Definiões de cores
        dgDados.HeaderStyle.BackColor = System.Drawing.Color.LightGray
        dgDados.DataSource = Session("dtParaExcel")
        dgDados.DataBind()
        
        dgDados.RenderControl(htmlWrite)
        'Exporta 
        Response.Write(stringWrite.ToString)
        'encerra
        Response.End()

    End Sub

    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '   Função para coletar os status de aprovação do colaborador
    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Private Function getStatusAprovacao(ByVal colCodigo As String, ByVal proCodigo As String, ByVal dataInicio As DateTime) As ArrayList

        Dim array As New ArrayList
        Dim linha As DataRow
        Dim aprovacaoGP As String = ""
        Dim aprovacaoGC As String = ""
        Dim aprovacaoDir As String = ""

        dataFim = dataInicio.AddMonths(1).AddDays(-1)

        SQL = "SELECT possueGP, possueGC, possueDir FROM v_projetos WHERE proCodigo = " & proCodigo

        If selectSQL(SQL) Then
            dr.Read()
            If dr.HasRows Then
                aprovacaoGP = dr("possueGP")
                aprovacaoGC = dr("possueGC")
                aprovacaoDir = dr("possueDir")
            End If
        End If

        SQL = "SELECT apoAprovacaoGP, apoAprovacaoGC, apoAprovacaoDir FROM v_apontamento WHERE colCodigo = " & colCodigo & "" & _
                    "AND proCodigo = " & proCodigo & " AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

        ' Verifica se em todas as colunas de aprovação esta como A de aprovado, R de reprovado, ou vazia que 
        ' significa que ainda não houve aprovação/reprovação
        If selectSQL(SQL) Then

            Dim dtStatusAprovacao As New DataTable("tblStatusAprovacao")

            dtStatusAprovacao.Columns.Add("aprovacaoGP", GetType(String))
            dtStatusAprovacao.Columns.Add("aprovacaoGC", GetType(String))
            dtStatusAprovacao.Columns.Add("aprovacaoDir", GetType(String))

            Dim numLinhas = 0
            While dr.Read
                dtStatusAprovacao.Rows.Add(New Object() {dr("apoAprovacaoGP"), dr("apoAprovacaoGC"), dr("apoAprovacaoDir")})
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
            lblMensagem.Text = SQL & "<br />" & sqlErro
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

    Public Sub dataSetParaExcel(ByVal ds As DataSet, ByVal sNome As String)
        'Limpar o conteudo
        Response.Clear()
        'Seta o ContentType para xls
        Response.ContentType = "application/vnd.ms-excel"
        'Seta o tipo e o nome do arquivo
        Response.AddHeader("Content-Disposition", ("attachment;filename=" + sNome))
        'Abaixo codifica os caracteres para o alfabeto latino
        Response.ContentEncoding = System.Text.Encoding.GetEncoding("Windows-1252")
        Response.Charset = "ISO-8859-1"
        'Desabilita o ViewState
        EnableViewState = False
        ' Cria o StringWriter
        Dim sw As StringWriter = New StringWriter
        Dim htw As HtmlTextWriter = New HtmlTextWriter(sw)
        'Cria um GridView
        Dim gv As GridView = New GridView
        gv.DataSource = ds.Tables(0)
        gv.DataBind()
        'Renderiza
        gv.RenderControl(htw)
        Response.Write(sw.ToString)
        Response.End()
    End Sub


    'Somente usei para atender a mudança na tabela tblFechamento, pois havia adicionado uns campos adicionais
    Private Sub atualiza_tblFechamento()
        'Dim arrayProCodigo As ArrayList = New ArrayList
        'Dim arrayGP As ArrayList = New ArrayList
        'Dim arrayGC As ArrayList = New ArrayList
        'Dim arrayDir As ArrayList = New ArrayList

        'SQL = "SELECT proCodigo FROM tblFechamento GROUP BY proCodigo ORDER BY proCodigo"

        'selectSQL(SQL)
        'While dr.Read()
        '    arrayProCodigo.Add(dr("proCodigo"))
        'End While

        'For i = 0 To (arrayProCodigo.Count - 1)

        '    SQL = "SELECT codGP, codGC, codDir FROM tblProjetos WHERE proCodigo = " & arrayProCodigo(i)

        '    selectSQL(SQL)
        '    dr.Read()
        '    If dr("codGP") IsNot DBNull.Value Then
        '        arrayGP.Add(dr("codGP"))
        '    Else
        '        arrayGP.Add("NULL")
        '    End If
        '    If dr("codGC") IsNot DBNull.Value Then
        '        arrayGC.Add(dr("codGC"))
        '    Else
        '        arrayGC.Add("NULL")
        '    End If
        '    If dr("codDir") IsNot DBNull.Value Then
        '        arrayDir.Add(dr("codDir"))
        '    Else
        '        arrayDir.Add("NULL")
        '    End If

        'Next i

        'For i = 0 To (arrayProCodigo.Count - 1)

        '    SQL = "UPDATE tblFechamento SET " & _
        '          "fecAprovadorGP = " & arrayGP(i) & ", " & _
        '          "fecAprovadorGC = " & arrayGC(i) & ", " & _
        '          "fecAprovadorDir = " & arrayDir(i) & " " & _
        '          "WHERE proCodigo = " & arrayProCodigo(i)

        '    If Not comandoSQL(SQL) Then
        '        Dim mensagem = sqlErro
        '    End If

        'Next i

        '======================
        ' Segunda Modificação
        '======================

        'SQL = "SELECT * FROM tblFechamento"

        'selectSQL(SQL)

        'Dim arrayProCodigo As New ArrayList
        'Dim arrayColCodigo As New ArrayList
        'Dim arrayFecDataInicio As New ArrayList
        'Dim arrayFecDataFim As New ArrayList

        'While dr.Read
        '    arrayProCodigo.Add(dr("proCodigo"))
        '    arrayColCodigo.Add(dr("colCodigo"))
        '    arrayFecDataInicio.Add(dr("fecDataInicio"))
        '    arrayFecDataFim.Add(dr("fecDataFim"))
        'End While

        'If IsNumeric("tete") Then

        'End If

        'For i = 0 To arrayColCodigo.Count - 1

        '    SQL = "SELECT colTipoEmpresa, colRAzaoSocial, colEmpresaCompartilhada FROM v_colaboradores WHERE colCodigo = " & arrayColCodigo(i)

        '    selectSQL(SQL)
        '    dr.Read()

        '    Dim tipoEmpresa = "NULL"
        '    Dim razaoSocial = "NULL"
        '    Dim empresaCompartilhada = "NULL"

        '    If Not IsDBNull(dr("colTipoEmpresa")) And dr("colTipoEmpresa").ToString.Trim <> "" Then
        '        tipoEmpresa = "'" & dr("colTipoEmpresa") & "'"
        '    End If

        '    If Not IsDBNull(dr("colRazaoSocial")) And dr("colRazaoSocial").ToString.Trim <> "" Then
        '        razaoSocial = "'" & dr("colRazaoSocial") & "'"
        '    End If

        '    If Not IsDBNull(dr("colEmpresaCompartilhada")) And dr("colEmpresaCompartilhada").ToString.Trim <> "" Then
        '        empresaCompartilhada = "'" & dr("colEmpresaCompartilhada") & "'"
        '    End If

        '    SQL = "UPDATE tblFechamento SET fecTipoEmpresa = " & tipoEmpresa & ", " & _
        '          "fecRazaoSocial = " & razaoSocial & ", fecEmpresaCompartilhada = " & empresaCompartilhada & " " & _
        '          "WHERE proCodigo = " & arrayProCodigo(i) & " AND colCodigo = " & arrayColCodigo(i) & " AND " & _
        '          "fecDataInicio = '" & arrayFecDataInicio(i) & "' AND fecDataFim = '" & arrayFecDataFim(i) & "'"

        '    If Not comandoSQL(SQL) Then
        '        Dim teste = ""
        '    End If

        'Next i

    End Sub

    Protected Sub btnAplicarFiltros_Click1(sender As Object, e As EventArgs)
        preencheTabela(getCompetenciaSelecionada())
    End Sub
End Class