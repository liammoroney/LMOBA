﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class BaseScript : NetworkBehaviour
{

    public Transform healthBar;
    [SyncVar]
    public float maxHealth;
    [SyncVar]
    public float healthRegen;
    [SyncVar]
    public float health;

    private void Start()
    {
        maxHealth = 200;
        healthRegen = 0.1f;
        health = 100;
    }

    private void FixedUpdate()
    {
        if (health < maxHealth)
            health += healthRegen;
    }

    private void Update()
    {
        if (health > maxHealth) health = maxHealth;
        // Healthbar
        healthBar.transform.forward = Vector3.up;
        //healthBar.transform.position = transform.position + Vector3.forward;
        healthBar.localScale = new Vector3(((health * 100) / maxHealth) * 0.005f, 0.2f, 1);
    }
}