Imports Dll100PortCyberGear

Public Class BusCanFactory
    Implements IBusCanFactory

    Public Function fuxBusCan(masterCANID As UInteger) As IBusCan Implements IBusCanFactory.fuxBusCan
        Return New BusCan(masterCANID)
    End Function
End Class
