using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject controlsUI;
    public TextMeshPro scoresText;

    private void Awake()
    {
        Highscores.Load();
        string scores = Highscores.GetScoresString();
        if( string.IsNullOrEmpty( scores ) )
        {
            scoresText.gameObject.SetActive( false );
            controlsUI.SetActive( true );
        }
        else
        {
            
            controlsUI.SetActive( false );
            scoresText.gameObject.SetActive( true );

            scoresText.text = scores;
        }
    }

}
