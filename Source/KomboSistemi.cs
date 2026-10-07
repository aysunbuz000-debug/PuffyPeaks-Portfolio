using System.Collections;

using UnityEngine;

using TMPro;



public class KomboSistemi : MonoBehaviour

{

    public static KomboSistemi instance;



    public GameManager gameManager;

    public TextMeshProUGUI komboYazisi;

    public TextMeshProUGUI bonusPatlamaYazisi;



    public int bonusEsigi = 10;

    public int bonusPuan = 100;



    public Color renk1_3 = new Color(0.4f, 0.9f, 0.4f);

    public Color renk4_6 = new Color(1f, 0.9f, 0.2f);

    public Color renk7_9 = new Color(1f, 0.55f, 0.1f);

    public Color renk10 = new Color(1f, 0.2f, 0.2f);



    public float popBuyume = 1.4f;

    public float popSuresi = 0.15f;

    public float kaybolmaSuresi = 0.3f;



    [Header("Kusursuz inis bonusu")]
    [Min(2)] public int hizliYukselisEsigi = 3;
    private int kusursuzInisSayisi;
    private int sonKusursuzPlatform = int.MinValue;
    private float sonInisZamani;

    private int lastPlatformID = int.MinValue;

    private int komboSayisi = 0;

    private Coroutine aktifAnimasyon;



    private void Awake()

    {

        instance = this;

    }



    private void Start()

    {

        if (komboYazisi != null) komboYazisi.gameObject.SetActive(false);

        if (bonusPatlamaYazisi != null) bonusPatlamaYazisi.gameObject.SetActive(false);

    }



    public void ZeminePasBasildi(int platformID)

    {

        if (platformID == lastPlatformID)

        {

            KomboBozuldu();

            return;

        }



        lastPlatformID = platformID;

        komboSayisi++;



        KomboGorseliniGuncelle();



        if (SesYoneticisi.instance != null)

            SesYoneticisi.instance.KomboSesiCal(komboSayisi);



        if (komboSayisi >= bonusEsigi)

        {

            BonusPatlat();

        }

    }



    public bool KusursuzInisiKontrolEt(int platformID, bool merkezde)
    {
        // Long gaps, repeated platforms and edge landings break the precision streak.
        if (!merkezde || platformID == sonKusursuzPlatform || Time.time - sonInisZamani > 4f)
            kusursuzInisSayisi = 0;
        bool farkliPlatform = platformID != sonKusursuzPlatform;
        sonKusursuzPlatform = platformID;
        sonInisZamani = Time.time;
        if (!merkezde || !farkliPlatform) return false;
        kusursuzInisSayisi++;
        if (kusursuzInisSayisi < Mathf.Max(2, hizliYukselisEsigi)) return false;
        kusursuzInisSayisi = 0;
        return true;
    }

    private void KomboGorseliniGuncelle()

    {

        if (komboYazisi == null) return;



        komboYazisi.gameObject.SetActive(true);

        komboYazisi.text = "x" + komboSayisi;



        Color renk = RenkSec(komboSayisi);

        renk.a = 1f;

        komboYazisi.color = renk;



        if (aktifAnimasyon != null) StopCoroutine(aktifAnimasyon);

        aktifAnimasyon = StartCoroutine(PopAnimasyonu());

    }



    private Color RenkSec(int sayi)

    {

        if (sayi >= 10) return renk10;

        if (sayi >= 7) return renk7_9;

        if (sayi >= 4) return renk4_6;

        return renk1_3;

    }



    private IEnumerator PopAnimasyonu()

    {

        komboYazisi.transform.localScale = Vector3.one * popBuyume;

        float zaman = 0f;



        while (zaman < popSuresi)

        {

            zaman += Time.deltaTime;

            float t = zaman / popSuresi;

            komboYazisi.transform.localScale = Vector3.Lerp(Vector3.one * popBuyume, Vector3.one, t);

            yield return null;

        }



        komboYazisi.transform.localScale = Vector3.one;

    }



    private void KomboBozuldu()

    {

        if (komboSayisi <= 1)

        {

            komboSayisi = 0;

            return;

        }



        StartCoroutine(KayboluşAnimasyonu());

        komboSayisi = 0;

    }



    private IEnumerator KayboluşAnimasyonu()

    {

        if (komboYazisi == null) yield break;



        float zaman = 0f;

        Color baslangicRenk = komboYazisi.color;

        Vector3 baslangicScale = komboYazisi.transform.localScale;



        while (zaman < kaybolmaSuresi)

        {

            zaman += Time.deltaTime;

            float t = zaman / kaybolmaSuresi;



            komboYazisi.transform.localScale = Vector3.Lerp(baslangicScale, Vector3.one * 0.5f, t);



            Color yeniRenk = baslangicRenk;

            yeniRenk.a = Mathf.Lerp(1f, 0f, t);

            komboYazisi.color = yeniRenk;



            yield return null;

        }



        komboYazisi.gameObject.SetActive(false);

        komboYazisi.transform.localScale = Vector3.one;

    }



    private void BonusPatlat()

    {

        if (gameManager != null) gameManager.BonusPuanEkle(bonusPuan);



        if (SesYoneticisi.instance != null)

            SesYoneticisi.instance.KomboBonusSesiCal();



        StartCoroutine(BonusPatlamaAnimasyonu());



        komboSayisi = 0;

        lastPlatformID = int.MinValue;

        if (komboYazisi != null) komboYazisi.gameObject.SetActive(false);

    }



    private IEnumerator BonusPatlamaAnimasyonu()

    {

        if (bonusPatlamaYazisi == null) yield break;



        bonusPatlamaYazisi.text = "+" + bonusPuan + "!";

        bonusPatlamaYazisi.gameObject.SetActive(true);

        bonusPatlamaYazisi.transform.localScale = Vector3.one * 2f;



        Color renk = renk10;

        renk.a = 1f;

        bonusPatlamaYazisi.color = renk;



        float zaman = 0f;

        float sure = 0.6f;



        while (zaman < sure)

        {

            zaman += Time.deltaTime;

            float t = zaman / sure;



            bonusPatlamaYazisi.transform.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one, t);



            Color c = bonusPatlamaYazisi.color;

            c.a = Mathf.Lerp(1f, 0f, Mathf.Clamp01((t - 0.5f) * 2f));

            bonusPatlamaYazisi.color = c;



            yield return null;

        }



        bonusPatlamaYazisi.gameObject.SetActive(false);

    }

}