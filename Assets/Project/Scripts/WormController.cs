using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WormController : MonoBehaviour
{
    public static WormController Singleton { get; private set; }

    [ChildGameObjectsOnly]
    public Transform head;

    public float speed = 0.8f;

    private WormVisuals visuals;

    private void Awake()
    {
        visuals = GetComponent<WormVisuals>();
        if( Singleton == null )
            Singleton = this;
    }



    public bool isChasing;
    public void StartChasing( Vector3 spawnPosition )
    {
        isChasing = true;
        visuals.CreateBody();
        visuals.MoveBody( spawnPosition );
        
    }

    
    void Update()
    {
        if( isChasing == false )
            return;

        int index = TerrainController.Singleton.ClosestSegmentIndexToPoint( head.transform.position );
        index = Mathf.Min( index + 1, TerrainController.Singleton.path.Count - 1 );
        head.transform.position = Vector3.MoveTowards( head.transform.position, TerrainController.Singleton.path[ index ], speed );

    }
}
