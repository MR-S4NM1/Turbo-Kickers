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
using Cinemachine;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public static LevelManager instance;

    [Header("UI")]
    [SerializeField] protected GameObject m_victoryPanel;
    [SerializeField] protected TextMeshProUGUI m_winnerstext;
    [SerializeField] protected GameObject m_exitButton;
    [SerializeField] protected TextMeshProUGUI m_gameInfo;
    [SerializeField] protected TextMeshProUGUI m_blueTeamScoreTMP;
    [SerializeField] protected TextMeshProUGUI m_redTeamScoreTMP;
    [SerializeField] protected TextMeshProUGUI m_firstTimerTimeTMP;
    [SerializeField] protected TextMeshProUGUI m_secondTimerTimeTMP;
    [SerializeField] protected List<TextMeshProUGUI> m_playersNamesList;
    [SerializeField] protected GameObject m_playersListPanel;
    [SerializeField] protected GameObject m_gamePanel;
    
    [Header("Score")]
    [SerializeField] protected int m_redTeamScore;
    [SerializeField] protected int m_blueTeamScore;

    [Header("Time")]
    [SerializeField] protected int m_firstTimerTime;
    [SerializeField] protected int m_secondTimerTime;

    [Header("Cinemachine")]
    [SerializeField] protected CinemachineTargetGroup targetGroup;
    [SerializeField] protected CinemachineImpulseSource m_impulseSource;

    [Header("Ball")]
    [SerializeField] protected Ball m_ball;

    [Header("Players")]
    [SerializeField] public bool m_playersCanMove;

    [Header("Particle Systems")]
    [SerializeField] ParticleSystem m_redParticleSystem;
    [SerializeField] ParticleSystem m_blueParticleSystem;

    PhotonView m_photonView;
    LevelManagerState m_currentState;

    public Team m_typeOfPlayer;

    //private void Awake()
    //{
    //    if (instance != null && instance != this)
    //    {
    //        Destroy(instance);
    //    }
    //    else
    //    {
    //        instance = this;
    //    }
    //}

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

        m_blueParticleSystem.gameObject.SetActive(false);
        m_redParticleSystem.gameObject.SetActive(false);

        m_victoryPanel.SetActive(false);
        m_gamePanel.SetActive(true);
        m_playersListPanel.SetActive(false);
        m_exitButton.SetActive(false);
        setLevelManagerSate(LevelManagerState.Waiting);

        m_redTeamScoreTMP.text = "Red: " + 0;
        m_redTeamScoreTMP.color = Color.red;
        m_blueTeamScoreTMP.text = "Blue: " + 0;
        m_blueTeamScoreTMP.color = Color.blue;

        m_photonView.RPC("activateOrDeactivateBall", RpcTarget.AllBuffered, false);
    }

    /// <summary>
    /// Levanta el Evento cuando los jugadores esten listos para la partida
    /// </summary>
    protected void setTypeOfPlayer()
    {
        byte m_ID = 1; //Codigo del Evento (1...199)
        object content = "Team where the player will be.";

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
    }

    protected void vibrationOfTheCamera()
    {
        byte m_ID = 2; //Codigo del Evento (1...199)

        object content = "Impulse.";

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
                checkWinnersAndUpdateUI();
                break;
        }
    }
    /// <summary>
    /// Inicializa el estado de Playing
    /// </summary>
    protected void playing()
    {
        assignTeamOfPlayer();
        setTypeOfPlayer();
        m_photonView.RPC("activateOrDeactivateBall", RpcTarget.AllBuffered, true);
        m_photonView.RPC("updateCurrentPlayers", RpcTarget.AllBuffered);
        StartCoroutine(timeToFinishGame());
    }

    protected void assignTeamOfPlayer()
    {
        print("Se crea Hastable con la asignacion del equipo");
        Player[] m_playersArray = PhotonNetwork.PlayerList;
        List<Team> teams = new List<Team>();

        int totalPlayers = m_playersArray.Length;
        int redTeamCount = Mathf.Max(1, Mathf.RoundToInt(totalPlayers * 0.5f));
        int blueTeamCount = totalPlayers - redTeamCount;

        teams.AddRange(Enumerable.Repeat(Team.Red, redTeamCount));
        teams.AddRange(Enumerable.Repeat(Team.Blue, blueTeamCount));

        teams = teams.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < m_playersArray.Length; i++)
        {
            Hashtable m_playerProperties = new Hashtable();
            m_playerProperties["Team"] = teams[i].ToString();
            m_playersArray[i].SetCustomProperties(m_playerProperties);
        }
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

    protected IEnumerator timerToStart()
    {
        while(m_firstTimerTime > 0)
        {
            print("WE CAN START!");
            yield return new WaitForSeconds(1);
            m_photonView.RPC("updateFirstTimer", RpcTarget.AllBuffered);
        }
        //m_photonView.RPC("deactivateFirstTimer", RpcTarget.All);
        setLevelManagerSate(LevelManagerState.Playing);
    }

    protected IEnumerator timeToFinishGame()
    {
        while (m_secondTimerTime > 0)
        {
            yield return new WaitForSeconds(1);
            m_photonView.RPC("updateSecondTimer", RpcTarget.AllBuffered);
        }
        //m_photonView.RPC("deactivateSecondTimer", RpcTarget.All);
        setLevelManagerSate(LevelManagerState.Finishing);
    }

    [PunRPC]
    protected void updateFirstTimer()
    {
        m_firstTimerTime -= 1;
        m_firstTimerTimeTMP.text = "Time: " + m_firstTimerTime.ToString();
    }

    [PunRPC]
    protected void deactivateFirstTimer()
    {
        m_firstTimerTimeTMP.gameObject.SetActive(false);
    }


    [PunRPC]
    protected void updateSecondTimer()
    {
        m_secondTimerTime -= 1;
        m_secondTimerTimeTMP.text = "Time: " + m_secondTimerTime.ToString();
    }

    [PunRPC]
    protected void deactivateSecondTimer()
    {
        m_secondTimerTimeTMP.gameObject.SetActive(false);
    }

    [PunRPC]
    protected void winnersInfo(string p_winners, Color p_winnersColor)
    {
        m_winnerstext.text = p_winners;
        m_winnerstext.color = p_winnersColor;
        print(m_winnerstext.text);
    }

    protected void checkWinnersAndUpdateUI()
    {
        if(m_blueTeamScore > m_redTeamScore)
        {
            m_photonView.RPC("winnersInfo", RpcTarget.All, "Blue team has won!", Color.blue);
        }
        else if(m_redTeamScore > m_blueTeamScore)
        {
            m_photonView.RPC("winnersInfo", RpcTarget.All, "Red team has won!", Color.red);
        }
        else
        {
            m_photonView.RPC("winnersInfo", RpcTarget.All, "Tie!", Color.white);
        }

        m_victoryPanel.SetActive(true);
        m_exitButton.SetActive(true);
        Cursor.visible = true;
    }

    public void getNewInfoGame(string p_playerInfo)
    {
        m_photonView.RPC("showFirstTimerInfo", RpcTarget.All, p_playerInfo);
    }
    IEnumerator WaitForRedParticleSystem()
    {
        m_redParticleSystem.gameObject.SetActive(true);
        m_redParticleSystem.Play();
        yield return new WaitForSeconds(m_redParticleSystem.main.duration);
        m_redParticleSystem.gameObject.SetActive(false);
    }

    IEnumerator WaitForBlueParticleSystem()
    {
        m_blueParticleSystem.gameObject.SetActive(true);
        m_blueParticleSystem.Play();
        yield return new WaitForSeconds(m_blueParticleSystem.main.duration);
        m_blueParticleSystem.gameObject.SetActive(false);
    }

    public void updateRedScore()
    {
        m_photonView.RPC("updateRedTeamScore", RpcTarget.All);
    }

    [PunRPC]
    protected void updateRedTeamScore()
    {
        m_redTeamScore += 1;
        m_redTeamScoreTMP.text = "Red: " + m_redTeamScore.ToString();
        vibrationOfTheCamera();
        StartCoroutine(WaitForRedParticleSystem());
        m_ball.gameObject.transform.position = new Vector3(0.0f, 0.8f, -0.35f);
        m_ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        m_ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        m_ball.m_hasEnteredTheRedGoal = false;
    } 

    public void updateBlueScore()
    {
        m_photonView.RPC("updateBlueTeamScore", RpcTarget.All);
    }

    [PunRPC]
    protected void updateBlueTeamScore()
    {
        m_blueTeamScore += 1;
        m_blueTeamScoreTMP.text = "Blue: " + m_blueTeamScore.ToString();
        vibrationOfTheCamera();
        StartCoroutine(WaitForBlueParticleSystem());
        m_ball.gameObject.transform.position = new Vector3(0.0f, 0.8f, -0.35f);
        m_ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        m_ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        m_ball.m_hasEnteredTheBlueGoal = false;
    }

    [PunRPC]
    protected void activateOrDeactivateBall(bool isActivated)
    {
        m_playersCanMove = isActivated;
        m_ball.gameObject.SetActive(isActivated);
    }

    [PunRPC]
    void updateCurrentPlayers()
    {
        int index = 0;
        Dictionary<int, Photon.Realtime.Player> playersList = Photon.Pun.PhotonNetwork.CurrentRoom.Players;
        foreach (KeyValuePair<int, Photon.Realtime.Player> player in playersList)
        {
            if(index < playersList.Count)
            {
                print(player.Value.NickName);
                m_playersNamesList[index].text = player.Value.NickName;
                m_playersNamesList[index].gameObject.transform.parent.GetComponent<Image>().enabled = true;
                index++;
            }
            else
            {
                m_playersNamesList[index].text = string.Empty;
            }
        }
    }

    public void showPlayersList(bool isShowingList)
    {
        if(isShowingList)
        {
            m_gamePanel.gameObject.SetActive(false);
            m_playersListPanel.gameObject.SetActive(true);
        }
        else
        {
            m_gamePanel.gameObject.SetActive(true);
            m_playersListPanel.gameObject.SetActive(false);
        }
    }


    //public void AddPlayerToCinemachineTargetGroup(Transform playerTransform)
    //{
    //    print(playerTransform.gameObject.name);
    //    m_photonView.RPC("addPlayerToCMTG", RpcTarget.AllBuffered, playerTransform);
    //}

    //[PunRPC]
    //protected void addPlayerToCMTG(Transform playerTransform)
    //{
    //    print(playerTransform.gameObject.name);
    //    targetGroup.AddMember(playerTransform, 1, 0);
    //}

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
