using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class TerrainController : MonoBehaviour
{
    public static TerrainController Singleton { get; private set; }

    public bool updateSeed = true;
    public int seed;

    public List<Vector2> path;

    [Range(1, 100)]
    public float startingCaveWidth = 1;

    [Range(1, 100)]
    public float minCaveWidth = 1;

    public float distanceToMinCaveWidth = 3000;

    [Range(1, 300)]
    public int segmentsPerChunk = 1;

    [Range(1, 100)]
    public int segmentLength = 1;

    public float pathNoiseScale = 0;
    [Range(1, 8)]
    public int pathNoiseOctaves = 4;
    public float pathNoisePersistence = 0.5f;
    public float pathNoiseLacunarity = 2;

    [Range(1, 100)]
    public int terrainPointDensity = 1;
    public int terrainNoiseScale = 1;
    [Range(1, 8)]
    public int terrainNoiseOctaves = 4;
    public float terrainNoisePersistence = 0.5f;
    public float terrainNoiseLacunarity = 2;

    List<Vector2> bottomPoints;
    List<Vector2> topPoints;

    public TerrainSection topSection;
    public TerrainSection bottomSection;

    public Transform playerTransform;

    float previousChunksDistance;

    public bool autoUpdatePath = true;

    [ReadOnly]
    public int overallIndex = 0;

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
    }

    void Start()
    {
        bottomPoints = new List<Vector2>();
        topPoints = new List<Vector2>();

        Reset();
        UpdateTerrain();
    }

#if UNITY_EDITOR
    private bool _ShowPathButton() { return Application.isPlaying; }
    [Button]
    [EnableIf("_ShowPathButton")]
    void AddPathButton()
    {
        AddPath();
        UpdateTerrain();
    }
    [Button]
    [EnableIf("_ShowPathButton")]
    void RemovePathButton()
    {
        RemovePath();
        UpdateTerrain();
    }
