public interface IWallDetector2D
{
    bool IsHittingWall { get; }

    Physics2DWallEvent OnWallStatusChanged { get; set; }
}