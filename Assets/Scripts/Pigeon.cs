using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;

public class Pigeon : MonoBehaviour
{
    // State Machine
    public enum pigeonState { uncaptured, followplayer, followcommand, imlistening, returntoplayer, resetpigeon, respawnpigeon, sendaway}
    public pigeonState state;

    //Pathfinding
    private NavMeshPath path;
    private float elapsed = 0.0f;

    // Cached Component References
    public GameObject player = null;
    public GameObject target = null;
    private Animator myAnimator;
    private AudioSource myAudio;
    public AudioClip wingflap;
    private CharacterController pigeon;
    public GameObject crosshair;
    public AudioClip[] audioClips;

    // Floats & Values
    [SerializeField] Vector3 hopDistance;
    Vector3 stop = new Vector3(0f, 0f, 0f);
    Vector3 floorposition = new Vector3(0f, 0f, 0f);
    public Vector3 pigeonVelocity;
    [SerializeField] float pigeonSpeed;
    private float gravity = -9.81f;
    Vector3 targetPosition;

    // Booleans
    public bool isTouchingGround;
    public bool jumppressed;
    public bool resetpigeon;

    void Start()
    {
        myAudio = gameObject.GetComponent<AudioSource>();
        path = new NavMeshPath();
        elapsed = 0.0f;
        myAnimator = GetComponentInChildren<Animator>();
        pigeon = GetComponent<CharacterController>();
        crosshair = GameObject.Find("crosshair");
    }

