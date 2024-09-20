using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHack : MonoBehaviour
{
    public KeyCode hackKey;
    public LayerMask enemyLayer;
    public Entity currentSelectedEntity;
    public float sphereRadius = 0.5f;
    public float sphereCastDistance = 10f;
    private Animator animator;
    private void Awake()
    {
        animator= GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(hackKey))
        {
            GameManager.Instance.ToggleHackMode(!GameManager.Instance.GetHackMode());
            animator.SetBool("Hacking", !animator.GetBool("Hacking"));
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (currentSelectedEntity!= null)
            {
                if (GetComponent<Entity>() != null)
                {
                    GetComponent<Entity>().CopySkill(currentSelectedEntity.GetSkillName());
                }
            }
        }
        CheckForEnemyHover();
    }

    void CheckForEnemyHover()
    {
        if (!GameManager.Instance.GetHackMode())
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform a SphereCast in the direction of the ray
        if (Physics.SphereCast(ray, sphereRadius, out hit, sphereCastDistance, enemyLayer))
        {
            // Get the Entity script on the object
            Entity entity = hit.collider.GetComponent<Entity>();

            if (entity != null)
            {
                // If there's a new entity under the mouse, deselect the old one
                if (currentSelectedEntity != null && currentSelectedEntity != entity)
                {
                    currentSelectedEntity.Selected = false;
                }

                // Set the selected entity and mark it as selected
                currentSelectedEntity = entity;
                currentSelectedEntity.Selected = true;
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
