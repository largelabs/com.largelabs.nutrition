using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    Interpolators i;
    Lerper<Vector3> x;
    public Vector3 y = new Vector3(10,10,10);
    public float time = 2f;
    // Start is called before the first frame update
    void Start()
    {
        i = gameObject.GetComponent<Interpolators>();
        x = i.Lerp_Vector3(new Vector3(0,0,0), y, time);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = x.GetCurrent();
    }
}
