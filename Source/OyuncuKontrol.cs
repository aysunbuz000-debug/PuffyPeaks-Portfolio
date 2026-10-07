using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OyuncuKontrol : MonoBehaviour
{
    public float moveSpeed = 10f;

    [Header("Oynanis temposu")]
    [Range(1f, 1.6f)] public float tempoCarpani = 1.25f;
    [Range(1f, 1.5f)] public float dususCarpani = 1.1f;
    [Range(1f, 1.25f)] public float hizliYukselisCarpani = 1.1f;
    private float temelYercekimi;
    private RuzgarBolgesi ruzgarBolgesi;

    public void RuzgarKaynaginiAyarla(RuzgarBolgesi kaynak) { ruzgarBolgesi = kaynak; }
    private Rigidbody2D rb;
    private float yatayGiris = 0f;

    public float ekranSiniriX = 2.4f;

    public float sersemlemeSuresi = 1.5f;
    private bool sersemlediMi = false;
    private float sersemlemeZamanlayici = 0f;

    public GameObject kalkanGorseli;
    public float kalkanSureSiniri = 6.0f;
    public float yanipSonmeBaslangicSuresi = 2.0f;
    public float yanipSonmeHizi = 0.15f;

    private bool kalkanVarMi = false;
    private float kalkanZamanlayici = 0f;
    private float yanipSonmeZamanlayici;

    public GameObject miknatisGorseli;
    public float miknatisSuresi = 5f;
    public float miknatisYaricap = 3f;
    public float miknatisCekmeHizi = 8f;
    private bool miknatisVarMi = false;
    private float miknatisZamanlayici = 0f;

    public int yildizSkorDegeri = 100;

    [System.Serializable]
    public class KiyafetEslesme
    {
        public int kiyafetID;
        public GameObject kiyafetObjesi;
    }

    public KiyafetEslesme[] kiyafetler;

    private Vector3 baslangicScale;

    void Awake()
    {
        baslangicScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) temelYercekimi = rb.gravityScale;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (kalkanGorseli != null) kalkanGorseli.SetActive(false);
        if (miknatisGorseli != null) miknatisGorseli.SetActive(false);

        ApplyPurchasedSkin();
    }

    void Update()
    {
        if (Time.timeScale <= 0f) { yatayGiris = 0f; return; }
        if (kalkanVarMi)
        {
            kalkanZamanlayici -= Time.deltaTime;

            if (kalkanZamanlayici <= yanipSonmeBaslangicSuresi && kalkanGorseli != null)
            {
                yanipSonmeZamanlayici += Time.deltaTime;
                if (yanipSonmeZamanlayici >= yanipSonmeHizi)
                {
                    kalkanGorseli.SetActive(!kalkanGorseli.activeSelf);
                    yanipSonmeZamanlayici = 0f;
                }
            }

            if (kalkanZamanlayici <= 0)
            {
                KalkaniKapat();
            }
        }

        if (miknatisVarMi)
        {
            miknatisZamanlayici -= Time.deltaTime;

            MiknatisCoinleriCek();

            if (miknatisZamanlayici <= 0)
            {
                MiknatisiKapat();
            }
        }

        if (sersemlediMi)
        {
            yatayGiris = 0f;
            transform.Rotate(0, 0, 500 * Time.deltaTime);

            sersemlemeZamanlayici -= Time.deltaTime;
            if (sersemlemeZamanlayici <= 0)
            {
                sersemlediMi = false;
                transform.rotation = Quaternion.identity;
            }

            EkranSinirlariniKontrolEt();
            return;
        }

        yatayGiris = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                yatayGiris = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                yatayGiris = 1f;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 dokunmaPozisyonu = Touchscreen.current.primaryTouch.position.ReadValue();
            if (dokunmaPozisyonu.x > Screen.width / 2) { yatayGiris = 1f; }
            else { yatayGiris = -1f; }
        }

        EkranSinirlariniKontrolEt();
    }

    private void MiknatisCoinleriCek()
    {
        Collider2D[] yakinCoinler = Physics2D.OverlapCircleAll(transform.position, miknatisYaricap);

        foreach (var coinCollider in yakinCoinler)
        {
            if (coinCollider.CompareTag("YildizItem"))
            {
                coinCollider.isTrigger = true;

                Rigidbody2D coinRb = coinCollider.GetComponent<Rigidbody2D>();
                if (coinRb != null)
                {
                    coinRb.linearVelocity = Vector2.zero;
                    coinRb.gravityScale = 0f;
                    coinRb.bodyType = RigidbodyType2D.Kinematic;
                }

                coinCollider.transform.position = Vector3.MoveTowards(
                    coinCollider.transform.position,
                    transform.position,
                    miknatisCekmeHizi * Time.deltaTime
                );
            }
        }
    }

    public void PlatformdanZipla(float temelHiz, bool hizliYukselis)
    {
        if (rb == null) return;
        ChallengeProgress.Jump();
        float tempo = Mathf.Clamp(tempoCarpani, 1f, 1.6f);
        // v * tempo and g * tempo^2 keep the normal jump height unchanged.
        rb.gravityScale = temelYercekimi * tempo * tempo;
        float bonus = hizliYukselis ? Mathf.Clamp(hizliYukselisCarpani, 1f, 1.25f) : 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, temelHiz * tempo * bonus);
    }

    private void OnDisable()
    {
        if (rb != null) rb.gravityScale = temelYercekimi;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            float tempo = Mathf.Clamp(tempoCarpani, 1f, 1.6f);
            float dusus = rb.linearVelocity.y < 0f ? Mathf.Clamp(dususCarpani, 1f, 1.5f) : 1f;
            rb.gravityScale = temelYercekimi * tempo * tempo * dusus;
            float ruzgar = ruzgarBolgesi != null ? ruzgarBolgesi.YatayHiziAl(rb.position.y) : 0f;
            rb.linearVelocity = new Vector2(yatayGiris * moveSpeed * tempo + ruzgar, rb.linearVelocity.y);
        }

        if (!sersemlediMi)
        {
            float mutlakX = Mathf.Abs(baslangicScale.x);
            if (yatayGiris > 0) transform.localScale = new Vector3(mutlakX, baslangicScale.y, baslangicScale.z);
            else if (yatayGiris < 0) transform.localScale = new Vector3(-mutlakX, baslangicScale.y, baslangicScale.z);
        }
    }

    private void EkranSinirlariniKontrolEt()
    {
        float karakterGenisligi = 0.4f;
        if (transform.position.x + karakterGenisligi > ekranSiniriX)
        {
            float yeniX = -ekranSiniriX + karakterGenisligi;
            transform.position = new Vector3(yeniX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x - karakterGenisligi < -ekranSiniriX)
        {
            float yeniX = ekranSiniriX - karakterGenisligi;
            transform.position = new Vector3(yeniX, transform.position.y, transform.position.z);
        }
    }

    private void KalkaniKapat()
    {
        kalkanVarMi = false;
        kalkanZamanlayici = 0f;
        if (kalkanGorseli != null) kalkanGorseli.SetActive(false);
    }

    private void MiknatisiKapat()
    {
        miknatisVarMi = false;
        miknatisZamanlayici = 0f;
        if (miknatisGorseli != null) miknatisGorseli.SetActive(false);

        if (SesYoneticisi.instance != null) SesYoneticisi.instance.MiknatisSesiDurdur();
    }

    public void GeciciKalkanVer(float sure)
    {
        kalkanVarMi = true;
        kalkanZamanlayici = sure;
        yanipSonmeZamanlayici = 0f;

        if (kalkanGorseli != null) kalkanGorseli.SetActive(true);
    }

    public void ApplyPurchasedSkin()
    {
        int kusanilanKiyafet = PlayerPrefs.GetInt("KusanilanKiyafetID", -1);

        foreach (var kiyafet in kiyafetler)
        {
            if (kiyafet.kiyafetObjesi != null)
                kiyafet.kiyafetObjesi.SetActive(kiyafet.kiyafetID == kusanilanKiyafet);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("YildizItem"))
        {
            if (SesYoneticisi.instance != null) SesYoneticisi.instance.YildizCal();
            if (GameManager.instance != null) GameManager.instance.CoinTopla();

            Destroy(collision.gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("KalkanItem"))
        {
            if (SesYoneticisi.instance != null) SesYoneticisi.instance.KalkanCal();

            kalkanVarMi = true;
            kalkanZamanlayici = kalkanSureSiniri;
            yanipSonmeZamanlayici = 0f;

            if (kalkanGorseli != null) kalkanGorseli.SetActive(true);

            Destroy(collision.gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("MiknatisItem"))
        {
            if (SesYoneticisi.instance != null) SesYoneticisi.instance.MiknatisSesiBaslat();

            miknatisVarMi = true;
            miknatisZamanlayici = miknatisSuresi;

            if (miknatisGorseli != null) miknatisGorseli.SetActive(true);

            Destroy(collision.gameObject);
            return;
        }

        if (collision.gameObject.name.Contains("Simsek"))
        {
            if (kalkanVarMi)
            {
                if (SesYoneticisi.instance != null) SesYoneticisi.instance.KalkanEngellemeCal();

                KalkaniKapat();
                Destroy(collision.gameObject);
            }
            else if (!sersemlediMi)
            {
                if (SesYoneticisi.instance != null) SesYoneticisi.instance.CarpmaCal();

                sersemlediMi = true;
                sersemlemeZamanlayici = sersemlemeSuresi;

                float rastgeleFirlatmaX = Random.Range(-3f, 3f);

                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(rastgeleFirlatmaX, rb.linearVelocity.y);
                }

                Destroy(collision.gameObject);
            }
        }
    }
}