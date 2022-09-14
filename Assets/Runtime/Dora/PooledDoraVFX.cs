using PathologicalGames;
using System;
using System.Collections;
using UnityEngine;

public class PooledDoraVFX : MonoBehaviour
{
	SpawnPool pool = null;
	ParticleSystem ps = null;
	public Action<PooledDoraVFX> OnDidEnd = null;

    #region PUBLIC API

    public void Init(SpawnPool i_pool)
    {
		pool = i_pool;
		if (null == ps) ps = GetComponent<ParticleSystem>();
		StartCoroutine(checkIfAlive());
	}

	public bool IsAlive => null != ps && ps.IsAlive(true);

	public void DespawnNow()
    {
		OnDidEnd = null;
		pool.Despawn(transform);
		transform.SetParent(pool.transform);
		StopAllCoroutines();
	}

	#endregion

	#region PRIVATE

	IEnumerator checkIfAlive()
	{
		while (true)
		{
			if (false == IsAlive)
			{
				OnDidEnd?.Invoke(this);
				yield break;
			}

			yield return null;

		}
	}

    #endregion
}
