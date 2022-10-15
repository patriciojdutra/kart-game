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

    [SerializeField]
    int life = 2;
    int currentLife;

    bool aguardandoParaNascer = false;

    // Start is called before the first frame update
    void Start()
    {
        currentLife = life;
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

    [PunRPC]
    public IEnumerator AguardarParaReviver(float countdownValue = 10)
    {
        aguardandoParaNascer = true;

        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        transform.Find("Vehicle").gameObject.SetActive(false);

        textMeshTitle.gameObject.SetActive(true);
        textMeshDescription.gameObject.SetActive(true);

        
        while (countdownValue > 0)
        {
            gameObject.transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 90f, transform.rotation.w);
            textMeshDescription.SetText(countdownValue.ToString());
            yield return new WaitForSeconds(1.0f);
            countdownValue--;
        }

        textMeshTitle.gameObject.SetActive(false);
        textMeshDescription.gameObject.SetActive(false);

        int p = Random.Range(0, 10);
        gameObject.transform.position = GameManager.Instance.spawnerList[10].position;
        gameObject.transform.rotation = GameManager.Instance.spawnerList[10].rotation;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        transform.Find("Vehicle").gameObject.SetActive(true);
        aguardandoParaNascer = false;
        

    }

    private void OnCollisionEnter(Collision col)
    {
        if (photonView.IsMine && !aguardandoParaNascer)
        {
            if (col.gameObject.tag == "Bomba")
            {
                currentLife -= 2;
                Debug.Log("Bomba Life " + currentLife);
            }

            if (col.gameObject.tag == "Missil")
            {
                currentLife -= 5;
                Debug.Log("Missil Life " + currentLife);
            }
        }

        if (currentLife <= 0)
        {
            morrer();
            currentLife = life;
        }
    }

}