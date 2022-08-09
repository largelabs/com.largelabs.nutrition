using System.Collections;
using UnityEngine;

public class DoraKernelFactory : MonoBehaviourBase
{
    [SerializeField] Transform[] anchors = null;
    [SerializeField] Transform[] normalAnchors = null;
    [SerializeField] GameObject kernel = null;

    GameObject[,] kernelMap = null;
    int currentRowIndex = 0;
    int currentColumnIndex = 0;
    Transform rowNormal = null;


    private void Start()
    {
        Populate();
        updateRowIndex(0);
    }

    private void Update()
    {
        float dot = Vector3.Dot(rowNormal.forward, Camera.main.transform.forward);

        if(dot < -1.1f || dot > -0.97f)
        {
            int sign = dot < -1f ? -1 : 1;
            transform.Rotate(Time.deltaTime * sign * 200f, 0f, 0f);
        }

    }

    private GUIStyle guiStyle = new GUIStyle(); //create a new variable
    private void OnGUI()
    {
        guiStyle.fontSize = 30;
        GUI.Label(new Rect(10, 10, 200, 40), currentRowIndex + "," + currentColumnIndex, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 40), "row normal " + rowNormal.forward + " - cam normal " + Camera.main.transform.forward, guiStyle);
        float dot = Vector3.Dot(rowNormal.forward, Camera.main.transform.forward);
        GUI.Label(new Rect(10, 90, 200, 40), "dot " + dot.ToString(), guiStyle);
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

        kernelMap = CollectionUtilities.Make2DArray<GameObject>(kernels, 12, 11);

        for(int i = 0; i < 12; i++)
        {
            for(int j = 0; j < 11; j++)
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

        for (int i = 0; i < 12; i++)
        {
            updateRowIndex(i);

            for (int j = 0; j < 11; j++)
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

    void updateRowIndex(int i_rowIndex)
    {
        currentRowIndex = i_rowIndex;
        rowNormal = normalAnchors[currentRowIndex];
    }

}
