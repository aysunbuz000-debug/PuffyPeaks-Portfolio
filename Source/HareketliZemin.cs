using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class HareketliZemin : MonoBehaviour

{

    public float hareketHizi = 2.0f;

    public float hareketMesafesi = 1.5f;

    [Header("Kalkan Üretim Ayarları")]

    public GameObject kalkanPrefab;

    [Range(0, 100)] public float kalkanCikmaShansi = 20f;

    private Vector3 baslangicPozisyonu;
    private bool hareketAktif;
    private float hareketZamani;

    public void HareketiAyarla(bool aktif, float hiz, float mesafe, float ekranSiniri)
    {
        hareketAktif = aktif;
        hareketHizi = Mathf.Max(0f, hiz);
        hareketMesafesi = Mathf.Max(0f, mesafe);
        baslangicPozisyonu = transform.position;
        hareketZamani = 0f;
        if (!aktif) return;
        Collider2D platformCollider = GetComponent<Collider2D>();
        float yariGenislik = platformCollider != null ? platformCollider.bounds.extents.x : 0.3f;
        float guvenliSinir = Mathf.Max(0f, ekranSiniri - yariGenislik);
        hareketMesafesi = Mathf.Min(hareketMesafesi, guvenliSinir);
        baslangicPozisyonu.x = Mathf.Clamp(baslangicPozisyonu.x,
            -guvenliSinir + hareketMesafesi, guvenliSinir - hareketMesafesi);
        transform.position = baslangicPozisyonu;
    }



    void Start()

    {

        baslangicPozisyonu = transform.position;

        if (kalkanPrefab != null)

        {

            float sans = Random.Range(0f, 100f);

            if (sans <= kalkanCikmaShansi)

            {

                Vector3 kalkanPozisyonu = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);

                GameObject yeniKalkan = Instantiate(kalkanPrefab, kalkanPozisyonu, Quaternion.identity);

                yeniKalkan.transform.SetParent(transform);

            }

        }

    }



    void FixedUpdate()
    {
        if (!hareketAktif) return;
        hareketZamani += Time.fixedDeltaTime;
        float yeniX = baslangicPozisyonu.x + Mathf.Sin(hareketZamani * hareketHizi) * hareketMesafesi;
        transform.position = new Vector3(yeniX, transform.position.y, transform.position.z);
    }
}