Imports System.DirectoryServices

Module funcoes_AD

    Public erroConectarAD As String

    'Dim LDAPCaminho As String = "LDAP://192.168.111.3"
    Dim LDAPCaminho As String = "LDAP://10.20.2.10"

    '==============================================================================================
    ' Função que verifica se existe ou não usuário no Active Directory
    '==============================================================================================
    Public Function verificarAutenticacaoAD(ByVal usuario As String, ByVal senha As String) As String

        Dim de As New DirectoryEntry(LDAPCaminho, usuario, senha)
        Dim deResultado As SearchResult
        'Dim resultado As SearchResult

        ' Cria uma instancia da "Procura do Directory"
        Dim deProcura As New DirectorySearcher()
        deProcura.SearchRoot = de

        'Seta o filtro a procura
        'deProcura.Filter = "(&(objectClass=user)(cn=" & usuario & "))"
        deProcura.Filter = "(sAMAccountName=" & usuario & ")"
        'resultado = deProcura.FindOne()

        deProcura.SearchScope = SearchScope.Subtree
        'deNomeCompleto = resultado.Properties("mail")(0).ToString()

        Try
            'Procura o primeiro resultado, se caso não houver retorna False
            deResultado = deProcura.FindOne()
            Return deResultado.GetDirectoryEntry.Properties("name").Value
        Catch ex As Exception
            erroConectarAD = ex.Message()
            Return ""
        End Try

    End Function

    '==============================================================================================
    ' Função que retorna o nome completo do Usuario no AD
    ' Paramentros, nome do usuário e a propriedade ex: "name", "cn", "mail", etc.
    '==============================================================================================
    Public Function getPropriedade(ByVal usuario As String, ByVal propriedade As String) As String

        ' Aqui um usuário sem grupo, sem permissão nenhuma no AD, aqui usado somente para validar o acesso
        Dim de As New DirectoryEntry(LDAPCaminho, "intranet", "P@ssword")
        Dim Resultado As SearchResult

        ' Cria uma instancia da "Procura do Directory"
        Dim deProcura As New DirectorySearcher("(sAMAccountname=" & usuario & ")")
        deProcura.SearchRoot = de
        Try
            Resultado = deProcura.FindOne()
            de = Resultado.GetDirectoryEntry()
            Return de.Properties(propriedade).Item(0).ToString()
        Catch ex As Exception
            ' Tratamento do erro
            erroConectarAD = ex.Message()
        End Try

        Return ""

    End Function

End Module
