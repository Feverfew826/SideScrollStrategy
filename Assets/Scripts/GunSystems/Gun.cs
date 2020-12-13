using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private void OnDestroy()
    {
        RuleManager.overridedGameRule = null;
    }

    public interface IGameRule
    {
        void OnFire(Gun gun, GameObject bulletPrefab);
    }
    public GameRuleManager<IGameRule> RuleManager { private set; get; } = new GameRuleManager<IGameRule>();

    public enum Type
    {
        HandGun,
        AssaultRifle,
        SubmachineGun
    }

    public enum ShootType
    {
        SemiAuto,
        Burst,
        FullAuto
    }

    [System.Serializable]
    public class Specification
    {
        public Type type;

        [SerializeField]
        public ShootType[] _shootTypes;
        public ShootType[] ShootTypes { get { return (_shootTypes.Length > 0) ? _shootTypes : new ShootType[]{ ShootType.SemiAuto }; } }

        public int burstNum = 3;

        [SerializeField]
        private int _maxLoadSize = 1;
        public int MaxLoadSize { get { return _maxLoadSize; } }

    }
    public Specification spec;

    public ShootType currentShootType;

    [HideInInspector]
    public int maxLoadSizeExtenstion;
    public int MaxLoadSize { get { return spec.MaxLoadSize + maxLoadSizeExtenstion; } }
    public enum ModelName
    {
        MyLittleGun
    }
    public ModelName modelName;

    public void PullTrigger()
    {
        Fire();
    }

    public void Fire()
    {
        if (IsLoaded())
            RuleManager.Rule?.OnFire(this, loadedBulletInfo.prefab);
    }

    [System.Serializable]
    public class BulletInfo
    {
        public BulletInfo(GameObject prefab, int num)
        {
            this.prefab = prefab;
            this.num = num;
        }
        public GameObject prefab;
        public int num;
    }

    public BulletInfo loadedBulletInfo = null;

    public BulletInfo[] Load(BulletInfo bulletInfo)
    {
        if(loadedBulletInfo.prefab != null)
        {
            if(loadedBulletInfo.prefab == bulletInfo.prefab)
            {
                int loaded = Mathf.Min(MaxLoadSize, loadedBulletInfo.num + bulletInfo.num);
                int remains = loadedBulletInfo.num + bulletInfo.num - loaded;

                loadedBulletInfo.num = loaded;

                return new BulletInfo[1] { new BulletInfo(bulletInfo.prefab, remains) };
            }
            else
            {
                BulletInfo oldBullet = loadedBulletInfo;

                int loaded = Mathf.Min(MaxLoadSize, bulletInfo.num);
                int remains = bulletInfo.num - loaded;

                loadedBulletInfo.prefab = bulletInfo.prefab;
                loadedBulletInfo.num = loaded;

                return new BulletInfo[2] { new BulletInfo(bulletInfo.prefab, remains), oldBullet };
            }
        }
        else
        {
            int loaded = Mathf.Min(MaxLoadSize, bulletInfo.num);
            int remains = bulletInfo.num - loaded;

            loadedBulletInfo = new BulletInfo(bulletInfo.prefab, loaded);

            return new BulletInfo[1] { new BulletInfo(bulletInfo.prefab, remains) };
        }
    }

    public bool IsLoaded()
    {
        if (loadedBulletInfo != null && loadedBulletInfo.num > 0)
            return true;
        else
            return false;
    }

}
