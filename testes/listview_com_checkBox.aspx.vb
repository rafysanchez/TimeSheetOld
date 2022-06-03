Imports System.Globalization
Imports System.Web

Partial Public Class listview_com_checkBox
    Inherits System.Web.UI.Page

    Dim SQL As String
    ' Array usado para guardar os códigos dos colaboradores que serão removidos do listViewTodosConsultores
    Dim exceto As ArrayList = New ArrayList
    Dim colCodigo As String
    Dim nome As String = ""
    Dim modulo As String = ""
    Dim nivel As String = ""
    Dim tipoContrato As String = ""
    Dim taxaAtual As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            preencheListaTodosConsultores(exceto)
        End If

    End Sub

    Private Sub btnTeste_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTeste.Click
        lblMsg.Text = ""
        '   Faz um loop
        For Each item As ListViewDataItem In lstViewTodosConsultores.Items
            Dim ckbox As CheckBox = CType(item, ListViewDataItem).FindControl("chkConsultores")
            If ckbox.Checked Then
                lblMsg.Text += lstViewTodosConsultores.DataKeys.Item(item.DataItemIndex).Item("colCodigo") & "<br />"
                exceto.Add(lstViewTodosConsultores.DataKeys.Item(item.DataItemIndex).Item("colCodigo"))
            End If
        Next
        preencheListaTodosConsultores(exceto)
    End Sub

    Private Sub preencheListaTodosConsultores(ByVal exceto As ArrayList)

        ' ''''''' Criando uma DataTable
        Dim dt As New DataTable("consultores")
        ' ''''''' Criando as colunas para a DataTable
        dt.Columns.Add("colCodigo", GetType(String))
        dt.Columns.Add("nome", GetType(String))
        dt.Columns.Add("modulo", GetType(String))
        dt.Columns.Add("nivel", GetType(String))
        dt.Columns.Add("contrato", GetType(String))
        dt.Columns.Add("taxaAtual", GetType(String))

        ' Limpa a lista de "Todos consultores"
        lstViewTodosConsultores.Items.Clear()

        ' Select de todos os consultores para adicionar a lista "Todos os consultores"
        SQL = " SELECT * FROM v_colaboradores "
        SQL += "WHERE colPerfil='Consultores' AND colStatus = 'Ativo' ORDER BY colNome"

        If selectSQL(SQL) Then
            While dr.Read()
                colCodigo = ""
                nome = ""
                modulo = ""
                nivel = ""
                tipoContrato = ""
                taxaAtual = ""
                If dr("colNome") IsNot DBNull.Value Then
                    nome = dr("colNome")
                    colCodigo = dr("colCodigo")
                End If
                If dr("colModulo") IsNot DBNull.Value Then
                    modulo = dr("colModulo")
                End If
                If dr("colNivel") IsNot DBNull.Value Then
                    nivel = dr("colNivel")
                End If
                If dr("colTipoContrato") IsNot DBNull.Value Then
                    If dr("colValorFechado") = "N" Then
                        tipoContrato = dr("colTipoContrato")
                    Else
                        tipoContrato = dr("colTipoContrato") & " - F"
                    End If
                End If
                If dr("colSalario") IsNot DBNull.Value Then
                    taxaAtual = dr("colSalario")
                    taxaAtual = Decimal.Parse(taxaAtual).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))
                End If
                ' Se não houver o código do colaborador no array "exceto" então adiciona a linha no listView
                If Not exceto.Contains(colCodigo) Then
                    dt.Rows.Add(New Object() {colCodigo, nome, modulo, nivel, tipoContrato, taxaAtual})
                End If
            End While
        Else
            lblMsg.Text = sqlErro
            Return
        End If

        gvTodosConsultores.DataSource = dt
        gvTodosConsultores.DataBind()

    End Sub

    Private Sub gvTodosConsultores_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvTodosConsultores.DataBound

        Dim gvrPager As GridViewRow = gvTodosConsultores.BottomPagerRow

        If gvrPager Is Nothing Then
            Return
        End If

        ' get your controls from the gridview
        Dim ddlPages As DropDownList = DirectCast(gvrPager.Cells(0).FindControl("ddlPages"), DropDownList)
        Dim lblPageCount As Label = DirectCast(gvrPager.Cells(0).FindControl("lblPageCount"), Label)

        If ddlPages IsNot Nothing Then
            ' populate pager
            For i As Integer = 0 To gvTodosConsultores.PageCount - 1

                Dim intPageNumber As Integer = i + 1
                Dim lstItem As New ListItem(intPageNumber.ToString())

                If i = gvTodosConsultores.PageIndex Then
                    lstItem.Selected = True
                End If

                ddlPages.Items.Add(lstItem)
            Next
        End If

        ' populate page count
        If lblPageCount IsNot Nothing Then
            lblPageCount.Text = gvTodosConsultores.PageCount.ToString()
        End If

    End Sub

    Protected Sub ddlPages_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvrPager As GridViewRow = gvTodosConsultores.BottomPagerRow
        Dim ddlPages As DropDownList = CType(gvrPager.Cells(0).FindControl("ddlPages"), DropDownList)
        gvTodosConsultores.PageIndex = ddlPages.SelectedIndex
        ' a method to populate your grid
        preencheListaTodosConsultores(exceto)
    End Sub

    Protected Sub paginate(ByVal sender As Object, ByVal e As CommandEventArgs)
        ' get the current page selected
        Dim intCurIndex As Integer = gvTodosConsultores.PageIndex
        Select Case (e.CommandArgument.ToString.ToLower)
            Case "first"
                gvTodosConsultores.PageIndex = 0
            Case "prev"
                If intCurIndex > 0 Then
                    gvTodosConsultores.PageIndex = (intCurIndex - 1)
                End If
            Case "next"
                gvTodosConsultores.PageIndex = (intCurIndex + 1)
            Case "last"
                gvTodosConsultores.PageIndex = gvTodosConsultores.PageCount
        End Select
        ' popultate the gridview control
        preencheListaTodosConsultores(exceto)
    End Sub

    

    'Private Sub gvTodosConsultores_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvTodosConsultores.RowDataBound

    '    If gvTodosConsultores.PageIndex < 2 Then

    '        Dim imgAnt As ImageButton = DirectCast(e.Row.FindControl("imgAnterior"), ImageButton)
    '        Dim imgPro As ImageButton = DirectCast(e.Row.FindControl("imgProximo"), ImageButton)
    '        Dim pnl As Panel = DirectCast(e.Row.FindControl("pnlNumeros"), Panel)
    '        Dim qtdPaginas As Integer = gvTodosConsultores.PageCount

    '        If gvTodosConsultores.PageIndex < 2 Then
    '            IncluiRodape(pnl, 1)
    '        ElseIf gvTodosConsultores.PageIndex > (gvTodosConsultores.PageCount - 3) Then
    '            IncluiRodape(pnl, (gvTodosConsultores.PageCount - 4))
    '        Else
    '            IncluiRodape(pnl, (gvTodosConsultores.PageIndex - 1))
    '        End If

    '        ' Verifico se é a primeira página, caso seja, esconde o botão anterior
    '        imgAnt.Visible = Not gvTodosConsultores.PageIndex = 0
    '        ' Verifico se é a última página, caso seja, esconde o botão próximo
    '        imgPro.Visible = Not gvTodosConsultores.PageIndex = (gvTodosConsultores.PageCount - 1)

    '    End If

    'End Sub

    'Protected Sub imgAnterior_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)

    '    If gvTodosConsultores.PageIndex > 0 Then
    '        gvTodosConsultores.PageIndex -= 1
    '    End If

    '    gvTodosConsultores.DataSource = dataTable
    '    gvTodosConsultores.DataBind()

    'End Sub

    'Protected Sub imgProximo_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)

    '    If gvTodosConsultores.PageIndex < (gvTodosConsultores.PageCount - 1) Then
    '        gvTodosConsultores.PageIndex += 1
    '    End If

    '    gvTodosConsultores.DataSource = dataTable
    '    gvTodosConsultores.DataBind()

    'End Sub

    'Private Sub IncluiRodape(ByVal pnl As Panel, ByVal startNumber As Integer)

    '    For i As Integer = startNumber To startNumber + 4
    '        If gvTodosConsultores.PageIndex <> (i - 1) Then
    '            Dim lnk As New LinkButton()
    '            lnk.CommandArgument = Convert.ToString(i - 1)
    '            lnk.Text = " " & i.ToString() & " "
    '            'Aqui eu adiciono o evento lnk_Click para cada LinkButton
    '            'da numeração da paginação.
    '            'lnk.Click += New EventHandler(lnk_Click)
    '            pnl.Controls.Add(lnk)
    '        Else
    '            'Aqui coloca a página atual com uma label, assim indicando que aquela página
    '            'é a página corrente.
    '            Dim lbl As New Label()
    '            lbl.Text = " " & i.ToString() & " "
    '            pnl.Controls.Add(lbl)
    '        End If
    '    Next

    'End Sub

    
    

End Class