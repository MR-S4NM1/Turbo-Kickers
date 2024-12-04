using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;
using System.Linq;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPunCallbacks, IOnEventCallback {
    #region References

    [SerializeField] protected Rigidbody rb;
    //[SerializeField] TextMeshPro m_nickname;
    [SerializeField] protected Animator m_myAnim;

    [SerializeField] Transform m_cam;
    [SerializeField] BoxCollider m_boxCollider;
    [SerializeField] GameObject m_triggerCollision;
    [SerializeField] ParticleSystem m_particleSystem;

    [Header("UI Player")]
    [SerializeField] TextMeshProUGUI m_currentRoleText;
    [SerializeField] TextMeshPro m_nicknameTMP;

    [SerializeField] protected string m_currentRoleName;
    [SerializeField] public GameObject[] m_arrowParts;
    [SerializeField] public Material[] m_materials;

    [Header("Ball")]
    [SerializeField] protected Transform m_ballPosition;
    [SerializeField] protected float m_kickForce;
    [SerializeField] protected bool m_canKick;

    PhotonView m_PV;

    #endregion

    #region Knobs

    [SerializeField] protected float playerSpeed;

    #endregion

    #region RuntimeVariables

    [SerializeField] protected float playerInputHorizontal;
    [SerializeField] protected float playerInputForward;
    Vector3 m_moveDirection;

    Vector3 cameraForward;
    Vector3 cameraRight;

    #endregion

    #region UnityMethods

    private void Start()
    {
        m_PV = GetComponent<PhotonView>();
        //m_PV.Owner.NickName = PhotonNetwork.NickName; // NO PEDIRLO NUNCA MÁS DE UNA VEZ.
        //m_nickname.text = m_PV.Owner.NickName;
        gameObject.name = m_PV.Owner.NickName;
        m_myAnim.SetBool("JOG", false);
        m_myAnim.SetBool("STOP", true);
        PhotonPeer.RegisterType(typeof(Color), (byte)'C', TypeTransformer.SerializeColor, TypeTransformer.DeserializeColor);
        m_boxCollider.enabled = false;
        m_triggerCollision.SetActive(true);
        m_particleSystem.gameObject.SetActive(false);
        m_currentRoleText.text = "Role...";
        m_nicknameTMP.text = gameObject.name;
    }

    private void Update()
    {
        if (!m_PV.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(m_currentRoleName == "")
            {
                LevelNetworkManager.Instance.disconnectFromCurrentRoom();
            }
            else
            {
                deathEvent(m_currentRoleName);
                LevelNetworkManager.Instance.disconnectFromCurrentRoom();
            }
        }

        if (Input.GetKeyDown(KeyCode.K) && m_canKick)
        {
            playerSpeed = 0;
            m_myAnim.SetBool("JOG", false);
            m_myAnim.SetBool("KICK", true);
            GameObject m_ball = m_ballPosition.GetChild(0).gameObject;
            m_ball.GetComponent<Rigidbody>().isKinematic = false;
            m_ball.gameObject.transform.SetParent(null);
            m_ball.GetComponent<Rigidbody>().AddForce(transform.forward * m_kickForce, ForceMode.Impulse);
            StartCoroutine(cooldownTimer());
        }
    }

    private void FixedUpdate()
    {
        if (!m_PV.IsMine)
        {
            return;
        }
        //m_nickname.transform.position = new Vector3(transform.position.x, transform.position.y + 4.5f, transform.position.z);
        PlayerMov();
        m_nicknameTMP.text = gameObject.name;
        m_nicknameTMP.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
    }

    private void OnTriggerStay(Collider p_other)
    {
        if (p_other.gameObject.CompareTag("Ball") && !m_canKick)
        {
            p_other.GetComponent<Rigidbody>().isKinematic = true;
            p_other.gameObject.transform.position = m_ballPosition.transform.position;
            p_other.gameObject.transform.SetParent(m_ballPosition.transform, true);
            m_canKick = true;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    #endregion

    #region PublicMethods

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case 1: // Evento de asignación de roles.
                GetNewGameplayRole();
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    #endregion

    #region LocalMethods

    void deathEvent(string role)
    {
        if (m_PV.IsMine)
        {
            if (role == "Traitor")
            {
                byte m_ID = 2;//Codigo del Evento (1...199)
                object content = role;
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
            }
            else if (role == "Innocent")
            {
                byte m_ID = 3;//Codigo del Evento (1...199)
                object content = role;
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
            }
        }
    }


    void PlayerMov()
    {
        if (m_PV.IsMine)
        {
            playerInputHorizontal = Input.GetAxisRaw("Horizontal");
            playerInputForward = Input.GetAxisRaw("Vertical");

            cameraForward = Camera.main.transform.forward;
            cameraRight = Camera.main.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;

            cameraForward.Normalize();
            cameraRight.Normalize();

            m_moveDirection = (cameraForward * playerInputForward + cameraRight * playerInputHorizontal).normalized;

            rb.velocity = m_moveDirection * playerSpeed;

            if (m_moveDirection.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(m_moveDirection);
                m_myAnim.SetBool("JOG", true);
                m_myAnim.SetBool("STOP", false);
                m_myAnim.SetFloat("MovingFloat", m_moveDirection.magnitude);
            }
            else
            {
                m_myAnim.SetBool("JOG", false);
                m_myAnim.SetBool("STOP", true);
            }
        }
    }

    void ShowNickname(string p_nickname)
    {
        if (m_PV.IsMine)
        {
            gameObject.GetComponentInChildren<TextMeshPro>().text = p_nickname;
        }
    }

    IEnumerator WaitForParticleSystem()
    {
        //PhotonNetwork.Instantiate("Tris Spark 2", this.gameObject.transform.position, Quaternion.identity);
        //Instantiate(m_particleSystem, this.gameObject.transform.position, Quaternion.identity);
        m_particleSystem.gameObject.SetActive(true);
        m_particleSystem.Play();
        yield return new WaitForSeconds(m_particleSystem.main.duration);
        deathEvent(m_currentRoleName);
        LevelManager.instance.getNewInfoGame(gameObject.name);
        PhotonNetwork.Destroy(gameObject);
    }

    void GetNewGameplayRole()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object role))
        {
            //LevelManager.AssignRole();

            string m_newPlayerRole = role.ToString();

            switch (m_newPlayerRole)
            {
                case "Innocent":
                    //Soy inocente
                    //gameObject.GetComponentInChildren<TextMeshPro>().text = "Innocent";
                    m_currentRoleText.text = "Innocent";
                    m_currentRoleName = "Innocent";
                    m_currentRoleText.color = Color.blue;
                    for(int i = 0; i < m_arrowParts.Length; ++i)
                    {
                        m_arrowParts[i].GetComponent<MeshRenderer>().material = m_materials[0];
                    }
                    break;
                case "Traitor":
                    //Soy una rata
                    //gameObject.GetComponentInChildren<TextMeshPro>().text = "Traitor";
                    m_currentRoleText.text = "Traitor";
                    m_currentRoleName = "Traitor";
                    m_currentRoleText.color = Color.red;
                    for (int i = 0; i < m_arrowParts.Length; ++i)
                    {
                        m_arrowParts[i].GetComponent<MeshRenderer>().material = m_materials[1];
                    }
                    break;
            }
        }
    }

    protected IEnumerator cooldownTimer()
    {
        yield return new WaitForSeconds(1);
        m_myAnim.SetBool("KICK", false);
        m_myAnim.SetBool("JOG", true);
        playerSpeed = 12;
        m_canKick = false;
    }

    #endregion
}
