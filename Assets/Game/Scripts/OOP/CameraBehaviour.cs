using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _camTransform;
    [SerializeField] private float _camSpeed;

    void Update()
    {
        _camTransform.position += Vector3.forward * Time.deltaTime * _camSpeed;
    }
}
