using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;

public class Panelkill : MonoBehaviourPunCallbacks
{


    // Start is called before the first frame update
    void Start()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            CreateText(transform, player.Value.NickName);
            Debug.Log(player.Value.NickName);
        }

    }

    // Update is called once per frame
    void Update()
    {
        ChecknumberKill();
    }

    [PunRPC]
    static Text CreateText(Transform parent, string namePlayer)
    {
        var go = new GameObject();
        go.name = namePlayer;
        go.transform.parent = parent;
        var text = go.AddComponent<Text>();      
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 60);
        text.text = namePlayer;
        text.fontSize = 50;
        text.color = Color.blue;
        text.alignment = TextAnchor.MiddleCenter;
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = ArialFont;
        return text;
    }

    void ChecknumberKill()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {

            var obj = gameObject.transform.Find(player.NickName);
            if (obj != null)
            {
                obj.GetComponent<Text>().text = player.NickName + " " + ScoreExtensions.GetScore(player);
            }

        }
    }
}
