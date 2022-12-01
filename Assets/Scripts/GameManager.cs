using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [Header("Limits")]
    [SerializeField] private int minPlayers;
    [SerializeField] private int maxPlayers;
    [SyncVar]
    public int playersCount;

    [Header("UI")]
    [SerializeField] private Text messageText;

    [Header("Customization")]
    private Color[] colors = { Color.red, Color.blue, Color.yellow, Color.green, Color.black };
    private string baseName = "Player_";

    private List<TankController> allPlayers = new List<TankController>();

    [SerializeField] private List<Text> nameLabels;
    [SerializeField] private List<Text> scoreLabels;

    [SerializeField] private int maxScore = 3;
    [SyncVar]
    private bool gameOver = false;
    private TankController winner;


    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    instance = new GameObject().AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ClientRpc]
    private void RpcUpdateScoreboard(string[] playerNames, int[] playerScores)
    {
        for (int i = 0; i < playersCount; i++)
        {
            nameLabels[i].text = playerNames[i];
            scoreLabels[i].text = playerScores[i].ToString();
        }
    }

    public void UpdateScoreboard()
    {
        if (isServer)
        {
            TankController probablyWinner = GetWinner();
            if (probablyWinner != null) 
            {
                gameOver = true;
                winner = probablyWinner;
            } 

            string[] names = new string[playersCount];
            int[] scores = new int[playersCount];

            for (int i = 0; i < playersCount; i++)
            {
                names[i] = allPlayers[i].GetComponent<TankCustomization>().playerName;
                scores[i] = allPlayers[i].Score;
            }
            RpcUpdateScoreboard(names, scores);
        }
    }

    public void AddPlayer(TankCustomization tankCustomization)
    {
        if (playersCount < maxPlayers)
        {
            allPlayers.Add(tankCustomization.GetComponent<TankController>());
            tankCustomization.playerColor = colors[playersCount];
            tankCustomization.playerName = baseName + playersCount.ToString();
            playersCount++;
        }
    }

    private void Start()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(EnterLobby());
        yield return StartCoroutine(PlayGame());
        yield return StartCoroutine(EndGame());
        yield return StartCoroutine(GameLoop());
    }

    private IEnumerator EnterLobby()
    {
        while(playersCount < minPlayers)
        {
            UpdateMessage("Waiting for players");
            DisablePlayers();
            yield return null;
        }
    }

    private IEnumerator PlayGame()
    {
        UpdateMessage("3");
        yield return new WaitForSeconds(1f);
        UpdateMessage("2");
        yield return new WaitForSeconds(1f);
        UpdateMessage("1");
        yield return new WaitForSeconds(1f);
        UpdateMessage("GO GO GO");
        yield return new WaitForSeconds(1f);
        UpdateMessage("");

        EnablePlayers();
        yield return null;
        UpdateScoreboard();

        while (!gameOver)
        {
            yield return null;
        }
    }

    private IEnumerator EndGame()
    {
        DisablePlayers();
        UpdateMessage($"Game Over!\n{winner.GetComponent<TankCustomization>().playerName} wins!");
        Reset();
        yield return new WaitForSeconds(3f);
        UpdateMessage("Restarting...");
        yield return new WaitForSeconds(3f);
    }

    [ClientRpc]
    private void RpcSetPlayersState(bool state)
    {
        TankController[] allPlayers = FindObjectsOfType<TankController>();
        foreach(TankController t in allPlayers)
        {
            t.enabled = state;
        }
    }

    private void EnablePlayers()
    {
        if (isServer)
            RpcSetPlayersState(true);
    }

    private void DisablePlayers()
    {
        if (isServer)
            RpcSetPlayersState(false);
    }

    [ClientRpc]
    private void RpcUpdateMessage(string message)
    {
        messageText.text = message;
    }

    private void UpdateMessage(string message)
    {
        if (isServer)
        {
            RpcUpdateMessage(message);
        }
    }

    private TankController GetWinner()
    {
        if (isServer)
        {
            for (int i = 0; i < playersCount; i++)
            {
                if (allPlayers[i].Score >= maxScore)
                {
                    return allPlayers[i];
                }
            }
        }
        return null;
    }

    private void Reset()
    {
        if (isServer)
        {
            RpcReset();
            UpdateScoreboard();
            winner = null;
            gameOver = false;
        }
    }

    [ClientRpc]
    private void RpcReset()
    {
        TankController[] allPlayers = FindObjectsOfType<TankController>();
        foreach (TankController tankController in allPlayers)
        {
            tankController.Score = 0;
        }
    }
}
