using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public float gravityScale;
    public CharacterController controller;

    public bool firstJumpActive;
    public bool secondJumpActive;
    public float secondJumpMultiplier;
    public float thirdJumpMultiplier;

    public AudioSource jump1;
    public AudioSource jump2;
    public AudioSource jump3;

    public Transform pivot;
    public float rotateSpeed;
    public Transform playerModel;

    public Animator anim;

    private float secondJumpTimer;
    private float thirdJumpTimer;
    private bool isMoving;
    
    private Vector3 moveDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //moveDirection = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, moveDirection.y, Input.GetAxis("Vertical") * moveSpeed);
        float yStore = moveDirection.y;
        
        moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));
        moveDirection = moveDirection.normalized * moveSpeed;
        moveDirection.y = yStore;

        if (controller.isGrounded)
        {
            moveDirection.y = 0f;
            if (Input.GetButtonDown("Jump") && firstJumpActive == false && secondJumpActive == false)
            {
                moveDirection.y = jumpForce;
                jump1.Play();
                Debug.Log("Jump");
                firstJumpActive = true;
                anim.SetBool("Jump1", true);
                anim.SetBool("Jump2", false);
                anim.SetBool("Jump3", false);

            }
            if(Input.GetButtonDown("Jump") && firstJumpActive && secondJumpTimer > 0f)
            {
                moveDirection.y = jumpForce * secondJumpMultiplier;
                jump2.Play();
                Debug.Log("Double Jump");
                secondJumpTimer = 0f;
                firstJumpActive = false;
                secondJumpActive = true;
                anim.SetBool("Jump1", false);
                anim.SetBool("Jump2", true);
                anim.SetBool("Jump3", false);
            }
            if (Input.GetButtonDown("Jump") && secondJumpActive && thirdJumpTimer > 0f && isMoving == true)
            {
                moveDirection.y = jumpForce * thirdJumpMultiplier;
                jump3.Play();
                Debug.Log("Triple Jump");
                thirdJumpTimer = 0f;
                firstJumpActive = false;
                secondJumpActive = false;
                anim.SetBool("Jump1", false);
                anim.SetBool("Jump2", false);
                anim.SetBool("Jump3", true);
            }
            if(secondJumpTimer >= 0.3f)
            {
                secondJumpTimer = 0f;
                firstJumpActive = false;
                anim.SetBool("Jump1", false);
                anim.SetBool("Jump2", false);
                anim.SetBool("Jump3", false);
            }
            if(thirdJumpTimer >= 0.2f)
            {
                thirdJumpTimer = 0f;
                secondJumpActive = false;
                anim.SetBool("Jump1", false);
                anim.SetBool("Jump2", false);
                anim.SetBool("Jump3", false);
            }
            if (firstJumpActive)
            {
                secondJumpTimer += Time.deltaTime;
            }
            if (secondJumpActive)
            {
                thirdJumpTimer += Time.deltaTime;
            }

            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }

        moveDirection.y = moveDirection.y + (Physics.gravity.y * Time.deltaTime * gravityScale);
        controller.Move(moveDirection * Time.deltaTime);

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }

        anim.SetBool("IsGrounded", controller.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal"))));
    }
}
