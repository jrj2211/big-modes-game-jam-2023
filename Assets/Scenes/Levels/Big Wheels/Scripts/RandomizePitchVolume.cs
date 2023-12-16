using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizePitchVolume : MonoBehaviour
{
    public float minPitch;
    public float maxPitch;
    public float minVolume;
    public float maxVolume;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.pitch = Random.Range(minPitch, maxPitch);
        source.volume = Random.Range(minVolume, maxVolume); 
    }
}
