using UnityEngine;
using TMPro;

// [ExecuteInEditMode]
public class DistanceText : MonoBehaviour
{
    public TerrainController terrain;
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = terrain.GetPlayerDistance();
        text.text = string.Format("{0:N0}", distance / 3f);
    }
}
