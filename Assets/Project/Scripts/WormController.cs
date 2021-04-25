using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WormController : MonoBehaviour
{
    public static WormController Singleton { get; private set; }

    [ChildGameObjectsOnly]
    public Transform head;
    private WormVisuals visuals;


    public float speed = 0.8f;
    public float chasePlayerDistance = 30f;
    

    


    public float headRotateSpeed = 4f;
    private float headAngle;

    public bool initalized;

    private void Awake()
    {
        visuals = GetComponent<WormVisuals>();
        if( Singleton == null )
            Singleton = this;
    }
    public void StartChasing( Vector3 spawnPosition )
    {
        initalized = true;
        visuals.CreateBody();
        visuals.MoveBody( spawnPosition );
    }



    void Update()
    {
        if( initalized == false )
            return;

        int index = TerrainController.Singleton.ClosestSegmentIndexToPoint( head.transform.position );
        index = Mathf.Min( index + 1, TerrainController.Singleton.path.Count - 1 );

        Vector3 target;

        if( Vector3.Distance( head.position, PlayerController.Singleton.transform.position ) < chasePlayerDistance )
            target = PlayerController.Singleton.transform.position;
        else
            target = TerrainController.Singleton.path[ index ];

        head.position = Vector3.MoveTowards( head.position, target, speed );

        float a = VectorExtras.VectorToDegrees( VectorExtras.Direction( head.position, target ) );
        headAngle = Mathf.MoveTowardsAngle( headAngle, a, headRotateSpeed );
        head.transform.rotation = Quaternion.AngleAxis( headAngle, Vector3.forward );

        Debug.DrawLine(head.position, target, Color.cyan);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere( head.position, chasePlayerDistance );
    }
}
