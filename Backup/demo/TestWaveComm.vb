Imports ATSMS

Public Class TestWaveComm

    Private oGsmModem As GSMModem

    Private Sub TestWaveComm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        oGsmModem = New GSMModem
        oGsmModem.Port = "COM6"
        oGsmModem.BaudRate = 115200
        oGsmModem.Parity = Common.EnumParity.None
        oGsmModem.StopBits = Common.EnumStopBits.One
        oGsmModem.FlowControl = Common.EnumFlowControl.Hardware
        oGsmModem.DataBits = 8

        Try
            oGsmModem.Connect()
            If oGsmModem.IsConnected Then
                MsgBox(oGsmModem.PhoneModel)
                MsgBox(oGsmModem.IMEI)
                MsgBox(oGsmModem.IMSI)
                MsgBox(oGsmModem.SendSMS("0126868739", "test msg"))
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

    End Sub
End Class