using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] Transform m_spawner;

    PhotonView m_PV;

    


    private void Start()
    {
        m_PV = GetComponent<PhotonView>();

        //if (m_PV.IsMine)
        //{
        //    PhotonNetwork.Instantiate("LevelManager", transform.position, Quaternion.identity);
        //}

        int posNum = Random.Range(0, m_spawner.childCount);

        PhotonNetwork.Instantiate("Player", m_spawner.GetChild(posNum).position, Quaternion.identity);
    }
}
