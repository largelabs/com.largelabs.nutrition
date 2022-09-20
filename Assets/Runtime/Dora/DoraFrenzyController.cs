using System.Collections;
using UnityEngine;

public class DoraFrenzyController : MonoBehaviourBase
{
    [SerializeField] private DoraRaycastController raycastController = null;
    [SerializeField] DoraGameplayData DoraGameplayData = null;
    [SerializeField] MaterialColorPingPong superKernelMaterialPingPong = null;

    private void OnEnable()
    {
        superKernelMaterialPingPong.StartPingPong(0.3f, -1);
    }

    #region PUBLIC API

    public IEnumerator PlayFrenzyMode(AutoRotator i_autoRotator)
    {
        i_autoRotator.SetRotationSpeedX(DoraGameplayData.FrenzyRotationSpeed);
        raycastController.StartAutoRotation();

        raycastController.StartAutoMove(DoraGameplayData.CursorAutoMoveSpeed);

        yield return this.Wait(DoraGameplayData.FrenzyTime);
    }

    public void StopFrenzyMode()
    {
        raycastController.StopAutoMove();
    }

    #endregion
}
