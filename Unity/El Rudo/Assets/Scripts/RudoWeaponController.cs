using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RudoWeaponController : MonoBehaviour
{
    public void AutoDestruction()
    {
        Destroy(gameObject);
    }
}
