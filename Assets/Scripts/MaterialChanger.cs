using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    [SerializeField]
    bool skinnedMesh = false;
    private Material[] originalMeshMaterials;
    private Material[] originalSkinnedMaterials;

    public Material hackMaterial;

    private SkinnedMeshRenderer[] skinnedmeshRenderers;
    private MeshRenderer[] meshRenderers;

    void Start()
    {
        //Get all the materials
        if (!skinnedMesh)
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            originalMeshMaterials = new Material[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                originalMeshMaterials[i] = meshRenderers[i].material;
            }
        }
        else
        {
            skinnedmeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            originalSkinnedMaterials = new Material[skinnedmeshRenderers.Length];
            for (int i = 0; i < skinnedmeshRenderers.Length; i++)
            {
                originalSkinnedMaterials[i] = skinnedmeshRenderers[i].material;
            }
        }
    }

    void Update()
    {
        // Check the hackMode
        if (GameManager.Instance.GetHackMode())
        {
            // Change all materials to hack material
            if (!skinnedMesh)
            {
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
                foreach (var renderer in skinnedmeshRenderers)
                {
                    if (renderer.material != hackMaterial)
                    {
                        renderer.material = hackMaterial;
                    }
                }
            }
        }
        else
        {
            // Change all materials back to the original materials

            if (!skinnedMesh)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    if (meshRenderers[i].material != originalMeshMaterials[i])
                    {
                        meshRenderers[i].material = originalMeshMaterials[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < skinnedmeshRenderers.Length; i++)
                {
                    if (skinnedmeshRenderers[i].material != originalSkinnedMaterials[i])
                    {
                        skinnedmeshRenderers[i].material = originalSkinnedMaterials[i];
                    }
                }
            }
        }
    }
}
