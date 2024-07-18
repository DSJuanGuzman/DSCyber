
Public Interface IBusCan
    Sub FinalizarCanal()
    Function secDispositius() As List(Of IDispositiu)
    Function fuxIMotor(vparIDispositiu As IDispositiu) As IMotor
    Function SendCustomCanMessage(arbitrationId As UInteger, data1 As Byte()) As UInteger
    Sub SendCanMessage(MotorCANID As UInteger, cmdMode As UInteger, data1 As Byte())
End Interface
