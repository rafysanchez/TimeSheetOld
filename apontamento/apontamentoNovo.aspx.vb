Imports System.DateTime
Imports System.Drawing
Imports System.Web.Services

Partial Public Class ApontamentoNovo
    Inherits System.Web.UI.Page

    Dim SQL As String
    'Dim primeiroDiaApo As String = "01"
    Dim linhasComErros As New ArrayList
    Protected URL

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ' Prepara URL para ser usada na chamada de função janelaNova (javascript), janela que mostra o apontamento selecionado
        URL = Request.Url.GetLeftPart(UriPartial.Authority) & VirtualPathUtility.ToAbsolute("~/")
        URL += "apontamento/visualizar.aspx"

        If ValidarSessao(Session("colPerfilLogado")) Then Response.Redirect("..\Default.aspx")

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Buffer = True
        Response.AddHeader("cache-control", "private")
        Response.AddHeader("pragma", "no-cache")
        Response.ExpiresAbsolute = "#January 1, 1990 00:00:01#"
        Response.Expires = 0

        lblTitulo1.Style.Add("font-style", "italic")

        Dim lblCarregando As Control = UpdateProgress2

        lblCarregando = lblCarregando.FindControl("lblCarregando")
        CType(lblCarregando, HtmlGenericControl).InnerText = "Carregando..."

        If Not IsPostBack Then

            'Dim versaoNavegador = Request.Browser.Version.ToString
            'lblTeste.Text = versaoNavegador

            ' Inicia o DropDownList dos Periodos
            ddlMes.Items.Add(New ListItem("", 0))
            ddlMes.Items.Add(New ListItem("Janeiro", 1))
            ddlMes.Items.Add(New ListItem("Fevereiro", 2))
            ddlMes.Items.Add(New ListItem("Março", 3))
            ddlMes.Items.Add(New ListItem("Abril", 4))
            ddlMes.Items.Add(New ListItem("Maio", 5))
            ddlMes.Items.Add(New ListItem("Junho", 6))
            ddlMes.Items.Add(New ListItem("Julho", 7))
            ddlMes.Items.Add(New ListItem("Agosto", 8))
            ddlMes.Items.Add(New ListItem("Setembro", 9))
            ddlMes.Items.Add(New ListItem("Outubro", 10))
            ddlMes.Items.Add(New ListItem("Novembro", 11))
            ddlMes.Items.Add(New ListItem("Dezembro", 12))

            divTabelaApontamento.Style.Add("display", "none")

            ' Preenche o comboBox Projetos de acordo com o código do usuário logado
            preencheComboBoxProjetos(Session("colCodigoLogado"))

            ' Preenche o camboBox Ano com ano inicio do sistema 2011 até o ano corrente
            Dim anoInicio = 2011
            While anoInicio <= Date.Now.Year
                ddlAno.Items.Add(anoInicio)
                anoInicio = anoInicio + 1
            End While
            ' Deixa selecionado como padrão o ano corrente
            ddlAno.SelectedValue = Date.Now.Year

        End If

    End Sub

    '==============================================================================================================
    '   Quando pressionado 'Apontar Hora' chama rotina para exibir a tabela de apontamento
    '==============================================================================================================
    Private Sub btnApontarHora_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnApontarHora.Click

        'Recupera o que o usuário escolheu no projeto e no periodo
        Dim mesNum As String = ddlMes.SelectedValue
        Dim anoNum As String = ddlAno.SelectedValue

        Dim dataInicio As DateTime = "01" & "/" & mesNum & "/" & anoNum

        lblTitulo1.Text = ""
        limpar(True)

        If ddlProjetos.SelectedIndex > 0 And ddlMes.SelectedIndex > 0 Then
            lblTitulo1.Style.Add("color", "Gray")
            lblTitulo1.Text = "Apontamento do Projeto : " & ddlProjetos.SelectedItem.Text
            preencheTabelaApontamento(dataInicio, anoNum, ddlProjetos.SelectedValue, Session("colCodigoLogado"))
        Else
            divTabelaApontamento.Style.Add("display", "none")
            lblTitulo1.Style.Add("color", "Red")
            lblTitulo1.Text = "Selecione um projeto e um mês para fazer o Apontamento!"
        End If

    End Sub

    Private Sub preencheTabelaApontamento(ByVal dataInicio As Date, ByVal anoNum As Integer,
                                          ByVal proCodigo As Integer, ByVal colCodigo As Integer)

        Dim feitoFechamento = False
        lblAlerta.Text = ""

        Session("p_colCodigo") = colCodigo
        Session("p_proCodigo") = proCodigo
        Session("p_dataInicio") = dataInicio

        btnSalvar.Enabled = True
        btnLimpar.Enabled = True

        qtdDiasMes.Value = Date.DaysInMonth(anoNum, dataInicio.Month)

        ' Guardando valor de variavel em imput do tipo hidden 
        varDataInicio.Value = dataInicio
        varProCodigo.Value = proCodigo

        Dim controle As Control = Me.form1
        Dim afpDiaEmDiante As Integer = 1
        divTabelaApontamento.Style.Add("display", "block")

        ' Preenche os Labels da tabela de apontamento, passando a primeira data somente
        preencheLabelDias(dataInicio)

        limpar(0) ' 0 - Limpa tudo

        ' Agora preencho todos os campos com os apontamentos salvos
        preencheCamposSalvos(colCodigo, proCodigo, dataInicio)

        habilitaDesabilitaDiasApontamento(colCodigo, proCodigo, dataInicio)

        lblTituloCalendario.Text = "Mês de Apontamento: " & Format(dataInicio, "MMMM").ToUpper & " de " & dataInicio.Year

    End Sub

    ' Preenche os Labels dias da tabela de apontamento
    Private Sub preencheLabelDias(ByVal dataInicio As DateTime)

        Dim dataFim As DateTime = dataInicio.AddMonths(1).AddDays(-1)
        Dim varData As DateTime = dataInicio
        Dim controle As Control = Me.form1
        Dim ListaFeriados As List(Of Feriados) = RetornarFeriados()

        ' Coleta em uma Array todos os dias do apontamento formatado no padrão "01/12 DOM"
        ' somente para exibição no calendario
        Dim count As Integer = 1
        Dim array(50) As String
        Dim arrayDatas(50) As String

        While varData <> dataFim.AddDays(1)
            arrayDatas(count) = varData
            If varData.Day <= 9 And varData.Month <= 9 Then
                array(count) = "0" & varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day <= 9 And varData.Month >= 10 Then
                array(count) = "0" & varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day >= 10 And varData.Month <= 9 Then
                array(count) = varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day >= 10 And varData.Month >= 10 Then
                array(count) = varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            varData = varData.AddDays(1)
            count = count + 1
        End While

        ' Preenche do lblDia1 até o lblDia28
        For i = 1 To 28
            controle = controle.FindControl("lblDia" & i)
            CType(controle, Label).Text = array(i)
        Next i

        linha29.Visible = True
        linha30.Visible = True
        linha31.Visible = True

        'Verifica total de dias para apontamento, para exibir ou não as últimas linhas
        Select Case count - 1
            Case 28
                linha29.Visible = False
                linha30.Visible = False
                linha31.Visible = False
            Case 29
                lblDia29.Text = array(29)
                linha30.Visible = False
                linha31.Visible = False
            Case 30
                lblDia29.Text = array(29)
                lblDia30.Text = array(30)
                linha31.Visible = False
            Case 31
                lblDia29.Text = array(29)
                lblDia30.Text = array(30)
                lblDia31.Text = array(31)
        End Select

        varData = dataInicio

        ' Loop para pintar as linhas que pertence a dias úteis e feriados
        For i = 1 To 31
            Dim VerFer As Feriados = (From dados In ListaFeriados Where (dados.Dia.Equals(varData.Day) AndAlso (dados.Mes.Equals(varData.Month))) Select dados).FirstOrDefault()

            If Not isDiaUtil(varData) Then
                controle = controle.FindControl("dia" & i)
                CType(controle, HtmlTableCell).BgColor = "#F05000"
            ElseIf Not IsNothing(VerFer) Then
                controle = controle.FindControl("dia" & i)
                CType(controle, HtmlTableCell).BgColor = "#0d076b"
                CType(controle, HtmlTableCell).Attributes.Add("title", VerFer.Descricao)
            Else
                controle = controle.FindControl("dia" & i)
                CType(controle, HtmlTableCell).BgColor = "#8A7DFF"
            End If
            varData = varData.AddDays(1)
        Next i

    End Sub

    ' Preenche a tabela de apontamento selecionada com os valores salvos
    Private Sub preencheCamposSalvos(ByVal colCodigo As Integer, ByVal proCodigo As Integer, ByVal dataInicio As DateTime)

        Dim dataFim As DateTime = dataInicio.AddMonths(1).AddDays(-1)

        SQL = "SELECT * FROM v_apontamento WHERE colCodigo = " & colCodigo & " and proCodigo = " & proCodigo
        SQL += " and apoData BETWEEN '" & dataInicio & "' and '" & dataFim & "' ORDER BY apoData"

        Dim horaNormal As String = ""
        Dim horaExtra As String = ""
        Dim horaTotal As String = ""
        Dim somaNormais As String = "00:00"
        Dim somaExtras As String = "00:00"
        Dim somaTotal As String = "00:00"
        Dim controle As Control = Me.form1

        Dim varData As DateTime = dataInicio

        If selectSQL(SQL) Then
            For i = 1 To DateDiff(DateInterval.Day, dataInicio, dataFim) + 1
                If dr.HasRows Then dr.Read() Else Exit For
                Try
                    For j = 1 To 31
                        Dim dataCompare As DateTime = DateTime.Parse(dr("apoData"))
                        If varData <> dataCompare Then
                            varData = varData.AddDays(1)
                            i = i + 1
                        Else
                            Exit For
                        End If
                    Next j

                    horaNormal = "00:00"
                    horaExtra = "00:00"
                    horaTotal = "00:00"

                    If dr("apoEntrada") IsNot DBNull.Value Then
                        controle = controle.FindControl("txtEntrada" & i)
                        CType(controle, HtmlInputText).Value = dr("apoEntrada")
                    End If

                    If dr("apoEntAlmoco") IsNot DBNull.Value Then
                        controle = controle.FindControl("txtEntAlmoco" & i)
                        CType(controle, HtmlInputText).Value = dr("apoEntAlmoco")
                    End If

                    If dr("apoSaiAlmoco") IsNot DBNull.Value Then
                        controle = controle.FindControl("txtSaiAlmoco" & i)
                        CType(controle, HtmlInputText).Value = dr("apoSaiAlmoco")
                    End If

                    If dr("apoSaida") IsNot DBNull.Value Then
                        controle = controle.FindControl("txtSaida" & i)
                        CType(controle, HtmlInputText).Value = dr("apoSaida")
                    End If

                    If dr("apoNormais") IsNot DBNull.Value Then
                        controle = controle.FindControl("lblNormal" & i)
                        horaNormal = dr("apoNormais")
                        CType(controle, HtmlInputText).Value = dr("apoNormais")
                    End If

                    If dr("apoExtras") IsNot DBNull.Value Then
                        controle = controle.FindControl("lblExtra" & i)
                        horaExtra = dr("apoExtras")
                        CType(controle, HtmlInputText).Value = dr("apoExtras")
                    End If

                    If dr("apoTotal") IsNot DBNull.Value Then
                        controle = controle.FindControl("lblTotal" & i)
                        horaTotal = dr("apoTotal")
                        CType(controle, HtmlInputText).Value = dr("apoTotal")
                    End If

                    If dr("apoDescricao") IsNot DBNull.Value Then
                        controle = controle.FindControl("txtAtividades" & i)
                        CType(controle, TextBox).Text = dr("apoDescricao").ToString
                    End If

                    If horaNormal = "00:00" And horaExtra = "00:00" And horaTotal = "00:00" Then
                        controle = controle.FindControl("lblNormal" & i)
                        CType(controle, HtmlInputText).Value = ""
                        controle = controle.FindControl("lblExtra" & i)
                        CType(controle, HtmlInputText).Value = ""
                        controle = controle.FindControl("lblTotal" & i)
                        CType(controle, HtmlInputText).Value = ""
                    End If

                    'Calculos dos totais de horas normais, extras e total das duas
                    somaNormais = somaHoras(somaNormais, horaNormal)
                    somaExtras = somaHoras(somaExtras, horaExtra)
                    somaTotal = somaHoras(somaTotal, horaTotal)

                Catch ex As Exception
