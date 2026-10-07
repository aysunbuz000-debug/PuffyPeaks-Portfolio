using UnityEngine;

using UnityEngine.UI;



public class SesIkonButonu : MonoBehaviour

{

    public enum SesTipi { Muzik, Efekt }



    [Header("Ayar")]

    public SesTipi sesTipi;



    [Header("Görseller")]

    public Image ikonGorseli;

    public Sprite acikSprite;

    public Sprite kapaliSprite;



    private Button butonComponent;



    private void Start()

    {

        butonComponent = GetComponent<Button>();

        if (butonComponent != null)

        {

            butonComponent.onClick.AddListener(Tiklandi);

        }



        GorunumuGuncelle();

    }



    private void Tiklandi()

    {

        if (SesYoneticisi.instance == null) return;



        if (sesTipi == SesTipi.Muzik)

            SesYoneticisi.instance.MuzikAcKapa();

        else

            SesYoneticisi.instance.EfektAcKapa();



        GorunumuGuncelle();

    }



    private void GorunumuGuncelle()

    {

        if (SesYoneticisi.instance == null || ikonGorseli == null) return;



        bool acikMi = (sesTipi == SesTipi.Muzik)

            ? SesYoneticisi.instance.MuzikAcikMi()

            : SesYoneticisi.instance.EfektAcikMi();



        ikonGorseli.sprite = acikMi ? acikSprite : kapaliSprite;

    }

}