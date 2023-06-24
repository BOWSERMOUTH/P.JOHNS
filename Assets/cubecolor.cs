using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubecolor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pigeon")
        {
            print("stupid ass cube hit the thing");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
