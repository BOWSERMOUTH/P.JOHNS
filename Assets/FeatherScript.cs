using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroySelf());
        IEnumerator DestroySelf()
        {
            yield return new WaitForSeconds(5f);
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
