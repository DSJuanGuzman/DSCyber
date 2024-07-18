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
