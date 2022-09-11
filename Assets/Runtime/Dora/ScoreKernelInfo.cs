public class ScoreKernelInfo
{
    float scoreMultiplier;
    KernelStatus kernelStatus;

    public ScoreKernelInfo(float i_mult, KernelStatus i_status)
    {
        scoreMultiplier = i_mult;
        kernelStatus = i_status;
    }

    public float ScoreMultiplier => scoreMultiplier;
    public KernelStatus KernelStatus => kernelStatus;
}
