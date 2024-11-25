using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DisableObject : MonoBehaviour
{
    /// Date: 14/09/2024
    /// Author: Alan Elias Carpinteyro Gastelum.
    /// Brief: Código para eliminar todo lo que no sea del jugador, de modo que no afecte a los demás jugadores.

    PhotonView m_PV;

    [SerializeField] List<GameObject> m_listGameObj;
    [SerializeField] List<MonoBehaviour> m_listScripts;
    void Start()
    {
        m_PV = GetComponent<PhotonView>();
        if (!m_PV.IsMine)
        {
            disableObjects();
            disableScripts();
        }
    }

    void disableObjects()
    {
        foreach (GameObject obj in m_listGameObj)
        {
            Destroy(obj);
        }
    }

    void disableScripts()
    {
        foreach (MonoBehaviour scripts in m_listScripts)
        {
            Destroy(scripts);
        }
    }
}
