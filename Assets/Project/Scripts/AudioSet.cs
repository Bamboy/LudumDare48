using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Asteroidians.Assets
{
    /// <summary>
    /// Returns a single audio clip from a set of random choices.
    /// </summary>
    [CreateAssetMenu( menuName = "Asteroidians/Audio Set" )][HideMonoScript]
    public class AudioSet : ScriptableObject
    {
        [Required]
        public List<AudioClip> sounds = new List<AudioClip>();
         
        public int Count
        {
            get
            {
                if( sounds == null )
                    return -1;
                else
                    return sounds.Count;
            }
        }

        public AudioClip this[ int index ]{ get { return sounds[ index ]; } }

        public AudioClip GetRandom()
        {
            if( sounds == null || sounds.Count == 0 )
                return null;
            else if( sounds.Count == 1 )
                return sounds[ 0 ];
            else
                return sounds[ Random.Range( 0, sounds.Count ) ];
        }
        /*
#if UNITY_EDITOR
        [Button][PropertyOrder(-1)]
        public void Play()
        {
            if( Count > 0 )
            {
                
                AudioSource.PlayClipAtPoint( GetRandom(), Vector3.zero );
            }
        }
#endif */
    }
}