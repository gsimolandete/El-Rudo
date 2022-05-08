using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RudoWeaponController : MonoBehaviour
{
    public void Disarm()
    {
        Transform parent = transform.parent;
        transform.SetParent(null);
        parent.localScale = new Vector3(0,0,0);
        GetComponent<Animator>().applyRootMotion = true;
        GetComponent<Animator>().Play("disarmAnimation");
    }
    public void AutoDestruction()
    {
        Destroy(this);
    }
}
