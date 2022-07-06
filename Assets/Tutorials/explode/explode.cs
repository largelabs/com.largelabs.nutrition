using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explode : MonoBehaviour
{
    [SerializeField] public int cubesPerAxis = 8;
    [SerializeField] public float delay = 1f;
    [SerializeField] public float force = 300f;
    [SerializeField] public float radius = 2f;


    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Main()
    {
        for (int x = 0; x < cubesPerAxis; x++)
        {
            for (int y = 0; y < cubesPerAxis; y++)
            {
                for (int z = 0; z < cubesPerAxis; z++)
                {
                    CreateCube(new Vector3(x, y, z));
                }
            }
        }

        Destroy(gameObject);
    }

    void CreateCube(Vector3 coordinates)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        Renderer rd = cube.GetComponent<Renderer>();
        rd.material = GetComponent<Renderer>().material;

        cube.transform.localScale = transform.localScale / cubesPerAxis;

        Vector3 firstCube = transform.position - (transform.localScale / 2) + (cube.transform.localScale / 2);
        cube.transform.position = firstCube + Vector3.Scale(coordinates, cube.transform.localScale);

        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.AddExplosionForce(force, transform.position, radius);
    }
    // Update is called once per frame
    void Update()
{
        }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "bullet")
            Invoke("Main",0);

    }
}
