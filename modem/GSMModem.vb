'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    Modem GSMModem
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
Imports System.Threading
Imports System.Text

Imports ATSMS.Common
Imports ATSMS.SMS.Encoder.ConcatenatedShortMessage
Imports ATSMS.SMS.Encoder.SMS
Imports ATSMS.SMS
Imports ATSMS.SMS.Decoder

Public Class GSMModem : Inherits Modem : Implements IDisposable

#Region "Call Status"
    Public Enum xeCallStatus
        xeCallStatusNoDialTone = 0
        xeCallStatusBusy = 1
        xeCallStatusNoCarrier = 2
        xeCallStatusNoAnswer = 3
        xeCallStatusConnected = 4
        xeCallStatusDialling = 5
        xeCallStatusRinging = 6
        xeCallStatusNoNetwork = 7
        xeCallStatusEndCall = 8
        xeCallStatusError = 9
        xeCallStatusNone = 10
    End Enum
#End Region

#Region "Events"
    Public Delegate Sub DoEvents()
    Public Event doE As DoEvents
    Public Event CallStatus(ByVal Status As xeCallStatus)
#End Region

#Region "Protected Members"

    Protected Shared iRefNo As Integer

    Protected isAutoDeleteReadMessage As Boolean
    Protected isAutoDeleteSentMessage As Boolean
    Protected isDeliveryReport As Boolean
    Protected strIMEI As String
    Protected strIMSI As String
    Protected isIncomingCallIndication As Boolean
    Protected enumLongMessage As EnumLongMessage
    Protected enumMessageMemory As EnumMessageMemory
    Protected strNetwork As String
    Protected isNewMessageIndication As Boolean
    Protected strOwnNumber As String
    Protected strPIN As String
    Protected strSMSC As String
    Protected enumValidity As ENUM_TP_VALID_PERIOD
    Protected strPhoneModel As String
    Protected strDtmfDigits As String
    Protected strMNC As String
    Protected strMCC As String
    Protected strLAI As String
    Protected strCellId As String
    Protected bClip As Boolean

    Protected msgTimer As Timer

    Protected oOutbox As Outbox
    Protected oMessageStore As MessageStore
    Protected bSendingSMS As Boolean
    Protected strMSISDN As String
    Protected bIncomingAnswered As Boolean
    Protected p_bCancelFlag As Boolean
    Protected p_bCallConnected As Boolean
#End Region

#Region "Constructor"

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()

        ' Initialize instances
        oOutbox = New Outbox
        oMessageStore = New MessageStore
        oMessageStore.Modem = Me

        ' SMS thread
        bSendingSMS = False
        msgTimer = New Timer(New TimerCallback(AddressOf SMSThread), Nothing, 0, 5000)

        iRefNo = 1

        enumMessageMemory = Common.EnumMessageMemory.PHONE
        enumValidity = ENUM_TP_VALID_PERIOD.OneDay
        isAutoDeleteReadMessage = False
        isAutoDeleteSentMessage = False

    End Sub
#End Region

#Region "Disposable"

    Protected Overridable Overloads Sub Dispose( _
                ByVal disposing As Boolean)
        If Not Me.disposed Then
            If disposing Then
                MyBase.Dispose()
            End If
            ' Add code here to release the unmanaged resource.

            ' Note that this is not thread safe.
        End If
        Me.disposed = True
    End Sub

#End Region

#Region "Properties"

    Public Property AutoDeleteReadMessage() As Boolean
        Get
            Return Me.isAutoDeleteReadMessage
        End Get
        Set(ByVal Value As Boolean)
            Me.isAutoDeleteReadMessage = Value
        End Set
    End Property

    Public Property AutoDeleteSentMessage() As Boolean
        Get
            Return Me.isAutoDeleteSentMessage
        End Get
        Set(ByVal Value As Boolean)
            Me.isAutoDeleteSentMessage = Value
        End Set
    End Property

    Public Property DeliveryReport() As Boolean
        Get
            Return Me.isDeliveryReport
        End Get
        Set(ByVal Value As Boolean)
            Me.isDeliveryReport = Value
        End Set
    End Property

    Public ReadOnly Property IMEI() As String
        Get
            If Len(strIMEI) = 0 Then
                strIMEI = GetIMEI()
            End If
            Return strIMEI
        End Get
    End Property

    Public ReadOnly Property IMSI() As String
        Get
            If Len(strIMSI) = 0 Then
                strIMSI = GetIMSI()
            End If
            Return strIMSI
        End Get
    End Property

    Public ReadOnly Property DtmfDigits() As String
        Get
            If Len(strDtmfDigits) = 0 Then
                strDtmfDigits = GetDtmfSupport()
            End If
            Return strDtmfDigits
        End Get
    End Property

    Public Property IncomingCallIndication() As Boolean
        Get
            Return Me.isIncomingCallIndication
        End Get
        Set(ByVal Value As Boolean)
            Me.isIncomingCallIndication = Value
            If (Me.isIncomingCallIndication) Then
                EnableCLIP()
            End If
        End Set
    End Property

    Public Property LongMessage() As EnumLongMessage
        Get
            Return Me.enumLongMessage
        End Get
        Set(ByVal Value As EnumLongMessage)
            Me.enumLongMessage = Value
        End Set
    End Property

    Public Property MessageMemory() As EnumMessageMemory
        Get
            Return Me.enumMessageMemory
        End Get
        Set(ByVal Value As EnumMessageMemory)
            Me.enumMessageMemory = Value
            SetMemoryLocation()
        End Set
    End Property

    Public ReadOnly Property Network() As String
        Get
            Return Me.strNetwork
        End Get
    End Property

    Public Property NewMessageIndication() As Boolean

        Get
            Return Me.isNewMessageIndication
        End Get
        Set(ByVal Value As Boolean)
            Me.isNewMessageIndication = Value
            If Me.isNewMessageIndication Then
                InitMsgIndication()
            End If
        End Set
    End Property

    Public ReadOnly Property OwnNumber() As String
        Get
            If Len(strOwnNumber) = 0 Then
                strOwnNumber = GetMSISDN()
            End If
            Return strOwnNumber
        End Get
    End Property

    Public ReadOnly Property MSISDN() As String
        Get
            If Len(strOwnNumber) = 0 Then
                strOwnNumber = GetMSISDN()
            End If
            Return strOwnNumber
        End Get
    End Property

    Public Property SMSC() As String
        Get
            If (Len(strSMSC) = 0) Then
                Me.strSMSC = GetSCA()
                If (Len(Me.strSMSC) > 0) Then
                    Me.strSMSC = Me.strSMSC.Split(",")(0).Trim()
                End If
            End If
            Return Me.strSMSC
        End Get
        Set(ByVal Value As String)
            Me.strSMSC = Value
        End Set
    End Property

    Public Property Validity() As ENUM_TP_VALID_PERIOD
        Get
            Return Me.enumValidity
        End Get
        Set(ByVal Value As ENUM_TP_VALID_PERIOD)
            Me.enumValidity = Value
        End Set
    End Property

    Public Property Clip() As Boolean
        Get
            Return Me.bClip
        End Get
        Set(ByVal value As Boolean)
            Me.bClip = value
            If Me.bClip Then
                EnableCLIP()
            Else
                DisableCLIP()
            End If
        End Set
    End Property

    Public ReadOnly Property PhoneModel() As String
        Get
            If (Len(strPhoneModel) = 0) Then
                strPhoneModel = GetPhoneModel()
            End If
            Return Me.strPhoneModel
        End Get
    End Property



#End Region

