using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource startAudio;
    public AudioSource ambienceAudio;
    public AudioSource walkAudio;
    public AudioSource pickAudio;
    public AudioSource giveAudio;
    public AudioSource trashAudio;
    public AudioSource transitionAudio;

    Dictionary<string, AudioSource> _sources;

    bool _isTransitionStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        var start = gameObject.AddComponent<AudioSource>();

        _sources = new Dictionary<string, AudioSource>() {
            {"start", startAudio},
            {"ambience", ambienceAudio},
            {"walk", walkAudio},
            {"pick", pickAudio},
            {"give", giveAudio},
            {"trash", trashAudio},
            {"transition", transitionAudio}
        };
    }
    
    public void Play (string name, bool loop = false) {
        _sources[name].loop = loop;
        _sources[name].Play();
    }

    public void Pause (string name) {
        _sources[name].Pause();
    }
}
