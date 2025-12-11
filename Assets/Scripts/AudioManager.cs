using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource vfxAudioSource;

    public AudioClip bgmClip;
    public AudioClip gameoverClip;

    public AudioClip coinClip;
    public AudioClip checkpointClip;
    public AudioClip jumpClip;
    public AudioClip skillpointClip;
    

    private void Start()
    {
        if (bgmClip != null)
            PlayBGM(bgmClip, 1f);
    }


    public void PlaySFX(AudioClip sfxClip, float volume = 1f)
    {
        if (sfxClip == null || vfxAudioSource == null) return;
        vfxAudioSource.PlayOneShot(sfxClip, volume);
    }

    public void PlayBGM(AudioClip bgm, float volume = 0.6f)
    {
        if (musicAudioSource == null || bgm == null) return;

        musicAudioSource.clip = bgm;
        musicAudioSource.volume = volume;
        musicAudioSource.loop = true;   // BGM luôn loop
        musicAudioSource.Play();
    }

    public void StopBGM()
    {
        if (musicAudioSource != null)
            musicAudioSource.Stop();
    }


}
