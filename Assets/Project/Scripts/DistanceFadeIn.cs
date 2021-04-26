using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistanceFadeIn : MonoBehaviour
{
    public TerrainController terrain;
    TextMeshProUGUI text;
    public float min = 10;
    public float max = 80;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Color c = text.color;
        c.a = VectorExtras.Remap(min, max, 0, 1, terrain.GetPlayerDistance());
        text.color = c;
    }
}
