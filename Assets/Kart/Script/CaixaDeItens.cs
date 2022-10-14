using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class CaixaDeItens : MonoBehaviour
{

    public float giro = 1;
    public float tempoDeEspera = 5;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        gameObject.transform.Rotate(new Vector3(0, giro * Time.deltaTime, giro * Time.deltaTime));
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            destruir();
        }
    }


    void destruir()
    {
        StartCoroutine(AguardarParaReaparecer(tempoDeEspera));
    }

    public IEnumerator AguardarParaReaparecer(float countdownValue)
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<AudioSource>().Play();

        while (countdownValue > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countdownValue--;
        }

        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
    }

}
