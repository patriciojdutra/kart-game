using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Player : MonoBehaviour
{
    public Image imageCaixa;
    public Item[] itemList = new Item[3];
    int itemSelecionado = -1;

    private void Start()
    {
        imageCaixa.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            atirar();
        }
      
    }

    void atirar()
    {
        if(itemSelecionado > -1)
        {
            GameObject bala = itemList[itemSelecionado].bala;
            Quaternion rotacaoBala = gameObject.transform.rotation;
            Vector3 posicaoBala = gameObject.transform.position;
            posicaoBala.z += 1.5f;

            Instantiate(bala, posicaoBala, rotacaoBala);
            imageCaixa.enabled = false;
            itemSelecionado = -1;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "CaixaDeItens")
        {
            gerarItem();
        }
    }

    void gerarItem()
    {
        itemSelecionado = Random.Range(0, itemList.Length);
        imageCaixa.sprite = itemList[itemSelecionado].image;
        imageCaixa.enabled = true;
    }

}
