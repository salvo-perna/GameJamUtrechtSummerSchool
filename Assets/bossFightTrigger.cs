using System.Collections;
using UnityEngine;

public class bossFightTrigger : MonoBehaviour
{

    public GameObject boss;
    public GameObject bossHealthCanvas;
    public AudioSource source;
    public AudioSource source2;
    float originalVolume;

    public AnimationCurve volumeCurve;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        originalVolume = source.volume;
        boss.SetActive(true);
        bossHealthCanvas.SetActive(true);
        StartCoroutine(BossTrigger());
        GetComponent<BoxCollider2D>().enabled = false;
    }

    IEnumerator BossTrigger()
    {
        float t = originalVolume;
        float g = 0;
        source2.Play();
        while (t > 0)
        {
            source.volume = volumeCurve.Evaluate(t);
            source2.volume = g;
            yield return null;
            t -= Time.deltaTime;
            g += Time.deltaTime;
        }

    }


}
