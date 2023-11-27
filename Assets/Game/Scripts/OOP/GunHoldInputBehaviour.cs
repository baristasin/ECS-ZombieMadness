using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHoldInputBehaviour : MonoBehaviour
{
    private Vector3 _firstMousePos;
    private Vector3 _lastMousePos;
    private Vector3 _mousePos;

    [SerializeField] private Transform _gunHoldTransform;
    [SerializeField] private float _sensitivity;

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _firstMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            _mousePos = Input.mousePosition;

            var movementVector = _mousePos - _firstMousePos;

            if(movementVector.x > 0.1f)
            {
                movementVector.x = Mathf.Clamp(movementVector.x,-1, 1);
                _firstMousePos = _mousePos;
                _gunHoldTransform.localPosition += new Vector3(movementVector.x,0,0) * _sensitivity * Time.deltaTime;
            }
            else if(movementVector.x < -0.1f)
            {
                movementVector.x = Mathf.Clamp(movementVector.x, -1, 1);
                _firstMousePos = _mousePos;
                _gunHoldTransform.localPosition += new Vector3(movementVector.x, 0, 0) * _sensitivity * Time.deltaTime;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _lastMousePos = Input.mousePosition;
        }
    }
}
