Imports System.Collections.Concurrent

Public Interface IJointControlService
    Sub EstablecerPosicion(id As Integer, SpeedValue As Single, AngleValue As Single)
    Sub EstablecerModoPosicion(Id As Integer)
    Function CrearJoint(Ids As List(Of Integer), configuration As ConfigurationEnum) As Joint
    Sub DesactivarJoint(Id As Integer)
    Sub ActivarJoint(Id As Integer)
    Sub SetJoint(joint As Joint)
    Function GetJoints() As ConcurrentDictionary(Of Integer, Joint)
    Sub Backward(Id As Integer, Speed As Single)
    Sub Forward(Id As Integer, Speed As Single)
End Interface
