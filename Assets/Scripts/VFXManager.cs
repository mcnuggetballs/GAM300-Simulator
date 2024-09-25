using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{

    static private VFXManager vfxManager;
    public static VFXManager Instance
    {
        get
        {
            if (vfxManager == null)
            {
                vfxManager = new VFXManager();
            }
            return vfxManager;
        }
    }

    public void Spawn(string name,Vector3 pos, float scale = 1)
    {
        GameObject item = Resources.Load(name) as GameObject;
        item.transform.position = pos;
        item.transform.localScale = new Vector3(scale, scale, scale);
    }
}
