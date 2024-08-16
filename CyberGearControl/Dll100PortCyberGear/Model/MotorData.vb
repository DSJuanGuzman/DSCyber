Public Class MotorData
    Property Posicion As Double
    Property Velocidad As Double
    Property Temperatura As Double
    Property Torque As Double
    Property CanID As Double

    Public Sub New(pos As Double, vel As Double, temp As Double, tor As Double, id As Double)
        Posicion = pos
        Velocidad = vel
        Temperatura = temp
        Torque = tor
        CanID = id
    End Sub
End Class
