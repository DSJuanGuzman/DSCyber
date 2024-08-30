
Public Interface IBusCan
    Event SingleParamReadReceived As Action(Of UInteger, Byte())
    Sub FinalitzarCanal()
    Function secDispositius() As List(Of IDispositiu)
    Function fuxIMotor(vparIDispositiu As IDispositiu) As IMotor
    Function EnviarMissatgeCanPersonalitzat(arbitrationId As UInteger, data1 As Byte()) As MotorData
    Sub EnviarMissatgeCan(MotorCANID As UInteger, cmdMode As UInteger, data1 As Byte())
    Function fuxIMotor(vparCANID As Integer) As IMotor
End Interface
