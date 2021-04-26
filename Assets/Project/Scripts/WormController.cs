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

    public float maxSpeed = 60f;
    public float minSpeed = 40f;
    public float maxSpeedDistance = 100f;
    public float minSpeedDistance = 20f;

    public AnimationCurve speedCurve;

    public float chasePlayerDistance = 30f;
    
    public float headRotateSpeed = 100f;
    private float headAngle;

    [ReadOnly]
    public bool initalized;
    [ReadOnly]
    public float speed;

    private float distToPlayer;

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
            target = TerrainController.Singleton.path[ index ] + (Vector2)TerrainController.Singleton.transform.position;
        
        float myDistance = TerrainController.Singleton.PointDistanceAlongPathTotal( head.position );
        float playerDistance = TerrainController.Singleton.PointDistanceAlongPathTotal( PlayerController.Singleton.transform.position );
        distToPlayer = playerDistance - myDistance;

        float percent = VectorExtras.ReverseLerp(distToPlayer, minSpeedDistance, maxSpeedDistance);
        float curve = speedCurve.Evaluate( percent ) * (maxSpeed - minSpeed);
        speed = curve + minSpeed; //VectorExtras.Remap( minSpeedDistance, maxSpeedDistance, minSpeed, maxSpeed, diff );


        head.position = Vector3.MoveTowards( head.position, target, speed * Time.deltaTime );

        float a = VectorExtras.VectorToDegrees( VectorExtras.Direction( head.position, target ) );
        headAngle = Mathf.MoveTowardsAngle( headAngle, a, headRotateSpeed * Time.deltaTime );
        head.transform.rotation = Quaternion.AngleAxis( headAngle, Vector3.forward );

        Debug.DrawLine(head.position, target, Color.cyan);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        //GUILayout.BeginArea( new Rect(8, 8, 300, 80) );
        GUILayout.BeginVertical();

        GUILayout.TextArea(string.Format("Worm Speed: {0:N2}", speed));
        GUILayout.TextArea(string.Format("Worm Dist: {0:N2}", distToPlayer));
        //GUILayout.TextArea(string.Format("Worm Dist: {0:N2}", speed));
        GUILayout.EndVertical();
        //GUILayout.EndArea();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere( head.position, chasePlayerDistance );
    }
#endif
}
