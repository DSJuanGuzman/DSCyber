
Public Interface IBusCan
    Sub FinalizarCanal()
    Function secDispositius() As List(Of IDispositiu)
    Function fuxIMotor(vparIDispositiu As IDispositiu) As IMotor
    Sub SendCustomCanMessage(arbitrationId As UInteger, data1 As Byte())
    Sub SendCanMessage(MotorCANID As UInteger, cmdMode As UInteger, data1 As Byte())
End Interface
