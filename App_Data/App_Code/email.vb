﻿Imports System.Net.Mail

Public Class email
    ''' <summary>
    ''' Envia um email
    ''' </summary>
    ''' <param name="from">Endereco do Remetente</param>
    ''' <param name="recepient">Destinatario</param>
    ''' <param name="bcc">recipiente Bcc</param>
    ''' <param name="cc">recipiente Cc</param>
    ''' <param name="subject">Assunto do email</param>
    ''' <param name="body">Corpo da mensagem de email</param>
    Public Shared Sub enviaMensagemEmail(ByVal from As String, ByVal recepient As String, ByVal bcc As String, _
      ByVal cc As String, ByVal subject As String, ByVal body As String)

        ' cria uma instância do objeto MailMessage
        Dim mMailMessage As New MailMessage()
        ' Define o endereço do remetente
        mMailMessage.From = New MailAddress(from)
        ' Define o destinario da mensagem
        mMailMessage.To.Add(New MailAddress(recepient))

        ' Verifica se o valor para bcc é null ou uma string vazia
        If Not bcc Is Nothing And bcc <> String.Empty Then
            ' Define o endereço bcc
            mMailMessage.Bcc.Add(New MailAddress(bcc))
        End If

        ' verifica se o valor para cc é nulo ou uma string vazia
        If Not cc Is Nothing And cc <> String.Empty Then
            ' Define o endereço cc
            mMailMessage.CC.Add(New MailAddress(cc))
        End If

        ' Define o assunto
        mMailMessage.Subject = subject
        ' Define o corpo da mensagem
        mMailMessage.Body = body

        ' Define o formato do email como HTML
        mMailMessage.IsBodyHtml = True
        ' Define a prioridade da mensagem como normal
        mMailMessage.Priority = MailPriority.Normal

        ' Cria uma instância de SmtpClient - Nota - Define qual o host a ser usado para envio
        ' de mensagens, no local de smtp.server.com use o nome do SEU servidor
        Dim mSmtpClient As New SmtpClient("smtp.server.com")
        ' Envia o email
        mSmtpClient.Send(mMailMessage)

    End Sub

End Class
