using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkorTablosuGosterici : MonoBehaviour
{
   
    public TextMeshProUGUI[] skorSatirlari;

    public Image[] satirArkaPlanlari; 

    public Color acikSatirRengi = new Color(1f, 1f, 1f, 0.15f);
    public Color koyuSatirRengi = new Color(1f, 1f, 1f, 0.3f);

    public Color altinRengi = new Color(1f, 0.84f, 0f);
    public Color gumusRengi = new Color(0.75f, 0.75f, 0.75f);
    public Color bronzRengi = new Color(0.8f, 0.5f, 0.2f);

    private void OnEnable()
    {
        Guncelle();
    }

    public void Guncelle()
    {
        var skorlar = YerelSkorTablosu.TumSkorlariAl();

        for (int i = 0; i < skorSatirlari.Length; i++)
        {
            if (skorSatirlari[i] == null) continue;

           
            if (i < skorlar.Count)
                skorSatirlari[i].text = skorlar[i].ToString();
            else
                skorSatirlari[i].text = "-";

          
            if (i == 0) skorSatirlari[i].color = altinRengi;
            else if (i == 1) skorSatirlari[i].color = gumusRengi;
            else if (i == 2) skorSatirlari[i].color = bronzRengi;
            else skorSatirlari[i].color = Color.white;

           
            if (satirArkaPlanlari != null && i < satirArkaPlanlari.Length && satirArkaPlanlari[i] != null)
            {
                satirArkaPlanlari[i].color = (i % 2 == 0) ? koyuSatirRengi : acikSatirRengi;
            }
        }
    }
}