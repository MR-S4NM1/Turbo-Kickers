using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviour
{
    PhotonView m_pv;

    void Start()
    {
        m_pv = GetComponent<PhotonView>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RedGoal"))
        {
            UpdateRedScore();
        }
        else if (other.gameObject.CompareTag("BlueGoal"))
        {
            UpdateBlueScore();
        }
    }

    void UpdateRedScore()
    {
        LevelManager.instance.updateRedScore();
    }

    void UpdateBlueScore()
    {
        LevelManager.instance.updateBlueScore();
    }

}
