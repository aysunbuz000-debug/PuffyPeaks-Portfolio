using System.Collections.Generic;
using UnityEngine;

public static class YerelSkorTablosu
{
    private const int MAKS_SKOR_SAYISI = 10;
    private const string SKOR_ANAHTAR_ONEK = "YuksekSkor_";

    public static void YeniSkorEkle(int yeniSkor)
    {
        List<int> skorlar = TumSkorlariAl();
        skorlar.Add(yeniSkor);
        skorlar.Sort((a, b) => b.CompareTo(a));

        if (skorlar.Count > MAKS_SKOR_SAYISI)
        {
            skorlar.RemoveRange(MAKS_SKOR_SAYISI, skorlar.Count - MAKS_SKOR_SAYISI);
        }

        for (int i = 0; i < MAKS_SKOR_SAYISI; i++)
        {
            if (i < skorlar.Count)
                PlayerPrefs.SetInt(SKOR_ANAHTAR_ONEK + i, skorlar[i]);
            else
                PlayerPrefs.SetInt(SKOR_ANAHTAR_ONEK + i, -1);
        }

        PlayerPrefs.Save();
    }

    public static List<int> TumSkorlariAl()
    {
        List<int> liste = new List<int>();
        for (int i = 0; i < MAKS_SKOR_SAYISI; i++)
        {
            int deger = PlayerPrefs.GetInt(SKOR_ANAHTAR_ONEK + i, -1);
            if (deger >= 0) liste.Add(deger);
        }
        return liste;
    }
}