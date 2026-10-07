using UnityEngine;
using Unity.Services.LevelPlay;

public class KVKKYoneticisi : MonoBehaviour
{
    public static KVKKYoneticisi instance;

    public GameObject onayliyorumTik;
    public GameObject onaylamiyorumTik;

    private const string KVKK_GOSTERILDI_KEY = "KVKKGosterildi";
    private const string KVKK_ONAY_KEY = "KVKKOnay";

    public GameObject kvkkPaneli;

    private System.Action onayTamamlaninca;

    private void GostergeleriGuncelle()
    {
        bool gosterildiMi = KVKKGosterildiMi();
        bool onayVar = KVKKOnayVerildiMi();

        if (onayliyorumTik != null)
            onayliyorumTik.SetActive(gosterildiMi && onayVar);

        if (onaylamiyorumTik != null)
            onaylamiyorumTik.SetActive(gosterildiMi && !onayVar);
    }

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
        if (kvkkPaneli != null) kvkkPaneli.SetActive(false);

        if (KVKKGosterildiMi())
        {
            LevelPlayPrivacySettings.SetGDPRConsent(KVKKOnayVerildiMi());
        }
    }

    public bool KVKKGosterildiMi() => PlayerPrefs.GetInt(KVKK_GOSTERILDI_KEY, 0) == 1;
    public bool KVKKOnayVerildiMi() => PlayerPrefs.GetInt(KVKK_ONAY_KEY, 0) == 1;

    public void ReklamOncesiKontrolEt(System.Action devamEt)
    {
        if (KVKKGosterildiMi())
        {
            devamEt?.Invoke();
            return;
        }

        onayTamamlaninca = devamEt;
        if (kvkkPaneli != null) kvkkPaneli.SetActive(true);
        GostergeleriGuncelle(); 
    }

    public void AyarlardanAc()
    {
        onayTamamlaninca = null;
        if (kvkkPaneli != null) kvkkPaneli.SetActive(true);
        GostergeleriGuncelle(); 
    }

    public void OnayVer()
    {
        PlayerPrefs.SetInt(KVKK_GOSTERILDI_KEY, 1);
        PlayerPrefs.SetInt(KVKK_ONAY_KEY, 1);
        PlayerPrefs.Save();

        LevelPlayPrivacySettings.SetGDPRConsent(true);
        GostergeleriGuncelle(); 
        Kapat();
    }

    public void OnayVerme()
    {
        PlayerPrefs.SetInt(KVKK_GOSTERILDI_KEY, 1);
        PlayerPrefs.SetInt(KVKK_ONAY_KEY, 0);
        PlayerPrefs.Save();

        LevelPlayPrivacySettings.SetGDPRConsent(false);
        GostergeleriGuncelle(); 
        Kapat();
    }

    private void Kapat()
    {
        if (kvkkPaneli != null) kvkkPaneli.SetActive(false);

        var callback = onayTamamlaninca;
        onayTamamlaninca = null;
        callback?.Invoke();
    }
}