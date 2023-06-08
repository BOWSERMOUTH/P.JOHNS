using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popo : MonoBehaviour
{
    private Animator myanim;
    public AudioSource myaudio;
    public int currentanimation;
    // Start is called before the first frame update
    void Start()
    {
        myaudio = GetComponent<AudioSource>();
        myanim = GetComponentInChildren<Animator>();
        RotateAnimations();
    }
    private void RotateAnimations()
    {
        if (currentanimation == 0)
        {
            BoolsOff();
            StartCoroutine(RandomWaitTime());
        }
        if (currentanimation == 1)
        {
            BoolsOff();
            myanim.SetBool("Walking", true);
            StartCoroutine(RandomWaitTime());
        }
        if (currentanimation == 2)
        {
            BoolsOff();
            myanim.SetBool("Running", true);
            StartCoroutine(RandomWaitTime());
        }
        if (currentanimation == 3)
        {
            BoolsOff();
            myanim.SetBool("Bark", true);
            myaudio.Play();
            //StartCoroutine(RandomWaitTime());
        }
    }
    private void BoolsOff()
    {
        myanim.SetBool("Walking", false);
        myanim.SetBool("Running", false);
        myanim.SetBool("Bark", false);
    }
    IEnumerator RandomWaitTime()
    {
        yield return new WaitForSeconds(Random.Range(5f, 5f));
        currentanimation = 3;
        //currentanimation = Random.Range(0, 3);
        RotateAnimations();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
