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

    [SerializeField] int m_redTeamScore;
    [SerializeField] int m_blueTeamScore;

    [SerializeField] GameObject m_victoryPanel;
    [SerializeField] TextMeshProUGUI m_Winnerstext;
    [SerializeField] GameObject m_exitButton;

    PhotonView m_photonView;
    LevelManagerState m_currentState;

    public Team m_typeOfPlayer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
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
        object content = "Asignacion de equipo del jugador";

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
        assignTeamOfPlayer();
        setTypeOfPlayer();
    }

    //Falta asignar cuantos roles hay segun la cantidad de jugadores
    void assignTeamOfPlayer()
    {
        print("Se crea Hastable con la asignacion del equipo");
        Player[] m_playersArray = PhotonNetwork.PlayerList;
        List<Team> teams = new List<Team>();

        int totalPlayers = m_playersArray.Length;
        int redTeamCount = Mathf.Max(1, Mathf.RoundToInt(totalPlayers * 0.5f));
        int blueTeamCount = totalPlayers - redTeamCount;

        teams.AddRange(Enumerable.Repeat(Team.Red, redTeamCount));
        teams.AddRange(Enumerable.Repeat(Team.Blue, blueTeamCount));

        m_playersArray = m_playersArray.OrderBy(x => Random.value).ToArray();

        for (int i = 0; i < m_playersArray.Length; i++)
        {
            if (i % 2 == 0)
            {
                Hashtable m_playerProperties = new Hashtable();
                m_playerProperties["Team"] = teams[0].ToString();
                m_playersArray[i].SetCustomProperties(m_playerProperties);
            }
            else if(i % 2 == 1)
            {
                Hashtable m_playerProperties = new Hashtable();
                m_playerProperties["Team"] = teams[1].ToString();
                m_playersArray[i].SetCustomProperties(m_playerProperties);
            }
        }
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
        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
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

    public void UpdateRedScore()
    {
        m_photonView.RPC("UpdateRedTeamScore", RpcTarget.All);
    }

    [PunRPC]
    void UpdateRedTeamScore()
    {
        m_redTeamScore++;
    } 

    public void UpdateBlueScore()
    {
        m_photonView.RPC("UpdateBlueTeamScore", RpcTarget.All);
    }

    [PunRPC]
    void UpdateBlueTeamScore()
    {

    }

}
public enum LevelManagerState
{
    None,
    Waiting,
    Playing,
    Finishing
}


public enum Team
{
    Blue,
    Red
}
