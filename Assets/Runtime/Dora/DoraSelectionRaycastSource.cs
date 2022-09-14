using UnityEngine;

public class DoraSelectionRaycastSource : MonoBehaviourBase
{
    [SerializeField] float maxLength = 5f;
    [SerializeField] DoraCellMap cellMap = null;
    [SerializeField] DoraCellSelector selector = null;
    [SerializeField] DoraInputs inputs = null;

    RaycastHit hitData;
    Ray ray;

    GameObject hitGo = null;

    #region UNITY AND CORE

    void Update()
    {
        ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * maxLength);

        Physics.Raycast(ray, out hitData, maxLength);

        if (null != hitData.transform)
            hitGo = hitData.transform.gameObject;
        else
            hitGo = null;

    }

    #endregion

    #region PUBLIC API

    public GameObject HitGo => hitGo;

    #endregion
}
