'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    SMS PDU
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
Imports ATSMS.SMS.Encoder.SMS
Imports ATSMS.SMS.Encoder.ConcatenatedShortMessage

Namespace SMS

    Public Class PDU
        Private DataCodingScheme As ENUM_TP_DCS

        Public Function GetPDU( _
                               ByVal ServiceCenterNumber As String, _
                               ByVal DestNumber As String, _
                               ByVal DataCodingScheme As ENUM_TP_DCS, _
                               ByVal ValidPeriod As ENUM_TP_VALID_PERIOD, _
                               ByVal MsgReference As Integer, _
                               ByVal StatusReport As Boolean, _
                               ByVal UserData As String) As String()
            'Check for SMS type
            Dim Type As Integer '0 for SMS;1 For ConcatenatedShortMessage
            Dim Result() As String
            Dim SMSObject As Object
            SMSObject = New SMS.Encoder.SMS
            Select Case DataCodingScheme
                Case ENUM_TP_DCS.DefaultAlphabet, ENUM_TP_DCS.Class2_UD_7bits
                    If UserData.Length > 160 Then
                        SMSObject = New SMS.Encoder.ConcatenatedShortMessage
                        Type = 1
                    End If
                Case ENUM_TP_DCS.UCS2
                    If UserData.Length > 70 Then
                        SMSObject = New SMS.Encoder.ConcatenatedShortMessage
                        Type = 1
                    End If
            End Select

            With SMSObject
                .ServiceCenterNumber = ServiceCenterNumber
                If StatusReport = True Then
                    .TP_Status_Report_Request = SMS.ENUM_TP_SRI.Request_SMS_Report
                Else
                    .TP_Status_Report_Request = SMS.ENUM_TP_SRI.No_SMS_Report
                End If
                .TP_Destination_Address = DestNumber
                .TP_Data_Coding_Scheme = DataCodingScheme
                .TP_Message_Reference = MsgReference
                .TP_Validity_Period = ValidPeriod
                .TP_User_Data = UserData
            End With

            If Type = 0 Then
                ReDim Result(0)
                Result(0) = SMSObject.GetSMSPDUCode
            Else
                Result = SMSObject.GetEMSPDUCode            'Note here must use GetEMSPDUCode to get right PDU codes
            End If
            Return Result
        End Function

        Public Function GetPDUConcatenated( _
                               ByVal ServiceCenterNumber As String, _
                               ByVal DestNumber As String, _
                               ByVal DataCodingScheme As ENUM_TP_DCS, _
                               ByVal ValidPeriod As ENUM_TP_VALID_PERIOD, _
                               ByVal MsgReference As Integer, _
                               ByVal StatusReport As Boolean, _
                               ByVal UserData As String) As String()
            'Check for SMS type
            Dim Type As Integer '0 for SMS;1 For ConcatenatedShortMessage
            Dim Result() As String
            Dim SMSObject As Object
            SMSObject = New SMS.Encoder.SMS
            Select Case DataCodingScheme
                Case ENUM_TP_DCS.DefaultAlphabet, ENUM_TP_DCS.Class2_UD_7bits
                    If UserData.Length > 160 Then
                        SMSObject = New SMS.Encoder.ConcatenatedShortMessage
                        Type = 1
                    End If
                Case ENUM_TP_DCS.UCS2
                    If UserData.Length > 70 Then
                        SMSObject = New SMS.Encoder.ConcatenatedShortMessage
                        Type = 1
                    End If
            End Select

            With SMSObject
                .ServiceCenterNumber = ServiceCenterNumber
                If StatusReport = True Then
                    .TP_Status_Report_Request = SMS.ENUM_TP_SRI.Request_SMS_Report
                Else
                    .TP_Status_Report_Request = SMS.ENUM_TP_SRI.No_SMS_Report
                End If
                .TP_Destination_Address = DestNumber
                .TP_Data_Coding_Scheme = DataCodingScheme
                .TP_Message_Reference = MsgReference
                .TP_Validity_Period = ValidPeriod
                .TP_User_Data_Concat = UserData
            End With

            ReDim Result(0)
            Result(0) = SMSObject.GetSMSPDUCodeConcat
            
            Return Result
        End Function

        Public Function GetATLength(ByVal pduString As String) As Integer
            GetATLength = (pduString.Length - Val("&H" & Mid(pduString, 1, 2)) * 2 - 2) / 2  'Calculate PDU Length for AT command
        End Function

    End Class

End Namespace