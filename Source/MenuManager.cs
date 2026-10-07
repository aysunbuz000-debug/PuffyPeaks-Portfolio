using UnityEngine;

using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class MenuManager : MonoBehaviour

{

    [Header("Ayarlar Paneli")]

    public GameObject ayarlarPaneli;
    public GameObject challengesPanel;

    public GameObject settingsShortcut;

    [Header("Ana Menü Paneli")]

    public GameObject AnaMenuPaneli;

    [Header("Market Paneli")]

    public GameObject marketPaneli;

    [Header("Lider Tablosu Paneli")]

    public GameObject liderTablosuPaneli;

    [Header("Oyun Sahnesi Adi")]

    public string oyunSahnesiAdi = "SampleScene";

    [Header("Nav Bar Aktif Göstergeleri")]

    public GameObject navAnaMenuAktif;

    public GameObject navMarketAktif;

    public GameObject navAyarlarAktif;

    private void Awake()

    {

        // Gameplay can leave scaled time paused when returning to this scene.

        Time.timeScale = 1f;

    }



    public void OyunuBaslat()

    {

        ButonSesiCal();

        SceneManager.LoadScene(oyunSahnesiAdi);

    }

    public void AyarlariAc()

    {

        ButonSesiCal();

        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(true);

        if (AnaMenuPaneli != null) AnaMenuPaneli.SetActive(false);

        if (marketPaneli != null) marketPaneli.SetActive(false);
        if (challengesPanel != null) challengesPanel.SetActive(false);

        NavGorunumunuGuncelle(-1);

    }

    public void AyarlariKapat()

    {

        ButonSesiCal();

        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(false);

        if (AnaMenuPaneli != null) AnaMenuPaneli.SetActive(true);

        NavGorunumunuGuncelle(0);

    }

    public void AnaMenuyeDon()

    {

        ButonSesiCal();

        if (AnaMenuPaneli != null) AnaMenuPaneli.SetActive(true);

        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(false);

        if (marketPaneli != null) marketPaneli.SetActive(false);
        if (challengesPanel != null) challengesPanel.SetActive(false);

        if (liderTablosuPaneli != null) liderTablosuPaneli.SetActive(false);

        NavGorunumunuGuncelle(0);

    }

    public void LiderTablosunuAc()

    {

        ButonSesiCal();

        if (liderTablosuPaneli != null) liderTablosuPaneli.SetActive(true);

        if (AnaMenuPaneli != null) AnaMenuPaneli.SetActive(false);

    }

    public void LiderTablosunuKapat()

    {

        ButonSesiCal();

        if (liderTablosuPaneli != null) liderTablosuPaneli.SetActive(false);

        if (AnaMenuPaneli != null) AnaMenuPaneli.SetActive(true);

    }

    public void NavGorunumunuGuncelle(int aktifIndex)

    {

        if (settingsShortcut != null) settingsShortcut.SetActive(aktifIndex == 0);

        if (navAnaMenuAktif != null) navAnaMenuAktif.SetActive(aktifIndex == 0);

        if (navMarketAktif != null) navMarketAktif.SetActive(aktifIndex == 1);

        if (navAyarlarAktif != null) navAyarlarAktif.SetActive(aktifIndex == 2);

    }

    public void OyundanCik()

    {

        ButonSesiCal();

        Debug.Log("Oyundan çikiliyor...");



#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else

        Application.Quit();

#endif

    }



    private void ButonSesiCal()

    {

        if (SesYoneticisi.instance != null) SesYoneticisi.instance.ButonCal();

    }



    public void KVKKPanelAc()

    {

        ButonSesiCal();

        if (KVKKYoneticisi.instance != null)

            KVKKYoneticisi.instance.AyarlardanAc();

    }

    public void GPGSLiderTablosunuGoster()

    {

        ButonSesiCal();

        if (LiderTablosuYoneticisi.instance != null)

        {

            LiderTablosuYoneticisi.instance.LiderTablosunuGoster();

        }

    }



}