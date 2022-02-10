using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    [HideInInspector] public CarController cc;
    public Transform currentTarget;
    public Transform[] chainLinks;
    public LineRenderer lr;
    private List<Transform> lineRendererPositionsList = new List<Transform>();

    public void ConnectChainToObject(Transform target)
    {
        currentTarget = target;
        var joint = currentTarget.parent.root.GetComponentInChildren<Joint2D>();
        if (joint != null) joint.connectedBody = cc.rb;
        else
        {
            joint = target.parent.root.gameObject.AddComponent<HingeJoint2D>();
            joint.connectedBody = cc.rb;
        }
    }

    public void DisconnectObject()
    {
        currentTarget.GetComponentInChildren<Joint2D>().connectedBody = null;
        currentTarget = null;
        InitializeLineRendererPositions();
    }

    private void LateUpdate()
    {
        for (int i = 0; i < lineRendererPositionsList.Count; i++)
        {
            lr.SetPosition(i, lineRendererPositionsList[i].position);
        }
    }

    public void InitializeLineRendererPositions()
    {
        lineRendererPositionsList.Clear();
        lineRendererPositionsList.Add(cc.transform);
        foreach (Transform transform in chainLinks) lineRendererPositionsList.Add(transform);
        if (currentTarget != null) lineRendererPositionsList.Add(currentTarget);
        lr.positionCount = lineRendererPositionsList.Count;
    }

    private void Awake()
    {
        cc = FindObjectOfType<CarController>();
        InitializeLineRendererPositions();
    }
}
