using System.Collections;
using UnityEngine;

public class DoraKernelFactory : MonoBehaviourBase
{
    [SerializeField] Transform[] anchors = null;
    [SerializeField] GameObject kernel = null;

    GameObject[,] kernelMap = null;
    int currentRowIndex = 0;
    int currentColumnIndex = 0;


    private GUIStyle guiStyle = new GUIStyle(); //create a new variable
    private void OnGUI()
    {
        guiStyle.fontSize = 30;
        GUI.Label(new Rect(10, 10, 200, 40), currentRowIndex + "," + currentColumnIndex, guiStyle);
    }

    [ExposePublicMethod]
    public void Populate()
    {
        int count = anchors.Length;

        GameObject[] kernels = new GameObject[count];

        for(int i = 0; i < count; i++)
        {
            GameObject go = GameObject.Instantiate(kernel);
            go.transform.SetParent(anchors[i]);

            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            kernels[i] = go;
        }

        kernelMap = CollectionUtilities.Make2DArray<GameObject>(kernels, 11, 12);

        for(int i = 0; i < 11; i++)
        {
            for(int j = 0; j < 12; j++)
            {
                kernelMap[i, j].name = i + "," + j;
            }
        }
    }

    [ExposePublicMethod]
    public void Explore()
    {
        StartCoroutine(exploreRoutine());
    }


    IEnumerator exploreRoutine()
    {
        Material curr = null;

        for (int i = 0; i < 11; i++)
        {
            currentRowIndex = i;

            for (int j = 0; j < 12; j++)
            {
                currentColumnIndex = j;

                if (null != curr) curr.SetColor("_Color", Color.white);
                curr = kernelMap[i, j].GetComponent<MeshRenderer>().material;

                curr.SetColor("_Color", Color.red);

                yield return new WaitForSeconds(0.5f);
            }
        }

        curr.SetColor("_Color", Color.white);
    }
}
