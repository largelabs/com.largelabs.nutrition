using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraKernelVFX : MonoBehaviourBase
{
    private static readonly string BURNT_SELECT_VFX_PREFAB = "VFX_Select_Smoke";
    List<PooledDoraVFX> liveVFX = null;
    SpawnPool vfxPool = null;
    Coroutine updateScaleRoutine = null;
    InterpolatorsManager interpolators = null;

    // Start is called before the first frame update
    public void Init(SpawnPool i_vfxPool, InterpolatorsManager i_interpolators)
    {
        vfxPool = i_vfxPool;
        interpolators = i_interpolators;
    }

    public void ResetVFX()
    {
        if (null != liveVFX)
        {
            int count = liveVFX.Count;
            for (int i = 0; i < count; i++)
            {
                liveVFX[i].DespawnNow();
            }

            liveVFX.Clear();
        }
    }

    public bool HasLiveVFX
    {
        get
        {
            if (null == liveVFX) return true;
            return liveVFX.Count == 0;
        }
    }

    public void PlayScaleAnimation(Vector3 i_target, AnimationCurve i_animationCurve, float i_animationSpeed)
    {
        StopScaleAnimation();
        float animationTime = Mathf.Abs(transform.localScale.z - i_target.z) / i_animationSpeed;
        ITypedAnimator<Vector3> scaleInterpolator = interpolators.Animate(transform.localScale, i_target, animationTime, new AnimationMode(i_animationCurve), false, 0f, null);

        updateScaleRoutine = StartCoroutine(updateScale(scaleInterpolator));
    }

    public void StopScaleAnimation()
    {
        this.DisposeCoroutine(ref updateScaleRoutine);
    }

    public void PlaySmokeVFX()
    {
        Transform vfxTr = vfxPool.Spawn(BURNT_SELECT_VFX_PREFAB);
        registerVfx(vfxTr.GetComponent<PooledDoraVFX>());
        vfxTr.SetParent(transform);
        vfxTr.localScale = MathConstants.VECTOR_3_ONE;
        vfxTr.localPosition = new Vector3(0f, 0f, 0.5f);

        vfxTr.SetParent(null);
        float scaleValue = Mathf.Max(vfxTr.localScale.x, vfxTr.localScale.y);
        vfxTr.localScale = MathConstants.VECTOR_3_ONE * scaleValue;
        vfxTr.SetParent(transform);
    }

    void registerVfx(PooledDoraVFX i_vfx)
    {
        if (null == liveVFX) liveVFX = new List<PooledDoraVFX>();
        liveVFX.Add(i_vfx);
        i_vfx.Init(vfxPool);
        i_vfx.OnDidEnd += unregisterVfx;
    }

    void unregisterVfx(PooledDoraVFX i_vfx)
    {
        if (true == liveVFX.Remove(i_vfx))
        {
            i_vfx.DespawnNow();
        }
    }

    IEnumerator updateScale(ITypedAnimator<Vector3> i_interpolator)
    {
        while (true == i_interpolator.IsAnimating)
        {
            transform.localScale = i_interpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref updateScaleRoutine);
    }
}
