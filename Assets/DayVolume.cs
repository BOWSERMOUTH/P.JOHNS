using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayVolume : MonoBehaviour
{
    Volume volume;
    HDRISky sky;
    [SerializeField] float skyRotationAmountPerFrame = 0.1f;
    void Awake()
    {
        //volume.profile.TryGet(out sky);
    }

    // Update is called once per frame
    void Update()
    {
        //sky.rotation.value = Mathf.Lerp(sky.rotation.value, sky.rotation.value + 10, skyRotationAmountPerFrame * Time.deltaTime);
        //if (sky.rotation.value == sky.rotation.max)
        //{
            //sky.rotation.value = 0;
        //}
    }
}
