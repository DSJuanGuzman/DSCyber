
Public Interface IBusCan
    Sub FinalitzarCanal()
    Function secDispositius() As List(Of IDispositiu)
    Function fuxIMotor(vparIDispositiu As IDispositiu) As IMotor
    Sub EnviarMissatgeCanPersonalitzat(arbitrationId As UInteger, data1 As Byte())
    Sub EnviarMissatgeCan(MotorCANID As UInteger, cmdMode As UInteger, data1 As Byte())
End Interface
