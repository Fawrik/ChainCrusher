using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image fader;
    public AudioSource mainMenuIntroMusicSource;
    public void StartGame() => StartCoroutine(StartGameCoroutine());

    private IEnumerator StartGameCoroutine()
    {
        mainMenuIntroMusicSource.DOFade(0, 2);
        fader.DOFade(1, 2);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Main");
    }
}
