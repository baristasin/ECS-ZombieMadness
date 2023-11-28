using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public float CamSpeed { get; set; }

    public Transform CamTransform => _openingCam;
    public Transform GunHoldTransform => _gunHoldTransform;

    [SerializeField] private GunHoldInputBehaviour _gunHoldInputBehaviour;
    [SerializeField] private Transform _openingCam;
    [SerializeField] private Transform _gameCam;
    [SerializeField] private Transform _gunHoldTransform;
    [SerializeField] private Transform _gameCamWeaponFirstTransform;

    public void SwitchToGameCam()
    {
        _gunHoldTransform.transform.SetParent(_gameCamWeaponFirstTransform);
        _gunHoldTransform.transform.localPosition = new Vector3(0,0,0);
        _gunHoldTransform.transform.rotation = Quaternion.Euler(0, 180f, 0);

        _openingCam.gameObject.SetActive(false);
        _gunHoldInputBehaviour.gameObject.SetActive(true);
    }

    void Update()
    {
        if (_openingCam.gameObject.activeSelf)
        {
            _openingCam.position += Vector3.forward * Time.deltaTime * CamSpeed;
        }

        if (Input.GetKeyDown("space"))
        {
            SwitchToGameCam();
        }

        _gameCam.position += Vector3.forward * Time.deltaTime * CamSpeed;
    }
}
