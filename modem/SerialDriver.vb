'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    Modem Serial Driver
'
' Copyright 2013 and Beyond
' All Rights Reserved
' ºººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººº
' €  All  rights reserved. No part of this  software  €€  This Software is Owned by        €
' €  may be reproduced or transmitted in any form or  €€                                   €
' €  by   any   means,  electronic   or  mechanical,  €€    GUANZON MERCHANDISING CORP.    €
' €  including recording, or by information  storage  €€     Guanzon Bldg. Perez Blvd.     €
' €  and  retrieval  systems, without  prior written  €€           Dagupan City            €
' €  from the author.                                 €€  Tel No. 522-1085 ; 522-9275      €
' ºººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººººº
'
' ==========================================================================================
'  Jheff [ 01/12/2013 09:05 am ]
'      Start this object
'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
Imports System.IO.Ports
Imports System.Text
Imports System.Threading
Imports System.Text.RegularExpressions

Imports ATSMS.Common

Public Class SerialDriver : Inherits SerialPort

    Private Const BUFFER_SIZE As Integer = 16384
    Private Const TIME_OUT As Integer = 3 * 1000
    Private Const WAIT_DATA_RETRIES = 50

    Private portEncoding As System.Text.Encoding
    Private isSendingCommand As Boolean

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
        Me.ReadTimeout = TIME_OUT
        Me.WriteTimeout = TIME_OUT
        portEncoding = New ASCIIEncoding()
        isSendingCommand = False
    End Sub


    ''' <summary>
    ''' Sending AT commands to the serial port
    ''' </summary>
    ''' <param name="commandString"></param>
    ''' <remarks></remarks>
    Public Sub Send(ByVal commandString As String)
        Try
            Dim callText As String
            Dim callCommand As Byte()
            Dim iCount As Integer
            callText = commandString
            iCount = 0
            callCommand = portEncoding.GetBytes(callText)
            isSendingCommand = True
            System.Diagnostics.Debug.WriteLine("Write Socket:" + commandString)
            If Me.IsOpen = False Then Me.Open()
            Me.Write(callCommand, 0, callCommand.Length)
            If Me.IsOpen = False Then Me.Open()
            While Me.BytesToRead <= 0
                Thread.Sleep(500)
                iCount = iCount + 1
                Debug.Print(iCount)
                If (iCount = 40) Then
                    isSendingCommand = False
                    Exit While
                End If
            End While
            isSendingCommand = False
        Catch ex As System.Exception
            isSendingCommand = False
            Throw New InvalidCommandException(ex.Message, ex)
        End Try
    End Sub

    ''' <summary>
    ''' Send AT command
    ''' </summary>
    ''' <param name="commandString"></param>
    ''' <returns></returns>
    ''' 
    ''' <remarks></remarks>
    Public Function SendCmd(ByVal commandString As String) As String
        If Not commandString.EndsWith(Chr(26)) Then
            If Not (commandString.EndsWith(ControlChars.Cr) Or _
                commandString.EndsWith(ControlChars.CrLf)) Then
                commandString = commandString + ControlChars.Cr
            End If
        End If
        Send(commandString)
        If BytesToRead > 0 Then
            Return ReadExisting()
        End If
        Return String.Empty
    End Function

    ''' <summary>
    ''' Send AT command and expect certain response
    ''' </summary>
    ''' <param name="commandString"></param>
    ''' <param name="expectedResponse"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendCmd(ByVal commandString As String, _
                              ByVal expectedResponse As String) As String
        Dim response As String = ""
        Dim iCount As Integer

        If Not commandString.EndsWith(Chr(26)) Then
            If Not (commandString.EndsWith(ControlChars.Cr) Or _
                commandString.EndsWith(ControlChars.CrLf)) Then
                commandString = commandString + ControlChars.Cr
            End If
        End If
        Debug.Print(commandString)
        Send(commandString)

        If BytesToRead > 0 Then
            response = ReadExisting()
            'System.Diagnostics.Debug.WriteLine("SendCmdByRes:" + response)
            If response Is Nothing Or response.IndexOf(expectedResponse) < 0 Then
                If Not response Is Nothing Then
                    If response.Trim = ATHandler.RESPONSE_ERROR Then
                        Throw New UnknownException("Unknown exception in sending command")
                    End If
                    If (response.IndexOf(ATHandler.RESPONSE_CMS_ERROR) >= 0) Then
                        ' Error in the command
                        Dim cols() As String = response.Split(":")
                        If cols.Length > 1 Then
                            Dim errorCode As String = cols(1)

                            ' Check the error code in the resource
                            Dim errorDescription As String = Resource.GetMessage(errorCode.Trim)

                            ' Set error code and description

                            ' Throw exception
                            If errorDescription <> "" Then
                                Return String.Empty
                                Exit Function
                                'jheff 
                                'disable q muna ito
                                'Throw New InvalidCommandException(errorDescription)
                            Else
                                'Throw New UnknownException("CMS Error code " + errorCode)
                            End If
                        End If
                    End If
                End If

                For iCount = 1 To 50
                    Debug.Print(iCount)
                    Thread.Sleep(1000)
                    If Me.IsOpen = False Then Me.Open()
                    'System.Diagnostics.Debug.WriteLine("SendCmdByRes1:" + response)
                    response = response + ReadExisting()
                    If response.IndexOf(expectedResponse) >= 0 Then
                        Exit For
                    ElseIf response.IndexOf("NO ANSWER") >= 0 Then
                        Exit For
                    ElseIf response.IndexOf("NO CARRIER") >= 0 Then
                        Exit For
                    ElseIf response.IndexOf("NO DIALTONE") >= 0 Then
                        Exit For
                    ElseIf response.IndexOf("NO NETWORK") >= 0 Then
                        Exit For
                    ElseIf response.IndexOf("BUSY") >= 0 Then
                        Exit For
                    ElseIf response.IndexOf("ERROR") >= 0 Then
                        Exit For
                    ElseIf response.IndexOf("+CMTI:") >= 0 Then
                        Exit For
                    ElseIf response.IndexOf("+CGREG: 1") >= 0 Then
                        Exit For
                    Else
                        Debug.Print("SD RESPONSE: " & response)
                    End If
                Next
            End If
        End If
        Return response
    End Function

    ''' <summary>
    ''' Send diagnostics commands
    ''' </summary>
    ''' <param name="commandString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Diagnose(ByVal commandString) As String
        Try
            Dim commands() As String = commandString.Split(ControlChars.Cr)
            Dim i As Integer
            Dim response As String = String.Empty
            For i = 0 To commands.Length - 1
                If commands(i).Trim <> String.Empty Then
                    response = response & commands(i) & ControlChars.CrLf
                    response = response & SendCmd(commands(i).Trim)
                End If
            Next
            Return response
        Catch ex As System.Exception
            Throw New InvalidCommandException(ex.Message, ex)
        End Try
        Return String.Empty
    End Function

    ''' <summary>
    ''' Send diagnostics command in a file
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <param name="outputFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Diagnose(ByVal fileName As String, ByVal outputFileName As String) As Boolean
        Try
            Dim fileReader As String
            fileReader = My.Computer.FileSystem.ReadAllText(fileName)
            Dim commands() As String = fileReader.Split(ControlChars.Cr)
            Dim i As Integer
            Dim response As String = String.Empty
            For i = 0 To commands.Length - 1
                If commands(i).Trim <> String.Empty Then
                    response = response & commands(i) & ControlChars.CrLf
                    response = response & SendCmd(commands(i).Trim)
                End If
            Next
            My.Computer.FileSystem.WriteAllText(outputFileName, response, True)
            Return True
        Catch ex As System.Exception
            Throw New InvalidCommandException(ex.Message, ex)
        End Try
        Return False
    End Function

    ''' <summary>
    ''' Read response from the serial port
    ''' </summary>
    ''' <param name="expectedResponse"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadResponse(ByVal expectedResponse As String) As String

        Dim response As String
        Dim iCount As Integer
        response = ""

        For iCount = 1 To 60
            response = ReadExisting()
            If response.Trim = ATHandler.RESPONSE_ERROR Then
                Throw New UnknownException("Unknown exception in retrieving data")
            End If

            If (response.IndexOf(ATHandler.RESPONSE_CMS_ERROR) >= 0) Then
                ' Error in the command
                Dim cols() As String = response.Split(":")
                If cols.Length > 1 Then
                    Dim errorCode As String = cols(1)

                    ' Check the error code in the resource
                    Dim errorDescription As String = Resource.GetMessage(errorCode.Trim)

                    ' Set error code and description

                    ' Throw exception
                    If errorDescription <> "" Then
                        Throw New InvalidCommandException(errorDescription)
                    Else
                        Throw New UnknownException("CMS Error code " + errorCode)
                    End If
                End If
            End If

            Debug.Print("READ RESPONSE: " & response & "»" & expectedResponse)
            If response.IndexOf(expectedResponse) >= 0 Then
                Exit For
            End If

            Thread.Sleep(1000)
        Next
        Return response
    End Function

    ''' <summary>
    ''' Indicate if command is being sent
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsCommandMode() As Boolean
        Get
            Return isSendingCommand
        End Get
    End Property

    Public Function Receive(ByVal expectedResponse As String) As String
        Dim response As String
        response = ""

        Do
            response = ReadExisting()
            If response.Trim = ATHandler.RESPONSE_ERROR Then
                Throw New UnknownException("Unknown exception in retrieving data")
            End If

            If (response.IndexOf(ATHandler.RESPONSE_CMS_ERROR) >= 0) Then
                ' Error in the command
                Dim cols() As String = response.Split(":")
                If cols.Length > 1 Then
                    Dim errorCode As String = cols(1)

                    ' Check the error code in the resource
                    Dim errorDescription As String = Resource.GetMessage(errorCode.Trim)

                    ' Set error code and description

                    ' Throw exception
                    If errorDescription <> "" Then
                        Throw New InvalidCommandException(errorDescription)
                    Else
                        Throw New UnknownException("CMS Error code " + errorCode)
                    End If
                End If
            End If

            If response.IndexOf(expectedResponse) >= 0 Then
                Exit Do
            End If
        Loop
        Return response
    End Function

    'Private Sub SerialDriver_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles Me.DataReceived
    '    Debug.Print(portEncoding.GetBytes(Me.ReadExisting).ToString)
    'End Sub
End Class