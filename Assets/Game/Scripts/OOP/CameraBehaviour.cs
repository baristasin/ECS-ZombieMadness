using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _openingCam1;
    [SerializeField] private Transform _openingCam2;

    public void SwitchToGameCam()
    {
        if (_openingCam1.gameObject.activeSelf)
        {
        _openingCam1.gameObject.SetActive(false);
            return;
        }

        if (_openingCam2.gameObject.activeSelf)
        {
            _openingCam2.gameObject.SetActive(false);
            return;
        }        
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SwitchToGameCam();
        }
    }
}
