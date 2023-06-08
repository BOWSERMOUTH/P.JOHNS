using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    public float pigeonmultiplier;
    public float maxHealth = 100f;
    public float objectCurrentHealth;
    public Transform healthbartransform;
    public Animator healthbaranim;

    public HealthManager healthBar;

    void Start()
    {
        objectCurrentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthbartransform = GetComponentInChildren<HealthManager>().transform;
        healthbartransform.localScale = new Vector3(0f, 0f, 0f);
        healthbaranim = healthbartransform.GetComponent<Animator>();
    }
    void Update()
    {
        healthBar.SetHealth(objectCurrentHealth);
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Pigeon")
        {
            pigeonmultiplier++;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Pigeon")
        {
            TakeDamage();
            healthbartransform.localScale = new Vector3(0.6818f, 0.6818f, 0.6818f);
            healthbaranim.SetBool("DamageShake", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Pigeon")
        {
            pigeonmultiplier--;
            healthbartransform.localScale = new Vector3(0f, 0f, 0f);
            healthbaranim.SetBool("DamageShake", false);
        }
    }
    private void TakeDamage()
    {
        objectCurrentHealth -= Time.deltaTime * pigeonmultiplier;
    }
}
