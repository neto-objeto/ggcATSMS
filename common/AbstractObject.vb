'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    Common AbstractObject
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
Namespace Common

    Public MustInherit Class AbstractObject

        Protected strAbout As String
        Protected isDebugMode As Boolean
        Protected strLogFolderPath As String
        Protected lLogSize As Long
        Protected enumLogType As EnumLogType
        Protected isAnyError As Boolean

#Region "Properties"
        Public ReadOnly Property About() As String
            Get
                Return Resource.GetMessage("ABOUT")
            End Get
        End Property

        Public Property DebugMode() As Boolean
            Get
                Return Me.isDebugMode
            End Get
            Set(ByVal Value As Boolean)
                Me.isDebugMode = Value
            End Set
        End Property

        Public Property LogFolderPath() As String
            Get
                Return Me.strLogFolderPath
            End Get
            Set(ByVal Value As String)
                Me.strLogFolderPath = Value
            End Set
        End Property

        Public ReadOnly Property LogSize() As Long
            Get
                Return Me.lLogSize
            End Get
        End Property

        Public Property LogType() As EnumLogType
            Get
                Return Me.enumLogType
            End Get
            Set(ByVal Value As EnumLogType)
                Me.enumLogType = Value
            End Set
        End Property
#End Region

#Region "Functions"

        Public Function IsError() As Boolean
            IsError = Me.isAnyError
        End Function

        Public Function IsError(ByVal showMsgBox As Boolean) As Boolean
            IsError = Me.isAnyError
        End Function

        Public Function IsError(ByVal showMsgBox As Boolean, _
                ByVal msgBoxTitle As String) As Boolean
            IsError = Me.isAnyError
        End Function

#End Region
    End Class

End Namespace
