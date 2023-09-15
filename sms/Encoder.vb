'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    SMS Encoder
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

    Namespace Encoder

        Public Class SMS

#Region "Protected Data"
            Protected SC_Number As String  'Note the plus!
            Protected TP_MTI As Byte = 1
            Protected TP_RD As Byte = 0
            Protected TP_VPF As Byte = 16
            Protected TP_UDHI As Byte
            Protected TP_SRR As Byte
            Protected TP_MR As Integer
            Protected TP_DA As String
            Protected TP_PID As Byte
            Protected TP_DCS As Byte
            Protected TP_VP As Byte
            Protected TP_UDL As Integer
            Protected TP_UD As String
#End Region

#Region "Properties"

            Public Property ServiceCenterNumber() As String
                Get
                    Return SC_Number
                End Get
                Set(ByVal Value As String)
                    If Value = "00" Or Len(Value) = 0 Then
                        SC_Number = "00"
                    Else
                        'Convert an ServiceCenterNumber to PDU Code
                        If InStr(Value, "+") Then
                            SC_Number = "91"
                        Else
                            SC_Number = "81"
                        End If

                        Value = Mid(Value, 2)
                        Dim i As Integer
                        If (Value.Length Mod 2) = 1 Then
                            Value += "F"
                        End If
                        For i = 1 To Value.Length Step 2
                            SC_Number += Swap(Mid(Value, i, 2))
                        Next
                        SC_Number = ByteToHex((SC_Number.Length - 2) / 2 + 1) + SC_Number
                    End If
                End Set

            End Property

            Public Property TP_Status_Report_Request() As ENUM_TP_SRI
                Get
                    Return TP_SRR
                End Get
                Set(ByVal Value As ENUM_TP_SRI)
                    TP_SRR = Value
                End Set
            End Property

            Public Property TP_Message_Reference() As Integer
                Get
                    Return TP_MR
                End Get
                Set(ByVal Value As Integer)
                    TP_MR = Value
                End Set
            End Property

            Public Property TP_Destination_Address() As String
                Get
                    Return TP_DA
                End Get
                Set(ByVal Value As String)
                    TP_DA = ""

                    If InStr(Value, "+") Then
                        TP_DA += "91"
                    Else
                        TP_DA += "81"
                    End If
                    Value = Value.Replace("+", "")
                    TP_DA = Format(Value.Length, "X2") + TP_DA
                    Dim i As Integer
                    If (Value.Length Mod 2) = 1 Then
                        Value += "F"
                    End If
                    For i = 1 To Value.Length Step 2
                        TP_DA += Swap(Mid(Value, i, 2))
                    Next
                End Set
            End Property


            Public Property TP_Data_Coding_Scheme() As ENUM_TP_DCS
                Get
                    Return TP_DCS
                End Get
                Set(ByVal Value As ENUM_TP_DCS)
                    TP_DCS = Value
                End Set
            End Property

            Public Property TP_Validity_Period() As ENUM_TP_VALID_PERIOD
                Get
                    Return TP_VP
                End Get
                Set(ByVal Value As ENUM_TP_VALID_PERIOD)
                    TP_VP = Value
                End Set
            End Property

            Public Overridable Property TP_User_Data() As String
                Get
                    Return TP_UD
                End Get
                Set(ByVal Value As String)
                    Select Case TP_DCS
                        Case Is = ENUM_TP_DCS.DefaultAlphabet
                            TP_UDL = Value.Length
                            TP_UD = Encode7Bit(Value)
                        Case Is = ENUM_TP_DCS.Class2_UD_7bits
                            TP_UDL = Value.Length
                            TP_UD = Encode7Bit(Value)
                        Case Is = ENUM_TP_DCS.UCS2
                            TP_UDL = Value.Length * 2
                            TP_UD = EncodeUCS2(Value)
                        Case Is = ENUM_TP_DCS.Class2_UD_8bits
                            TP_UDL = Value.Length * 2
                            TP_UD = EncodeUCS2(Value)
                        Case Else
                            TP_UD = Value
                    End Select
                End Set
            End Property

            Public Overridable Property TP_User_Data_Concat() As String
                Get
                    Return TP_UD
                End Get
                Set(ByVal Value As String)
                    Select Case TP_DCS
                        Case Is = ENUM_TP_DCS.DefaultAlphabet
                            TP_UDL = Value.Length
                            TP_UD = EncodeUserData(MapTo7BitAlphabet(Value))
                        Case Is = ENUM_TP_DCS.Class2_UD_7bits
                            TP_UDL = Value.Length
                            TP_UD = Encode7Bit(Value)
                        Case Is = ENUM_TP_DCS.UCS2
                            TP_UDL = Value.Length * 2
                            TP_UD = EncodeUCS2(Value)
                        Case Is = ENUM_TP_DCS.Class2_UD_8bits
                            TP_UDL = Value.Length * 2
                            TP_UD = EncodeUCS2(Value)
                        Case Else
                            TP_UD = Value
                    End Select
                End Set
            End Property
