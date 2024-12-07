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

    public static UIManager instance;

    PhotonView m_PV;

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