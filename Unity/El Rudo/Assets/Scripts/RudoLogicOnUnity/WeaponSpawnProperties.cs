using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSpawnProperties", menuName = "Rudo/Weapon", order = 1)]
public class WeaponSpawnProperties : ScriptableObject
{
    public Sprite sprite;
    public Vector2 offset= new Vector2(0.658f,0);
    public Vector2 scale= new Vector2(0.4f,0.4f);
}
