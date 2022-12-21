using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundEffect : MonoBehaviour
{
    public Transform soundSource;
    public SoundSettings settings;
    
    [System.Serializable]
    public struct SoundSettings
    {
        public AudioClip effect;
        public float baseVolume;
        public float basePitch;
        [Space]
        public bool attachToPositionTransform;
        public bool is3D;
        

    }

    private const float VolumeRange = 0.1f;
    private const float PitchRange = 0.3f;

    public void PlaySoundEffect()
    {
        // Determines where the sound where be played.
        var soundPlayPos = soundSource ? soundSource : transform;
        
        // Spawn a new game object, this will be where the effect plays.
        GameObject soundObject = new GameObject
        {
            name = "Sound Effect ( " + settings.effect.name + " )",
            transform =
            {
                position = soundPlayPos.position
            }
        };
        
        // Parents the newly spawned object to the spawn position given. 
        if (settings.attachToPositionTransform) soundObject.transform.parent = soundSource;
        
        // Add an audioSource component to this sound object. We will modify this before playing it.
        AudioSource source = soundObject.AddComponent<AudioSource>();
        
        source.clip = settings.effect;
        
        source.volume = UnityEngine.Random.Range(settings.baseVolume - VolumeRange, settings.baseVolume + VolumeRange);
        source.pitch = UnityEngine.Random.Range(settings.basePitch - PitchRange, settings.basePitch + PitchRange);
        
        source.spatialBlend = settings.is3D ? 0 : 1;
        source.Play();
        
        // Cleans up the effect once its done playing
        Destroy(soundObject, settings.effect.length);

    }
}
