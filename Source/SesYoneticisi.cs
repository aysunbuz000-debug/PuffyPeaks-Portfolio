using UnityEngine;

public class SesYoneticisi : MonoBehaviour
{
    public static SesYoneticisi instance;

    public AudioSource sfxKaynagi;
    public AudioSource muzikKaynagi;

    public AudioClip[] zplamaSesleri;
    public AudioClip yildizSesi;
    public AudioClip kalkanSesi;
    public AudioClip kalkanEngellemeSesi;
    public AudioClip carpmaSesi;
    public AudioClip gameOverSesi;
    public AudioClip butonSesi;
    public AudioClip komboSesi;
    public AudioClip komboBonusSesi;

    [Range(0f, 0.1f)] public float komboPitchArtisi = 0.04f;

    [Range(0f, 0.3f)] public float pitchDegisimAraligi = 0.1f;

    public AudioClip arkaPlanMuzigi;

    public AudioSource miknatisSesKaynagi;
    public AudioClip miknatisSesi;

    private const string MUZIK_ACIK_KEY = "MuzikAcik";
    private const string EFEKT_ACIK_KEY = "EfektAcik";

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

        MuzikDurumunuUygula();
    }

    private void Start()
    {
        if (muzikKaynagi != null && arkaPlanMuzigi != null)
        {
            muzikKaynagi.clip = arkaPlanMuzigi;
            muzikKaynagi.loop = true;
            muzikKaynagi.Play();
        }
    }

    public void MiknatisSesiBaslat()
    {
        if (miknatisSesKaynagi != null && miknatisSesi != null && EfektAcikMi())
        {
            miknatisSesKaynagi.clip = miknatisSesi;
            miknatisSesKaynagi.loop = true;
            miknatisSesKaynagi.Play();
        }
    }

    public void MiknatisSesiDurdur()
    {
        if (miknatisSesKaynagi != null) miknatisSesKaynagi.Stop();
    }

    public void EfektCal(AudioClip clip)
    {
        if (!EfektAcikMi()) return;
        if (sfxKaynagi != null && clip != null)
        {
            sfxKaynagi.pitch = 1f;
            sfxKaynagi.PlayOneShot(clip);
        }
    }

    public void EfektCalRastgelePitch(AudioClip clip)
    {
        if (!EfektAcikMi()) return;
        if (sfxKaynagi != null && clip != null)
        {
            sfxKaynagi.pitch = 1f + Random.Range(-pitchDegisimAraligi, pitchDegisimAraligi);
            sfxKaynagi.PlayOneShot(clip);
        }
    }

    public void ZiplamaCal()
    {
        if (zplamaSesleri != null && zplamaSesleri.Length > 0)
        {
            AudioClip secilenSes = zplamaSesleri[Random.Range(0, zplamaSesleri.Length)];
            EfektCalRastgelePitch(secilenSes);
        }
    }

    public void YildizCal() => EfektCalRastgelePitch(yildizSesi);
    public void KalkanCal() => EfektCal(kalkanSesi);
    public void CarpmaCal() => EfektCal(carpmaSesi);
    public void GameOverCal() => EfektCal(gameOverSesi);
    public void ButonCal() => EfektCal(butonSesi);
    public void KalkanEngellemeCal() => EfektCal(kalkanEngellemeSesi);

    public void MuzikAcKapa()
    {
        bool yeniDurum = !MuzikAcikMi();
        PlayerPrefs.SetInt(MUZIK_ACIK_KEY, yeniDurum ? 1 : 0);
        PlayerPrefs.Save();
        MuzikDurumunuUygula();
    }

    public void EfektAcKapa()
    {
        bool yeniDurum = !EfektAcikMi();
        PlayerPrefs.SetInt(EFEKT_ACIK_KEY, yeniDurum ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool MuzikAcikMi() => PlayerPrefs.GetInt(MUZIK_ACIK_KEY, 1) == 1;
    public bool EfektAcikMi() => PlayerPrefs.GetInt(EFEKT_ACIK_KEY, 1) == 1;

    private void MuzikDurumunuUygula()
    {
        if (muzikKaynagi != null) muzikKaynagi.mute = !MuzikAcikMi();
    }

    public void KomboSesiCal(int seviye)
    {
        if (!EfektAcikMi()) return;
        if (sfxKaynagi != null && komboSesi != null)
        {
            sfxKaynagi.pitch = 1f + Mathf.Min(seviye, 10) * komboPitchArtisi;
            sfxKaynagi.PlayOneShot(komboSesi);
        }
    }

    public void KomboBonusSesiCal() => EfektCal(komboBonusSesi);
}