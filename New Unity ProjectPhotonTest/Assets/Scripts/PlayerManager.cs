using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Collections;
using Photon.Pun.Demo.PunBasics;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private GameObject beams;

    private bool isFiring = false;

    public float Health = 1f;

    private void Awake()
    {
        if (beams == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
        }
        else
        {
            beams.SetActive(false);
        }
    }

    private void Start()
    {
        CameraWork cameraWork = this.gameObject.GetComponent<CameraWork>();

        if (cameraWork != null && photonView.IsMine)
            cameraWork.OnStartFollowing();
        else
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            processInputs();

            if (Health <= 0f)
                GameManager.Instance.LeaveRoom();
        }

        if (beams != null && isFiring != beams.activeInHierarchy)
        {
            beams.SetActive(isFiring);
        }
    }

    private void processInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isFiring)
            isFiring = true;
        if (Input.GetKeyUp(KeyCode.Mouse0) && isFiring)
            isFiring = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (!other.name.Contains("Beam"))
            return;

        Health -= 0.1f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (!other.name.Contains("Beam"))
            return;

        Health -= 0.1f * Time.deltaTime;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isFiring);
            stream.SendNext(Health);
        }
        else
        {
            this.isFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }
}
