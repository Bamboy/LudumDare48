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

    [ChildGameObjectsOnly]
    public SpriteRenderer head;

    List<GameObject> segments;
    List<Vector3> positions;
    void Start()
    {
        segments = new List<GameObject>();
        positions = new List<Vector3>();
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
        }
    }

    void Update()
    {
        if (positions == null || positions.Count <= 0 || segments == null || segments.Count <= 0)
            return;

        positions[0] = VectorExtras.AnchoredMovePosTowardTarget(head.transform.position, positions[0], segmentSpacingMax);

        float a = Vector2.Angle(segments[0].transform.position, head.transform.position);
        segments[0].transform.rotation = Quaternion.AngleAxis(a, Vector3.forward);
        segments[0].transform.position = positions[0];

        Vector3 nextPos = VectorExtras.AnchoredMovePosTowardTarget(head.transform.position, positions[0], segmentSpacingMax);
        for (int i = 1; i < segments.Count; i++)
        {
            positions[i] = VectorExtras.AnchoredMovePosTowardTarget(nextPos, positions[i], segmentSpacingMax);

            float ang = Vector2.Angle(segments[i].transform.position, nextPos);
            segments[i].transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);
            segments[i].transform.position = positions[i];
            // + new Vector3(UnityEngine.Random.Range(-randomVariance, randomVariance),UnityEngine.Random.Range(-randomVariance, randomVariance),0f)
            nextPos = VectorExtras.AnchoredMovePosTowardTarget(positions[i - 1], positions[i], segmentSpacingMax);
        }

    }
}
