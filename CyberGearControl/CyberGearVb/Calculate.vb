Imports System


' Funciones numéricas para transformar tipos de datos a valores.
''' <summary>
''' Clase que proporciona funcionalidades matemáticas para el procesamiento de datos en el sistema de control CyberGear.
''' </summary>
''' <remarks>
''' Incluye funciones para mapear valores de entrada a un rango específico, así como posibles otras operaciones matemáticas.
''' Principalmente utilizada para manejar y transformar señales y datos en el sistema de control.
''' </remarks>
Friend Class Calculate

    ' Mapea una entrada al rango de 0 a 65535, con 'val' como el valor de entrada.
    Public Shared Function FToU(val As Double, xmin As Double, xmax As Double) As UInteger
        ' Calcula la longitud del intervalo objetivo y el intervalo original.
        Dim targetRange As Double = xmax - xmin
        Dim originalRange As Double = 65535 - 1 ' De 0 a 65535

        ' Asegúrate de que el valor esté dentro del rango de xmin a xmax.
        If val < xmin OrElse val > xmax Then
            Throw New ArgumentOutOfRangeException(NameOf(val), $"El valor debe estar entre {xmin} y {xmax}.")
        End If

        ' Calcula el factor de escala.
        Dim scaleFactor As Double = originalRange / targetRange

        ' Aplica el mapeo.
        Dim mappedValue As Double = (val - xmin) * scaleFactor

        ' Redondea y convierte a un entero.
        Return CUInt(Math.Round(mappedValue))
    End Function
    Public Shared Function FloatToUInt(x As Single, x_min As Single, x_max As Single, bits As Integer) As Integer
        Dim span As Single = x_max - x_min
        Dim offset As Single = x_min

        ' Limitar el valor de x dentro de los límites
        If x > x_max Then
            x = x_max
        ElseIf x < x_min Then
            x = x_min
        End If

        ' Convertir el valor flotante al valor entero deseado
        Return CInt((x - offset) * ((1 << bits) - 1) / span)
    End Function

    ' Mapea una entrada al rango de 0 a 65535, con 'x' como el valor de entrada.
    Public Shared Function UToF(x As Integer, xmin As Double, xmax As Double) As Double
        ' Asegúrate de que el valor esté entre 0 y 65535.
        If x < 0 OrElse x > 65535 Then
            Throw New ArgumentOutOfRangeException(NameOf(x), "x debe estar entre 0 y 65535.")
        End If

        ' Calcula la longitud del intervalo original y el intervalo objetivo.
        Dim originalRange As Double = 65535 ' De 0 a 65535
        Dim targetRange As Double = xmax - xmin

        ' Calcula el factor de escala.
        Dim scaleFactor As Double = targetRange / originalRange

        ' Aplica el mapeo.
        Dim mappedValue As Double = (x * scaleFactor) + xmin

        ' Devuelve el valor mapeado.
        Return mappedValue
    End Function

End Class
