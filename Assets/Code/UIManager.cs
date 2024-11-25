using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class UIManager : MonoBehaviour
{
    /// Date: 11/09/2024
    /// Author: Miguel Angel Garcia Elizalde y Alan Elias Carpinteyro Gastelum.
    /// Brief: Código del la interfaz de usuario (UI).

    public static UIManager Instance;

    PhotonView m_PV;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    private void Start()
    {
        m_PV = GetComponent<PhotonView>();
        //m_TimerText.text = "Time to start: " + remainingTime.ToString("0");
    }

    public void leaveCurrentRoomFromEditor()
    {
        LevelNetworkManager.Instance.disconnectFromCurrentRoom();
    }
}