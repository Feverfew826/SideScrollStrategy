using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAndGunGameMode : JumpAndReachGameMode,
    TriggerVolume.GameRule,
    Gun.GameRule,
    GunSlinger.GameRule,
    Bullet.GameRule
{
    enum BulletHandleType
    {
        Bullet
    }
    private Dictionary<GameObject, BulletHandleType> bulletHandleTypeCache = null;



    void OnEnable()
    {
        InjectRule();
    }

    void OnDisable()
    {
        ClearRule();
    }

    // Start is called before the first frame update
    void Start()
    {
        bulletHandleTypeCache = new Dictionary<GameObject, BulletHandleType>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void InjectRule()
    {
        base.InjectRule();
        TriggerVolume.gameRule = this;
        GunSlinger.gameRule = this;
        Gun.gameRule = this;
        Bullet.gameRule = this;
    }

    protected override void ClearRule()
    {
        base.ClearRule();
        TriggerVolume.gameRule = null;
        GunSlinger.gameRule = null;
        Gun.gameRule = null;
        Bullet.gameRule = null;
    }

    void TriggerVolume.GameRule.OnEnter(TriggerVolume triggerVolume, Collider2D collision)
    {
        MyPlayer myPlayer = collision.GetComponent<MyPlayer>();
        if (myPlayer != null)
        {
            switch (triggerVolume.type)
            {
                case TriggerVolume.Type.Death:
                    OnEnterDeath(triggerVolume, collision);
                    break;
                case TriggerVolume.Type.Win:
                    OnEnterWin(triggerVolume, collision);
                    break;
                case TriggerVolume.Type.Gun:
                    OnEnterGun(triggerVolume, collision);
                    break;
                case TriggerVolume.Type.BulletBundle:
                    OnEnterBulletBundle(triggerVolume, collision);
                    break;
                default:
                    break;
            }
        }
    }

    protected void OnEnterGun(TriggerVolume triggerVolume, Collider2D collision)
    {
        GunSlinger gunSlinger = collision.GetComponent<GunSlinger>();
        if (gunSlinger != null)
        {
            Gun gun = triggerVolume.GetComponentInParent<Gun>();
            Destroy(triggerVolume.gameObject);

            gunSlinger.Equip(gun);
        }
    }

    protected void OnEnterBulletBundle(TriggerVolume triggerVolume, Collider2D collision)
    {
        GunSlinger gunSlinger = collision.GetComponent<GunSlinger>();
        if (gunSlinger != null)
        {
            BulletBundle bulletBundle = triggerVolume.GetComponentInParent<BulletBundle>();
            Destroy(triggerVolume.gameObject);

            gunSlinger.AddBullet(new Gun.BulletInfo(bulletBundle.bulletPrefab, bulletBundle.num));
            Destroy(bulletBundle.gameObject);
        }
    }

    public void OnFire(Gun gun, GameObject bulletPrefab)
    {
        switch(gun.modelName)
        {
            case Gun.ModelName.MyLittleGun:
                HandleBullet(bulletPrefab, gun.transform.position, gun.transform.rotation, 1000);
                break;
            default:
                break;
        }
    }

    public void HandleBullet(GameObject bulletPrefab, Vector3 position, Quaternion rotation, float power)
    {
        BulletHandleType bulletHandleType;
        if (bulletHandleTypeCache.TryGetValue(bulletPrefab, out bulletHandleType))
        {
            switch(bulletHandleType)
            {
                case BulletHandleType.Bullet:
                    Bullet.Shoot(bulletPrefab, position, rotation, power);
                    break;
                default:
                    Debug.Log("Not implemented!");
                    break;
            }
        }
        else
        {
            if (bulletPrefab.GetComponent<Bullet>())
                bulletHandleTypeCache.Add(bulletPrefab, BulletHandleType.Bullet);
        }
    }

    void GunSlinger.GameRule.OnUnequip(Gun gun)
    {
        gun.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 200);

        GameObject newGo = new GameObject("TriggerVolume");

        TriggerVolume triggerVolume = newGo.AddComponent<TriggerVolume>();
        triggerVolume.type = TriggerVolume.Type.Gun;

        BoxCollider2D boxCollider = newGo.AddComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        boxCollider.isTrigger = true;
        boxCollider.size = gun.GetComponent<BoxCollider2D>().size;

        newGo.transform.parent = gun.transform;
        newGo.transform.localPosition = Vector3.zero;

        triggerVolume.EnableTriggerAfterSeconds(3);
    }

    public void OnHit(Bullet bullet, Collision2D collision)
    {
        if(collision.relativeVelocity.magnitude > 1)
        {
            Destroy(bullet.gameObject);
        }
    }

    public bool RegisterBullet(GameObject bulletPrefab)
    {
        return false;
    }
}
