using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAnimatorManager : MonoBehaviourPun
{
    [SerializeField]
    private float directionDampTime = 0.25f;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if (!animator)
            Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;

        if (!animator)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (vertical < 0)
            vertical = 0;

        animator.SetFloat("Speed", horizontal * horizontal + vertical * vertical);
        animator.SetFloat("Direction", horizontal, directionDampTime, Time.deltaTime); 
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // only allow jumping if we are running.
        if (stateInfo.IsName("Base Layer.Run"))
        {
            // When using trigger parameter
            if (Input.GetButtonDown("Fire2"))
            {
                animator.SetTrigger("Jump");
            }
        }
    }
}
