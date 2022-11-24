using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public float masterVolume;

    private AudioSource musicSource;
    public GameObject audioPlayer;

    public AudioClip[] sounds;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        musicSource = GetComponent<AudioSource>();

        SetMasterVolume(0.5f);
    }

    public void PlaySound(int i, float volume, float pitch)
    {
        AudioSource audio = Instantiate(audioPlayer, transform).GetComponent<AudioSource>();

        audio.pitch = pitch += Random.Range(-0.1f, 0.1f);
        audio.volume = volume * masterVolume;

        audio.PlayOneShot(sounds[i]);

        Destroy(audio.gameObject, sounds[i].length);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;

        musicSource.volume = masterVolume;
    }
}
