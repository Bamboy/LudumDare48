using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class TerrainController : MonoBehaviour
{
    public List<Vector2> path;

    [Range(1, 100)]
    public float caveWidth = 1;

    [Range(1, 300)]
    public int segmentsPerChunk = 1;

    [Range(1, 1000)]
    public int segmentLength = 1;

    [Range(1, 1000)]
    public float pathNoiseIntensity = 1;

    [Range(0.01f, 1f)]
    public float pathNoiseFactor = 1;

    [Range(1, 100)]
    public int terrainPointDensity = 1;

    [Range(1, 100)]
    public float terrainNoiseIntensity = 1;

    [Range(0.01f, 1f)]
    public float terrainNoiseFactor = 1;

    List<Vector2> bottomPoints;
    List<Vector2> topPoints;

    public TerrainSection topSection;
    public TerrainSection bottomSection;

    public Transform playerTransform;

    float previousChunksDistance;

    [Button]
    void Start()
    {
        UpdatePath();

        bottomPoints = new List<Vector2>();
        topPoints = new List<Vector2>();

        UpdateTerrain();
    }

    void UpdatePath()
    {
        path = new List<Vector2>();
        path.Add(new Vector2(-segmentLength, segmentLength));
        path.Add(Vector2.zero);

        for (int i = 0; i < segmentsPerChunk; i++)
        {
            float noise = Mathf.PerlinNoise(i * pathNoiseFactor, 1337) * 2 - 1;
            Vector2 point = new Vector2(1, -0.5f) * i * segmentLength + (Vector2.one * noise * pathNoiseIntensity);
            path.Add(point);
        }
    }

    public void UpdateTerrain()
    {
        bottomPoints.Clear();
        topPoints.Clear();

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

            // Debug.DrawLine(path[i - 1], path[i - 1] + startPerpCutoff * 100, Color.magenta);
            // Debug.DrawLine(path[i], path[i] + endPerpCutoff * 100, Color.cyan);

            for (int j = 0; j < segment.magnitude; j += terrainPointDensity)
            {
                Vector2 point = Vector2.Lerp(path[i - 1], path[i], j / segment.magnitude);
                float distanceAlongPath = pathLength + Vector2.Distance(path[i - 1], point);
                float bottomNoise = Mathf.PerlinNoise(distanceAlongPath * terrainNoiseFactor, 0) * terrainNoiseIntensity;
                float topNoise = Mathf.PerlinNoise(distanceAlongPath * terrainNoiseFactor, 100) * terrainNoiseIntensity;

                Vector2 bottomPoint = point - ((perp * caveWidth) + (perp * bottomNoise));
                Vector2 topPoint = point + ((perp * caveWidth) + (perp * topNoise));

                if (!isLeft(path[i - 1], path[i - 1] + startPerpCutoff, bottomPoint) && isLeft(path[i], path[i] + endPerpCutoff, bottomPoint))
                {
                    bottomPoints.Add(bottomPoint);
                }
                if (!isLeft(path[i - 1], path[i - 1] + startPerpCutoff, topPoint) && isLeft(path[i], path[i] + endPerpCutoff, topPoint))
                {
                    topPoints.Add(topPoint);
                }
            }

            pathLength += segment.magnitude;
        }

        float skirtPadding = 100;
        topPoints.Insert(0, topPoints[0] + Vector2.one * skirtPadding);
        topPoints.Add(topPoints[topPoints.Count - 1] + new Vector2(1, -1) * skirtPadding);
        bottomPoints.Insert(0, bottomPoints[0] + new Vector2(-1, 1) * skirtPadding);
        bottomPoints.Add(bottomPoints[bottomPoints.Count - 1] + -Vector2.one * skirtPadding);

        Vector2 topMax = new Vector2(float.MinValue, float.MinValue);
        foreach (Vector2 p in topPoints)
        {
            if (p.x > topMax.x)
            {
                topMax = new Vector2(p.x, topMax.y);
            }
            if (p.y > topMax.y)
            {
                topMax = new Vector2(topMax.x, p.y);
            }
        }

        Vector2 bottomMin = new Vector2(float.MaxValue, float.MaxValue);
        foreach (Vector2 p in bottomPoints)
        {
            if (p.x < bottomMin.x)
            {
                bottomMin = new Vector2(p.x, bottomMin.y);
            }
            if (p.y < bottomMin.y)
            {
                bottomMin = new Vector2(bottomMin.x, p.y);
            }
        }

        topPoints.Insert(0, topMax);
        bottomPoints.Insert(0, bottomMin);

        if (Application.isPlaying)
        {
            topSection.UpdateSpline(topPoints);
            bottomSection.UpdateSpline(bottomPoints);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        UpdatePath();
        UpdateTerrain();
#endif

        for (int i = 1; i < path.Count; i++)
        {
            Debug.DrawLine(path[i - 1], path[i], Color.white);
        }

        for (int i = 1; i < bottomPoints.Count; i++)
        {
            Debug.DrawLine(bottomPoints[i - 1], bottomPoints[i], Color.blue);
        }

        for (int i = 1; i < topPoints.Count; i++)
        {
            Debug.DrawLine(topPoints[i - 1], topPoints[i], Color.red);
        }

    }

    int ClosestSegmentIndexToPoint(Vector2 point)
    {
        float shortestDistance = float.MaxValue;
        int currentSegmentIndex = 0;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 projectedPos = VectorExtras.ProjectPointOnLineSegment(path[i - 1], path[i], point);
            float dist = Vector2.Distance(projectedPos, point);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                currentSegmentIndex = i;
            }
        }

        return currentSegmentIndex;
    }

    float PointDistanceAlongPath(Vector2 point)
    {
        int currentSegmentIndex = ClosestSegmentIndexToPoint(point);

        float distance = 0;
        for (int i = 1; i < currentSegmentIndex; i++)
        {
            distance += Vector2.Distance(path[i - 1], path[i]);
        }
        Vector2 projected = VectorExtras.ProjectPointOnLineSegment(path[currentSegmentIndex - 1], path[currentSegmentIndex], point);
        distance += Vector2.Distance(path[currentSegmentIndex - 1], projected);

        return distance;
    }

    Vector2 ClosestPointAlongPath(Vector2 point)
    {
        int currentSegmentIndex = ClosestSegmentIndexToPoint(point);
        return VectorExtras.ProjectPointOnLineSegment(path[currentSegmentIndex - 1], path[currentSegmentIndex], point);
    }

    void OnDrawGizmos()
    {
        DebugExtension.DrawPoint(ClosestPointAlongPath(playerTransform.position), Color.cyan);

        foreach (Vector2 p in bottomPoints)
        {
            DebugExtension.DrawPoint(p, Color.blue);
        }

        foreach (Vector2 p in topPoints)
        {
            DebugExtension.DrawPoint(p, Color.red);
        }
    }

    public bool isLeft(Vector2 a, Vector2 b, Vector2 c)
    {
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
    }
}
