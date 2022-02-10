using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicHandler : MonoBehaviour
{
    public delegate void BeatAction();
    public static event BeatAction OnBeat;
    public float bgBeatVal = .001f;
    public AudioSource audioSource;
    public AudioClip currentMusic;
    public AudioClip earlyMusic;
    public AudioClip midMusic;
    public AudioClip lateMusic;
    private float audioVolumeMax;
    public bool changeMusic = false;
    public AudioSource stinger;

    public CarHealth carHealth;

    public float delay;
    void Start()
    {
        StartCoroutine(nameof(StartTheShow));
        audioSource = GetComponent<AudioSource>();
        currentMusic = audioSource.clip;
    }

    IEnumerator StartTheShow()
    {
        yield return new WaitForSeconds(1);
        GetComponent<AudioSource>().Play();
        StartCoroutine(nameof(Beat));
    }

    IEnumerator Beat()
    {
        if (OnBeat != null)
        {
            OnBeat();
            GameObject.Find("BG").transform.DOPunchScale(Vector3.one * bgBeatVal, .2f, 10, 1);

            if (changeMusic)
            {
                stinger.PlayOneShot(stinger.clip);
                audioSource.clip = currentMusic;
                audioSource.Play();
                changeMusic = false;
            }

        }
        yield return new WaitForSeconds(delay);
        StartCoroutine(nameof(Beat));
    }


    public void FadeMusic(float volume, float fadeTime)
    {
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, volume, fadeTime);
    }



}