    private void PigeonState()
    {
        if (state == pigeonState.uncaptured)
        {
            return;
        }
        if (state == pigeonState.imlistening)
        {
            ImListening();
        }
        if (state == pigeonState.followplayer)
        {
            FollowPlayer();
            IsFlying();
        }
        if (state == pigeonState.followcommand)
        {
            FollowCommand();
        }
        if (state == pigeonState.resetpigeon)
        {
            ResetPigeon();
        }
        if (state == pigeonState.respawnpigeon)
        {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 6f, target.transform.position.z);
            state = pigeonState.returntoplayer;
        }
        if (state == pigeonState.sendaway)
        {
            myAnimator.SetBool("Flying", true);
            transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(Random.Range(player.transform.position.x -20f, player.transform.position.x + 20f), player.transform.position.y + 15f, gameObject.transform.position.z), 2f);
        }
    }
    private void Return2Player()
    {
        LookAtTarget();
        target = player;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, pigeonSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, player.transform.position) <= 1f)
        {
            crosshair.GetComponent<Crosshair>().thingivehit = null;
            state = pigeonState.resetpigeon;
        }
    }
    private void ImListening()
    {
        LookAtTarget();
        myAnimator.SetBool("Walk", false);
        myAnimator.SetBool("PigeonListen", true);
        gravity = 0f;
        pigeon.Move(stop);
    }
    private void ResetPigeon()
    {
        {
            LookAtTarget();
            myAnimator.SetBool("Flying", false);
            myAnimator.SetBool("Walk", false);
            myAnimator.SetBool("Jump", false);
            myAnimator.SetBool("PigeonListen", false);
            resetpigeon = false;
            state = pigeonState.followplayer;
        }
    }
    // If Pigeon is touching ground, turn flying off & remove velocity
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Carryable")
        {
            transform.localScale = new Vector3((transform.localScale.x * -1f), transform.localScale.y, transform.localScale.z);
            state = pigeonState.returntoplayer;
        }
        if (other.tag == "Player")
        {
            player = other.gameObject;
            target = player;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Interactable")
        {
            target = collision.gameObject;
        }
    }

    // Pigeon Faces Target
    private void LookAtTarget()
    {
        if (player.GetComponent<PJohns>().target != null)
        {
            target = player.GetComponent<PJohns>().target.gameObject;
        }
        else
        {
            target = player;
        }
        if (transform.position.x < target.transform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        // Look left
        else if (transform.position.x > target.transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
    // Hops in direction of PJohn
    private void FollowPlayer()
    {
        LookAtTarget();
        myAnimator.SetBool("PigeonListen", false);
        // Target Player & Draw Pathfinding in Red
        gravity = -9.81f;
        target = player;
        elapsed += Time.deltaTime;
        if (elapsed > .1f)
        {
            elapsed -= .1f;
            NavMeshHit floorhit;
            if (NavMesh.SamplePosition(transform.position, out floorhit, 7.0f, NavMesh.AllAreas))
            {
                floorposition = floorhit.position;
                Debug.DrawRay(targetPosition, Vector3.down, Color.white, 7.0f);
            }
            NavMesh.CalculatePath(floorposition, target.transform.position, NavMesh.AllAreas, path);
        }
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        Vector3 firstPointDirection = path.corners[1] - transform.position;
        firstPointDirection = firstPointDirection.normalized;
        // If close to PJOHNS, Move Towards him
        if (Vector3.Distance(transform.position, target.transform.position) > 1f && (Vector3.Distance(transform.position, target.transform.position) < 5f))
        {
            if (isTouchingGround)
            {
                hopDistance.x = 2f;
                hopDistance.z = 1f;
                myAnimator.SetBool("Walk", true);
            }
        }
        // If far from PJOHNS, jump towards him
        if (Vector3.Distance(transform.position, target.transform.position) > 5f)
        {
            if (isTouchingGround && !jumppressed)
            {
                myAnimator.SetBool("Walk", false);
                hopDistance.x = Random.Range(5f, 8f);
                hopDistance.z = 2f;
                hopDistance.y += Mathf.Sqrt(2.5f * -1.0f * gravity);
                jumppressed = true;
            }
        }
        // If at PJOHNS, stop
        if (Vector3.Distance(transform.position, target.transform.position) < 1f)
        {
            if (isTouchingGround)
            {
                myAnimator.SetBool("Flying", false);
                myAnimator.SetBool("Walk", false);
                hopDistance.x = 0f;
                hopDistance.z = 0f;
            }
        }
        if (Vector3.Distance(transform.position, target.transform.position) > 10f)
        {
            state = pigeonState.respawnpigeon;
        }
        // If on the ground stop vertical movement
        if (isTouchingGround && hopDistance.y < 0)
        {
            hopDistance.y = 0f;
            jumppressed = false;
        }
        hopDistance.y += gravity * Time.deltaTime;
        pigeonVelocity = new Vector3((firstPointDirection.x * hopDistance.x), hopDistance.y, firstPointDirection.z * hopDistance.z);
        pigeon.Move(pigeonVelocity * Time.deltaTime);
    }
    private void FlipSprite()
    {
        bool whichDirectionPlayerFacing = Mathf.Abs(pigeon.velocity.x) > Mathf.Epsilon;
        if (whichDirectionPlayerFacing)
        {
            transform.localScale = new Vector3(Mathf.Sign(pigeon.velocity.x), 1f, 1f);
        }
    }

    private void FollowCommand()
    {
        LookAtTarget();
        myAnimator.SetBool("PigeonListen", false);
        myAnimator.SetBool("Idle", false);
        myAnimator.SetBool("Walk", false);
        myAnimator.SetBool("Flying", true);
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, pigeonSpeed * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        float floorDistanceFromFoot = pigeon.stepOffset - .25f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, floorDistanceFromFoot) || pigeon.isGrounded)
        {
            Debug.DrawRay(transform.position, Vector3.down * floorDistanceFromFoot, Color.yellow);
            return true;
        }
        return false;
    }
    private void IsFlying()
    {
        if (!isTouchingGround)
        {
            myAnimator.SetBool("Flying", true);
        }
        else
        {
            myAnimator.SetBool("Flying", false);
        }
    }
    public void CooSound()
    {
        myAudio.clip = audioClips[Random.Range(0, 3)];
        myAudio.pitch = Random.Range(.9f, 1.3f);
        myAudio.Play();
        //myaudiosource.pitch = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = IsGrounded();
        IsGrounded();
        FlipSprite();
        PigeonState();
    }
}
