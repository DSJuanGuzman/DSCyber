Imports Dll100PortCyberGear

' Clase que representa un dispositivo en el sistema CyberGear.
''' <summary>
''' Clase que representa un dispositivo en el sistema CyberGear.
''' </summary>
''' <remarks>
''' Implementa la interfaz IDispositiu y proporciona un método para obtener el código del dispositivo.
''' </remarks>
Friend Class Dispositiu
        Implements IDispositiu

        Private ReadOnly codi As Integer

    ' Constructor que inicializa el código del dispositivo.
    ''' <summary>
    ''' Crea una instancia de la clase Dispositiu con el CANID del dispositivo encontrado.
    ''' </summary>
    ''' <param name="_codi">El CANID del dispositivo.</param>
    Public Sub New(_codi As Integer)
            codi = _codi
        End Sub

        ' Devuelve el código del dispositivo.
        ''' <summary>
        ''' Devuelve el código del dispositivo.
        ''' </summary>
        ''' <returns>El código del dispositivo.</returns>
        Public Function senCodi() As Integer Implements IDispositiu.senCodi
        Return codi
    End Function
    End Class

