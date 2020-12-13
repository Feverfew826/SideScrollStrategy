using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSlinger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        bullets = new List<Gun.BulletInfo>();
        RuleManager.Rule?.OnStart(this);
    }

    private void OnDestroy()
    {
        RuleManager.overridedGameRule = null;
    }

    public interface IGameRule
    {
        void OnStart(GunSlinger gunSlinger);
        void OnUnequip(GunSlinger gunSlinger, Gun gun);
    }
    public GameRuleManager<IGameRule> RuleManager { private set; get; } = new GameRuleManager<IGameRule>();

    public GameObject gunHand;

    [SerializeField]
    private Gun _equippedGun;
    public Gun EquippedGun { get { return _equippedGun; } }

    public Gun.BulletInfo selectedBullet;
    public List<Gun.BulletInfo> bullets = null;

    public void Equip(Gun gun)
    {
        gun.GetComponent<BoxCollider2D>().enabled = false;
        gun.GetComponent<Rigidbody2D>().simulated = false;
        gun.transform.parent = gunHand.transform;
        gun.transform.localPosition = Vector3.zero;
        _equippedGun = gun;
        if(gun.loadedBulletInfo.prefab == null || gun.loadedBulletInfo.num == 0)
            Load();
    }

    public void Unequip()
    {
        if (EquippedGun != null)
        {
            Gun unequippedGun = EquippedGun;
            _equippedGun = null;

            unequippedGun.GetComponent<BoxCollider2D>().enabled = true;
            unequippedGun.GetComponent<Rigidbody2D>().simulated = true;
            unequippedGun.transform.parent = null;

            RuleManager.Rule?.OnUnequip(this, unequippedGun);

        }
    }

    public void Fire()
    {
        if(EquippedGun != null)
        {
            EquippedGun.PullTrigger();
        }
    }

    public void Load()
    {
        if (selectedBullet.prefab != null && EquippedGun != null)
        {
            Gun.BulletInfo[] returnedBullets = EquippedGun.Load(selectedBullet);
            foreach(Gun.BulletInfo returned in returnedBullets)
            {
                int index = bullets.FindIndex(p => p.prefab == returned.prefab);
                if (index == -1)
                {
                    bullets.Add(returned);
                }
                else
                { 
                    bullets[index].num += returned.num;
                }
            }
        }
    }

    public void AddBullet(Gun.BulletInfo bulletInfo)
    {
        if (bullets.Count == 0)
        {
            bullets.Add(bulletInfo);
            selectedBullet = bulletInfo;
            Load();
        }
        else
        {
            int index = bullets.FindIndex(p => p.prefab == bulletInfo.prefab);
            if (index == -1)
            {
                bullets.Add(bulletInfo);
            }
            else
            {
                bullets[index].num += bulletInfo.num;
            }
        }
        Load();
    }
}
