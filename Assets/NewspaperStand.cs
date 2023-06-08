using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewspaperStand : MonoBehaviour
{
    public float currentHealth;
    public ObjectHealth objecthealth;
    private Animator myanim;


    // Start is called before the first frame update
    void Start()
    {
        myanim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = objecthealth.objectCurrentHealth;

    }
}
