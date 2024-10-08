using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{

    [SerializeField] private bool _upsideCameraActive = false;
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _sensitivity = 5.0f;
    [SerializeField] private Camera _freeCamera;
    [SerializeField] private Camera _upsideCamera;

    private void OnEnable() => CameraEvents.OnTogglingView += SetCamera;
    private void OnDisable() => CameraEvents.OnTogglingView -= SetCamera;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1)) // If right click is pressed
        {
            Cursor.visible = false;
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            float axisY = Input.GetAxis("Vertical");
            float axisX = Input.GetAxis("Horizontal");
            if (_upsideCameraActive)
            {
                _upsideCamera.transform.position += _upsideCamera.transform.forward * mouseY * _speed*2 * Time.deltaTime;
                
                _upsideCamera.transform.position += _upsideCamera.transform.up * axisY * _speed * Time.deltaTime;
                _upsideCamera.transform.position += _upsideCamera.transform.right * axisX * _speed * Time.deltaTime;
            }
            else
            {
                // Move the camera forward, backward, left, and right
                _freeCamera.transform.position += _freeCamera.transform.forward * axisY * _speed * Time.deltaTime;
                _freeCamera.transform.position += _freeCamera.transform.right * axisX * _speed * Time.deltaTime;

                // Rotate the camera based on the mouse movement
                _freeCamera.transform.eulerAngles += new Vector3(-mouseY * _sensitivity, mouseX * _sensitivity, 0);
            }
        }
        else
        {
            Cursor.visible = true;
        }
    }

    private void SetCamera(bool upsideView)
    {
        _upsideCameraActive = upsideView;
        _upsideCamera.gameObject.SetActive(_upsideCameraActive);
        _freeCamera.gameObject.SetActive(!_upsideCameraActive);
    }
}
