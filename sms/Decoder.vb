'€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€
' Guanzon Software Engineering Group
' Guanzon Group of Companies
' Perez Blvd., Dagupan City
'
'    SMS Decoder
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
Imports System.Text

Namespace SMS

    Namespace Decoder

        Public MustInherit Class SMSBase

            'Note all of following various with TP_ can be found in GSM 03.40
            Public SCAddressLength As Byte  'Service Center Address length
            Public SCAddressType As Byte    'Service Center Type[See GSM 03.40]
            Public SCAddressValue As String 'Service Center nuber
            Public FirstOctet As Byte       'See GSM 03.40

            Public TP_PID As Byte
            Public TP_DCS As Byte
            Public TP_UDL As Byte
            Public TP_UDH As String
            Public TP_UD As String
            Public Text As String
            Public Type As SMSType
            Public UserData As String

            Public MustOverride Sub GetOrignalData(ByVal PDUCode As String)

            'Get a byte from PDU string
            Shared Function GetByte(ByRef PDUCode As String) As Byte
                Dim r As Byte = Val("&H" + Mid(PDUCode, 1, 2))
                PDUCode = Mid(PDUCode, 3)
                Return r
            End Function

            'Get a string of certain length
            Shared Function GetString(ByRef PDUCode As String, ByVal Length As Integer) As String
                Dim r As String = Mid(PDUCode, 1, Length)
                PDUCode = Mid(PDUCode, Length + 1)
                Return r
            End Function

            'Get date from SCTS format
            Shared Function GetDate(ByRef SCTS As String, ByRef tz As String) As Date
                Dim year, month, day, hour, minute, second, timezone As Integer

                year = Val(Swap(GetString(SCTS, 2))) + 2000
                month = Val(Swap(GetString(SCTS, 2)))
                day = Val(Swap(GetString(SCTS, 2)))
                hour = Val(Swap(GetString(SCTS, 2)))
                minute = Val(Swap(GetString(SCTS, 2)))
                second = Val(Swap(GetString(SCTS, 2)))
                timezone = Val(Swap(GetString(SCTS, 2)))

                If timezone >= 0 Then
                    tz = "GMT+" & timezone / 4
                Else
                    tz = "GMT-" & timezone / 4
                End If

                Dim result As New Date(year, month, day, hour, minute, second)
                Return result
            End Function

            'Swap two bit
            Shared Function Swap(ByRef TwoBitStr As String) As String
                Dim c() As Char = TwoBitStr.ToCharArray
                Dim t As Char
                t = c(0)
                c(0) = c(1)
                c(1) = t
                Return (c(0) + c(1)).ToString
            End Function

            'Get phone address
            Shared Function GetAddress(ByRef Address As String) As String
                Dim tmpChar As Char() = Address.ToCharArray
                Dim i As Integer, result As String
                result = ""
                For i = 0 To tmpChar.GetUpperBound(0) Step 2
                    result += Swap(tmpChar(i) + tmpChar(i + 1))
                Next
                If InStr(result, "F") Then result = Mid(result, 1, result.Length - 1)
                Return result
            End Function

            Shared Function GetSMSType(ByVal PDUCode As String) As SMS.SMSType
                'Get first october
                Dim FirstOctet As Byte

                Dim L As Integer = SMSBase.GetByte(PDUCode)
                SMSBase.GetByte(PDUCode)
                SMSBase.GetString(PDUCode, (L - 1) * 2)
                FirstOctet = SMSBase.GetByte(PDUCode)
                '
                'Get base code. Use last 2 bit and whether there's a header as remark
                Dim t1 As Integer = FirstOctet And 3 '00000011
                Dim t2 As Integer = FirstOctet And 64 '01000000
                '
                If t1 = 3 And t2 = 64 Then Return SMS.SMSType.EMS_SUBMIT
                Return t1 + t2
            End Function

            'Decode a unicode string
            Shared Function DecodeUnicode(ByVal strUnicode As String) As String
                Dim code As String = ""
                Dim j As Integer
                Dim c() As String       'temp
                ReDim c(strUnicode.Length / 4)     '2 Byte a Unicode char

                For j = 0 To strUnicode.Length \ 4 - 1
                    Dim d() As Char = strUnicode.ToCharArray(j * 4, 4)
                    c(j) = "&H" & CType(d, String)
                    c(j) = ChrW(Val(c(j)))
                    code += c(j)
                Next
                Return code
            End Function

