using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void AssignValue(int points) => text.text = points.ToString();
    void Start() => StartCoroutine(TweenText());

    public IEnumerator TweenText()
    {
        text.rectTransform.DOPunchScale(Vector3.one / 2, .4f, 2, 1);
        text.rectTransform.DOLocalMoveY(text.rectTransform.localPosition.x + 4, 2.5f).SetEase(Ease.OutCirc);
        yield return new WaitForSeconds(2.5f);
        text.DOFade(0, 1);

        Destroy(gameObject, 2);
    }
}
