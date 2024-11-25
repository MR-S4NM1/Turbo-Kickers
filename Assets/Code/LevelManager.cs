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

public class LevelManager : MonoBehaviourPunCallbacks, IOnEventCallback
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

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// Levanta el Evento cuando los jugadores esten listos para la partida
    /// </summary>
    void setNewRoleEvent()
    {
        byte m_ID = 1;//Codigo del Evento (1...199)
        object content = "Asignacion de nuevo rol...";
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
        assignRole();
        setNewRoleEvent();
    }

    //Falta asignar cuantos roles hay segun la cantidad de jugadores
    void assignRole(){
        print("Se crea Hastable con la asignacion del nuevo rol");
        Player[] m_playersArray = PhotonNetwork.PlayerList;
        //GameplayRole[] m_gameplayRole = { GameplayRole.Innocent, GameplayRole.Traitor };
        //List<GameplayRole> m_gameplayRole = new List<GameplayRole>();

        //Solución Roberto
        //if (m_playersArray.Length <= 4)
        //{
        //    m_gameplayRole.Add(GameplayRole.Traitor);
        //    m_traitorsLeft = 1;
        //    for (int i = m_gameplayRole.Count; i < m_playersArray.Length; ++i)
        //    {
        //        m_gameplayRole.Add(GameplayRole.Innocent);
        //        m_innocentsLeft++;
        //    }
        //}

        ////m_gameplayRole = m_gameplayRole.OrderBy(x => Random.value).ToArray();
        ////m_gameplayRole = m_gameplayRole.OrderBy(x => Random.value).ToList();

        //for (int i = 0; i < m_playersArray.Length; ++i)
        //{
        //    int index = Random.Range(0, m_gameplayRole.Count);
        //    Hashtable m_playerProperties = new Hashtable();
        //    m_playerProperties["Role"] = m_gameplayRole[index].ToString();
        //    m_gameplayRole.RemoveAt(index);
        //    m_playersArray[i].SetCustomProperties(m_playerProperties);
        //}

        //Solución Carpi y ajustes Mike
        List<GameplayRole> roles = new List<GameplayRole>();

        int totalPlayers = m_playersArray.Length;
        int traitorCount = Mathf.Max(1, Mathf.RoundToInt(totalPlayers * m_traitorPercentage));
        int innocentCount = totalPlayers - traitorCount;

        roles.AddRange(Enumerable.Repeat(GameplayRole.Traitor, traitorCount));
        roles.AddRange(Enumerable.Repeat(GameplayRole.Innocent, innocentCount));

        for(int i = 0; i < traitorCount; ++i)
        {
            m_traitorsLeft++;
        }

        for (int i = 0; i < innocentCount; ++i)
        {
            m_innocentsLeft++;
        }

        //m_traitorsLeft = traitorCount;
        //m_innocentsLeft = innocentCount;

        //shuffleRolesList(roles);

        for(int i = 0; i < m_playersArray.Length; i++)
        {
            int randIndex = Random.Range(0, roles.Count);
            Hashtable m_playerProperties = new Hashtable();
            m_playerProperties["Role"] = roles[randIndex].ToString();
            m_playersArray[i].SetCustomProperties(m_playerProperties);
            roles.RemoveAt(randIndex);
        }

        //Solución 1 Mike
        //List<GameplayRole> m_gameplayRolesLeft = new List<GameplayRole>(); //Creamos una lista con los roles, los cuales iremos eliminando.

        /*for(int i = 0; i < m_gameplayRole.Length; ++i)
        {
            m_gameplayRolesLeft.Add(m_gameplayRole[i]); //Llenamos la lista con los roles originales.
        }*/

        //for (int i = 0; i < m_gameplayRole.Length / 2; ++i)
        //{
        //    m_gameplayRolesLeft.Add(m_gameplayRole[0]);
        //}
        //for (int i = m_gameplayRole.Length / 2; i < m_gameplayRole.Length; ++i)
        //{
        //    m_gameplayRolesLeft.Add(m_gameplayRole[1]); //Llenamos la lista con los roles originales.
        //}

        /*m_gameplayRole = m_gameplayRole.OrderBy(x => Random.value).ToArray();
        m_gameplayRolesLeft = m_gameplayRole.OrderBy(x => Random.value).ToList(); //Revolvemos los elementos de la lista para tener aletoriedad.*/

        //m_playersArray = m_playersArray.OrderBy(x => Random.value).ToArray();

        //for (int i = 0; i < m_playersArray.Length; i++)
        //{
        //    //Si el arreglo de jugadores es más chico que el de roles o si el iterador ya está en la diferencia entre número de jugadores y roles restantes,
        //    //entonces que ya elija un rol aleatorio, lo asigne y luego vaya eliminando los elementos para que no se repitan.
        //    if ((m_playersArray.Length <= m_gameplayRole.Length) || (i == (m_playersArray.Length - m_gameplayRole.Length)))
        //    {
        //        print(m_gameplayRolesLeft[0].ToString());
        //        Hashtable m_playerProperties = new Hashtable();
        //        m_playerProperties["Role"] = m_gameplayRolesLeft[/*i % m_gameplayRolesLeft.Count*/ Random.Range(0, m_gameplayRolesLeft.Count - 1)].ToString();
        //        m_playersArray[i].SetCustomProperties(m_playerProperties);
        //        m_gameplayRolesLeft.Remove(m_gameplayRolesLeft[0]);
        //    }
        //    else
        //    {
        //        //Si no se cumple lo anterior, que la computadora decida entonces sin limitaciones.
        //        Hashtable m_playerProperties = new Hashtable();
        //        m_playerProperties["Role"] = m_gameplayRole[/*i % m_gameplayRole.Length*/ Random.Range(0, m_gameplayRole.Length)].ToString();

        //        m_playersArray[i].SetCustomProperties(m_playerProperties);
        //    }
        //}

        //Solución 2 Mike
        //for (int i = 0; i < m_playersArray.Length / 2; ++i)
        //{
        //    Hashtable m_playerProperties = new Hashtable();
        //    m_playerProperties["Role"] = m_gameplayRolesLeft[0].ToString();
        //    m_playersArray[i].SetCustomProperties(m_playerProperties);
        //}
        //for (int i = m_playersArray.Length / 2; i < m_playersArray.Length; ++i)
        //{
        //    Hashtable m_playerProperties = new Hashtable();
        //    m_playerProperties["Role"] = m_gameplayRolesLeft[1].ToString();
        //    m_playersArray[i].SetCustomProperties(m_playerProperties);
        //}

        //print(m_gameplayRolesLeft.Count); //Para este punto será cero, ya que ya se han asignado todos los roles y, por ende, eliminado todos de la lista :P
    }

    //Fisher-Yates Shuffle
    void shuffleRolesList(List<GameplayRole> p_rolesList)
    {
        for (int i = p_rolesList.Count - 1; i > 0; i--){
            int j = UnityEngine.Random.Range(0, i + 1);
            GameplayRole temp_role = p_rolesList[j];
            p_rolesList[i] = p_rolesList[j];
            p_rolesList[i] = temp_role;
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
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 4)
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

    //private void OnEnable() {
    //    PhotonNetwork.AddCallbackTarget(this);
    //}

    //private void OnDisable() {
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //}

    //public void OnEvent(EventData photonEvent)
    //{
    //    byte eventCode = photonEvent.Code;
    //    if (eventCode == 1)
    //    {
    //        string data = (string)photonEvent.CustomData;
    //        //getNewGameplayRole();
    //        //Hacer algo con el string
    //    }
    //}

    void retireTraitorWhoHasDied()
    {
        m_traitorsLeft--;

        if (m_traitorsLeft == 0)
        {
            setLevelManagerSate(LevelManagerState.Finishing);
            m_photonView.RPC("WinnersInfo", RpcTarget.All, "Ganaron los inocentes", Color.blue);
        }
    }

    void retireInnocentWhoHasDied()
    {
        m_innocentsLeft--;

        if (m_innocentsLeft == 0)
        {
            setLevelManagerSate(LevelManagerState.Finishing);
            m_photonView.RPC("WinnersInfo", RpcTarget.All, "Ganaron los traidores", Color.red);
        }
    }

    [PunRPC]
    void WinnersInfo(string p_winners, Color p_winnersColor)
    {
        m_gameInfo.text = "";
        m_Winnerstext.text = p_winners;
        m_Winnerstext.color = p_winnersColor;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case 1:
                break;
            case 2: // Evento de jugador que ha muerto.
                retireTraitorWhoHasDied();
                break;
            case 3:
                retireInnocentWhoHasDied();
                break;
        }

        /*if (eventCode == 1)
        {
            string data = (string)photonEvent.CustomData;
            //Hacer algo con el string

            GetNewGameplayRole();
        }*/
    }

    public void getNewInfoGame(string p_playerInfo)
    {
        m_photonView.RPC("showNewGameInfo", RpcTarget.All, p_playerInfo);
    }

    [PunRPC]
    void showNewGameInfo(string p_name)
    {
        m_gameInfo.text = "El jugador: " + p_name + " ha quedado eliminado";
    }
}
public enum LevelManagerState
{
    None,
    Waiting,
    Playing,
    Finishing
}


public enum GameplayRole
{
    Innocent,
    Traitor
}
