using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {

    Animator playerAnimator;
    Rigidbody2D playerBody;
    Vector3 faceLeft = new Vector3(-1, 1, 1), faceRight = new Vector3(1, 1, 1);
    AudioSource playerSource;
    public AudioClip[] soundEffects;
    public bool isDead = false, doIntro;
    public float maxHealth, health;
    bool damaged, startIntro = true;
    //States
    public bool _moving, _intro;
    // Walking
    float momentum, maxSpeedRight = 7.5f, maxSpeedLeft = -7.5f;
    bool changedDirection = false, isRight = true, againstWall = false;

    //Jumping
    float startJumpHeight, maxJumpHeight = 4f, yMomentum = 0, maxFall = -20f;
    bool isGrounded = true, isHoldingJump = false, isJumping = false, jumpEnd = false, slipped = false;   

    void Start ()
    {
        health = maxHealth;
        playerAnimator = GetComponent<Animator>();
        playerBody = GetComponent<Rigidbody2D>();
        playerSource = GetComponent<AudioSource>();
        playerAnimator.SetBool("IsGrounded", true);

        if(doIntro)
        {
            _moving = false;
            _intro = true;
        }
        else
        {
            _moving = true;
            _intro = false;
        }
	}

    IEnumerator TakeDamage()
    {
        damaged = true;
        health--;
        yield return new WaitForSeconds(0.5f);
        damaged = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        ContactPoint2D contact = other.contacts[0];
        //print(contact.normal);

        if (other.gameObject.tag == "Slope")
        {
            print("ayy");
            if (contact.normal.x != 1.0f || contact.normal.x != -1.0f)
            {
                print("lmao");
                if (contact.normal.y == -1)
                {
                    print("lmayo");
                    if (!jumpEnd)
                    {
                        jumpEnd = true;
                        yMomentum = 0;
                        if (isHoldingJump)
                        {
                            isHoldingJump = false;
                        }
                    }
                }
                else
                {
                    isGrounded = true;
                    isJumping = false;
                    jumpEnd = false;
                    slipped = false;
                    yMomentum = 0;
                }
            }
            else
            {
                print("doot");
                momentum = 0;
                againstWall = true;
                if (!jumpEnd)
                {
                    jumpEnd = true;
                    yMomentum = 0;
                    if (isHoldingJump)
                    {
                        isHoldingJump = false;
                    }
                }
            }

        }
        else if (other.gameObject.name == "coin(Clone)" || other.gameObject.name == "crystal(Clone)" || other.gameObject.name == "crystal 1(Clone)")
        {
            print("ding");
            playerSource.clip = soundEffects[2];
            playerSource.Play();
        }
        else
        {
            if (contact.normal.x == 0)
            {
                if (contact.normal.y == -1)  //Above
                {

                    if (!jumpEnd)
                    {
                        jumpEnd = true;
                        yMomentum = 0;
                        if (isHoldingJump)
                        {
                            isHoldingJump = false;
                        }
                    }
                    if (other.gameObject.tag == "DestructablePlatform")
                    {
                        if (other.gameObject.name == "Mystery")
                        {
                            other.gameObject.GetComponent<mysteryBox>().Break();
                        }
                        else
                        {
                            Destroy(other.gameObject);
                        }
                    }
                    if (other.gameObject.tag == "Enemy" & !damaged)
                    {
                        playerSource.clip = soundEffects[0];
                        playerSource.Play();
                        StartCoroutine(TakeDamage());
                    }
                }
                else if (contact.normal.y == 1) //Below
                {
                    if (other.gameObject.tag == "Platform" || other.gameObject.tag == "DestructablePlatform")
                    {
                        isGrounded = true;
                        isJumping = false;
                        jumpEnd = false;
                        slipped = false;
                        yMomentum = 0;
                    }
                    else if (other.gameObject.tag == "Enemy")
                    {
                        isGrounded = true;
                        yMomentum = 0;
                        if (Input.GetKey(KeyCode.Space))
                        {
                            isHoldingJump = true;
                            jumpEnd = false;
                        }
                        else
                        {
                            jumpEnd = true;
                        }
                        //playerSource.clip = soundEffects[1];
                        //playerSource.Play();
                        TryJump();
                        other.gameObject.GetComponent<enemyScript>().health--;
                    }
                }
            }
            else    //To left or right
            {
                // Zero out momentum if you bump into a wall
                if (other.gameObject.tag == "Platform" || other.gameObject.tag == "DestructablePlatform")
                {
                    momentum = 0;
                    againstWall = true;
                }
                else if (other.gameObject.tag == "Enemy" & !damaged)
                {
                    momentum = 10 * contact.normal.x;
                    playerSource.clip = soundEffects[0];
                    playerSource.Play();
                    StartCoroutine(TakeDamage());
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        
        ContactPoint2D contact = other.contacts[0];

        if (other.gameObject.tag == "Slope")
        {
            isGrounded = false;
            againstWall = false;
        }
        else if (contact.normal.x == 0) //Above or below
        {
            if (contact.normal.y == -1)  //Above
            {

            }
            else if (contact.normal.y == 1) //Below
            {

                if (other.gameObject.tag == "Platform" || other.gameObject.tag == "DestructablePlatform")
                {
                    isGrounded = false;
                }
                else if (other.gameObject.tag == "Enemy")
                {
                    isGrounded = false;
                }
            }
        }
        else    //To left or right
        {
            if (other.gameObject.tag == "Platform" || other.gameObject.tag == "DestructablePlatform")
            {
                againstWall = false;
            }
        }
    }

    void SetDirection()
    {
        if (Input.GetAxisRaw("Horizontal") == -1 && isRight)
        {
            transform.localScale = faceLeft;
            isRight = false;
            changedDirection = true;
        }

        if (Input.GetAxisRaw("Horizontal") == 1 &! isRight)
        {
            transform.localScale = faceRight;
            isRight = true;
            changedDirection = true;
        }
    }

    void SetMomentum()
    {
        if (Input.GetAxisRaw("Sprint") == 1)
        {
            maxSpeedLeft = -12.5f;
            maxSpeedRight = 12.5f;
        }
        else
        {
            maxSpeedLeft = -7.5f;
            maxSpeedRight = 7.5f;
        }

        if (Input.GetAxisRaw("Horizontal") == 1 && isRight &! changedDirection)
        {
            if (momentum >= maxSpeedRight)
            {
                momentum = maxSpeedRight;
            }
            else
            {
                momentum += 0.25f;
            }
        }
        else if (Input.GetAxisRaw("Horizontal") == 1 && isRight && changedDirection)
        {
            if (momentum < 0)
            {
                momentum += 1.0f;
            }
            else if (momentum >= 0)
            {
                changedDirection = false;
                momentum += 0.25f;
            }
        }
        else if (Input.GetAxisRaw("Horizontal") == -1 &! isRight &! changedDirection)
        {
            if (momentum <= maxSpeedLeft)
            {
                momentum = maxSpeedLeft;
            }
            else
            {
                momentum -= 0.25f;
            }
        }
        else if (Input.GetAxisRaw("Horizontal") == -1 &! isRight && changedDirection)
        {
            if (momentum > 0)
            {
                momentum -= 1.0f;
            }
            else if (momentum <= 0)
            {
                changedDirection = false;
                momentum -= 0.25f;
            }
        }
        else
        {
            if (momentum < 0)
            {
                momentum += 0.25f;
            }
            else if (momentum > 0)
            {
                momentum -= 0.25f;
            }
            else
            {
                momentum = 0;
            }
        }
        if (momentum < 0)
        {
            playerAnimator.SetFloat("WalkSpeed", momentum * -1);
        }
        else
        {
            playerAnimator.SetFloat("WalkSpeed", momentum);
        }

        playerBody.velocity = new Vector3(momentum, playerBody.velocity.y);

    }

    void TryJump()
    {
        if (isGrounded)
        {
            isJumping = true;
            startJumpHeight = transform.position.y;
            yMomentum = 15f;
        }
        else if (isJumping &! jumpEnd)
        {
            if (transform.position.y - startJumpHeight >= maxJumpHeight)
            {
                jumpEnd = true;
                yMomentum -= 1.0f;
                if (isHoldingJump)
                {
                    isHoldingJump = false; 
                }
            }
            else
            {
                yMomentum -= 0.1f;  // Reduce momentum to create arc
            }
        }
        else if (jumpEnd && yMomentum > maxFall)
        {
            yMomentum -= 1.0f;
        }
        else if (jumpEnd && yMomentum <= maxFall)
        {
            yMomentum = maxFall;
        }
    }

    void Die()
    {
        isDead = true;
        GetComponent<SpriteRenderer>().enabled = false;
        momentum = 0;
        Destroy(GetComponent<Rigidbody2D>());
        this.enabled = false;
    }



    public void spawnBoost()
    {
        //TryJump();
        //yield return new WaitForSeconds(0.5f);  
    }

    void Update()
    {
        if (_moving)
        {
            if (health <= 0)
            {
                Die();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!jumpEnd)
                {
                    isHoldingJump = true;
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                isHoldingJump = false;
                if (!jumpEnd && isJumping)
                {
                    jumpEnd = true;
                }
            }

            if (isHoldingJump)
            {
                TryJump();
            }

            if (!isHoldingJump && isJumping && yMomentum > maxFall)
            {
                yMomentum -= 1.0f;
            }
            else if (!isHoldingJump && isJumping && yMomentum <= maxFall)
            {
                yMomentum = maxFall;
            }


            if (!isGrounded & !isJumping & !slipped)       //Fell off ledge
            {
                slipped = true;
                yMomentum = 0f;
            }
            else if (!isGrounded & !isJumping && slipped && yMomentum > maxFall)
            {
                yMomentum -= 1.0f;
            }
            else if (!isGrounded & !isJumping && slipped && yMomentum <= maxFall)
            {
                yMomentum = maxFall;
            }

            playerBody.velocity = new Vector3(playerBody.velocity.x, yMomentum);
            playerAnimator.SetBool("IsGrounded", isGrounded);
            playerAnimator.SetBool("IsDamaged", damaged);
        }
        else if (_intro)
        {
            if (startIntro)
            {
                playerBody.velocity = new Vector3(6f, 25f);
                startIntro = false;
            }
            else if (playerBody.velocity.y != 0)
            {
                //print("I BELIEVE I CAN FLY");
            }
            else
            {
                playerBody.velocity = Vector2.zero;
                _intro = false;
                _moving = true;
            }
            playerAnimator.SetBool("IsGrounded", isGrounded);
        }
        else // No valid states
        {
            _intro = false;
            _moving = true;
        }

    }
    
	void FixedUpdate ()
    {
        if (_moving)
        {
            SetDirection();
            SetMomentum();
        }
        else if (_intro)
        {

        }
	}
}
