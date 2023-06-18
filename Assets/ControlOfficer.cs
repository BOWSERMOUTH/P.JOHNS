using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class ControlOfficer : MonoBehaviour
{
    // States
    public enum controlState { Searching, Stop, Approach, Strike, }
    [SerializeField] controlState state;
    [SerializeField] List<AudioClip> clips;
    private bool startedSearching = true;
    private bool startedSpotting = false;
    private bool startedApproaching = false;
    private bool atApproachPoint = false;

    // Control Officer Components
    public NavMeshAgent myNma;
    private Animator myAnimator;
    private SpriteRenderer myspriteren;
    private AudioSource myAudio;
    public float sightrotation;
    public bool spotted = false;
    public GameObject currentTarget = null;

    // Other GameObject References
    private GameObject player;
    private GameManager gameman;
    [SerializeField] GameObject targetPosition;

    void Start()
    {
        myAnimator = gameObject.GetComponentInChildren<Animator>();
        targetPosition = GameObject.Find("TeleporterEnd");
        myspriteren = GetComponentInChildren<SpriteRenderer>();
        myAudio = gameObject.GetComponentInChildren<AudioSource>();
        gameman = GameObject.Find("GameManager").GetComponent<GameManager>();
        myNma = GetComponent<NavMeshAgent>();
        player = GameObject.Find("PJohns");
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
        // INVESTIGATING
        if (state == controlState.Approach)
        {
            // ONLY PLAYS ONCE
            if (startedApproaching)
            {
                myAudio.pitch = .3f;
                myAudio.Play();
                myAnimator.speed = .5f;
                myNma.SetDestination(targetPosition.transform.position);
                myNma.speed = 1f;
                myNma.isStopped = false;
                atApproachPoint = true;
                startedApproaching = false;
            }
            // If you get to approach place and haven't seen Pigeons, wait 3s & return to Searching
            float distance = Vector3.Distance(transform.position, targetPosition.transform.position);
            if (distance <= .3f && atApproachPoint)
            {
                myNma.isStopped = true;
                myAnimator.SetBool("Walking", false);
                StartCoroutine(SecondTimer());
                IEnumerator SecondTimer()
                {
                    yield return new WaitForSeconds(3);
                    myAudio.pitch = 1f;
                    gameman.UnSpotted();
                    startedSearching = true;
                    state = controlState.Searching;
                }
                atApproachPoint = false;
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
        LayerMask playerLayerMask = 1 << 6;
        LayerMask interLayerMask = 1 << 8;
        LayerMask mask = defaultLayerMask | playerLayerMask | interLayerMask;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        Vector3 currentPos = Vector3.back * 3f;
        sightrotation += 520 * Time.deltaTime;
        if (sightrotation >= 180)
        {
            sightrotation = 0f;
        }
        currentPos = Quaternion.Euler(0, sightrotation, 0) * currentPos;
        Debug.DrawRay(rayOrigin, currentPos, Color.green);
        // If the raycast hits Player, go to SPOTTED mode. 
        if (Physics.Raycast(rayOrigin, currentPos, out hit, 3f, mask))
        {
            if (hit.transform.tag == "Pigeon")
            {
                spotted = true;
                currentTarget = hit.transform.gameObject;
                gameman.Spotted();
                myAudio.Play();
                startedSpotting = true;
                state = controlState.Stop;
            }
        }
        else
        {
            spotted = false;
        }
    }
}
