using PathologicalGames;
using System.Collections;
using UnityEngine;

public class ParticleEffectAutoDespawn : MonoBehaviour
{
	private static readonly string POOL_NAME = "VFX_POOL";
	SpawnPool pool = null;
	ParticleSystem ps = null;

	void OnEnable()
	{
		ps = GetComponent<ParticleSystem>();
		pool = PoolManager.Pools[POOL_NAME];
		StartCoroutine(checkIfAlive());
	}

    public bool IsAlive => null != ps && ps.IsAlive(true);

	public void Despawn()
	{
		pool.Despawn(transform);
		transform.SetParent(pool.transform);
	}

	IEnumerator checkIfAlive()
	{
		while (true && ps != null)
		{
			if (false == IsAlive)
			{
				Despawn();
				yield break;
			}

			yield return null;

		}
	}
}
