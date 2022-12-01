using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : NetworkBehaviour
{
    [SyncVar(hook ="UpdateHealthbar")]
    private float currentHealth;

    [SerializeField] private float maxHealth;

    [SyncVar]
    private bool isAlive = true;
    public bool IsAlive => isAlive;

    [SerializeField] private GameObject deathFX;

    [SerializeField] private Slider healthbar;

    private void Start()
    {
        Reset();
    }

    public void TakeDamage(float damage, TankController killer)
    {
        if (!isServer) return;

        currentHealth -= damage; 
        if (currentHealth <= 0)
        {
            if (GetComponent<TankController>() != killer)
            {
                killer.IncreaseScore();
                GameManager.Instance.UpdateScoreboard();
            }
            RpcDie();
        }
    }

    [ClientRpc]
    private void RpcDie()
    {
        isAlive = false;
        if (deathFX != null)
        {
            ParticleSystem fx = 
                Instantiate(deathFX, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            fx.Play();
            Destroy(fx.gameObject, 3f);
        }

        SetActiveState(false);

        gameObject.SendMessage("NeedToRespawn");
    }

    private void SetActiveState(bool state)
    {
        GetComponent<SphereCollider>().enabled = state;
        foreach(Renderer mr in GetComponentsInChildren<Renderer>())
        {
            mr.enabled = state;
        }
        foreach(Canvas c in GetComponentsInChildren<Canvas>())
        {
            c.enabled = state;
        }
    }
    private void UpdateHealthbar(float oldValue, float newValue)
    {
        healthbar.value = Mathf.Clamp(newValue, 0f, maxHealth);
    }

    public void Reset()
    {
        currentHealth = maxHealth;
        SetActiveState(true);
        isAlive = true;
    }
}
