using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SpawnerManager : MonoBehaviour
{
    PhotonView m_PV;

    private void Start()
    {
        m_PV = GetComponent<PhotonView>();

        if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("playerType", out object playerType))
        {
            string playerTypeFromHashTable = (string)playerType;

            switch (playerTypeFromHashTable)
            {
                case "Vexa_A":
                    PhotonNetwork.Instantiate("Vefects_Vexa_A", new Vector3(Random.Range(-15, 15), 0.0f, Random.Range(-12, 12)), Quaternion.identity);
                    break;
                case "Vexa_B":
                    PhotonNetwork.Instantiate("Vefects_Vexa_B", new Vector3(Random.Range(-15, 15), 0.0f, Random.Range(-12, 12)), Quaternion.identity);
                    break;
                case "Vexa_C":
                    PhotonNetwork.Instantiate("Vefects_Vexa_C", new Vector3(Random.Range(-15, 15), 0.0f, Random.Range(-12, 12)), Quaternion.identity);
                    break;
                case "Vexa_D":
                    PhotonNetwork.Instantiate("Vefects_Vexa_D", new Vector3(Random.Range(-15, 15), 0.0f, Random.Range(-12, 12)), Quaternion.identity);
                    break;
            }
        }   
    }
}
