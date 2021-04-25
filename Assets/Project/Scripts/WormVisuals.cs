using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WormVisuals : MonoBehaviour
{
    public Sprite body;

    //public float randomVariance = 1;
    public float segmentSpacingMax = 5f;
    public int segmentCount = 10;
    public float bodyRotateSpeed = 5f;

    [ChildGameObjectsOnly]
    public SpriteRenderer head;

    List<GameObject> segments;
    List<Vector3> positions;
    List<float> angles;
    void Start()
    {
        segments = new List<GameObject>();
        positions = new List<Vector3>();
        angles = new List<float>();
    }
    public void CreateBody()
    {
        head.sortingOrder = segmentCount + 1;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject obj = new GameObject("body " + i, new Type[] { typeof(SpriteRenderer) });
            obj.GetComponent<SpriteRenderer>().sprite = body;
            obj.GetComponent<SpriteRenderer>().sortingOrder = segmentCount - i;

            obj.transform.SetParent(this.transform);

            obj.transform.localPosition = new Vector3(-segmentSpacingMax, 0f, 0f);
            segments.Add(obj);
            positions.Add(Vector3.zero);
            angles.Add( 0f );
        }
    }

    /// <summary>
    /// Moves the entire body
    /// </summary>
    public void MoveBody(Vector3 position)
    {
        head.transform.position = position;
        for (int i = 0; i < positions.Count; i++)
        {
            positions[i] = position;
            angles[i] = 0;
        }
    }

    void Update()
    {
        if (positions == null || positions.Count <= 0 || segments == null || segments.Count <= 0)
            return;

        positions[0] = VectorExtras.AnchoredMovePosTowardTarget(head.transform.position, positions[0], segmentSpacingMax);

        float ang = VectorExtras.VectorToDegrees( VectorExtras.Direction( segments[0].transform.position, head.transform.position ) );
        angles[0] = Mathf.MoveTowardsAngle( angles[0], ang, bodyRotateSpeed * Time.deltaTime );
        segments[0].transform.rotation = Quaternion.AngleAxis(angles[0], Vector3.forward);
        segments[0].transform.position = positions[0];

        Vector3 nextPos = VectorExtras.AnchoredMovePosTowardTarget(head.transform.position, positions[0], segmentSpacingMax);
        for (int i = 1; i < segments.Count; i++)
        {
            positions[i] = VectorExtras.AnchoredMovePosTowardTarget(nextPos, positions[i], segmentSpacingMax / 3f);

            ang = VectorExtras.VectorToDegrees( VectorExtras.Direction( segments[i].transform.position, nextPos ) );
            angles[i] = Mathf.MoveTowardsAngle( angles[i], ang, bodyRotateSpeed * Time.deltaTime );
            segments[i].transform.rotation = Quaternion.AngleAxis(angles[i], Vector3.forward);
            segments[i].transform.position = positions[i];

            nextPos = VectorExtras.AnchoredMovePosTowardTarget(positions[i - 1], positions[i], segmentSpacingMax / 3f);
        }

    }
}
