using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Random = UnityEngine.Random;


public class Player : MonoBehaviourPunCallbacks
{
    public Image imageCaixa;
    public GameObject miraFrente;
    public GameObject miraTras;
    public Item[] itemList = new Item[3];
    int itemSelecionado = -1;

    void Awake()
    {
        if (photonView.IsMine)
        {
            if (imageCaixa == null)
                imageCaixa = GetComponent<Image>();

            if (miraFrente == null)
                miraFrente = transform.Find("MiraFrente").gameObject;

            if (miraTras == null)
                miraTras = transform.Find("MiraTras").gameObject;
        } 
    }

    void Start()
    {     
        imageCaixa.enabled = false;      
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Atirar();
        }
    }

    void Atirar()
    {
        if(itemSelecionado > -1 && photonView.IsMine)
        {
            GameObject bala = itemList[itemSelecionado].bala;
            Quaternion rotacaoBala = miraFrente.transform.rotation;
            Vector3 posicaoBala = miraFrente.transform.position;
            PhotonNetwork.Instantiate(bala.name, posicaoBala, rotacaoBala);

            imageCaixa.enabled = false;
            itemSelecionado = -1;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "CaixaDeItens" && photonView.IsMine)
        {
            GerarItem();
        }
    }

    void GerarItem()
    {
        if (photonView.IsMine)
        {
            itemSelecionado = Random.Range(0, itemList.Length);
            imageCaixa.sprite = itemList[itemSelecionado].image;
            imageCaixa.enabled = true;
        }
    }
}
