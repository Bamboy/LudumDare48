using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TerrainSection : MonoBehaviour
{
    SpriteShapeController ssc;
    ShadowCaster2DController scc;

    void Awake()
    {
        ssc = GetComponent<SpriteShapeController>();
        scc = GetComponent<ShadowCaster2DController>();
    }

    public void UpdateSpline(List<Vector2> points)
    {
        ssc.spline.Clear();
        for (int i = 0; i < points.Count; i++)
        {
            ssc.spline.InsertPointAt(i, points[i]);
        }

        scc.UpdateShadowFromPoints(points.ToArray());
    }

}
