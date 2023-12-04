using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _openingCam;

    public void SwitchToGameCam()
    {
        _openingCam.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SwitchToGameCam();
        }
    }
}
