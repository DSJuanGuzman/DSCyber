Imports Dll100PortCyberGear

Public Class Joint
    Property _Motores As List(Of IMotor)
    Property _Configuration As ConfigurationEnum
    Property _JointId As Integer

    Public Sub New(Id As Integer, configuracion As ConfigurationEnum, motores As List(Of IMotor))
        _Configuration = configuracion
        _Motores = motores
        _JointId = Id
    End Sub
End Class
