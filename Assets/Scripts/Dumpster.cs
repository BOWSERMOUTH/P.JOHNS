using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dumpster : MonoBehaviour
{
    Animator myAnimator;
    private AudioSource myAudioSource;
    public AudioClip dumpsterOpen;
    public AudioClip dumpsterClose;
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();
    }
    public void IntoDumpster()
    {
        myAnimator.SetBool("GetIn", true);
    }
    public void OutDumpster()
    {
        myAnimator.SetBool("GetIn", false);
        myAnimator.SetBool("GetOut", true);
    }
    private void OpenDumpster()
    {
        myAudioSource.pitch = (Random.Range(.7f, 1));
        myAudioSource.PlayOneShot(dumpsterOpen, 1.0f);
    }
    private void CloseDumpster()
    {
        myAudioSource.pitch = (Random.Range(.7f, 1));
        myAudioSource.PlayOneShot(dumpsterClose, 1.0f);
    }
}