#End Region

#Region "Functions"
            Public Shared Function CheckForEncoding(ByVal Content As String) As ENUM_TP_DCS
                Dim i As Integer
                For i = 1 To Content.Length
                    If Asc(Mid(Content, i, 1)) < 0 Then
                        Return ENUM_TP_DCS.UCS2
                    End If
                Next
                Return ENUM_TP_DCS.DefaultAlphabet
            End Function

            Public Overridable Function GetSMSPDUCode() As String
                Dim PDUCode As String
                'Check User Data Length

                If TP_DCS = ENUM_TP_DCS.DefaultAlphabet Then
                    If TP_UD.Length > 280 Then Throw New System.Exception("User Data is TOO LONG for SMS")
                End If
                If TP_DCS = ENUM_TP_DCS.UCS2 Then
                    If TP_UD.Length > 280 Then Throw New System.Exception("User Data is TOO LONG for SMS")
                End If
                If TP_DCS = TP_DCS = ENUM_TP_DCS.Class2_UD_7bits Then
                    If TP_UD.Length > 280 Then Throw New System.Exception("User Data is TOO LONG for SMS")
                End If
                'Make PDUCode
                PDUCode = SC_Number
                PDUCode += FirstOctet()
                PDUCode += Format(TP_MR, "X2")
                PDUCode += TP_DA
                PDUCode += Format(TP_PID, "X2")
                PDUCode += Format(TP_DCS, "X2")
                PDUCode += Format(TP_VP, "X2")
                PDUCode += Format(TP_UDL, "X2")
                PDUCode += TP_UD
                Return PDUCode
            End Function

            Public Overridable Function GetSMSPDUCodeConcat() As String
                Dim PDUCode As String

                PDUCode = SC_Number
                PDUCode += "51"
                PDUCode += "00"
                PDUCode += TP_DA
                PDUCode += Format(TP_PID, "X2")
                PDUCode += Format(TP_DCS, "X2")
                PDUCode += Format(TP_VP, "X2")
                'PDUCode += Format(TP_UDL, "X2")
                PDUCode += TP_UD
                Debug.Print(PDUCode)
                Return PDUCode
            End Function

            Public Overridable Function FirstOctet() As String
                Return ByteToHex(TP_MTI + TP_VPF + TP_SRR + TP_UDHI)
            End Function

            Shared Function ByteToHex(ByVal aByte As Byte) As String
                Dim result As String
                result = Format(aByte, "X2")
                Return result
            End Function

