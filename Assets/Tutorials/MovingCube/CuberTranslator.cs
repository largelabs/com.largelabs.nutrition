using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuberTranslator : MonoBehaviour
{
    [SerializeField] float min = 3f;
    [SerializeField] float max = 7f;
    [SerializeField] float speed = 1f;
    [SerializeField] Color finalColor = Color.yellow;

    MeshRenderer rnd = null;
    Material mat = null;

    private void Awake()
    {
        setPosX(min);
        rnd = GetComponent<MeshRenderer>();
        mat = rnd.material;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        updatePos();
    }

    void updatePos()
    {

        Vector3 position = transform.position;
        if (position.x >= max || position.x < min) return;

        float dt = Time.deltaTime;

        float x = position.x + speed * dt;
        if (x > max)
        {
            x = max;
            updateColor(finalColor);
        }

        setPosX(x);
    }

    void updateColor(Color i_col)
    {
        mat.color = i_col;
    }

    void setPosX(float i_x)
    {
        Vector3 position = transform.position;
        position.x = i_x;
        transform.position = position;
    }

    private void LateUpdate()
    {
        
    }


    private void OnDestroy()
    {
        
    }

}
