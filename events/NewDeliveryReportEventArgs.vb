'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    Events NewDeliveryReportEventArgs
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
Public Class NewDeliveryReportEventArgs

    Private dtmDeliveryTimeStamp As DateTime
    Private iMessageReference As Integer
    Private strPhone As String
    Private dtmSentTimeStamp As DateTime
    Private blnStatus As Boolean

    Public Property DeliveryDateTime() As DateTime
        Get
            Return Me.dtmDeliveryTimeStamp
        End Get
        Set(ByVal Value As DateTime)
            Me.dtmDeliveryTimeStamp = Value
        End Set
    End Property

    Public Property MessageReference() As Integer
        Get
            Return Me.iMessageReference
        End Get
        Set(ByVal Value As Integer)
            Me.iMessageReference = Value
        End Set
    End Property

    Public Property Phone() As String
        Get
            Return Me.strPhone
        End Get
        Set(ByVal Value As String)
            Me.strPhone = Value
        End Set
    End Property

    Public Property SentTimeStamp() As DateTime
        Get
            Return Me.dtmSentTimeStamp
        End Get
        Set(ByVal Value As DateTime)
            Me.dtmSentTimeStamp = Value
        End Set
    End Property

    Public Property Status() As Boolean
        Get
            Return Me.blnStatus
        End Get
        Set(ByVal Value As Boolean)
            Me.blnStatus = Value
        End Set
    End Property

End Class