#Region "Encode7Bit"
            Shared Function Encode7Bit(ByVal Content As String) As String
                'Prepare
                Dim CharArray As Char() = Content.ToCharArray
                Dim c As Char
                Dim t As String = String.Empty
                For Each c In CharArray
                    t = CharTo7Bits(c) + t
                Next
                'Add "0"
                Dim i As Integer
                If (t.Length Mod 8) <> 0 Then
                    For i = 1 To 8 - (t.Length Mod 8)
                        t = "0" + t
                    Next
                End If
                'Split into 8bits
                Dim result As String = ""
                For i = t.Length - 8 To 0 Step -8
                    result = result + BitsToHex(Mid(t, i + 1, 8))
                Next
                Return result
            End Function

            Shared Function BitsToHex(ByVal Bits As String) As String
                'Convert 8Bits to Hex String
                Dim i, v As Integer
                For i = 0 To Bits.Length - 1
                    v = v + Val(Mid(Bits, i + 1, 1)) * 2 ^ (7 - i)
                Next
                Dim result As String
                result = Format(v, "X2")
                Return result
            End Function

            Shared Function CharTo7Bits(ByVal c As Char) As String
                If c = "@" Then Return "0000000"
                Dim Result As String = ""
                Dim i As Integer
                For i = 0 To 6
                    If (Asc(c) And 2 ^ i) > 0 Then
                        Result = "1" + Result
                    Else
                        Result = "0" + Result
                    End If
                Next
                Return Result
            End Function

            Private Function EncodeUserData(ByVal strUserData As String) As String
                Dim lsres As String, lsres2 As String, encSMS As String
                Dim lenEmNr As Long, lenSMS As Long
                Dim encRecNr As String, intNrPrefix As String
                Dim lnCtr As Long

                lenSMS = Len(strUserData)

                'Convert to 7-bit bitstream, reverse bits
                For lnCtr = 0 To Len(strUserData) - 1
                    lsres = lsres + StrReverse(Bin(CLng(Asc(Mid$(strUserData, lnCtr + 1, 1))), 7))
                Next lnCtr

                'Check whether the string length can be divided by 8 without fractions.
                lnCtr = Len(lsres) And 7
                If lnCtr <> 0 Then
                    lsres = lsres & New String("0", 8 - lnCtr)
                End If

                For lnCtr = 0 To Len(lsres) - 1 Step 8
                    lsres2 = StrReverse(Mid$(lsres, lnCtr + 1, 8))
                    encSMS = encSMS & Hex(BinToDec(Left$(lsres2, 4))) & Hex(BinToDec(Right$(lsres2, 4)))
                Next lnCtr

                encSMS = Right$("00" & Hex(lenSMS), 2) & encSMS

                EncodeUserData = encSMS
            End Function

            'For EncodeUserData
            Private Function Bin(ByVal Value As Long, Optional ByVal Digits As Long = -1) As String
                Dim lnExponent As Integer
                Dim lsResult As New String("0", 32)
                
                Do
                    If Value And Power2(lnExponent) Then
                        ' we found a bit that is set, clear it
                        Mid$(lsResult, 32 - lnExponent, 1) = "1"
                        Value = Value Xor Power2(lnExponent)
                    End If

                    lnExponent = lnExponent + 1
                Loop While Value

                If Digits < 0 Then
                    ' trim non significant digits, if digits was omitted or negative
                    Bin = Mid$(lsResult, 33 - lnExponent)
                Else
                    ' else trim to the requested number of digits
                    Bin = Right$(lsResult, Digits)
                End If
            End Function

            Private Function Power2(ByVal exponent As Long) As Long
                Static lnRes(0 To 31) As Long
                Dim lnCtr As Long

                ' rule out errors
                If exponent < 0 Or exponent > 31 Then Err.Raise(5)

                ' initialize the array at the first call
                If lnRes(0) = 0 Then
                    lnRes(0) = 1
                    For lnCtr = 1 To 30
                        lnRes(lnCtr) = lnRes(lnCtr - 1) * 2
                    Next
                    ' this is a special case
                    lnRes(31) = &H80000000
                End If

                ' return the result
                Power2 = lnRes(exponent)
            End Function

            Private Function BinToDec(ByVal Value As String) As Long
                Dim lsResult As Long, lnCtr As Integer, lnExponent As Integer

                For lnCtr = Len(Value) To 1 Step -1
                    Select Case Asc(Mid$(Value, lnCtr, 1))
                        Case 48      ' "0", do nothing
                        Case 49      ' "1", add the corresponding power of 2
                            lsResult = lsResult Or Power2(lnExponent)
                        Case Else
                            Err.Raise(5)      ' Invalid procedure call or argument
                    End Select
                    lnExponent = lnExponent + 1
                Next

                BinToDec = lsResult
            End Function

            Private Function MapTo7BitAlphabet(ByVal strData As String)
                Dim lsNewData As String
                Dim lsNewChar As String
                Dim blnEscapeChar As Boolean
                Dim lnCtr As Long

                lsNewData = ""
                For lnCtr = 1 To Len(strData)
                    lsNewChar = Chr(MapChar(Mid(strData, lnCtr, 1), blnEscapeChar))
                    If blnEscapeChar Then lsNewData = lsNewData & Chr(27)
                    lsNewData = lsNewData & lsNewChar
                Next

                MapTo7BitAlphabet = lsNewData
            End Function

            Private Function MapChar(ByVal lValue As String, ByRef blnEscapeChar As Boolean) As Byte
                Dim retval As Byte

                If Len(lValue) > 1 Then
                    lValue = Left(lValue, 1)
                ElseIf Len(lValue) < 1 Then
                    Err.Raise(5) 'invalid
                    Exit Function
                End If

                '   Case 10: GSM7ToAscii = "\n"
                '   Case 13: GSM7ToAscii = "\r"
                '   Case 16: GSM7ToAscii = "\u0394"
                '   Case 18: GSM7ToAscii = "\u03a6"
                '   Case 19: GSM7ToAscii = "\u0393"
                '   Case 20: GSM7ToAscii = "\u039b"
                '   Case 21: GSM7ToAscii = "\u03a9"
                '   Case 22: GSM7ToAscii = "\u03a0"
                '   Case 23: GSM7ToAscii = "\u03a8"
                '   Case 24: GSM7ToAscii = "\u03a3"
                '   Case 25: GSM7ToAscii = "\u0398"
                '   Case 26: GSM7ToAscii = "\u039e"
                '   Case 27: GSM7ToAscii = "&#8364;"

                blnEscapeChar = False
                Select Case lValue
                    Case "@" : retval = 0
                    Case "£" : retval = 1
                    Case "$" : retval = 2
                    Case "¥" : retval = 3
                    Case "è" : retval = 4
                    Case "é" : retval = 5
                    Case "ú" : retval = 6
                    Case "ì" : retval = 7
                    Case "ò" : retval = 8
                    Case "Ç" : retval = 9
                    Case "Ø" : retval = 11
                    Case "ø" : retval = 12
                    Case "Å" : retval = 14
                    Case "å" : retval = 15
                    Case "_" : retval = 17
                    Case "^" : retval = 20 : blnEscapeChar = True
                    Case "Æ" : retval = 28
                    Case "æ" : retval = 29
                    Case "ß" : retval = 30
                    Case "Ê" : retval = 31
                    Case "¤" : retval = 36
                    Case "{" : retval = 40 : blnEscapeChar = True
                    Case "}" : retval = 41 : blnEscapeChar = True
                    Case "\" : retval = 47 : blnEscapeChar = True
                    Case "[" : retval = 60 : blnEscapeChar = True
                    Case "~" : retval = 61 : blnEscapeChar = True
                    Case "]" : retval = 62 : blnEscapeChar = True
                    Case "|" : retval = 64 : blnEscapeChar = True
                    Case "¡" : retval = 64
                    Case "Ä" : retval = 91
                    Case "Ö" : retval = 92
                    Case "Ñ" : retval = 93
                    Case "Ü" : retval = 94
                    Case "§" : retval = 95
                    Case "¿" : retval = 96
                    Case "€" : retval = 101 : blnEscapeChar = True
                    Case "ä" : retval = 123
                    Case "ö" : retval = 124
                    Case "ñ" : retval = 125
                    Case "ü" : retval = 126
                    Case "à" : retval = 127
                    Case Else : retval = Asc(lValue)
                End Select

                MapChar = retval
            End Function
