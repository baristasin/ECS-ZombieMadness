using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadParentController : MonoBehaviour
{
    [SerializeField] private float _roadPrefabSpeed;

    void Update()
    {
        transform.position -= new Vector3(0, 0, 1f) * Time.deltaTime * _roadPrefabSpeed;
    }
}
