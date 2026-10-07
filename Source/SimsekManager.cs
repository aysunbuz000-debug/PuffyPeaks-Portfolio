using System.Collections;

using System.Collections.Generic;

using UnityEngine;

public class SimsekManager : MonoBehaviour

{

    [Header("Şimşek Ayarları")]

    public GameObject simsekPrefab;

    public float uretimSuresi = 3.5f;

    public float spawnGenisligi = 2.5f;

    public float yukariMesafe = 7.0f;

    [Header("Referanslar")]

    public Transform karakterTransform;

    private float zamanlayici;

    void Start()

    {

        zamanlayici = uretimSuresi;

    }

    void Update()

    {

        if (karakterTransform == null) return;

        zamanlayici -= Time.deltaTime;

        if (zamanlayici <= 0)

        {

            SimsekUret();

            zamanlayici = uretimSuresi;

        }

    }

    void SimsekUret()

    {

        float rastgeleX = Random.Range(karakterTransform.position.x - spawnGenisligi, karakterTransform.position.x + spawnGenisligi);

        float uretimY = karakterTransform.position.y + yukariMesafe;

        Vector3 uretimPozisyonu = new Vector3(rastgeleX, uretimY, 0);

        GameObject yeniSimsek = Instantiate(simsekPrefab, uretimPozisyonu, Quaternion.identity);

        Destroy(yeniSimsek, 5f);

    }

}