using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    #region References
    [SerializeField] protected TextMeshProUGUI m_roomName;
    PhotonConnection m_photonConnectionManager;
    #endregion

    #region UnityMethods

    private void Start()
    {
        m_photonConnectionManager = FindAnyObjectByType<PhotonConnection>();
    }

    #endregion

    #region PublicMethods

    public void setNewRoomName(string p_roomName)
    {
        m_roomName.text = p_roomName;
    }

    #endregion

}
