using System.Collections;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

public class CaixaDeItens : MonoBehaviour
{

    public float giro = 1;
    public float tempoDeEspera = 5;


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
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " play som ");

        while (countdownValue > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countdownValue--;
        }

        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
    }

}
