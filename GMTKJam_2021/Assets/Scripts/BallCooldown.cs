using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCooldown : MonoBehaviour
{
    public float timeRemaining = 10;
    public bool timerIsRunning = false;
    public bool canShoot;

    private void Start()
    {
        timerIsRunning = false;
        canShoot = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) StartTimer();
        if (timerIsRunning) Tick();

    }

    public void StartTimer()
    {
        timeRemaining = 1;
        timerIsRunning = true;
    }

    public void Tick()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            canShoot = false;
        }
        else
        {
            ResetTimer();
        }
    }
    public void ResetTimer()
    {
        canShoot = true;
        timeRemaining = 0;
        timerIsRunning = false;
    }
}