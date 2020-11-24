using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSlinger : MonoBehaviour
{
    public interface GameRule
    {
        void OnUnequip(Gun gun);
        bool RegisterBullet(GameObject bulletPrefab);
    }
    static public GameRule gameRule;

    public GameObject gunHand;
    private Gun equippedGun = null;

    public Gun.BulletInfo selectedBullet;
    public List<Gun.BulletInfo> bullets = null;

    // Start is called before the first frame update
    void Start()
    {
        bullets = new List<Gun.BulletInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Equip(Gun gun)
    {
        gun.GetComponent<BoxCollider2D>().enabled = false;
        gun.GetComponent<Rigidbody2D>().simulated = false;
        gun.transform.parent = gunHand.transform;
        gun.transform.localPosition = Vector3.zero;
        equippedGun = gun;
        if(gun.loadedBulletInfo == null || gun.loadedBulletInfo.num == 0)
            Load();
    }

    public void Unequip()
    {
        if (equippedGun != null)
        {
            Gun unequippedGun = equippedGun;
            equippedGun = null;

            unequippedGun.GetComponent<BoxCollider2D>().enabled = true;
            unequippedGun.GetComponent<Rigidbody2D>().simulated = true;
            unequippedGun.transform.parent = null;

            if (gameRule != null)
                gameRule.OnUnequip(unequippedGun);
        }

    }

    public void Fire()
    {
        if(equippedGun != null)
        {
            equippedGun.PullTrigger();
        }
    }

    public void Load()
    {
        if (selectedBullet != null && equippedGun != null)
        {
            Gun.BulletInfo[] returnedBullets = equippedGun.Load(selectedBullet);
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
