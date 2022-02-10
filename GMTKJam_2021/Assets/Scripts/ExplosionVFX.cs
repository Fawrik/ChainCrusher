using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ExplosionVFX : MonoBehaviour
{
    public AudioClip explosionSFX;
    public void ExploVFX(float radius)
    {
        StartCoroutine(ExploCorout(radius));
    }
    private IEnumerator ExploCorout(float radius)
    {
        AudioSource.PlayClipAtPoint(explosionSFX, transform.position);
        
        transform.DOScale(radius*2.25f, .5f);
        yield return new WaitForSeconds(.25f);
        GetComponent<SpriteRenderer>().DOFade(0, .2f);
        yield return new WaitForSeconds(.55f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Bullet>())
        {
            print($"destroyed {other.gameObject.name}");
            Destroy(other.gameObject);
        }
    }
}