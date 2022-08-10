using System.Collections;
using UnityEngine;

public class DoraKernelFactory : MonoBehaviourBase
{
    [SerializeField] Transform[] anchors = null;
    [SerializeField] Transform[] normalAnchors = null;
    [SerializeField] GameObject kernel = null;
    [SerializeField] InterpolatorsManager interpolators = null;

    DoraCellData[,] doraMap = null;
    int currentRowIndex = 0;
    int currentColumnIndex = 0;
    Transform rowNormal = null;


    private void Start()
    {
        Populate(true);
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

   /* private GUIStyle guiStyle = new GUIStyle(); //create a new variable
    private void OnGUI()
    {
        guiStyle.fontSize = 30;
        GUI.Label(new Rect(10, 10, 200, 40), currentRowIndex + "," + currentColumnIndex, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 40), "row normal " + rowNormal.forward + " - cam normal " + Camera.main.transform.forward, guiStyle);
        float dot = Vector3.Dot(rowNormal.forward, Camera.main.transform.forward);
        GUI.Label(new Rect(10, 90, 200, 40), "dot " + dot.ToString(), guiStyle);
    }
   */

    public void Populate(bool i_animated)
    {
        int count = anchors.Length;

        DoraCellData[] kernels = new DoraCellData[count];
        DoraCellData currData = null;
        DoraKernel currKernel = null;
        Transform currAnchor = null;

        for (int i = 0; i < count; i++)
        {
            currAnchor = anchors[i];

            GameObject go = GameObject.Instantiate(kernel);
            go.transform.SetParent(anchors[i]);
            go.transform.localPosition = MathConstants.VECTOR_3_ZERO;
            go.transform.localRotation = MathConstants.QUATERNION_IDENTITY;
            go.transform.localScale = MathConstants.VECTOR_3_ONE;

            currKernel = go.GetComponent<DoraKernel>();
            currKernel.Init(interpolators);

            currData = kernels[i] = new DoraCellData();
            currData.Init(currKernel, currAnchor);

            if (false == i_animated)
                currKernel.Appear(false);
        }

        doraMap = CollectionUtilities.Make2DArray<DoraCellData>(kernels, 12, 11);

        for(int i = 0; i < 12; i++)
        {
            for(int j = 0; j < 11; j++)
            {
                currData = doraMap[i, j];
                currData.SetCoords(new Vector2Int(i, j));
                currData.SetKernelName(i + "," + j);
            }
        }

        if(true == i_animated)
        {
            StartCoroutine(populateAnimated(0,6, true));
            StartCoroutine(populateAnimated(6, 12, false));
        }

    }

    IEnumerator populateAnimated(int i_startRow, int i_endRow, bool i_updateRowIndex)
    {
        for (int i = i_startRow; i < i_endRow; i++)
        {
            if(true == i_updateRowIndex) updateRowIndex(i);
            for (int j = 0; j < 11; j++)
            {
                if(true == doraMap[i, j].HasKernel)
                {
                    doraMap[i, j].Kernel.Appear(true);
                    yield return new WaitForSeconds(0.025f);
                }
            }
        }
    }


    void updateRowIndex(int i_rowIndex)
    {
        currentRowIndex = i_rowIndex;
        rowNormal = normalAnchors[currentRowIndex];
    }

}
