using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketManager : MonoBehaviour
{
    public GameObject uyariPaneli;
    public TextMeshProUGUI uyariYazisi;
    public float uyariSuresi = 1.5f;
    private Coroutine aktifUyariCoroutine;

    [System.Serializable]
    public class MarketOgesi
    {
        public string ogeAdi;
        public int ogeID;
        public int fiyat;
        public Image kiyafetGorseli;
        public Button onizlemeButonu;
        public Vector2 previewSize = new Vector2(150, 110);
        public Vector2 previewPosition = new Vector2(0, 70);
        public Button satinAlButonu;
        public Button kusanButonu;
        public Button cikarButonu;
        public Image butonCercevesi;
        public TextMeshProUGUI fiyatYazisi;
    }

public GameObject marketPaneli;
    public GameObject anaMenuPaneli;
    public GameObject ayarlarPaneli;
    public TextMeshProUGUI bakiyeYazisi;

    public GameObject kiyafetlerIcerik;


    public MarketOgesi[] kiyafetOgeleri;

    public Color satinAlinmamisRenk = new Color(0.95f, 0.95f, 0.95f);
    public Color satinAlinmisRenk = new Color(0.85f, 1f, 0.85f);
    public Color kusaniliyorRenk = new Color(0.75f, 0.85f, 1f);

    public MenuManager menuManager;
    public Image onizlemeKiyafeti;
    public TextMeshProUGUI onizlemeAdi;
    public Button varsayilanButonu;
    public ScrollRect marketScroll;

    private const string SATIN_ALINAN_ONEK = "SatinAlindi_";
    private const string KUSANILAN_KIYAFET_KEY = "KusanilanKiyafetID";

    private void Start()
    {
        

        foreach (var oge in kiyafetOgeleri)
        {
            int id = oge.ogeID;
            if (oge.onizlemeButonu != null) oge.onizlemeButonu.onClick.AddListener(() => KiyafetOnizle(id));

            if (oge.satinAlButonu != null)
                oge.satinAlButonu.onClick.AddListener(() => OgeyeTikla(id));

            if (oge.kusanButonu != null)
                oge.kusanButonu.onClick.AddListener(() => KiyafetKusan(id));

            if (oge.cikarButonu != null)
                oge.cikarButonu.interactable = false;
        }



        if (varsayilanButonu != null) varsayilanButonu.onClick.AddListener(KiyafetSeciminiKaldir);
        KiyafetOnizle(PlayerPrefs.GetInt(KUSANILAN_KIYAFET_KEY, -1));
        if (marketScroll != null) marketScroll.content = kiyafetlerIcerik.GetComponent<RectTransform>();
        MarketArayuzunuGuncelle();
    }



    public void MarketiAc()
    {
        ButonSesiCal();

        if (marketPaneli != null) marketPaneli.SetActive(true);
        if (anaMenuPaneli != null) anaMenuPaneli.SetActive(false);
        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(false);
        if (menuManager != null) { if (menuManager.challengesPanel != null) menuManager.challengesPanel.SetActive(false); menuManager.NavGorunumunuGuncelle(1); }
        MarketArayuzunuGuncelle();
    }

    public void MarketiKapat()
    {
        ButonSesiCal();

        if (marketPaneli != null) marketPaneli.SetActive(false);
        if (anaMenuPaneli != null) anaMenuPaneli.SetActive(true);
        if (menuManager != null) menuManager.NavGorunumunuGuncelle(0);
    }

    private void ButonSesiCal()
    {
        if (SesYoneticisi.instance != null) SesYoneticisi.instance.ButonCal();
    }



    private void OgeyeTikla(int ogeID)
    {
        MarketOgesi secilenOge = System.Array.Find(kiyafetOgeleri, o => o.ogeID == ogeID);
        if (secilenOge == null) return;

        bool satinAlinmis = PlayerPrefs.GetInt(SATIN_ALINAN_ONEK + ogeID, 0) == 1;

        if (!satinAlinmis)
        {
            int bakiye = PlayerPrefs.GetInt("ToplamCuzdanPuani", 0);
            if (bakiye >= secilenOge.fiyat)
            {
                bakiye -= secilenOge.fiyat;
                PlayerPrefs.SetInt("ToplamCuzdanPuani", bakiye);
                PlayerPrefs.SetInt(SATIN_ALINAN_ONEK + ogeID, 1);
                PlayerPrefs.Save();
                KiyafetKusan(ogeID);
            }
            else
            {
                UyariGoster("Not enough coins!");
            }
        }

        MarketArayuzunuGuncelle();
    }

    private void UyariGoster(string mesaj)
    {
        if (uyariPaneli == null || uyariYazisi == null) return;

        if (aktifUyariCoroutine != null) StopCoroutine(aktifUyariCoroutine);
        aktifUyariCoroutine = StartCoroutine(UyariCoroutine(mesaj));
    }

    private IEnumerator UyariCoroutine(string mesaj)
    {
        uyariYazisi.text = mesaj;
        uyariPaneli.SetActive(true);

        yield return new WaitForSecondsRealtime(uyariSuresi);

        uyariPaneli.SetActive(false);
    }

    private void KiyafetKusan(int ogeID)
    {
        KiyafetOnizle(ogeID);
        PlayerPrefs.SetInt(KUSANILAN_KIYAFET_KEY, ogeID);
        PlayerPrefs.Save();

        OyuncuKontrol oyuncu = FindFirstObjectByType<OyuncuKontrol>();
        if (oyuncu != null) oyuncu.ApplyPurchasedSkin();

        MarketArayuzunuGuncelle();
    }

    public void KiyafetSeciminiKaldir()
    {
        KiyafetOnizle(-1);
        PlayerPrefs.SetInt(KUSANILAN_KIYAFET_KEY, -1);
        PlayerPrefs.Save();

        OyuncuKontrol oyuncu = FindFirstObjectByType<OyuncuKontrol>();
        if (oyuncu != null) oyuncu.ApplyPurchasedSkin();

        MarketArayuzunuGuncelle();
    }

    
    public void KiyafetOnizle(int id)
    {

        var oge = System.Array.Find(kiyafetOgeleri, o => o.ogeID == id);
        if (onizlemeAdi != null) onizlemeAdi.text = oge != null ? oge.ogeAdi : "Classic Puffy";
        if (onizlemeKiyafeti == null) return;
        onizlemeKiyafeti.gameObject.SetActive(oge != null && oge.kiyafetGorseli != null);
        if (oge == null || oge.kiyafetGorseli == null) return;
        onizlemeKiyafeti.sprite = oge.kiyafetGorseli.sprite;
        var rect = onizlemeKiyafeti.rectTransform;
        rect.anchoredPosition = oge.previewPosition;
        rect.sizeDelta = oge.previewSize;
    }

    public void MarketArayuzunuGuncelle()
    {
        int bakiye = PlayerPrefs.GetInt("ToplamCuzdanPuani", 0);
        if (bakiyeYazisi != null) bakiyeYazisi.text = "" + bakiye;

        int kusanilanID = PlayerPrefs.GetInt(KUSANILAN_KIYAFET_KEY, -1);

        foreach (var oge in kiyafetOgeleri)
        {
            bool satinAlinmis = PlayerPrefs.GetInt(SATIN_ALINAN_ONEK + oge.ogeID, 0) == 1;
            bool kusaniliyor = (kusanilanID == oge.ogeID);

            if (oge.butonCercevesi != null)
            {
                if (kusaniliyor) oge.butonCercevesi.color = kusaniliyorRenk;
                else if (satinAlinmis) oge.butonCercevesi.color = satinAlinmisRenk;
                else oge.butonCercevesi.color = satinAlinmamisRenk;
            }

            if (oge.satinAlButonu != null)
            {
                oge.satinAlButonu.gameObject.SetActive(!satinAlinmis);
                if (oge.fiyatYazisi != null) oge.fiyatYazisi.text = "BUY  " + oge.fiyat.ToString("N0");
            }

            if (oge.kusanButonu != null)
                oge.kusanButonu.gameObject.SetActive(satinAlinmis && !kusaniliyor);

            if (oge.cikarButonu != null)
                oge.cikarButonu.gameObject.SetActive(kusaniliyor);
        }
    }
}