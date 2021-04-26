using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistanceFadeIn : MonoBehaviour
{
    public TerrainController terrain;
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Color c = text.color;
        c.a = VectorExtras.Remap(10, 80, 0, 1, terrain.GetPlayerDistance());
        text.color = c;
    }
}
