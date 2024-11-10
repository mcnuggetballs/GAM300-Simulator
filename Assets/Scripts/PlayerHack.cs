using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHack : MonoBehaviour
{
    public KeyCode hackKey;
    public LayerMask enemyLayer;
    public Hackable currentSelectedEntity;
    public float sphereRadius = 0.5f;
    public float sphereCastDistance = 10f;
    private Animator animator;
    [SerializeField]
    GameObject hackBarGameObject;
    [SerializeField]
    Image hackBar;
    float hackBarAmount = 0;
    [Header("Hack Charge")]
    protected float chargeValue = 0;
    public float maxChargeValue = 40;
    public float chargeHitAmount = 10;
    [SerializeField]
    Image hackCharge;
    [SerializeField]
    GameObject hackChargeNotif;
    [SerializeField]
    GameObject crosshair;
    Coroutine hackNotifCoroutine = null;
    [Header("HackModeUI")]
    [SerializeField]
    GameObject defaultHackDisplay;
    [SerializeField]
    GameObject smashHackDisplay;
    [SerializeField]
    GameObject hookHackDisplay;
    [SerializeField]
    GameObject explodableHackDisplay;
    [SerializeField]
    GameObject shootHackDisplay;
    [SerializeField]
    GameObject smashHackedDisplay;
    [SerializeField]
    GameObject hookHackedDisplay;
    [SerializeField]
    GameObject explodableHackedDisplay;
    [SerializeField]
    GameObject shootHackedDisplay;
    [SerializeField]
    GameObject hackingInProgressDisplay;
    public bool isHackingHackable = false;
    public bool hasHacked = false;
    float hackDisplayTimer = 0;
    float hackDisplayTime = 1.5f;
    protected Hackable lastHackedHackable = null;
    [SerializeField]
    AudioSource insufficientHackChargeSource;
    float vivianVoiceCooldown = 3.0f;
    float vivianVoiceTimer = 3.0f;
    public void DisplayHackNotification()
    {
        if (hackChargeNotif)
        {
            if (hackNotifCoroutine != null)
            {
                StopCoroutine(hackNotifCoroutine);
            }
            hackChargeNotif.SetActive(true);
            if (insufficientHackChargeSource != null && vivianVoiceTimer >= vivianVoiceCooldown)
            {
                vivianVoiceTimer = 0;
                insufficientHackChargeSource.Play();
            }
            
            hackNotifCoroutine = StartCoroutine(DisableHackChargeNotif(3.0f));
        }
    }
    public float GetChargeValue()
    {
        return chargeValue;
    }
    public void RemoveChargeValue(float amount)
    {
        chargeValue -= amount;
        if (chargeValue < 0)
        {
            chargeValue = 0;
        }
    }
    public void AddChargeValue(float amount)
    {
        chargeValue += amount;
        if (chargeValue > maxChargeValue)
        {
            chargeValue = maxChargeValue;
        }
    }
    private void Awake()
    {
        animator= GetComponent<Animator>();
        if (hackChargeNotif)
        {
            hackChargeNotif.SetActive(false);
        }
        if (crosshair)
        {
            crosshair.SetActive(false);
        }
    }
    private void DisableAllDisplay()
    {
        defaultHackDisplay.SetActive(false);
        explodableHackDisplay.SetActive(false);
        explodableHackedDisplay.SetActive(false);
        smashHackDisplay.SetActive(false);
        smashHackedDisplay.SetActive(false);
        hookHackDisplay.SetActive(false);
        hookHackedDisplay.SetActive(false); 
        hackingInProgressDisplay.SetActive(false);
        shootHackDisplay.SetActive(false);
        shootHackedDisplay.SetActive(false);
    }
    public void UpdateHackDisplay()
    {
        if (hasHacked)
        {
            hackDisplayTimer += Time.deltaTime;
            if (hackDisplayTimer <= hackDisplayTime)
            {
                if (lastHackedHackable)
                {
                    if (lastHackedHackable.GetComponent<ExplodingHackable>())
                    {
                        DisableAllDisplay();
                        explodableHackedDisplay.SetActive(true);
                    }
                    else if (lastHackedHackable.GetComponent<EnemyHackable>())
                    {
                        if (lastHackedHackable.GetComponent<EnemyHackable>().GetEnemySkill().skillName == "Slam")
                        {
                            DisableAllDisplay();
                            smashHackedDisplay.SetActive(true);
                        }
                        else if (lastHackedHackable.GetComponent<EnemyHackable>().GetEnemySkill().skillName == "Hook")
                        {
                            DisableAllDisplay();
                            hookHackedDisplay.SetActive(true);
                        }
                        else if (lastHackedHackable.GetComponent<EnemyHackable>().GetEnemySkill().skillName == "Shoot")
                        {
                            DisableAllDisplay();
                            shootHackedDisplay.SetActive(true);
                        }
                    }
                }
                else
                {
                    hasHacked = false;
                    hackDisplayTimer = 0;
                }
            }
            else
            {
                hasHacked = false;
                hackDisplayTimer = 0;
            }
        }
        else if (currentSelectedEntity != null)
        {
            if (!isHackingHackable)
            {
                if (currentSelectedEntity.GetComponent<ExplodingHackable>())
                {
                    DisableAllDisplay();
                    explodableHackDisplay.SetActive(true);
                }
                else if (currentSelectedEntity.GetComponent<EnemyHackable>())
                {
                    if (currentSelectedEntity.GetComponent<EnemyHackable>().GetEnemySkill().skillName == "Slam")
                    {
                        DisableAllDisplay();
                        smashHackDisplay.SetActive(true);
                    }
                    else if (currentSelectedEntity.GetComponent<EnemyHackable>().GetEnemySkill().skillName == "Hook")
                    {
                        DisableAllDisplay();
                        hookHackDisplay.SetActive(true);
                    }
                    else if (currentSelectedEntity.GetComponent<EnemyHackable>().GetEnemySkill().skillName == "Shoot")
                    {
                        DisableAllDisplay();
                        shootHackDisplay.SetActive(true);
                    }
                }
            }
            else
            {
                DisableAllDisplay();
                hackingInProgressDisplay.SetActive(true);
            }
        }
        else
        {
            DisableAllDisplay();
            defaultHackDisplay.SetActive(true);
        }
    }

    void Update()
    {
        vivianVoiceTimer += Time.deltaTime;
        if (hackCharge)
        {
            hackCharge.fillAmount = chargeValue / maxChargeValue;
        }

        if (Input.GetKeyDown(hackKey))
        {
            if (GetComponent<ThirdPersonControllerRB>() && GetComponent<ThirdPersonControllerRB>().Grounded)
            {
                if (GameManager.Instance.GetHackMode() == false)
                {
                    if (chargeValue >= 10)
                    {
                        GameManager.Instance.ToggleHackMode(true);
                        animator.SetBool("Hacking", true);
                        AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.HackSounds[2], transform.position);
                        if (crosshair)
                        {
                            crosshair.SetActive(true);
                        }
                    }
                    else
                    {
                        DisplayHackNotification();
                    }
                }
                else
                {
                    GameManager.Instance.ToggleHackMode(false);
                    animator.SetBool("Hacking", false);
                    AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.HackSounds[3], transform.position);
                    if (crosshair)
                    {
                        crosshair.SetActive(false);
                    }
                }
            }
        }
        if (chargeValue < 10 && GameManager.Instance.GetHackMode() == true)
        {
            GameManager.Instance.ToggleHackMode(false);
            animator.SetBool("Hacking", false);
            AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.MiscSounds[1], 0.2f, transform.position);

            DisplayHackNotification();
            if (crosshair)
            {
                crosshair.SetActive(false);
            }
        }

        if (!GameManager.Instance.GetHackMode())
        {
            hackBarGameObject.SetActive(false);
            hackBarAmount = 0;
            return;
        }
        if (Input.GetMouseButton(0) && currentSelectedEntity != null)
        {
            if (chargeValue >= 10)
            {
                isHackingHackable = true;
                hackBar.fillAmount = hackBarAmount/ currentSelectedEntity.hackDuration;
                hackBarGameObject.SetActive(true);
                hackBarAmount += Time.deltaTime;
                if (hackBarAmount >= currentSelectedEntity.hackDuration * SkillTreeManager.Instance.hackTimeMultiplier)
                {
                    hackBarAmount = 0.0f;
                    lastHackedHackable = currentSelectedEntity;
                    currentSelectedEntity.Hack(GetComponent<Entity>());
                    //here
                    if (GetComponent<FlashCardCollider>() != null)
                    {
                        FlashCardDisplay.Instance.Activate(GetComponent<FlashCardCollider>().displayImages);
                        Destroy(GetComponent<FlashCardCollider>());
                    }
                    NotifSystem.Instance.SkipEnterOne(8);
                    AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.HackSounds[0], transform.position);
                    if (currentSelectedEntity.GetComponent<EnemyAI>())
                    {
                        if (currentSelectedEntity.GetComponent<Animator>())
                        {
                            currentSelectedEntity.GetComponent<Animator>().SetTrigger("Stun");
                        }
                        currentSelectedEntity.GetComponent<EnemyAI>().Aggro();
                    }
                    hasHacked = true;
                    currentSelectedEntity.hacked = true;
                    currentSelectedEntity.Selected = false;
                    currentSelectedEntity = null;
                    chargeValue -= 10.0f;
                }
            }
            //else
            //{
            //    isHackingHackable = false;
            //    hackBarGameObject.SetActive(false);
            //    hackBarAmount = 0;
            //}
        }
        else
        {
            isHackingHackable = false;
            hackBarGameObject.SetActive(false);
            hackBarAmount = 0;

            CheckForEnemyHover();
        }

        UpdateHackDisplay();
    }

    void CheckForEnemyHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform a SphereCast in the direction of the ray
        if (Physics.SphereCast(ray, sphereRadius, out hit, Mathf.Infinity, enemyLayer))
        {
            // Get the Entity script on the object
            Hackable hackable = hit.collider.GetComponent<Hackable>();
            if (hackable && hackable.hacked)
            {
                hackable = null;
            }

            if (hackable != null)
            {
                // If there's a new entity under the mouse, deselect the old one
                if (currentSelectedEntity != null && currentSelectedEntity != hackable)
                {
                    currentSelectedEntity.Selected = false;
                }

                if (currentSelectedEntity != hackable && Vector3.Distance(transform.GetComponent<Entity>().neck.position, hackable.transform.position) <= sphereCastDistance)
                {
                    // Set the selected entity and mark it as selected
                    currentSelectedEntity = hackable;
                    currentSelectedEntity.Selected = true;
                }
            }
        }
        else
        {
            // If no enemy is hit by the SphereCast, deselect the current entity
            if (currentSelectedEntity != null)
            {
                currentSelectedEntity.Selected = false;
                currentSelectedEntity = null;
            }
        }
    }

    IEnumerator DisableHackChargeNotif(float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);
        if (hackChargeNotif)
        {
            hackChargeNotif.SetActive(false);
        }
        yield return null;
    }
}
