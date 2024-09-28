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
    
    private void Awake()
    {
        animator= GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(hackKey))
        {
            if (GetComponent<ThirdPersonControllerRB>() && GetComponent<ThirdPersonControllerRB>().Grounded)
            {
                GameManager.Instance.ToggleHackMode(!GameManager.Instance.GetHackMode());
                animator.SetBool("Hacking", !animator.GetBool("Hacking"));
                AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.MiscSounds[1],0.2f,transform.position);
            }
        }

        if (!GameManager.Instance.GetHackMode())
        {
            hackBarGameObject.SetActive(false);
            hackBarAmount = 0;
            return;
        }
        if (Input.GetMouseButton(0))
        {
            if (currentSelectedEntity != null)
            {
                hackBar.fillAmount = hackBarAmount;
                hackBarGameObject.SetActive(true);
                hackBarAmount += Time.deltaTime;
                if (hackBarAmount >= 1.0f)
                {
                    hackBarAmount = 0.0f;
                    currentSelectedEntity.Hack(GetComponent<Entity>());
                    currentSelectedEntity.hacked = true;
                    currentSelectedEntity.Selected = false;
                    currentSelectedEntity = null;
                }
            }
            else
            {
                hackBarGameObject.SetActive(false);
                hackBarAmount = 0;
            }
        }
        else
        {
            hackBarGameObject.SetActive(false);
            hackBarAmount = 0;

            CheckForEnemyHover();
        }
    }

    void CheckForEnemyHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform a SphereCast in the direction of the ray
        if (Physics.SphereCast(ray, sphereRadius, out hit, sphereCastDistance, enemyLayer))
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

                if (currentSelectedEntity != hackable)
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
}
