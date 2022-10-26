// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to handle typical game management requirements
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine.UI;


public class GameManager : MonoBehaviourPunCallbacks
{

    [Header("Login Panel")]
    public GameObject MainPanel;
    public InputField PlayerNameInput;

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    public Transform[] spawnerList = new Transform[10];

    [SerializeField]
    TMP_Text textTempo;

    [SerializeField]
    private float tempoDoJogo;
    private float tempoAtual;

    byte gameCode = 2;
    byte endGameCode = 3;
    byte timeCode = 4;
    bool startGame = false;
    bool endGame = false;
    bool jogoEstaRodando = false;

    #region MonoBehaviour CallBacks

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("AWAKE");
    }

    void Start()
    {
        Debug.Log("Start");
    }

    void Update()
    {
        if (jogoEstaRodando)
        {
            Temporizador();
        }
    }

    void Temporizador()
    {

        if (tempoAtual > 0)
            tempoAtual -= Time.deltaTime;
        else
        {
            tempoAtual = 0;
            EndGame();
        }

        float minutos = Mathf.FloorToInt(tempoAtual / 60);
        float segundos = Mathf.FloorToInt(tempoAtual % 60);
        textTempo.text = string.Format("{0:00} : {1:00}", minutos, segundos);
    }

    void StartGame()
    {
        tempoAtual = tempoDoJogo;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(gameCode, true, raiseEventOptions, SendOptions.SendReliable);
    }

    void EndGame()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(gameCode, false, raiseEventOptions, SendOptions.SendReliable);
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
        if (eventCode == gameCode)
        {
            jogoEstaRodando = (bool)photonEvent.CustomData;

            if (jogoEstaRodando == false)
                showResult();

        }
        else if (eventCode == timeCode)
        {
            tempoAtual = (float)photonEvent.CustomData;

            if (tempoAtual < tempoDoJogo && tempoAtual > 0)
            {
                jogoEstaRodando = true;
            }
        }
    }

    void showResult()
    {
        Player playerVencedor = PhotonNetwork.LocalPlayer;
        double score = ScoreExtensions.GetScore(playerVencedor);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            double scorePlayer = ScoreExtensions.GetScore(player);

            if (scorePlayer > score)
            {
                playerVencedor = player;
                score = scorePlayer;
            }
        }

        textTempo.text = playerVencedor.NickName + " foi o vencedor";
    }

    #endregion

    public void OnLoginButtonClicked()
    {
        Debug.Log("Botao clicado");
        string playerName = PlayerNameInput.text != ""
            ? PlayerNameInput.text
            : "Player " + Random.Range(1000, 10000);

        PhotonNetwork.LocalPlayer.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
    }

    #region PUN CALLBACKS

    public override void OnConnectedToMaster()
    {

        Debug.Log("conectado ao server");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Entrou no Lobby");
        PhotonNetwork.JoinRoom("Afya");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {

        Debug.Log("errou");
        if (returnCode == ErrorCode.GameDoesNotExist)
        {
            RoomOptions sala = new RoomOptions { MaxPlayers = 20 };
            PhotonNetwork.CreateRoom("Afya", sala, null);
            Debug.Log("Criando sala");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Iniciando o jogo");
        if (PhotonNetwork.IsConnected)
        {
            var playerCount = PhotonNetwork.CountOfPlayersInRooms;
            PhotonNetwork.Instantiate(playerPrefab.name, spawnerList[playerCount].position, spawnerList[playerCount].rotation, 0);
            if (PhotonNetwork.IsMasterClient)
            {
                StartGame();
            }

            MainPanel.SetActive(false);
        }

        //PhotonNetwork.LoadLevel("Game");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " entrou");

        if (PhotonNetwork.IsMasterClient)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(timeCode, tempoAtual, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("erxxxc");
    }

    #endregion
}
