using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime;
    private float currentLifetime;

    [SerializeField] private float damage;
    [SerializeField] private GameObject explosionFX;

    private TankController owner;
    public TankController Owner 
    { 
        get { return owner; } 
        set { owner = value; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TankHealth health = collision.gameObject.GetComponent<TankHealth>();
        if (health != null)
        {
            health.TakeDamage(damage, owner);
            Explode();
        }
    }

    void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifetime) Explode();
    }

    public void Explode()
    {
        if (explosionFX != null) 
        {
            ParticleSystem fx = 
                Instantiate(explosionFX, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            fx.Play();
            Destroy(fx.gameObject, 1f);
        }
        Destroy(gameObject);
    }
}
