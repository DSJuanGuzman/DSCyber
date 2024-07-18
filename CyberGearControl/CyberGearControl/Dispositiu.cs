using Dll100PortCyberGear;
using nsConstants;

namespace CyberGear
{
    internal class Dispositiu : IDispositiu
    {
        private int codi;

        public Dispositiu(int _codi)
        {
            codi = _codi;
        }

        public int senCodi()
        {
            return this.codi;
        }
    }
}
