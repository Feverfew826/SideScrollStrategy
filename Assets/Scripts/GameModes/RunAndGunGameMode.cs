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
        GameRuleManager<GunSlinger.IGameRule>.defaultGameRule = new TemporalGunSlingerGameRule();
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
        MyUnit myUnit = collision.GetComponent<MyUnit>();
        if (myUnit != null)
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
        gunSlinger.RuleManager.overridedGameRule = new GunSlingerGameRule();
    }

    protected virtual void OnFireEquippedGun(GunSlinger gunSlinger, Gun gun, GameObject bulletPrefab, GunSlingerGameRule gunSlingerGameRule)
    {
        switch (gun.modelName)
        {
            case Gun.ModelName.MyLittleGun:
                if (bulletPrefab.GetComponent<Bullet>())
                {
                    float power = 1000;
                    Quaternion rotation = Quaternion.Euler(0, 0, 270);
                    if (gun.transform.rotation.eulerAngles.y != 0)
                    { 
                        power *= -1;
                        rotation = Quaternion.Euler(0, 0, 90);
                    }
                    Bullet bullet = Bullet.Shoot(bulletPrefab, gun.transform.position, rotation, power);
                    bullet.RuleManager.overridedGameRule = new ShotBulletGameRule(gunSlinger, gun, gunSlingerGameRule);
                }
                break;
            default:
                break;
        }
    }

    protected virtual void OnHitShotBullet(GunSlinger shooter, Gun usedGun, Bullet bullet, Collider2D collider)
    {
        Debug.Log("Shooter: " + shooter + ", Gun: " + usedGun + ", Bullet: " + bullet);
    }

    protected class TemporalGunSlingerGameRule : GunSlinger.IGameRule
    {
        void GunSlinger.IGameRule.OnDestroy(GunSlinger gunSlinger)
        {
            return;
        }

        void GunSlinger.IGameRule.OnEquip(GunSlinger gunSlinger, Gun gun)
        {
            return;
        }

        void GunSlinger.IGameRule.OnStart(GunSlinger gunSlinger)
        {
            gameMode.OnStartGunSlinger(gunSlinger);
        }

        void GunSlinger.IGameRule.OnUnequip(GunSlinger gunSlinger, Gun gun)
        {
            return;
        }
    }

    protected class GunSlingerGameRule : GunSlinger.IGameRule
    {
        public interface DeathSubscriber
        {
            void OnDeath(GunSlingerGameRule gunSlingerGameRule);
        }
        List<DeathSubscriber> deathSubscribers = new List<DeathSubscriber>();

        public void UnSubscribeDeath(DeathSubscriber deathSubscriber)
        {
            deathSubscribers.Remove(deathSubscriber);
        }

        public void SubscribeDeath(DeathSubscriber deathSubscriber)
        {
            deathSubscribers.Add(deathSubscriber);
        }

        void GunSlinger.IGameRule.OnDestroy(GunSlinger gunSlinger)
        {
            deathSubscribers.ForEach(a => a.OnDeath(this));
        }

        void GunSlinger.IGameRule.OnStart(GunSlinger gunSlinger)
        {
            return;
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

        void GunSlinger.IGameRule.OnEquip(GunSlinger gunSlinger, Gun gun)
        {
            gun.RuleManager.overridedGameRule = new EquippedGunGameRule(gunSlinger, this);
        }
    }

    class UnequippedGunGameRule : Gun.IGameRule
    {
        void Gun.IGameRule.OnDestroy(Gun gun)
        {
            return;
        }

        void Gun.IGameRule.OnFire(Gun gun, GameObject bulletPrefab)
        {
            Debug.LogError("Unequipped gun shot.");
        }
    }

    protected class EquippedGunGameRule : Gun.IGameRule, GunSlingerGameRule.DeathSubscriber
    {
        GunSlinger gunSlinger;
        GunSlingerGameRule gunSlingerGameRule;
        public EquippedGunGameRule(GunSlinger gunSlinger, GunSlingerGameRule gunSlingerGameRule)
        {
            this.gunSlinger = gunSlinger;
            this.gunSlingerGameRule = gunSlingerGameRule;
            gunSlingerGameRule.SubscribeDeath(this);
        }
        void GunSlingerGameRule.DeathSubscriber.OnDeath(GunSlingerGameRule gunSlinger)
        {
            this.gunSlinger = null;
            gunSlingerGameRule = null;
        }

        void Gun.IGameRule.OnDestroy(Gun gun)
        {
            if (gunSlingerGameRule != null)
                gunSlingerGameRule.UnSubscribeDeath(this);
        }

        void Gun.IGameRule.OnFire(Gun gun, GameObject bulletPrefab)
        {
            gameMode.OnFireEquippedGun(gunSlinger, gun, bulletPrefab, gunSlingerGameRule);
        }
    }

    class UnshotBulletGameRule : Bullet.IGameRule
    {
        public static int wastedBulletCount = 0;
        void Bullet.IGameRule.OnDestroy(Bullet bullet)
        {
            wastedBulletCount++;
        }

        void Bullet.IGameRule.OnHit(Bullet bullet, Collider2D collider)
        {
        }
    }

    class ShotBulletGameRule : Bullet.IGameRule, GunSlingerGameRule.DeathSubscriber
    {
        GunSlinger shooter;
        readonly Gun usedGun;
        GunSlingerGameRule gunSlingerGameRule;

        public ShotBulletGameRule(GunSlinger shooter, Gun usedGun, GunSlingerGameRule gunSlingerGameRule)
        {
            this.shooter = shooter;
            this.usedGun = usedGun;
            this.gunSlingerGameRule = gunSlingerGameRule;
            gunSlingerGameRule.SubscribeDeath(this);
        }

        void GunSlingerGameRule.DeathSubscriber.OnDeath(GunSlingerGameRule gunSlinger)
        {
            this.shooter = null;
            gunSlingerGameRule = null;
        }

        void Bullet.IGameRule.OnDestroy(Bullet bullet)
        {
            if(gunSlingerGameRule != null)
                gunSlingerGameRule.UnSubscribeDeath(this);
        }

        void Bullet.IGameRule.OnHit(Bullet bullet, Collider2D collider)
        {
            if(shooter != null)
                gameMode.OnHitShotBullet(shooter, usedGun, bullet, collider);
        }
    }
}