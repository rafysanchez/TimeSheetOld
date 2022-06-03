Imports System.ComponentModel
Imports System.Data
Imports System.Data.SqlClient

Module funcoesGerais

    Public sqlConn As New SqlConnection
    Dim sqlComando As SqlCommand
    Public dr As SqlDataReader
    Public ds As New DataSet
    Public da As New SqlDataAdapter
    Public sqlErro As String = ""
    Public exSQL As SqlException
    Public Function RetornarDadosCalendario(ByVal CodProjeto As Integer) As List(Of Calendario)
        Dim Retorno As New List(Of Calendario)
        Try
            'Cria a lista com o calendario
            Dim Sql As String = "select  cln.DsCalendario,cln.Dia,cln.Mes,cln.Ano,cln.Hora" & vbCrLf &
                                "from tblProjetos prj with(nolock)" & vbCrLf &
                                "inner join tblCalendario cln with(nolock)  on (prj.proNomeCalendario = cln.DsCalendario and prj.proAnoCalendario=cln.Ano)" & vbCrLf &
                                "where prj.proCodigo=" & CodProjeto


            If selectSQL(Sql) Then
                If dr.HasRows Then
                    While dr.Read
                        Dim Calen As New Calendario
                        Calen.Dia = Integer.Parse(dr("Dia"))
                        Calen.Mes = Integer.Parse(dr("Mes"))
                        Calen.Ano = Integer.Parse(dr("Ano"))
                        Calen.Hora = dr("Hora").ToString
                        Retorno.Add(Calen)
                    End While
                Else
                End If

            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return Retorno
    End Function
    Public Sub PreencherCalendSalvos(ByRef Cbo As DropDownList)
        Try
            Cbo.Items.Clear()

            'Cria a lista com os feriados
            Dim Sql As String = "select distinct dscalendario,ano from tblcalendario with(nolock)"

            If selectSQL(Sql) Then
                If dr.HasRows Then
                    Cbo.Items.Add(New ListItem("Selecione", ""))
                    While dr.Read
                        Cbo.Items.Add(New ListItem(String.Format("Calendário:{0} -  Ano:{1}", dr("dscalendario"), dr("ano")), String.Format("{0}|{1}", dr("dscalendario"), dr("ano"))))
                    End While
                Else
                    Cbo.Items.Add(New ListItem("Não existem calendário(s)", ""))
                End If

            End If

            Cbo.SelectedValue = ""
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    Function getConnectionString() As String
        Return My.Settings.strConnAddvisor
    End Function

    '==========================================================================================================
    '   Função para se conectar no banco de dados, se sucesso retorna True, senão retorma False e atualiza a 
    ' a variavel global sqlErro
    '==========================================================================================================
    Function conectaSQL() As Boolean

        sqlConn.Close()

        sqlConn.ConnectionString = getConnectionString()

        Try
            sqlConn.Open()
            Return True
        Catch ex As Exception
            sqlErro = ex.Message()
            Return False
        End Try

    End Function

    '==========================================================================================================
    '   Função para se gravar, inserir, deletar, etc, no banco de dados, como parametro uma string de 
    ' comando, ex: "insert into table...", se sucesso retorna True, senão retorma False e atualiza a 
    ' a variavel global sqlErro
    '==========================================================================================================
    Function comandoSQL(ByVal SQL As String) As Boolean

        If conectaSQL() Then

            sqlComando = New SqlCommand(SQL, sqlConn)

            Try
                sqlComando.ExecuteNonQuery()
                Return True
            Catch ex As SqlException
                sqlErro = ex.Message()
                exSQL = ex
                Return False
            End Try

        Else
            Return False
        End If

    End Function

    '=============================================================================================================
    '    Faz um select no banco de dados, se sucesso retorna True e atualiza o objeto global dr (SqlDataReader), 
    ' senão retorna False e atualiza a variavel global sqlErro
    '=============================================================================================================
    Function selectSQL(ByVal SQL As String) As Boolean

        If conectaSQL() Then

            ds.Clear()

            sqlComando = New SqlCommand(SQL, sqlConn)

            Try
                da = New SqlDataAdapter(sqlComando)
                da.Fill(ds)
                dr = sqlComando.ExecuteReader()
                Return True
            Catch ex As Exception
                sqlErro = ex.Message()
                Return False
            End Try

        End If

    End Function

    '==========================================================================================
    '   Retorna o número de registros encontrados em um select
    '==========================================================================================
    Function selectSQLQtdRegistros(ByVal SQL As String) As Integer

        Dim qtdRegistros = 0

        If conectaSQL() Then
            sqlComando = New SqlCommand(SQL, sqlConn)
            Try
                dr = sqlComando.ExecuteReader() 'dr é um objeto global, não preciso fazer um "return dr" para usa-lo
                While dr.Read
                    qtdRegistros += 1
                End While
                Return qtdRegistros
            Catch ex As Exception
                sqlErro = ex.Message()
                Return -1   ' Retornando -1 significa que deu erro.
            End Try
        Else
            Return -1
        End If

    End Function

    '==========================================================================================
    '   Retorna se há registros no select passado como parâmetro
    '       Retorno 0  = Não há linhas
    '       Retorno 1  = Há linhas
    '       Retorno -1 = Ocorreu algum erro
    '==========================================================================================
    Function existeDados(ByVal SQL As String) As Integer

        If conectaSQL() Then
            sqlComando = New SqlCommand(SQL, sqlConn)
            Try
                dr = sqlComando.ExecuteReader() 'dr é um objeto global, não preciso fazer um "return dr" para usa-lo
                If dr.HasRows() Then
                    Return 1                    ' Há linhas
                Else
                    Return 0                    ' Não há linhas
                End If
            Catch ex As Exception
                sqlErro = ex.Message()
                Return -1                       ' Retornando -1 significa que deu erro.
            End Try
        Else
            Return -1
        End If

    End Function

    '=============================================================================================================
    '    Valida usuário para utilização do sistema
    '    Retorno:
    '       0 = Login e Senha encontrados
    '       1 = Login e Senha não encontrados
    '       2 = Erro com conexão do banco de dados
    '=============================================================================================================
    Function autenticarUsuario(ByVal usuario As String, ByVal senha As String) As String

        Dim SQL As String

        SQL = "SELECT colNome, perCodigo, colSenha, colCodigo, colStatus, colPerfil, colMudarSenha FROM v_colaboradores " &
                "WHERE colLogin = '" & usuario & "' and colSenha = '" & senha & "'"

        If conectaSQL() Then

            sqlComando = New SqlCommand(SQL, sqlConn)

            Try
                dr = sqlComando.ExecuteReader()
                If dr.HasRows Then
                    Return "0"  'Login e Senha encontrados
                End If
                Return "1"      'Login e Senha não encontrados
            Catch ex As Exception
                sqlErro = ex.Message()
                Return "2"      'Erro com conexão do banco de dados
            End Try
        Else
            Return "2"          'Erro com conexão do banco de dados
        End If

    End Function

    '===================================================================================================
    ' Função para validar se CPF é valido 
    '===================================================================================================
    Function isCPF(ByVal CPF As String) As Boolean
        Dim i, a, n1, n2 As Integer

        CPF = CPF.Replace(".", "").Replace(",", "").Replace("/", "").Replace("-", "")
        CPF = CPF.Trim

        If CPF = "" OrElse
          CPF.Trim.Length <> 11 OrElse
          CPF = "11111111111" OrElse
          CPF = "22222222222" OrElse
          CPF = "33333333333" OrElse
          CPF = "44444444444" OrElse
          CPF = "55555555555" OrElse
          CPF = "66666666666" OrElse
          CPF = "77777777777" OrElse
          CPF = "88888888888" OrElse
          CPF = "99999999999" Then
            Return False
        End If

        For a = 0 To 1
            n1 = 0
            For i = 1 To 9 + a
                n1 = n1 + Val(Mid(CPF, i, 1)) * (11 + a - i)
            Next
            n2 = 11 - (n1 - (Int(n1 / 11) * 11))
            If n2 = 10 Or n2 = 11 Then n2 = 0
            If n2 <> Val(Mid(CPF, 10 + a, 1)) Then
                Return False
            End If
        Next

        Return True
    End Function

    '=============================================================================================
    ' Função para validar se CNPJ é valido 
    '=============================================================================================
    Function isCNPJ(ByVal CGC As String) As Boolean
        Dim cnpj As String
        Dim num(14) As Integer
        Dim soma, result1, result2 As Integer

        CGC = CGC.Replace(".", "").Replace(",", "").Replace("/", "").Replace("-", "")
        cnpj = CGC.Trim

        If cnpj.Length <> 14 Or
            cnpj = "00000000000000" Or
            cnpj = "11111111111111" Or
            cnpj = "22222222222222" Or
            cnpj = "33333333333333" Or
            cnpj = "44444444444444" Or
            cnpj = "55555555555555" Or
            cnpj = "66666666666666" Or
            cnpj = "77777777777777" Or
            cnpj = "88888888888888" Or
            cnpj = "99999999999999" Then
            Return False
        Else
            num(1) = CInt(Mid(cnpj, 1, 1))
            num(2) = CInt(Mid(cnpj, 2, 1))
            num(3) = CInt(Mid(cnpj, 3, 1))
            num(4) = CInt(Mid(cnpj, 4, 1))
            num(5) = CInt(Mid(cnpj, 5, 1))
            num(6) = CInt(Mid(cnpj, 6, 1))
            num(7) = CInt(Mid(cnpj, 7, 1))
            num(8) = CInt(Mid(cnpj, 8, 1))
            num(9) = CInt(Mid(cnpj, 9, 1))
            num(10) = CInt(Mid(cnpj, 10, 1))
            num(11) = CInt(Mid(cnpj, 11, 1))
            num(12) = CInt(Mid(cnpj, 12, 1))
            num(13) = CInt(Mid(cnpj, 13, 1))
            num(14) = CInt(Mid(cnpj, 14, 1))
            soma = num(1) * 5 + num(2) * 4 + num(3) * 3 + num(4) * 2 + num(5) * 9 + num(6) * 8 + num(7) * 7 + num(8) * 6 + num(9) * 5 + num(10) * 4 + num(11) * 3 + num(12) * 2
            soma = soma - (11 * (Int(soma / 11)))

            If soma = 0 Or soma = 1 Then
                result1 = 0
            Else
                result1 = 11 - soma
            End If

            If result1 = num(13) Then
                soma = num(1) * 6 + num(2) * 5 + num(3) * 4 + num(4) * 3 + num(5) * 2 + num(6) * 9 + num(7) * 8 + num(8) * 7 + num(9) * 6 + num(10) * 5 + num(11) * 4 + num(12) * 3 + num(13) * 2
                soma = soma - (11 * (Int(soma / 11)))

                If soma = 0 Or soma = 1 Then
                    result2 = 0
                Else
                    result2 = 11 - soma
                End If

                If result2 = num(14) Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End If

    End Function

    '=============================================================================================
    ' Função para formatat CPF ou CNPJ 
    '=============================================================================================
    Function formatarCPF_CNPJ(ByVal campo As String, ByVal formatado As Boolean)
        ' Retira formato
        Dim numeroLimpo As String = campo.Replace(".", "").Replace("-", "").Replace("/", "")
        ' Pega o tamanho da string menos os digitos verificadores
        Dim tamanho As Integer = (numeroLimpo.Length - 2)
        ' Verifica se o tamanho do código informado é válido
        If tamanho <> 9 And tamanho <> 12 Then
            Return False
        End If

        '   If formatado Then
        '       ' Seleciona a máscara para cpf ou cnpj
        '    Dim mascara As String = (tamanho = 9) ? '###.###.###-##' : '##.###.###/####-##'

        '   $indice = -1;
        '   for ($i=0; $i < strlen($mascara); $i++) {
        '        if ($mascara[$i]=='#') $mascara[$i] = $codigoLimpo[++$indice];
        '           End If
        '    //retorna o campo formatado
        '    $retorno = $mascara;

        'else
        '    //se não quer formatado, retorna o campo limpo
        '    $retorno = $codigoLimpo;
        'End If

        'return $retorno;

        Dim mascaraCNPJ As New MaskedTextProvider("00\.000\.000/0000-00")
        mascaraCNPJ.Set("12123123123412")
        Dim CNPJFormatado As String = mascaraCNPJ.ToString

        Return True

    End Function

    ''' <summary>
    ''' Verifica se o e-mail é válido
    ''' </summary>
    ''' <param name="emailEndereco">Endereço de e-mail a ser validado</param>
    Function isEmail(ByVal emailEndereco As String) As Boolean

        ' Pattern ou mascara de verificação
        Dim pattern As String = "^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"

        ' Verifica se o email corresponde a pattern/mascara
        Dim emailAddressMatch As Match = Regex.Match(emailEndereco, pattern)

        ' Caso corresponda
        If emailAddressMatch.Success Then
            Return True
        Else
            Return False
        End If

    End Function

    '======================================================================================================================
    ' Função para soma de horas, ex: somaHoras("08:00", "08:00"), retornará 16:00
    '======================================================================================================================
    Public Function somaHoras(ByVal hora1 As String, ByVal hora2 As String) As String

        Dim Hora As Long
        Dim Min As Long
        Dim sRet

        'Corrige nulo para hora
        If Len(hora2) <= 0 Or hora2 = " " Then hora2 = "00:00"
        If Len(hora1) <= 0 Or hora1 = " " Then hora1 = "00:00"

        Hora = CLng(Left(hora1, InStr(1, hora1, ":") - 1))
        Hora = Hora + CLng(Left(hora2, InStr(1, hora2, ":") - 1))

        Min = CLng(Right(hora1, 2))
        Min = Min + CLng(Right(hora2, 2))

        If Min >= 60 Then
            Hora = Hora + 1
            Min = Min - 60
        End If

        sRet = Format(Hora & ":" & Format(Min, "00"), "short time")

        somaHoras = sRet

    End Function
    Public Function somaHorasnew(ByVal hora1 As String) As Double

        'Corrige nulo para hora
        If String.IsNullOrWhiteSpace(hora1) Then hora1 = "00:00"

        Dim SomaHora As TimeSpan = TimeSpan.Parse(hora1.Split(":")(0).ToString())
        Dim SomaMinutos As TimeSpan = TimeSpan.FromMinutes(Double.Parse(hora1.Split(":")(1)))
        Dim Final As TimeSpan = SomaHora.Add(SomaMinutos)
        Return Final.TotalHours

    End Function
    Public Function FomatarDataHora(ByVal HoraDecimal As Double) As String
        Dim Valor = TimeSpan.FromHours(HoraDecimal).ToString("dd\:hh\:mm")
        If HoraDecimal >= 0 Then
            Return Format(Int16.Parse(Valor.Split(":")(0)) + Int16.Parse(Valor.Split(":")(1)) & ":" & Valor.Split(":")(2), "short time")
        Else
            Return "-" & Format(Int16.Parse(Valor.Split(":")(0)) + Int16.Parse(Valor.Split(":")(1)) & ":" & Valor.Split(":")(2), "short time")
        End If

    End Function
    Public Function FormatarHoraApontamento(ByVal HoraDecimal As Double) As String
        If HoraDecimal >= 0 Then
            Return TimeSpan.FromHours(HoraDecimal).ToString("dd\:mm")
        Else
            Return "-" & TimeSpan.FromHours(HoraDecimal).ToString("dd\:mm")
        End If
    End Function
    Public Function ValidarSessao(ByVal Sessao As Object) As Boolean
        Return IsNothing(Sessao)
    End Function
    Public Function ValidarPIS(ByVal strPISPASEP As String) As Boolean
        Dim strPeso As String
        Dim intTotalAs As Int16
        Dim intContAs As Int16
        Dim intResultado As Int16
        Dim intResto As Int16
        Dim intTotal As Int16 = 0
        strPeso = "3298765432"

        strPISPASEP = strPISPASEP.Replace("-", "").Replace(".", "").Replace("/", "")

        'Validando a entrada dos dados
        If strPISPASEP = "" Or Len(strPISPASEP) <> 11 Then
            strPISPASEP = False
            Exit Function
        End If

        For intCont = 1 To 10
            intResultado = Mid(strPISPASEP, intCont, 1) * Mid(strPeso, intCont, 1)
            intTotal = intTotal + intResultado
        Next

        'Resto da Divisao
        intResto = intTotal Mod 11

        If intResto <> 0 Then
            intResto = 11 - intResto
        End If

        If intResto = 10 Or intResto = 11 Then
            intResto = Mid(intResto, 2, 1)
        End If

        If CInt(intResto) <> CInt(Mid(strPISPASEP, 11, 1)) Then
            ValidarPIS = False
            Exit Function
        End If

        ValidarPIS = True

    End Function

    Public Function SomaHorasNova(ByVal hora2 As String) As Double

        If String.IsNullOrWhiteSpace(hora2) Then hora2 = "00:00"

        Return TimeSpan.Parse(hora2).TotalHours

    End Function
    Public Function SomaMinutos(ByVal hora2 As String) As Double

        If String.IsNullOrWhiteSpace(hora2) Then hora2 = "00:00"

        Return TimeSpan.Parse(hora2).TotalMinutes


    End Function

    '====================================================================================
    '   Tira a diferença entre dois horários
    '====================================================================================
    Public Function subtraiHoras(ByVal hora1 As String, ByVal hora2 As String) As String

        Dim resultado1 As TimeSpan
        Dim corretivoHora2 As Boolean = False

        Try
            If CDate(hora1) < CDate(hora2) Then
                resultado1 = CDate(hora2) - CDate(hora1)
            Else
                resultado1 = CDate(hora1) - CDate(hora2)
            End If
            Return FormatDateTime(resultado1.ToString, DateFormat.ShortTime)
        Catch ex As Exception
        End Try

        Return "00:00"

    End Function

    '====================================================================================
    '   Converte um horario em decimal, ex.: 18:30 = 18,5
    '====================================================================================
    Public Function converteHorasDecimal(ByVal horas As String) As Decimal

        Dim expRegular = "([0-9]+|\d):[0-5]\d" ' Aceita no formato de horas, por exemplo "999:59"

        Dim vetorHoras(2) As String
        Dim intHoras As Integer
        Dim intMinutos As Integer
        Dim resultado As Decimal



        If Regex.IsMatch(horas, expRegular) Then
            'intHoras = TimeSpan.FromHours(vetorHoras(0)).TotalHours
            ' intMinutos = TimeSpan.FromMinutes(vetorHoras(1)).TotalMinutes
            If (horas.IndexOf("-") >= 0) Then
                vetorHoras = horas.ToString.Replace("-", "").Split(":")
                intHoras = vetorHoras(0)
                intMinutos = vetorHoras(1)

                resultado = intHoras + (intMinutos / 60) * -1
            Else
                vetorHoras = horas.ToString.Split(":")
                intHoras = vetorHoras(0)
                intMinutos = vetorHoras(1)

                resultado = intHoras + (intMinutos / 60)
            End If
            Return resultado


        Else

            Return -1   ' Significa que o dado passado não segue o padrão de horas, ex.: "999:59"

        End If

    End Function

    '====================================================================================
    '   Converte um decimal em formato horatio, ex.: 18,5 = 18:30
    '====================================================================================
    Public Function converteDecimalHoras(ByVal decHoras As String) As String

        Dim resultado As String
        Dim decMinutos
        Dim strHoras As String
        Dim strMinutos As String

        decMinutos = CStr(Math.Round((decHoras - Int(decHoras)) / 100 * 60, 2)) & "0"

        If Int(decHoras) < 10 Then
            strHoras = "0" & Int(decHoras)
        Else
            strHoras = Int(decHoras)
        End If

        strMinutos = Mid(decMinutos, 3, 2)

        If strMinutos = "" Then
            strMinutos = "00"
        End If

        resultado = strHoras & ":" & strMinutos

        Return resultado

    End Function

    '==========================================================================================================
    ' Função para auxiliar na marcação dos feriados
    '==========================================================================================================
    Public Function isDiaUtil(ByVal dia As DateTime) As Boolean
        If dia.DayOfWeek = System.DayOfWeek.Saturday Or dia.DayOfWeek = System.DayOfWeek.Sunday Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Function RetornarFeriados() As List(Of Feriados)
        Dim Feriados As New List(Of Feriados)
        Try
            'Cria a lista com os feriados
            Dim Sql As String = "select dia,mes,Descricao from tblFeriados with(nolock) order by mes,dia"

            If selectSQL(Sql) Then
                If dr.HasRows Then
                    While dr.Read
                        Dim Fer As New Feriados()
                        Fer.Dia = Integer.Parse(dr("dia"))
                        Fer.Mes = Integer.Parse(dr("mes"))
                        Fer.Descricao = dr("Descricao")
                        Feriados.Add(Fer)
                    End While
                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
        Return Feriados
    End Function

    '==========================================================================================================
    ' Retorna somente números da String dada como parametro
    '==========================================================================================================
    Public Function getSomenteNumeros(ByVal texto As String) As String

        Dim resultado As String = ""

        For i = 0 To (texto.Length - 1)
            If IsNumeric(texto.Substring(i, 1)) Then
                resultado += texto.Substring(i, 1)
            End If
        Next i

        Return resultado

    End Function

    ''' <summary>
    ''' Retorna o nome do colaborador através de seu código
    ''' </summary>
    ''' <param name="colCodigo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getNomeColaborador(ByVal colCodigo) As String

        Dim SQL = "SELECT colNome FROM v_colaboradores WHERE colCodigo = " & colCodigo

        If Not selectSQL(SQL) Then
            Return ""
        End If

        dr.Read()

        If dr.HasRows Then
            Return dr("colNome")
        Else
            Return "Nenhum"
        End If

    End Function

    ''' <summary>
    ''' Retorna o responsavel do projeto, se caso não houver GC, exibe o Diretor
    ''' </summary>
    ''' <param name="colCodigo"></param>
    ''' <param name="proCodigo"></param>
    ''' <param name="dataInicio"></param>
    ''' <param name="dataFim"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function getResponsavelProjeto(colCodigo As Integer, proCodigo As Integer, dataInicio As Date, dataFim As Date) As String

        Dim SQL = ""

        SQL =
        "SELECT apoAprovadorGC, apoAprovadorDir " &
        "FROM v_apontamento " &
        "WHERE colCodigo = " & colCodigo & " AND proCodigo = " & proCodigo & " " &
        "AND apoData BETWEEN '" & dataInicio & "' AND '" & dataFim & "'"

        If selectSQL(SQL) Then
            dr.Read()
            If IsNumeric(dr("apoAprovadorGC")) Then
                SQL = "SELECT colNome FROM v_colaboradores WHERE colCodigo = " & dr("apoAprovadorGC")
                If selectSQL(SQL) Then
                    dr.Read()
                    Return dr("colNome")
                Else
                    Return ""
                End If
            ElseIf IsNumeric(dr("apoAprovadorDir")) Then
                SQL = "SELECT colNome FROM v_colaboradores WHERE colCodigo = " & dr("apoAprovadorDir")
                If selectSQL(SQL) Then
                    dr.Read()
                    Return dr("colNome")
                Else
                    Return ""
                End If
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    ''' <summary>
    ''' Ordena e retorna o array passado como parametro
    ''' </summary>
    ''' <param name="arrayParaOrdernar"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function removeRepetidosVazios(ByVal arrayParaOrdernar As ArrayList) As ArrayList

        Dim i = 0

        While i < (arrayParaOrdernar.Count - 1)

            If arrayParaOrdernar(i).ToString.Trim <> "" Then

                Dim j = i + 1

                While j < (arrayParaOrdernar.Count)

                    If arrayParaOrdernar(i) = arrayParaOrdernar(j) Then
                        arrayParaOrdernar.RemoveAt(j)
                    End If

                    j = j + 1

                End While

            Else

                arrayParaOrdernar.RemoveAt(i)

            End If

            i = i + 1

        End While

        Return arrayParaOrdernar

    End Function


End Module
