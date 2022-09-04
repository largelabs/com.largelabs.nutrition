using UnityEngine;

public class DoraSelectionRaycastSource : MonoBehaviourBase
{
    [SerializeField] float maxLength = 5f;
    [SerializeField] DoraCellMap cellMap = null;
    [SerializeField] DoraAbstractCellSelector selector = null;

    RaycastHit hitData;
    Ray ray;

    GameObject hitGo = null;

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

    public GameObject HitGo => hitGo;
}
