using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using Random = UnityEngine.Random;
using TMPro;

public class Vida : MonoBehaviourPunCallbacks
{
    
    [SerializeField]
    TMP_Text textMeshTitle;

    [SerializeField]
    TMP_Text textMeshDescription;

    bool aguardandoParaNascer = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        if (gameObject.transform.position.y < -30 && !aguardandoParaNascer)
        {           
            morrer();
        }
    }

    void morrer()
    {
        StartCoroutine(AguardarParaReviver());
    }

    public IEnumerator AguardarParaReviver(float countdownValue = 5)
    {
        aguardandoParaNascer = true;
        int p = Random.Range(0, 10);

        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        transform.Find("Vehicle").gameObject.SetActive(false);
        gameObject.transform.position = GameManager.Instance.spawnerList[p].position;
        gameObject.transform.rotation = GameManager.Instance.spawnerList[p].rotation;

        textMeshTitle.gameObject.SetActive(true);
        textMeshDescription.gameObject.SetActive(true);

        
        while (countdownValue > 0)
        {
            textMeshDescription.SetText(countdownValue.ToString());
            yield return new WaitForSeconds(1.0f);
            countdownValue--;
        }

        textMeshTitle.gameObject.SetActive(false);
        textMeshDescription.gameObject.SetActive(false);

        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        transform.Find("Vehicle").gameObject.SetActive(true);
        aguardandoParaNascer = false;

    }

}