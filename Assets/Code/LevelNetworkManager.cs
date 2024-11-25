using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LevelNetworkManager : MonoBehaviourPunCallbacks
{
    /// Date: 14/09/2024
    /// Author: Miguel Angel Garcia Elizalde y Alan Elias Carpinteyro Gastelum.
    /// Brief: Código del la interfaz de usuario (UI).

    #region Knobs

    #endregion

    #region RuntimeVariables

    public static LevelNetworkManager Instance;

    PhotonView m_PV;

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            m_PV = GetComponent<PhotonView>();
        }
        else
        {
            Destroy(Instance);
        }
    }

    public void disconnectFromCurrentRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print("Salió el player: " + otherPlayer.NickName);
        //PlayersWinnerManager.Instance.PlayersCount--;
    }
}
