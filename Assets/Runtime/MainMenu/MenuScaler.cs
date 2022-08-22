using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScaler : MonoBehaviourBase
{
    [SerializeField] List<Transform> items;
    [SerializeField] Transform indicator;

    Camera mainCamera;
    float pixelWidth;



    private void Start()
    {
        mainCamera = Camera.main;
        pixelWidth = mainCamera.pixelWidth;
        StartCoroutine(AdjustWidth());
    }

    private IEnumerator AdjustWidth()
    {
        while (true)
        {
            if (mainCamera.pixelWidth == pixelWidth)
                yield return new WaitForSeconds(0.25f);
            else
            {
                pixelWidth = mainCamera.pixelWidth;
                float sepDistance = pixelWidth / (items.Count + 1);
                for (int i = 0; i < items.Count; i++)
                {
                    float positionX = Camera.main.ScreenToWorldPoint(new Vector3((i + 1) * sepDistance, 0, 20)).x;
                    items[i].position = new Vector3(positionX, 0, 0);
                }
                indicator.position = new Vector3(items[0].position.x, indicator.position.y, indicator.position.z);
            }
        }
    }
}
