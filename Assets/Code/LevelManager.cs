using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public static LevelManager instance;

    [SerializeField] TextMeshProUGUI m_gameInfo;

    [Range(0.1f, 0.2f)][SerializeField] float m_traitorPercentage;

    [SerializeField] int m_traitorsLeft;
    [SerializeField] int m_innocentsLeft;

    [SerializeField] GameObject m_victoryPanel;
    [SerializeField] TextMeshProUGUI m_Winnerstext;
    [SerializeField] GameObject m_exitButton;

    PhotonView m_photonView;
    LevelManagerState m_currentState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    void Start()
    {
        m_photonView = GetComponent<PhotonView>();

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    print("Soy el master client");
        //}

        m_victoryPanel.SetActive(false);
        m_exitButton.SetActive(false);
        setLevelManagerSate(LevelManagerState.Waiting);
    }

    /// <summary>
    /// Levanta el Evento cuando los jugadores esten listos para la partida
    /// </summary>
    void setTypeOfPlayer()
    {
        byte m_ID = 1; //Codigo del Evento (1...199)
        object content = "Asignacion de tipo de jugador";

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public LevelManagerState CurrentState { get { return m_currentState; } }
    public LevelManagerState getLevelManagerSate()
    {
        return m_currentState;
    }

    public void setLevelManagerSate(LevelManagerState p_newState)
    {
        if (p_newState == m_currentState)
        {
            return;
        }
        m_currentState = p_newState;

        switch (m_currentState)
        {
            case LevelManagerState.None:
                break;
            case LevelManagerState.Waiting:
                break;
            case LevelManagerState.Playing:
                playing();
                break;
            case LevelManagerState.Finishing:
                m_photonView.RPC("activateExitButton", RpcTarget.All);
                break;
        }
    }
    /// <summary>
    /// Inicializa el estado de Playing
    /// </summary>
    void playing()
    {
        assignTypeOfPlayer();
        setTypeOfPlayer();
    }

    //Falta asignar cuantos roles hay segun la cantidad de jugadores
    void assignTypeOfPlayer(){
        print("Se crea Hastable con la asignacion del nuevo rol");
        Player[] m_playersArray = PhotonNetwork.PlayerList;
        //List<GameplayRole> roles = new List<GameplayRole>();

        //for(int i = 0; i < m_playersArray.Length; i++)
        //{
        //    int randIndex = Random.Range(0, roles.Count);
        //    Hashtable m_playerProperties = new Hashtable();
        //    m_playerProperties["Role"] = roles[randIndex].ToString();
        //    m_playersArray[i].SetCustomProperties(m_playerProperties);
        //    roles.RemoveAt(randIndex);
        //}
    }

    [PunRPC]
    void activateExitButton()
    {
        m_victoryPanel.SetActive(true);
        m_exitButton.SetActive(true);
        Cursor.visible = true;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            StartCoroutine(timerToStart());
        }
    }

    //Probablemente Se necesite RPC
    IEnumerator timerToStart()
    {
        yield return new WaitForSeconds(3);
        setLevelManagerSate(LevelManagerState.Playing);
    }

    [PunRPC]
    void WinnersInfo(string p_winners, Color p_winnersColor)
    {
        m_gameInfo.text = "";
        m_Winnerstext.text = p_winners;
        m_Winnerstext.color = p_winnersColor;
    }

    public void getNewInfoGame(string p_playerInfo)
    {
        m_photonView.RPC("showNewGameInfo", RpcTarget.All, p_playerInfo);
    }
}
public enum LevelManagerState
{
    None,
    Waiting,
    Playing,
    Finishing
}


public enum TypeOfPlayer
{
    Blue,
    Red,
    Green,
    Yellow
}
