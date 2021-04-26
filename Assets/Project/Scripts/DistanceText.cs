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
        text.text = string.Format("<align=center><size=18><color=white>         Distance</color></size></align>\n{0:N0} <size=18>km</size>", distance / 3f);
    }
}
