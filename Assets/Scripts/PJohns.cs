using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PJohns : MonoBehaviour
{
    public enum playerState { GeneralMovement, Falling, Crouch, Climb, EndClimb, Hide, Whisper, Freeze, MindControl, Frozen, Transition, SendAway, Ledge }
    //Config
    [SerializeField] playerState state;
    public GameObject target;
    public List<GameObject> pigeonbox;
    [SerializeField] float jumpHeight;
    private float walkSpeed = 2f;
    private float zwalkSpeed = 1.9f;
    private float runSpeed = 4f;
    private float zrunSpeed = 3.9f;
    [SerializeField] float climbSpeed = 3f;
    [SerializeField] float crouchSpeed = 1.8f;
    [SerializeField] bool freezeCharacter;
    [SerializeField] bool doubleJump;
    [SerializeField] bool pigeonShield;
    [SerializeField] bool dismiss;
    [SerializeField] bool mindControl;
    [SerializeField] bool fly;
    public Vector3 fallspeed;
    public Vector3 playerVelocity;
    public Vector3 direction;
    public Vector3 ledgeGrabPoint;
    private Vector3 foodPosA;
    private Vector3 foodPosB;
    private float gravity = -9.81f;
    public int foodmove;
    private int doubletapcount = 0;
    public bool foodCollected = false;
    public bool zipToPlayer = false;
    public bool isTouchingGround;
    public bool jumppressed = false;
    public bool isCrouching;
    public bool playerIsWalking;
    public bool touchingDumpster;
    public bool touchingLadder;
    public bool holdingLadder;
    public bool imWhispering;
    public bool imHiding;
    public bool pigeonshiding = false;
    private bool ledgecapable;
    private bool topraybool;
    private bool lowraybool;
    private bool climbdirection;
    private bool endclimbanimation; 
    private float ledgerayx;


    //Cached Component References
    private GameObject collectedFood;
    public GameObject gamemanager;
    private CharacterController controller;
    public BoxCollider myCollider;
    Animator myAnimator;
    AudioSource myAudioSource;
    public AudioClip[] audioClips;
    Text actionText;
    private GameObject followcam = null;

    //Other Object References
    public Vector3 currentLedge;
    //public List<GameObject> pigeonbox = new List<GameObject>();
    private GameManager gameManager;
    private GameObject crossHair;
    GameObject hotdog;
    public GameObject currentDumpster = null;
    public GameObject currentLadder = null;
    private float ladderYpos;
    private float ladderXpos;


    // Start is called before the first frame update
    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
        myAudioSource = gameObject.GetComponent<AudioSource>();
        myAnimator = gameObject.GetComponentInChildren<Animator>();
        controller = gameObject.GetComponent<CharacterController>();
        myCollider = gameObject.GetComponent<BoxCollider>();
        crossHair = GameObject.Find("crosshair");
        crossHair.GetComponent<SpriteRenderer>().enabled = false;
        hotdog = GameObject.Find("Hotdog");
    }
    private void PlayerState()
    {
        if (state == playerState.GeneralMovement)
        {
            Walk();
        }
        if (state == playerState.Crouch)
        {
            Crouch();
        }
        if (state == playerState.Falling)
        {
            controller.Move(playerVelocity * Time.deltaTime);
        }
        if (state == playerState.Whisper)
        {
            Whispering();
        }
        if (state == playerState.Ledge)
        {
            LedgeHang();
        }
        if (state == playerState.Climb)
        {
            ClimbLadder();
        }
        if (state == playerState.EndClimb)
        {
            EndClimb();
        }
        if (state == playerState.Hide)
        {
            DumpsterDive();
        }
    }
    private void Walk()
    {
        myAnimator.SetBool("CrouchWalking", false);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3((horizontal * walkSpeed), 0, (vertical * zwalkSpeed));
        if (direction.magnitude >= 0.1f)
            {
                controller.Move(direction * Time.deltaTime);
            }
        // Run by holding LEFTSHIFT
        bool playerisRunning = Mathf.Abs(direction.x) > 2.5f;
        myAnimator.SetBool("Running", playerisRunning);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            walkSpeed = runSpeed;
            zwalkSpeed = zrunSpeed;
        }
        else
        {
            walkSpeed = 2f;
            zwalkSpeed = 1.9f;
        }
        //JUMPING LOGIC
        // Press SPACE to Jump if on the ground
        if (Input.GetKeyDown(KeyCode.Space) && isTouchingGround && !jumppressed)
        {
            myAnimator.SetBool("Jump", true);
            myAudioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            myAudioSource.Play();
            playerVelocity.y = 0f;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -1.0f * gravity);
            jumppressed = true;
        }
        // If on the ground stop vertical movement
        if (isTouchingGround && playerVelocity.y < 0)
        {
            myAnimator.SetBool("Jump", false);
            playerVelocity.y = 0f;
            jumppressed = false;
        }
        // Transition to Crouch
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(PauseAnimation());
            IEnumerator PauseAnimation()
            {
                yield return new WaitForSeconds(.5f);
                if (!isCrouching)
                {
                    isCrouching = true;
                }
                else if (isCrouching)
                {
                    isCrouching = false;
                }
            }
            playerIsWalking = false;
            state = playerState.Crouch;
        }
        if (Input.GetKey(KeyCode.Q) && (!isCrouching) && (!jumppressed))
        {
            state = playerState.Whisper;
        }
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Animate walking based on movement
        bool playerHasHorizontalSpeed = Mathf.Abs(direction.x + direction.z) > Mathf.Epsilon; // bool deciding if velocity is higher than 0
        myAnimator.SetBool("Walking", playerHasHorizontalSpeed);
        // Ledge grab logic
        currentLedge.z = transform.position.z;
        Vector3 raydirection = new Vector3(transform.localScale.x, 0f, 0f);
        Vector3 topraycast = new Vector3(transform.position.x, transform.position.y + 1.7f, transform.position.z);
        Vector3 lowraycast = new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z);
        Vector3 ledgeraycast = new Vector3(ledgerayx, transform.position.y + 2f, transform.position.z);
        RaycastHit topray;
        if (Physics.Raycast(topraycast, raydirection, out topray, .5f) && topray.collider.tag == "Ledge")
        {
            Debug.DrawRay(topraycast, raydirection * .5f, Color.green);
            topraybool = true;
        }
        else
        {
            topraybool = false;
        }
        RaycastHit lowray;
        if (Physics.Raycast(lowraycast, raydirection, out lowray, .5f) && lowray.collider.tag == "Ledge")
        {
            ledgerayx = lowray.point.x;
            currentLedge.x = lowray.point.x;
            Debug.DrawRay(lowraycast, raydirection * .5f, Color.blue);
            lowraybool = true;
        }
        else
        {
            lowraybool = false;
        }
        RaycastHit ledgeray;
        if (!topraybool && lowraybool)
        {
            if (Physics.Raycast(ledgeraycast, Vector3.down, out ledgeray, 2f) && ledgeray.collider.tag == "Ledge")
            {
                currentLedge.y = ledgeray.point.y;
                Debug.DrawRay(ledgeraycast, Vector3.down * 1f, Color.yellow);
                ledgecapable = true;
            }
        }
        else
        {
            ledgecapable = false;
        }
        // grab the ledge
        if (ledgecapable)
        {
            ledgeGrabPoint = currentLedge;
            state = playerState.Ledge;
        }
        // Ladder Climbing
        RaycastHit frontray;
        RaycastHit sideray;
        int layerMask = 1 << 8;
        Vector3 forwardraycast = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (Physics.Raycast(forwardraycast, raydirection, out sideray, .5f, layerMask))
        {
            Debug.DrawRay(forwardraycast, raydirection, Color.cyan);
            if (sideray.collider.gameObject.tag == "Ladder")
            {
                ladderYpos = sideray.point.y;
                ladderXpos = sideray.point.x;
                currentLadder = sideray.collider.gameObject;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    holdingLadder = true;
                    transform.position = new Vector3(transform.position.x, ladderYpos, currentLadder.transform.position.z);
                    climbdirection = true;
                    state = playerState.Climb;
                }
            }
        }
        if (Physics.Raycast(forwardraycast, Vector3.forward, out frontray, .5f, layerMask))
        {
            Debug.DrawRay(forwardraycast, Vector3.forward, Color.cyan);
            if (frontray.collider.gameObject.tag == "Dumpster")
            {
                touchingDumpster = true;
                currentDumpster = frontray.collider.gameObject;
            }
            if (frontray.collider.gameObject.tag == "Ladder")
            {
                ladderYpos = frontray.point.y;
                currentLadder = frontray.collider.gameObject;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    holdingLadder = true;
                    transform.position = new Vector3(currentLadder.transform.position.x, ladderYpos, currentLadder.transform.position.z - 0.5f);
                    state = playerState.Climb;
                }
            }
        }
        else
        {
            currentLadder = null;
            touchingDumpster = false;
            currentDumpster = null;
        }
        // Transition to Hiding
        if (touchingDumpster == true && Input.GetKeyDown(KeyCode.E))
        {
            state = playerState.Hide;
        }
        Falling();
    }
    private void Falling()
    {
        if (controller.velocity.y < -6f)
        {
            myAnimator.SetBool("Crouching", false);
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Falling", true);
        }
        else if (isTouchingGround == true)
        {
            myAnimator.SetBool("Falling", false);
        }
    }
    private void Crouch()
    {
        // Animation tells isCrouching = true & freezes character until after animation completes
        if (Input.GetKeyDown(KeyCode.C) && (!isCrouching))
        {
            myAnimator.SetBool("Crouching", true);
            controller.center = new Vector3(0f, 0.45f, 0f);
            controller.height = .86f;
        }
        // Else if I press C and I am crouching
        else if (Input.GetKeyDown(KeyCode.C) && (isCrouching))
        {
            myAnimator.SetBool("Crouching", false);
            myAnimator.SetBool("NotCrouching", false);
            isCrouching = false;
            controller.center = new Vector3(0f, 0.79f, 0f);
            controller.height = 1.47f;
            StartCoroutine(PauseAnimation());
            IEnumerator PauseAnimation()
            {
                yield return new WaitForSeconds(.5f);
                isCrouching = false;
                state = playerState.GeneralMovement;
            }
        }
        // X Axis Crouching
        if (isCrouching == true)
        {
            float horizontalinputspeed = Input.GetAxis("Horizontal"); // value is between -1 to +1
            float verticalinputspeed = Input.GetAxis("Vertical");
            direction = new Vector3((horizontalinputspeed * crouchSpeed), gravity, (verticalinputspeed * crouchSpeed)); // controlling speed of character
            controller.Move(direction * Time.deltaTime);
            bool playerHasHorizontalSpeed = Mathf.Abs(direction.x + direction.z) > Mathf.Epsilon; // bool deciding if velocity is higher than 0
            myAnimator.SetBool("CrouchWalking", playerHasHorizontalSpeed);
        }
    }
    private void Whispering()
    {
        // Whisper To Hotdog While Holding Q..
        if (Input.GetKey(KeyCode.Q))
        {
            myAnimator.SetBool("TalkToBirds", true);
            myAnimator.SetBool("Walking", false);
            crossHair.GetComponent<Crosshair>().crosshairstate = Crosshair.CrosshairState.isactive;
            hotdog.GetComponent<Hotdog>().hotdogState = Hotdog.pigeonState.imlistening;
            foreach (GameObject obj in pigeonbox)
            {
                obj.GetComponent<Pigeon>().state = Pigeon.pigeonState.imlistening;
            }
        }
        // If you hit NOTHING -> General Movement
        else if (Input.GetKeyUp(KeyCode.Q) && crossHair.GetComponent<Crosshair>().ivehitsomething == false)
        {
            myAnimator.SetBool("TalkToBirds", false);
            state = playerState.GeneralMovement;
            crossHair.GetComponent<Crosshair>().crosshairstate = Crosshair.CrosshairState.isdisabled;
            hotdog.GetComponent<Hotdog>().hotdogState = Hotdog.pigeonState.resetpigeon;
            foreach (GameObject obj in pigeonbox)
            {
                obj.GetComponent<Pigeon>().state = Pigeon.pigeonState.followplayer;
            }
        }
        // If you let go of Q after hitting something -> General Movement
        else if (Input.GetKeyUp(KeyCode.Q) && crossHair.GetComponent<Crosshair>().ivehitsomething == true)
        {
            state = playerState.GeneralMovement;
            myAnimator.SetBool("TalkToBirds", false);
            hotdog.GetComponent<Hotdog>().hotdogState = Hotdog.pigeonState.followcommand;
            foreach (GameObject obj in pigeonbox)
            {
                obj.GetComponent<Pigeon>().state = Pigeon.pigeonState.followcommand;
            }
        }
    }
    private void LedgeHang()
    {
        myAnimator.SetBool("Walking", false);
        myAnimator.SetBool("Jump", false);
        myAnimator.SetBool("Running", false);
        myAnimator.SetBool("LedgeGrab", true);
        transform.position = currentLedge;
        myCollider.isTrigger = true;
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerVelocity.y = 0f;
            myAnimator.SetBool("LedgePull", true);
            StartCoroutine(WaitForPull());
        }
        IEnumerator WaitForPull()
        {
            yield return new WaitForSeconds(0.5714285f);
            transform.position = currentLedge;
            myAnimator.SetBool("LedgeGrab", false);
            myAnimator.SetBool("LedgePull", false);
            state = playerState.GeneralMovement;
        }
    }
    private void ClimbLadder()
    {
        if (!climbdirection)
        {
            myAnimator.SetBool("ClimbFront", true);
        }
        else
        {
            myAnimator.SetBool("ClimbSide", true);
        }
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(0f, (vertical * climbSpeed), 0f);
        if (direction.magnitude >= 0.1f)
        {
            controller.Move(direction * Time.deltaTime);
            myAnimator.SetFloat("ClimbSpeed", vertical);
        }
        else
        {
            myAnimator.SetFloat("ClimbSpeed", 0f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myAnimator.SetBool("ClimbFront", false);
            myAnimator.SetBool("ClimbSide", false);
            state = playerState.GeneralMovement;
        }
    }
    private void EndClimb()
    {
        if (!climbdirection && endclimbanimation == true)
        {
            StartCoroutine(FinishClimbFront());
            endclimbanimation = false;
        }
        else if (climbdirection && endclimbanimation == true)
        {
            StartCoroutine(FinishClimbSide());
            endclimbanimation = false;
        }
        IEnumerator FinishClimbFront()
        {
            myAnimator.SetBool("EndClimbFront", true);
            yield return new WaitForSeconds(1f);
            transform.position = new Vector3(transform.position.x, (transform.position.y + .2f), (transform.position.z + .5f));
            yield return new WaitForSeconds(1f);
            myAnimator.SetBool("EndClimbFront", false);
            myAnimator.SetBool("ClimbFront", false);
            myAnimator.SetBool("ClimbSide", false);
            state = playerState.GeneralMovement;
        }
        IEnumerator FinishClimbSide()
        {
            transform.position = new Vector3(transform.position.x + .2f, transform.position.y + .5f, transform.position.z);
            myAnimator.SetBool("EndClimbSide", true);
            yield return new WaitForSeconds(.5f);
            transform.position = new Vector3(transform.position.x + .5f, transform.position.y + .4f, transform.position.z);
            yield return new WaitForSeconds(.5f);
            myAnimator.SetBool("EndClimbSide", false);
            myAnimator.SetBool("ClimbFront", false);
            myAnimator.SetBool("ClimbSide", false);
            state = playerState.GeneralMovement;
        }
    }
    private void DumpsterDive()
    {
        myAnimator.SetBool("Running", false);
        if (imHiding == false)
        {
            controller.enabled = false;
            currentDumpster.gameObject.GetComponent<Dumpster>().IntoDumpster();
            myAnimator.SetBool("IntoDumpster", true);
            imHiding = true;
            StartCoroutine(MovingIntoDumpster());
        }
        else if (Input.GetKeyDown(KeyCode.E) && imHiding == true)
        {
            currentDumpster.gameObject.GetComponent<Dumpster>().OutDumpster();
            myAnimator.SetBool("OutDumpster", true);
            myAnimator.SetBool("IntoDumpster", false);
            imHiding = false;
            StartCoroutine(MovingOutDumpster());
            state = playerState.GeneralMovement;
        }
    }
    IEnumerator MovingIntoDumpster()
    {
        yield return new WaitForSeconds(.37f);
        transform.position = new Vector3(transform.position.x, (transform.position.y + .3f), (transform.position.z + .5f));
    }
    IEnumerator MovingOutDumpster()
    {
        yield return new WaitForSeconds(.37f);
        transform.position = new Vector3(transform.position.x, (transform.position.y + .3f), (transform.position.z - .5f));
        controller.enabled = true;
    }
    private void CalculateGravity()
    {

        if (!isTouchingGround)
        {
            state = playerState.Falling;
        }
        else
        {
            myAnimator.SetBool("Falling", false);
            state = playerState.GeneralMovement;
        }
    }
    private void FlipSprite()
    {
        bool whichDirectionPlayerFacing = Mathf.Abs(direction.x) > Mathf.Epsilon;
        if (whichDirectionPlayerFacing)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1f, 1f);
        }
    }
    public void FootstepAudio()
    {
        myAudioSource.PlayOneShot(audioClips[3], .668f);
    }
    private bool IsGrounded()
    {
        float floorDistanceFromFoot = controller.stepOffset - .2f;
        RaycastHit hit;
        int allButAwareness = ~1 << 15;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, floorDistanceFromFoot, allButAwareness) || controller.isGrounded)
        {
            Debug.DrawRay(transform.position, Vector3.down * floorDistanceFromFoot, Color.yellow);
            return true;
        }
        return false;
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "TopLadder" && state == playerState.Climb)
        {
            endclimbanimation = true;
            state = playerState.EndClimb;
        }
        if (collider.gameObject.tag == "Danger")
        {
            gamemanager.GetComponent<GameManager>().PoliceTime();
        }
        // If object is NotCaptured
        if (collider.gameObject.layer == 11)
        {
            pigeonbox.Add(collider.gameObject);
            foreach (var obj in pigeonbox)
            {
                obj.layer = 12;
                obj.GetComponent<Pigeon>().state = Pigeon.pigeonState.followplayer;
            }
        }
        if (collider.gameObject.tag == "Food")
        {
            foodmove = 2;
            collectedFood = collider.gameObject;
            collectedFood.tag = "Untagged";
            gamemanager.GetComponent<GameManager>().Whoosh();
            foodPosA = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            foodCollected = true;
        }
    }
    private void CollectFood()
    {
        foodPosB = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        if (foodCollected)
        {
            collectedFood.transform.position = Vector3.MoveTowards(collectedFood.transform.position, foodPosA, 4 * Time.deltaTime);
            if (collectedFood.transform.position == foodPosA && foodmove > 0)
            {
                foodmove--;
                foodPosA = foodPosB;
            }
            else if (foodmove <= 0)
            {
                gamemanager.GetComponent<GameManager>().AddFood();
                Destroy(collectedFood);
                foodCollected = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = IsGrounded();
        IsGrounded();
        //CalculateGravity();
        FlipSprite();
        PlayerState();
        CollectFood();
    }
}
