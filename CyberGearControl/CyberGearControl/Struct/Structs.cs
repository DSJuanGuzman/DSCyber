
namespace CyberGear.Struct
{
    public struct CanMessageResult
    {
        public byte[] Data { get; }
        public uint Id { get; }

        public CanMessageResult(byte[] data, uint id)
        {
            Data = data;
            Id = id;
        }
    }

    public struct ParsedMessage
    {
        public byte MotorCanId { get; }
        public double Position { get; }
        public double Velocity { get; }
        public double Torque { get; }

        public ParsedMessage(byte motorCanId, double position, double velocity, double torque)
        {
            MotorCanId = motorCanId;
            Position = position;
            Velocity = velocity;
            Torque = torque;
        }
    }
}
