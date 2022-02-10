using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [SerializeField]
    float SpawnTime = 20f;

    [SerializeField]
    Planet planetPrefab;
    float timeStamp = 0f;
    void Update()
    {
        if (Time.time > timeStamp)
        {
            timeStamp = Time.time + SpawnTime;
            Planet planet = Instantiate(planetPrefab);
        }
        
    }
}
