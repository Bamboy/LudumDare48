using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Highscores
{
    const string SCORE_COUNT = "SCORE_COUNT";
    const string SCORE_FORMAT = "SCORE_{0}";

    public static List<int> scores;
    public static int scoreCount = 0;

    public static void Load()
    {
        if( PlayerPrefs.HasKey( SCORE_COUNT ) )
        {
            scoreCount = PlayerPrefs.GetInt(SCORE_COUNT, 0);
        }

        scores = new List<int>(scoreCount);
        for( int i = 0; i < scoreCount; i++ )
        {
            scores[i] = PlayerPrefs.GetInt( string.Format(SCORE_FORMAT, i), -1 );
        }

        scores = new List<int>( 
            from s in scores
                orderby s descending
                    select s );
    }

    public static void AddScore( int value )
    {
        if( scores == null )
            Load();

        scores.Add( value );
    }

    public static void SaveScores()
    {
        scores = new List<int>( 
            from s in scores
                orderby s descending
                    select s );

        PlayerPrefs.SetInt( SCORE_COUNT, scores.Count );
        for( int i = 0; i < scores.Count; i++ )
            PlayerPrefs.SetInt( string.Format(SCORE_FORMAT, i), scores[i] );
        
        PlayerPrefs.Save();
    }

    public static string GetScoresString()
    {
        if( scores == null || scores.Count <= 0 )
            return string.Empty;

        StringBuilder b = new StringBuilder("HIGH SCORES");
        for( int i = 0; i < Mathf.Min(scores.Count, 10); i++ )
        {
            b.AppendFormat("\n<b>{0}.</b> {1:N0} km", i+1, scores[i]);
        }
        
        return b.ToString();
    }

}
