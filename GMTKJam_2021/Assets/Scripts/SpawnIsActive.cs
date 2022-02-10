using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIsActive : MonoBehaviour
{

    public bool isActive = false;

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }
}
