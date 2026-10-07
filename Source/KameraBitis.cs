using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KameraBitis : MonoBehaviour
{
    public Transform karakter; 
    public float dusmeToleransi = 6f; 

    void Update()
    {
        if (karakter != null)
        {
            
            if (karakter.position.y < transform.position.y - dusmeToleransi)
            {
                GameManager.instance.OyunuBitir();
            }
        }
    }
}