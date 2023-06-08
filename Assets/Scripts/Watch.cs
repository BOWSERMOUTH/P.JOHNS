using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Watch : MonoBehaviour
{
    private float timeofday;
    private int fifteenminincrements;
    //Watch Component References
    public GameObject colon;
    private TMP_Text watchText;
    private AudioSource myAudioSource;
    private Animator myAnimator;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Blink");
        watchText = GameObject.Find("TimeText").GetComponent<TMP_Text>();
        myAudioSource = gameObject.GetComponent<AudioSource>();
        myAnimator = gameObject.GetComponent<Animator>();
    }
    IEnumerator Blink()
    {
        yield return new WaitForSeconds(1);
        colon.SetActive(false);
        yield return new WaitForSeconds(1);
        colon.SetActive(true);
        StartCoroutine("Blink");
    }
    public void WatchSound()
    {
        myAnimator.SetBool("ShowWatch", true);
        myAudioSource.Play();
        StartCoroutine("FinishAnimation");
    }
    IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(3.5f);
        myAnimator.SetBool("ShowWatch", false);
    }
    private void SendToWatchFace()
    {
        fifteenminincrements = GameObject.Find("GameManager").GetComponent<GameManager>().fifteenminincrements;
        switch (fifteenminincrements)
        {
            case 0:
                watchText.SetText("09 00");
                break;
            case 1:
                watchText.SetText("09 15");
                break;
            case 2:
                watchText.SetText("09 30");
                break;
            case 3:
                watchText.SetText("09 45");
                break;
            case 4:
                watchText.SetText("10 00");
                break;
            case 5:
                watchText.SetText("10 15");
                break;
            case 6:
                watchText.SetText("10 30");
                break;
            case 7:
                watchText.SetText("10 45");
                break;
            case 8:
                watchText.SetText("11 00");
                break;
            case 9:
                watchText.SetText("11 15");
                break;
            case 10:
                watchText.SetText("11 30");
                break;
            case 11:
                watchText.SetText("11 45");
                break;
            case 12:
                watchText.SetText("12 00");
                break;
            case 13:
                watchText.SetText("12 15");
                break;
            case 14:
                watchText.SetText("12 30");
                break;
            case 15:
                watchText.SetText("12 45");
                break;
            case 16:
                watchText.SetText("01 00");
                break;
            case 17:
                watchText.SetText("01 15");
                break;
            case 18:
                watchText.SetText("01 30");
                break;
            case 19:
                watchText.SetText("01 45");
                break;
            case 20:
                watchText.SetText("02 00");
                break;
            case 21:
                watchText.SetText("02 15");
                break;
            case 22:
                watchText.SetText("02 30");
                break;
            case 23:
                watchText.SetText("02 45");
                break;
            case 24:
                watchText.SetText("03 00");
                break;
            case 25:
                watchText.SetText("03 15");
                break;
            case 26:
                watchText.SetText("03 30");
                break;
            case 27:
                watchText.SetText("03 45");
                break;
            case 28:
                watchText.SetText("04 00");
                break;
            case 29:
                watchText.SetText("04 15");
                break;
            case 30:
                watchText.SetText("04 30");
                break;
            case 31:
                watchText.SetText("04 45");
                break;
            case 32:
                watchText.SetText("05 00");
                break;
            case 33:
                watchText.SetText("05 15");
                break;
            case 34:
                watchText.SetText("05 30");
                break;
            case 35:
                watchText.SetText("05 45");
                break;
            case 36:
                watchText.SetText("06 00");
                break;
            case 37:
                watchText.SetText("06 15");
                break;
            case 38:
                watchText.SetText("06 30");
                break;
            case 39:
                watchText.SetText("06 45");
                break;
            case 40:
                watchText.SetText("07 00");
                break;
            case 41:
                watchText.SetText("07 15");
                break;
            case 42:
                watchText.SetText("07 30");
                break;
            case 43:
                watchText.SetText("07 45");
                break;
            case 44:
                watchText.SetText("08 00");
                break;
            case 45:
                watchText.SetText("08 15");
                break;
            case 46:
                watchText.SetText("08 30");
                break;
            case 47:
                watchText.SetText("08 45");
                break;
            case 48:
                fifteenminincrements = 0;
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        SendToWatchFace();
    }
}
