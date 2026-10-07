using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using TMPro;

public class LiderTablosuYoneticisi : MonoBehaviour
{
    public static LiderTablosuYoneticisi instance;

    public TextMeshProUGUI debugMetni;

    private bool girisYapildiMi = false;

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
    }

    private void Start()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        DebugYaz("Play Games başlatılıyor, giriş deneniyor...");
        GirisYap();
    }

    public void GirisYap()
    {
        DebugYaz("Authenticate çağrıldı, bekleniyor...");

        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            girisYapildiMi = (success == SignInStatus.Success);
            Debug.Log("Play Games giriş sonucu: " + success);
            DebugYaz(girisYapildiMi
                ? " Giriş BAŞARILI. Kullanıcı: " + PlayGamesPlatform.Instance.GetUserDisplayName()
                : " Giriş BAŞARISIZ. Durum: " + success);
        });
    }

    [System.Obsolete]
    public void SkorGonder(int skor)
    {
        if (!girisYapildiMi)
        {
            DebugYaz(" Skor gönderilemedi, giriş yapılmamış.");
            return;
        }

        Social.ReportScore(skor, GPGSIds.leaderboard_best_peaks, (bool basarili) =>
        {
            Debug.Log("Skor gönderildi mi: " + basarili);
            DebugYaz(basarili ? " Skor gönderildi: " + skor : " Skor gönderilemedi.");
        });
    }

    public void LiderTablosunuGoster()
    {
        DebugYaz("Kupa butonuna basıldı. girisYapildiMi: " + girisYapildiMi);

        if (!girisYapildiMi)
        {
            DebugYaz("Giriş yapılmamış, tekrar deneniyor...");
            GirisYap();
            return;
        }

        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_best_peaks);
    }

    public bool GirisYapildiMi()
    {
        return girisYapildiMi;
    }

    private void DebugYaz(string mesaj)
    {
        Debug.Log("[LiderTablosu] " + mesaj);
        if (debugMetni != null)
        {
            debugMetni.text = mesaj;
        }
    }
}