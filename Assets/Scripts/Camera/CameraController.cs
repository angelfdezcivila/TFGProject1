using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private bool _upsideCamera = false;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float sensitivity = 5.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            if (_upsideCamera)
            {
                // transform.position += transform.right * mouseX * speed * Time.deltaTime;
                transform.position += transform.forward * mouseY * speed * Time.deltaTime;
                
                transform.position += transform.up * Input.GetAxis("Vertical") * speed * Time.deltaTime;
                transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            }
            else
            {
                // Move the camera forward, backward, left, and right
                transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
                transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

                // Rotate the camera based on the mouse movement
                transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);
            }
        }
        else
        {
            Cursor.visible = true;
        }
    }
}
