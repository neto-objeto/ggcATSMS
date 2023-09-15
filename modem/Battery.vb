'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    Modem Battery
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
''' Battery class
''' </summary>
''' <remarks></remarks>
Public Class Battery

    Private iBatteryLevel As Integer
    Private iMinLevel As Integer
    Private iMaxLevel As Integer
    Private bBatteryCharged As Boolean

    Public Property BatteryLevel() As Integer
        Get
            Return Me.iBatteryLevel
        End Get
        Set(ByVal value As Integer)
            Me.iBatteryLevel = value
        End Set
    End Property

    Public Property MinimumLevel() As Integer
        Get
            Return Me.iMinLevel
        End Get
        Set(ByVal value As Integer)
            Me.iMinLevel = value
        End Set
    End Property

    Public Property MaximumLevel() As Integer
        Get
            Return Me.iMaxLevel
        End Get
        Set(ByVal value As Integer)
            Me.iMaxLevel = value
        End Set
    End Property

    Public Property BatteryCharged() As Boolean
        Get
            Return Me.bBatteryCharged
        End Get
        Set(ByVal value As Boolean)
            Me.bBatteryCharged = value
        End Set
    End Property

End Class

