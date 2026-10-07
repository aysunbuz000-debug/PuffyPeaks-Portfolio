using UnityEngine;



public class MenuCanlandirici : MonoBehaviour

{

    public enum HareketTipi

    {

        YuzmeYukariAsagi,

        NabizBuyukKucuk,

        YanaKayma

    }



    [Header("Hareket Ayarı")]

    public HareketTipi hareketTipi = HareketTipi.YuzmeYukariAsagi;



    [Header("Genlik (ne kadar hareket etsin)")]

    [Tooltip("Yüzme/kayma için piksel/birim, Nabız için ölçek yüzdesi (0.05 = %5)")]

    public float genlik = 15f;



    [Header("Hız")]

    public float hiz = 1.5f;



    [Header("Başlangıç Gecikmesi (opsiyonel, farklı objeler senkron olmasın diye)")]

    public float fazKaymasi = 0f;



    private Vector3 baslangicPozisyon;

    private Vector3 baslangicOlcek;



    private void Start()

    {

        baslangicPozisyon = transform.localPosition;

        baslangicOlcek = transform.localScale;

    }



    private void Update()

    {

        float t = (Time.unscaledTime + fazKaymasi) * hiz;



        switch (hareketTipi)

        {

            case HareketTipi.YuzmeYukariAsagi:

                float yOfset = Mathf.Sin(t) * genlik;

                transform.localPosition = baslangicPozisyon + new Vector3(0f, yOfset, 0f);

                break;



            case HareketTipi.NabizBuyukKucuk:

                float olcekFaktoru = 1f + Mathf.Sin(t) * genlik;

                transform.localScale = baslangicOlcek * olcekFaktoru;

                break;



            case HareketTipi.YanaKayma:

                float xOfset = Mathf.Sin(t) * genlik;

                transform.localPosition = baslangicPozisyon + new Vector3(xOfset, 0f, 0f);

                break;

        }

    }

}



