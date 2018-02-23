using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]                                                           //allows the sound class to appear in the inspector when a class has a public sound object initialized, in this case an array of sound objects
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    public bool loop;


    [HideInInspector]
    public AudioSource source;
}



public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;                                                      //declares an array of variable size, value can be changed in inspector, in order so that properties such as volume, looping, and sound name be stored for each sound 
                                                                                //the size of the array is determined in the inspector and when the sound class' Awake() function is called, the values of each are iniatialized based on inspector input
    void Awake()
    {
        foreach (Sound s in sounds)                                             //all required instances of the sound class determined in the inspector are actually created before the Start() function is called
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;

        }
    }

    void Start()
    {
        Play("menu");                                                           //plays the menu background music, name "menu" comes from the inspector

    }

    public void Play(string name)                                               //functions are created here so that other classes can more simply call these functions to tell the audiomanager to play a sound, inputed sound begins to play
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }   
    public void Stop(string name)                                               //inputted sound stops playing when this function is called
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }


}
