using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject maviZeminPrefab;
    public GameObject yesilZeminPrefab;
    [Range(0, 100)] public int yesilZeminSansi = 10;
    public int platformCount = 300;

    [Header("Ruzgar bolgeleri")]
    public bool ruzgarAktif = true;
    public float ilkRuzgarYuksekligi = 14f;
    public float ruzgarAraligi = 26f;
    public float ruzgarBolgeYuksekligi = 4f;
    public float ruzgarYatayHizi = 1.8f;
    public Vector2 spawnRangeX = new Vector2(-2.0f, 2.0f);
    public Vector2 spawnRangeY = new Vector2(2.5f, 4.5f);

    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject pauseButton;
    public TextMeshProUGUI pauseMusicLabel;
    public TextMeshProUGUI pauseSoundLabel;
    public GameObject pauseSettingsPanel;
    public bool IsPaused { get; private set; }
    private float pauseTimeScale = 1f;
    public TextMeshProUGUI skorText;
    public Transform karakterTransform;

    public TextMeshProUGUI gameOverSkorYazisi;
    public GameObject yeniRekorObjesi;

    public int coinBasinaDeger = 10;
    private int toplananCoinSayisi = 0;

    public TextMeshProUGUI coinBonusYazisi;
    public TextMeshProUGUI toplamSkorYazisi;
    public float coinBonusGecikmesi = 0.5f;
    public float sayimSuresi = 0.6f;

    private int toplamBonusPuan = 0;

    public GameObject reklamButonu;
    public float devamKalkanSuresi = 3f;
    private bool reklamKullanildiMi = false;

    public GameObject baslangicArkaPlanSeti;
    public GameObject morArkaPlanSeti;
    public GameObject uclulerArkaPlanSeti;
    public float gecisSuresi = 1.5f;

    private const string EN_YUKSEK_SKOR_KEY = "EnYuksekSkor";

    private Vector3 lastSpawnPosition;
    private bool isGameOver = false;
    private float enYuksekNokta = 0f;
    private int guncelSkor = 0;



    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        lastSpawnPosition = Vector3.zero;
        if (pauseButton) pauseButton.SetActive(true);
        IsPaused = false;
        if (pausePanel) pausePanel.SetActive(false);
        if (pauseSettingsPanel) pauseSettingsPanel.SetActive(false);

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (coinBonusYazisi != null) coinBonusYazisi.gameObject.SetActive(false);

        if (karakterTransform != null) enYuksekNokta = karakterTransform.position.y;

        TekArkaPlaniHazirla();
        bool oncekiZeminHareketli = false;

        for (int i = 0; i < platformCount; i++)
        {
            float randomY = Random.Range(spawnRangeY.x, spawnRangeY.y);
            lastSpawnPosition.y += randomY;
            lastSpawnPosition.x = Random.Range(spawnRangeX.x, spawnRangeX.y);
            GameObject uretilecekPrefab = maviZeminPrefab;
            int rastgeleZar = Random.Range(0, 101);
            if (rastgeleZar <= yesilZeminSansi && yesilZeminPrefab != null) uretilecekPrefab = yesilZeminPrefab;
            GameObject zemin = Instantiate(uretilecekPrefab, lastSpawnPosition, Quaternion.identity);
            float yukseklikPuani = lastSpawnPosition.y * 10f;
            HareketliZemin hareket = zemin.GetComponent<HareketliZemin>();
            bool hareketli = hareket != null && !oncekiZeminHareketli &&
                Random.value < HareketliZeminOraniAl(yukseklikPuani);
            if (hareket != null)
            {
                float zorluk = Mathf.Clamp01((yukseklikPuani - 300f) / 3700f);
                hareket.HareketiAyarla(hareketli, Mathf.Lerp(0.65f, 1.35f, zorluk),
                    Mathf.Lerp(0.4f, 0.95f, zorluk), 2.4f);
            }
            oncekiZeminHareketli = hareketli;
        }
        if (ruzgarAktif && karakterTransform != null)
        {
            OyuncuKontrol oyuncu = karakterTransform.GetComponent<OyuncuKontrol>();
            if (oyuncu != null)
            {
                RuzgarBolgesi ruzgar = gameObject.AddComponent<RuzgarBolgesi>();
                ruzgar.ilkYukseklik = ilkRuzgarYuksekligi;
                ruzgar.aralik = ruzgarAraligi;
                ruzgar.bolgeYuksekligi = ruzgarBolgeYuksekligi;
                ruzgar.yatayHiz = ruzgarYatayHizi;
                ruzgar.Hazirla(karakterTransform, lastSpawnPosition.y, oyuncu.ekranSiniriX);
                oyuncu.RuzgarKaynaginiAyarla(ruzgar);
            }
        }
    }

    public int YukseklikSkoruAl()
    {
        return Mathf.RoundToInt(enYuksekNokta * 10);
    }

    private void Update()
    {
        if (!isGameOver && !IsPaused && karakterTransform != null) SkorHesapla();
        if (!isGameOver && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) { if (IsPaused) ResumeGame(); else PauseGame(); }
        if (isGameOver && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) YenidenBaslat();
    }

    private void SkorHesapla()
    {
        if (karakterTransform.position.y > enYuksekNokta)
        {
            enYuksekNokta = karakterTransform.position.y;
            ChallengeProgress.Height(YukseklikSkoruAl());
        }

        GuncelSkorGoster();
    }

    private void GuncelSkorGoster()
    {
        guncelSkor = Mathf.RoundToInt(enYuksekNokta * 10) + toplamBonusPuan;
        if (skorText != null) skorText.text = "SCORE  " + guncelSkor;
    }

    public void BonusPuanEkle(int miktar)
    {
        toplamBonusPuan += miktar;
        GuncelSkorGoster();
    }

    // Difficulty follows platform height, never coin or combo bonus score.
    public float HareketliZeminOraniAl(float puan)
    {
        if (puan < 300f) return 0f;
        if (puan < 1000f) return Mathf.Lerp(0f, 0.2f, Mathf.InverseLerp(300f, 1000f, puan));
        if (puan < 2200f) return Mathf.Lerp(0.2f, 0.32f, Mathf.InverseLerp(1000f, 2200f, puan));
        if (puan < 4000f) return Mathf.Lerp(0.32f, 0.42f, Mathf.InverseLerp(2200f, 4000f, puan));
        return 0.42f;
    }

    private void TekArkaPlaniHazirla()
    {
        if (baslangicArkaPlanSeti != null)
        {
            baslangicArkaPlanSeti.SetActive(true);
            foreach (SpriteRenderer sr in baslangicArkaPlanSeti.GetComponentsInChildren<SpriteRenderer>(true))
            {
                Color renk = sr.color;
                renk.a = 1f;
                sr.color = renk;
            }
        }
        if (morArkaPlanSeti != null) morArkaPlanSeti.SetActive(false);
        if (uclulerArkaPlanSeti != null) uclulerArkaPlanSeti.SetActive(false);
    }

    public void CoinTopla()
    {
        toplananCoinSayisi++;
        ChallengeProgress.Star();
    }

    [System.Obsolete]
    public void OyunuBitir()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (pauseButton) pauseButton.SetActive(false);
        IsPaused = false;
        if (pausePanel) pausePanel.SetActive(false);
        if (pauseSettingsPanel) pauseSettingsPanel.SetActive(false);
        reklamKullanildiMi = false;

        if (SesYoneticisi.instance != null) SesYoneticisi.instance.GameOverCal();

        int coinBonus = toplananCoinSayisi * coinBasinaDeger;
        int toplamSkor = guncelSkor + coinBonus;

        int eskiCuzdanPuani = PlayerPrefs.GetInt("ToplamCuzdanPuani", 0);
        PlayerPrefs.SetInt("ToplamCuzdanPuani", eskiCuzdanPuani + toplamSkor);

        YerelSkorTablosu.YeniSkorEkle(toplamSkor);

        if (LiderTablosuYoneticisi.instance != null)
        {
            LiderTablosuYoneticisi.instance.SkorGonder(toplamSkor);
        }

        int enYuksekSkor = PlayerPrefs.GetInt(EN_YUKSEK_SKOR_KEY, 0);
        bool yeniRekorMu = toplamSkor > enYuksekSkor;

        if (yeniRekorMu)
        {
            enYuksekSkor = toplamSkor;
            PlayerPrefs.SetInt(EN_YUKSEK_SKOR_KEY, enYuksekSkor);
        }

        PlayerPrefs.Save();

        if (gameOverSkorYazisi != null) gameOverSkorYazisi.text = "SCORE  " + guncelSkor;
        if (yeniRekorObjesi != null) yeniRekorObjesi.SetActive(yeniRekorMu);

        if (reklamButonu != null) reklamButonu.SetActive(true);
        if (toplamSkorYazisi != null) toplamSkorYazisi.gameObject.SetActive(false);

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        if (coinBonus > 0)
        {
            StartCoroutine(CoinBonusGoster(coinBonus, toplamSkor));
        }
        else if (toplamSkorYazisi != null)
        {
            toplamSkorYazisi.gameObject.SetActive(true);
            toplamSkorYazisi.text = "TOTAL  " + toplamSkor;
        }
    }

    private IEnumerator CoinBonusGoster(int coinBonus, int toplamSkor)
    {
        yield return new WaitForSecondsRealtime(coinBonusGecikmesi);

        if (coinBonusYazisi != null)
        {
            coinBonusYazisi.gameObject.SetActive(true);
            coinBonusYazisi.text = "+ 0";
        }

        float gecenZaman = 0f;

        while (gecenZaman < sayimSuresi)
        {
            gecenZaman += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(gecenZaman / sayimSuresi);
            int gosterilenSayi = Mathf.RoundToInt(Mathf.Lerp(0, coinBonus, t));

            if (coinBonusYazisi != null) coinBonusYazisi.text = "+ " + gosterilenSayi;

            yield return null;
        }

        if (coinBonusYazisi != null) coinBonusYazisi.text = "+ " + coinBonus;

        yield return new WaitForSecondsRealtime(0.4f);

        if (toplamSkorYazisi != null)
        {
            toplamSkorYazisi.gameObject.SetActive(true);
            toplamSkorYazisi.text = "TOTAL  " + toplamSkor;
        }
    }

    public void ReklamlaDevamEtButonu()
    {
        if (reklamKullanildiMi) return;

        if (KVKKYoneticisi.instance != null)
        {
            KVKKYoneticisi.instance.ReklamOncesiKontrolEt(() =>
            {
                if (ReklamYoneticisi.instance != null)
                {
                    ReklamYoneticisi.instance.ReklamGoster(DevamEt);
                }
            });
        }
        else if (ReklamYoneticisi.instance != null)
        {
            ReklamYoneticisi.instance.ReklamGoster(DevamEt);
        }
    }

    private void DevamEt()
    {
        reklamKullanildiMi = true;
        if (pauseButton) pauseButton.SetActive(true);
        if (reklamButonu != null) reklamButonu.SetActive(false);

        isGameOver = false;
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (coinBonusYazisi != null) coinBonusYazisi.gameObject.SetActive(false);

        if (karakterTransform != null)
        {
            Vector3 yeniPozisyon = new Vector3(0f, enYuksekNokta - 1f, 0f);
            karakterTransform.position = yeniPozisyon;

            Rigidbody2D rb = karakterTransform.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            OyuncuKontrol oyuncu = karakterTransform.GetComponent<OyuncuKontrol>();
            if (oyuncu != null) oyuncu.GeciciKalkanVer(devamKalkanSuresi);
        }
    }

    public void YenidenBaslat()
    {
        Time.timeScale = 1f;
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AnaMenuyeGit()
    {
        Time.timeScale = 1f;
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }

    public void PauseGame()
    {
        if (isGameOver || IsPaused || pausePanel == null) return;
        pauseTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
        IsPaused = true;
        pausePanel.SetActive(true);
        if (pauseSettingsPanel) pauseSettingsPanel.SetActive(false);
        Time.timeScale = 0f;
        PlayerPrefs.Save();
    }
    public void ResumeGame()
    {
        if (!IsPaused || isGameOver) return;
        if (pausePanel) pausePanel.SetActive(false);
        if (pauseSettingsPanel) pauseSettingsPanel.SetActive(false);
        IsPaused = false;
        Time.timeScale = pauseTimeScale;
    }
    public void ShowPauseSettings()
    {
        if (!IsPaused) return;
        if (pausePanel) pausePanel.SetActive(false);
        if (pauseSettingsPanel) pauseSettingsPanel.SetActive(true);
        RefreshPauseAudio();
    }
    public void ClosePauseSettings()
    {
        if (!IsPaused) return;
        if (pauseSettingsPanel) pauseSettingsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(true);
    }
    public void ToggleMusic() { if (SesYoneticisi.instance) SesYoneticisi.instance.MuzikAcKapa(); RefreshPauseAudio(); }
    public void ToggleSound() { if (SesYoneticisi.instance) SesYoneticisi.instance.EfektAcKapa(); RefreshPauseAudio(); }
    public void RefreshPauseAudio()
    {
        if (pauseMusicLabel) pauseMusicLabel.text = "MUSIC  " + (PlayerPrefs.GetInt("MuzikAcik", 1) == 1 ? "ON" : "OFF");
        if (pauseSoundLabel) pauseSoundLabel.text = "SOUND  " + (PlayerPrefs.GetInt("EfektAcik", 1) == 1 ? "ON" : "OFF");
    }
    private void OnApplicationPause(bool paused) { if (paused) { PauseGame(); PlayerPrefs.Save(); } }
    private void OnApplicationFocus(bool focus) { if (!focus) PauseGame(); }

    public int GuncelSkoruAl()
    {
        return guncelSkor;
    }
}