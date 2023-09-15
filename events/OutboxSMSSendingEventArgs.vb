'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    Events OutboxSMSSendingEventArgs
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
Imports ATSMS.Common

Public Class OutboxSMSSendingEventArgs

#Region "Private Members"
    Private strDestinationNumber As String
    Private enumPriority As EnumQueuePriority
    Private strMsgNo As String
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

    Public Property Priority() As EnumQueuePriority
        Get
            Return Me.enumPriority
        End Get
        Set(ByVal value As EnumQueuePriority)
            Me.enumPriority = value
        End Set
    End Property

    Public Property QueueMessageKey() As String
        Get
            Return Me.strMsgNo
        End Get
        Set(ByVal value As String)
            Me.strMsgNo = value
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
