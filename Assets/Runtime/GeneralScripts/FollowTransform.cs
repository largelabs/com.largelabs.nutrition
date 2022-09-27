using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] protected Transform transformToFollow = null;
    [SerializeField] private Vector3 offset = MathConstants.VECTOR_3_ZERO;
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY= true;
    [SerializeField] private bool followZ = true;
    [SerializeField] private bool followRotation = true;
    [SerializeField] private float delayBetweenUpdates = 0.1f;

    Coroutine followUpdateRoutine = null;

    private void Start()
    {
        if (followUpdateRoutine == null)
            followUpdateRoutine = StartCoroutine(followUpdateSequence());
    }

    private void OnDestroy()
    {
        this.DisposeCoroutine(ref followUpdateRoutine);
    }

    private IEnumerator followUpdateSequence()
    {
        while (true)
        {
            if (transformToFollow != null)
            {
                Vector3 newPos = transformToFollow.position + offset;
                this.transform.position = new Vector3(
                    followX ? newPos.x : transform.position.x,
                    followY ? newPos.y : transform.position.y,
                    followZ ? newPos.z : transform.position.z);

                if (followRotation)
                    this.transform.rotation = transformToFollow.rotation;
            }

            yield return this.Wait(delayBetweenUpdates);
        }
    }
}
