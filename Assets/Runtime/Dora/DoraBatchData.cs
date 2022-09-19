using UnityEngine;

[CreateAssetMenu(fileName = "DoraBatchData", menuName = "ScriptableObjects/Dora/DoraBatchData", order = 1)]
public class DoraBatchData : ScriptableObject
{
    [Header("Base Settings")]
    [SerializeField] private DoraData assignedDoraData = null;
    [SerializeField] [Range(1, 4)] private int doraInBatch = 4;

    [Header("Burnt Kernels")]
    [SerializeField] private float maxBurntPercentage = 0.25f;
    [SerializeField] private float maxBurntPercentageIncrease = 0.05f;
    [SerializeField] private float maxBurntPercentageLimit = 0.4f;
    [SerializeField] private DoraDurabilityManager.Distribution distributionStyleLvl1 = DoraDurabilityManager.Distribution.EdgeFocused;
    [SerializeField] private DoraDurabilityManager.Distribution distributionStyleLvl2 = DoraDurabilityManager.Distribution.CenterFocused;
    [SerializeField] private DoraDurabilityManager.Distribution distributionStyleLvl3 = DoraDurabilityManager.Distribution.Uniform;

    [Header("Scoring")]
    [SerializeField] private int batchFinishScoreBonus = 10;
    [SerializeField] private float batchFinishTimeBonus = 10f;

    [Header("Super Kernels")]
    // the chance a super kernel appears in a cob (checked once per cob)
    [SerializeField] private float superKernelChance = 0.15f;
    // the increase added to the chance after each failed check (resets for every cob)
    [SerializeField] private float superKernelChanceIncrease = 0.025f;
    // the max number of super kernels in the same cob
    [SerializeField] private int maxSuperKernelsPerCob = 1;
    // the max number of cobs that can have a super kernel in the same batch
    // if at least one super kernel was spawned in a cob it counts towards the maximum
    [SerializeField] private int maxSuperKernelsPerBatch = 3;

    #region GETTERS
    public int DoraInBatch => doraInBatch;
    public DoraData AssignedDoraData => assignedDoraData;

    public float MaxBurntPercentage => maxBurntPercentage;
    public float MaxBurntPercentageIncrease => maxBurntPercentageIncrease;
    public float MaxBurntPercentageLimit => maxBurntPercentageLimit;

    public DoraDurabilityManager.Distribution DistrubutionStyleLVL1 => distributionStyleLvl1;
    public DoraDurabilityManager.Distribution DistrubutionStyleLVL2 => distributionStyleLvl2;
    public DoraDurabilityManager.Distribution DistrubutionStyleLVL3 => distributionStyleLvl3;

    public int BatchFinishScoreBonus => batchFinishScoreBonus;
    public float BatchFinishTimeBonus => batchFinishTimeBonus;

    public float SuperKernelChance => superKernelChance;
    public float SuperKernelChanceIncrease => superKernelChanceIncrease;
    public int MaxSuperKernelsPerCob => maxSuperKernelsPerCob;
    public int MaxSuperKernelsPerBatch => maxSuperKernelsPerBatch;
    #endregion
}
