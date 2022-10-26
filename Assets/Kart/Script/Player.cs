using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using Random = UnityEngine.Random;
using TMPro;
using Unity.Burst.CompilerServices;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class Player : MonoBehaviourPunCallbacks
{
    //--------Notificação-----------------------------------
    [SerializeField]
    GameObject localPanelNotification;

    //--------Panel kill-----------------------------------
    [SerializeField]
    GameObject localPanelKill;

    //--------Item-----------------------------------
    public Image imageCaixa;
    public GameObject miraFrente;
    public GameObject miraTras;
    public Item[] itemList = new Item[3];
    int itemSelecionado = -1;

    //--------Vida-----------------------------------
    [SerializeField]
    TMP_Text textMeshTitle;
    [SerializeField]
    TMP_Text textMeshDescription;
    [SerializeField]
    int life = 2;
    int currentLife;
    bool aguardandoParaNascer = false;

    void Awake()
    {

        localPanelKill.SetActive(false);
        localPanelNotification.SetActive(false);

        if (photonView.IsMine)
        {
            photonView.Owner.SetScore(0);

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
        currentLife = life;
        imageCaixa.enabled = false;

        if (!photonView.IsMine)
            return;


        localPanelKill.SetActive(true);
        localPanelNotification.SetActive(true);
        UpdatePlacar();
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Atirar();
        }
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

    
    static Text CreateText(Transform parent, string namePlayer, TextAnchor textAnchor = TextAnchor.MiddleCenter)
    {
        var go = new GameObject();
        go.name = namePlayer;
        go.transform.parent = parent;
        var text = go.AddComponent<Text>();
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 30);
        text.text = namePlayer;
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = textAnchor;
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = ArialFont;
        return text;
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

    void GerarItem()
    {
        if (photonView.IsMine)
        {
            itemSelecionado = Random.Range(0, itemList.Length);
            imageCaixa.sprite = itemList[itemSelecionado].image;
            imageCaixa.enabled = true;
        }
    }

    void morrer()
    {
        StartCoroutine(AguardarParaReviver());
    }


    [PunRPC]
    public IEnumerator AguardarParaReviver(float countdownValue = 5)
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
        //gameObject.transform.position = GameManager.Instance.spawnerList[10].position;
        //gameObject.transform.rotation = GameManager.Instance.spawnerList[10].rotation;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        transform.Find("Vehicle").gameObject.SetActive(true);
        aguardandoParaNascer = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "CaixaDeItens" && photonView.IsMine)
        {
            GerarItem();
        }
    }

    private void OnCollisionEnter(Collision col)
    {

        var otherPlayer = col.gameObject.GetComponent<PhotonView>();


        if (photonView.IsMine && !aguardandoParaNascer && (otherPlayer == null || !otherPlayer.IsMine))
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

            if (currentLife <= 0)
            {
                otherPlayer.Owner.AddScore(1);

                string msg = otherPlayer.Owner.NickName + " matou " + PhotonNetwork.LocalPlayer.NickName;
                ChangeNumberKill(msg);

                morrer();
                currentLife = life;
            }
        }
    }

    private void ChangeNumberKill(string msg)
    {
        byte code = 1;
        string content = msg;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(code, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void RemoveAllChild(GameObject obj)
    {
        int childst = obj.transform.childCount;
        for (int i = 0; i < childst; i++)
        {
            Destroy(obj.transform.GetChild(i).gameObject);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 1)
        {
            string data = (string)photonEvent.CustomData;

            var textData = CreateText(localPanelNotification.transform, data, TextAnchor.MiddleLeft);
            Destroy(textData.gameObject, 5);
            UpdatePlacar();
        }
    }

    private void UpdatePlacar()
    {
        RemoveAllChild(localPanelKill);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            Text txtPlayer = CreateText(localPanelKill.transform, player.NickName);
            txtPlayer.text = player.NickName + " " + ScoreExtensions.GetScore(player);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ChangeNumberKill("");
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ChangeNumberKill("");
        }
    }
}
