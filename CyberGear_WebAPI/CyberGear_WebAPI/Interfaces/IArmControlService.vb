Public Interface IArmControlService
    ' Crea una nueva articulación (Joint) y la agrega al brazo.
    Function CrearJoint(Ids As List(Of Integer), configuration As ConfigurationEnum) As Joint

    ' Activa una articulación específica del brazo por su ID.
    Sub ActivarJoint(Id As Integer)

    ' Desactiva todas las articulaciones del brazo.
    Sub ActivateArm()
    Sub DeactivateArm()

    ' Mueve el brazo a su posición inicial.
    Sub MoveToInitialPosition()

    ' Mueve el brazo a una posición específica.
    Sub MoveToPosition(positions As Dictionary(Of Integer, Tuple(Of Single, Single)))

    ' Agrega una articulación al brazo.
    Sub AddJoint(joint As Joint)

    ' Obtiene todas las articulaciones del brazo.
    Function GetJoints() As List(Of Joint)

    ' Establece la posición para una articulación específica.
    Sub SetJointPosition(jointId As Integer, speed As Single, angle As Single)

    ' Establece el modo de posición para todas las articulaciones del brazo.
    Sub SetPositionModeForAllJoints()

    ' Desactiva una articulación específica.
    Sub DeactivateJoint(jointId As Integer)
End Interface
