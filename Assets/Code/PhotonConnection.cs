using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class PhotonConnection : MonoBehaviourPunCallbacks
{
    /// Date: 11/09/2024
    /// Author: Miguel Angel Garcia Elizalde y Alan Elias Carpinteyro Gastelum.
    /// Brief: Código para conectar a cuartos y servidores de Photon.

    [Header("Room Info")]
    [SerializeField] TMP_InputField m_newNickname;
    [SerializeField] TMP_InputField m_newRoomName;
    [SerializeField] TMP_InputField m_newNumberOfPlayer;

    [Header("All error messages")]
    [SerializeField] TextMeshProUGUI m_joinRoomFailedTextMeshProUGUI;
    [SerializeField] TextMeshProUGUI m_createRoomFailedTextMeshProUGUI;
    [SerializeField] TextMeshProUGUI m_nicknameFailedTextMeshProUGUI;
    [SerializeField] TextMeshProUGUI m_creditsTextMeshProUGUI;

    [Header("All buttons")]
    [SerializeField] Button m_playButton;
    [SerializeField] Button m_exitButton;
    [SerializeField] Button m_backButton;
    [SerializeField] Button m_vsModeButton;

    [Header("All main menus subpanels")]
    [SerializeField] GameObject m_titlePanel;
    [SerializeField] GameObject m_setupPanel;

    [Header("All major panels")]
    [SerializeField] GameObject m_loadingPanel;
    [SerializeField] GameObject m_mainMenuPanel;

    [Header("Type of player")]
    public TypeOfPlayerMenu m_typeOfPlayerMenu;

    protected bool userHasChosenPlayerType;


    [SerializeField] protected RoomItem m_roomItemButton;
    protected List<RoomItem> m_roomItemsList;
    [SerializeField] protected Transform m_contentObject;


    void Start()
    {
        m_roomItemsList = new List<RoomItem>();
        PhotonNetwork.ConnectUsingSettings();
        userHasChosenPlayerType = false;
    }

    public override void OnConnectedToMaster()
    {
        print("Se ha conectado al Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("Se ha entrado al Lobby Abstracto");
        m_loadingPanel.SetActive(false);
        m_mainMenuPanel.SetActive(true);
        m_titlePanel.SetActive(true);
        Cursor.visible = true;
        userHasChosenPlayerType = false;
    }

    public override void OnJoinedRoom()
    {
        print("Se entró al room");
        PhotonNetwork.LoadLevel("Gameplay");
        userHasChosenPlayerType = false;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        /*print("Hubo un error al crear el room: " + message);
        m_createRoomFailedTextMeshProUGUI.gameObject.SetActive(true);
        m_createRoomFailedTextMeshProUGUI.text = "Hubo un error al crear el room: " + m_roomInputField.text;*/
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Hubo un error al entrar el room: " + message);
        /*m_joinRoomFailedTextMeshProUGUI.gameObject.SetActive(true);
        m_joinRoomFailedTextMeshProUGUI.text = "Hubo un error al entrar al room: " + m_roomInputField.text;*/
    }

    RoomOptions NewRoomInfo(int p_maxPlayersInThisRoom)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = p_maxPlayersInThisRoom;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        return roomOptions;
    }

    public void joinRoom()
    {
        if (m_newNickname.text == "")
        {
            m_nicknameFailedTextMeshProUGUI.gameObject.SetActive(true);
            m_nicknameFailedTextMeshProUGUI.text = "Introduce un nombre, no lo dejes en blanco.";
            print("Necesita un nombre.");
            return;
        }
        else
        {
            PhotonNetwork.NickName = m_newNickname.text;
        }

        if (m_newRoomName.text == "")
        {
            //m_joinRoomFailedTextMeshProUGUI.text = "Este espacio no lo puedes dejar en blanco.";
            //m_joinRoomFailedTextMeshProUGUI.gameObject.SetActive(true);
        }
        else
        {
            if (userHasChosenPlayerType)
            {
                PhotonNetwork.JoinRoom(m_newRoomName.text);
            }
        }
    }

    //public void joinRoom(string p_roomName)
    //{
    //    PhotonNetwork.JoinRoom(p_roomName);
    //}

    public void createRoom()
    {
        if (m_newNickname.text == "")
        {
            m_nicknameFailedTextMeshProUGUI.gameObject.SetActive(true);
            m_nicknameFailedTextMeshProUGUI.text = "Introduce un nombre, no lo dejes en blanco.";
            print("Necesita un nombre.");
            return;
        }
        else
        {
            PhotonNetwork.NickName = m_newNickname.text;
        }

        if(int.Parse(m_newNumberOfPlayer.text) % 2 == 1)
        {
            print("Debe ser par");
            return;
        }
        else if((int.Parse(m_newNumberOfPlayer.text) % 2 == 0) && (int.Parse(m_newNumberOfPlayer.text) < 11))
        {
            if (m_newRoomName.text == "")
            {
                //m_createRoomFailedTextMeshProUGUI.text = "Este espacio no lo puedes dejar en blanco.";
                //m_createRoomFailedTextMeshProUGUI.gameObject.SetActive(true);
            }
            else
            {
                if (userHasChosenPlayerType)
                {
                    PhotonNetwork.CreateRoom(m_newRoomName.text, NewRoomInfo(int.Parse(m_newNumberOfPlayer.text)), null);
                }
            }
        }
    }

    public void PlayButton()
    {
        //m_nicknameFailedTextMeshProUGUI.gameObject.SetActive(false);

        m_titlePanel.SetActive(false);
        m_setupPanel.SetActive(true);

        //m_newRoomName.gameObject.SetActive(true);
        //m_newNickname.gameObject.SetActive(true);

        //m_playButton.gameObject.SetActive(false);
        //m_exitButton.gameObject.SetActive(false);
        //m_backButton.gameObject.SetActive(true);
        //m_vsModeButton.gameObject.SetActive(true);
        //m_creditsTextMeshProUGUI.gameObject.SetActive(true);
        userHasChosenPlayerType = false;

        //m_nicknameFailedTextMeshProUGUI.gameObject.SetActive(true);
        //m_nicknameFailedTextMeshProUGUI.text = "Introduce un nombre, no lo dejes en blanco.";
        //return;
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        //m_creditsTextMeshProUGUI.gameObject.SetActive(false);
        m_titlePanel.SetActive(true);
        m_setupPanel.SetActive(false);

        //m_newRoomName.gameObject.SetActive(false);
        //m_newNickname.gameObject.SetActive(false);

        //m_playButton.gameObject.SetActive(true);
        //m_exitButton.gameObject.SetActive(true);
        //m_backButton.gameObject.SetActive(false);
        //m_vsModeButton.gameObject.SetActive(false);
    }

    public void setPlayerA()
    {
        setTypeOfPlayer(TypeOfPlayerMenu.Vexa_A);
        userHasChosenPlayerType = true;
    }

    public void setPlayerB()
    {
        setTypeOfPlayer(TypeOfPlayerMenu.Vexa_B);
        userHasChosenPlayerType = true;
    }

    public void setPlayerC()
    {
        setTypeOfPlayer(TypeOfPlayerMenu.Vexa_C);
        userHasChosenPlayerType = true;
    }

    public void setPlayerD()
    {
        setTypeOfPlayer(TypeOfPlayerMenu.Vexa_D);
        userHasChosenPlayerType = true;
    }

    protected void setTypeOfPlayer(TypeOfPlayerMenu p_typeOfPlayer)
    {
        m_typeOfPlayerMenu = p_typeOfPlayer;
        Hashtable playerProperties = new Hashtable();
        playerProperties["playerType"] = p_typeOfPlayer.ToString();
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomItem roomInfo in m_roomItemsList)
        {
            Destroy(roomInfo.gameObject);
        }
        m_roomItemsList.Clear();

        foreach(RoomInfo roomInfo in roomList)
        {
            if (roomInfo.IsOpen)
            {
                RoomItem m_newButtonRoom = Instantiate(m_roomItemButton, m_contentObject);
                m_newButtonRoom.setNewRoomName(roomInfo.Name, roomInfo.ToStringFull());
                m_roomItemsList.Add(m_newButtonRoom);
            }
        }
    }
}

public enum TypeOfPlayerMenu
{
    Vexa_A,
    Vexa_B,
    Vexa_C,
    Vexa_D
}

