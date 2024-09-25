using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager
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
        GameObject.Instantiate(Resources.Load(name,typeof(GameObject)), pos, Quaternion.identity);
    }
}
