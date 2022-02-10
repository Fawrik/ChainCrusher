using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawtest : MonoBehaviour
{

    public GameObject sourceGO, targetGO, desiredTargetGO;

    public float scalar = 2;
    private void OnDrawGizmos()
    {
        var source = sourceGO.transform.position;
        var target = targetGO.transform.position;
        var desiredTarget = desiredTargetGO.transform.position;

        var dir =source+ (target - source).normalized * scalar;



        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(source, .25f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(target, .25f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(desiredTarget, .25f);

        Debug.DrawLine(source, dir, Color.green);

    }
}