#endif

    void AddPath()
    {
        for (int i = 0; i < segmentsPerChunk; i++)
        {
            int ind = i + overallIndex;
            float noise = Perlin1D(ind, pathNoiseScale, pathNoiseOctaves, pathNoisePersistence, pathNoiseLacunarity);
            Vector2 point;
            float noiseFactor = VectorExtras.Remap(0, 100, 0, 1, ind);
            point = Vector2.down * ind * segmentLength + (Vector2.right * noise * noiseFactor);
            path.Add(point);
        }
        overallIndex += segmentsPerChunk;
    }

    void RemovePath()
    {
        float distance = 0;
        for (int i = 1; i < segmentsPerChunk + 1; i++)
        {
            distance += Vector2.Distance(path[i - 1], path[i]);
        }

        previousChunksDistance += distance;

        path.RemoveRange(0, segmentsPerChunk);
    }

    float Perlin1D(float x, float scale, int octaves, float persistence, float lacunarity, float shift = 0)
    {
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float amplitude = 1;
        float frequency = 1;
        float result = 0;

        for (int i = 0; i < octaves; i++)
        {
            float sample = x / scale * frequency;
            float value = Mathf.PerlinNoise(sample, shift + seed) * 2 - 1;
            result += value * amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return result;
    }

    public void UpdateTerrain()
    {
        bottomPoints.Clear();
        topPoints.Clear();

        float pathLength = 0;
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

                float sample = previousChunksDistance + pathLength + Vector2.Distance(path[i - 1], point);
                float bottomNoise = Perlin1D(sample, terrainNoiseScale, terrainNoiseOctaves, terrainNoisePersistence, terrainNoiseLacunarity, 0);
                float topNoise = Perlin1D(sample, terrainNoiseScale, terrainNoiseOctaves, terrainNoisePersistence, terrainNoiseLacunarity, 100);

                float caveWidth = VectorExtras.Remap(0, distanceToMinCaveWidth, startingCaveWidth, minCaveWidth, sample);

                Vector2 bottomPoint = point - ((perp * caveWidth) + (perp * bottomNoise));
                Vector2 topPoint = point + ((perp * caveWidth) + (perp * topNoise));

                if (!IsLeft(path[i - 1], path[i - 1] + startPerpCutoff, bottomPoint) && IsLeft(path[i], path[i] + endPerpCutoff, bottomPoint))
                {
                    bottomPoints.Add(bottomPoint);
                }
                if (!IsLeft(path[i - 1], path[i - 1] + startPerpCutoff, topPoint) && IsLeft(path[i], path[i] + endPerpCutoff, topPoint))
                {
                    topPoints.Add(topPoint);
                }
            }

            pathLength += segment.magnitude;
        }

        float skirtPadding = 3000;

        topPoints.Insert(0, topPoints[0] + Vector2.right * skirtPadding);
        topPoints.Add(topPoints[topPoints.Count - 1] + Vector2.down * skirtPadding);
        topPoints.Add(topPoints[topPoints.Count - 1] + Vector2.right * skirtPadding);

        bottomPoints.Insert(0, bottomPoints[0] + Vector2.left * skirtPadding);
        bottomPoints.Add(bottomPoints[bottomPoints.Count - 1] + Vector2.down * skirtPadding);
        bottomPoints.Add(bottomPoints[bottomPoints.Count - 1] + Vector2.left * skirtPadding);

        if (Application.isPlaying)
        {
            topSection.UpdateSpline(topPoints);
            bottomSection.UpdateSpline(bottomPoints);
        }
    }

    [Button]
    void Reset()
    {
        if (updateSeed)
        {
            seed = (int)System.DateTime.Now.Millisecond;
        }

        previousChunksDistance = 0;
        overallIndex = 0;
        path = new List<Vector2>();

        AddPath();
        AddPath();
        UpdateTerrain();

        transform.position = -(Vector3)path[0];
    }

    void Update()
    {

#if UNITY_EDITOR
        if (autoUpdatePath && !Application.isPlaying)
        {
            Reset();
        }
#endif 

        Vector2 pos = transform.position;

        for (int i = 1; i < path.Count; i++)
        {
            Debug.DrawLine(pos + path[i - 1], pos + path[i], Color.white);
        }

        for (int i = 1; i < bottomPoints.Count; i++)
        {
            Debug.DrawLine(pos + bottomPoints[i - 1], pos + bottomPoints[i], Color.blue);
        }

        for (int i = 1; i < topPoints.Count; i++)
        {
            Debug.DrawLine(pos + topPoints[i - 1], pos + topPoints[i], Color.red);
        }


        Vector2 playerPos = playerTransform.position;
        int currentSegmentIndex = ClosestSegmentIndexToPoint(playerPos);

        if (currentSegmentIndex > segmentsPerChunk + (segmentsPerChunk / 2.0f))
        {
            RemovePath();
            AddPath();
            UpdateTerrain();

            if (WormController.Singleton.initalized == false)
                WormController.Singleton.StartChasing(pos + path[0]);
        }

    }

    public int ClosestSegmentIndexToPoint(Vector2 point)
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

    public float PointDistanceAlongPath(Vector2 point)
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

    public float PointDistanceAlongPathTotal(Vector2 point)
    {
        return PointDistanceAlongPath(point) + previousChunksDistance;
    }

    public float GetPlayerDistance()
    {
        return PointDistanceAlongPathTotal(playerTransform.position);
    }

    Vector2 ClosestPointAlongPath(Vector2 point)
    {
        int currentSegmentIndex = ClosestSegmentIndexToPoint(point);
        return VectorExtras.ProjectPointOnLineSegment(path[currentSegmentIndex - 1], path[currentSegmentIndex], point);
    }

    void OnDrawGizmos()
    {
        // if (Application.isPlaying == false)
        //     return;

        Vector2 pos = transform.position;

        DebugExtension.DrawPoint(pos + ClosestPointAlongPath(playerTransform.position), Color.cyan);

        foreach (Vector2 p in bottomPoints)
        {
            DebugExtension.DrawPoint(pos + p, Color.blue);
        }

        foreach (Vector2 p in topPoints)
        {
            DebugExtension.DrawPoint(pos + p, Color.red);
        }
    }

    public bool IsLeft(Vector2 a, Vector2 b, Vector2 c)
    {
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
    }
}
