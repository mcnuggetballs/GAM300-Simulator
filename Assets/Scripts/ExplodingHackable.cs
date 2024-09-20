using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingHackable : Hackable
{
    [SerializeField]
    GameObject explosionPrefab;
    public override void Hack(Entity player)
    {
        Instantiate(explosionPrefab,transform.position,Quaternion.identity,null);
        Destroy(this);
    }


}
