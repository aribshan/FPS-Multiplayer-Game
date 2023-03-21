using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "new gun")]
public class GunInfo : ItemInfo
{
    public float damage;
    public float vRecoil;
    public float vRecoilRandomness;
    public float hRecoilRandomness;
    public float returnRecoilSens;
    public float bulletSpread;
    public float weaponKick;
    public float ReloadSpeed;
    
}
