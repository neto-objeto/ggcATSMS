'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    SMS ServiceIndication
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
Imports System
Imports System.Collections
Imports System.IO
Imports System.Text

Namespace SMS

    ' Allowed values of the action attribute of the indication tag
    Public Enum ServiceIndicationAction
        NotSet
        signal_none
        signal_low
        signal_medium
        signal_high
        delete
    End Enum 'ServiceIndicationAction

    ' <summary>
    ' Encapsulates the Service Indication WAP Push instruction.
    ' Full documentation can be found at http://www.openmobilealliance.org/tech/affiliates/wap/wap-167-serviceind-20010731-a.pdf?doc=wap-167-serviceind-20010731-a.pdf
    ' </summary>
    Public Class ServiceIndication
        ' Well known DTD token
        Public Const DOCUMENT_DTD_ServiceIndication As Byte = &H5 ' ServiceIndication 1.0 Public Identifier

        ' Tag Tokens
        Public Const TAGTOKEN_si As Byte = &H5
        Public Const TAGTOKEN_indication As Byte = &H6
        Public Const TAGTOKEN_info As Byte = &H7
        Public Const TAGTOKEN_item As Byte = &H8

        ' Attribute Tokens
        Public Const ATTRIBUTESTARTTOKEN_action_signal_none As Byte = &H5
        Public Const ATTRIBUTESTARTTOKEN_action_signal_low As Byte = &H6
        Public Const ATTRIBUTESTARTTOKEN_action_signal_medium As Byte = &H7
        Public Const ATTRIBUTESTARTTOKEN_action_signal_high As Byte = &H8
        Public Const ATTRIBUTESTARTTOKEN_action_signal_delete As Byte = &H9
        Public Const ATTRIBUTESTARTTOKEN_created As Byte = &HA
        Public Const ATTRIBUTESTARTTOKEN_href As Byte = &HB
        Public Const ATTRIBUTESTARTTOKEN_href_http As Byte = &HC ' http://
        Public Const ATTRIBUTESTARTTOKEN_href_http_www As Byte = &HD ' http://www.
        Public Const ATTRIBUTESTARTTOKEN_href_https As Byte = &HE ' https://
        Public Const ATTRIBUTESTARTTOKEN_href_https_www As Byte = &HF ' https://www.
        Public Const ATTRIBUTESTARTTOKEN_si_expires As Byte = &H10
        Public Const ATTRIBUTESTARTTOKEN_si_id As Byte = &H11
        Public Const ATTRIBUTESTARTTOKEN_class As Byte = &H12

        ' Attribute Value Tokens
        Public Const ATTRIBUTEVALUETOKEN_com As Byte = &H85 ' .com/
        Public Const ATTRIBUTEVALUETOKEN_edu As Byte = &H86 ' .edu/
        Public Const ATTRIBUTEVALUETOKEN_net As Byte = &H87 ' .net/
        Public Const ATTRIBUTEVALUETOKEN_org As Byte = &H88 ' .org/
        Private Shared hrefStartTokens As Hashtable
        Private Shared attributeValueTokens As Hashtable

        Public Href As String
        Public text As String
        Public CreatedAt As DateTime
        Public ExpiresAt As DateTime
        Public Action As ServiceIndicationAction


        Shared Sub New()
            hrefStartTokens = New Hashtable()
            hrefStartTokens.Add("https://www.", ATTRIBUTESTARTTOKEN_href_https_www)
            hrefStartTokens.Add("http://www.", ATTRIBUTESTARTTOKEN_href_http_www)
            hrefStartTokens.Add("https://", ATTRIBUTESTARTTOKEN_href_https)
            hrefStartTokens.Add("http://", ATTRIBUTESTARTTOKEN_href_http)

            attributeValueTokens = New Hashtable()
            attributeValueTokens.Add(".com/", ATTRIBUTEVALUETOKEN_com)
            attributeValueTokens.Add(".edu/", ATTRIBUTEVALUETOKEN_edu)
            attributeValueTokens.Add(".net/", ATTRIBUTEVALUETOKEN_net)
            attributeValueTokens.Add(".org/", ATTRIBUTEVALUETOKEN_org)
        End Sub 'New


        Public Sub New(ByVal href As String, ByVal text As String, ByVal action As ServiceIndicationAction)
            Me.Href = href
            Me.Text = text
            Me.Action = action
        End Sub 'New


        Public Sub New(ByVal href As String, ByVal text As String, ByVal createdAt As DateTime, ByVal expiresAt As DateTime)
            MyClass.New(href, text, ServiceIndicationAction.NotSet)
            Me.CreatedAt = createdAt
            Me.ExpiresAt = expiresAt
        End Sub 'New


        Public Sub New(ByVal href As String, ByVal text As String, ByVal createdAt As DateTime, ByVal expiresAt As DateTime, ByVal action As ServiceIndicationAction)
            MyClass.New(href, text, action)
            Me.CreatedAt = createdAt
            Me.ExpiresAt = expiresAt
        End Sub 'New

        ' <summary>
        ' Gets the token for the action attribute
        ' </summary>
        ' <param name="action">Interruption level instruction to the handset</param>
        ' <returns>well known byte value for the action attribute</returns>
        Protected Function GetActionToken(ByVal action As ServiceIndicationAction) As Byte
            Select Case action
                Case ServiceIndicationAction.delete
                    Return ATTRIBUTESTARTTOKEN_action_signal_delete
                Case ServiceIndicationAction.signal_high
                    Return ATTRIBUTESTARTTOKEN_action_signal_high
                Case ServiceIndicationAction.signal_low
                    Return ATTRIBUTESTARTTOKEN_action_signal_low
                Case ServiceIndicationAction.signal_medium
                    Return ATTRIBUTESTARTTOKEN_action_signal_medium
                Case Else
                    Return ATTRIBUTESTARTTOKEN_action_signal_none
            End Select
        End Function 'GetActionToken

        ' <summary>
        ' Encodes the DateTime to the stream.
        ' DateTimes are encoded as Opaque Data with each number in the string represented
        ' by its 4-bit binary value
        ' eg: 1999-04-30 06:40:00
        ' is encoded as 199904300640.
        ' Trailing zero values are not included.
        ' </summary>
        ' <param name="stream">Target stream</param>
        ' <param name="date">DateTime to encode</param>
        Protected Sub WriteDate(ByVal stream As MemoryStream, ByVal [date] As DateTime)
            Dim buffer(7) As Byte

            buffer(0) = CByte([date].Year / 100)
            buffer(1) = CByte([date].Year Mod 100)
            buffer(2) = CByte([date].Month)
            buffer(3) = CByte([date].Day)

            Dim dateLength As Integer = 4

            If [date].Hour > 0 Then
                buffer(4) = CByte([date].Hour)
                dateLength = 5
            End If

            If [date].Minute > 0 Then
                buffer(5) = CByte([date].Minute)
                dateLength = 6
            End If

            If [date].Second > 0 Then
                buffer(6) = CByte([date].Second)
                dateLength = 7
            End If
        End Sub 'WriteDate
    End Class 'ServiceIndication 
End Namespace