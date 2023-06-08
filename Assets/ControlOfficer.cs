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
    private bool startSearching = true;

    // Control Officer Components
    public NavMeshAgent myNma;

    void Start()
    {
    }
    private void ControlOfficerState()
    {
        // SEARCHING
        if (state == controlState.Searching)
        {
            // ONLY HAPPENS ONCE
            if (startSearching)
            {
                // ControlOfficer is on the move to destination
                myNma.isStopped = false;
                myAnimator.SetBool("Walking", true);
                myAnimator.speed = 1f;
                myNma.speed = 1.65f;
                // Set Destination To TeleporterEnd
                myNma.SetDestination(targetPosition.transform.position);
                startSearching = false;
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
        // SPOTTED
        if (state == policeState.Spotted)
        {
            // ONLY HAPPENS ONCE
            if (startedSpotting)
            {
                // If spotted, stop PoliceForce audio and stop walking
                myAudio.Pause();
                myNma.isStopped = true;
                myAnimator.SetBool("Walking", false);
                startedSpotting = false;
            }
            if (spotted)
            {
                StartCoroutine(SpottingTimer());
                IEnumerator SpottingTimer()
                {
                    spotted = false;
                    yield return new WaitForSeconds(3);
                    myAnimator.SetBool("Walking", true);
                    startedInvestigating = true;
                    state = policeState.Investigating;
                }
            }
            // Shoots a raycast that always looks at PJohns, but will hit Default and Interactable objects inbetween. 
            HonedSight();
        }
        // INVESTIGATING
        if (state == policeState.Investigating)
        {
            // ONLY PLAYS ONCE
            if (startedInvestigating)
            {
                myAudio.pitch = .3f;
                myAudio.Play();
                myAnimator.speed = .5f;
                myNma.SetDestination(lastSpottedPoint);
                myNma.speed = 1f;
                myNma.isStopped = false;
                atInvestigationPoint = true;
                startedInvestigating = false;
            }
            if (spotlightTime >= 3f)
            {
                myAudio.pitch = 1f;
                myAudio.PlayOneShot(clips[0], 1f);
                state = policeState.Caught;
            }
            // If you get to investigation place and haven't seen PJohns, wait 3s & return to Searching
            float distance = Vector3.Distance(transform.position, lastSpottedPoint);
            if (distance <= .3f && atInvestigationPoint)
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
                    state = policeState.Searching;
                }
                atInvestigationPoint = false;
            }
            HonedSight();
        }
        // CAUGHT
        if (state == policeState.Caught)
        {
            myAnimator.SetBool("Whistle", true);
            myNma.SetDestination(player.transform.position);
            spotlightTime = 0f;
        }
    }
}