#Region "Private Functions"

    Private Function SendTextSMS( _
               ByVal destinationNumber As String, _
               ByVal textMessage As String) _
                   As String

        If commandHandler.Is_AT_CMGF_0_Supported Then
            Try
                Dim responses() As String
                Dim response As String
                response = serialDriver.SendCmd(ATHandler.CMGF_COMMAND & "=0", ATHandler.RESPONSE_OK)
                responses = ParseATResponse(response)
                If responses(1).Trim() = ATHandler.RESPONSE_OK Then
                    Dim pduObject As New PDU
                    Dim PDUCodes() As String = pduObject.GetPDU( _
                                        "", destinationNumber, ENUM_TP_DCS.DefaultAlphabet, _
                                        Me.Validity, 1, Me.isDeliveryReport, _
                                        textMessage)
                    Dim i, j, ATLength As Integer

                    For i = 0 To PDUCodes.Length - 1
                        ATLength = pduObject.GetATLength(PDUCodes(i))
                        For j = 0 To 3
                            response = serialDriver.SendCmd(ATHandler.CMGW_COMMAND & "=" & ATLength & "", ">")
                            If response Is Nothing Or response = String.Empty Then
                                response = serialDriver.SendCmd(ATHandler.CMGW_COMMAND & "=" & ATLength & "", ">")
                            Else
                                Exit For
                            End If
                        Next

                        responses = ParseATResponse(serialDriver.SendCmd(PDUCodes(i) & Chr(26), ATHandler.RESPONSE_OK))
                        If responses.Length > 1 Then
                            Dim line As String = responses(0)
                            Dim cols() As String = line.Split(":")
                            If (cols.Length > 1) Then
                                Dim index As Integer = cols(1).Trim
                                responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CMSS_COMMAND & "=" & index & "", ATHandler.RESPONSE_OK))
                                If responses.Length > 1 Then
                                    line = responses(0)
                                    cols = line.Split(":")
                                    If (cols.Length > 1) Then
                                        response = cols(1).Trim

                                        If isAutoDeleteSentMessage Then
                                            Try
                                                DeleteSMS(index)
                                            Catch ex As Exception
                                                isAutoDeleteSentMessage = False
                                            End Try
                                        End If

                                        Return response
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
                Return String.Empty
            Catch ex As System.Exception
                Throw New InvalidOpException("Error sending SMS message: " + ex.Message, ex)
            End Try
        Else
            Try
                Dim response As String = serialDriver.SendCmd(ATHandler.CMGF_COMMAND & "=1", ATHandler.RESPONSE_OK)
                response = serialDriver.SendCmd(ATHandler.CMGS_COMMAND & "=""" & destinationNumber & """")
                Dim responses() As String = ParseATResponse(serialDriver.SendCmd(textMessage & Chr(26), ATHandler.CMGS_RESPONSE))
                'Dim responses() As String = ParseATResponse(serialDriver.SendCmd(textMessage & Chr(26)))
                If responses.Length > 1 Then
                    If Not responses(0) Is Nothing Then
                        Dim line As String = responses(0)
                        Dim cols() As String = line.Split(":")
                        If (cols.Length > 1) Then
                            Return cols(1).Trim
                        End If
                    End If
                End If
                Return String.Empty
            Catch ex As System.Exception
                Throw New InvalidOpException("Error sending SMS message: " + ex.Message, ex)
            End Try
        End If
    End Function

    Private Function SendHexSMS(ByVal destinationNumber As String, ByVal hexMessage As String _
                                ) As String
        Try
            Dim response As String = serialDriver.SendCmd(ATHandler.CMGF_COMMAND & "=0", ATHandler.RESPONSE_OK)
            If response.Trim() = ATHandler.RESPONSE_OK Then
                Dim pduObject As New PDU
                Dim ATLength As Integer
                Dim responses() As String
                ATLength = pduObject.GetATLength(hexMessage)
                response = serialDriver.SendCmd(ATHandler.CMGW_COMMAND & "=" & ATLength & "", ">")
                responses = ParseATResponse(serialDriver.SendCmd(hexMessage & Chr(26), ATHandler.RESPONSE_OK))
                If responses.Length > 1 Then
                    Dim line As String = responses(0)
                    Dim cols() As String = line.Split(":")
                    If (cols.Length > 1) Then
                        Dim index As Integer = cols(1).Trim
                        responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CMSS_COMMAND & "=" & index & "", ATHandler.RESPONSE_OK))
                        If responses.Length > 1 Then
                            line = responses(0)
                            cols = line.Split(":")
                            If (cols.Length > 1) Then
                                response = cols(1).Trim

                                If isAutoDeleteSentMessage Then
                                    Try
                                        DeleteSMS(index)
                                    Catch ex As Exception
                                        isAutoDeleteSentMessage = False
                                    End Try
                                End If

                                Return response
                            End If
                        End If
                    End If
                End If
            End If
            Return String.Empty
        Catch ex As System.Exception
            Throw New InvalidOperationException("Error sending SMS message: " + ex.Message, ex)
        End Try
    End Function

    Private Function SendUnicodeSMS(ByVal destinationNumber As String, _
                ByVal textMessage As String) As String
        Try
            Dim response As String = serialDriver.SendCmd("AT+CSCS?", "+CSCS")
            Dim results() As String = response.Split(":")
            If results.Length > 1 Then
                Dim defaultCharater As String = results(1).Trim()
                response = serialDriver.SendCmd("AT+CSCS=""UCS2""")
                If response.Trim = ATHandler.RESPONSE_OK Then
                    SendUnicodeSMS = SendSMSInPDU(destinationNumber, textMessage, ENUM_TP_DCS.UCS2)

                    'Reset back
                    serialDriver.SendCmd("AT+CSCS=" + defaultCharater)
                    Exit Function
                End If
            End If
            SendUnicodeSMS = String.Empty
        Catch ex As System.Exception
            Throw New InvalidOperationException("Error sending SMS message: " + ex.Message, ex)
        End Try
    End Function

    Private Function SendClass27BitSMS(ByVal destinationNumber As String, _
               ByVal textMessage As String) As String
        Try
            SendClass27BitSMS = SendSMSInPDU(destinationNumber, textMessage, ENUM_TP_DCS.Class2_UD_7bits)
        Catch ex As System.Exception
            Throw New InvalidOperationException("Error sending SMS message: " + ex.Message, ex)
        End Try
    End Function

    Private Function SendClass28BitSMS(ByVal destinationNumber As String, _
                    ByVal textMessage As String) As String
        Try
            SendClass28BitSMS = SendSMSInPDU(destinationNumber, textMessage, ENUM_TP_DCS.Class2_UD_8bits)
        Catch ex As System.Exception
            Throw New InvalidOperationException("Error sending SMS message: " + ex.Message, ex)
        End Try
    End Function

    Private Function GetDtmfSupport() As String
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.VTS_COMMAND & "=?"))
            Dim results As String = response(0).Trim
            If (results.Contains(ATHandler.VTS_RESPONSE)) Then
                Dim cols() As String = results.Split(":")
                Dim dtmfDigits As String = cols(1).Trim
                GetDtmfSupport = dtmfDigits.Substring(dtmfDigits.IndexOf("(") + 1, dtmfDigits.IndexOf(")") - 1)
            Else
                GetDtmfSupport = ATHandler.RESPONSE_NOT_SUPPORTED
            End If
        Catch ex As System.Exception
            Throw New InvalidOpException("DTMF not supported", ex)
        End Try
    End Function

    Private Function GetIMEI() As String
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CGSN_COMMAND))
            If Not response Is Nothing And response.Length > 0 Then
                If Not response(0) Is Nothing Then
                    If response(0) <> ATHandler.RESPONSE_ERROR Then
                        Return response(0).Trim
                    Else
                        Throw New InvalidOpException("Unable to retrieve IMEI")
                    End If
                Else
                    Throw New InvalidOpException("Unable to retrieve IMEI")
                End If
            Else
                Throw New InvalidOpException("Unable to retrieve IMEI")
            End If
        Catch ex As System.Exception
            Throw New InvalidOpException("Cannot retrieve IMEI", ex)
        End Try
    End Function

    Private Function GetIMSI() As String
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CIMI_COMMAND))
            If Not response Is Nothing And response.Length > 0 Then
                If Not response(0) Is Nothing Then
                    If response(0) <> ATHandler.RESPONSE_ERROR Then
                        Return response(0).Trim
                    Else
                        Throw New InvalidOpException("Unable to retrieve IMSI")
                    End If
                Else
                    Throw New InvalidOpException("Unable to retrieve IMSI")
                End If
            Else
                Throw New InvalidOpException("Unable to retrieve IMEI")
            End If
        Catch ex As System.Exception
            Throw New InvalidOpException("Unable to retrieve IMSI", ex)
        End Try
    End Function

    Private Function GetSCA() As String
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CSCA_COMMAND & "?", ATHandler.CSCA_RESPONSE))
            If Not response Is Nothing Then
                If response.Length > 0 Then
                    If Not response(0) Is Nothing Then
                        Dim cols() As String = response(0).Split(":")
                        Return cols(1).Replace("""", "").Trim
                    End If
                End If
            End If
            Return String.Empty
        Catch ex As System.Exception
            Throw New InvalidOpException("Error getting SCA", ex)
        End Try
    End Function

    Private Function SendSMSInPDU(ByVal destinationNumber As String, _
                                 ByVal textMessage As String, _
                                 ByVal dcs As ENUM_TP_DCS _
                               ) As String
        Try
            If commandHandler.Is_AT_CMGF_0_Supported Then
                Dim response As String = ""
                Dim pduObject As New PDU
                Dim PDUCodes() As String = pduObject.GetPDU( _
                                    "", destinationNumber, dcs, _
                                    Me.Validity, 1, Me.isDeliveryReport, _
                                    textMessage)
                Dim i, ATLength As Integer
                Dim responses() As String

                For i = 0 To PDUCodes.Length - 1
                    ATLength = pduObject.GetATLength(PDUCodes(i))
                    response = serialDriver.SendCmd(ATHandler.CMGS_COMMAND & "=" & ATLength & "", ">")
                    responses = ParseATResponse(serialDriver.SendCmd(PDUCodes(i) & Chr(26), ATHandler.CMGS_RESPONSE))
                    If responses.Length > 1 Then
                        Dim line As String = responses(0)
                        Dim cols() As String = line.Split(":")
                        If (cols.Length > 1) Then
                            Dim index As Integer = cols(1).Trim
                            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CMSS_COMMAND & "=" & index & "", ATHandler.RESPONSE_OK))
                            If responses.Length > 1 Then
                                line = responses(0)
                                cols = line.Split(":")
                                If (cols.Length > 1) Then
                                    response = cols(1).Trim
                                    Return response
                                End If
                            End If
                        End If
                    End If
                Next
            End If
            Return String.Empty
        Catch ex As System.Exception
            Throw New InvalidOperationException("Error sending SMS message: " + ex.Message, ex)
        End Try
    End Function

    Private Function SendSMSConcatenated(ByVal destinationNumber As String, _
                                 ByVal textMessage As String, _
                                 ByVal dcs As ENUM_TP_DCS _
                               ) As String
        Try
            If commandHandler.Is_AT_CMGF_0_Supported Then
                Dim response As String = ""
                Dim pduObject As New PDU
                Dim PDUCodes() As String = pduObject.GetPDUConcatenated( _
                                    "", destinationNumber, dcs, _
                                    Me.Validity, 1, Me.isDeliveryReport, _
                                    textMessage)
                Dim i, ATLength As Integer
                Dim responses() As String

                ATLength = pduObject.GetATLength(PDUCodes(0))
                response = serialDriver.SendCmd(ATHandler.CMGS_COMMAND & "=" & ATLength & "", ">")
                responses = ParseATResponse(serialDriver.SendCmd(PDUCodes(i) & Chr(26), ATHandler.CMGS_RESPONSE))

                If responses.Length > 1 Then
                    'Dim line As String = responses(0)
                    'Dim cols() As String = line.Split(":")
                    'If (cols.Length > 1) Then
                    '    Dim index As Integer = cols(1).Trim
                    '    responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CMSS_COMMAND & "=" & index & "", ATHandler.RESPONSE_OK))
                    '    If responses.Length > 1 Then
                    '        line = responses(0)
                    '        cols = line.Split(":")
                    '        If (cols.Length > 1) Then
                    '            response = cols(1).Trim
                    '            Return response
                    '        End If
                    '    End If
                    'End If
                End If
            End If
            Return String.Empty
        Catch ex As System.Exception
            Throw New InvalidOperationException("Error sending SMS message: " + ex.Message, ex)
        End Try
    End Function

    Private Function SendLongMessageConcat(ByVal sNumberx As String, _
                                    ByVal sMessage As String) As String

        Dim lnNoMessage As Integer
        Dim lsUserData As String
        Dim lnUserData As Integer
        Dim lsConcatMsg As String
        Dim lsUDHValue As String
        Dim lnSeqNmbr As Integer

        Dim lnCtr As Integer
        Dim lnStart As Integer
        Dim lnLength As Integer
        Dim lnReference As Integer

        lsUserData = sMessage
        lnUserData = Len(lsUserData)
        lnNoMessage = lnUserData \ 153 + IIf(lnUserData Mod 153 > 0, 1, 0)
        lnReference = GenerateReferenceNo()
        Debug.Print(lnReference)

        lnStart = IIf(lnSeqNmbr = 0, 1, (153 * lnSeqNmbr) + 1)
        For lnCtr = lnSeqNmbr + 1 To lnNoMessage
            lnLength = IIf(lnUserData > (153 * lnCtr), 153, lnUserData - 153)
            lsConcatMsg = Mid(lsUserData, lnStart, lnLength)
            lsUDHValue = EncodeUDHAscii(lnReference, lnNoMessage, lnCtr)
            Call SendSMSConcatenated(sNumberx, lsUDHValue & lsConcatMsg, ENUM_TP_DCS.DefaultAlphabet)

            lnStart = lnLength + lnStart
        Next

        Return String.Empty
    End Function

    Public Function EncodeUDHAscii(ByVal ReferNo As Integer, _
      ByVal NoOfMessage As Integer, _
      ByVal MsgSequence As Integer) As String
        Dim lasUDHBin(6) As String
        Dim lsResult As String
        Dim lsTemp As String
        Dim lnCtr As Integer

        lasUDHBin(0) = Dec2Bin7(5)
        lasUDHBin(1) = Dec2Bin7(0)
        lasUDHBin(2) = Dec2Bin7(3)
        lasUDHBin(3) = Dec2Bin7(ReferNo)
        lasUDHBin(4) = Dec2Bin7(NoOfMessage)
        lasUDHBin(5) = Dec2Bin7(MsgSequence)
        lasUDHBin(6) = Dec2Bin7(0) ' receiver of the additional character when converted to gsm 7 character

        lsTemp = Mid(lasUDHBin(0), 1, 1)
        EncodeUDHAscii = GSM7ToAscii(Mid(lasUDHBin(0), 2))
        For lnCtr = 1 To UBound(lasUDHBin) - 1
            lsResult = GSM7ToAscii(Right(lasUDHBin(lnCtr), 7 - lnCtr) & Mid(lasUDHBin(lnCtr + 1), 1, lnCtr))
            EncodeUDHAscii = EncodeUDHAscii & lsResult
        Next
        lsResult = GSM7ToAscii(Mid(lasUDHBin(lnCtr - 1), 1, lnCtr - 1))
        EncodeUDHAscii = EncodeUDHAscii & lsResult
    End Function

    Private Function Dec2Bin7(ByVal Value As Integer) As String
        Dec2Bin7 = Mid(Dec2Bin(Value), 2)
    End Function

    Private Function Dec2Bin(ByVal Value As Integer) As String
        Dim lnExponent As Integer
        Dim lsResult As New String("0", 8)

        ' this is faster than creating the string by appending chars
        'lsResult = String$(8, "0")

        Do
            If Value And Power2(lnExponent) Then
                ' we found a bit that is set, clear it
                Mid$(lsResult, 8 - lnExponent, 1) = "1"
                Value = Value Xor Power2(lnExponent)
            End If
            lnExponent = lnExponent + 1
        Loop While Value
        Dec2Bin = lsResult
    End Function

    Private Function GSM7ToAscii(ByVal lsBinary7 As String) As String
        Dim lnGSM7Dec As Long

        lnGSM7Dec = BinToDec(lsBinary7)
        Select Case lnGSM7Dec
            Case 0 : GSM7ToAscii = "@"
            Case 1 : GSM7ToAscii = "£"
            Case 2 : GSM7ToAscii = "$"
            Case 3 : GSM7ToAscii = "¥"
            Case 4 : GSM7ToAscii = "è"
            Case 5 : GSM7ToAscii = "é"
            Case 6 : GSM7ToAscii = "ù"
            Case 7 : GSM7ToAscii = "ì"
            Case 8 : GSM7ToAscii = "ò"
            Case 9 : GSM7ToAscii = "Ç"
                '   Case 10: GSM7ToAscii = "\n"
            Case 11 : GSM7ToAscii = "Ø"
            Case 12 : GSM7ToAscii = "ø"
                '   Case 13: GSM7ToAscii = "\r"
            Case 14 : GSM7ToAscii = "Å"
            Case 15 : GSM7ToAscii = "å"
                '   Case 16: GSM7ToAscii = "\u0394"
            Case 17 : GSM7ToAscii = "_"
                '   Case 18: GSM7ToAscii = "\u03a6"
                '   Case 19: GSM7ToAscii = "\u0393"
                '   Case 20: GSM7ToAscii = "\u039b"
                '   Case 21: GSM7ToAscii = "\u03a9"
                '   Case 22: GSM7ToAscii = "\u03a0"
                '   Case 23: GSM7ToAscii = "\u03a8"
                '   Case 24: GSM7ToAscii = "\u03a3"
                '   Case 25: GSM7ToAscii = "\u0398"
                '   Case 26: GSM7ToAscii = "\u039e"
                '   Case 27: GSM7ToAscii = "&#8364;"
            Case 28 : GSM7ToAscii = "Æ"
            Case 29 : GSM7ToAscii = "æ"
            Case 30 : GSM7ToAscii = "ß"
            Case 31 : GSM7ToAscii = "É"
            Case 36 : GSM7ToAscii = "¤"
            Case 64 : GSM7ToAscii = "¡"
            Case 91 : GSM7ToAscii = "Ä"
            Case 92 : GSM7ToAscii = "Ö"
            Case 93 : GSM7ToAscii = "Ñ"
            Case 94 : GSM7ToAscii = "Ü"
            Case 95 : GSM7ToAscii = "§"
            Case 96 : GSM7ToAscii = "¿"
            Case 123 : GSM7ToAscii = "ä"
            Case 124 : GSM7ToAscii = "ö"
            Case 125 : GSM7ToAscii = "ñ"
            Case 126 : GSM7ToAscii = "ü"
            Case 127 : GSM7ToAscii = "à"
            Case Else : GSM7ToAscii = Chr(lnGSM7Dec)
        End Select
    End Function

    Private Function BinToDec(ByVal Value As String) As Long
        Dim lsResult As Long, lnCtr As Integer, lnExponent As Integer

        For lnCtr = Len(Value) To 1 Step -1
            Select Case Asc(Mid$(Value, lnCtr, 1))
                Case 48      ' "0", do nothing
                Case 49      ' "1", add the corresponding power of 2
                    lsResult = lsResult Or Power2(lnExponent)
                Case Else
                    Err.Raise(5)      ' Invalid procedure call or argument
            End Select
            lnExponent = lnExponent + 1
        Next

        BinToDec = lsResult
    End Function

    Private Function Power2(ByVal exponent As Long) As Long
        Static lnRes(0 To 31) As Long
        Dim lnCtr As Long

        ' rule out errors
        If exponent < 0 Or exponent > 31 Then Err.Raise(5)

        ' initialize the array at the first call
        If lnRes(0) = 0 Then
            lnRes(0) = 1
            For lnCtr = 1 To 30
                lnRes(lnCtr) = lnRes(lnCtr - 1) * 2
            Next
            ' this is a special case
            lnRes(31) = &H80000000
        End If

        ' return the result
        Power2 = lnRes(exponent)
    End Function

    Public Function SendPDUSMS(ByVal destinationNumber As String, _
                                 ByVal textMessage As String) As Boolean
        Try
            Dim responses() As String
            Dim response As String

            response = serialDriver.SendCmd(ATHandler.CMGF_COMMAND & "=0", ATHandler.RESPONSE_OK)
            responses = ParseATResponse(response)
            If UBound(responses) = 0 Then
                Return False
                Exit Function
            End If

            If responses(1).Trim() = ATHandler.RESPONSE_OK Then
                Dim pduObject As New PDU
                Dim PDUCodes() As String = pduObject.GetPDU( _
                                        "", destinationNumber, ENUM_TP_DCS.DefaultAlphabet, _
                                        Me.Validity, 1, Me.isDeliveryReport, _
                                        textMessage)
                Dim i, ATLength As Integer

                For i = 0 To PDUCodes.Length - 1
                    'Debug.Print(PDUCodes(i))
                    ATLength = pduObject.GetATLength(PDUCodes(i))
                    'Debug.Print("SEND LENGTH")
                    response = serialDriver.SendCmd(ATHandler.CMGS_COMMAND & "=" & ATLength & "", ">")
                    'Debug.Print("PDU")
                    responses = ParseATResponse(serialDriver.SendCmd(PDUCodes(i) & Chr(26), ATHandler.CMGS_RESPONSE))

                    If responses.Length > 1 Then
                        'Debug.Print("response")
                        If Not responses(1) Is Nothing Then
                            Dim line As String = responses(1)
                            Dim cols() As String = line.Split(":")
                            If (cols.Length > 1) Then
                                If Not IsNumeric(cols(1).Trim) Then
                                    Return False
                                End If

                                Dim index As Integer = cols(1).Trim

                                If isAutoDeleteSentMessage Then
                                    Try
                                        DeleteSMS(index)
                                    Catch ex As Exception
                                        isAutoDeleteSentMessage = False
                                    End Try
                                End If

                                Return True
                            End If
                        End If
                        If i = PDUCodes.Length - 1 Then Return True
                    Else
                        'Debug.Print("wala response")
                        Return False
                        Exit For
                    End If
                Next
            End If
        Catch ex As System.Exception
            'Throw New InvalidOperationException("Error sending SMS message: " + ex.Message, ex)
            Return False
        End Try
    End Function

    Private Function GetMSISDN() As String
        If Not commandHandler.Is_AT_CNUM_Supported Then
            Return ATHandler.RESPONSE_NOT_SUPPORTED
        End If
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CNUM_COMMAND, ATHandler.RESPONSE_OK))
            Dim retCode As String = response(0).Trim
            If retCode = ATHandler.RESPONSE_ERROR Then
                GetMSISDN = ATHandler.RESPONSE_NOT_SUPPORTED
            Else
                response = retCode.Split(":")
                If (response.Length > 1) Then
                    retCode = response(1)
                    response = retCode.Split(",")
                    If (response.Length > 1) Then
                        Return response(1).Trim
                    Else
                        commandHandler.Is_AT_CNUM_Supported = False
                        Return String.Empty
                    End If
                Else
                    commandHandler.Is_AT_CNUM_Supported = False
                    Return String.Empty
                End If
            End If
        Catch ex As System.Exception
            Throw New InvalidOpException("Cannot retrieve MSISDN", ex)
        End Try
    End Function

    Private Function GetPhoneModel() As String
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.GMM_COMMAND))
            Return response(0)
        Catch ex As System.Exception
            Throw New GeneralException(ex.Message, ex)
        End Try
    End Function

    Private Shared Function GenerateReferenceNo() As Integer
        Dim referenceNo As Integer = iRefNo
        iRefNo = iRefNo + 1
        Return referenceNo
        'Dim r As New Random()
        ''Dim msgRefNo As Integer
        'Return r.Next(60001, 100000)
    End Function

    Private Sub SetMemoryLocation()
        Try
            Dim memoryLocation As String
            If enumMessageMemory = Common.EnumMessageMemory.PHONE Then
                memoryLocation = "ME"
            Else
                memoryLocation = "SM"
            End If
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CPMS_COMMAND & "=""" & memoryLocation & """"))
        Catch ex As System.Exception
            Throw New GeneralException(ex.Message, ex)
        End Try
    End Sub

#End Region

#Region "Friend Functions"

    Friend Function GetSMSMessages(ByVal messageType As MessageStore.EnumMessageType) As SMSMessage()
        Try
            Dim commandString As String = String.Empty

            If messageType = MessageStore.EnumMessageType.ReceivedUnreadMessages Then
                commandString = ATHandler.CMGL_COMMAND & "=0"
                'commandString = ATHandler.CMGL_COMMAND & "=" & Chr(34) & "REC UNREAD" & Chr(34)
            ElseIf messageType = MessageStore.EnumMessageType.ReceivedReadMessages Then
                commandString = ATHandler.CMGL_COMMAND & "=1"
                'commandString = ATHandler.CMGL_COMMAND & "=" & Chr(34) & "REC READ" & Chr(34)
            ElseIf messageType = MessageStore.EnumMessageType.StoredUnsentMessages Then
                commandString = ATHandler.CMGL_COMMAND & "=2"
                'commandString = ATHandler.CMGL_COMMAND & "=" & Chr(34) & "STO UNSENT" & Chr(34)
            ElseIf messageType = MessageStore.EnumMessageType.StoredSentMessages Then
                commandString = ATHandler.CMGL_COMMAND & "=3"
                'commandString = ATHandler.CMGL_COMMAND & "=" & Chr(34) & "STO SENT" & Chr(34)
            ElseIf messageType = MessageStore.EnumMessageType.AllMessages Then
                commandString = ATHandler.CMGL_COMMAND & "=4"
                'commandString = ATHandler.CMGL_COMMAND & "=" & Chr(34) & "ALL" & Chr(34)
            End If

            Dim response() As String = ParseATResponse(serialDriver.SendCmd(commandString, ATHandler.RESPONSE_OK))
            If Not response Is Nothing Then
                If response.Length > 1 Then
                    Dim i As Integer
                    Dim line As String
                    Dim cols() As String
                    Dim msgContent As String
                    Dim smsList() As SMSMessage
                    Dim iCount As Integer
                    Dim iMsgIndex As Integer

                    smsList = Nothing
                    For i = 0 To response.Length - 1
                        If response(i) Is Nothing Then
                            Continue For
                        End If
                        line = response(i)
                        If (Not line Is Nothing And line.IndexOf(ATHandler.CMGL_RESPONSE & ":") >= 0) Then
                            cols = line.Split(":")
                            If cols.Length > 1 Then
                                cols = cols(1).Split(",")
                                If cols.Length > 1 Then
                                    iMsgIndex = Convert.ToInt32(cols(0).Trim)
                                End If
                            End If

                            msgContent = response(i + 1)
                            Dim e As NewMessageReceivedEventArgs = DecodeHexPDUReceived(msgContent)
                            ReDim Preserve smsList(iCount)
                            iCount = iCount + 1
                            Dim sms As New SMSMessage
                            sms.SMSC = e.SMSC
                            sms.PhoneNumber = e.MSISDN
                            sms.Timestamp = e.Timestamp
                            sms.TimestampRFC = e.TimestampRFC
                            sms.TimeZone = e.TimeZone
                            sms.Text = e.TextMessage
                            sms.Index = iMsgIndex
                            sms.Data = e.ReferenceNo
                            sms.EMSTotolPiece = e.TotalParts
                            sms.EMSCurrentPiece = e.SeqNo
                            smsList(iCount - 1) = sms
                        End If
                    Next
                    Return smsList
                End If
            End If
        Catch ex As System.Exception
            'jheff sms
            'Throw New InvalidOpException("Unable to retrieve SMS ", ex)
        End Try
        Return Nothing
    End Function

#End Region

#Region "Public Function"

    Public Function DeleteSMS(ByVal index As Integer, Optional ByVal type As EnumSMSDeleteOption = EnumSMSDeleteOption.Index) As Boolean
        Try
            Dim response() As String
            If type = EnumSMSDeleteOption.Index Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGD_COMMAND & "=" & index, ATHandler.RESPONSE_OK))
            ElseIf type = EnumSMSDeleteOption.Preferred_Message_Store Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGD_COMMAND & "=1,1", ATHandler.RESPONSE_OK))
            ElseIf type = EnumSMSDeleteOption.Preferered_Message_Store_And_MO_Sent Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGD_COMMAND & "=1,2", ATHandler.RESPONSE_OK))
            ElseIf type = EnumSMSDeleteOption.Preferered_Message_Store_And_MO_Sent_Unsent Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGD_COMMAND & "=1,3", ATHandler.RESPONSE_OK))
            ElseIf type = EnumSMSDeleteOption.All_Messages Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGD_COMMAND & "=1,4", ATHandler.RESPONSE_OK))
            End If
        Catch ex As System.Exception
            Throw New InvalidOpException("Unable to delete SMS ", ex)
        End Try
    End Function

    Public Function GetStorageSupported() As String()
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CPMS_COMMAND & "=?"))
            response = response(0).Split(":")
            If (response.Length > 1) Then
                Dim stringSeparators() As String = {"),("}
                response = response(1).Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
                Dim i As Integer
                For i = 0 To response.Length - 1
                    response(i) = response(i).Replace("""", "")
                    response(i) = response(i).Replace("(", "")
                    response(i) = response(i).Replace(")", "")
                    response(i) = response(i).Replace(",", "")
                Next
                Return response
            End If
            GetStorageSupported = Nothing
        Catch ex As System.Exception
            Throw New InvalidOperationException("Unable to query supported storage", ex)
        End Try
    End Function

    Public Function GetStorageSetting() As Storage()
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CPMS_COMMAND & "?"))
            response = response(0).Split(":")
            If (response.Length > 1) Then
                response = response(1).Split(",")
                Dim storage((response.Length / 3) - 1) As Storage
                Dim i As Integer
                For i = 0 To storage.Length - 1
                    storage(i) = New Storage
                    storage(i).Name = response(i * 3).Replace("""", "").Trim()
                    storage(i).Used = Convert.ToInt32(response((i * 3) + 1))
                    storage(i).Total = Convert.ToInt32(response((i * 3) + 2))
                Next
                Return storage
            End If
            GetStorageSetting = Nothing
        Catch ex As System.Exception
            Throw New InvalidOperationException("Unable to query storage settings", ex)
        End Try
    End Function

    Public Function EnablePDUMode() As Boolean
        Try
            Dim responses() As String
            Dim response As String
            response = serialDriver.SendCmd(ATHandler.CMGF_COMMAND & "=0", ATHandler.RESPONSE_OK)
            responses = ParseATResponse(response)
            If responses(0).Trim() = ATHandler.RESPONSE_OK Then
                commandHandler.Is_AT_CMGF_0_Supported = True
                Return True
            End If

            commandHandler.Is_AT_CMGF_0_Supported = False
            Return False
        Catch ex As System.Exception
            Throw New InvalidOpException("Cannot enable PDU MODE", ex)
        End Try
    End Function

    Public Function EnableCLIP() As Boolean
        If Not commandHandler.Is_AT_CLIP_Supported Then
            Return False
        End If
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CLIP_COMMAND & "=1"))
            Dim retVal As String = response(0)
            If Trim(retVal) = "" Then Return False

            If retVal.Equals(ATHandler.RESPONSE_OK) Then
                Return True
                'response = ParseATResponse(serialDriver.SendCmd(ATHandler.CLIP_COMMAND & "?", ATHandler.CLIP_RESPONSE & ":"))
                'retVal = response(0)
                'Dim cols() As String = retVal.Split(":")
                'If cols.Length > 1 Then
                '    cols = cols(1).Split(",")
                '    If cols(0).Trim = "1" Then
                '        Return True
                '    End If
                'End If
            End If
            Return False
        Catch ex As System.Exception
            'Throw New InvalidOpException("Cannot enable CLIP", ex)
            Return False
        End Try
    End Function

    Public Function DisableCLIP() As Boolean
        If Not commandHandler.Is_AT_CLIP_Supported Then
            Return False
        End If

        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CLIP_COMMAND & "=0"))
            Dim retVal As String = response(0)
            If retVal.Equals(ATHandler.RESPONSE_OK) Then
                Return True
                'response = ParseATResponse(serialDriver.SendCmd(ATHandler.CLIP_COMMAND & "?", ATHandler.CLIP_RESPONSE & ":"))
                'retVal = response(0)
                'Dim cols() As String = retVal.Split(":")
                'If cols.Length > 1 Then
                '    cols = cols(1).Split(",")
                '    If cols(0).Trim = "0" Then
                '        Return True
                '    End If
                'End If
            End If
            Return False
        Catch ex As System.Exception
            Throw New InvalidOpException("Cannot disable CLIP", ex)
        End Try
    End Function

    Public Function EnableCLIR() As Boolean
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CLIR_COMMAND & "=1"))
            Dim retVal As String = response(0)
            If retVal.Equals(ATHandler.RESPONSE_OK) Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CLIR_COMMAND & "?", ATHandler.CLIR_RESPONSE & ":"))
                retVal = response(0)
                Dim cols() As String = retVal.Split(":")
                If cols.Length > 1 Then
                    cols = cols(1).Split(",")
                    If cols(0).Trim = "1" Then
                        Return True
                    End If
                End If
            End If
            EnableCLIR = False
        Catch ex As System.Exception
            Throw New InvalidOpException("Cannot enable CLIR", ex)
        End Try
    End Function

    Public Function DisableCLIR() As Boolean
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CLIR_COMMAND & "=0"))
            Dim retVal As String = response(0)
            If retVal.Equals(ATHandler.RESPONSE_OK) Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CLIR_COMMAND & "?", ATHandler.CLIR_RESPONSE & ":"))
                retVal = response(0)
                Dim cols() As String = retVal.Split(":")
                If cols.Length > 1 Then
                    cols = cols(1).Split(",")
                    If cols(0).Trim = "0" Then
                        Return True
                    End If
                End If
            End If
            DisableCLIR = False
        Catch ex As System.Exception
            Throw New InvalidOpException("Cannot disable CLIR", ex)
        End Try
    End Function

    Public Function SendUSSD(ByVal commandStr As String) As String
        Return String.Empty
    End Function

    Public Function GetLocation() As Location
        Try
            Dim responses() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.COPS_COMMAND & "?", ATHandler.COPS_RESPONSE))
            Dim retCode As String = responses(0)
            Dim loc As New Location
            Dim parts() As String = retCode.Split(":")
            If parts.Length > 1 Then
                parts = parts(1).Split(",")
                If parts.Length >= 3 Then
                    Dim networkInfo As String = parts(2).Replace("""", "")
                    If networkInfo.Length >= 5 Then
                        loc.MCC = networkInfo.Substring(0, 3)
                        loc.MNC = networkInfo.Substring(3, 2)
                        responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CREG_COMMAND & "=2"))
                        If responses(0) = ATHandler.RESPONSE_OK Then
                            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CREG_COMMAND & "?", ATHandler.CREG_RESPONSE))
                            responses = responses(0).Split(":")
                            If responses.Length > 1 Then
                                responses = responses(1).Split(",")
                                If responses.Length >= 4 Then
                                    Dim tmp1 As String = responses(2).Replace("""", "")
                                    Dim tmp2 As String = responses(3).Replace("""", "")
                                    loc.LAI = Convert.ToInt32(tmp1, 16)
                                    loc.CellID = Convert.ToInt32(tmp2, 16)
                                End If
                            End If

                        End If
                    End If
                    Return loc
                End If
            End If
            GetLocation = Nothing
        Catch ex As System.Exception
            Throw New InvalidOpException("Cannot query location", ex)
        End Try
    End Function

    Public Function GetBatteryLevel() As Battery
        If Not commandHandler.Is_AT_CBC_Supported Then
            Throw New InvalidOpException("Unable to query battery level")
        End If

        Try
            Dim responses() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CBC_COMMAND & "=?", ATHandler.CBC_RESPONSE))
            responses = responses(0).Split(":")
            If responses.Length > 1 Then
                Dim battery As New Battery
                Dim stringSeparators() As String = {"),("}
                Dim parts() As String = responses(1).Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
                If parts.Length > 1 Then
                    parts = parts(1).Split("-")
                    If parts.Length = 2 Then
                        battery.MinimumLevel = Convert.ToInt32(parts(0), 10)
                        battery.MaximumLevel = Convert.ToInt32(parts(1).Replace(")", ""), 10)

                        ' Then check current level
                        responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CBC_COMMAND, ATHandler.CBC_RESPONSE))
                        responses = responses(0).Split(":")
                        If responses.Length > 1 Then
                            responses = responses(1).Split(",")
                            If responses.Length > 1 Then
                                battery.BatteryLevel = Convert.ToInt32(responses(1).Trim, 10)
                                If responses(0).Trim = "0" Then
                                    battery.BatteryCharged = False
                                Else
                                    battery.BatteryCharged = True
                                End If
                            End If
                        End If
                    End If
                    Return battery
                End If
            End If
            GetBatteryLevel = Nothing
        Catch ex As System.Exception
            Throw New InvalidOpException("Unable to query battery level", ex)
        End Try
    End Function

    Public Function GetRssi() As Rssi
        Try
            Dim responses() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CREG_COMMAND & "=1"))
            If responses(0) <> ATHandler.RESPONSE_ERROR Then
                commandHandler.Is_AT_CREG_Supported = True
            Else
                commandHandler.Is_AT_CREG_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CREG_Supported = True
        End Try

        If Not commandHandler.Is_AT_CSQ_Supported Then
            'Throw New InvalidOpException("Unable to query RSSI")
        End If

        Try
            Dim responses() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.CSQ_COMMAND & "=?", ATHandler.CSQ_RESPONSE))
            If Trim(responses(0)) = "" Then
                Dim rssi As New Rssi

                rssi.Minimum = 0
                rssi.Maximum = 0
                rssi.Current = 0
                Return rssi
            End If

            Dim lsResponse As String

            If UBound(responses) > 0 Then
                If Trim(responses(1)) = "OK" Then
                    lsResponse = responses(0)
                Else
                    lsResponse = responses(1)
                End If
            Else
                Dim rssi As New Rssi

                rssi.Minimum = 0
                rssi.Maximum = 0
                rssi.Current = 0
                Return rssi
            End If

            responses = lsResponse.Split(":")
            If responses.Length > 1 Then
                Dim rssi As New Rssi
                Dim stringSeparators() As String = {","}
                Dim parts() As String = responses(1).Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
                If parts.Length > 1 Then
                    parts = parts(0).Split("-")
                    If parts.Length = 2 Then
                        rssi.Minimum = Convert.ToInt32(parts(0).Replace("(", "").Trim, 10)
                        rssi.Maximum = Convert.ToInt32(parts(1).Trim, 10)

                        ' Then check current level
                        responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CSQ_COMMAND, ATHandler.CSQ_RESPONSE))
                        If UBound(responses) > 0 Then
                            responses = responses(1).Split(":")
                            If responses.Length > 1 Then
                                responses = responses(1).Split(",")
                                If responses.Length > 1 Then
                                    If IsNumeric(responses(0).Trim) Then
                                        rssi.Current = Convert.ToInt32(responses(0).Trim, 10)
                                    Else
                                        rssi.Current = 0
                                    End If
                                End If
                            End If
                        Else
                            rssi.Current = 0
                        End If
                    End If
                    Return rssi
                End If
            End If
            Return Nothing
        Catch ex As System.Exception
            'Throw New InvalidOpException("Unable to query RSSI", ex)
        End Try
    End Function

    ''' <summary>
    ''' Send SMS
    ''' </summary>
    ''' <param name="destinationNumber"></param>
    ''' <param name="textMessage"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendSMS(ByVal destinationNumber As String, _
                    ByVal textMessage As String _
                    ) As String
        Dim msgId As String = String.Empty
        If Me.Encoding = Common.EnumEncoding.GSM_Default_7Bit Then
            msgId = SendTextSMS(destinationNumber, textMessage)
            'msgId = SendUnicodeSMS(destinationNumber, textMessage)
        ElseIf Me.Encoding = Common.EnumEncoding.Class2_7_Bit Then
            msgId = SendClass27BitSMS(destinationNumber, textMessage)
        ElseIf Me.Encoding = Common.EnumEncoding.Class2_8_Bit Then
            msgId = SendClass28BitSMS(destinationNumber, textMessage)
        ElseIf Me.Encoding = Common.EnumEncoding.Hex_Message Then
            msgId = SendHexSMS(destinationNumber, textMessage)
        ElseIf Me.Encoding = Common.EnumEncoding.Unicode_16Bit Then
            msgId = SendUnicodeSMS(destinationNumber, textMessage)
        Else
            msgId = SendTextSMS(destinationNumber, textMessage)
        End If

        If AutoDeleteSentMessage Then
            Try
                PurgeMessageStore(EnumSMSDeleteOption.Preferered_Message_Store_And_MO_Sent_Unsent)
            Catch ex As Exception
                AutoDeleteSentMessage = False
            End Try
        End If

        Return msgId
    End Function

    ''' <summary>
    ''' Send SMS
    ''' </summary>
    ''' <param name="destinationNumber"></param>
    ''' <param name="textMessage"></param>
    ''' <param name="encoding"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendSMS(ByVal destinationNumber As String, _
                   ByVal textMessage As String, _
                   ByVal encoding As EnumEncoding) As String
        Dim msgId As String = String.Empty
        If encoding = Common.EnumEncoding.GSM_Default_7Bit Then
            If textMessage.Length > 160 Then
                msgId = SendLongMessageConcat(destinationNumber, textMessage)
            Else
                msgId = SendTextSMS(destinationNumber, textMessage)
            End If
        ElseIf encoding = Common.EnumEncoding.Class2_7_Bit Then
            msgId = SendClass27BitSMS(destinationNumber, textMessage)
        ElseIf encoding = Common.EnumEncoding.Class2_8_Bit Then
            msgId = SendClass28BitSMS(destinationNumber, textMessage)
        ElseIf encoding = Common.EnumEncoding.Hex_Message Then
            msgId = SendHexSMS(destinationNumber, textMessage)
        ElseIf Me.Encoding = Common.EnumEncoding.Unicode_16Bit Then
            msgId = SendUnicodeSMS(destinationNumber, textMessage)
        Else
            msgId = SendTextSMS(destinationNumber, textMessage)
        End If

        If AutoDeleteSentMessage Then
            Try
                PurgeMessageStore(EnumSMSDeleteOption.Preferered_Message_Store_And_MO_Sent_Unsent)
            Catch ex As Exception
                AutoDeleteSentMessage = False
            End Try
        End If

        Return msgId
    End Function

    Public Function PurgeMessageStore(Optional ByVal type As EnumSMSDeleteOption = EnumSMSDeleteOption.Preferred_Message_Store) As Boolean
        Try
            Dim response() As String
            If type = EnumSMSDeleteOption.Preferred_Message_Store Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGD_COMMAND & "=1,1", ATHandler.RESPONSE_OK))
            ElseIf type = EnumSMSDeleteOption.Preferered_Message_Store_And_MO_Sent Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGD_COMMAND & "=1,2", ATHandler.RESPONSE_OK))
            ElseIf type = EnumSMSDeleteOption.Preferered_Message_Store_And_MO_Sent_Unsent Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGD_COMMAND & "=1,3", ATHandler.RESPONSE_OK))
            ElseIf type = EnumSMSDeleteOption.All_Messages Then
                response = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGD_COMMAND & "=1,4", ATHandler.RESPONSE_OK))
            End If
        Catch ex As System.Exception
            Throw New InvalidOpException("Unable to purge message store ", ex)
        End Try
    End Function

    Public ReadOnly Property MessageStore() As MessageStore
        Get
            Return oMessageStore
        End Get
    End Property

    Public Function Dial(ByVal phoneNo As String) As Boolean
        p_bCancelFlag = False
        Dim ldTimexxx As Date = Now
        Dim lnCtr As Integer = 0

        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.ATD_COMMAND & phoneNo + ";"))
            Dim results As String
            If UBound(response) > 0 Then
                results = response(1)
            Else
                results = response(0)
            End If

            Dim lsStatus() As String
            Dim lbDialling As Boolean = False
            Dim lbRinging As Boolean = False

            If (results = ATHandler.RESPONSE_OK) Then
                Do Until 1 = 0
                    Dim lsResponse As String = serialDriver.SendCmd("AT+CLCC", ATHandler.RESPONSE_OK)

                    Debug.Print("RESPONSE: " & lsResponse)
                    If InStr(lsResponse, "NO CARRIER") > 0 Then
                        Debug.Print("no carrier")
                        RaiseEvent CallStatus(xeCallStatus.xeCallStatusNoCarrier)
                        Exit Do
                    ElseIf InStr(lsResponse, "NO DIALTONE") > 0 Then
                        RaiseEvent CallStatus(xeCallStatus.xeCallStatusNoDialTone)
                        Debug.Print("no dialtone")

                        'If Not resetDevice() Then
                        '    MsgBox("Please try to restart your computer" & vbCrLf & _
                        '              "and turn off all affected device and turn it on again before executing the system...", vbCritical, "WARNING")
                        'End If

                        'RaiseEvent CallStatus(xeCallStatusNoDialTone, p_nSubscriber)
                        'Exit Function
                        Exit Do
                    ElseIf InStr(lsResponse, "BUSY") > 0 Then
                        RaiseEvent CallStatus(xeCallStatus.xeCallStatusBusy)
                        Debug.Print("busy")
                        Exit Do
                    ElseIf InStr(lsResponse, "NO ANSWER") > 0 Then
                        RaiseEvent CallStatus(xeCallStatus.xeCallStatusNoAnswer)
                        Debug.Print("no answer")
                        Exit Do
                    ElseIf InStr(lsResponse, "ERROR") > 0 Then
                        RaiseEvent CallStatus(xeCallStatus.xeCallStatusError)
                        Debug.Print("error response when dialling")
                        Exit Do
                    ElseIf InStr(lsResponse, "NO NETWORK") > 0 Then
                        RaiseEvent CallStatus(xeCallStatus.xeCallStatusNoNetwork)
                        Debug.Print("no network")
                        Exit Do
                    ElseIf InStr(lsResponse, "+CLCC: ") > 0 Then
                        lsResponse = Mid(lsResponse, InStr(lsResponse, "+CLCC"))
                        lsStatus = lsResponse.Split(",")

                        If UBound(lsStatus) = "7" Then
                            Select Case lsStatus(2)
                                Case "0"
                                    RaiseEvent CallStatus(xeCallStatus.xeCallStatusConnected)
                                    Debug.Print("connected")
                                    p_bCallConnected = True
                                    Exit Do
                                Case "2"
                                    'If Not lbDialling Then
                                    RaiseEvent CallStatus(xeCallStatus.xeCallStatusDialling)
                                    Debug.Print("dialling")
                                    lbDialling = True
                                    'End If
                                Case "3"
                                    'If Not lbRinging Then
                                    RaiseEvent CallStatus(xeCallStatus.xeCallStatusRinging)
                                    Debug.Print("ringing")
                                    lbRinging = True
                                    'Exit Do
                                    'End If
                                Case Else
                                    RaiseEvent CallStatus(xeCallStatus.xeCallStatusNone)
                                    Debug.Print("no status")
                                    Exit Do
                            End Select
                        End If

                        If InStr(lsResponse, "ERROR") > 0 Then
                            RaiseEvent CallStatus(xeCallStatus.xeCallStatusError)
                            Debug.Print("error response when connected, after dialling")
                            Exit Do
                        End If
                    Else
                        Debug.Print("STATUS: " & "NONE")
                        RaiseEvent CallStatus(xeCallStatus.xeCallStatusNone)
                    End If

                    If p_bCancelFlag Then
                        Debug.Print("cancel flag")
                        RaiseEvent CallStatus(xeCallStatus.xeCallStatusEndCall)
                        p_bCancelFlag = False
                        Exit Do
                    End If

                    If Trim(lsResponse) = "" Then
                        Debug.Print("empty" & "»" & ldTimexxx & "»" & Now)
                        If DateDiff("s", ldTimexxx, Now) >= 10 Then
                            RaiseEvent CallStatus(xeCallStatus.xeCallStatusError)
                            Debug.Print("error: empty response while dialing")
                            Exit Do
                        End If
                    Else
                        Debug.Print("Response: " & lsResponse & Trim(lsResponse).IndexOf("AT+CLCC"))
                        If Trim(lsResponse).Contains("AT+CLCC") Then
                            Debug.Print(DateDiff("s", ldTimexxx, Now))
                            If DateDiff("s", ldTimexxx, Now) >= 10 Then
                                Debug.Print("error: at+clcc response")
                                RaiseEvent CallStatus(xeCallStatus.xeCallStatusError)
                                Exit Do
                            End If
                        Else
                            ldTimexxx = Now
                        End If
                    End If
                    Thread.Sleep(500)
                Loop

                If p_bCancelFlag Then
                    Call CHUPHangUp()
                    p_bCancelFlag = False
                End If

                Return True
            End If
            Dial = False
        Catch ex As System.Exception
            'Throw New InvalidOpException("Unable to dial", ex)
        End Try
    End Function

    Public Function Answer() As Boolean
        p_bCancelFlag = True
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.ATA_COMMAND))
            Dim results As String = response(0)
            If (results = ATHandler.RESPONSE_OK) Then
                Dim args As New NewIncomingCallEventArgs
                args.CallerID = strMSISDN
                args.Ringing = False
                args.Answered = True
                args.Status = "CONNECTED"
                RaiseEvent NewIncomingCall(args)

                bIncomingAnswered = True
                Return True
            End If
            Return False
        Catch ex As System.Exception
            Throw New InvalidOpException("Unable to answer", ex)
        End Try
    End Function

    Public Function CHUPHangUp() As Boolean
        p_bCancelFlag = True
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(commandHandler.CHUPHangUpCommand))
            Dim results As String = response(0)
            If (results = ATHandler.RESPONSE_OK) Then
                Return True
            End If
            Return False
        Catch ex As System.Exception
            Throw New InvalidOpException("Unable to hang up", ex)
        End Try
    End Function

    Public Function HangUp() As Boolean
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(commandHandler.HangUpCommand))
            Dim results As String = response(0)
            If (results = ATHandler.RESPONSE_OK) Then
                Return True
            End If
            Return False
        Catch ex As System.Exception
            Throw New InvalidOpException("Unable to hang up", ex)
        End Try
    End Function

    Public Function SendDtmf(ByVal dtmfDigits As String) As Boolean
        Try
            If Len(Trim(dtmfDigits)) = 0 Then
                Return False
            End If
            Dim parts() As String = dtmfDigits.Split(",")
            Dim i As Integer
            Dim responses() As String
            Dim retCode As String
            For i = 0 To parts.Length - 1
                Dim ranges() As String = parts(i).Split("-")
                If (ranges.Length > 1) Then
                    Dim startRange As String = ranges(0).Trim
                    Dim endRange As String = ranges(1).Trim
                    responses = ParseATResponse(serialDriver.SendCmd(ATHandler.VTS_COMMAND & "=" + startRange))
                    retCode = responses(0)
                    If (retCode <> ATHandler.RESPONSE_OK) Then
                        Return False
                    End If
                    Do
                        startRange = Chr(Asc(startRange) + 1)
                        responses = ParseATResponse(serialDriver.SendCmd(ATHandler.VTS_COMMAND & "=" + startRange))
                        retCode = responses(0)
                        If (retCode <> ATHandler.RESPONSE_OK) Then
                            Return False
                        End If
                    Loop While startRange <> endRange
                Else
                    responses = ParseATResponse(serialDriver.SendCmd(ATHandler.VTS_COMMAND & "=" + ranges(0).Trim))
                    retCode = responses(0)
                    If (retCode <> ATHandler.RESPONSE_OK) Then
                        Return False
                    End If
                End If
            Next
            Return True
        Catch ex As System.Exception
            Throw New InvalidOpException("Unable to send DTMF", ex)
        End Try
    End Function

    Public Function InitMsgIndication() As Integer
        If Not commandHandler.Is_AT_CNMI_Supported Then
            Throw New InvalidOpException("Unable to set message indication")
        End If
        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.MESSAGE_INDICATION_COMMAND & "=?"))
            If response.Length > 1 Then
                If response(1).Trim = ATHandler.RESPONSE_OK Then
                    response = ParseATResponse(serialDriver.SendCmd(ATHandler.MESSAGE_INDICATION_COMMAND & "=" & commandHandler.MsgIndication))
                    Dim results As String = response(0)
                    If (results = ATHandler.RESPONSE_OK) Then
                        Return 0
                    End If
                    If results.IndexOf(ATHandler.RESPONSE_ERROR) >= 0 Then
                        Dim cols() As String = results.Split(":")
                        If cols.Length > 1 Then
                            Return Convert.ToInt16(cols(1).Trim())
                        End If
                    End If
                End If
            End If
        Catch ex As System.Exception
            Throw New InvalidOpException("Unable to set message indication", ex)
        End Try
    End Function

    Public Function GetMsgIndication() As String

        Try
            Dim response() As String = ParseATResponse(serialDriver.SendCmd(ATHandler.MESSAGE_INDICATION_COMMAND & "=?"))
            If response.Length > 1 Then
                If response(1).Trim = ATHandler.RESPONSE_OK Then
                    response = ParseATResponse(serialDriver.SendCmd(ATHandler.MESSAGE_INDICATION_COMMAND & "?"))
                    response = response(0).Split(":")
                    If (response.Length > 1) Then
                        Return response(1).Trim()
                    End If
                End If
            End If
            GetMsgIndication = String.Empty
        Catch ex As System.Exception
            Throw New InvalidOpException("Unable to query message indication", ex)
        End Try
    End Function

    Public Function DecodeHexPDU(ByVal PDU As String) As NewMessageReceivedEventArgs
        'Dim s As Object = Nothing
        Dim e As New NewMessageReceivedEventArgs
        Dim PDUCode As String = PDU.Replace(vbCrLf, "")
        If PDUCode.StartsWith("00") Then
            PDUCode = "02810011" & Mid(PDUCode, 5, PDUCode.Length - 2)
        End If

        Dim baseInfo As PDUDecoder.BaseInfo = PDUDecoder.Decode(PDUCode)
        e.MSISDN = baseInfo.SourceNumber
        e.Timestamp = baseInfo.ReceivedDate
        e.TextMessage = baseInfo.Text
        e.TotalParts = baseInfo.EMSTotolPiece
        e.SeqNo = baseInfo.EMSCurrentPiece
        e.ReferenceNo = GenerateReferenceNo()
        e.TimeZone = baseInfo.TimeZone

        'Dim T As SMSBase.SMSType = SMSBase.GetSMSType(PDUCode)
        'Select Case T
        '    Case SMSBase.SMSType.EMS_RECEIVED
        '        s = New EMS_RECEIVED(PDUCode)
        '        e.MSISDN = s.SrcAddressValue
        '        e.Timestamp = s.TP_SCTS()
        '    Case SMSBase.SMSType.SMS_RECEIVED
        '        s = New SMS_RECEIVED(PDUCode)
        '        e.MSISDN = s.SrcAddressValue
        '        e.Timestamp = s.TP_SCTS
        'End Select
        'If s.tp_DCS = 0 Or s.tp_DCS = 242 Then
        '    If T = SMSBase.SMSType.SMS_RECEIVED Or T = SMSBase.SMSType.SMS_STATUS_REPORT Or T = SMSBase.SMSType.SMS_SUBMIT Then
        '        e.TextMessage += s.decode7bit(s.tp_UD, s.TP_UDL)
        '    End If
        '    If T = SMSBase.SMSType.EMS_RECEIVED Or T = SMSBase.SMSType.EMS_SUBMIT Then
        '        e.TextMessage = s.decode7bit(s.tp_ud, s.tp_udl - 8 * (1 + s.tp_udhl) / 7)
        '    End If
        'Else
        '    e.TextMessage = s.DecodeUnicode(s.TP_UD)
        'End If

        DecodeHexPDU = e
    End Function

    Public Function DecodeHexPDUReceived(ByVal PDU As String) As NewMessageReceivedEventArgs
        'Dim s As Object = Nothing
        Dim e As New NewMessageReceivedEventArgs
        Dim PDUCode As String = PDU.Replace(vbCrLf, "")
        If PDUCode.StartsWith("00") Then
            PDUCode = "02810011" & Mid(PDUCode, 5, PDUCode.Length - 2)
        End If

        Dim baseInfo As PDUDecoder.BaseInfo = PDUDecoder.Decode(PDUCode)
        e.MSISDN = baseInfo.SourceNumber
        e.Timestamp = baseInfo.ReceivedDate
        e.TextMessage = baseInfo.Text
        e.TotalParts = baseInfo.EMSTotolPiece
        e.SeqNo = baseInfo.EMSCurrentPiece
        'e.SMSC = Me.SMSC
        e.ReferenceNo = baseInfo.Data
        e.TimeZone = baseInfo.TimeZone

        DecodeHexPDUReceived = e
    End Function

    ''' <summary>
    ''' Send diagnostics commands
    ''' </summary>
    ''' <param name="commandString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Diagnose(ByVal commandString As String) As String
        Return serialDriver.Diagnose(commandString)
    End Function

    ''' <summary>
    ''' Send diagnostics command in a file
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <param name="outputFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Diagnose(ByVal fileName As String, ByVal outputFileName As String) As Boolean
        Return serialDriver.Diagnose(fileName, outputFileName)
    End Function

#End Region

#Region "Private Procedures"

    Private Sub serialPort_PinChanged(ByVal sender As Object, _
                            ByVal e As System.IO.Ports.SerialPinChangedEventArgs) Handles serialDriver.PinChanged

        'If e.EventType = SerialPinChange.Ring Then
        'MsgBox("ring")
        'ElseIf e.EventType = SerialPinChange.Break Then
        'MsgBox("break")
        'ElseIf e.EventType = SerialPinChange.CDChanged Then
        'MsgBox("cd changed")
        'ElseIf e.EventType = SerialPinChange.CtsChanged Then
        'MsgBox("cts changed")

        'ElseIf e.EventType = SerialPinChange.DsrChanged Then
        'MsgBox("cts changed")
        'End If
    End Sub

    Private Sub serialDriver_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles serialDriver.DataReceived
        If Not serialDriver.IsCommandMode Then
            If (serialDriver.BytesToRead > 0) Then
                Thread.Sleep(1000)
                Try
                    Dim responses() As String = ParseATResponse(serialDriver.ReadExisting())

                    If (responses.Length > 0 And Not responses(0) Is Nothing) Then
                        If responses(0).Trim.StartsWith(ATHandler.RINGBACK_RESPONSE) Then
                            '' Check line status  until the receiving party picks up the phone
                            'Do Until 1 = 0
                            '    Dim nStatus

                            '    Dim strResponse

                            '    serialDriver.Send("AT+CLCC")

                            '    strResponse = ""

                            '    strResponse = strResponse + serialDriver.ReadExisting     ' Read +CLCC Response
                            '    strResponse = strResponse + serialDriver.ReadExisting      ' Read OK

                            '    ' Strip OK
                            '    strResponse = Replace(strResponse, "OK", "")

                            '    If (InStr(strResponse, "NO CARRIER") = 1) Then
                            '        Debug.Print("No Carrier reported while calling remote party")
                            '        Exit Do
                            '    End If

                            '    If (InStr(strResponse, "BUSY") = 1) Then
                            '        Debug.Print("Busy tone received while calling remote party")
                            '        Exit Do
                            '    End If

                            '    If (InStr(strResponse, "ERROR") = 1) Then
                            '        Debug.Print("Error occured while calling remote party")
                            '        Exit Do
                            '    End If

                            '    If (InStr(strResponse, "+CLCC: ") = 1) Then

                            '        nStatus = Split(strResponse, ",", 5)

                            '        Select Case nStatus(2)
                            '            Case 0
                            '                'bConnected = True
                            '                Debug.Print("connected")
                            '                Exit Do
                            '            Case 2
                            '                Debug.Print("Dialing")
                            '            Case 3
                            '                Debug.Print("Ringing")
                            '        End Select
                            '    End If

                            '    Thread.Sleep(500)
                            'Loop)
                        ElseIf responses(0).Trim.StartsWith(ATHandler.RING_RESPONSE) Then
                            If Me.IncomingCallIndication Then
                                Dim strResult As String = ""
                                If responses.Length <= 1 Or responses(1) Is Nothing Or responses(1) = "" Then
                                    responses = ParseATResponse(serialDriver.ReadResponse(ATHandler.CLIP_RESPONSE))
                                    If responses.Length >= 1 Then
                                        strResult = responses(0)
                                    End If
                                Else
                                    strResult = responses(1)
                                End If
                                'If (responses.Length > 1 And Not responses(1) Is Nothing) Then
                                If strResult <> "" Then
                                    'responses = responses(1).Split(":")
                                    responses = strResult.Split(":")
                                    If (responses.Length > 1) Then
                                        responses = responses(1).Split(",")
                                        If (responses.Length > 1) Then
                                            If responses(0) Is Nothing Then
                                                Return
                                            End If
                                            Dim strMSISDN As String = responses(0).Replace("""", "").Trim
                                            Dim args As New NewIncomingCallEventArgs
                                            args.CallerID = strMSISDN
                                            args.Ringing = True
                                            args.Answered = False
                                            args.Status = "RINGING"
                                            RaiseEvent NewIncomingCall(args)

                                            bIncomingAnswered = False
                                        End If
                                    End If
                                End If
                            End If
                        ElseIf responses(0).Trim.StartsWith(ATHandler.STOPRING_RESPONSE) Then
                            Dim args As New NewIncomingCallEventArgs
                            args.CallerID = String.Empty
                            args.Ringing = False
                            RaiseEvent NewIncomingCall(args)
                        ElseIf responses(0).Trim.StartsWith(ATHandler.CMTI_RESPONSE) Then
                            If isNewMessageIndication Then
                                responses = responses(0).Split(":")
                                If responses.Length > 1 Then
                                    responses = responses(1).Split(",")
                                    If (responses.Length > 1) Then
                                        serialDriver.SendCmd(ATHandler.CPMS_COMMAND & "=" & responses(0).Trim())
                                        Dim index As Integer = responses(1)
                                        responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGR_COMMAND & "=" & index, ATHandler.CMGR_RESPONSE))
                                        If (responses.Length > 1) Then
                                            If responses(2) Is Nothing Then
                                                Return
                                            End If

                                            Dim msg As String = responses(2).Trim
                                            Dim arg As NewMessageReceivedEventArgs = DecodeHexPDU(msg)
                                            RaiseEvent NewMessageReceived(arg)

                                            If isAutoDeleteReadMessage Then
                                                Try
                                                    ' Remove read messages
                                                    'PurgeMessageStore(EnumSMSDeleteOption.Preferred_Message_Store)
                                                    DeleteSMS(index)
                                                Catch ex As Exception
                                                    isAutoDeleteReadMessage = False
                                                End Try
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        ElseIf responses(0).Trim.StartsWith(ATHandler.CMT_RESPONSE) Then
                            Dim arg As NewMessageReceivedEventArgs = DecodeHexPDUReceived(responses(1))
                            RaiseEvent NewMessageReceived(arg)
                        ElseIf responses(0).Trim.StartsWith(ATHandler.VOICE_RESPONSE) Then
                            If Me.IncomingCallIndication Then
                                Dim strResult As String = ""
                                If responses.Length <= 1 Or responses(1) Is Nothing Or responses(1) = "" Then
                                    responses = ParseATResponse(serialDriver.ReadResponse(ATHandler.CLIP_RESPONSE))
                                    If responses.Length >= 1 Then
                                        strResult = responses(0)
                                    End If
                                Else
                                    strResult = responses(1)
                                End If
                                'If (responses.Length > 1 And Not responses(1) Is Nothing) Then
                                If strResult <> "" Then
                                    'responses = responses(1).Split(":")
                                    responses = strResult.Split(":")
                                    If (responses.Length > 1) Then
                                        responses = responses(1).Split(",")
                                        If (responses.Length > 1) Then
                                            If responses(0) Is Nothing Then
                                                Return
                                            End If
                                            Dim strMSISDN As String = responses(0).Replace("""", "").Trim
                                            Dim args As New NewIncomingCallEventArgs
                                            args.CallerID = strMSISDN
                                            args.Ringing = True
                                            args.Answered = False
                                            args.Status = "RINGING"
                                            RaiseEvent NewIncomingCall(args)

                                            bIncomingAnswered = False
                                        End If
                                    End If
                                End If
                            End If
                        ElseIf responses(0).Trim.StartsWith(ATHandler.CSQ_RESPONSE) Then
                            Dim Rssi As Rssi
                            Rssi = GetRssi()

                            Dim args As New SignalStrengthEventArgs

                            args.Current = Rssi.Current
                            args.Maximum = Rssi.Maximum
                            args.Minimum = Rssi.Minimum
                            RaiseEvent CheckSignalStrength(args)
                        Else
                            Dim strResponse As String = responses(0)
                            If (InStr(strResponse, "NO CARRIER") = 1) Then
                                Console.WriteLine("No Carrier reported while calling remote party")
                                'If bIncomingAnswered Then
                                Dim args As New NewIncomingCallEventArgs
                                args.CallerID = strMSISDN
                                args.Ringing = False
                                args.Answered = bIncomingAnswered
                                args.Status = strResponse
                                RaiseEvent NewIncomingCall(args)

                                bIncomingAnswered = False
                                Exit Sub
                                'End If
                            End If

                            If (InStr(strResponse, "HANGUP") = 1) Then
                                Console.WriteLine("No Carrier reported while calling remote party")

                                If bIncomingAnswered Then
                                    Dim args As New NewIncomingCallEventArgs
                                    args.CallerID = strMSISDN
                                    args.Ringing = False
                                    args.Answered = True
                                    args.Status = strResponse
                                    RaiseEvent NewIncomingCall(args)

                                    bIncomingAnswered = False
                                    Exit Sub
                                End If
                            End If

                            If (InStr(strResponse, "BUSY") = 1) Then
                                Console.WriteLine("Busy tone received while calling remote party")

                                If bIncomingAnswered Then
                                    Dim args As New NewIncomingCallEventArgs
                                    args.CallerID = strMSISDN
                                    args.Ringing = False
                                    args.Answered = True
                                    args.Status = strResponse
                                    RaiseEvent NewIncomingCall(args)

                                    bIncomingAnswered = False
                                    Exit Sub
                                End If
                            End If

                            If (InStr(strResponse, "ERROR") = 1) Then
                                Console.WriteLine("Error occured while calling remote party")

                                If bIncomingAnswered Then
                                    Dim args As New NewIncomingCallEventArgs
                                    args.CallerID = strMSISDN
                                    args.Ringing = False
                                    args.Answered = True
                                    args.Status = strResponse
                                    RaiseEvent NewIncomingCall(args)

                                    bIncomingAnswered = False
                                    Exit Sub
                                End If
                            End If
                        End If
                    End If
                Catch ex As System.InvalidOperationException
                    Debug.Print(ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Function MsgReceivePDU(ByVal sData As String) As String
        Dim lsData As String
        Dim lsValue() As String
        Dim lsPDUData As String

        On Error Resume Next

        lsData = Replace(sData, vbCrLf, ",")
        lsData = Mid(lsData, 6, Len(lsData))
        lsValue = Split(lsData, ",")

        lsPDUData = lsValue(2)
        Return lsPDUData
    End Function
#End Region

#Region "Public Procedures"

    ''' <summary>
    ''' Send SMS to outbox to be delivered later.
    ''' </summary>
    ''' <param name="destinationNumber"></param>
    ''' <param name="textMessage"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendSMSToOutbox(ByVal destinationNumber As String, _
            ByVal textMessage As String) As String
        Dim message As New Message
        message.B_Number = destinationNumber
        message.Message = textMessage
        message.Encoding = Me.Encoding
        oOutbox.Add(message)
        Return message.ReferenceNo
    End Function

    ''' <summary>
    ''' Send SMS to outbox to be delivered later.
    ''' </summary>
    ''' <param name="destinationNumber"></param>
    ''' <param name="textMessage"></param>
    ''' <param name="priority"></param>
    ''' <param name="alertMessage"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendSMSToOutbox(ByVal destinationNumber As String, _
                              ByVal textMessage As String, _
                              ByVal priority As EnumQueuePriority, _
                              ByVal alertMessage As Boolean) As String
        Dim message As New Message
        message.B_Number = destinationNumber
        message.Message = textMessage
        message.Encoding = Me.Encoding
        message.Priority = priority
        message.AlertMessage = alertMessage
        oOutbox.Add(message)
        SendSMSToOutbox = message.ReferenceNo
    End Function

    ''' <summary>
    ''' Send SMS to outbox to be delivered later.
    ''' </summary>
    ''' <param name="destinationNumber"></param>
    ''' <param name="textMessage"></param>
    ''' <param name="priority"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendSMSToOutbox(ByVal destinationNumber As String, _
                             ByVal textMessage As String, _
                             ByVal priority As EnumQueuePriority) As String
        Dim message As New Message
        message.B_Number = destinationNumber
        message.Message = textMessage
        message.Encoding = Me.Encoding
        message.Priority = priority
        oOutbox.Add(message)
        SendSMSToOutbox = message.ReferenceNo
    End Function

    ''' <summary>
    ''' Analyze phone capabilities
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CheckATCommands()
        Dim responses() As String

        Try
            responses = ParseATResponse(serialDriver.SendCmd("ATZ"))
        Catch ex As System.Exception

        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd("AT+CFUN=0", ATHandler.RESPONSE_OK))
            If responses.Length > 0 Then
                responses = ParseATResponse(serialDriver.SendCmd("AT+CFUN=1", "+CREG: 1"))
            End If
        Catch ex As System.Exception            '
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.HANG_UP_COMMAND))
            If responses(0) = ATHandler.RESPONSE_OK Then
                commandHandler.Is_ATH_Supported = True
            Else
                commandHandler.Is_ATH_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_ATH_Supported = False
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CHUP_HANG_UP_COMMAND & "=?"))
            If responses(0) = ATHandler.RESPONSE_OK Then
                commandHandler.Is_AT_CHUP_Supported = True
            Else
                commandHandler.Is_AT_CHUP_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CHUP_Supported = False
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CLIP_COMMAND & "=?", ATHandler.CLIP_RESPONSE))
            If responses.Length > 0 Then
                commandHandler.Is_AT_CLIP_Supported = True
            Else
                commandHandler.Is_AT_CLIP_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CLIP_Supported = False
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGS_COMMAND & "=?"))
            If responses(0) = ATHandler.RESPONSE_OK Then
                commandHandler.Is_AT_CMGS_Supported = True
            Else
                commandHandler.Is_AT_CMGS_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CMGS_Supported = False
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGW_COMMAND & "=?"))
            If responses(0) <> ATHandler.RESPONSE_ERROR Then
                commandHandler.Is_AT_CMGW_Supported = True
            Else
                commandHandler.Is_AT_CMGW_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CMGW_Supported = False
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CMSS_COMMAND & "=?"))
            If responses(0) <> ATHandler.RESPONSE_ERROR Then
                commandHandler.Is_AT_CMSS_Supported = True
            Else
                commandHandler.Is_AT_CMSS_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CMSS_Supported = False
        End Try

        Try
            commandHandler.Is_AT_CMGF_0_Supported = False
            commandHandler.Is_AT_CMGF_1_Supported = False
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGF_COMMAND & "=?", ATHandler.CMGF_RESPONSE))
            If responses.Length > 0 Then
                If Trim(responses(0)) <> "" Then
                    responses = responses(0).Split(":")
                    If responses.Length > 1 Then
                        responses(1) = responses(1).Replace("(", "")
                        responses(1) = responses(1).Replace(")", "")
                        responses = responses(1).Split(",")
                        If responses.Length > 0 Then
                            Dim i As Integer
                            Dim v As String
                            For i = 0 To responses.Length - 1
                                v = responses(i).Trim
                                If v = "0" Then
                                    commandHandler.Is_AT_CMGF_0_Supported = True
                                ElseIf v = "1" Then
                                    commandHandler.Is_AT_CMGF_1_Supported = True
                                End If
                            Next

                        End If
                    End If
                End If
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CMGF_0_Supported = False
            commandHandler.Is_AT_CMGF_1_Supported = False
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CMGF_COMMAND & "=0"))
            If responses(0) <> ATHandler.RESPONSE_OK Then
                commandHandler.Is_AT_CMGF_0_Supported = True
            Else
                commandHandler.Is_AT_CMGF_0_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CSQ_Supported = False
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CSQ_COMMAND & "=?"))
            If responses(0) <> ATHandler.RESPONSE_ERROR Then
                commandHandler.Is_AT_CSQ_Supported = True
            Else
                commandHandler.Is_AT_CSQ_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CSQ_Supported = False
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CBC_COMMAND & "=?"))
            If responses(0) <> ATHandler.RESPONSE_ERROR Then
                commandHandler.Is_AT_CBC_Supported = True
            Else
                commandHandler.Is_AT_CBC_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CBC_Supported = False
        End Try

        Try
            Dim i As Integer
            commandHandler.Is_SMS_Received_Supported = True

            If Not commandHandler.Is_SMS_Received_Supported Then
                commandHandler.Is_AT_CNMI_Supported = False
            End If

            If commandHandler.Is_SMS_Received_Supported Then
                ' Check CNMI support
                responses = ParseATResponse(serialDriver.SendCmd(ATHandler.MESSAGE_INDICATION_COMMAND & "=?", ATHandler.CNMI_RESPONSE))
                'responses(0) = "+CNMI: (0-2),(0-3),(0,2,3),(0-2),(0,1)"
                If responses(0) <> ATHandler.RESPONSE_ERROR Then
                    If Trim(responses(0)) <> "" Then
                        responses = responses(0).Split(":")
                        If responses.Length > 1 Then
                            Dim stringSeparators() As String = {"),("}
                            responses = responses(1).Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
                            If responses.Length > 1 Then
                                For i = 0 To responses.Length - 1
                                    responses(i) = responses(i).Replace("(", "")
                                    responses(i) = responses(i).Replace(")", "")
                                    responses(i) = responses(i).Trim
                                Next

                                ' For CNMI, 1st number is : 2 or 3
                                Dim range As Hashtable = ATHandler.GetNumberRange(responses(0))
                                If range.Contains("2") Or range.Contains("3") Then
                                    If range.Contains("2") Then
                                        commandHandler.MsgIndication = "2,"
                                    ElseIf range.Contains("3") Then
                                        commandHandler.MsgIndication = "3,"
                                    End If
                                Else
                                    commandHandler.Is_SMS_Received_Supported = False
                                End If

                                ' 2nd number must support 1
                                If commandHandler.Is_SMS_Received_Supported Then
                                    range = ATHandler.GetNumberRange(responses(1))
                                    If Not range.Contains("1") Then
                                        commandHandler.Is_SMS_Received_Supported = False
                                    Else
                                        commandHandler.MsgIndication += "1,0,0,"
                                    End If
                                End If

                                ' For CNMI, last number is 0 or 1
                                If commandHandler.Is_SMS_Received_Supported Then
                                    range = ATHandler.GetNumberRange(responses(responses.Length - 1))
                                    If range.Contains("0") Then
                                        commandHandler.MsgIndication += "0"
                                    ElseIf range.Contains("1") Then
                                        commandHandler.MsgIndication += "1"
                                    End If
                                End If

                                'Dim value As String = responses(0)
                                'If value.IndexOf("2") >= 0 Then
                                '    atHandler.MsgIndication = "2,1,0,0,"
                                'ElseIf value.IndexOf("3") >= 0 Then
                                '    oFeature.CNMI_COMMAND_STRING = "AT+CNMI=3,1,0,0,0"
                                'End If
                                'oFeature.SUPPORT_AT_CNMI = True
                            End If
                        End If
                        commandHandler.Is_SMS_Received_Supported = True
                    End If
                Else
                    commandHandler.Is_AT_CNMI_Supported = False
                    commandHandler.Is_SMS_Received_Supported = False
                End If
            End If
        Catch ex As System.Exception
            commandHandler.Is_SMS_Received_Supported = False
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CPMS_COMMAND & "=?", ATHandler.CPMS_RESPONSE))
            If responses(0) <> ATHandler.RESPONSE_ERROR Then
                Try
                    responses = responses(0).Split(":")
                Catch ex As NullReferenceException
                    commandHandler.Is_AT_CPMS_Supported = False
                End Try

                If responses.Length > 1 Then
                    If responses(1).IndexOf("()") >= 0 Then
                        commandHandler.Is_AT_CPMS_Supported = False
                        If commandHandler.Is_SMS_Received_Supported Then
                            commandHandler.Is_SMS_Received_Supported = False
                        End If
                    Else
                        commandHandler.Is_AT_CPMS_Supported = True
                    End If
                End If

            Else
                commandHandler.Is_AT_CPMS_Supported = False
            End If
        Catch ex As System.NullReferenceException
            commandHandler.Is_AT_CPMS_Supported = False
        Catch ex As System.Exception
            commandHandler.Is_AT_CPMS_Supported = False
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.COPS_COMMAND & "=3,2"))
            If responses(0) <> ATHandler.RESPONSE_ERROR Then
                commandHandler.Is_Numeric_MCC_MNC_Supported = True
            Else
                commandHandler.Is_Numeric_MCC_MNC_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_Numeric_MCC_MNC_Supported = True
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CNUM_COMMAND & "=?"))
            If responses(0) <> ATHandler.RESPONSE_ERROR Then
                commandHandler.Is_AT_CNUM_Supported = True
            Else
                commandHandler.Is_AT_CNUM_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CNUM_Supported = False
        End Try

        If commandHandler.Is_AT_CHUP_Supported Then
            commandHandler.HangUpCommand = ATHandler.CHUP_HANG_UP_COMMAND
        ElseIf commandHandler.Is_ATH_Supported Then
            commandHandler.HangUpCommand = ATHandler.HANG_UP_COMMAND
        End If

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CSCS_COMMAND & "=" & Chr(34) & "GSM" & Chr(34) & Chr(13)))
            If responses(0) <> ATHandler.RESPONSE_ERROR Then
                commandHandler.Is_AT_CSCS_Supported = True
            Else
                commandHandler.Is_AT_CSCS_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CSCS_Supported = True
        End Try

        Try
            responses = ParseATResponse(serialDriver.SendCmd(ATHandler.CREG_COMMAND & "=1"))
            If responses(0) <> ATHandler.RESPONSE_ERROR Then
                commandHandler.Is_AT_CREG_Supported = True
            Else
                commandHandler.Is_AT_CREG_Supported = False
            End If
        Catch ex As System.Exception
            commandHandler.Is_AT_CREG_Supported = True
        End Try
    End Sub

    Public Function ResetModem() As Boolean
        Dim responses() As String

        responses = ParseATResponse(serialDriver.SendCmd("ATZ", ATHandler.RESPONSE_OK))
        If responses.Length > 0 Then
            Try
                responses = ParseATResponse(serialDriver.SendCmd("AT+CFUN=0", ATHandler.RESPONSE_OK))
                If responses.Length > 0 Then
                    responses = ParseATResponse(serialDriver.SendCmd("AT+CFUN=1", "+CREG: 1"))
                    If responses.Length > 0 Then
                        Return True
                    End If
                End If
                Return False
            Catch ex As System.Exception
                Return False
            End Try
        End If
    End Function
#End Region

#Region "Event"

    'Public Event NewDeliveryReport(ByVal sender As Object, _
    '       ByVal e As NewDeliveryReportEventArgs)

    Public Event CheckSignalStrength( _
           ByVal e As SignalStrengthEventArgs)

    Public Event NewIncomingCall( _
           ByVal e As NewIncomingCallEventArgs)

    Public Event NewMessageReceived( _
           ByVal e As NewMessageReceivedEventArgs)

    Public Event OutboxSMSSending(ByVal e As OutboxSMSSendingEventArgs)

    Public Event OutboxSMSSent(ByVal e As OutboxSMSSentEventArgs)

    'Public Event OutboxVCalendarSending()

    'Public Event OutboxVCalendarSent()

    'Public Event OutboxVCardSending()

    'Public Event OutboxVCardSent()

    'Public Event OutboxWapPushSending()

    'Public Event OutboxWapPushSent()
#End Region

#Region "SMS Thread"
    Private Sub SMSThread(ByVal state As Object)

        If bSendingSMS Then
            Return
        End If

        bSendingSMS = True
        If oOutbox.Count > 0 Then
            While oOutbox.Count > 0
                ' Send the SMS
                Dim msg As Message = oOutbox.Dequeue()
                Dim arg1 As New OutboxSMSSendingEventArgs
                arg1.DestinationNumber = msg.B_Number
                arg1.Priority = msg.Priority
                arg1.QueueMessageKey = msg.ReferenceNo
                arg1.Message = msg.Message
                'RaiseEvent OutboxSMSSending(arg1)
                Dim arg2 As New OutboxSMSSentEventArgs
                Try
                    ' Send the message
                    Dim msgId As String = SendSMS(msg.B_Number, msg.Message, msg.Encoding)
                    arg2.DestinationNumber = msg.B_Number
                    arg2.MessageReference = msgId
                    arg2.QueueMessageKey = msg.ReferenceNo
                    arg2.Message = msg.Message
                    arg2.SendResult = True
                    RaiseEvent OutboxSMSSent(arg2)
                Catch e As System.Exception
                    arg2.SendResult = False

                    ' To do set specific error code here 
                    ' TODO -----------------------------------------------------------------------------
                    arg2.ErrorCode = ReturnCode.SEND_MESSAGE_ERROR
                    arg2.ErrorDescription = e.Message
                    RaiseEvent OutboxSMSSent(arg2)

                    MsgBox(e.Message)

                    'Throw New SMSSendException(e.Message, e)
                Finally

                End Try

                ' Sleep for 5 seconds between each send
                'Thread.Sleep(5000)
            End While

        End If

        bSendingSMS = False
    End Sub
#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        msgTimer.Dispose()
    End Sub
End Class

