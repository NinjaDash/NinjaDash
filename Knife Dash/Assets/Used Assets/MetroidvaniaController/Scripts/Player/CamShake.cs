using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamShake : MonoBehaviour
{
	CinemachineVirtualCamera VCam;
	CinemachineBasicMultiChannelPerlin shakeCam;
	[SerializeField] float intensity;
	[SerializeField] float duration = 0.2f;
	private float Shaketimer;

	
	private void Awake()
	{
		VCam = GetComponent<CinemachineVirtualCamera>();
		shakeCam = VCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
	}

	public void ShakeCamera()
	{
		shakeCam.m_AmplitudeGain = intensity;
		Shaketimer = duration;
	}

	private void Update()
	{
		if (Shaketimer > 0)
		{
			Shaketimer -= Time.deltaTime;
			if (Shaketimer <= 0)
			{
				shakeCam.m_AmplitudeGain = 0;
			}
		}
	}
}
