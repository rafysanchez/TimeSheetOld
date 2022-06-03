Partial Public Class exibeRecursos
    Inherits System.Web.UI.Page

    Protected URLVoltar
    Dim listaEmailsRepetidos As New List(Of String)
    Dim list As New List(Of String)
    Dim listaEmails As New List(Of String)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

#If DEBUG Then
        Session("sqlFiltros") = "SELECT * FROM [v_recursos] WHERE [recFrente]='ABAP' AND [recSkill]='Sênior' ORDER BY [recNome]"
#End If

        If Not IsPostBack Then

            ViewState("grid") = list
            ViewState("emails") = listaEmails

            URLVoltar = Request.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~/") _
                  & "consultaRecursos/recursos.aspx"

            voltar1.HRef = URLVoltar
            voltar2.HRef = URLVoltar

            btnCarregaPagina.Style.Add("visibility", "hidden")

            ' Tentando fazer com que a página ao carregar no postBack aparecesse o updateProgress, mas por enquanto sem sucesso,
            ' ai tive que fazer a chamada da sub btnCarrergaPagina_Click()
            carregaGrid(0)

        End If

    End Sub

    '*****************************
    '   Eventos
    '*****************************

    Private Sub carregaGrid(pagina As Integer)

        fonteDados.ConnectionString = getConnectionString()
        fonteDados.SelectCommand = Session("sqlFiltros")
        gridRecursos.DataBind()
        gridRecursos.PageIndex = 0

    End Sub

    Protected Sub ckbSelTodos_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)

        Dim CheckBoxTodos As CheckBox = CType(gridRecursos.HeaderRow.FindControl("ckbSelTodos"), CheckBox)

        For Each row As GridViewRow In gridRecursos.Rows
            Dim checkBox As CheckBox = CType(row.FindControl("ckbSelItem"), CheckBox)
            checkBox.Checked = CheckBoxTodos.Checked
        Next

        salvaStatusCheckBox()
        preparaLinkParaEnvioEmail(ViewState("emails"))

    End Sub

    Private Sub gridRecursos_DataBound(sender As Object, e As System.EventArgs) Handles gridRecursos.DataBound

        carregaStatusCheckBox()

    End Sub

    Private Sub gridRecursos_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gridRecursos.PageIndexChanging

        salvaStatusCheckBox()

        carregaGrid(e.NewPageIndex)

    End Sub

    Protected Sub ckbSelItem_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)

        salvaStatusCheckBox()
        preparaLinkParaEnvioEmail(ViewState("emails"))

    End Sub

    '*****************************
    '   Sub-rotinas
    '*****************************

    Private Sub salvaStatusCheckBox()

        list = ViewState("grid")
        listaEmails = ViewState("emails")

        Dim email = ""
        Dim msn = ""

        For Each row As GridViewRow In gridRecursos.Rows

            Dim id As String = gridRecursos.DataKeys(row.RowIndex)("recCodigo").ToString
            Dim isChecked As Boolean = CType(row.FindControl("ckbSelItem"), CheckBox).Checked

            email = CType(row.FindControl("linkEmail"), HyperLink).Text.Trim
            msn = CType(row.FindControl("linkMSN"), HyperLink).Text.Trim

            If isChecked Then
                If list.Contains(id) = False Then
                    list.Add(id)
                    ' Adiciona email ao array para adicionar no link de enviar e-mails
                    If email <> "" Then
                        listaEmails.Add(email)

                    ElseIf msn <> "" Then
                        listaEmails.Add(msn)
                    End If
                End If
            ElseIf list.Contains(id) Then
                list.Remove(id)
                If email <> "" Then ' Remove email no array para adicionar no link de enviar e-mails
                    listaEmails.Sort()
                    Dim i = 0
                    While i < listaEmails.Count
                        If listaEmails(i) = email Then
                            listaEmails.RemoveAt(i)
                            Exit While
                        End If
                        i = i + 1
                    End While
                ElseIf msn <> "" Then ' Remove msn no array para adicionar no link de enviar e-mails
                    listaEmails.Sort()
                    Dim i = 0
                    While i < listaEmails.Count
                        If listaEmails(i) = msn Then
                            listaEmails.RemoveAt(i)
                            Exit While
                        End If
                        i = i + 1
                    End While
                End If
            End If

        Next

        ViewState("grid") = list
        ViewState("emails") = listaEmails

        Select Case list.Count
            Case 0
                lblMensagem.Text = "Nenhum consultor selecionado"
            Case 1
                lblMensagem.Text = list.Count & " consultor selecionado"
            Case Else
                lblMensagem.Text = list.Count & " consultores selecionados"
        End Select

    End Sub

    Private Sub preparaLinkParaEnvioEmail(lista As List(Of String))

        Dim lst As List(Of String) = New List(Of String)

        Dim i = 0

        For i = 0 To lista.Count - 1
            lst.Add(lista(i))
        Next i

        lst.Sort()

        i = 0
        ' Loop que remove dados repetidos no array ou list
        While i < lst.Count
            If lst(i).ToString.Trim <> "" Then
                Dim j = i + 1
                While j < lst.Count
                    If lst(i) = lst(j) Then
                        lst.RemoveAt(j)
                        j = i + 1
                    Else
                        j = j + 1
                    End If
                End While
            Else
                lst.RemoveAt(i)
            End If
            i = i + 1
        End While

        Dim str = ""

        For i = 0 To lst.Count - 1
            str += lst(i) & ";"
        Next

        If str <> "" Then
            str = "mailto:?bcc=" & str.Substring(0, (str.Length - 1)).Trim
            hplEnviarEmails1.Visible = True
            hplEnviarEmails1.NavigateUrl = str
            hplEnviarEmails2.Visible = True
            hplEnviarEmails2.NavigateUrl = str
            lblMensagem.Text += " e " & lst.Count & " e-mails."
        Else
            hplEnviarEmails1.Visible = False
            hplEnviarEmails2.Visible = False
            lblMensagem.Text += " e nenhum e-mail."
        End If

        'lblMensagem.Text = str.Length

    End Sub

    Private Sub carregaStatusCheckBox()

        Dim list As List(Of String) = CType(ViewState("grid"), List(Of String))

        Try
            list = ViewState("grid")
        Catch ex As Exception
        End Try

        For Each row As GridViewRow In gridRecursos.Rows
            Dim id As String = gridRecursos.DataKeys(row.RowIndex)("recCodigo").ToString
            If list.Contains(id) Then
                Dim checkBox As CheckBox = CType(row.FindControl("ckbSelItem"), CheckBox)
                checkBox.Checked = True
            End If
        Next

    End Sub

End Class