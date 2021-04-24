using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WormVisuals : MonoBehaviour
{

    public Sprite body;
    

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

        for( int i = 0; i < segmentCount; i++ )
        {
            GameObject obj = new GameObject("body "+i, new Type[] { typeof( SpriteRenderer ) } );
            obj.GetComponent<SpriteRenderer>().sprite = body;
            obj.GetComponent<SpriteRenderer>().sortingOrder = -i;

            obj.transform.SetParent( this.transform );

            obj.transform.localPosition = new Vector3(-segmentSpacingMax, 0f, 0f);
            segments.Add( obj );
            positions.Add( Vector3.zero );
        }

    }

    void Update()
    {
        //segments[0].transform.SetParent( head.transform, false );

        //Vector3 pos = head.transform.position - new Vector3(-segmentSpacing, 0f, 0f);

        positions[0] = VectorExtras.AnchoredMovePosTowardTarget( head.transform.position, positions[0], segmentSpacingMax );//Vector3.MoveTowards(positions[0], head.transform.position, 1f);
        
        float a = Vector2.Angle( segments[0].transform.position, head.transform.position );
        segments[0].transform.rotation = Quaternion.AngleAxis( a, Vector3.forward );
        segments[0].transform.position = positions[0];

        Vector3 nextPos = VectorExtras.AnchoredMovePosTowardTarget( head.transform.position, positions[0], segmentSpacingMax );
        for( int i = 1; i < segments.Count; i++ )
        {
            positions[i] = VectorExtras.AnchoredMovePosTowardTarget( nextPos, positions[i], segmentSpacingMax );//Vector3.MoveTowards(positions[i], positions[i-1], 0.5f);

            
            float ang = Vector2.Angle( segments[i].transform.position, nextPos ); //segments[i-1].transform.position
            segments[i].transform.rotation = Quaternion.AngleAxis( ang, Vector3.forward );

            segments[i].transform.position = positions[i];


            nextPos = VectorExtras.AnchoredMovePosTowardTarget( positions[i-1], positions[i], segmentSpacingMax );
                //Direction(segments[i].transform.position, nextPos) * segmentSpacing + segments[i-1].transform.position;
            
        }

    }

    static Vector3 Direction( Vector3 origin, Vector3 target )
    {
        return (target - origin).normalized;
    }
}
