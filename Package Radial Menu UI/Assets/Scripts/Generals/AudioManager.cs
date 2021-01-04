using System;
using UnityEngine;

[Serializable]
public class Sound 
{
    public string Name;
    public AudioClip Clip;
    public bool Playing;

    [HideInInspector] public AudioSource Source;
    [Range(0, 1)] public float Volume;
    [Range(.1f, 3)] public float Pitch;
}


public class AudioManager : MonoBehaviour
{    
    public Sound[] Sounds;      // TODO: Assign clips with their specific name to call


    private void Awake() {
        foreach (Sound s in Sounds) {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;

            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
        }
    }

    // Plays "named" clip on gameobject that calling
    public void PlayOnMe(string name, GameObject obj) {
        AudioSource source;
        obj.TryGetComponent<AudioSource>(out source);
        
        if (!source) {
            source = InitOnMeSource(name, obj);
        }

        //if(!source.isPlaying) {
            source.Play();
        //}
    }

    // Creates Audio Source with "named" clip on gameobject that calling
    private AudioSource InitOnMeSource(string name, GameObject obj) {
        Sound s = GetSound(name);
        
        AudioSource source;
        source = obj.AddComponent<AudioSource>();    
        source.clip = s.Clip;
        source.volume = s.Volume;
        source.pitch = s.Pitch;
        
        return source;
    }

    // Pauses the "named" playing clip on gameobject that calling
    public void PauseOnMe(string name, GameObject obj) {
        AudioSource source;
        if(obj.TryGetComponent<AudioSource>(out source) && source.isPlaying) {
            source.Pause();
        }
    }

    // Plays the "named" clip till the end of clip on this gameobject
    public void PlayOne(string name) {
        Sound s = GetSound(name);
        if (!s.Playing) {
            s.Source.Play();
            s.Playing = true;
        }
    }

    // Plays the "named" clip on this gameobject
    public void Play(string name) {
        Sound s = GetSound(name);
        s.Source.Play();
    }

    // Pauses the "named" clip on this gameobject
    public void Pause(string name) {
        Sound s = GetSound(name);
        s.Source.Pause();
    }

    // Gets the clip according to "name" from array that assigned in inspector 
    public Sound GetSound(string name) {
        return Array.Find(Sounds, sound => sound.Name == name);
    }
}