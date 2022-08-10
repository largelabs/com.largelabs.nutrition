public interface IAccelerationConfig : IVelocityConfig
{
    float AccelerationX { get; }

    float AccelerationY { get; }

    float AccelerationZ { get; }

    float DescelerationX { get; }

    float DescelerationY { get; }

    float DescelerationZ { get; }
}