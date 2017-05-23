using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Rigidbody2D myRidigBody;

    [SerializeField]
    private float movementSpeed;

    private Animator myAnimator;

    // Use this for initialization
    void Start() {
        facingRight = true;
        myRidigBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
    }

    private bool facingRight;

    private bool attack;

    private bool slide;

    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    private bool isGrounded;

    private bool isJump;

    private bool jumpAttack;

    [SerializeField]
    private bool airControl;

    [SerializeField]
    private float jumpForce;

    // Update is called once per frame
    void FixedUpdate() {

        float horizontal = Input.GetAxis("Horizontal");

        isGrounded = IsGrounded();

        HandleMovement(horizontal);

        Flip(horizontal);

        HandleAttacks();

        HandleLayers();

        ResetValues();
    }

    private void HandleMovement(float horizontal)
    {
        if(myRidigBody.velocity.y < 0)
        {
            myAnimator.SetBool("land", true);
        }

        if ((isGrounded || airControl) && !myAnimator.GetBool("Slide") && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            myRidigBody.velocity = new Vector2(horizontal * movementSpeed, myRidigBody.velocity.y);
        }

        if(isGrounded && isJump)
        {
            isGrounded = false;

            myRidigBody.AddForce(new Vector2(0, jumpForce));

            myAnimator.SetTrigger("jump");
        }

        if (slide && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide")) {
            myAnimator.SetBool("slide", true);
        }
        else if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            myAnimator.SetBool("slide", false);
        }

        myAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    private void HandleAttacks()
    {
        if (attack && isGrounded && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            myAnimator.SetTrigger("attack");
            myRidigBody.velocity = Vector2.zero;
        }
        if (jumpAttack && !isGrounded && !myAnimator.GetCurrentAnimatorStateInfo(1).IsName("jumpAttack")) {
            myAnimator.SetBool("jumpAttack", true);
        }
        if(!jumpAttack && !myAnimator.GetCurrentAnimatorStateInfo(1).IsName("jumpAttack"))
        {
            myAnimator.SetBool("jumpAttack", false);
        }

    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            attack = true;
            jumpAttack = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            slide = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJump = true;
        }

    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;

            theScale.x *= -1;

            transform.localScale = theScale;
        }
    }

    private void ResetValues()
    {
        attack = false;
        slide = false;
        isJump = false;
        jumpAttack = false;
    }

    private bool IsGrounded()
    {
        if(myRidigBody.velocity.y <= 0)
        {
            foreach(Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position,
                    groundRadius, whatIsGround);

                for(int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i].gameObject != gameObject)
                    {
                        myAnimator.ResetTrigger("jump");
                        myAnimator.SetBool("land", false);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void HandleLayers()
    {
        if (!isGrounded)
        {
            myAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            myAnimator.SetLayerWeight(1, 0);
        }
    }
}
