using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlackHole : MonoBehaviour
{
    public bool blowUp = true;
    Vector3 regularSize;
    Vector3 bigSize;
    public float timer;
    public float timeCap = 3f;
    public float breatheTime = 1;
    public Ease easeOut;
    public Ease easeIn;
    
    void Start()
    {
        regularSize = transform.localScale;
        bigSize = transform.localScale * 2f;
    }

    // Update is called once per frame
    void Update()
    {
        Breath();
    }

    void Breath()
    {
        if (blowUp)
        {
            //DOTween.To(() => transform.localScale, x => transform.localScale = x, bigSize, timeCap).SetLoops(3, LoopType.Yoyo);
            transform
                .DOScale(bigSize, breatheTime)
                .SetEase(easeOut);
            
        }
        else
        {
            //DOTween.To(() => transform.localScale, x => transform.localScale = x, regularSize, timeCap);

            transform
                .DOScale(regularSize, breatheTime)
                .SetEase(easeIn);

        }

        if (timer < timeCap)
        {
            timer += Time.deltaTime;
        }
        else
        {
            blowUp = !blowUp;
            timer = 0;
        }


    }

    public void IncreaseBlackHoleSize()
    {
        regularSize = bigSize * 1f;
        bigSize *= 1.25f;
        timer = 0;
        blowUp = true;
        breatheTime *= .75f;

    }
}
