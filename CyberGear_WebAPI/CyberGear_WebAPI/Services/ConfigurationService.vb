Imports CyberGearVb
Imports Dll060CyberGear
Imports Dll100PortCyberGear

Public Class ConfigurationService
    Implements IConfigurationService
    Public Property _busCanFactory As IBusCanFactory
    Public Property _busCan As IBusCan

    Public Sub IniciarServicio() Implements IConfigurationService.IniciarServicio
        _busCanFactory = New BusCanFactory()
        _busCan = _busCanFactory.fuxBusCan(0)
        If _busCan IsNot Nothing Then
            Console.WriteLine("El servicio se ha iniciado correctamente")
        Else
            Console.WriteLine("Ha ocurrido un error mientras se iniciaba el servicio")
        End If
    End Sub
    Public Sub CerrarServicio() Implements IConfigurationService.CerrarServicio
        _busCan.FinalitzarCanal()
        Console.WriteLine("Se ha cerrado el servicio")
    End Sub
    Public Function IniciarMotor(Id As Integer) As IMotor Implements IConfigurationService.IniciarMotor
        If _busCan IsNot Nothing Then
            Dim _Motor As IMotor = _busCan.fuxIMotor(Id)
            Return _Motor
        End If
        Console.WriteLine("No se ha iniciado el motor")
        Return Nothing
    End Function

    Public Function BuscarMotores() As List(Of IMotor) Implements IConfigurationService.BuscarMotores
        Dim Motores As New List(Of IMotor)
        If _busCan IsNot Nothing Then
            Dim SecDispositius As List(Of IDispositiu) = _busCan.secDispositius()
            If SecDispositius IsNot Nothing Then
                Console.WriteLine("Dispositivos Disponibles:")
                For i As Integer = 0 To SecDispositius.Count() - 1
                    Console.WriteLine($"{i} Motor CAN ID: {SecDispositius(i).senCodi}")
                    Dim _Motor As IMotor = _busCan.fuxIMotor(SecDispositius(Console.ReadLine()))
                    If _Motor IsNot Nothing Then
                        Motores.Add(_Motor)
                    End If
                Next
            End If
            Return Motores
        End If
        Console.WriteLine("No se ha iniciado el motor")
        Return Nothing
    End Function
End Class
