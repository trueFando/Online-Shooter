using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShooting : NetworkBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletSpeed;

    [SerializeField] private float shootCooldown;
    private float timeFromLastShot;

    [SerializeField] LayerMask obstacleMask;

    [Command]
    private void CmdShoot(Quaternion bulletRotation)
    {
        Rigidbody bulletRB = 
            Instantiate(bullet, bulletSpawn.position, bulletRotation).GetComponent<Rigidbody>();
        bulletRB.velocity = bulletSpawn.transform.forward * bulletSpeed;
        bulletRB.GetComponent<Bullet>().Owner = GetComponent<TankController>();
        NetworkServer.Spawn(bulletRB.gameObject);
    }

    private void Update()
    {
        timeFromLastShot += Time.deltaTime;
    }

    public void Shoot(Quaternion bulletRotation)
    {
        if (timeFromLastShot >= shootCooldown) 
        {
            RaycastHit hit;
            Vector3 center = new Vector3(transform.position.x, bulletSpawn.position.y, transform.position.z);
            Vector3 direction = (bulletSpawn.position - center).normalized;
            if (Physics.SphereCast(center, 0.25f, direction, out hit, 3f, obstacleMask, QueryTriggerInteraction.Ignore))
            {
                GetComponent<TankHealth>().TakeDamage(20f, GetComponent<TankController>());
                Bullet b = Instantiate(bullet, bulletSpawn.position, Quaternion.identity).GetComponent<Bullet>();
                b.Explode();
            }
            else
            {
                CmdShoot(bulletRotation);
            }
            timeFromLastShot = 0;
        }
        
    }
}
