using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    private Material[] originalMaterials;

    public Material hackMaterial;

    private SkinnedMeshRenderer[] meshRenderers;

    void Start()
    {
        //Get all the materials
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        originalMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }
    }

    void Update()
    {
        // Check the hackMode
        if (GameManager.Instance.GetHackMode())
        {
            // Change all materials to hack material
            foreach (var renderer in meshRenderers)
            {
                if (renderer.material != hackMaterial)
                {
                    renderer.material = hackMaterial;
                }
            }
        }
        else
        {
            // Change all materials back to the original materials
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if (meshRenderers[i].material != originalMaterials[i])
                {
                    meshRenderers[i].material = originalMaterials[i];
                }
            }
        }
    }
}
