Imports Dll100PortCyberGear

Public Class BusCanWrapper
    Private ReadOnly _busCan As IBusCan

    Public Sub New(masterCANID As UInteger)
        _busCan = New BusCan(masterCANID)
    End Sub

    Public Function GetDispositius() As List(Of IDispositiu)
        Return _busCan.secDispositius()
    End Function

    Public Function GetMotor(dispositiu As IDispositiu) As IMotor
        Return _busCan.fuxIMotor(dispositiu)
    End Function

    Public Sub Dispose()
        _busCan.Dispose()
    End Sub
End Class
