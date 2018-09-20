using System.Collections;
using UnityEngine;

public class SwitchMusic : MonoBehaviour
{


    public AudioSource source;
    public AudioSource source2;
    float originalVolume;

    public AnimationCurve volumeCurve;

    public void Switch()
    {
        originalVolume = source.volume;
        StartCoroutine(BossEnd());
    }

    IEnumerator BossEnd()
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