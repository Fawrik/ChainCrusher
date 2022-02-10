using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using Random = UnityEngine.Random;

public class CameraBrain : MonoBehaviour
{
    public Transform player;
    public CinemachineVirtualCamera vCam;

    private void Awake()
    {
        vCam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void CamShake(float duration, float strength) //works best with values between .1 and .5 //ScreenShake that can be called easily from anywhere
    {
        StartCoroutine(CamShakeCoroutine(duration, strength));
    }

    private IEnumerator CamShakeCoroutine(float duration, float strength) 
    {
        Vector3 originalPosition = new Vector3(0, 5, -10);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-.5f, .5f) * strength;
            float yOffset = Random.Range(-.5f, .5f) * strength;
            transform.localPosition = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null; //Waits for one frame
        }

        transform.position = originalPosition;
    }

    public void ShakeCamera(float duration, float intensity)
    {
        StartCoroutine(ShakeCameraCoroutine(duration, intensity));
    }

    private IEnumerator ShakeCameraCoroutine(float duration, float intensity)
    {
        CinemachineBasicMultiChannelPerlin cinMachPerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
       
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            cinMachPerlin.m_AmplitudeGain = intensity;
            elapsedTime += Time.deltaTime;
            yield return null; //Waits for one frame
        }
        cinMachPerlin.m_AmplitudeGain = 0;
    }
}