#If DEBUG Then
                    lblTeste.Text = "Mensagem de erro interno: " & ex.Message
#End If
                End Try

                varData = varData.AddDays(1)

            Next i
        End If

        If somaNormais.Length = 4 Then
            somaNormais = "0" & somaNormais
        End If
        If somaExtras.Length = 4 Then
            somaExtras = "0" & somaExtras
        End If
        If somaTotal.Length = 4 Then
            somaTotal = "0" & somaTotal
        End If

        lblTotalNormais.Value = somaNormais
        lblTotalExtras.Value = somaExtras
        lblTotalTotal.Value = somaTotal

    End Sub
    Private Sub HabilitarDesabilitarCampo(ByVal Controle As HtmlInputText, ByVal Calen As Calendario)
        Try
            Controle.Disabled = IsNothing(Calen)
        Catch ex As Exception
            Throw ex
        End Try

    End Sub
    Private Sub HabilitarDesabilitarCampo(ByVal Controle As TextBox, ByVal Calen As Calendario)
        Try
            If IsNothing(Calen) Then
                Controle.Enabled = False
            Else
                Controle.Enabled = True
            End If

        Catch ex As Exception
            Throw ex
        End Try

    End Sub
    Function VerificarDataInicio(ByVal Data As DateTime, ByVal DataComp As DateTime) As Boolean
        Try
            Return (DataComp > Data)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    ' Verifica data de cada linha, se o usuário pode ou não apontar
    Private Sub habilitaDesabilitaDiasApontamento(ByVal colCodigo As Integer, ByVal proCodigo As Integer, ByVal dataInicio As DateTime)

        Dim dataFim As DateTime = dataInicio.AddMonths(1).AddDays(-1)
        Dim varData As DateTime = dataInicio
        Dim data As DateTime = dataInicio
        Dim simNaoPorque As ArrayList = New ArrayList
        Dim controle As Control = Me.form1
        Dim DatasCalendario As List(Of Calendario) = RetornarDadosCalendario(proCodigo)
        Dim DataInicioConsultor As DateTime

        'Verifica qual é a data de inicio do consultor
        SQL = "select convert(varchar(10),colDataInicio,103) as colDataInicio from tblColaboradores where colCodigo=" & colCodigo

        If selectSQL(SQL) Then
            If dr.HasRows Then
                dr.Read()
                DataInicioConsultor = DateTime.Parse(dr("colDataInicio"))
            End If
        End If

        lblMensagem.Style.Add("font-style", "italic")
        lblMensagem.Style.Add("color", "Red")

        simNaoPorque = permiteApontamento(varData, proCodigo, colCodigo)

        Dim UltimoDia As Integer = Date.DaysInMonth(dataInicio.Year, dataInicio.Month)

        For i = 1 To 31
            Dim Calen As Calendario = (From dados In DatasCalendario Where (dados.Dia.Equals(i) AndAlso (dados.Mes.Equals(dataInicio.Month))) Select dados).FirstOrDefault()
            lblMensagem.Text = ""
            Dim DataCorrente As DateTime

            If i > UltimoDia Then
                DataCorrente = DateTime.Parse(String.Concat(UltimoDia, "/", dataInicio.Month, "/", dataInicio.Year))
            Else
                DataCorrente = DateTime.Parse(String.Concat(i, "/", dataInicio.Month, "/", dataInicio.Year))
            End If

            Dim Desabilitar As Boolean = VerificarDataInicio(DataCorrente, DataInicioConsultor)

            ' Desabilita tudo antes e vou habilitando conforme certas condições
            controle = controle.FindControl("txtEntrada" & i)
            CType(controle, HtmlInputText).Disabled = True
            CType(controle, HtmlInputText).Style.Add("background-color", "#EBEBEB")

            controle = controle.FindControl("txtEntAlmoco" & i)
            CType(controle, HtmlInputText).Disabled = True
            CType(controle, HtmlInputText).Style.Add("background-color", "#EBEBEB")

            controle = controle.FindControl("txtSaiAlmoco" & i)
            CType(controle, HtmlInputText).Disabled = True
            CType(controle, HtmlInputText).Style.Add("background-color", "#EBEBEB")

            controle = controle.FindControl("txtSaida" & i)
            CType(controle, HtmlInputText).Disabled = True
            CType(controle, HtmlInputText).Style.Add("background-color", "#EBEBEB")

            controle = controle.FindControl("txtAtividades" & i)
            CType(controle, TextBox).Enabled = False
            CType(controle, TextBox).Style.Add("background-color", "#EBEBEB")

            Try
                Select Case simNaoPorque(i)
                    Case 0
                        ' Habilito campos quando satisfaz requisitos como, periodo afpAberto, e não aprovado, etc.
                        controle = controle.FindControl("txtEntrada" & i)
                        CType(controle, HtmlInputText).Disabled = Desabilitar
                        CType(controle, HtmlInputText).Style.Add("background-color", "White")
                        'verifica se é para habilitar o dia ou não de acordo com o calendario do projeto
                        If DatasCalendario.Count > 0 And Not Desabilitar Then HabilitarDesabilitarCampo(CType(controle, HtmlInputText), Calen)

                        controle = controle.FindControl("txtEntAlmoco" & i)
                        CType(controle, HtmlInputText).Disabled = Desabilitar
                        CType(controle, HtmlInputText).Style.Add("background-color", "White")
                        'verifica se é para habilitar o dia ou não de acordo com o calendario do projeto
                        If DatasCalendario.Count > 0 And Not Desabilitar Then HabilitarDesabilitarCampo(CType(controle, HtmlInputText), Calen)

                        controle = controle.FindControl("txtSaiAlmoco" & i)
                        CType(controle, HtmlInputText).Disabled = Desabilitar
                        CType(controle, HtmlInputText).Style.Add("background-color", "White")
                        'verifica se é para habilitar o dia ou não de acordo com o calendario do projeto
                        If DatasCalendario.Count > 0 And Not Desabilitar Then HabilitarDesabilitarCampo(CType(controle, HtmlInputText), Calen)

                        controle = controle.FindControl("txtSaida" & i)
                        CType(controle, HtmlInputText).Disabled = Desabilitar
                        CType(controle, HtmlInputText).Style.Add("background-color", "White")
                        'verifica se é para habilitar o dia ou não de acordo com o calendario do projeto
                        If DatasCalendario.Count > 0 And Not Desabilitar Then HabilitarDesabilitarCampo(CType(controle, HtmlInputText), Calen)

                        'Nesta caso a logica deve ser ao contraria
                        controle = controle.FindControl("txtAtividades" & i)
                        CType(controle, TextBox).Enabled = IIf(Desabilitar = False, True, False)
                        CType(controle, TextBox).Style.Add("background-color", "White")

                        'verifica se é para habilitar o dia ou não de acordo com o calendario do projeto
                        If DatasCalendario.Count > 0 And Not Desabilitar Then HabilitarDesabilitarCampo(CType(controle, TextBox), Calen)

                    Case 1
                        'Mensagem do código 1
                    Case 2
                        'Mensagem do código 2
                        lblMensagem.Text = "Projeto Inativo"
                    Case 3
                        'Mensagem do código 3
                        lblMensagem.Text = "Existem datas fora do range de abertura e fechamento do período"
                    Case 4
                        'Mensagem do código 4
                        lblMensagem.Text = "Não foi preenchida a tabela de períodos em ""Configurações/Abertura e fechamento de meses"""
                    Case 5
                        'Mensagem do código 5
                        lblMensagem.Text = "Período fechado para apontamento"
                    Case 6
                        'Mensagem do código 6
                        lblMensagem.Text = "Apontamento já foi aprovado"
                    Case 7
                        'Mensagem do código 7
                        lblMensagem.Text = "Data fora do período do Projeto"
                End Select
            Catch ex As Exception
            End Try

        Next i
        ''   0 = Permite apontamento; 
        ''   1 = Erro na conexão com banco de dados;
        ''   2 = Projeto Inativo;
        ''   3 = Data selecionada fora do range de abertura e fechamento do periodo;
        ''   4 = Não foi preenchida a tabela "AFPeriodo" Precisa configura-la em Configurações/Abertura e fechamento de meses
        ''   5 = Periodo fechado para apontamento
        ''   6 = Apontamento aprovado

    End Sub

    ''' <summary>
    '''  Função que retorna um array com códigos, e esta lista de códigos são:
    '''   0 = Permite apontamento; 
    '''   1 = Erro na conexão com banco de dados;
    '''   2 = Projeto Inativo;
    '''   3 = Data selecionada fora do range de abertura e fechamento do projeto;
    '''   4 = Não foi preenchida a tabela "AFPeriodo" Precisa configura-la em Configurações/Abertura e fechamento de meses
    '''   5 = Periodo fechado para apontamento
    '''   6 = Apontamento aprovado
    '''   7 = Data fora do periodo do Projeto
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="proCodigo"></param>
    ''' <param name="colCodigo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function permiteApontamento(ByVal data As DateTime, ByVal proCodigo As Integer, ByVal colCodigo As Integer) As ArrayList

        Dim permite As ArrayList = New ArrayList
        Dim afpAberto As String
        Dim afpDiaEmDiante As Integer
        Dim varData = data
        Dim dataFim = data.AddMonths(1).AddDays(-1)
        Dim proDataInicio As Date
        Dim proDataFim As Date

        ' Checo se projeto esta como Inativo para impedir o apontamento
        SQL = "SELECT proStatus, proDataInicio, proDataFim FROM v_projetos WHERE proCodigo = " & proCodigo
        If selectSQL(SQL) Then
            dr.Read()
            If dr("proStatus").ToString.ToLower = "inativo" Then
                permite.Add(2) 'Projeto Inativo
                Return permite
            Else
                proDataInicio = dr("proDataInicio")
                proDataFim = dr("proDataFim")
            End If
        Else
            permite.Add(1) 'Erro de conexão com banco de dados
            lblMensagem.Text = sqlErro
            Return permite
        End If

        ' Checo se o periodo esta afpAberto para data em questão
        SQL = "SELECT afpAberto, afpDiaEmDiante FROM tblAFPeriodo WHERE afpMes = " & ddlMes.SelectedValue
        If selectSQL(SQL) Then
            dr.Read()
            If dr.HasRows Then
                If dr("afpAberto") IsNot DBNull.Value Or dr("afpAberto").ToString.Trim <> "" Then
                    afpAberto = dr("afpAberto")
                Else
                    permite.Add(4) 'Precisa configurar em Configurações/Abertura e fechamento de meses
                    Return permite
                End If
                If dr("afpDiaEmDiante") IsNot DBNull.Value Or dr("afpDiaEmDiante").ToString.Trim <> "" Then
                    afpDiaEmDiante = dr("afpDiaEmDiante")
                Else
                    permite.Add(4) 'Precisa configurar em Configurações/Abertura e fechamento de meses
                    Return permite
                End If
            Else
                permite.Add(4) 'Precisa configurar em Configurações/Abertura e fechamento de meses
                Return permite
            End If
        Else
            permite.Add(1) 'Erro de conexão com banco de dados
            lblMensagem.Text = sqlErro
            Return permite
        End If

        While varData <= dataFim

            Dim permitido = True

            If afpAberto = "A" Then
                ' Data passada como parametro é menor que data configurada, se sim, então aquele dia esta fechado para apontamento
                If varData.Day < afpDiaEmDiante Then
                    permite.Add(5) 'Periodo fechado para apontamento
                    permitido = False
                End If
                If permitido Then
                    ' Checo se a data de apontamento esta no range da data inicio e data termino do projeto
                    If varData < proDataInicio Or varData > proDataFim Then
                        permite.Add(7) 'Periodo fechado para apontamento
                        permitido = False
                    End If
                    SQL = "SELECT apoAprovacaoGP, apoAprovacaoGC, apoAprovacaoDir FROM v_apontamento " &
                          "WHERE apoData = '" & varData & "' AND proCodigo = " & proCodigo & " AND colCodigo = " & colCodigo
                    If Not selectSQL(SQL) Then
                        permite.Add(1) 'Erro de conexão com banco de dados
                        lblMensagem.Text = sqlErro
                        Return permite
                    End If
                    If dr.HasRows Then dr.Read()
                    If permitido Then
                        ' Verifica se algum dos gestores aprovou
                        If dr.HasRows Then
                            permitido = True

                            If dr("apoAprovacaoGP") IsNot DBNull.Value Then
                                If dr("apoAprovacaoGP") = "A" Then
                                    permite.Add(6) 'Apontamento aprovado
                                    permitido = False
                                End If
                            End If

                            If dr("apoAprovacaoDir") IsNot DBNull.Value Then
                                If dr("apoAprovacaoDir") = "A" Then
                                    permite.Add(6) 'Apontamento aprovado
                                    permitido = False
                                End If
                            End If

                            If dr("apoAprovacaoGC") IsNot DBNull.Value Then
                                If dr("apoAprovacaoGC") = "A" Then
                                    permite.Add(6) 'Apontamento aprovado
                                    permitido = False
                                End If
                            End If
                        Else
                            permitido = True
                        End If
                    End If
                End If
            Else
                permite.Add(5) 'Periodo fechado para apontamento
                permitido = False
            End If

            varData = varData.AddDays(1)

            If permitido Then
                permite.Add(0)
            End If

        End While

        Return permite

    End Function

    '==================================================================================================
    '   Preenche a tabela de Apontamento necessitando em session os valores Data Inicio e Data Fim
    '==================================================================================================
    Private Sub preencherTabelaApontamento(ByVal primDiaApo As Integer, ByVal mesNum As Integer,
                                           ByVal anoNum As Integer, ByVal permitirApontar As Boolean)

        Dim lblCarregando As Control = UpdateProgress2

        lblCarregando = lblCarregando.FindControl("lblCarregando")
        CType(lblCarregando, HtmlGenericControl).InnerText = "Carregando..."

        Dim dataInicio As DateTime
        Dim dataFim As DateTime
        Dim varData As DateTime
        Dim colCodigo As Integer = Session("colCodigoLogado")
        Dim proCodigo As Integer = varProCodigo.Value
        Dim feitoFechamento = False
        Dim simNaoPorque As ArrayList = New ArrayList()

        dataInicio = DateTime.Parse(primDiaApo & "/" & mesNum & "/" & anoNum)
        dataFim = dataInicio.AddMonths(1).AddDays(-1)

        qtdDiasMes.Value = Date.DaysInMonth(anoNum, mesNum)

        ' Guardando valor de variavel em imput do tipo hidden 
        varDataInicio.Value = dataInicio

        Dim controle As Control = Me.form1
        Dim afpDiaEmDiante As Integer = 1
        divTabelaApontamento.Style.Add("display", "block")
        'divMensagemBloqueio.Style.Add("display", "none")
        'divMensagemBloqueio2.Style.Add("display", "none")

        limpar(True) ' True = Limpa tudo, para que em seguida seja preenchido com novos dados

        varData = dataInicio
        For i = 1 To 31

            ' Desabilita tudo antes e vou habilitando conforme certas condições
            controle = controle.FindControl("txtEntrada" & i)
            CType(controle, HtmlInputText).Disabled = True
            CType(controle, HtmlInputText).Style.Add("background-color", "White")

            controle = controle.FindControl("txtEntAlmoco" & i)
            CType(controle, HtmlInputText).Disabled = True
            CType(controle, HtmlInputText).Style.Add("background-color", "White")

            controle = controle.FindControl("txtSaiAlmoco" & i)
            CType(controle, HtmlInputText).Disabled = True
            CType(controle, HtmlInputText).Style.Add("background-color", "White")

            controle = controle.FindControl("txtSaida" & i)
            CType(controle, HtmlInputText).Disabled = True
            CType(controle, HtmlInputText).Style.Add("background-color", "White")

            controle = controle.FindControl("txtAtividades" & i)
            CType(controle, TextBox).Enabled = False
            CType(controle, TextBox).Style.Add("background-color", "White")

            If varData.AddDays(i - 1) > "26/11/2011" Then

                ' Verifica se dia esta afpAberto e não aprovado
                SQL = "SELECT afpAberto, afpDiaEmDiante FROM tblAFPeriodo WHERE afpMes = " & mesNum

                If Not selectSQL(SQL) Then
                    lblMensagem.Text = sqlErro
                    Return
                End If

                dr.Read()

                simNaoPorque = permiteApontamento(varData, proCodigo, colCodigo)

                ' Verifico se na primeira posição do array é True, pois é o valor que a função "permiteApontamento" retorna
                If simNaoPorque(0) Then
                    ' Habilito campos quando satisfaz requisitos como, periodo afpAberto, e não aprovado
                    controle = controle.FindControl("txtEntrada" & i)
                    CType(controle, HtmlInputText).Disabled = False
                    CType(controle, HtmlInputText).Style.Add("background-color", "White")

                    controle = controle.FindControl("txtEntAlmoco" & i)
                    CType(controle, HtmlInputText).Disabled = False
                    CType(controle, HtmlInputText).Style.Add("background-color", "White")

                    controle = controle.FindControl("txtSaiAlmoco" & i)
                    CType(controle, HtmlInputText).Disabled = False
                    CType(controle, HtmlInputText).Style.Add("background-color", "White")

                    controle = controle.FindControl("txtSaida" & i)
                    CType(controle, HtmlInputText).Disabled = False
                    CType(controle, HtmlInputText).Style.Add("background-color", "White")

                    controle = controle.FindControl("txtAtividades" & i)
                    CType(controle, TextBox).Enabled = True
                    CType(controle, TextBox).Style.Add("background-color", "White")

                Else

                    lblAviso.Text = simNaoPorque(1).ToString

                End If

            End If

        Next i

        If Not permitirApontar Then
            'divMensagemBloqueio.Style.Add("display", "block")
        Else
            SQL = "SELECT afpAberto, afpDiaEmDiante FROM tblAFPeriodo WHERE afpMes = " & mesNum
            If selectSQL(SQL) Then
                dr.Read()
                If dr("afpAberto") = "F" Then
                    ' Se o mês esta fechado ficará desabilitado o apontamento
                    'divMensagemBloqueio2.Style.Add("display", "block")
                    permitirApontar = False
                Else
                    ' Se o mês estiver em afpAberto, então coleto o dia que servirá para definir quais dias serão
                    ' afpAbertos para apontamento.
                    afpDiaEmDiante = dr("afpDiaEmDiante")
                    ' Session usada para ser usada com a função Limpar Formulario
                    varafpDiaEmDiante.Value = afpDiaEmDiante

                    ' Verifico se já foi realizado o fechamento do financeiro
                    SQL = "SELECT colCodigo FROM v_fechamento " &
                          "WHERE colCodigo = " & Session("colCodigoLogado") & " AND " &
                          "proCodigo = " & ddlProjetos.SelectedValue & " AND " &
                          "fecDataInicio = '" & dataInicio & "' AND " &
                          "fecDataFim = '" & dataFim & "'"
                    selectSQL(SQL)
                    If dr.HasRows Then
                        feitoFechamento = True
                    End If

                End If
            Else
                lblTitulo1.Text = sqlErro
            End If
        End If

        ' Se falso bloqueia o apontamento
        If Not permitirApontar Then

            'btnSalvar.Enabled = False
            'btnLimpar.Enabled = False

            ' Loop para setar as caixas de textos como desabilitadas para inserção de dados
            For i = 1 To 31

                controle = controle.FindControl("txtEntrada" & i)
                CType(controle, HtmlInputText).Disabled = True
                CType(controle, HtmlInputText).Style.Add("background-color", "White")

                controle = controle.FindControl("txtEntAlmoco" & i)
                CType(controle, HtmlInputText).Disabled = True
                CType(controle, HtmlInputText).Style.Add("background-color", "White")

                controle = controle.FindControl("txtSaiAlmoco" & i)
                CType(controle, HtmlInputText).Disabled = True
                CType(controle, HtmlInputText).Style.Add("background-color", "White")

                controle = controle.FindControl("txtSaida" & i)
                CType(controle, HtmlInputText).Disabled = True
                CType(controle, HtmlInputText).Style.Add("background-color", "White")

                controle = controle.FindControl("txtAtividades" & i)
                CType(controle, TextBox).Enabled = False
                CType(controle, TextBox).Style.Add("background-color", "White")

            Next i

        Else

            btnSalvar.Enabled = True
            btnLimpar.Enabled = True

            ' Se verdadeiro todos os campos devem ficar desativados, pois para este projeto durante este periodo já foi
            ' feito o fechamento
            If feitoFechamento Then
                afpDiaEmDiante = 32 ' 32 faz o fechamento de todos os dias do mês
                lblTitulo1.Style.Add("color", "Red")
                lblTitulo1.Text = "O projeto e mês selecionado já foi feito o fechamento, assim não podendo realizar apontamentos."
            End If

            ' Loop para setar as caixas de textos como habilitadas para inserção de dados
            For i = 1 To 31

                ' No campo afpDiaEmDiante na tabela tblAFPeriodo, foi configurado um número
                ' todos os dias antes desta data ficarão bloqueados para apontamento

                controle = controle.FindControl("txtEntrada" & i)
                If i < afpDiaEmDiante Then
                    CType(controle, HtmlInputText).Disabled = True
                    CType(controle, HtmlInputText).Style.Add("background-color", "#EBEBEB")
                Else
                    CType(controle, HtmlInputText).Disabled = False
                    CType(controle, HtmlInputText).Style.Add("background-color", "White")
                End If

                controle = controle.FindControl("txtEntAlmoco" & i)
                If i < afpDiaEmDiante Then
                    CType(controle, HtmlInputText).Disabled = True
                    CType(controle, HtmlInputText).Style.Add("background-color", "#EBEBEB")
                Else
                    CType(controle, HtmlInputText).Disabled = False
                    CType(controle, HtmlInputText).Style.Add("background-color", "White")
                End If

                controle = controle.FindControl("txtSaiAlmoco" & i)
                If i < afpDiaEmDiante Then
                    CType(controle, HtmlInputText).Disabled = True
                    CType(controle, HtmlInputText).Style.Add("background-color", "#EBEBEB")
                Else
                    CType(controle, HtmlInputText).Disabled = False
                    CType(controle, HtmlInputText).Style.Add("background-color", "White")
                End If

                controle = controle.FindControl("txtSaida" & i)
                If i < afpDiaEmDiante Then
                    CType(controle, HtmlInputText).Disabled = True
                    CType(controle, HtmlInputText).Style.Add("background-color", "#EBEBEB")
                Else
                    CType(controle, HtmlInputText).Disabled = False
                    CType(controle, HtmlInputText).Style.Add("background-color", "White")
                End If

                controle = controle.FindControl("txtAtividades" & i)
                If i < afpDiaEmDiante Then
                    CType(controle, TextBox).Enabled = False
                    CType(controle, TextBox).Style.Add("background-color", "#EBEBEB")
                Else
                    CType(controle, TextBox).Enabled = True
                    CType(controle, TextBox).Style.Add("background-color", "White")
                End If

            Next i

            'divMensagemBloqueio.Style.Add("display", "none")

        End If

        lblTituloCalendario.Text = "Mês de Apontamento: " & Format(dataFim, "MMMM").ToUpper & " de " & dataFim.Year

        ' Coleta em uma Array todos os dias do apontamento formatado no padrão "01/12 DOM"
        ' somente para exibição no calendario
        Dim count As Integer = 1
        Dim array(50) As String
        Dim arrayDatas(50) As String
        varData = dataInicio
        While varData <> dataFim.AddDays(1)
            arrayDatas(count) = varData
            If varData.Day <= 9 And varData.Month <= 9 Then
                array(count) = "0" & varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day <= 9 And varData.Month >= 10 Then
                array(count) = "0" & varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day >= 10 And varData.Month <= 9 Then
                array(count) = varData.Day & "/0" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            If varData.Day >= 10 And varData.Month >= 10 Then
                array(count) = varData.Day & "/" & varData.Month & " " & Format(varData, "ddd").ToUpper
            End If
            varData = varData.AddDays(1)
            count = count + 1
        End While

        lblDia1.Text = array(1)
        lblDia2.Text = array(2)
        lblDia3.Text = array(3)
        lblDia4.Text = array(4)
        lblDia5.Text = array(5)
        lblDia6.Text = array(6)
        lblDia7.Text = array(7)
        lblDia8.Text = array(8)
        lblDia9.Text = array(9)
        lblDia10.Text = array(10)
        lblDia11.Text = array(11)
        lblDia12.Text = array(12)
        lblDia13.Text = array(13)
        lblDia14.Text = array(14)
        lblDia15.Text = array(15)
        lblDia16.Text = array(16)
        lblDia17.Text = array(17)
        lblDia18.Text = array(18)
        lblDia19.Text = array(19)
        lblDia20.Text = array(20)
        lblDia21.Text = array(21)
        lblDia22.Text = array(22)
        lblDia23.Text = array(23)
        lblDia24.Text = array(24)
        lblDia25.Text = array(25)
        lblDia26.Text = array(26)
        lblDia27.Text = array(27)
        lblDia28.Text = array(28)
        linha29.Visible = True
        linha30.Visible = True
        linha31.Visible = True

        'Verifica total de dias para apontamento, para exibir ou não as últimas linhas
        Select Case count - 1
            Case 28
                linha29.Visible = False
                linha30.Visible = False
                linha31.Visible = False
            Case 29
                lblDia29.Text = array(29)
                linha30.Visible = False
                linha31.Visible = False
            Case 30
                lblDia29.Text = array(29)
                lblDia30.Text = array(30)
                linha31.Visible = False
            Case 31
                lblDia29.Text = array(29)
                lblDia30.Text = array(30)
                lblDia31.Text = array(31)
        End Select

        ' Restaura novamente a variavel varData para ser usada neste loop abaixo
        varData = dataInicio

        ' Loop para pintar as linhas que pertence a dias úteis e feriados
        For i = 1 To 31
            If Not isDiaUtil(varData) Then
                controle = controle.FindControl("dia" & i)
                CType(controle, HtmlTableCell).BgColor = "#F05000"
            Else
                controle = controle.FindControl("dia" & i)
                CType(controle, HtmlTableCell).BgColor = "#8A7DFF"
            End If
            varData = varData.AddDays(1)
        Next i

        SQL = "SELECT * FROM v_apontamento WHERE colCodigo = " & colCodigo & " and proCodigo = " & proCodigo
        SQL += " and apoData BETWEEN '" & dataInicio & "' and '" & dataFim & "' ORDER BY apoData"

        Dim horaNormal As String = ""
        Dim horaExtra As String = ""
        Dim horaTotal As String = ""
        Dim somaNormais As String = "00:00"
        Dim somaExtras As String = "00:00"
        Dim somaTotal As String = "00:00"

        ' Restaura novamente a variavel usada anteriormente com a data de inicio
        varData = dataInicio

        If selectSQL(SQL) Then
            For i = 1 To DateDiff(DateInterval.Day, dataInicio, dataFim) + 1
                dr.Read()
                Try
                    For j = 1 To 31
                        Dim dataCompare As DateTime = DateTime.Parse(dr("apoData"))
                        If varData <> dataCompare Then
                            varData = varData.AddDays(1)
                            i = i + 1
                        Else
                            Exit For
                        End If
                    Next j

                    horaNormal = "00:00"
                    horaExtra = "00:00"
                    horaTotal = "00:00"

                    If dr("apoEntrada") IsNot DBNull.Value Then
                        controle = controle.FindControl("txtEntrada" & i)
                        CType(controle, HtmlInputText).Value = dr("apoEntrada")
                    End If

                    If dr("apoEntAlmoco") IsNot DBNull.Value Then
                        controle = controle.FindControl("txtEntAlmoco" & i)
                        CType(controle, HtmlInputText).Value = dr("apoEntAlmoco")
                    End If

                    If dr("apoSaiAlmoco") IsNot DBNull.Value Then
                        controle = controle.FindControl("txtSaiAlmoco" & i)
                        CType(controle, HtmlInputText).Value = dr("apoSaiAlmoco")
                    End If

                    If dr("apoSaida") IsNot DBNull.Value Then
                        controle = controle.FindControl("txtSaida" & i)
                        CType(controle, HtmlInputText).Value = dr("apoSaida")
                    End If

                    If dr("apoNormais") IsNot DBNull.Value Then
                        controle = controle.FindControl("lblNormal" & i)
                        horaNormal = dr("apoNormais")
                        CType(controle, HtmlInputText).Value = dr("apoNormais")
                    End If

                    If dr("apoExtras") IsNot DBNull.Value Then
                        controle = controle.FindControl("lblExtra" & i)
                        horaExtra = dr("apoExtras")
                        CType(controle, HtmlInputText).Value = dr("apoExtras")
                    End If

                    If dr("apoTotal") IsNot DBNull.Value Then
                        controle = controle.FindControl("lblTotal" & i)
                        horaTotal = dr("apoTotal")
                        CType(controle, HtmlInputText).Value = dr("apoTotal")
                    End If

                    If dr("apoDescricao") IsNot DBNull.Value Then
                        controle = controle.FindControl("txtAtividades" & i)
                        CType(controle, TextBox).Text = dr("apoDescricao").ToString
                    End If

                    If horaNormal = "00:00" And horaExtra = "00:00" And horaTotal = "00:00" Then
                        controle = controle.FindControl("lblNormal" & i)
                        CType(controle, HtmlInputText).Value = ""
                        controle = controle.FindControl("lblExtra" & i)
                        CType(controle, HtmlInputText).Value = ""
                        controle = controle.FindControl("lblTotal" & i)
                        CType(controle, HtmlInputText).Value = ""
                    End If

                    'Calculos dos totais de horas normais, extras e total das duas
                    somaNormais = somaHoras(somaNormais, horaNormal)
                    somaExtras = somaHoras(somaExtras, horaExtra)
                    somaTotal = somaHoras(somaTotal, horaTotal)

                Catch ex As Exception
#If DEBUG Then
                    lblTeste.Text = "Mensagem de erro interno: " & ex.Message
#End If
                End Try

                varData = varData.AddDays(1)

            Next i
        End If

        If somaNormais.Length = 4 Then
            somaNormais = "0" & somaNormais
        End If
        If somaExtras.Length = 4 Then
            somaExtras = "0" & somaExtras
        End If
        If somaTotal.Length = 4 Then
            somaTotal = "0" & somaTotal
        End If

        lblTotalNormais.Value = somaNormais
        lblTotalExtras.Value = somaExtras
        lblTotalTotal.Value = somaTotal

    End Sub

    '========================================================================================================================
    '   Calcula todos os campos de entrada de horas que o usuário digitou
    '========================================================================================================================
    Private Function calculaTotais() As Boolean

        Dim horaEntrada As String
        Dim horaEntAlmoco As String
        Dim horaSaiAlmoco As String
        Dim horaSaida As String
        Dim diferenca1 As String
        Dim diferenca2 As String
        Dim resultado As String

        Dim horaNormal As String = "00:00"
        Dim horaExtra As String = "00:00"
        Dim horaTotal As String = "00:00"
        Dim somaNormais As String = "00:00"
        Dim somaExtras As String = "00:00"
        Dim somaTotal As String = "00:00"

        Dim ctrlEntrada As Control = Me.form1
        Dim ctrlEntAlmoco As Control = Me.form1
        Dim ctrlSaiAlmoco As Control = Me.form1
        Dim ctrlSaida As Control = Me.form1
        Dim ctrlAtividades As Control = Me.form1
        Dim controle As Control = Me.form1

        Dim deuErro = False

        For i = 1 To 31
            Try
                ctrlEntrada = ctrlEntrada.FindControl("txtEntrada" & i)
                horaEntrada = CType(ctrlEntrada, HtmlInputText).Value

                ctrlEntAlmoco = ctrlEntAlmoco.FindControl("txtEntAlmoco" & i)
                horaEntAlmoco = CType(ctrlEntAlmoco, HtmlInputText).Value

                ctrlSaiAlmoco = ctrlSaiAlmoco.FindControl("txtSaiAlmoco" & i)
                horaSaiAlmoco = CType(ctrlSaiAlmoco, HtmlInputText).Value

                ctrlSaida = ctrlSaida.FindControl("txtSaida" & i)
                horaSaida = CType(ctrlSaida, HtmlInputText).Value

                CType(ctrlEntrada, TextBox).BackColor = Color.White
                CType(ctrlEntAlmoco, TextBox).BackColor = Color.White
                CType(ctrlSaiAlmoco, TextBox).BackColor = Color.White
                CType(ctrlSaida, TextBox).BackColor = Color.White
                lblTitulo1.Text = ""

                ''''''''''''''''' 1° Parte - Validação ''''''''''''''''''''''

                ' Checo se tem algum campo com valores incorretos
                If horaEntrada.Trim <> "" Then
                    If Not IsDate(horaEntrada) Then
                        CType(ctrlEntrada, TextBox).BackColor = Color.Yellow
                        deuErro = True
                    End If
                End If
                If horaEntAlmoco.Trim <> "" Then
                    If Not IsDate(horaEntAlmoco) Then
                        CType(ctrlEntAlmoco, TextBox).BackColor = Color.Yellow
                        deuErro = True
                    End If
                End If
                If horaSaiAlmoco.Trim <> "" Then
                    If Not IsDate(horaSaiAlmoco) Then
                        CType(ctrlSaiAlmoco, TextBox).BackColor = Color.Yellow
                        deuErro = True
                    End If
                End If
                If horaSaida.Trim <> "" Then
                    If Not IsDate(horaSaida) Then
                        CType(ctrlSaida, TextBox).BackColor = Color.Yellow
                        deuErro = True
                    End If
                End If

                If deuErro Then
                    lblTitulo1.Style.Add("color", "red")
                    lblTitulo1.Text = "Existe campo com valores incorretos, não é possivel fazer calculos."
                    controle = controle.FindControl("lblNormal" & i)
                    CType(controle, HtmlInputText).Value = ""
                    controle = controle.FindControl("lblExtra" & i)
                    CType(controle, HtmlInputText).Value = ""
                    controle = controle.FindControl("lblTotal" & i)
                    CType(controle, HtmlInputText).Value = ""
                    Exit Try
                End If

                ' Se entrada ou saida estiverem vazios não se calcula
                If horaEntrada.Trim = "" Or horaSaida.Trim = "" Then
                    Exit Try
                End If

                ''''''''''''''''''''' 2° Parte - Calculo ''''''''''''''''''''''''''''''''

                ' Se os campos entrada e saida do almoço estiverem vazios, somente tira a diferença entre entrada e saida
                If horaEntAlmoco.Trim = "" Or horaSaiAlmoco.Trim = "" Then
                    If horaEntrada < horaSaida Then
                        diferenca1 = subtraiHoras(horaEntrada, horaSaida)
                    Else
                        If horaSaida = "00:00" Then
                            horaSaida = "23:59"
                        End If
                        diferenca1 = subtraiHoras(horaSaida, horaEntrada)
                        diferenca1 = FormatDateTime(DateTime.Parse(diferenca1).AddMinutes(1), DateFormat.ShortTime)
                    End If
                    resultado = diferenca1
                Else
                    If horaEntrada < horaEntAlmoco Then
                        diferenca1 = subtraiHoras(horaEntrada, horaEntAlmoco)
                    Else
                        If horaEntAlmoco = "00:00" Then
                            horaEntAlmoco = "23:59"
                        End If
                        diferenca1 = subtraiHoras(horaEntAlmoco, horaEntrada)
                        diferenca1 = FormatDateTime(DateTime.Parse(diferenca1).AddMinutes(1), DateFormat.ShortTime)
                    End If
                    If horaSaiAlmoco < horaSaida Then
                        diferenca2 = subtraiHoras(horaSaiAlmoco, horaSaida)
                    Else
                        If horaSaida = "00:00" Then
                            horaSaida = "23:59"
                        End If
                        diferenca2 = subtraiHoras(horaSaiAlmoco, horaSaida)
                        diferenca2 = FormatDateTime(DateTime.Parse(diferenca2).AddMinutes(1), DateFormat.ShortTime)
                    End If
                    resultado = somaHoras(diferenca1, diferenca2)
                End If

                If resultado <> "0:00" Then
                    If CDate(resultado) > CDate("08:00") Then
                        controle = controle.FindControl("lblNormal" & i)
                        CType(controle, HtmlInputText).Value = "" = "08:00"

                        controle = controle.FindControl("lblExtra" & i)
                        CType(controle, HtmlInputText).Value = "" = subtraiHoras(resultado, "08:00")

                        controle = controle.FindControl("lblTotal" & i)
                        CType(controle, HtmlInputText).Value = "" = subtraiHoras(resultado, "00:00")
                    Else
                        controle = controle.FindControl("lblNormal" & i)
                        CType(controle, HtmlInputText).Value = "" = subtraiHoras(resultado, "00:00")

                        controle = controle.FindControl("lblExtra" & i)
                        CType(controle, HtmlInputText).Value = "" = "00:00"

                        controle = controle.FindControl("lblTotal" & i)
                        CType(controle, HtmlInputText).Value = "" = subtraiHoras(resultado, "00:00")
                    End If
                Else
                    controle = controle.FindControl("lblNormal" & i)
                    CType(controle, HtmlInputText).Value = ""
                    controle = controle.FindControl("lblExtra" & i)
                    CType(controle, HtmlInputText).Value = ""
                    controle = controle.FindControl("lblTotal" & i)
                    CType(controle, HtmlInputText).Value = ""
                End If

            Catch ex As Exception