#End Region

            Shared Function EncodeUCS2(ByVal Content As String) As String
                Dim i, v As Integer
                Dim result, t As String
                result = ""
                For i = 1 To Content.Length
                    v = AscW(Mid(Content, i, 4))
                    t = Format(v, "X4")
                    result += t
                Next
                Return result
            End Function

            Shared Function Swap(ByRef TwoBitStr As String) As String
                'Swap two bit like "EF" TO "FE"
                Dim c() As Char = TwoBitStr.ToCharArray
                Dim t As Char
                t = c(0)
                c(0) = c(1)
                c(1) = t
                Return (c(0) + c(1)).ToString
            End Function
#End Region
        End Class

        Public Class ConcatenatedShortMessage
            Inherits SMS
            Private TotalMessages As Integer

            Public Function GetEMSPDUCode() As String()
                Select Case TP_DCS
                    Case ENUM_TP_DCS.UCS2
                        TotalMessages = (TP_UD.Length / 4) \ 66 + ((TP_UD.Length / 4 Mod 66) = 0)
                    Case ENUM_TP_DCS.DefaultAlphabet
                        TotalMessages = (TP_UD.Length \ 266) - ((TP_UD.Length Mod 266) = 0)
                End Select

                Dim Result(TotalMessages) As String
                Dim tmpTP_UD As String = String.Empty
                Dim i As Integer
                TP_UDHI = 2 ^ 6
                Dim Reference As Integer = Rnd(1) * 65536   '16bit Reference Number 'See 3GPP Document
                For i = 0 To TotalMessages
                    Select Case TP_DCS
                        Case ENUM_TP_DCS.UCS2
                            tmpTP_UD = Mid(TP_UD, i * 66 * 4 + 1, 66 * 4)
                            'When TP_UDL is odd, the max length of an Unicode string in PDU code is 66 Charactor.See [3GPP TS 23.040 V6.5.0 (2004-09] 9.2.3.24.1
                        Case ENUM_TP_DCS.DefaultAlphabet
                            tmpTP_UD = Mid(TP_UD, i * 133 * 2 + 1, 133 * 2)
                    End Select
                    Result(i) = SC_Number
                    Result(i) += FirstOctet()
                    Result(i) += Format(TP_MR, "X2")
                    'Next segement TP_MR must be increased
                    'TP_MR += 1
                    Result(i) += TP_DA
                    Result(i) += Format(TP_PID, "X2")
                    Result(i) += Format(TP_DCS, "X2")
                    Result(i) += Format(TP_VP, "X2")
                    If TP_DCS = ENUM_TP_DCS.UCS2 Then
                        TP_UDL = tmpTP_UD.Length / 2 + 6 + 1 '6:IE
                    End If
                    If TP_DCS = ENUM_TP_DCS.DefaultAlphabet Then
                        TP_UDL = Fix((tmpTP_UD.Length + 7 * 2) * 4 / 7)   '6:length of IE
                        ''#################################
                        ''still problem here:
                        ''when the charcter is several times of 7 of the last message, tp_udl will not correct!
                        ''to correct this problem i write some code below. that's may not perfect solution.
                        ''#################################
                        'If i = TotalMessages And ((tmpTP_UD.Length Mod 14) = 0) Then
                        '    tp_udl -= 1
                        'End If
                    End If
                    Result(i) += Format(TP_UDL, "X2")
                    Result(i) += "060804" 'TP_UDHL and some of Concatenated message
                    Result(i) += Format(Reference, "X4")
                    Result(i) += Format(TotalMessages + 1, "X2")
                    Result(i) += Format(i + 1, "X2")
                    Result(i) += tmpTP_UD
                Next
                Return Result
            End Function

            Public Overrides Function FirstOctet() As String
                Return ByteToHex(TP_MTI + TP_VPF + TP_SRR + TP_UDHI)
            End Function
        End Class
    End Namespace
End Namespace