using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_mouse_movements : MonoBehaviour

{
    
    [SerializeField] private Vector2 sensitivity;
    private Vector2 rotation; // the current rotation in degrees

    // Start is called before the first frame update
    void Start()
    {
        
    }

   

    // Update is called once per frame
    void Update()
    {
        Vector2 watendedVelocity = GetInput() * sensitivity;
        rotation += watendedVelocity * Time.deltaTime;
        transform.localEulerAngles = new Vector3(rotation.y, rotation.x, 0);

        
    }
    private Vector2 GetInput()
    {
        // getting the input for controls (keyboard and mouse)
        Vector2 input = new Vector2(
        Input.GetAxis("Mouse X"),
        Input.GetAxis("Mouse Y"));
        return input;
    }
}
