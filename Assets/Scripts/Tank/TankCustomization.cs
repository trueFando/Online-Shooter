using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankCustomization : NetworkBehaviour
{
    [SyncVar(hook = "SetTankColor")]
    [SerializeField] public Color playerColor;

    [SyncVar(hook ="SetTankName")]
    [SerializeField] public string playerName;
    [SerializeField] private Text nameText;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        CmdAddMeOnServer();
    }

    private void SetTankName(string oldName, string newName)
    {
        playerName = newName;
        nameText.text = playerName;
    }

    private void SetTankColor(Color oldColor, Color newColor)
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer r in renderers)
        {
            r.material.color = newColor;
        }
    }

    [Command]
    private void CmdAddMeOnServer()
    {
        GameManager.Instance.AddPlayer(this);
    }
}
