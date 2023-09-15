'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    Modem Location
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
''' Location class
''' </summary>
''' <remarks></remarks>
Public Class Location
    Private strMCC As String
    Private strMNC As String
    Private strLAI As String
    Private strCellID As String

    Public Property MCC() As String
        Get
            Return Me.strMCC
        End Get
        Set(ByVal value As String)
            Me.strMCC = value
        End Set
    End Property

    Public Property MNC() As String
        Get
            Return Me.strMNC
        End Get
        Set(ByVal value As String)
            Me.strMNC = value
        End Set
    End Property

    Public Property LAI() As String
        Get
            Return Me.strLAI
        End Get
        Set(ByVal value As String)
            Me.strLAI = value
        End Set
    End Property

    Public Property CellID() As String
        Get
            Return Me.strCellID
        End Get
        Set(ByVal value As String)
            Me.strCellID = value
        End Set
    End Property
End Class

