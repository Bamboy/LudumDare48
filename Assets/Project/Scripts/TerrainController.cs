using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class TerrainController : MonoBehaviour
{
    SpriteShapeController ssc;
    ShadowCaster2DController shadowCaster2DController;

    List<Vector2> path;

    [Range(3, 100)]
    public float caveWidth = 3;

    [Range(1, 10)]
    public int pointDensity = 1;

    [Range(1, 100)]
    public float noiseIntensity = 1;

    List<Vector2> leftPoints;
    List<Vector2> rightPoints;

    [Button]
    void Start()
    {
        ssc = GetComponent<SpriteShapeController>();
        shadowCaster2DController = GetComponent<ShadowCaster2DController>();

        path = new List<Vector2>();
        path.Add(Vector2.zero);
        path.Add(new Vector2(-100, -100));
        path.Add(new Vector2(-300, -150));

        leftPoints = new List<Vector2>();
        rightPoints = new List<Vector2>();

        UpdateTerrain();
    }

    public void UpdateTerrain()
    {
        leftPoints.Clear();
        rightPoints.Clear();

        float pathLength = Vector2.Distance(path[0], path[1]);
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 segment = path[i] - path[i - 1];
            Vector2 perp = Vector2.Perpendicular(segment).normalized;

            Vector2 startPerpCutoff = perp;
            if (i > 1)
            {
                Vector2 prevSegment = path[i - 1] - path[i - 2];
                Vector2 prevPerp = Vector2.Perpendicular(prevSegment).normalized;
                startPerpCutoff = ((startPerpCutoff + prevPerp) / 2).normalized;
            }

            Vector2 endPerpCutoff = perp;
            if (i < path.Count - 1)
            {
                Vector2 nextSegment = path[i + 1] - path[i];
                Vector2 nextPerp = Vector2.Perpendicular(nextSegment).normalized;
                endPerpCutoff = ((endPerpCutoff + nextPerp) / 2).normalized;
            }

            Debug.DrawLine(path[i - 1], path[i - 1] + startPerpCutoff * 100, Color.magenta);
            Debug.DrawLine(path[i], path[i] + endPerpCutoff * 100, Color.cyan);

            for (int j = 0; j < segment.magnitude; j += pointDensity)
            {
                Vector2 point = Vector2.Lerp(path[i - 1], path[i], j / segment.magnitude);
                float distanceAlongPath = pathLength + Vector2.Distance(path[i - 1], point);
                float leftNoise = Mathf.PerlinNoise(distanceAlongPath, 0) * noiseIntensity;
                float rightNoise = Mathf.PerlinNoise(distanceAlongPath, 100) * noiseIntensity;

                Vector2 leftPoint = point - ((perp * caveWidth) + (perp * leftNoise));
                Vector2 rightPoint = point + ((perp * caveWidth) + (perp * rightNoise));

                if (!isLeft(path[i - 1], path[i - 1] + startPerpCutoff, leftPoint) && isLeft(path[i], path[i] + endPerpCutoff, leftPoint))
                {
                    leftPoints.Add(leftPoint);
                }
                if (!isLeft(path[i - 1], path[i - 1] + startPerpCutoff, rightPoint) && isLeft(path[i], path[i] + endPerpCutoff, rightPoint))
                {
                    rightPoints.Add(rightPoint);
                }
            }

            pathLength += segment.magnitude;
        }
    }

    void Update()
    {
        UpdateTerrain();

        for (int i = 1; i < path.Count; i++)
        {
            Debug.DrawLine(path[i - 1], path[i], Color.white);
        }

        for (int i = 1; i < leftPoints.Count; i++)
        {
            Debug.DrawLine(leftPoints[i - 1], leftPoints[i], Color.blue);
        }

        for (int i = 1; i < rightPoints.Count; i++)
        {
            Debug.DrawLine(rightPoints[i - 1], rightPoints[i], Color.red);
        }

    }

    void OnDrawGizmos()
    {
        foreach (Vector2 p in leftPoints)
        {
            DebugExtension.DrawPoint(p, Color.blue);
        }

        foreach (Vector2 p in rightPoints)
        {
            DebugExtension.DrawPoint(p, Color.red);
        }
    }

    void UpdateSpline()
    {
        ssc.spline.Clear();

        List<Vector2> points = new List<Vector2>();
        points.Add(new Vector2(-1, -1));
        points.Add(new Vector2(-1, 1));
        points.Add(new Vector2(1, 1));
        points.Add(new Vector2(1, -1));

        for (int i = 0; i < points.Count; i++)
        {
            ssc.spline.InsertPointAt(i, points[i]);
        }

        shadowCaster2DController.UpdateShadowFromPoints(points.ToArray());
    }

    public bool isLeft(Vector2 a, Vector2 b, Vector2 c)
    {
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
    }
}
