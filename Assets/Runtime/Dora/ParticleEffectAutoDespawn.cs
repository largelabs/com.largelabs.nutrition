using PathologicalGames;
using System.Collections;
using UnityEngine;

public class ParticleEffectAutoDespawn : MonoBehaviour
{
	private static readonly string POOL_NAME = "VFX_POOL";
	SpawnPool pool = null;

	void OnEnable()
	{
		pool = PoolManager.Pools[POOL_NAME];
		StartCoroutine(checkIfAlive());
	}

	IEnumerator checkIfAlive()
	{
		ParticleSystem ps = this.GetComponent<ParticleSystem>();

		while (true && ps != null)
		{
			yield return new WaitForSeconds(0.5f);
			if (!ps.IsAlive(true))
			{
				pool.Despawn(transform);
				transform.SetParent(pool.transform);
			}
		}
	}
}
