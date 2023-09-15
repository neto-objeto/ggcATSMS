'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    Modem ATHandler
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
''' <summary>
''' AT Command class
''' </summary>
''' <remarks></remarks>
Public Class ATHandler

    ''' <summary>
    ''' AT response
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared RESPONSE_ERROR As String = "ERROR"
    Public Shared RESPONSE_NO_CARRIER As String = "NO CARRIER"
    Public Shared RESPONSE_OK As String = "OK"
    Public Shared RESPONSE_NOT_SUPPORTED As String = "NOT SUPPORTED"
    Public Shared RESPONSE_CMS_ERROR = "+CMS ERROR"
    Public Shared RESPONSE_PIN_REQUIRED = "SIM PIN"
    Public Shared RESPONSE_PIN_NOT_REQUIRED = "READY"

    ''' <summary>
    ''' AT commands
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared KEEP_ALIVE_COMMAND As String = "AT"
    Public Shared ECHO_COMMAND As String = "ATE1"
    Public Shared NO_ECHO_COMMAND As String = "ATE0"
    Public Shared MANUFACTURER_COMMAND As String = "ATI0"
    Public Shared REVISION_COMMAND As String = "ATI2"
    Public Shared HANG_UP_COMMAND As String = "ATH"
    Public Shared CHUP_HANG_UP_COMMAND As String = "AT+CHUP"
    Public Shared MESSAGE_INDICATION_COMMAND As String = "AT+CNMI"
    Public Shared CLIP_COMMAND As String = "AT+CLIP"
    Public Shared CMGS_COMMAND As String = "AT+CMGS"
    Public Shared CMGW_COMMAND As String = "AT+CMGW"
    Public Shared CMSS_COMMAND As String = "AT+CMSS"
    Public Shared CMGF_COMMAND As String = "AT+CMGF"
    Public Shared CSQ_COMMAND As String = "AT+CSQ"
    Public Shared CBC_COMMAND As String = "AT+CBC"
    Public Shared CPMS_COMMAND As String = "AT+CPMS"
    Public Shared COPS_COMMAND As String = "AT+COPS"
    Public Shared CNUM_COMMAND As String = "AT+CNUM"
    Public Shared VTS_COMMAND As String = "AT+VTS"
    Public Shared CGSN_COMMAND As String = "AT+CGSN"
    Public Shared CIMI_COMMAND As String = "AT+CIMI"
    Public Shared CSCA_COMMAND As String = "AT+CSCA"
    Public Shared GMM_COMMAND As String = "AT+GMM"
    Public Shared CMGL_COMMAND As String = "AT+CMGL"
    Public Shared CMGD_COMMAND As String = "AT+CMGD"
    Public Shared CLIR_COMMAND As String = "AT+CLIR"
    Public Shared CREG_COMMAND As String = "AT+CREG"
    Public Shared ATD_COMMAND As String = "ATD"
    Public Shared ATA_COMMAND As String = "ATA"
    Public Shared CMGR_COMMAND As String = "AT+CMGR"
    Public Shared CPIN_COMMAND As String = "AT+CPIN"
    Public Shared CSCS_COMMAND As String = "AT+CSCS"

    Public Shared CLIP_RESPONSE As String = "+CLIP"
    Public Shared CMGF_RESPONSE As String = "+CMGF"
    Public Shared CNMI_RESPONSE As String = "+CNMI"
    Public Shared CPMS_RESPONSE As String = "+CPMS"
    Public Shared CMGS_RESPONSE As String = "+CMGS"
    Public Shared VTS_RESPONSE As String = "VTS"
    Public Shared CNUM_RESPONSE As String = "+CNUM"
    Public Shared CMGL_RESPONSE As String = "+CMGL"
    Public Shared CLIR_RESPONSE As String = "+CLIR"
    Public Shared COPS_RESPONSE As String = "+COPS"
    Public Shared CREG_RESPONSE As String = "+CREG"
    Public Shared CBC_RESPONSE As String = "+CBC"
    Public Shared CSQ_RESPONSE As String = "+CSQ"
    Public Shared RING_RESPONSE As String = "RING"
    Public Shared VOICE_RESPONSE As String = "+CRING: VOICE"
    Public Shared STOPRING_RESPONSE As String = "STOPRING"
    Public Shared RINGBACK_RESPONSE As String = "RINGBACK"
    Public Shared CMGR_RESPONSE As String = "+CMGR"
    Public Shared CMTI_RESPONSE As String = "+CMTI"
    Public Shared CMT_RESPONSE As String = "+CMT"
    Public Shared CPIN_RESPONSE As String = "+CPIN"
    Public Shared CSCA_RESPONSE As String = "+CSCA"

    Private isAT_CHUP_Command_Supported As Boolean
    Private isATH_Command_Supported As Boolean
    Private isAT_CNUM_Command_Supported As Boolean
    Private isAT_CLIP_Command_Supported As Boolean
    Private isAT_CMGS_Command_Supported As Boolean
    Private isAT_CMGW_Command_Supported As Boolean
    Private isAT_CMSS_Command_Supported As Boolean
    Private isAT_CMGF_0_Command_Supported As Boolean
    Private isAT_CMGF_1_Command_Supported As Boolean
    Private isAT_CBC_Command_Supported As Boolean
    Private isAT_CSQ_Command_Supported As Boolean
    Private isAT_CPMS_Command_Supported As Boolean
    Private isAT_CMNI_Command_Supported As Boolean
    Private isAT_CSCS_Command_Supported As Boolean
    Private isAT_CREG_Command_Supported As Boolean

    Private isNumeric_MCC_MNC_Supported As Boolean
    Private isSMS_Received_Supported As Boolean

    'Private strCmdKeepAlive As String
    'Private strCmdEcho As String
    'Private strCmdNoEcho As String
    'Private strCmdManufacturer As String
    'Private strCmdRevision As String
    'Private strCmdCHUPHangUp As String
    'Private strCmdHangUp As String
    'Private strCmdMsgIndication As String
    'Private strCmdCLIP As String
    'Private strCmdCMGS As String
    'Private strCmdCMGW As String
    'Private strCmdCMSS As String
    'Private strCmdCMGF As String
    'Private strCmdCSQ As String
    'Private strCmdCBC As String
    'Private strCmdCPMS As String
    'Private strCmdCOPS As String
    'Private strCmdCNUM As String
    'Private strCmdVTS As String
    'Private strCmdCGSN As String
    'Private strCmdCIMI As String
    'Private strCmdCSCA As String
    'Private strCmdGMM As String

    'Private strCLIPResponse As String
    'Private strCMGFResponse As String
    'Private strCNMIResponse As String
    'Private strCPMSResponse As String
    'Private strCMGSResponse As String
    'Private strVTFResponse As String
    'Private strCNUMResponse As String

    Private strMsgIndication As String
    Private strNoMsgIndication As String
    Private strHangUpCommand As String
    Private strCHUPHangUpCommand As String

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        'KeepAliveCommand = "AT"
        'EchoCommand = "ATE1"
        'NoEchoCommand = "ATE0"
        'ManufacturerCommand = "ATI0"
        'RevisionCommand = "ATI2"
        'HangUpCommand = "ATH"
        'CHUPHangUpCommand = "AT+CHUP"
        'MsgIndicationCommand = "AT+CNMI"
        'CLIPCommand = "AT+CLIP"
        'CMGSCommand = "AT+CMGS"
        'CMGWCommand = "AT+CMGW"
        'CMSSCommand = "AT+CMSS"
        'CMGFCommand = "AT+CMGF"
        'CSQCommand = "AT+CSQ"
        'CBCCommand = "AT+CBC"
        'CPMSCommand = "AT+CPMS"
        'COPSCommand = "AT+COPS"
        'CNUMCommand = "AT+CNUM"
        'VTSCommand = "AT+VTS"
        'CGSNCommand = "AT+CGSN"
        'CIMICommand = "AT+CIMI"
        'CSCACommand = "AT+CSCA"
        'GMMCommand = "AT+GMM"

        'CLIPResponse = "+CLIP"
        'CMGFREsponse = "+CMGF"
        'CNMIResponse = "+CNMI"
        'CPMSResponse = "+CPMS"
        'CMGSResponse = "+CMGS"
        'VTSResponse = "VTS"
        'CNUMResponse = "+CNUM"

        MsgIndication = "2,1,0,0,0"
        'MsgIndication = "2,1"

        HangUpCommand = HANG_UP_COMMAND
        CHUPHangUpCommand = "AT+CHUP"


        Is_AT_CHUP_Supported = True
        Is_ATH_Supported = True
        Is_AT_CNUM_Supported = True
        Is_AT_CLIP_Supported = True
        Is_AT_CMGS_Supported = True
        Is_AT_CMGW_Supported = True
        Is_AT_CMSS_Supported = True
        Is_AT_CMGF_0_Supported = True
        Is_AT_CMGF_1_Supported = True
        Is_SMS_Received_Supported = True
        Is_AT_CNMI_Supported = True
        Is_AT_CBC_Supported = True
        Is_AT_CSQ_Supported = True
        Is_Numeric_MCC_MNC_Supported = True
        Is_AT_CPMS_Supported = True
        Is_AT_CSCS_Supported = True
    End Sub

    Public Property Is_AT_CHUP_Supported() As Boolean
        Get
            Return isAT_CHUP_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CHUP_Command_Supported = value
        End Set
    End Property

    Public Property Is_ATH_Supported() As Boolean
        Get
            Return isATH_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isATH_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CNUM_Supported() As Boolean
        Get
            Return isAT_CNUM_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CNUM_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CLIP_Supported() As Boolean
        Get
            Return isAT_CLIP_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CLIP_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CMGS_Supported() As Boolean
        Get
            Return isAT_CMGS_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CMGS_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CMGW_Supported() As Boolean
        Get
            Return isAT_CMGW_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CMGW_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CMSS_Supported() As Boolean
        Get
            Return isAT_CMSS_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CMSS_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CMGF_0_Supported() As Boolean
        Get
            Return isAT_CMGF_0_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CMGF_0_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CMGF_1_Supported() As Boolean
        Get
            Return isAT_CMGF_1_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CMGF_1_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CBC_Supported() As Boolean
        Get
            Return isAT_CBC_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CBC_Command_Supported = value
        End Set
    End Property


    Public Property Is_AT_CSQ_Supported() As Boolean
        Get
            Return isAT_CSQ_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CSQ_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CPMS_Supported() As Boolean
        Get
            Return isAT_CPMS_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CPMS_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CNMI_Supported() As Boolean
        Get
            Return isAT_CMNI_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CMNI_Command_Supported = value
        End Set
    End Property

    Public Property Is_Numeric_MCC_MNC_Supported() As Boolean
        Get
            Return isNumeric_MCC_MNC_Supported
        End Get
        Set(ByVal value As Boolean)
            isNumeric_MCC_MNC_Supported = value
        End Set
    End Property

    Public Property Is_SMS_Received_Supported() As Boolean
        Get
            Return isSMS_Received_Supported
        End Get
        Set(ByVal value As Boolean)
            isSMS_Received_Supported = value
        End Set
    End Property

    Public Property Is_AT_CSCS_Supported() As Boolean
        Get
            Return isAT_CSCS_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CSCS_Command_Supported = value
        End Set
    End Property

    Public Property Is_AT_CREG_Supported() As Boolean
        Get
            Return isAT_CREG_Command_Supported
        End Get
        Set(ByVal value As Boolean)
            isAT_CREG_Command_Supported = value
        End Set
    End Property

    'Public Property EchoCommand() As String
    '    Get
    '        Return strCmdEcho
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdEcho = value
    '    End Set
    'End Property

    'Public Property NoEchoCommand() As String
    '    Get
    '        Return strCmdNoEcho
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdNoEcho = value
    '    End Set
    'End Property

    'Public Property KeepAliveCommand() As String
    '    Get
    '        Return strCmdKeepAlive
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdKeepAlive = value
    '    End Set
    'End Property

    'Public Property ManufacturerCommand() As String
    '    Get
    '        Return strCmdManufacturer
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdManufacturer = value
    '    End Set
    'End Property

    'Public Property RevisionCommand() As String
    '    Get
    '        Return strCmdRevision
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdRevision = value
    '    End Set
    'End Property

    'Public Property CHUPHangUpCommand() As String
    '    Get
    '        Return strCmdCHUPHangUp
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCHUPHangUp = value
    '    End Set
    'End Property

    'Public Property HangUpCommand() As String
    '    Get
    '        Return strCmdHangUp
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdHangUp = value
    '    End Set
    'End Property

    'Public Property MsgIndicationCommand() As String
    '    Get
    '        Return strCmdMsgIndication
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdMsgIndication = value
    '    End Set
    'End Property

    'Public Property CLIPCommand() As String
    '    Get
    '        Return strCmdCLIP
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCLIP = value
    '    End Set
    'End Property

    'Public Property CLIPResponse() As String
    '    Get
    '        Return strCLIPResponse
    '    End Get
    '    Set(ByVal value As String)
    '        strCLIPResponse = value
    '    End Set
    'End Property

    'Public Property CMGSCommand() As String
    '    Get
    '        Return strCmdCMGS
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCMGS = value
    '    End Set
    'End Property

    'Public Property CMGWCommand() As String
    '    Get
    '        Return strCmdCMGW
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCMGW = value
    '    End Set
    'End Property

    'Public Property CMSSCommand() As String
    '    Get
    '        Return strCmdCMSS
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCMSS = value
    '    End Set
    'End Property

    'Public Property CMGFCommand() As String
    '    Get
    '        Return strCmdCMGF
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCMGF = value
    '    End Set
    'End Property

    'Public Property CMGFREsponse() As String
    '    Get
    '        Return strCMGFResponse
    '    End Get
    '    Set(ByVal value As String)
    '        strCMGFResponse = value
    '    End Set
    'End Property

    'Public Property CSQCommand() As String
    '    Get
    '        Return strCmdCSQ
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCSQ = value
    '    End Set
    'End Property '

    'Public Property CBCCommand() As String
    '    Get
    '        Return strCmdCBC
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCBC = value
    '    End Set
    'End Property


    Public Property MsgIndication() As String
        Get
            Return strMsgIndication
        End Get
        Set(ByVal value As String)
            strMsgIndication = value
        End Set
    End Property

    Public Property NoMsgIndication() As String
        Get
            Return strNoMsgIndication
        End Get
        Set(ByVal value As String)
            strNoMsgIndication = value
        End Set
    End Property

    'Public Property CNMIResponse() As String
    '    Get
    '        Return strCNMIResponse
    '    End Get
    '    Set(ByVal value As String)
    '        strCNMIResponse = value
    '    End Set
    'End Property

    'Public Property CPMSCommand() As String
    '    Get
    '        Return strCmdCPMS
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCPMS = value
    '    End Set
    'End Property

    'Public Property CPMSResponse() As String
    '    Get
    '        Return strCPMSResponse
    '    End Get
    '    Set(ByVal value As String)
    '        strCPMSResponse = value
    '    End Set
    'End Property

    'Public Property COPSCommand() As String
    '    Get
    '        Return strCmdCOPS
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCOPS = value
    '    End Set
    'End Property

    'Public Property CNUMCommand() As String
    '    Get
    '        Return strCmdCNUM
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCNUM = value
    '    End Set
    'End Property

    'Public Property CMGSResponse() As String
    '    Get
    '        Return strCMGSResponse
    '    End Get
    '    Set(ByVal value As String)
    '        strCMGSResponse = value
    '    End Set
    'End Property

    'Public Property VTSCommand() As String
    '    Get
    '        Return strCmdVTS
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdVTS = value
    '    End Set
    'End Property

    'Public Property VTSResponse() As String
    '    Get
    '        Return strVTFResponse
    '    End Get
    '    Set(ByVal value As String)
    '        strVTFResponse = value
    '    End Set
    'End Property

    'Public Property CGSNCommand() As String
    '    Get
    '        Return strCmdCGSN
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCGSN = value
    '    End Set
    'End Property

    'Public Property CIMICommand() As String
    '    Get
    '        Return strCmdCIMI
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCIMI = value
    '    End Set
    'End Property

    'Public Property CSCACommand() As String
    '    Get
    '        Return strCmdCSCA
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdCSCA = value
    '    End Set
    'End Property

    'Public Property CNUMResponse() As String
    '    Get
    '        Return strCNUMResponse
    '    End Get
    '    Set(ByVal value As String)
    '        strCNUMResponse = value
    '    End Set
    'End Property

    'Public Property GMMCommand() As String
    '    Get
    '        Return strCmdGMM
    '    End Get
    '    Set(ByVal value As String)
    '        strCmdGMM = value
    '    End Set
    'End Property


    Public Property HangUpCommand() As String
        Get
            Return strHangUpCommand
        End Get
        Set(ByVal value As String)
            strHangUpCommand = value
        End Set
    End Property

    Public Property CHUPHangUpCommand() As String
        Get
            Return strCHUPHangUpCommand
        End Get
        Set(ByVal value As String)
            strCHUPHangUpCommand = value
        End Set
    End Property

    ''' <summary>
    ''' Retrieve the supported range of values from the AT responses
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetNumberRange(ByVal str As String) As Hashtable
        If str Is Nothing Then Return Nothing
        str = str.Trim()
        If str = String.Empty Then Return Nothing

        Dim resultMap As New Hashtable
        Dim numberList() As String
        Dim idx As Integer
        If str.IndexOf(",") > 0 Then
            numberList = str.Split(",")
            For idx = 0 To numberList.Length - 1
                resultMap.Add(numberList(idx), numberList(idx))
            Next
        ElseIf str.IndexOf("-") > 0 Then
            numberList = str.Split("-")
            If numberList.Length = 2 Then
                For idx = 0 To numberList.Length - 1
                    numberList(idx) = numberList(idx).Trim()
                Next
                If IsNumeric(numberList(0)) And IsNumeric(numberList(0)) Then
                    Dim lowerBound As Integer = Convert.ToInt16(numberList(0))
                    Dim upperbound As Integer = Convert.ToInt16(numberList(1))
                    For idx = lowerBound To upperbound
                        resultMap.Add(idx.ToString, idx.ToString)
                    Next
                End If
            End If
        Else
            resultMap.Add(str, str)
        End If
        Return resultMap
    End Function

End Class

