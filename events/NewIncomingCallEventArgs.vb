'�������������������������������������������������������������������������������������������
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    Events NewIncomingCallEventArgs
'
' Copyright 2013 and Beyond
' All Rights Reserved
' ������������������������������������������������������������������������������������������
' �  All  rights reserved. No part of this  software  ��  This Software is Owned by        �
' �  may be reproduced or transmitted in any form or  ��                                   �
' �  by   any   means,  electronic   or  mechanical,  ��    GUANZON MERCHANDISING CORP.    �
' �  including recording, or by information  storage  ��     Guanzon Bldg. Perez Blvd.     �
' �  and  retrieval  systems, without  prior written  ��           Dagupan City            �
' �  from the author.                                 ��  Tel No. 522-1085 ; 522-9275      �
' ������������������������������������������������������������������������������������������
'
' ==========================================================================================
'  Jheff [ 01/12/2013 09:05 am ]
'      Start this object
'�������������������������������������������������������������������������������������������
Public Class NewIncomingCallEventArgs

    Private strCallerID As String
    Private bolRingingx As Boolean
    Private bolAnswered As Boolean
    Private strStatusxx As String

    Public Property CallerID() As String
        Get
            Return Me.strCallerID
        End Get
        Set(ByVal value As String)
            Me.strCallerID = value
        End Set
    End Property

    Public Property Ringing() As Boolean
        Get
            Return Me.bolRingingx
        End Get
        Set(ByVal Value As Boolean)
            Me.bolRingingx = Value
        End Set
    End Property

    Public Property Answered() As Boolean
        Get
            Return Me.bolAnswered
        End Get
        Set(ByVal Value As Boolean)
            Me.bolAnswered = Value
        End Set
    End Property

    Public Property Status() As String
        Get
            Return Me.strStatusxx
        End Get
        Set(ByVal Value As String)
            Me.strStatusxx = Value
        End Set
    End Property
End Class
