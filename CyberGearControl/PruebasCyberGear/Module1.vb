Imports CyberGearVb
Imports Dll100PortCyberGear

Module Module1

    Sub Main()
        Dim busCanFactory As IBusCanFactory = New BusCanFactory()
        Dim _busCan As IBusCan
        Console.WriteLine("Presione cualquier tecla para iniciar el servicio")
        Console.ReadKey()
        Console.WriteLine("Ingrese el MasterCANID:")
        Dim MasterCANID As Integer = Console.ReadLine()
        _busCan = busCanFactory.fuxBusCan(MasterCANID)
        If _busCan IsNot Nothing Then
            Dim SecDispositius As List(Of IDispositiu) = _busCan.secDispositius()
            If SecDispositius IsNot Nothing Then
                Console.WriteLine("Dispositivos Disponibles:")
                For i As Integer = 0 To SecDispositius.Count() - 1
                    Console.WriteLine($"{i} Motor CAN ID: {SecDispositius(i).senCodi}")
                Next
                Console.WriteLine("Por favor seleccione el motor a iniciar:")
                Dim _Motor As IMotor = _busCan.fuxIMotor(SecDispositius(Console.ReadLine()))
                If _Motor IsNot Nothing Then
                    Console.WriteLine("A continuacion se realizaran las pruebas del motor")
                    Console.WriteLine("Definir Cero Mecanico")
                    _Motor.ActivarMotor()
                    _Motor.EstablirZeroMecanic()
                    _Motor.EstablirIDDispositiu(MasterCANID, &H7E)
                    Console.WriteLine("Iniciando el modo de posicion..")
                    _Motor.EstablirModePosicio()
                    Console.WriteLine("ingrese velocidad Limite Para el modo posicion:")
                    _Motor.EstablirLimitVelocitat(Console.ReadLine())
                    Console.WriteLine("Ingrese la pocision objetivo (en radianes)")
                    _Motor.EstablirPosicio(Console.ReadLine())
                    Console.ReadKey()
                    Console.WriteLine("iniciando Modo de Control Manual con valores de prueba")
                    _Motor.EstablirModeControl()
                    _Motor.EnviarComandaControlMotor(0.1F, 4.0F, 1.0F, 2.0F, 0.1F)
                    Console.ReadKey()
                    _Motor.EnviarComandaControlMotor(0.1F, 0.0F, 1.0F, 2.0F, 0.1F)
                    Console.ReadKey()
                    Console.WriteLine("Load end lap counting mechanical angle:")
                    _Motor.LlegirParametreUnic(&H701B)
                    Console.ReadKey()
                    _Motor.DesactivarMotor()
                    _busCan.FinalitzarCanal()
                End If
            End If
        End If
    End Sub
End Module
