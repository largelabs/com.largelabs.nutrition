public interface IWallDetector
{
    bool IsHittingWall { get; }

    Physics2DWallEvent OnWallStatusChanged { get; set; }
}