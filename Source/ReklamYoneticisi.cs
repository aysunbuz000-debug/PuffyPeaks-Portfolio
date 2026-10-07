using UnityEngine;
using Unity.Services.LevelPlay;
using TMPro;

public class ReklamYoneticisi : MonoBehaviour
{
    public static ReklamYoneticisi instance;

    public string gameId = "";
    public string rewardedAdUnitId = "";

    public TextMeshProUGUI debugMetni;

    private LevelPlayRewardedAd rewardedAd;
    private bool sdkHazirMi = false;
    private bool oduluAldiMi = false;

    public delegate void DevamEtCallback();
    private DevamEtCallback aktifCallback;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DebugYaz("Başlatılıyor... gameId: " + gameId);

        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;
        LevelPlay.Init(gameId);
    }

    private void OnInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("LevelPlay başarıyla başlatıldı.");
        DebugYaz("LevelPlay başlatıldı. Reklam yükleniyor...");
        sdkHazirMi = true;
        RewardedAdOlustur();
    }

    private void OnInitFailed(LevelPlayInitError error)
    {
        Debug.LogError("LevelPlay başlatılamadı: " + error);
        DebugYaz(" INIT HATASI: " + error);
    }

    private void RewardedAdOlustur()
    {
        rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);
        rewardedAd.OnAdLoaded += OnAdLoaded;
        rewardedAd.OnAdLoadFailed += OnAdLoadFailed;
        rewardedAd.OnAdDisplayed += OnAdDisplayed;
        rewardedAd.OnAdDisplayFailed += OnAdDisplayFailed;
        rewardedAd.OnAdRewarded += OnAdRewarded;
        rewardedAd.OnAdClosed += OnAdClosed;
        rewardedAd.LoadAd();
    }

    private void OnAdLoaded(LevelPlayAdInfo info)
    {
        Debug.Log("Ödüllü reklam hazır.");
        DebugYaz(" Reklam HAZIR! (network: " + info.AdNetwork + ")");
    }

    private void OnAdLoadFailed(LevelPlayAdError error)
    {
        Debug.Log("Reklam yüklenemedi: " + error);
        DebugYaz(" YÜKLEME HATASI:\n" + error.ToString());
    }

    private void OnAdDisplayed(LevelPlayAdInfo info) { }

    private void OnAdDisplayFailed(LevelPlayAdInfo info, LevelPlayAdError error)
    {
        Debug.Log("Reklam gösterilemedi: " + error);
        DebugYaz(" GÖSTERİM HATASI:\n" + error.ToString());
    }

    private void OnAdRewarded(LevelPlayAdInfo info, LevelPlayReward reward)
    {
        oduluAldiMi = true;
    }

    private void OnAdClosed(LevelPlayAdInfo info)
    {
        if (oduluAldiMi && aktifCallback != null)
        {
            aktifCallback.Invoke();
        }
        oduluAldiMi = false;
        aktifCallback = null;
        if (rewardedAd != null) rewardedAd.LoadAd();
    }

    public bool ReklamHazirMi()
    {
        return sdkHazirMi && rewardedAd != null && rewardedAd.IsAdReady();
    }

    public void ReklamGoster(DevamEtCallback basariliCallback)
    {
        if (ReklamHazirMi())
        {
            aktifCallback = basariliCallback;
            rewardedAd.ShowAd();
        }
        else
        {
            Debug.Log("Reklam henüz hazır değil, biraz bekle.");
            DebugYaz("Butona basıldı ama reklam hazır değil.\nsdkHazirMi: " + sdkHazirMi + "\nrewardedAd null mu: " + (rewardedAd == null));
        }
    }

    private void DebugYaz(string mesaj)
    {
        if (debugMetni != null)
        {
            debugMetni.text = mesaj;
        }
    }
}