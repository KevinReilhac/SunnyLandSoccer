using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for the camera shake caused by recoil and on taken damage
/// </summary>
public class CameraShake : MonoBehaviour
{
	
	[SerializeField] private Transform camTransform;
	[SerializeField] private float shakeDuration = 0.2f;
	[SerializeField] private float shakeAmount = 0.2f;

	private float _shakeDuration;
	private float _shakeAmount;
	private float _decreaseFactor = 1.0f;
	private Vector3 startPos;
	private Quaternion startRot;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

	private void Start()
	{
		startPos = camTransform.position;
		startRot = camTransform.rotation;
	}

    public void DoCameraShake()
    {
       this._shakeAmount = this.shakeAmount;
       this._shakeDuration = this.shakeDuration;
    }

    void Update()
    {
        if (_shakeDuration > 0) {
            camTransform.localEulerAngles = camTransform.localEulerAngles + Random.insideUnitSphere * _shakeAmount;

            _shakeDuration -= Time.deltaTime * _decreaseFactor;
        }
        else {
            _shakeDuration = 0f;
			camTransform.position = startPos;
			camTransform.rotation = startRot;
        }

    }
}