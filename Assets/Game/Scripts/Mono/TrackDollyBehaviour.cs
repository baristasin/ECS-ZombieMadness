using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TrackDollyBehaviour : MonoBehaviour
{
    [SerializeField] private CinemachineSmoothPath _trackDollyPath;
    [SerializeField] private Transform _trackDollyTransform;
    [SerializeField] private float _trackDollySpeed;

    private float _distance;

    private bool _isInitialized;

    private void Start()
    {
        _trackDollyTransform.position = _trackDollyPath.EvaluatePositionAtUnit(_distance, CinemachinePathBase.PositionUnits.Distance);
        _trackDollyTransform.rotation = _trackDollyPath.EvaluateOrientationAtUnit(_distance, CinemachinePathBase.PositionUnits.Distance) * Quaternion.Euler(0, 180f, 0);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.U))
        {
            _isInitialized = true;
        }

        if (!_isInitialized) return;

        _trackDollyTransform.position = _trackDollyPath.EvaluatePositionAtUnit(_distance, CinemachinePathBase.PositionUnits.Distance);
        _trackDollyTransform.rotation = _trackDollyPath.EvaluateOrientationAtUnit(_distance, CinemachinePathBase.PositionUnits.Distance) * Quaternion.Euler(0,180f,0);
        _distance += _trackDollySpeed * Time.deltaTime;

        //_trackDollySpeed += Time.deltaTime;

    }
}
