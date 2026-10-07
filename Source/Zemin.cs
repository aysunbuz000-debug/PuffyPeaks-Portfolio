using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zemin : MonoBehaviour
{
    public float jumpforce = 12f;

    public int maxZiplamaHakki = 2;
    private int kalanZiplamaSayisi;

    [Range(0f, 1f)] public float solukasmaOrani = 0.6f;
    private SpriteRenderer spriteRenderer;
    private float baslangicAlpha;

    public AudioClip ozelZiplamaSesi;

    public GameObject kalkanPrefab;
    [Range(0, 100)] public float kalkanCikmaShansi = 20f;

    public GameObject coinPrefab;
    [Range(0, 100)] public float coinCikmaShansi = 40f;

    public GameObject miknatisPrefab;
    [Range(0, 100)] public float miknatisCikmaShansi = 20f;

    void Start()
    {
        kalanZiplamaSayisi = maxZiplamaHakki;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) baslangicAlpha = spriteRenderer.color.a;

        OgeUret();
    }

    private void OgeUret()
    {
        float zar = Random.Range(0f, 100f);
        GameObject secilenPrefab = null;

        float esikKalkan = kalkanCikmaShansi;
        float esikMiknatis = esikKalkan + miknatisCikmaShansi;

        if (zar <= esikKalkan && kalkanPrefab != null)
        {
            secilenPrefab = kalkanPrefab;
        }
        else if (zar <= esikMiknatis && miknatisPrefab != null)
        {
            secilenPrefab = miknatisPrefab;
        }
        else if (coinPrefab != null)
        {
            secilenPrefab = coinPrefab;
        }

        if (secilenPrefab != null)
        {
            Vector3 pozisyon = new Vector3(transform.position.x, transform.position.y + 0.6f, 0f);
            GameObject yeniObje = Instantiate(secilenPrefab, pozisyon, Quaternion.identity);
            yeniObje.transform.SetParent(transform);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            if (rb.linearVelocity.y <= 0.1f)
            {
                if (SesYoneticisi.instance != null)
                {
                    if (ozelZiplamaSesi != null)
                        SesYoneticisi.instance.EfektCalRastgelePitch(ozelZiplamaSesi);
                    else
                        SesYoneticisi.instance.ZiplamaCal();
                }

                bool hizliYukselis = false;
                if (KomboSistemi.instance != null)
                {
                    int platformID = gameObject.GetInstanceID();
                    KomboSistemi.instance.ZeminePasBasildi(platformID);
                    Collider2D platformCollider = GetComponent<Collider2D>();
                    bool merkezde = platformCollider != null &&
                        Mathf.Abs(rb.worldCenterOfMass.x - platformCollider.bounds.center.x) <=
                        platformCollider.bounds.size.x * 0.2f;
                    hizliYukselis = KomboSistemi.instance.KusursuzInisiKontrolEt(platformID, merkezde);
                }

                OyuncuKontrol oyuncu = collision.gameObject.GetComponent<OyuncuKontrol>();
                if (oyuncu != null)
                    oyuncu.PlatformdanZipla(jumpforce, hizliYukselis);
                else
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpforce);

                kalanZiplamaSayisi--;

                if (spriteRenderer != null && kalanZiplamaSayisi > 0)
                {
                    float kullanilanOran = 1f - ((float)kalanZiplamaSayisi / maxZiplamaHakki);
                    Color yeniRenk = spriteRenderer.color;
                    yeniRenk.a = baslangicAlpha * (1f - solukasmaOrani * kullanilanOran);
                    spriteRenderer.color = yeniRenk;
                }

                if (kalanZiplamaSayisi <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}