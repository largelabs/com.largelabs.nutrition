using System.Collections;
using UnityEngine;

public class DoraKernelFactory : MonoBehaviourBase
{
    [SerializeField] Transform[] anchors = null;
    [SerializeField] Transform[] normalAnchors = null;
    [SerializeField] GameObject kernel = null;
    [SerializeField] InterpolatorsManager interpolators = null;

    DoraKernel[,] kernelMap = null;
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

    private GUIStyle guiStyle = new GUIStyle(); //create a new variable
    private void OnGUI()
    {
        guiStyle.fontSize = 30;
        GUI.Label(new Rect(10, 10, 200, 40), currentRowIndex + "," + currentColumnIndex, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 40), "row normal " + rowNormal.forward + " - cam normal " + Camera.main.transform.forward, guiStyle);
        float dot = Vector3.Dot(rowNormal.forward, Camera.main.transform.forward);
        GUI.Label(new Rect(10, 90, 200, 40), "dot " + dot.ToString(), guiStyle);
    }

    public void Populate(bool i_animated)
    {
        int count = anchors.Length;

        DoraKernel[] kernels = new DoraKernel[count];
        DoraKernel curr = null;

        for (int i = 0; i < count; i++)
        {
            GameObject go = GameObject.Instantiate(kernel);
            go.transform.SetParent(anchors[i]);
            go.transform.localPosition = MathConstants.VECTOR_3_ZERO;
            go.transform.localRotation = MathConstants.QUATERNION_IDENTITY;
            go.transform.localScale = MathConstants.VECTOR_3_ONE;

            curr = kernels[i] = go.GetComponent<DoraKernel>();
            curr.Init(interpolators);

            if (false == i_animated)
                curr.Appear(false);
        }

        kernelMap = CollectionUtilities.Make2DArray<DoraKernel>(kernels, 12, 11);

        for(int i = 0; i < 12; i++)
        {
            for(int j = 0; j < 11; j++)
            {
                kernelMap[i, j].name = i + "," + j;
            }
        }

        if(true == i_animated)
        {
            StartCoroutine(populateAnimated());
        }

    }

    IEnumerator populateAnimated()
    {
        for (int i = 0; i < 12; i++)
        {
            updateRowIndex(i);
            for (int j = 0; j < 11; j++)
            {
                kernelMap[i, j].Appear(true);
                yield return new WaitForSeconds(0.05f);
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
        for (int i = 0; i < 12; i++)
        {
            updateRowIndex(i);

            for (int j = 0; j < 11; j++)
            {
                currentColumnIndex = j;

                StartCoroutine(bounceScale(kernelMap[i, j].transform));

                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    IEnumerator bounceScale(Transform i_tr)
    {
        AnimationMode mode = new AnimationMode(AnimationType.Bounce);
        ITypedAnimator<Vector3> scaleInterpolator = interpolators.Animate(i_tr.localScale, i_tr.localScale * 2f, 0.5f, mode, false);
        while(true == scaleInterpolator.IsActive)
        {
            i_tr.localScale = scaleInterpolator.Current;
            yield return null;
        }
    }

    void updateRowIndex(int i_rowIndex)
    {
        currentRowIndex = i_rowIndex;
        rowNormal = normalAnchors[currentRowIndex];
    }

}