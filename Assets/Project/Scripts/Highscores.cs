using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Highscores
{
    const string SCORE_COUNT = "SCORE_COUNT";
    const string SCORE_LAST = "SCORE_LAST";
    const string SCORE_FORMAT = "SCORE_{0}";

    public static List<int> scores;
    public static int scoreCount = 0;
    public static int lastScore = -99;

    public static void Load()
    {
        if( PlayerPrefs.HasKey( SCORE_LAST ) )
            lastScore = PlayerPrefs.GetInt(SCORE_LAST, -1);

        if( PlayerPrefs.HasKey( SCORE_COUNT ) )
            scoreCount = PlayerPrefs.GetInt(SCORE_COUNT, 0);

        scores = new List<int>(scoreCount);
        for( int i = 0; i < scoreCount; i++ )
        {
            scores.Add( PlayerPrefs.GetInt( string.Format(SCORE_FORMAT, i), -1 ) );
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

        lastScore = value;
        scores.Add( value );
        SaveScores();
    }

    public static void SaveScores()
    {
        scores = new List<int>( 
            from s in scores
                orderby s descending
                    select s );

        PlayerPrefs.SetInt( SCORE_COUNT, scores.Count );
        PlayerPrefs.SetInt( SCORE_LAST, lastScore );
        for( int i = 0; i < scores.Count; i++ )
            PlayerPrefs.SetInt( string.Format(SCORE_FORMAT, i), scores[i] );
        
        PlayerPrefs.Save();
    }

    public static string GetScoresString()
    {
        if( scores == null || scores.Count <= 0 )
            return string.Empty;

        bool includedLast = false;

        StringBuilder b = new StringBuilder("<align=center>HIGH  SCORES</align>\n");
        for( int i = 0; i < Mathf.Min(scores.Count, 10); i++ )
        {
            if( scores[ i ] == lastScore )
            {
                b.AppendFormat( "\n<color=yellow>{0}. {1:N0}<size=12>km</size></color>", i + 1, scores[ i ] );
                includedLast = true;
            }
            else
                b.AppendFormat( "\n{0}. {1:N0}<size=12>km</size>", i + 1, scores[ i ] );
        }

        if( includedLast == false )
        {
            int last = -1;
            for( int i = 0; i < scores.Count; i++ )
            {
                if( scores[ i ] == lastScore )
                {
                    last = i;
                    break;
                }
            }
            if( last >= 0 )
                b.AppendFormat( "\n<align=left>. . .</align>\n<color=yellow>{0}. {1:N0}<size=12>km</size></color>", last + 1, lastScore );
        }

        return b.ToString();
    }

}
