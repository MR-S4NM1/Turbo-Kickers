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
    /// Brief: C�digo para conectar a cuartos y servidores de Photon.

    [Header("Room Info")]

    //[SerializeField] TMP_InputField m_roomInputField;
    [SerializeField] TMP_InputField m_newNickname;
    [SerializeField] TMP_InputField m_newRoomName;
    [SerializeField] TMP_InputField m_newNumberOfPlayer;

    //[SerializeField] TextMeshProUGUI m_joinRoomFailedTextMeshProUGUI;
    //[SerializeField] TextMeshProUGUI m_createRoomFailedTextMeshProUGUI;
    [SerializeField] TextMeshProUGUI m_nicknameFailedTextMeshProUGUI;
    [SerializeField] TextMeshProUGUI m_creditsTextMeshProUGUI;

    [SerializeField] Button m_playButton;
    [SerializeField] Button m_exitButton;
    [SerializeField] Button m_backButton;
    [SerializeField] Button m_vsModeButton;
    [SerializeField] Button m_coopModeButton;

    [SerializeField] GameObject m_menuModePanel;
    [SerializeField] GameObject m_selectModePanel;

    [SerializeField] GameObject m_loadingPanel;
    [SerializeField] GameObject m_menuPanel;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
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
        m_menuPanel.SetActive(true);
        Cursor.visible = true;
    }

    public override void OnJoinedRoom()
    {
        print("Se entr� al room");
        PhotonNetwork.LoadLevel("Gameplay");
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

    RoomOptions NewRoomInfo()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
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
            PhotonNetwork.JoinRoom(m_newRoomName.text);
        }
    }

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

        if (m_newRoomName.text == "")
        {
            //m_createRoomFailedTextMeshProUGUI.text = "Este espacio no lo puedes dejar en blanco.";
            //m_createRoomFailedTextMeshProUGUI.gameObject.SetActive(true);
        }
        else
        {
            PhotonNetwork.CreateRoom(m_newRoomName.text, NewRoomInfo(), null);
        }
    }

    public void PlayButton()
    {
        if(m_newNickname.text != "")
        {
            m_nicknameFailedTextMeshProUGUI.gameObject.SetActive(false);

            m_menuModePanel.SetActive(false);
            m_selectModePanel.SetActive(true);

            m_newRoomName.gameObject.SetActive(false);
            m_newNickname.gameObject.SetActive(false);

            m_playButton.gameObject.SetActive(false);
            m_exitButton.gameObject.SetActive(false);
            m_backButton.gameObject.SetActive(true);
            m_vsModeButton.gameObject.SetActive(true);
            m_creditsTextMeshProUGUI.gameObject.SetActive(true);
        }
        else
        {
            m_nicknameFailedTextMeshProUGUI.gameObject.SetActive(true);
            m_nicknameFailedTextMeshProUGUI.text = "Introduce un nombre, no lo dejes en blanco.";
            return;
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        m_creditsTextMeshProUGUI.gameObject.SetActive(false);
        m_menuModePanel.SetActive(true);
        m_selectModePanel.SetActive(false);

        m_newRoomName.gameObject.SetActive(true);
        m_newNickname.gameObject.SetActive(true);

        m_playButton.gameObject.SetActive(true);
        m_exitButton.gameObject.SetActive(true);
        m_backButton.gameObject.SetActive(false);
        m_vsModeButton.gameObject.SetActive(false);
    }

    void setNewColorPlayer(Color p_newColor)
    {
        gameObject.GetComponent<SpriteRenderer>().color = p_newColor;
        Hashtable playerProperties = new Hashtable();
        playerProperties["playerColor"] = p_newColor;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    void DamageOtherPlayer(Player p_otherPlayer)
    {
        Hashtable playerStats = new Hashtable();
        playerStats["damage"] = 1;
        p_otherPlayer.SetCustomProperties(playerStats);
    }
}
