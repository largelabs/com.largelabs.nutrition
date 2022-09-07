public class ScoreKernelInfo
{
    float scoreMultiplier;
    DoraKernel.KernelStatus kernelStatus;

    public ScoreKernelInfo(float i_mult, DoraKernel.KernelStatus i_status)
    {
        scoreMultiplier = i_mult;
        kernelStatus = i_status;
    }

    public float ScoreMultiplier => scoreMultiplier;
    public DoraKernel.KernelStatus KernelStatus => kernelStatus;
}