#Region "'Decode 7Bit to english"
            'Fixed decode "@" charactor
            'I use a new method, it is easily to understand but look much longer than before.
            Shared Function InvertHexString(ByVal HexString As String) As String
                'For example:
                '123456
                '===>
                '563412
                Dim Result As New StringBuilder
                Dim i As Integer

                For i = HexString.Length - 2 To 0 Step -2
                    Result.Append(HexString.Substring(i, 2))
                Next
                Return Result.ToString
            End Function

            Shared Function ByteToBinary(ByVal Dec As Byte) As String
                Dim result As String = ""
                Dim temp As Byte = Dec
                Do
                    result = (temp Mod 2) & result
                    If temp = 1 Or temp = 0 Then Exit Do
                    temp = temp \ 2
                Loop
                result = result.PadLeft(8, "0")
                Return result
            End Function

            Shared Function BinaryToInt(ByVal Binary As String) As Integer
                Dim Result As Integer
                Dim i As Integer
                For i = 0 To Binary.Length - 1
                    Result = Result + Val(Binary.Substring(Binary.Length - i - 1, 1)) * 2 ^ i
                Next
                Return Result
            End Function

            Shared Function Decode7Bit(ByVal str7BitCode As String, ByVal charCount As Integer) As String
                Dim inv7BitCode As String = InvertHexString(str7BitCode)
                Dim binary As String = ""
                Dim result As String = ""
                Dim i As Integer
                For i = 0 To inv7BitCode.Length - 1 Step 2
                    binary += ByteToBinary(Val("&H" & inv7BitCode.Substring(i, 2)))
                Next
                Dim Temp As Integer
                For i = 1 To charCount
                    Temp = BinaryToInt(binary.Substring(binary.Length - i * 7, 7))
                    'There is a problem:
                    '"@" charactor is decoded to binary "0000000", but its Ascii Code is 64!!
                    'Don't know what to do with it,maybe it is a bug!
                    If Temp = 0 Then Temp = 64
                    result = result + ChrW(Temp)
                Next
                Return (result)
            End Function

            'Shared Function Decode7Bit(ByVal str7BitCode As String, ByVal charCount As Integer) As String
            '    Dim inv7BitCode As String = InvertHexString(str7BitCode)
            '    Dim binary As String = ""
            '    Dim result As String = ""
            '    Dim i As Integer

            '    For i = 0 To inv7BitCode.Length - 1 Step 2
            '        binary += ByteToBinary(Val("&H" & inv7BitCode.Substring(i, 2)))
            '    Next

            '    charCount = charCount + 1
            '    binary = binary.PadRight(charCount * 7, "0")

            '    Dim Temp As Integer
            '    For i = 1 To charCount
            '        Temp = BinaryToInt(binary.Substring(binary.Length - i * 7, 7))
            '        'There is a problem:
            '        '"@" charactor is decoded to binary "0000000", but its Ascii Code is 64!!
            '        'Don't know what to do with it,maybe it is a bug!
            '        If Temp = 0 Then Temp = 64
            '        result = result + ChrW(Temp)
            '    Next

            '    If Mid(result, 1, 1) = "@" Then
            '        result = Mid(result, 2, result.Length - 1)
            '    End If

            '    If Mid(result, Len(result), 1) = "@" Then
            '        result = Mid(result, 1, Len(result) - 1)
            '    End If

            '    Return (result)
            'End Function

            Shared Function Decode7BitSMS(ByVal str7BitCode As String, ByVal charCount As Integer) As String
                'Dim inv7BitCode As String = InvertHexString(str7BitCode)
                Dim inv7BitCode As String = str7BitCode
                Dim binary As String = ""
                Dim septet As String = ""
                Dim result As String = ""
                Dim i As Integer
                Dim count = 0
                Dim octetArray()
                Dim restArray()
                Dim septetsArray()
                Dim s = 1

                For i = 0 To inv7BitCode.Length - 1 Step 2
                    binary += ByteToBinary(Val("&H" & inv7BitCode.Substring(i, 2)))
                Next

                For i = 0 To binary.Length - 1
                    ReDim Preserve octetArray(count)
                    octetArray(count) = binary.Substring(i, i + 8)

                    ReDim Preserve restArray(count)
                    restArray(count) = octetArray(count).ToString.Substring(0, (s Mod 8))

                    ReDim Preserve septetsArray(count)
                    septetsArray(count) = octetArray(count).ToString.Substring((s Mod 8), 8 - 1)

                    s += 1
                    count += 1
                    If (s = 8) Then
                        s = 1
                    End If
                    i = i + 8
                Next

                Dim Temp As Integer
                For i = 1 To charCount
                    Temp = BinaryToInt(binary.Substring(binary.Length - (i * 7), 7))
                    'There is a problem:
                    '"@" charactor is decoded to binary "0000000", but its Ascii Code is 64!!
                    'Don't know what to do with it,maybe it is a bug!
                    If Temp = 0 Then Temp = 64
                    result = result + ChrW(Temp)
                Next

                Return (result)
            End Function

            Shared Function DecodeSevenBit(ByVal input, ByVal truelength)
                Dim byteString = ""
                Dim octetArray()
                Dim restArray()
                Dim septetsArray()

                Dim s = 1
                Dim count = 0
                Dim matchcount = 0
                Dim smsMessage = ""
                Dim i = 0

                Try
                    For i = 0 To input.Length - 1 Step 2
                        byteString += ByteToBinary(Val("&H" & input.Substring(i, 2)))
                    Next

                    i = 0
                    For i = 0 To byteString.Length - 1
                        ReDim Preserve octetArray(count)
                        octetArray(count) = byteString.Substring(i, 8)

                        ReDim Preserve restArray(count)
                        restArray(count) = octetArray(count).ToString.Substring(0, (s Mod 8))

                        ReDim Preserve septetsArray(count)
                        septetsArray(count) = octetArray(count).ToString.Substring((s Mod 8), 8 - s)

                        s += 1
                        count += 1
                        If (s = 8) Then
                            s = 1
                        End If
                        i = i + 7
                    Next

                    For i = 0 To restArray.Length - 1
                        If i Mod 7 = 0 Then
                            If i <> 0 Then
                                smsMessage = smsMessage + sevenbitdefault(BinaryToInt(restArray(i - 1)))
                                matchcount += 1
                            End If

                            smsMessage = smsMessage + sevenbitdefault(BinaryToInt(septetsArray(i)))
                            matchcount += 1
                        Else
                            smsMessage = smsMessage + sevenbitdefault(BinaryToInt(septetsArray(i) + restArray(i - 1)).ToString)
                            matchcount += 1
                        End If
                    Next

                    If matchcount <> truelength Then
                        smsMessage = smsMessage + sevenbitdefault(BinaryToInt(restArray(i - 1)).ToString)
                    End If
                Catch ex As Exception
                    Return String.Empty
                End Try

                Return smsMessage
            End Function

            Shared Function sevenbitdefault(ByVal Index As Integer) As String
                Dim lsValue As String

                lsValue = ""
                Select Case Index
                    Case 0 : lsValue = "@"
                    Case 1 : lsValue = "£"
                    Case 2 : lsValue = "$"
                    Case 3 : lsValue = "¥"
                    Case 4 : lsValue = "è"
                    Case 5 : lsValue = "é"
                    Case 6 : lsValue = "ù"
                    Case 7 : lsValue = "ì"
                    Case 8 : lsValue = "ò"
                    Case 9 : lsValue = "Ç"
                    Case 10 : lsValue = "\n"
                    Case 11 : lsValue = "Ø"
                    Case 12 : lsValue = "ø"
                    Case 13 : lsValue = "\r"
                    Case 14 : lsValue = "Å"
                    Case 15 : lsValue = "å"
                        'Case 16 : lsValue = "\u0394"
                    Case 17 : lsValue = "_"
                        ' Case 18 : lsValue = "\u03a6"
                        ' Case 19 : lsValue = "\u0393"
                        ' Case 20 : lsValue = "\u039b"
                        'Case 21 : lsValue = "\u03a9"
                        'Case 22 : lsValue = "\u03a0"
                        'Case 23 : lsValue = "\u03a8"
                        'Case 24 : lsValue = "\u03a3"
                        'Case 25 : lsValue = "\u0398"
                        'Case 26 : lsValue = "\u039e"
                    Case 27 : lsValue = "€"
                    Case 28 : lsValue = "Æ"
                    Case 29 : lsValue = "æ"
                    Case 30 : lsValue = "ß"
                    Case 31 : lsValue = "É"
                    Case 32 : lsValue = " "
                    Case 33 : lsValue = "!"
                    Case 34 : lsValue = """"
                    Case 35 : lsValue = "#"
                    Case 36 : lsValue = "¤"
                    Case 37 : lsValue = "%"
                    Case 38 : lsValue = "&"
                    Case 39 : lsValue = "\"
                    Case 40 : lsValue = "("
                    Case 41 : lsValue = ")"
                    Case 42 : lsValue = "*"
                    Case 43 : lsValue = "+"
                    Case 44 : lsValue = ","
                    Case 45 : lsValue = "-"
                    Case 46 : lsValue = "."
                    Case 47 : lsValue = "/"
                    Case 48 : lsValue = "0"
                    Case 49 : lsValue = "1"
                    Case 50 : lsValue = "2"
                    Case 51 : lsValue = "3"
                    Case 52 : lsValue = "4"
                    Case 53 : lsValue = "5"
                    Case 54 : lsValue = "6"
                    Case 55 : lsValue = "7"
                    Case 56 : lsValue = "8"
                    Case 57 : lsValue = "9"
                    Case 58 : lsValue = ":"
                    Case 59 : lsValue = ";"
                    Case 60 : lsValue = "<"
                    Case 61 : lsValue = "="
                    Case 62 : lsValue = ">"
                    Case 63 : lsValue = "?"
                    Case 64 : lsValue = "¡"
                    Case 65 : lsValue = "A"
                    Case 66 : lsValue = "B"
                    Case 67 : lsValue = "C"
                    Case 68 : lsValue = "D"
                    Case 69 : lsValue = "E"
                    Case 70 : lsValue = "F"
                    Case 71 : lsValue = "G"
                    Case 72 : lsValue = "H"
                    Case 73 : lsValue = "I"
                    Case 74 : lsValue = "J"
                    Case 75 : lsValue = "K"
                    Case 76 : lsValue = "L"
                    Case 77 : lsValue = "M"
                    Case 78 : lsValue = "N"
                    Case 79 : lsValue = "O"
                    Case 80 : lsValue = "P"
                    Case 81 : lsValue = "Q"
                    Case 82 : lsValue = "R"
                    Case 83 : lsValue = "S"
                    Case 84 : lsValue = "T"
                    Case 85 : lsValue = "U"
                    Case 86 : lsValue = "V"
                    Case 87 : lsValue = "W"
                    Case 88 : lsValue = "X"
                    Case 89 : lsValue = "Y"
                    Case 90 : lsValue = "Z"
                    Case 91 : lsValue = "Ä"
                    Case 92 : lsValue = "Ö"
                    Case 93 : lsValue = "Ñ"
                    Case 94 : lsValue = "Ü"
                    Case 95 : lsValue = "§"
                    Case 96 : lsValue = "¿"
                    Case 97 : lsValue = "a"
                    Case 98 : lsValue = "b"
                    Case 99 : lsValue = "c"
                    Case 100 : lsValue = "d"
                    Case 101 : lsValue = "e"
                    Case 102 : lsValue = "f"
                    Case 103 : lsValue = "g"
                    Case 104 : lsValue = "h"
                    Case 105 : lsValue = "i"
                    Case 106 : lsValue = "j"
                    Case 107 : lsValue = "k"
                    Case 108 : lsValue = "l"
                    Case 109 : lsValue = "m"
                    Case 110 : lsValue = "n"
                    Case 111 : lsValue = "o"
                    Case 112 : lsValue = "p"
                    Case 113 : lsValue = "q"
                    Case 114 : lsValue = "r"
                    Case 115 : lsValue = "s"
                    Case 116 : lsValue = "t"
                    Case 117 : lsValue = "u"
                    Case 118 : lsValue = "v"
                    Case 119 : lsValue = "w"
                    Case 120 : lsValue = "x"
                    Case 121 : lsValue = "y"
                    Case 122 : lsValue = "z"
                    Case 123 : lsValue = "ä"
                    Case 124 : lsValue = "ö"
                    Case 125 : lsValue = "ñ"
                    Case 126 : lsValue = "ü"
                    Case 127 : lsValue = "à"
                    Case Else : lsValue = ""
                End Select

                Return lsValue
            End Function
#End Region

            Public Sub New()

            End Sub
        End Class

        Public Class SMS_RECEIVED
            Inherits SMSBase
            Public SrcAddressLength As Byte
            Public SrcAddressType As Byte
            Public SrcAddressValue As String
            Public TP_SCTS As Date
            Public TP_TIMEZONE As String

            Sub New(ByVal PDUCode As String)
                Type = SMSType.SMS_RECEIVED
                GetOrignalData(PDUCode)
            End Sub

            Public Overrides Sub GetOrignalData(ByVal PDUCode As String)
                Debug.Print(PDUCode)
                SCAddressLength = GetByte(PDUCode)
                SCAddressType = GetByte(PDUCode)
                SCAddressValue = GetAddress((GetString(PDUCode, (SCAddressLength - 1) * 2)))
                FirstOctet = GetByte(PDUCode)

                SrcAddressLength = GetByte(PDUCode)
                SrcAddressType = GetByte(PDUCode)
                SrcAddressLength += SrcAddressLength Mod 2
                SrcAddressValue = GetAddress((GetString(PDUCode, SrcAddressLength)))

                TP_PID = GetByte(PDUCode)
                TP_DCS = GetByte(PDUCode)
                TP_SCTS = GetDate(GetString(PDUCode, 14), TP_TIMEZONE)
                TP_UDL = GetByte(PDUCode)
                TP_UD = GetString(PDUCode, TP_UDL * 2)
            End Sub
        End Class

        Public Class SMS_SUBMIT
            Inherits SMSBase
            Public TP_MR As Byte
            Public DesAddressLength As Byte
            Public DesAddressType As Byte
            Public DesAddressValue As String
            Public TP_VP As Byte
            Sub New(ByVal PDUCode As String)
                Type = SMSType.SMS_SUBMIT
                GetOrignalData(PDUCode)
            End Sub

            Public Overrides Sub GetOrignalData(ByVal PDUCode As String)
                SCAddressLength = GetByte(PDUCode)
                SCAddressType = GetByte(PDUCode)
                SCAddressValue = GetAddress((GetString(PDUCode, (SCAddressLength - 1) * 2)))
                FirstOctet = GetByte(PDUCode)

                TP_MR = GetByte(PDUCode)

                DesAddressLength = GetByte(PDUCode)
                DesAddressType = GetByte(PDUCode)
                DesAddressLength += DesAddressLength Mod 2
                DesAddressValue = GetAddress((GetString(PDUCode, DesAddressLength)))

                TP_PID = GetByte(PDUCode)
                TP_DCS = GetByte(PDUCode)
                TP_VP = GetByte(PDUCode)
                TP_UDL = GetByte(PDUCode)
                TP_UD = GetString(PDUCode, TP_UDL * 2)
            End Sub
        End Class

        Public Class EMS_RECEIVED
            Inherits SMS_RECEIVED
            Public Structure InfoElem       'See document "How to create EMS"
                Public Identifier As Byte
                Public Length As Byte
                Public Data As String
            End Structure
            Public TP_UDHL As Byte

            Public IE() As InfoElem

            Sub New(ByVal PDUCode As String)
                MyBase.New(PDUCode)
            End Sub

            Public Overrides Sub GetOrignalData(ByVal PDUCode As String)
                Dim lsTP_UD As String

                SCAddressLength = GetByte(PDUCode)
                SCAddressType = GetByte(PDUCode)
                SCAddressValue = GetAddress(GetString(PDUCode, (SCAddressLength - 1) * 2))
                FirstOctet = GetByte(PDUCode)

                SrcAddressLength = GetByte(PDUCode)
                SrcAddressType = GetByte(PDUCode)
                SrcAddressLength += SrcAddressLength Mod 2
                SrcAddressValue = GetAddress((GetString(PDUCode, SrcAddressLength)))

                TP_PID = GetByte(PDUCode)
                TP_DCS = GetByte(PDUCode)
                TP_SCTS = GetDate(GetString(PDUCode, 14), TP_TIMEZONE)
                TP_UDL = GetByte(PDUCode)

                lsTP_UD = GetString(PDUCode, TP_UDL * 2)
                TP_UD = lsTP_UD

                TP_UDHL = GetByte(lsTP_UD)
                IE = EMS_RECEIVED.GetIE(GetString(lsTP_UD, TP_UDHL * 2))
            End Sub

            'Get Informat Elements 
            Shared Function GetIE(ByVal IECode As String) As InfoElem()
                Dim tmp As String = IECode, t As Integer = 0
                Dim result() As InfoElem
                result = Nothing
                Do Until IECode = ""
                    ReDim Preserve result(t)
                    With result(t)
                        .Identifier = GetByte(IECode)
                        .Length = GetByte(IECode)
                        .Data = GetString(IECode, .Length * 2)
                    End With
                    t += 1
                Loop
                Return result
            End Function
        End Class

        Public Class EMS_SUBMIT
            Inherits SMS_SUBMIT

            Sub New(ByVal PDUCode As String)
                MyBase.New(PDUCode)
                Type = SMSType.EMS_SUBMIT
            End Sub

            Public TP_UDHL As Byte

            Public IE() As EMS_RECEIVED.InfoElem

            Public Overrides Sub GetOrignalData(ByVal PDUCode As String)
                Dim lsTP_UD As String

                SCAddressLength = GetByte(PDUCode)
                SCAddressType = GetByte(PDUCode)
                SCAddressValue = GetAddress(GetString(PDUCode, (SCAddressLength - 1) * 2))
                FirstOctet = GetByte(PDUCode)

                TP_MR = GetByte(PDUCode)

                DesAddressLength = GetByte(PDUCode)
                DesAddressType = GetByte(PDUCode)
                DesAddressLength += DesAddressLength Mod 2
                DesAddressValue = GetAddress(GetString(PDUCode, DesAddressLength))

                TP_PID = GetByte(PDUCode)
                TP_DCS = GetByte(PDUCode)
                TP_VP = GetByte(PDUCode)
                TP_UDL = GetByte(PDUCode)
                lsTP_UD = GetString(PDUCode, TP_UDL * 2)
                TP_UD = lsTP_UD

                TP_UDHL = GetByte(lsTP_UD)
                IE = EMS_RECEIVED.GetIE(GetString(lsTP_UD, TP_UDHL * 2))
            End Sub
        End Class

        Public Class SMS_STATUS_REPORT
            Inherits SMS_RECEIVED
            Public TP_MR As Byte
            Public TP_DP As Date
            Public Status As EnumStatus

            Public Enum EnumStatus
                Success = 0
                NotSend = 96
                NoResponseFromSME = 98
            End Enum

            Sub New(ByVal PDUCode As String)
                MyBase.New(PDUCode)
                Type = SMSType.SMS_STATUS_REPORT
            End Sub

            Public Overrides Sub GetOrignalData(ByVal PDUCode As String)
                SCAddressLength = GetByte(PDUCode)
                SCAddressType = GetByte(PDUCode)
                SCAddressValue = GetAddress(GetString(PDUCode, (SCAddressLength - 1) * 2))

                FirstOctet = GetByte(PDUCode)

                TP_MR = GetByte(PDUCode)

                SrcAddressLength = GetByte(PDUCode)
                SrcAddressType = GetByte(PDUCode)
                SrcAddressLength += SrcAddressLength Mod 2
                SrcAddressValue = GetAddress(GetString(PDUCode, SrcAddressLength))

                TP_SCTS = GetDate(GetString(PDUCode, 14), TP_TIMEZONE)
                TP_DP = GetDate(GetString(PDUCode, 14), TP_TIMEZONE)

                Status = GetByte(PDUCode)

                'Status report do not have content so I set it a zero length string
                TP_UD = ""
            End Sub
        End Class

        Public Class PDUDecoder
            Public Structure BaseInfo
                Public SourceNumber As String
                Public DestinationNumber As String
                Public ReceivedDate As Date
                Public TimeZone As String
                Public Text As String
                Public Type As SMS.SMSType
                Public Data As String
                Public EMSTotolPiece As Integer
                Public EMSCurrentPiece As Integer
                Public StatusFromReport As SMS_STATUS_REPORT.EnumStatus

                Public DestinationReceivedDate As Object
            End Structure

            Sub New(ByVal PDUCode As String)
                Decode(PDUCode)
            End Sub

            Public Shared Function Decode(ByVal PDUCode As String) As BaseInfo
                Dim result As BaseInfo
                result = Nothing

                Try
                    Dim s As Object = Nothing
                    Dim T As SMSType = SMSBase.GetSMSType(PDUCode)
                    result.Type = T

                    Select Case T
                        Case SMSType.EMS_RECEIVED
                            s = New EMS_RECEIVED(PDUCode)
                            result.SourceNumber = s.SrcAddressValue
                            If s.SrcAddressType = &H91 Then result.SourceNumber = "+" + result.SourceNumber
                            result.ReceivedDate = s.TP_SCTS
                            If Not (s.IE Is Nothing) Then
                                Dim Data As String = s.IE(0).Data
                                result.Data = Mid(Data, 1, 4)
                                result.EMSTotolPiece = CInt(Mid(Data, 3, 2))
                                result.EMSCurrentPiece = CInt(Mid(Data, 5, 2))
                            End If
                        Case SMSType.SMS_RECEIVED
                            s = New SMS_RECEIVED(PDUCode)
                            result.SourceNumber = s.SrcAddressValue
                            If s.SrcAddressType = &H91 Then result.SourceNumber = "+" + result.SourceNumber
                            result.ReceivedDate = s.TP_SCTS
                        Case SMSType.EMS_SUBMIT
                            s = New EMS_SUBMIT(PDUCode)
                            result.DestinationNumber = s.DesAddressValue
                            If s.DesAddressType = &H91 Then result.DestinationNumber = "+" + result.DestinationNumber
                            If Not (s.IE Is Nothing) Then
                                Dim Data As String = s.IE(0).Data
                                result.Data = Mid(Data, 1, 4)
                                result.EMSTotolPiece = CInt(Mid(Data, 3, 2))
                                result.EMSCurrentPiece = CInt(Mid(Data, 5, 2))
                            End If
                        Case SMSType.SMS_SUBMIT
                            s = New SMS_SUBMIT(PDUCode)
                            result.DestinationNumber = s.DesAddressValue
                            If s.DesAddressType = &H91 Then result.DestinationNumber = "+" + result.DestinationNumber
                        Case SMSType.SMS_STATUS_REPORT
                            s = New SMS_STATUS_REPORT(PDUCode)
                            result.SourceNumber = s.SrcAddressValue
                            If s.SrcAddressType = &H91 Then result.SourceNumber = "+" + result.SourceNumber
                            result.ReceivedDate = s.TP_SCTS
                            result.DestinationReceivedDate = s.TP_DP()
                            result.StatusFromReport = s.status
                        Case Else
                            Stop
                    End Select

                    '###########################
                    'Correct when s is SMS type, no TP_UDL is found.
                    'Note:Only EMS has the TP_UDHL and TP_UDH.
                    '###########################
                    If s.tp_DCS = 0 Or _
                        s.tp_DCS = 242 Then
                        If T = SMSType.SMS_RECEIVED Or T = SMSType.SMS_STATUS_REPORT Or T = SMSType.SMS_SUBMIT Then
                            '#############################
                            'add a parameter
                            '############################
                            result.Text = s.decode7bit(s.tp_UD, s.TP_UDL)
                        End If

                        If T = SMSType.EMS_RECEIVED Or T = SMSType.EMS_SUBMIT Then
                            'result.Text = s.decode7bit(s.tp_ud, s.tp_udl - 8 * (1 + s.tp_udhl) / 7)
                            result.Text = s.decode7bit(s.tp_ud, s.tp_udl)
                        End If
                    ElseIf s.tp_DCS = 246 Then
                        result.Text = s.DecodeUnicode(s.TP_UD)
                    Else
                        result.Text = s.DecodeUnicode(s.TP_UD)
                    End If

                    'If s.tp_DCS = 0 Or _
                    '    s.tp_DCS = 242 Then
                    '    If T = SMSType.SMS_RECEIVED Or T = SMSType.SMS_STATUS_REPORT Or T = SMSType.SMS_SUBMIT Then
                    '        '#############################
                    '        'add a parameter
                    '        '############################

                    '        result.Text = s.decode7bitsms(s.tp_UD, s.TP_UDL)
                    '        'result.Text = s.decodesevenbit(s.tp_UD, s.TP_UDL)
                    '    End If

                    '    If T = SMSType.EMS_RECEIVED Or T = SMSType.EMS_SUBMIT Then
                    '        result.Text = s.decode7bit(s.tp_UD, s.tp_UDL - 8 * (1 + s.tp_udhl) / 7)
                    '        'result.Text = s.decodesevenbit(s.tp_UD, s.tp_UDL - 8 * (1 + s.tp_udhl) / 7)
                    '        'jheff [03-23-2015] 
                    '        'remove first seven charter which is the UDH
                    '        result.Text = Mid(result.Text, 6).ToString.Replace("\n", "")
                    '        result.Text = Left(result.Text, result.Text.Length - 1)
                    '        'If Right(result.Text, 1) = "@" Then
                    '        '    result.Text = Left(result.Text, Len(result.Text) - 1)
                    '        'End If
                    '    End If
                    'ElseIf s.tp_DCS = 246 Then
                    '    result.Text = s.DecodeUnicode(s.TP_UD)
                    'Else
                    '    result.Text = s.DecodeUnicode(s.TP_UD)
                    'End If
                Catch err As System.Exception
                    result.Text = "PDU decoding exception:" & PDUCode
                End Try
                Return result
            End Function
        End Class

    End Namespace
End Namespace