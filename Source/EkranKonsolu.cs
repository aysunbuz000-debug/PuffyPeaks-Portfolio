using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;


public class EkranKonsolu : MonoBehaviour
{
    public TextMeshProUGUI konsolMetni;
    public int maksimumSatir = 25;

    private readonly List<string> loglar = new List<string>();

    private void OnEnable()
    {
        Application.logMessageReceived += LogAlindi;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= LogAlindi;
    }

    private void LogAlindi(string logString, string stackTrace, LogType type)
    {
        string renk = "white";
        if (type == LogType.Error || type == LogType.Exception) renk = "red";
        else if (type == LogType.Warning) renk = "yellow";

        loglar.Add($"<color={renk}>{logString}</color>");

        if (loglar.Count > maksimumSatir)
        {
            loglar.RemoveAt(0);
        }

        Guncelle();
    }

    private void Guncelle()
    {
        if (konsolMetni == null) return;

        StringBuilder sb = new StringBuilder();
        foreach (var satir in loglar)
        {
            sb.AppendLine(satir);
        }
        konsolMetni.text = sb.ToString();
    }
}