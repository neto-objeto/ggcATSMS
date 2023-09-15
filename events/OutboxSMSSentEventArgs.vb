'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    Events OutboxSMSSentEventArgs
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
Public Class OutboxSMSSentEventArgs

#Region "Private Members"

    Private strDestinationNumber As String
    Private lngErrorCode As Long
    Private strErrorDescription As String
    Private strMessageReference As String
    Private strQueueMessageKey As String
    Private blnSendResult As Boolean
    Private strMsg As String

#End Region

#Region "Properties"

    Public Property DestinationNumber() As String
        Get
            Return Me.strDestinationNumber
        End Get
        Set(ByVal value As String)
            Me.strDestinationNumber = value
        End Set
    End Property

    Public Property ErrorCode() As Long
        Get
            Return Me.lngErrorCode
        End Get
        Set(ByVal value As Long)
            Me.lngErrorCode = value
        End Set
    End Property

    Public Property ErrorDescription() As String
        Get
            Return Me.strErrorDescription
        End Get
        Set(ByVal value As String)
            Me.strErrorDescription = value
        End Set
    End Property

    Public Property MessageReference() As String
        Get
            Return Me.strMessageReference
        End Get
        Set(ByVal value As String)
            Me.strMessageReference = value
        End Set
    End Property

    Public Property QueueMessageKey() As String
        Get
            Return Me.strQueueMessageKey
        End Get
        Set(ByVal value As String)
            Me.strQueueMessageKey = value
        End Set
    End Property

    Public Property SendResult() As Boolean
        Get
            Return Me.blnSendResult
        End Get
        Set(ByVal value As Boolean)
            Me.blnSendResult = value
        End Set
    End Property

    Public Property Message() As String
        Get
            Return strMsg
        End Get
        Set(ByVal value As String)
            strMsg = value
        End Set
    End Property

#End Region

End Class
