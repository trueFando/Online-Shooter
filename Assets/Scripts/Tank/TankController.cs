using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : NetworkBehaviour
{
    private TankHealth tankHealth;
    private TankMovement tankMovement;
    private TankShooting tankShooting;
    private Vector3 currentInput;

    private Vector3 originalSpawnPoint;
    private NetworkStartPosition[] spawnPoints;

    private int score;
    public int Score 
    { 
        get { return score; }
        set { score = value; }
    }

    public void IncreaseScore()
    {
        score++;
    }

    private void Start()
    {
        tankHealth = GetComponent<TankHealth>();
        tankMovement = GetComponent<TankMovement>();
        tankShooting = GetComponent<TankShooting>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        originalSpawnPoint = transform.position;
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
    }

    private Vector3 GetInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        return new Vector3(h, 0, v);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer || !tankHealth.IsAlive) return;

        tankMovement.MoveTank(currentInput);
    }

    private void Update()
    {
        if (!isLocalPlayer || !tankHealth.IsAlive) return;

        currentInput = GetInput();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 rot = new Vector3(-90f, 0f, tankMovement.Tower.rotation.eulerAngles.y);
            tankShooting.Shoot(Quaternion.Euler(rot));
        }

        Vector3 towerDirection = MousePositionInWorld.GetWorldPoint(Input.mousePosition,
            tankMovement.Tower.position.y) - tankMovement.Tower.position;

        tankMovement.RotateTower(-towerDirection);
        tankMovement.RotateBody(currentInput);
    }

    private void NeedToRespawn()
    {
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
        tankHealth.Reset();
        tankMovement.GetRB.velocity = Vector3.zero;
        transform.position = GetRandomSpawnPoint();
    }

    private Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints.Length > 0)
        {
            bool foundPoint = false;
            Vector3 newPoint = new Vector3();
            while (!foundPoint)
            {
                NetworkStartPosition startPos = spawnPoints[Random.Range(0, spawnPoints.Length)];
                SpawnPoint spawnPoint = startPos.GetComponent<SpawnPoint>();
                if (spawnPoint.IsOccupied == false)
                {
                    foundPoint = true;
                    newPoint = startPos.transform.position;
                }
            }
            return newPoint;
        }
            
        return originalSpawnPoint;
    }
}
