using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAndGunGameMode : JumpAndReachGameMode
{
    public static new RunAndGunGameMode gameMode = null;

    void OnEnable()
    {
        gameMode = this;
        JumpAndReachGameMode.gameMode = this;
        InjectRule();
    }

    void OnDisable()
    {
        ClearRule();
    }

    protected override void InjectRule()
    {
        base.InjectRule();
        GameRuleManager<GunSlinger.IGameRule>.defaultGameRule = new GunSlingerGameRule();
        GameRuleManager<Gun.IGameRule>.defaultGameRule = new UnequippedGunGameRule();
        GameRuleManager<Bullet.IGameRule>.defaultGameRule = new UnshotBulletGameRule();
    }

    protected override void ClearRule()
    {
        base.ClearRule();
        GameRuleManager<TriggerVolume.IGameRule>.defaultGameRule = null;
        GameRuleManager<GunSlinger.IGameRule>.defaultGameRule = null;
        GameRuleManager<Gun.IGameRule>.defaultGameRule = null;
        GameRuleManager<Bullet.IGameRule>.defaultGameRule = null;
    }

    protected override void OnEnterTriggerVolume(TriggerVolume triggerVolume, Collider2D collision)
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

            if (gunSlinger.EquippedGun != null)
            {
                gunSlinger.EquippedGun.RuleManager.overridedGameRule = null;
                gunSlinger.Unequip();
            }

            gunSlinger.Equip(gun);
            gunSlinger.EquippedGun.RuleManager.overridedGameRule = new EquippedGunGameRule(gunSlinger);
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
    protected virtual void OnStartGunSlinger(GunSlinger gunSlinger)
    {
        return;
    }

    protected virtual void OnFireEquippedGun(GunSlinger gunSlinger, Gun gun, GameObject bulletPrefab)
    {
        switch (gun.modelName)
        {
            case Gun.ModelName.MyLittleGun:
                if (bulletPrefab.GetComponent<Bullet>())
                {
                    Bullet bullet = Bullet.Shoot(bulletPrefab, gun.transform.position, Quaternion.Euler(0, 0, 90), 1000);
                    bullet.RuleManager.overridedGameRule = new ShotBulletGameRule(gunSlinger, gun);
                }
                break;
            default:
                break;
        }
    }

    protected virtual void OnHitShotBullet(GunSlinger shooter, Gun usedGun, Bullet bullet, Collision2D collision)
    {
        Debug.Log("Shooter: " + shooter + ", Gun: " + usedGun + ", Bullet: " + bullet);
    }

    protected class GunSlingerGameRule : GunSlinger.IGameRule
    {
        void GunSlinger.IGameRule.OnStart(GunSlinger gunSlinger)
        {
            gameMode.OnStartGunSlinger(gunSlinger);
        }

        void GunSlinger.IGameRule.OnUnequip(GunSlinger gunSlinger, Gun gun)
        {
            gun.RuleManager.overridedGameRule = null;
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
    }

    class UnequippedGunGameRule : Gun.IGameRule
    {
        void Gun.IGameRule.OnFire(Gun gun, GameObject bulletPrefab)
        {
            Debug.LogError("Unequipped gun shot.");
        }
    }

    protected class EquippedGunGameRule : Gun.IGameRule
    {
        readonly GunSlinger gunSlinger;
        public EquippedGunGameRule(GunSlinger gunSlinger)
        { 
            this.gunSlinger = gunSlinger;
        }

        void Gun.IGameRule.OnFire(Gun gun, GameObject bulletPrefab)
        {
            gameMode.OnFireEquippedGun(gunSlinger, gun, bulletPrefab);
        }
    }

    class UnshotBulletGameRule : Bullet.IGameRule
    {
        public static int wastedBulletCount = 0;
        void Bullet.IGameRule.OnDestroy(Bullet bullet)
        {
            wastedBulletCount++;
        }

        void Bullet.IGameRule.OnHit(Bullet bullet, Collision2D collision)
        {
        }
    }

    class ShotBulletGameRule : Bullet.IGameRule
    {
        readonly GunSlinger shooter;
        readonly Gun usedGun;

        public ShotBulletGameRule(GunSlinger shooter, Gun usedGun)
        {
            this.shooter = shooter;
            this.usedGun = usedGun;
        }
        void Bullet.IGameRule.OnDestroy(Bullet bullet)
        {
        }

        void Bullet.IGameRule.OnHit(Bullet bullet, Collision2D collision)
        {
            gameMode.OnHitShotBullet(shooter, usedGun, bullet, collision);
        }
    }
}