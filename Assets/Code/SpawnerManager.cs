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

        if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("playerType", out object playerType))
        {
            string playerTypeFromHashTable = (string)playerType;

            int posNum = Random.Range(0, m_spawner.childCount);

            switch (playerTypeFromHashTable)
            {
                case "Vexa_A":
                    PhotonNetwork.Instantiate("Vefects_Vexa_A", m_spawner.GetChild(posNum).position, Quaternion.identity);
                    break;
                case "Vexa_B":
                    PhotonNetwork.Instantiate("Vefects_Vexa_B", m_spawner.GetChild(posNum).position, Quaternion.identity);
                    break;
                case "Vexa_C":
                    PhotonNetwork.Instantiate("Vefects_Vexa_C", m_spawner.GetChild(posNum).position, Quaternion.identity);
                    break;
                case "Vexa_D":
                    PhotonNetwork.Instantiate("Vefects_Vexa_D", m_spawner.GetChild(posNum).position, Quaternion.identity);
                    break;
            }
        }   
    }
}
