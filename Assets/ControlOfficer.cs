using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

public class ControlOfficer : MonoBehaviour
{
    // States
    public enum controlState { Searching, Spot, Approach, Strike, }
    [SerializeField] controlState state;
    [SerializeField] List<AudioClip> clips;
    private bool startedSearching = true;
    private bool startedSpotting = false;
    private bool startedApproaching = false;
    private bool reachingApproach = false;
    private bool atApproachPoint = false;

    // Control Officer Components
    public NavMeshAgent myNma;
    private Animator myAnimator;
    private SpriteRenderer myspriteren;
    private AudioSource myAudio;
    public AudioSource walkAudio;
    public float sightrotation;
    public bool spotted = false;
    public GameObject currentTarget = null;
    private Vector3 offsetdistance;
    private int whichway;
    public SphereCollider spherecollider;
    private GameObject searchlight;
    public GameObject caughtbirdparticle;

    // Other GameObject References
    private GameObject player;
    private GameManager gameman;
    [SerializeField] GameObject targetPosition;

    void Start()
    {
        gameman = GameObject.Find("GameManager").GetComponent<GameManager>();
        myAnimator = gameObject.GetComponentInChildren<Animator>();
        targetPosition = GameObject.Find("TeleporterEnd");
        myspriteren = GetComponentInChildren<SpriteRenderer>();
        myAudio = gameObject.GetComponent<AudioSource>();
        myNma = GetComponent<NavMeshAgent>();
        player = GameObject.Find("PJohns");
        searchlight = GameObject.Find("SearchLight");
    }
    private void ControlOfficerState()
    {
        // SEARCHING
        if (state == controlState.Searching)
        {
            // ONLY HAPPENS ONCE
            if (startedSearching)
            {
                // ControlOfficer is on the move to destination
                walkAudio.Play();
                myNma.isStopped = false;
                myAnimator.SetBool("Walking", true);
                myAnimator.speed = 1f;
                myNma.speed = 1.65f;
                // Set Destination To TeleporterEnd
                myNma.SetDestination(targetPosition.transform.position);
                startedSearching = false;
            }
            // Police Audio Matches PoliceAmbience Audio
            myAudio.time = gameman.GetComponent<AudioSource>().time;
            if (!myAudio.isPlaying)
            {
                myAudio.pitch = 1f;
                myAudio.Play();
            }
            // If Police is at TeleporterEnd, destroy self
            float distance = Vector3.Distance(transform.position, targetPosition.transform.position);
            if (distance <= .3f)
            {
                myAnimator.SetBool("Walking", false);
                gameman.lowervolume = true;
                Destroy(gameObject);
            }
            // 3 fanned out Rays looking for PJohns
            GeneralSight();
        }
        // SPOT
        if (state == controlState.Spot)
        {
            if (spotted)
            {
                myAnimator.SetBool("Walking", false);
                myNma.isStopped = true;
                myAudio.PlayOneShot(clips[0], 1f);
                StartCoroutine(SpottedAnimation());
                IEnumerator SpottedAnimation()
                {
                    searchlight.transform.LookAt(currentTarget.transform);
                    myAnimator.SetBool("Prepare", true);
                    yield return new WaitForSeconds(1.2f);
                    myAnimator.SetBool("Prepare", false);
                    startedApproaching = true;
                    state = controlState.Approach;
                }
                spotted = false;
            }
        }
        // INVESTIGATING
        if (state == controlState.Approach)
        {
            searchlight.transform.LookAt(currentTarget.transform);
            // ONLY PLAYS ONCE
            if (startedApproaching)
            {
                if (transform.position.x > currentTarget.transform.position.x)
                {
                    whichway = 1;
                }
                else
                {
                    whichway = -1;
                }
                offsetdistance = new Vector3((currentTarget.transform.position.x + (1f * whichway)), currentTarget.transform.position.y, currentTarget.transform.position.z);
                myNma.isStopped = false;
                myAnimator.SetBool("Walking", true);
                myNma.SetDestination(offsetdistance);
                myNma.speed = .7f;
                myAnimator.speed = myNma.speed;
                reachingApproach = true;
                startedApproaching = false;
                walkAudio.Play();
            }
            // If you get to approach place and haven't seen Pigeons, wait 3s & return to Searching
            float distance = Vector3.Distance(transform.position, offsetdistance);
            if (distance <= .2f && reachingApproach)
            {
                myNma.isStopped = true;
                myAnimator.SetBool("Walking", false);
                StartCoroutine(SecondTimer());
                reachingApproach = false;
                IEnumerator SecondTimer()
                {
                    searchlight.transform.LookAt(currentTarget.transform);
                    walkAudio.Stop();
                    myAnimator.speed = 1;
                    yield return new WaitForSeconds(1.5f);
                    myAnimator.SetBool("Strike", true);
                    yield return new WaitForSeconds(.3f);
                    myAudio.PlayOneShot(clips[Random.Range(1, 3)], 1f);
                    SwingSound();
                    yield return new WaitForSeconds(2f);
                    myAnimator.SetBool("Strike", false);
                    if (currentTarget == null)
                    {
                        startedSearching = true;
                        searchlight.transform.rotation = Quaternion.Euler((25.663f * whichway), (90 * transform.localScale.x), 0);
                        state = controlState.Searching;
                    }
                    else
                    {
                        startedApproaching = true;
                    }
                }
            }
        }
        // CAUGHT
        if (state == controlState.Strike)
        {

        }
    }
    private void GeneralSight()
    {
        // Creates 3 Raycasts fanning out in front of him
        RaycastHit hit;
        LayerMask defaultLayerMask = 1 << 0;
        LayerMask pigeonLayerMask = 1 << 16;
        LayerMask interLayerMask = 1 << 8;
        LayerMask mask = defaultLayerMask | pigeonLayerMask | interLayerMask;
        Vector3 rayAtAnkle = new Vector3(transform.position.x, transform.position.y + .3f, transform.position.z);
        Vector3 rayAtFeet = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        // Ray Length & Direction
        Vector3 currentPos = Vector3.back * 3f;
        Vector3 currentDiagPos = new Vector3(transform.localScale.x, 0,0) * 3f;
        sightrotation += 800 * Time.deltaTime;
        if (sightrotation >= 360)
        {
            sightrotation = 0f;
        }
        currentPos = Quaternion.Euler(0, sightrotation, 0) * currentPos;
        currentDiagPos = Quaternion.Euler(0, -sightrotation, 45) * currentDiagPos;
        Debug.DrawRay(rayAtAnkle, currentPos, Color.green);
        Debug.DrawRay(rayAtFeet, currentDiagPos, Color.blue);
        // If the raycast hits Player, go to SPOTTED mode. 
        if (Physics.Raycast(rayAtAnkle, currentPos, out hit, 4f, mask))
        {
            if (hit.transform.tag == "Pigeon")
            {
                spotted = true;
                currentTarget = hit.transform.gameObject;
                startedSpotting = true;
                state = controlState.Spot;
            }
        }
        else if (Physics.Raycast(rayAtFeet, currentDiagPos, out hit, 4f, mask))
        {
            if (hit.transform.tag == "Pigeon")
            {
                spotted = true;
                currentTarget = hit.transform.gameObject;
                startedSpotting = true;
                state = controlState.Spot;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pigeon")
        {
            Instantiate(caughtbirdparticle, new Vector3(other.transform.position.x, other.transform.position.y + .3f, other.transform.position.z), Quaternion.identity);
            Destroy(other.gameObject);
        }
    }
    private void FlipSprite()
    {
        bool whichDirectionPlayerFacing = myNma.velocity.x > 0f;
        if (whichDirectionPlayerFacing)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    public void FootStep()
    {
        myAudio.PlayOneShot(clips[3], 1f);
    }
    public void WalkJingle()
    {
        walkAudio.Play();
    }
    public void SwingSound()
    {
        myAudio.PlayOneShot(clips[Random.Range(5, 7)], 1f);
    }
    public void ControlOfficerHit()
    {
        myAudio.PlayOneShot(clips[7], 1f);
    }
    void Update()
    {
        ControlOfficerState();
        FlipSprite();
    }
}