#If DEBUG Then
                lblTeste.Text = ex.Message
#End If
            End Try
        Next i

        ' Loop para calcular os horarios totais no rodapé
        For i = 1 To 31
            controle = controle.FindControl("lblNormal" & i)
            horaNormal = CType(controle, HtmlInputText).Value
            If horaNormal = "" Then
                horaNormal = "00:00"
            End If

            controle = controle.FindControl("lblExtra" & i)
            horaExtra = CType(controle, HtmlInputText).Value
            If horaExtra = "" Then
                horaExtra = "00:00"
            End If

            controle = controle.FindControl("lblTotal" & i)
            horaTotal = CType(controle, HtmlInputText).Value
            If horaTotal = "" Then
                horaTotal = "00:00"
            End If

            'Calculos dos totais de horas normais, extras e total
            somaNormais = somaHoras(somaNormais, horaNormal)
            somaExtras = somaHoras(somaExtras, horaExtra)
            somaTotal = somaHoras(somaTotal, horaTotal)

        Next i

        lblTotalNormais.Value = somaNormais
        lblTotalExtras.Value = somaExtras
        lblTotalTotal.Value = somaTotal

        If deuErro Then
            Return False
        Else
            Return True
        End If

    End Function

    '==============================================================================================================
    '   Preenche a lista de projetos que o consultor está associado
    '==============================================================================================================
    Private Sub preencheComboBoxProjetos(ByVal colCodigo As Integer)

        ' Preenche o ddlProjetos com os projetos que o consultor esta associado e os que estão ativos
        SQL = "EXEC sp_projetosConsultor " & colCodigo
        If selectSQL(SQL) Then
            While dr.Read()
                ' Checa se projeto esta com o Status de Ativo
                If UCase(dr("proStatus")) = "ATIVO" Then
                    ddlProjetos.Items.Add(New ListItem(dr("proNome"), dr("proCodigo")))
                Else
                    ' Não é mais para exibir projetos INATIVOS
                    'ddlProjetos.Items.Add(New ListItem(dr("proNome") & " __INATIVO", dr("proCodigo")))
                End If
            End While
        Else
            lblTitulo1.Text = "Falha ao conectar no banco de dados."
        End If

    End Sub

    Private Sub btnLimpar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLimpar.Click

        limpar(False)

        calculaTotais()

    End Sub

    Private Sub limpar(ByVal apagarTudo As Boolean)

        Dim controle As Control
        controle = Me.form1
        lblMensagem.Text = ""

        Dim i = 1

        If apagarTudo Then

            ' Limpa todos os TextBox no formulario
            For i = 1 To 31

                controle = controle.FindControl("txtEntrada" & i)
                CType(controle, HtmlInputText).Value = ""

                controle = controle.FindControl("txtEntAlmoco" & i)
                CType(controle, HtmlInputText).Value = ""

                controle = controle.FindControl("txtSaiAlmoco" & i)
                CType(controle, HtmlInputText).Value = ""

                controle = controle.FindControl("txtSaida" & i)
                CType(controle, HtmlInputText).Value = ""

                controle = controle.FindControl("lblNormal" & i)
                CType(controle, HtmlInputText).Value = ""

                controle = controle.FindControl("lblExtra" & i)
                CType(controle, HtmlInputText).Value = ""

                controle = controle.FindControl("lblTotal" & i)
                CType(controle, HtmlInputText).Value = ""

                controle = controle.FindControl("txtAtividades" & i)
                CType(controle, TextBox).Text = ""

            Next i

        Else

            ' Coleta o range de data que esta preenchido a tabela de apontamento
            Dim dataInicio As Date = Session("p_dataInicio")
            Dim dataFim As Date = dataInicio.AddMonths(1).AddDays(-1)

            ' Apaga somente as linhas que podem ser editadas
            While dataInicio <= dataFim

                Dim array As ArrayList = New ArrayList

                ' Coleta se na data corrente esta habilitado para apontamento, pois se estiver significa que pode apagar esta linha
                'array = permiteApontamento(dataInicio, varProCodigo.Value, txtColCodigo.Value)

                controle = controle.FindControl("txtEntrada" & i)

                ' Se verdadeiro, significa que o apontamento esta liberado, portando pode-se apagar esta linha
                If Not CType(controle, HtmlInputText).Disabled Then

                    controle = controle.FindControl("txtEntrada" & i)
                    CType(controle, HtmlInputText).Value = ""

                    controle = controle.FindControl("txtEntAlmoco" & i)
                    CType(controle, HtmlInputText).Value = ""

                    controle = controle.FindControl("txtSaiAlmoco" & i)
                    CType(controle, HtmlInputText).Value = ""

                    controle = controle.FindControl("txtSaida" & i)
                    CType(controle, HtmlInputText).Value = ""

                    controle = controle.FindControl("lblNormal" & i)
                    CType(controle, HtmlInputText).Value = ""

                    controle = controle.FindControl("lblExtra" & i)
                    CType(controle, HtmlInputText).Value = ""

                    controle = controle.FindControl("lblTotal" & i)
                    CType(controle, HtmlInputText).Value = ""

                    controle = controle.FindControl("txtAtividades" & i)
                    CType(controle, TextBox).Text = ""

                End If

                '' Se verdadeiro, significa que o apontamento esta liberado, portando pode-se apagar esta linha
                'If array(0) Then
                '    controle = controle.FindControl("txtEntrada" & i)
                '    CType(controle, HtmlInputText).Value = ""

                '    controle = controle.FindControl("txtEntAlmoco" & i)
                '    CType(controle, HtmlInputText).Value = ""

                '    controle = controle.FindControl("txtSaiAlmoco" & i)
                '    CType(controle, HtmlInputText).Value = ""

                '    controle = controle.FindControl("txtSaida" & i)
                '    CType(controle, HtmlInputText).Value = ""

                '    controle = controle.FindControl("lblNormal" & i)
                '    CType(controle, HtmlInputText).Value = ""

                '    controle = controle.FindControl("lblExtra" & i)
                '    CType(controle, HtmlInputText).Value = ""

                '    controle = controle.FindControl("lblTotal" & i)
                '    CType(controle, HtmlInputText).Value = ""

                '    controle = controle.FindControl("txtAtividades" & i)
                '    CType(controle, TextBox).Text = ""
                'End If

                i = i + 1
                dataInicio = dataInicio.AddDays(1)

            End While

        End If

    End Sub

    '=========================================================================================================
    '   Chama a subrotina para gravação no banco de dados
    '=========================================================================================================
    Private Sub btnSalvar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSalvar.Click
        lblMensagem.Text = ""
        Dim data As DateTime = varDataInicio.Value
        ddlMes.SelectedValue = data.Month
        salvarNoBanco(varProCodigo.Value, varDataInicio.Value)
    End Sub

    Private Sub salvarNoBanco(ByVal proCodigo As Integer, ByVal dataInicio As Date)

        ' Preparando variaveis que serão usadas nos SELECTs do Bando de dados
        Dim colCodigo As Integer = Session("colCodigoLogado")

        Dim varData As DateTime = dataInicio
        Dim dataFim As DateTime = varData.AddMonths(1).AddDays(-1)
        Dim dataUtil As String
        Dim campoComErro = False

        Dim entrada As Control = form1
        Dim entAlmoco As Control = Me.form1
        Dim saiAlmoco As Control = Me.form1
        Dim saida As Control = Me.form1
        Dim atividades As Control = Me.form1
        Dim horaNormal As Control = Me.form1
        Dim horaExtra As Control = Me.form1
        Dim horaTotal As Control = Me.form1

        Dim sEntrada As String = "NULL"
        Dim sEntAlmoco As String = "NULL"
        Dim sSaiAlmoco As String = "NULL"
        Dim sSaida As String = "NULL"
        Dim sHoraNormal As String = "NULL"
        Dim sHoraExtra As String = "NULL"
        Dim sHoraTotal As String = "NULL"
        Dim sAtividades As String = "NULL"
        Dim BancoHoras As String = "NULL"
        Dim Calcular As Boolean = False
        Dim ListaDatas As List(Of Calendario) = RetornarDadosCalendario(proCodigo)


        '#############################################
        'Verifica se o colaborador é horas fechadas ou clt caso seja efetua o cálculo de horas excedentes ou faltantes
        SQL = "SELECT UPPER(COLTIPOCONTRATO) AS COLTIPOCONTRATO,UPPER(COLVALORFECHADO) AS COLVALORFECHADO FROM TBLCOLABORADORES WITH(NOLOCK) WHERE COLCODIGO=" & Session("colCodigoLogado")

        If selectSQL(SQL) Then
            If dr.HasRows Then
                dr.Read()
                If dr("COLTIPOCONTRATO") = "CLT" Then
                    Calcular = True
                ElseIf dr("COLTIPOCONTRATO") = "PJ" And dr("COLVALORFECHADO") = "S" Then
                    Calcular = True
                End If
            End If
        End If

        Dim i = 1

        While varData <= dataFim

            sEntrada = "NULL"
            sEntAlmoco = "NULL"
            sSaiAlmoco = "NULL"
            sSaida = "NULL"
            sHoraNormal = "NULL"
            sHoraExtra = "NULL"
            sHoraTotal = "NULL"
            sAtividades = "NULL"

            ' Primeiro coleto todas as informações para insert ou update no banco
            entrada = entrada.FindControl("txtEntrada" & i)
            entAlmoco = entAlmoco.FindControl("txtEntAlmoco" & i)
            saiAlmoco = saiAlmoco.FindControl("txtSaiAlmoco" & i)
            saida = saida.FindControl("txtSaida" & i)

            horaNormal = horaNormal.FindControl("lblNormal" & i)
            horaExtra = horaExtra.FindControl("lblExtra" & i)
            horaTotal = horaTotal.FindControl("lblTotal" & i)

            atividades = atividades.FindControl("txtAtividades" & i)

            sHoraNormal = "'" & CType(horaNormal, HtmlInputText).Value & "'"
            sHoraExtra = "'" & CType(horaExtra, HtmlInputText).Value & "'"
            sHoraTotal = "'" & CType(horaTotal, HtmlInputText).Value & "'"

            ' Expressão regular para horas (HH:MM)
            Dim reg As New Regex("([0-1]\d|2[0-3]):([0-5]\d)")

            '''''''' Prepara variaveis para gravação no banco ''''''''''''
            If Not CType(entrada, HtmlInputText).Value.Trim = "" Then
                If reg.IsMatch(CType(entrada, HtmlInputText).Value) Then
                    sEntrada = "'" & CType(entrada, HtmlInputText).Value & "'"
                Else
                    campoComErro = True
                End If
            End If

            If Not CType(entAlmoco, HtmlInputText).Value.Trim = "" Then
                If reg.IsMatch(CType(entAlmoco, HtmlInputText).Value) Then
                    sEntAlmoco = "'" & CType(entAlmoco, HtmlInputText).Value & "'"
                Else
                    campoComErro = True
                End If
            End If

            If Not CType(saiAlmoco, HtmlInputText).Value.Trim = "" Then
                If reg.IsMatch(CType(saiAlmoco, HtmlInputText).Value) Then
                    sSaiAlmoco = "'" & CType(saiAlmoco, HtmlInputText).Value & "'"
                Else
                    campoComErro = True
                End If
            End If

            If Not CType(saida, HtmlInputText).Value.Trim = "" Then
                If reg.IsMatch(CType(saida, HtmlInputText).Value) Then
                    sSaida = "'" & CType(saida, HtmlInputText).Value & "'"
                Else
                    campoComErro = True
                End If
            End If

            If Not CType(atividades, TextBox).Text.Trim = "" Then
                sAtividades = CType(atividades, TextBox).Text.Trim
                sAtividades = sAtividades.Replace("'", "''")
                sAtividades = "'" & sAtividades & "'"
            End If

            If isDiaUtil(varData) Then
                dataUtil = "S"
            Else
                dataUtil = "N"
            End If

            If sEntrada = "NULL" And sEntAlmoco = "NULL" And sSaiAlmoco = "NULL" And sSaida = "NULL" Then
                sHoraNormal = "NULL"
                sHoraExtra = "NULL"
                sHoraTotal = "NULL"
            End If

            BancoHoras = "NULL"

            ' Se campo estiver habilitado, então faço um insert ou update, ou seja, linha desabilitada não faz nada
            If CType(entrada, HtmlInputText).Disabled = False Then

                'Efetua o calculo para banco de horas caso seja necessário
                If Calcular And sHoraTotal <> "NULL" Then
                    Dim Calen As Calendario = (From dados In ListaDatas Where (dados.Dia.Equals(varData.Day) AndAlso (dados.Mes.Equals(varData.Month))) Select dados).FirstOrDefault()
                    Dim TotalHoras As TimeSpan = TimeSpan.FromHours(somaHorasnew(sHoraTotal.Replace("'", "")))
                    Dim QuantidadeDia As TimeSpan

                    'verifica se existe calendario atribuido ao projeto
                    If Not IsNothing(Calen) Then
                        QuantidadeDia = TimeSpan.FromHours(somaHorasnew(Calen.Hora))
                    Else
                        QuantidadeDia = TimeSpan.FromHours(somaHorasnew(ConfigurationManager.AppSettings("HorasDia")))
                    End If

                    Dim TotalBanco As TimeSpan = TotalHoras.Subtract(QuantidadeDia)

                    If ((TotalHoras > QuantidadeDia Or TotalHoras < QuantidadeDia)) Then
                        If TotalBanco < TimeSpan.Parse("0") Then
                            BancoHoras = FormatarHoraApontamento(TotalBanco.TotalHours * -1)
                            BancoHoras = "'-" & BancoHoras & "'"
                        Else
                            BancoHoras = FormatarHoraApontamento(TotalBanco.TotalHours)
                            BancoHoras = "'" & BancoHoras & "'"
                        End If
                    Else
                        BancoHoras = "NULL"
                    End If
                End If

                SQL = "IF EXISTS (" &
                          "SELECT colCodigo FROM tblApontamento WHERE apoData = '" & varData & "' " &
                          "AND colCodigo = " & colCodigo & " AND proCodigo = " & proCodigo & ") " &
                      "BEGIN " &
                          "UPDATE tblApontamento SET " &
                          "apoEntrada = " & sEntrada & " " &
                          ",apoEntAlmoco = " & sEntAlmoco & " " &
                          ",apoSaiAlmoco = " & sSaiAlmoco & " " &
                          ",apoSaida = " & sSaida & " " &
                          ",apoNormais = " & sHoraNormal & " " &
                          ",apoExtras = " & sHoraExtra & " " &
                          ",apoTotal = " & sHoraTotal & " " &
                          ",apoDescricao = " & sAtividades & " " &
                          ",apoBanco = " & BancoHoras & " " &
                          "WHERE colCodigo = " & colCodigo & " " &
                          "AND proCodigo = " & proCodigo & " " &
                          "AND apoData = '" & varData & "'" &
                      "END " &
                      "ELSE BEGIN " &
                          "INSERT INTO tblApontamento (colCodigo, proCodigo, apoData, apoEntrada, apoEntAlmoco, apoSaiAlmoco, " &
                          "apoSaida ,apoNormais, apoExtras, apoTotal, apoDiaUtil, apoDescricao,apoBanco) " &
                          "VALUES (" &
                          " " & colCodigo & "" &
                          "," & proCodigo & "" &
                          ",'" & varData & "'" &
                          "," & sEntrada & "" &
                          "," & sEntAlmoco & "" &
                          "," & sSaiAlmoco & "" &
                          "," & sSaida & "" &
                          "," & sHoraNormal & "" &
                          "," & sHoraExtra & "" &
                          "," & sHoraTotal & "" &
                          ",'" & dataUtil & "'" &
                          "," & sAtividades &
                          "," & BancoHoras & "); " &
                      "END "

                If comandoSQL(SQL) Then
                    lblMensagem.Style.Add("color", "gray")
                    lblMensagem.Text = "Apontamento salvo com sucesso."
                Else
                    lblMensagem.Style.Add("color", "red")
                    lblMensagem.Text = sqlErro
                    Return
                End If

                ' Se algum campo estiver preenchido incorretamente, exibe a mensagem de que tal linha não foi gravada
                If campoComErro Then
                    lblMensagem.Style.Add("color", "red")
                    lblMensagem.Text = "Apontamento salvo, mas não foram salvo os campos preenchidos incorretamente"
                End If

            End If

            varData = varData.AddDays(1)
            i = i + 1

        End While

        '############################################

    End Sub

End Class