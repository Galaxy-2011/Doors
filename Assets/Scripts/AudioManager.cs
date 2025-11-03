using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop;
    public float spatialBlend; // 0 = 2D, 1 = 3D
    public float minDistance = 1f;
    public float maxDistance = 500f;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixing")]
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup ambienceMixer;

    [Header("Sound Collections")]
    public Sound[] entitySounds;
    public Sound[] doorSounds;
    public Sound[] itemSounds;
    public Sound[] uiSounds;
    public Sound[] ambientSounds;
    public Sound[] musicTracks;

    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();
    private Sound currentAmbience;
    private Sound currentMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSounds()
    {
        InitializeSoundArray(entitySounds, sfxMixer);
        InitializeSoundArray(doorSounds, sfxMixer);
        InitializeSoundArray(itemSounds, sfxMixer);
        InitializeSoundArray(uiSounds, sfxMixer);
        InitializeSoundArray(ambientSounds, ambienceMixer);
        InitializeSoundArray(musicTracks, musicMixer);
    }

    private void InitializeSoundArray(Sound[] sounds, AudioMixerGroup mixer)
    {
        foreach (Sound s in sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            s.source = source;

            source.clip = s.clip;
            source.volume = s.volume;
            source.pitch = s.pitch;
            source.loop = s.loop;
            source.spatialBlend = s.spatialBlend;
            source.minDistance = s.minDistance;
            source.maxDistance = s.maxDistance;
            source.outputAudioMixerGroup = mixer;

            soundDictionary[s.name] = s;
        }
    }

    public void PlaySound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.source.Play();
        }
    }

    public void PlaySoundAtPosition(string name, Vector3 position)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume);
        }
    }

    public void StopSound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.source.Stop();
        }
    }

    public void PlayMusic(string name, float fadeTime = 1f)
    {
        if (soundDictionary.TryGetValue(name, out Sound newMusic))
        {
            if (currentMusic != null)
            {
                StartCoroutine(CrossfadeMusic(currentMusic, newMusic, fadeTime));
            }
            else
            {
                newMusic.source.Play();
            }
            currentMusic = newMusic;
        }
    }

    public void PlayAmbience(string name, float fadeTime = 1f)
    {
        if (soundDictionary.TryGetValue(name, out Sound newAmbience))
        {
            if (currentAmbience != null)
            {
                StartCoroutine(CrossfadeAmbience(currentAmbience, newAmbience, fadeTime));
            }
            else
            {
                newAmbience.source.Play();
            }
            currentAmbience = newAmbience;
        }
    }

    private System.Collections.IEnumerator CrossfadeMusic(Sound oldMusic, Sound newMusic, float fadeTime)
    {
        float elapsedTime = 0;
        float startVolume = oldMusic.source.volume;
        newMusic.source.volume = 0;
        newMusic.source.Play();

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeTime;

            oldMusic.source.volume = Mathf.Lerp(startVolume, 0, t);
            newMusic.source.volume = Mathf.Lerp(0, newMusic.volume, t);

            yield return null;
        }

        oldMusic.source.Stop();
        oldMusic.source.volume = startVolume;
    }

    private System.Collections.IEnumerator CrossfadeAmbience(Sound oldAmbience, Sound newAmbience, float fadeTime)
    {
        float elapsedTime = 0;
        float startVolume = oldAmbience.source.volume;
        newAmbience.source.volume = 0;
        newAmbience.source.Play();

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeTime;

            oldAmbience.source.volume = Mathf.Lerp(startVolume, 0, t);
            newAmbience.source.volume = Mathf.Lerp(0, newAmbience.volume, t);

            yield return null;
        }

        oldAmbience.source.Stop();
        oldAmbience.source.volume = startVolume;
    }
}