﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetworkedPlayerScript : NetworkBehaviour
{
    public ShootingScript shootingScript;
    public GameObject skin;
    public TextMesh nameTag;
    public Transform healthBar;
    private Transform healthBarHUD;
    private Transform manaBarHUD;

    public Canvas tabCanvas;

    Text playersList;

    Renderer[] renderers;

    [SyncVar]
    public string playerName;
    [SyncVar]
    public Color playerColour;
    [SyncVar]
    public float health;
    [SyncVar]
    public float mana;

    [SyncVar]
    public int playerCount;

    [SyncVar]
    public string names;

    public SyncListString namesList;
    public Vector3 spawnPoint;

    public Camera p_Camera;
    [HideInInspector]
    public Camera myCamera;

    //[HideInInspector]
    //[SyncVar]
    public NetworkInstanceId id;
    //[SyncVar]
    public float idfloat;

    public bool isAI;

    public Material outlineMaterial;
    private Material originalMaterial;
    public Renderer myRenderer;

    private GameManager gManager;

    void Awake()
    {

        renderers = GetComponentsInChildren<Renderer>();
        health = 100;
        mana = 100;

        healthBarHUD = GameObject.Find("HealthHUD").GetComponent<Transform>();
        manaBarHUD = GameObject.Find("ManaHUD").GetComponent<Transform>();

        namesList = new SyncListString();

        spawnPoint = transform.position;

        originalMaterial = myRenderer.material;

        tabCanvas = GameObject.Find("TabCanvas").GetComponent<Canvas>();
        tabCanvas.enabled = false;

        gManager = GameObject.Find("Server").GetComponent<GameManager>();
        if(isAI)
            gManager.playerList.Add("AI player");
        else
            gManager.playerList.Add("Player");
    }

    public void Select()
    {
        myRenderer.material = outlineMaterial;
    }

    public void DeSelect()
    {
        myRenderer.material = originalMaterial;
    }

    private void FixedUpdate()
    {
        if (health < 100)
            health += 0.01f;
        if (mana < 100)
            mana += 0.05f;
    }

    void Update()
    {
        // Set player name and color of model
        nameTag.text = playerName;
        skin.GetComponent<Renderer>().material.color = playerColour;

        // Healthbar
        healthBar.transform.forward = Vector3.up;
        healthBar.transform.position = transform.position + Vector3.forward;
        healthBar.localScale = new Vector3(health * 0.005f, 0.2f, 1);

        if (isLocalPlayer)
        {
            healthBarHUD.localScale = new Vector3(health * 0.01f, 0.4f, 2);
            manaBarHUD.localScale = new Vector3(mana * 0.01f, 0.4f, 2);
        }

        if (namesList != null && false)
        {
            if (namesList.Count == 0)
                playersList.text = "Players: " + playerCount.ToString();
            if (namesList.Count == 1)
                playersList.text = "Players: " + playerCount.ToString() + "\n" + namesList[0].ToString();
            if (namesList.Count == 2)
                playersList.text = "Players: " + playerCount.ToString() + "\n" + namesList[0].ToString() + "\n" + namesList[1].ToString();
            if (namesList.Count == 3)
                playersList.text = "Players: " + playerCount.ToString() + "\n" + namesList[0].ToString() + "\n" + namesList[1].ToString() + "\n" + namesList[2].ToString();
            if (namesList.Count == 4)
                playersList.text = "Players: " + playerCount.ToString() + "\n" + namesList[0].ToString() + "\n" + namesList[1].ToString() + "\n" + namesList[2].ToString() + "\n" + namesList[3].ToString();
        }
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                tabCanvas.enabled = true;
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                tabCanvas.enabled = false;
            }
        }
    }

    [Command]
    void CmdSendName(string _playerName)
    {
        //Debug.Log("_playerNameStart: " + _playerName.ToString());
        GetComponent<ServerScript>().RpcUpdateCount(_playerName);
    }

    public override void OnStartLocalPlayer()
    {
        //fpsController.enabled = true;
        shootingScript.enabled = true;
        //candyMaterialSwitcher.SwitchMaterial(true);

        gameObject.name = "LOCAL Player";

        playerName = "Liam";

        //Debug.Log("playerNameStart: " + playerName.ToString());
        CmdSendName(playerName);
        //Debug.Log("creating camera");
        myCamera = Instantiate(p_Camera);
        myCamera.GetComponent<CameraController>().SetPlayer(transform);

        id = netId;
        idfloat = netId.Value;

        base.OnStartLocalPlayer();        
    }

    void ToggleRenderer(bool isAlive)
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = isAlive;
    }

    void ToggleControls(bool isAlive)
    {
        shootingScript.enabled = isAlive;
    }

    [ClientRpc]
    public void RpcResolveHit(float damage)
    {
        //Debug.Log("ouch");
        health -= damage;
        if (health <= 0)
        {
            //ToggleRenderer(false);
            transform.position = spawnPoint;
            gameObject.SetActive(false);

            if (isLocalPlayer)
            {
                Transform spawn = NetworkManager.singleton.GetStartPosition();
                transform.position = spawn.position;
                transform.rotation = spawn.rotation;

                ToggleControls(false);
            }
            Invoke("Respawn", 2f);
        }
    }

    void Respawn()
    {
        //ToggleRenderer(true);
        gameObject.SetActive(true);
        health = 100;

        if (isLocalPlayer)
        {
            ToggleControls(true);
        }
    }
}