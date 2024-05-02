using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using StageGenerator;
using UnityEngine;

public class StageEditor : MonoBehaviour
{
    [SerializeField] private LayerMask layersToCheck;
    [SerializeField] private Camera _freeCamera;
    [SerializeField] private Camera _upsideCamera;
    private Camera _camera;
    private Vector3 _lastCursorPosition;

    private void OnEnable() => CameraEvents.OnTogglingView += SetCamera;
    private void OnDisable() => CameraEvents.OnTogglingView -= SetCamera;

    private void SetCamera(bool upsideView)
    {
        _camera = upsideView ? _upsideCamera : _freeCamera;
    }

    private void Awake()
    {
        _camera = _freeCamera;
    }


    void Update()
    {
        // _camera.transform.Translate(Vector3.right*5*Time.deltaTime); // Para testear si la cámara se referencia correctamente
        Cell cell = CellAtCursorPosition();
        if (cell != null)
        {
            if (Input.GetMouseButtonDown(0)) //generar/borrar obstáculo
            {
                
                //Si es una zona válida para obstáculo, empezar con el modo edición
                // En otro caso, si es un obstáculo, borrar y actualizar listas en Stage
            }else if (Input.GetMouseButtonDown(1)) //generar/borrar salida
            {
                //Si es una zona válida para salida, empezar con el modo edición
                // En otro caso, si es un salida, borrar y actualizar listas en Stage
            }

        }
    }

    // Realizamos un Raycast para obtener la posición en el mundo donde el rayo intersecta un objeto cualquiera
    private Cell CellAtCursorPosition()
    {
        Cell cell = null;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit,Mathf.Infinity, layersToCheck))
        {
            cell = hit.transform.parent.gameObject.GetComponentInParent<Cell>();
        }

        return cell;
    }
}
