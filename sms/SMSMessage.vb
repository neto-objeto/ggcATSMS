'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    SMS SMSMessage
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
Namespace SMS

    Public Class SMSMessage

#Region "Private Members"

        Private iIndex As Integer
        Private strPhoneNumber As String
        Private strSMSC As String
        Private strText As String
        Private strData As String
        Private dtTimestamp As DateTime
        Private strTimeStampRFC As String
        Private oGSMModem As GSMModem
        Private strTimeZone As String  ' Message timezone
        Private iEMSTotolPiece As Integer
        Private iEMSCurrentPiece As Integer

#End Region

#Region "Public Properties"

        Public Property Index() As Integer
            Get
                Return Me.iIndex
            End Get
            Friend Set(ByVal value As Integer)
                Me.iIndex = value
            End Set
        End Property

        Public Property EMSTotolPiece() As Integer
            Get
                Return Me.iEMSTotolPiece
            End Get
            Friend Set(ByVal value As Integer)
                Me.iEMSTotolPiece = value
            End Set
        End Property

        Public Property EMSCurrentPiece() As Integer
            Get
                Return Me.iEMSCurrentPiece
            End Get
            Friend Set(ByVal value As Integer)
                Me.iEMSCurrentPiece = value
            End Set
        End Property

        Public Property PhoneNumber() As String
            Get
                Return Me.strPhoneNumber
            End Get
            Friend Set(ByVal value As String)
                Me.strPhoneNumber = value
            End Set
        End Property

        Public Property SMSC() As String
            Get
                Return Me.strSMSC
            End Get
            Friend Set(ByVal value As String)
                Me.strSMSC = value
            End Set
        End Property

        Public Property Text() As String
            Get
                Return Me.strText
            End Get
            Friend Set(ByVal value As String)
                Me.strText = value
            End Set
        End Property

        Public Property Data() As String
            Get
                Return Me.strData
            End Get
            Friend Set(ByVal value As String)
                Me.strData = value
            End Set
        End Property

        Public Property Timestamp() As DateTime
            Get
                Return Me.dtTimestamp
            End Get
            Friend Set(ByVal value As DateTime)
                Me.dtTimestamp = value
            End Set
        End Property

        Public Property TimeZone() As String
            Get
                Return Me.strTimeZone
            End Get
            Set(ByVal value As String)
                Me.strTimeZone = value
            End Set
        End Property

        Public Property TimestampRFC() As String
            Get
                Return Me.strTimeStampRFC
            End Get
            Friend Set(ByVal value As String)
                Me.strTimeStampRFC = value
            End Set
        End Property

        Public Property Modem() As GSMModem
            Get
                Return Me.oGSMModem
            End Get
            Set(ByVal value As GSMModem)
                Me.oGSMModem = value
            End Set
        End Property

#End Region

#Region "Public Procedures"

        Public Sub Delete()
            If Not oGSMModem Is Nothing And oGSMModem.IsConnected Then
                oGSMModem.DeleteSMS(Me.Index)
            End If

        End Sub

#End Region
    End Class

End Namespace
