Imports Dll100PortCyberGear

Public Class BusCanFactory
    Implements IBusCanFactory

    Public Function CreateBusCan(masterCANID As UInteger) As IBusCan Implements IBusCanFactory.CreateBusCan
        Return New BusCan(masterCANID)
    End Function
End Class
