﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShootingScript : NetworkBehaviour
{
    public ParticleSystem _muzzleFlash;
    public AudioSource _gunAudio;
    public GameObject _impactPrefab;
    public Transform cameraTransform;

    //[HideInInspector]
    public bool isCastingSpell = false;
    private bool isThrowingFireball = false;

    public GameObject p_fireball;
    public GameObject p_fire;
    public Transform t_shoot;
    private Text fireballTimer;
    [HideInInspector]
    public float fireballTimeValue = 0;
    private RawImage fireballIcon;

    public GameObject p_AutoAttack;

    private GameObject fire;
    ParticleSystem.Particle[] m_Particles;
    public float fireScale { get; private set ;}
    private GameObject fireball;

    private float lastAutoTime = 0;
    private float autoDelay = 1;
    [HideInInspector]
    public Vector3 autoTarget;
    [HideInInspector]
    public float autoRange { get; private set; }
    //[HideInInspector]
    public Transform target = null;
    //[HideInInspector]
    public bool isCasting = false;

    public GameObject p_Lightning;
    private Text lightningTimer;
    private RawImage lightningIcon;
    private float lightningStartTime;
    private float lightningCastTime = 1;
    private bool lightningCasting = false;
    GameObject lightning;
    Transform lightningTarget;

    public GameObject p_BlinkLight;
    private Text blinkTimer;
    private RawImage blinkIcon;
    private float blinkTimeValue = 0;

    public MeshRenderer attackRangeMesh;

    public MeshRenderer orbPullMesh;
    private Text orbPullTimer;
    private RawImage orbPullIcon;
    private float orbPullRange = 15;
    public float orbPullStrength = 1;

    public GameObject p_Ultimate;
    private Text ultimateTimer;
    private RawImage ultimateIcon;
    private float ultimateTimeValue = 0;

    private void Awake()
    {
        ultimateTimer = GameObject.Find("UltimateTimer").GetComponent<Text>();
        ultimateIcon = GameObject.Find("UltimateIcon").GetComponent<RawImage>();

        blinkTimer = GameObject.Find("BlinkTimer").GetComponent<Text>();
        blinkIcon = GameObject.Find("BlinkIcon").GetComponent<RawImage>();

        fireballTimer = GameObject.Find("FireballTimer").GetComponent<Text>();
        fireballIcon = GameObject.Find("FireballIcon").GetComponent<RawImage>();

        lightningTimer = GameObject.Find("LightningTimer").GetComponent<Text>();
        lightningIcon = GameObject.Find("LightningIcon").GetComponent<RawImage>();
        lightningTimer.enabled = false;

        orbPullTimer = GameObject.Find("OrbPullTimer").GetComponent<Text>();
        orbPullIcon = GameObject.Find("OrbPullIcon").GetComponent<RawImage>();
        orbPullTimer.enabled = false;

        autoRange = 10;

        if (!isLocalPlayer)
            return;

        attackRangeMesh.enabled = false;
        orbPullMesh.enabled = false;
    }

    public override void OnStartLocalPlayer()
    {

        attackRangeMesh.enabled = false;
        orbPullMesh.enabled = false;
    }

    void Update()
    {
        t_shoot.transform.forward = transform.forward;

        if (isLocalPlayer) ultimateTimer.text = (Mathf.Ceil(ultimateTimeValue)).ToString();
        if (ultimateTimeValue > 0)
        {
            ultimateTimeValue -= Time.deltaTime;
            if (isLocalPlayer)
            {
                ultimateTimer.enabled = true;
                ultimateIcon.color = new Color(0.1f, 0.1f, 0.1f, 1);
            }
        }
        else
        {
            if (isLocalPlayer)
            {
                ultimateIcon.color = new Color(1, 1, 1, 1);
                ultimateTimer.enabled = false;
            }
        }

        if (isLocalPlayer)  blinkTimer.text = (Mathf.Ceil(blinkTimeValue)).ToString();
        if (blinkTimeValue > 0)
        {
            blinkTimeValue -= Time.deltaTime;
            if (isLocalPlayer)
            {
                blinkTimer.enabled = true;
                blinkIcon.color = new Color(0.1f, 0.1f, 0.1f, 1);
            }
        }
        else
        {
            if (isLocalPlayer)
            {
                blinkIcon.color = new Color(1, 1, 1, 1);
                blinkTimer.enabled = false;
            }
        }

        if (isLocalPlayer) fireballTimer.text = (Mathf.Ceil(fireballTimeValue)).ToString();
        if (fireballTimeValue > 0)
        {
            fireballTimeValue -= Time.deltaTime;
            if (isLocalPlayer)
            {
                fireballTimer.enabled = true;
                fireballIcon.color = new Color(0.1f, 0.1f, 0.1f, 1);
            }
        }
        else
        {
            if (isLocalPlayer)
            {
                fireballIcon.color = new Color(1, 1, 1, 1);
                fireballTimer.enabled = false;
            }
        }

        if (lightningCasting)
        {
            if(Time.time > lightningStartTime + lightningCastTime)
            {
                Destroy(lightning);
                lightningCasting = false;
            }
            else
            {
                lightning.transform.FindChild("LightningStart").transform.position = transform.position;
                lightning.transform.FindChild("LightningEnd").transform.position = lightningTarget.position;
                lightningTarget.GetComponent<NetworkedPlayerScript>().RpcResolveHit(0.25f);
                GetComponent<NetworkedPlayerScript>().mana -= 0.2f;
            }
        }


        if (!isLocalPlayer)
            return;

        attackRangeMesh.transform.localScale = new Vector3(autoRange * 0.226f, 1, autoRange * 0.226f);
        
        if (Input.GetMouseButtonDown(0))
        {
            // Attack move
            if (attackRangeMesh.enabled)
            {
                // Find closest enemy that we can see
                RaycastHit hit;
                if (Physics.Raycast(GetComponent<NetworkedPlayerScript>().myCamera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Transform closestEnemy = null;
                    float closestDistance = 21;
                    Collider[] hitColliders = Physics.OverlapSphere(hit.point, 20);
                    for (int i = 0; i < hitColliders.Length; i++)
                    {
                        GameObject temp = hitColliders[i].gameObject;
                        if (temp.GetComponent<NetworkedPlayerScript>() && temp.GetComponent<NetworkedPlayerScript>().netId.Value != GetComponent<NetworkedPlayerScript>().netId.Value)
                        {
                            float distance = Vector3.Distance(transform.position, temp.transform.position);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestEnemy = temp.transform;
                            }
                        }
                    }
                    if (closestEnemy)
                    {
                        target = closestEnemy.transform;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            attackRangeMesh.enabled = true;
        }
        else if (Input.anyKeyDown)
        {
            attackRangeMesh.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!lightningCasting)
            {
                RaycastHit hit;
                if (Physics.Raycast(GetComponent<NetworkedPlayerScript>().myCamera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.transform.GetComponent<NetworkedPlayerScript>() && hit.transform.GetComponent<NetworkedPlayerScript>().netId != netId)
                    {
                        CreateLightning(hit.transform);
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateFire();
        }
        if (Input.GetKey(KeyCode.Q))
        {
            RaycastHit hit;
            if (Physics.Raycast(GetComponent<NetworkedPlayerScript>().myCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                GetComponent<PlayerMovement>().rotationTarget = hit.point;
                //Debug.DrawLine(transform.position, hit.point);
            }
            IncreaseFire();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            RaycastHit hit;
            if (Physics.Raycast(GetComponent<NetworkedPlayerScript>().myCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Vector3 pos = (new Vector3(hit.point.x, t_shoot.transform.position.y, hit.point.z));
                CreateFireBall(pos);
            }
        }
        // Flash
        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit hit;
            if (Physics.Raycast(GetComponent<NetworkedPlayerScript>().myCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Vector3 direction = (hit.point - transform.position);
                Blink(direction);
            }
        }
        // Ultimate
        if (Input.GetKeyDown(KeyCode.R))
        {
            RaycastHit hit;
            if (Physics.Raycast(GetComponent<NetworkedPlayerScript>().myCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                StartCoroutine(FireFancy(hit.point));
                
                //Instantiate(p_Ultimate, t_shoot.position, t_shoot.rotation, null);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            // Set target
            RaycastHit hit;
            if (Physics.Raycast(GetComponent<NetworkedPlayerScript>().myCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.transform.GetComponent<NetworkedPlayerScript>() && hit.transform.GetComponent<NetworkedPlayerScript>().netId != netId
                    && hit.transform.GetComponent<NetworkedPlayerScript>().team != GetComponent<NetworkedPlayerScript>().team)
                {
                    target = hit.transform;
                }
                else if (hit.transform.GetComponent<BaseScript>())
                {
                    target = hit.transform;
                }
                else if (hit.transform.GetComponent<TowerScript>())
                {
                    target = hit.transform;
                }
            }
        }
        if(target)
        {
            if (Vector3.Distance(transform.position, target.position) < autoRange)
            {
                AutoAttack(target);
                autoTarget = target.position;
                StartCoroutine(StopCasting(0.5f));
                target = null;
            }
            else
            {
                // If we are not in range to auto attack, move closer to target
                //GetComponent<PlayerMovement>().waypoint = target.position;
            }
        }
        if (Input.GetKey(KeyCode.E))
        {
            GetComponent<NetworkedPlayerScript>().mana -= 1;
            orbPullMesh.transform.localScale = new Vector3(orbPullRange * 0.226f, 1, orbPullRange * 0.226f);
            orbPullMesh.enabled = true;
            List<Transform> players = new List<Transform>();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, orbPullRange);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                GameObject temp = hitColliders[i].gameObject;
                if (temp.GetComponent<NetworkedPlayerScript>() && temp.GetComponent<NetworkedPlayerScript>().netId.Value != GetComponent<NetworkedPlayerScript>().netId.Value
                    && temp.GetComponent<NetworkedPlayerScript>().team != GetComponent<NetworkedPlayerScript>().team)
                {
                    players.Add(temp.transform);
                }
            }
            for(int i = 0; i < players.Count; i++)
            {
                Vector3 dir = Vector3.MoveTowards(players[i].transform.position, transform.position, 25 * orbPullStrength * Time.deltaTime) - players[i].transform.position;
                players[i].GetComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);
            }
        }
        else
        {
            orbPullMesh.enabled = false;
        }
    }

    public IEnumerator FireFancy(Vector3 point)
    {
        if (ultimateTimeValue <= 0 && !isCastingSpell)
        {
            isCastingSpell = true;
            if (GetComponent<NetworkedPlayerScript>().anim)
            {
                GetComponent<NetworkedPlayerScript>().anim.SetTrigger("AutoAttack");
            }

            GetComponent<PlayerMovement>().rotationTarget = point;
            // Wait until fully rotated and animation is finished until we fire spell
            yield return new WaitForSeconds(0.25f);
            GameObject autoAttack = Instantiate(p_Ultimate, t_shoot.position, t_shoot.rotation, null);
            autoAttack.GetComponent<FancyScript>().damage = GetComponent<NetworkedPlayerScript>().attackDamge;
            autoAttack.GetComponent<FancyScript>().creator = GetComponent<NetworkedPlayerScript>();
            ultimateTimeValue = 5;
            isCastingSpell = false;
        }
    }

    public void Blink(Vector3 direction)
    {
        if (blinkTimeValue <= 0)
        {
            Instantiate(p_BlinkLight, transform.position, transform.rotation);
            transform.position = transform.position + direction.normalized * 5;
            Instantiate(p_BlinkLight, transform.position, transform.rotation);
            blinkTimeValue = 5;
        }
    }

    private IEnumerator StopCasting(float time)
    {
        yield return new WaitForSeconds(time);
        isCasting = false;
    }

    [Command]
    void CmdHitPlayer(GameObject hit, int damage)
    {
        hit.GetComponent<NetworkedPlayerScript>().RpcResolveHit(damage);
    }

    public void CreateLightning(Transform targetT)
    {
        lightningStartTime = Time.time;
        lightning = Instantiate(p_Lightning);
        lightningTarget = targetT;
        lightningCasting = true;
    }

    public void AutoAttack(Transform targetT)
    {
        if (Time.time > lastAutoTime + autoDelay && !isCasting && !isCastingSpell)
        {
            isCasting = true;
            if (GetComponent<NetworkedPlayerScript>().anim)
            {
                GetComponent<NetworkedPlayerScript>().anim.SetTrigger("AutoAttack");
            }

            //transform.LookAt(new Vector3(t_shoot.position.x, transform.position.y, t_shoot.position.z));
            GetComponent<PlayerMovement>().rotationTarget = targetT.position;
            GameObject autoAttack = Instantiate(p_AutoAttack, t_shoot.position, t_shoot.rotation, null);
            autoAttack.GetComponent<AutoAttackScript>().target = targetT;
            autoAttack.GetComponent<AutoAttackScript>().damage = GetComponent<NetworkedPlayerScript>().attackDamge;
            autoAttack.GetComponent<AutoAttackScript>().creator = GetComponent<NetworkedPlayerScript>();
            lastAutoTime = Time.time;
        }
    }

    public void CreateFire()
    {
        if (fireballTimeValue <= 0 && !isCastingSpell)
        {
            isCastingSpell = true;
            if (GetComponent<NetworkedPlayerScript>().anim)
            {
                //GetComponent<NetworkedPlayerScript>().anim.Play("CreateFire");
                GetComponent<NetworkedPlayerScript>().anim.SetTrigger("CreateFire");
            }

            fireScale = 0.5f;
            fire = Instantiate(p_fire, t_shoot.position, t_shoot.rotation, transform);
            GetComponent<UnityEngine.AI.NavMeshAgent>().speed *= 0.5f;
            fire.transform.parent = t_shoot;
        }
    }

    public void IncreaseFire()
    {
        if (!fire)
            return;

        if (fire != null && fireScale < 2 && GetComponent<NetworkedPlayerScript>().mana > 0)
        {
            GetComponent<NetworkedPlayerScript>().mana -= 0.5f;
            fireScale += Time.deltaTime * 0.1f;
            fire.transform.localScale = new Vector3(fireScale, fireScale, fireScale);
        }
    }

    public void CreateFireBall(Vector3 pos)
    {
        if (!fire || isThrowingFireball)
            return;

        isThrowingFireball = true;

        if (GetComponent<NetworkedPlayerScript>().anim)
        {
            //GetComponent<NetworkedPlayerScript>().anim.Play("CreateFireball");
            GetComponent<NetworkedPlayerScript>().anim.SetTrigger("CreateFireball");
        }

        Invoke("FireBallDelay", 0.5f);
    }

    private void FireBallDelay()
    {
        Destroy(fire);

        t_shoot.forward = transform.forward;
        fireball = Instantiate(p_fireball, t_shoot.position, t_shoot.rotation, null);
        fireball.transform.localScale = new Vector3(fireScale, fireScale, fireScale);
        fireball.GetComponent<FireballScript>().Remove();
        fireball.GetComponent<FireballScript>().creator = GetComponent<NetworkedPlayerScript>();
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed *= 2.0f;
        fireballTimeValue = 2;
        isCastingSpell = false;
        isThrowingFireball = false;

        if (GetComponent<NetworkedPlayerScript>().anim)
        {
            GetComponent<NetworkedPlayerScript>().anim.SetTrigger("FireballEnd");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, autoRange);
    }
}