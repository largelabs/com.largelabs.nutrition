using UnityEngine;
using UnityEngine.UI;

public class UIDoraKernel : MonoBehaviourBase
{
    [SerializeField] Image kernelImage = null;

    ScoreKernelInfo scoreInfo = null;

    #region PUBLIC API

    public void SetScoreInfo(ScoreKernelInfo i_scoreInfo)
    {
        scoreInfo = i_scoreInfo;
    }

    public ScoreKernelInfo ScoreInfo => scoreInfo;

    public RectTransform Rect => transform as RectTransform;

    public void Reset()
    {
        Color temp = kernelImage.color;
        kernelImage.color = new Color(temp.r, temp.g, temp.b, 1f);

        scoreInfo = null;
    }

    #endregion
}
