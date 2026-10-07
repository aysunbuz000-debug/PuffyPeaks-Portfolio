using UnityEngine;

// Height bands affect gameplay independently of the camera and visual effects.
public class RuzgarBolgesi : MonoBehaviour
{
    public float ilkYukseklik = 14f;
    public float aralik = 26f;
    public float bolgeYuksekligi = 4f;
    public float yatayHiz = 1.8f;

    private const int CizgiSayisi = 12;
    private readonly LineRenderer[] cizgiler = new LineRenderer[CizgiSayisi];
    private Transform oyuncu;
    private Camera oyunKamerasi;
    private Material cizgiMateryali;
    private float sonYukseklik;
    private float ekranYarisi;
    private bool hazir;

    public void Hazirla(Transform hedef, float sonPlatformY, float yarimGenislik)
    {
        if (hazir) return;
        oyuncu = hedef;
        sonYukseklik = sonPlatformY;
        ekranYarisi = Mathf.Max(1f, yarimGenislik);
        oyunKamerasi = Camera.main;
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            Debug.LogWarning("Ruzgar cizgi shader'i bulunamadi; ruzgar devre disi.");
            return;
        }
        cizgiMateryali = new Material(shader);
        SpriteRenderer karakterGorseli = hedef.GetComponent<SpriteRenderer>();
        for (int i = 0; i < CizgiSayisi; i++)
        {
            GameObject obje = new GameObject("RuzgarCizgisi_" + i);
            obje.transform.SetParent(transform, false);
            LineRenderer cizgi = obje.AddComponent<LineRenderer>();
            cizgi.sharedMaterial = cizgiMateryali;
            cizgi.useWorldSpace = true;
            cizgi.positionCount = 7;
            cizgi.startWidth = 0.025f;
            cizgi.endWidth = 0.045f;
            cizgi.numCapVertices = 3;
            cizgi.numCornerVertices = 2;
            cizgi.startColor = new Color(0.7f, 0.95f, 1f, 0f);
            cizgi.endColor = new Color(0.85f, 0.98f, 1f, 0.48f);
            if (karakterGorseli != null)
            {
                cizgi.sortingLayerID = karakterGorseli.sortingLayerID;
                cizgi.sortingOrder = karakterGorseli.sortingOrder + 1;
            }
            cizgi.enabled = false;
            cizgiler[i] = cizgi;
        }
        hazir = true;
    }

    private float GuvenliAralik { get { return Mathf.Max(Mathf.Max(1f, bolgeYuksekligi) + 4f, aralik); } }

    public float YatayHiziAl(float yukseklik)
    {
        if (!hazir || !isActiveAndEnabled) return 0f;
        int bolge = Mathf.RoundToInt((yukseklik - ilkYukseklik) / GuvenliAralik);
        float merkez = ilkYukseklik + bolge * GuvenliAralik;
        if (bolge < 0 || merkez > sonYukseklik) return 0f;
        float yarimYukseklik = Mathf.Max(1f, bolgeYuksekligi) * 0.5f;
        float uzaklik = Mathf.Abs(yukseklik - merkez);
        // Soft edges prevent abrupt steering changes on entry and exit.
        float etki = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(yarimYukseklik * 0.65f, yarimYukseklik, uzaklik));
        return (bolge % 2 == 0 ? 1f : -1f) * Mathf.Clamp(yatayHiz, 0f, 3f) * etki;
    }

    private void Update()
    {
        if (!hazir || oyuncu == null) return;
        float referansY = oyunKamerasi != null ? oyunKamerasi.transform.position.y : oyuncu.position.y;
        int bolge = Mathf.Max(0, Mathf.RoundToInt((referansY - ilkYukseklik) / GuvenliAralik));
        float merkez = ilkYukseklik + bolge * GuvenliAralik;
        float gorusYarisi = oyunKamerasi != null && oyunKamerasi.orthographic ? oyunKamerasi.orthographicSize : 7f;
        bool gorunur = merkez <= sonYukseklik && Mathf.Abs(merkez - referansY) < gorusYarisi + bolgeYuksekligi;
        float yon = bolge % 2 == 0 ? 1f : -1f;
        for (int i = 0; i < CizgiSayisi; i++)
        {
            LineRenderer cizgi = cizgiler[i];
            cizgi.enabled = gorunur;
            if (!gorunur) continue;
            float faz = Mathf.Repeat(Time.time * 0.32f + i * 0.618f, 1f);
            float x = Mathf.Lerp(-ekranYarisi - 1f, ekranYarisi + 1f, faz) * yon;
            float y = merkez + ((i % 6) / 5f - 0.5f) * Mathf.Max(1f, bolgeYuksekligi) * 0.8f;
            float alpha = Mathf.Sin(faz * Mathf.PI) * 0.48f;
            cizgi.endColor = new Color(0.85f, 0.98f, 1f, alpha);
            cizgi.SetPosition(0, new Vector3(x - yon * 0.7f, y, 0f));
            cizgi.SetPosition(1, new Vector3(x - yon * 0.48f, y + 0.04f, 0f));
            cizgi.SetPosition(2, new Vector3(x - yon * 0.24f, y + 0.05f, 0f));
            cizgi.SetPosition(3, new Vector3(x, y, 0f));
            cizgi.SetPosition(4, new Vector3(x - yon * 0.14f, y + 0.12f, 0f));
            cizgi.SetPosition(5, new Vector3(x, y, 0f));
            cizgi.SetPosition(6, new Vector3(x - yon * 0.14f, y - 0.12f, 0f));
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < cizgiler.Length; i++)
            if (cizgiler[i] != null) cizgiler[i].enabled = false;
    }

    private void OnDestroy()
    {
        if (cizgiMateryali != null) Destroy(cizgiMateryali);
    }
}