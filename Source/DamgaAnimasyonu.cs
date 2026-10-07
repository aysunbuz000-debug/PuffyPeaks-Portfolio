using System.Collections;

using UnityEngine;



public class DamgaAnimasyonu : MonoBehaviour

{

    [Header("Animasyon Ayarları")]

    public float baslangicOlcek = 2.5f;

    public float sallanmaOlcek = 0.85f;

    public float sure = 0.4f;



    public void Oynat()

    {

        StopAllCoroutines();

        StartCoroutine(DamgaCoroutine());

    }



    private IEnumerator DamgaCoroutine()

    {

        transform.localScale = Vector3.one * baslangicOlcek;

        float zaman = 0f;



        while (zaman < sure)

        {

            zaman += Time.unscaledDeltaTime;

            float t = zaman / sure;

            float olcek = Mathf.Lerp(baslangicOlcek, sallanmaOlcek, EaseOutBack(t));

            transform.localScale = Vector3.one * olcek;

            yield return null;

        }



        float donusZamani = 0f;

        float donusSuresi = 0.15f;

        Vector3 baslangic = transform.localScale;

        while (donusZamani < donusSuresi)

        {

            donusZamani += Time.unscaledDeltaTime;

            float t = donusZamani / donusSuresi;

            transform.localScale = Vector3.Lerp(baslangic, Vector3.one, t);

            yield return null;

        }



        transform.localScale = Vector3.one;

    }



    private float EaseOutBack(float t)

    {

        float c1 = 1.70158f;

        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);

    }

}