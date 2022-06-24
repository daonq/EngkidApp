using UnityEngine;
namespace Stories
{
    public class StorySoundManager: MonoBehaviour
    {
        public AudioClip[] clips;
        public AudioSource[] audioSource;
        public void play(int index)
        {
            if (audioSource[0].isPlaying == false)
            {
                audioSource[0].clip = clips[index];
                audioSource[0].Play();
            }
        }
        public void playEffect(int index)
        {
            if (audioSource[1].isPlaying == false)
            {
                audioSource[1].clip = clips[index];
                audioSource[1].Play();
            }
        }
    }
}
