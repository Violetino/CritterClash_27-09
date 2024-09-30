using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AI_Movement : MonoBehaviourPun, IPunObservable
{
    private Animator animator;
    public float moveSpeed = 0.2f;

    private Vector3 stopPosition;
    private float walkTime;
    public float walkCounter;
    private float waitTime;
    public float waitCounter;

    private int WalkDirection;
    public bool isWalking;

    // PUN-related variables
    private Vector3 networkedPosition;
    private Quaternion networkedRotation;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        //So that all the prefabs don't move/stop at the same time
        walkTime = Random.Range(3, 6);
        waitTime = Random.Range(5, 7);

        waitCounter = waitTime;
        walkCounter = walkTime;

        ChooseDirection();

        if (!photonView.IsMine)
        {
            networkedPosition = transform.position;
            networkedRotation = transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            // Local player controls the AI
            if (isWalking)
            {
                animator.SetBool("isRunning", true);
                walkCounter -= Time.deltaTime;

                switch (WalkDirection)
                {
                    case 0:
                        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        transform.position += transform.forward * moveSpeed * Time.deltaTime;
                        break;
                    case 1:
                        transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                        transform.position += transform.forward * moveSpeed * Time.deltaTime;
                        break;
                    case 2:
                        transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                        transform.position += transform.forward * moveSpeed * Time.deltaTime;
                        break;
                    case 3:
                        transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                        transform.position += transform.forward * moveSpeed * Time.deltaTime;
                        break;
                }

                if (walkCounter <= 0)
                {
                    stopPosition = transform.position;
                    isWalking = false;
                    transform.position = stopPosition;
                    animator.SetBool("isRunning", false);
                    waitCounter = waitTime;
                }
            }
            else
            {
                waitCounter -= Time.deltaTime;

                if (waitCounter <= 0)
                {
                    ChooseDirection();
                }
            }
        }
        else
        {
            // Remote AI movement synchronization
            transform.position = Vector3.Lerp(transform.position, networkedPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkedRotation, Time.deltaTime * 10);
        }
    }

    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4);
        isWalking = true;
        walkCounter = walkTime;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Local player writes data to the stream.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Remote player reads data from the stream.
            networkedPosition = (Vector3)stream.ReceiveNext();
            networkedRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
