using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviour
{
    #region References
    [SerializeField] protected Rigidbody rb;
    #endregion

    #region RuntimeVariables
    PhotonView m_pv;
    [SerializeField] public bool m_hasEnteredTheRedGoal;
    [SerializeField] public bool m_hasEnteredTheBlueGoal;
    #endregion
    void Start()
    {
        m_pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("RedGoal"))
        {
            if (m_hasEnteredTheRedGoal)
            {
                return;
            }
            UpdateRedScore();
        }
        else if (other.gameObject.CompareTag("BlueGoal"))
        {
            if (m_hasEnteredTheBlueGoal)
            {
                return;
            }
            UpdateBlueScore();
        }

        if (other.gameObject.CompareTag("SideWall"))
        {
            this.gameObject.transform.position = new Vector3(0.0f, 0.8f, -0.35f);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
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
