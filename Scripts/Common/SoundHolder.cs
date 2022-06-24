using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHolder : MonoBehaviour
{

    public AudioSource SoundSource;
    public SoundHolder Instance;
    public AudioClip keyboard;
    

    private void Awake()
    {
        //if (Instance)
        //{
        //    DestroyImmediate(gameObject);
        //}
        //else
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySound(AudioClip clip)
    {
        SoundSource.clip = clip;
        SoundSource.Play();
    }

    public void PlaySoundKeyboard()
    {
        SoundSource.PlayOneShot(keyboard);
    }

    public void CancelSound()
    {
        SoundSource.Stop();
    }
}
