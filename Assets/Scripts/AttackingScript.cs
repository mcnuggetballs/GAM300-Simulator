using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingScript : MonoBehaviour
{
    [SerializeField]
    protected Animator anim;
    public int noOfClicks = 0;
    protected ThirdPersonController controller;
    public string skillName;
    PlayerHack playerHack;
    [Header("Cheat")]
    [SerializeField]
    Transform teleportLiftPos;
    [SerializeField]
    Transform teleportLevel1Pos;
    [SerializeField]
    Transform cameraRoot;

    private void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<ThirdPersonController>();
        }
        playerHack = GetComponent<PlayerHack>();
    }
    // Update is called once per frame
    public void ResetClicks()
    {
        noOfClicks = 0;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Entity[] entities = FindObjectsOfType<Entity>();
            foreach(Entity entity in entities)
            {
                if (entity != GetComponent<Entity>())
                {
                    entity.TakeDamage(100000);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            transform.position = teleportLiftPos.position;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            transform.position = teleportLevel1Pos.position;
            //cameraRoot.rotation = Quaternion.Euler(0f, 180.0f, 0f);
        }

        if (Input.GetMouseButtonDown(0) && !TimeManager.Instance.IsGamePaused())
        {
            OnClick();
        }

        if (playerHack)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (playerHack.GetChargeValue() >= 10)
                {
                    if (GetComponent<Entity>())
                    {
                        if (GetComponent<Entity>().skill && GetComponent<ThirdPersonControllerRB>().Grounded && anim.GetBool("Hit2") == false && anim.GetBool("Hit1") == false)
                        {
                            if (GetComponent<Entity>().skill.GetCooldownRemaining() <= 0)
                            {
                                if (GetComponent<Entity>().skill.Activate(gameObject))
                                {
                                    playerHack.RemoveChargeValue(10.0f);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (GetComponent<Entity>() && GetComponent<Entity>().skill)
                    {
                        playerHack.DisplayHackNotification();
                    }
                }
            }
        }

        //temp fix
        //if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hit2") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Hit1") && !anim.GetCurrentAnimatorStateInfo(0).IsName("ClosePhone") && !anim.GetCurrentAnimatorStateInfo(0).IsName("OpenPhone"))
        //{
        //    GetComponent<ThirdPersonControllerRB>().disableMovement = false;
        //}
        //else
        //{
        //    GetComponent<ThirdPersonControllerRB>().disableMovement = true;
        //}
    }
    void OnClick()
    {
        if (noOfClicks == 0)
        {
            noOfClicks++;
        }
        if (noOfClicks == 1 && anim.GetBool("Hit2") == false)
        {
            anim.SetBool("Hit1", true);
            anim.SetBool("Hit2", false);
            noOfClicks++;
        }
        if (noOfClicks >= 2 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25f && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit1"))
        {
            anim.SetBool("Hit2", true);
            anim.SetBool("Hit1", false);
            noOfClicks++;
        }
        if (noOfClicks >= 3 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25f && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit2"))
        {
            anim.SetBool("Hit3", true);
            anim.SetBool("Hit2", false);
            ResetClicks();
        }
    }
}
