using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    private void footstepnow()
    {
        GameObject.Find("PJohns").GetComponent<PJohns>().FootstepAudio();
    }
}
