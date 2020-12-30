using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyBotAiExtensions
{
    public static bool IsSameSide(this MyBotAi thisBot, MyBotAi otherBot)
    {
        if (thisBot.destroyOperationTarget == otherBot.destroyOperationTarget)
            return true;
        else
            return false;
    }
}

public static class BulletExtensions
{
    public static void Ricochet(this Bullet bullet)
    {
        bullet.GetComponent<BoxCollider2D>().enabled = false;
        Rigidbody2D rigidBody = bullet.GetComponent<Rigidbody2D>();

        rigidBody.gravityScale = 0.5f;
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce((UnityEngine.Random.insideUnitCircle + Vector2.up) * 100);
        rigidBody.AddTorque(UnityEngine.Random.Range(-1000, 1000));
    }
}

public class SideScrollStrategyGameMode : RunAndGunGameMode
{
    public static new SideScrollStrategyGameMode gameMode;

    private void Start()
    {
        StartCoroutine(SpawnEnemyPerSeconds(3));
        StartCoroutine(AddCostPerSecond());
    }

    private void OnEnable()
    {
        gameMode = this;
        JumpAndReachGameMode.gameMode = this;
        RunAndGunGameMode.gameMode = this;
        InjectRule();

        UpdateGuisAboutCost();

        shouldSpawnEnemy = true;
        shouldAddCost = true;
    }

    private void OnDisable()
    {
        shouldSpawnEnemy = false;
        ClearRule();
    }

    [System.Serializable]
    public struct CreatableUnitDesign
    {
        public GameObject prefab;
        public Sprite image;
        public int cost;
        public float coolTime;
        public float remainCoolTime;
    }

    [System.Serializable]
    public struct CreatableEnemyDesign
    {
        public GameObject prefab;
        public bool canCreateInfinite;
        public int maximumCreatableNumber;
        public int currentCount;
    }

    [Header("Guis")]
    public UnitInfoGui unitInfoGui;
    public UnitCreationGui unitCreationGui;
    public UnityEngine.UI.Text costText;

    [Header("Units")]
    public CreatableUnitDesign[] creatableUnits;
    public CreatableEnemyDesign[] creatableEnemies;

    [Header("Instances")]
    public FreeMovement freeMover;
    public GameObject enemyBarrack;
    public GameObject playerBarrack;
    public GameObject enemyGameStart;

    [Header("Variables")]
    public int currentCost;
    public int costPerSeconds;

    private int unitSeqNum;
    private int enemySeqNum;
    private List<int> countsOfEnemies = new List<int>();

    private bool shouldSpawnEnemy = false;
    private bool shouldAddCost = false;

    protected override void InjectRule()
    {
        base.InjectRule();
        GameRuleManager<SelectableVolume.IGameRule>.defaultGameRule = new SelectableVolumeGameRule();
    }

    protected override void ClearRule()
    {
        base.ClearRule();
    }

    public List<UnitDefinitionGui.UnitDefinition> GetCreatableUnitDefinitions()
    {
        if (creatableUnits != null)
        {
            List<UnitDefinitionGui.UnitDefinition> retVal = new List<UnitDefinitionGui.UnitDefinition>();
            for (int i = 0; i < creatableUnits.Length; i++)
            {
                retVal.Add(
                    new UnitDefinitionGui.UnitDefinition
                    {
                        name = creatableUnits[i].prefab.name,
                        image = (creatableUnits[i].image != null) ? creatableUnits[i].image : creatableUnits[i].prefab.GetComponent<SpriteRenderer>().sprite,
                        cost = creatableUnits[i].cost,
                        coolTime = creatableUnits[i].coolTime
                    }
                    );
            }

            return retVal;
        }
        else
            return new List<UnitDefinitionGui.UnitDefinition>();
    }

    public void OnClickUnitDefinitionGui(UnitDefinitionGui unitDefinitionGui, int relatedCreatableUnitsIndex)
    {
        if(creatableUnits[relatedCreatableUnitsIndex].remainCoolTime != 0f)
        {
            Debug.Log("Still cooling.");
        }
        else if( currentCost < creatableUnits[relatedCreatableUnitsIndex].cost )
        {
            Debug.Log("Not enough cost.");
        }
        else
        {
            GameObject unitGo = Instantiate<GameObject>(creatableUnits[relatedCreatableUnitsIndex].prefab, gameStartPosition.transform.position, Quaternion.identity);
            unitGo.name = "Unit" + unitSeqNum++;
            MyBotAi myBotAi = unitGo.GetComponent<MyBotAi>();
            myBotAi.destroyOperationTarget = enemyBarrack;

            currentCost -= creatableUnits[relatedCreatableUnitsIndex].cost;
            creatableUnits[relatedCreatableUnitsIndex].remainCoolTime = creatableUnits[relatedCreatableUnitsIndex].coolTime;

            UpdateGuisAboutCost();

            StartCoroutine(CoolTimeReductionCoroutine(relatedCreatableUnitsIndex));
            unitDefinitionGui.StartCoolTimeDisplayUpdate();
        }
    }

    IEnumerator CoolTimeReductionCoroutine(int index)
    {
        while(creatableUnits[index].remainCoolTime != 0f)
        {
            yield return new WaitForSeconds(0.03f);
            creatableUnits[index].remainCoolTime = Mathf.Max(0f, creatableUnits[index].remainCoolTime - 0.03f);
        }
    }

    IEnumerator SpawnEnemyPerSeconds(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            if (shouldSpawnEnemy)
                CreateEnemy();
        }
    }

    IEnumerator AddCostPerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if(shouldAddCost)
            { 
                currentCost += costPerSeconds;
                UpdateGuisAboutCost();
            }
        }
    }

    private void UpdateGuisAboutCost()
    {
        costText.text = currentCost.ToString();

        IReadOnlyList<UnitDefinitionGui> unitDefGuis = unitCreationGui.GetUnitDefinitionGuis();
        foreach (UnitDefinitionGui unitDefGui in unitDefGuis)
        {
            unitDefGui.UpdateAvailableDisplay(currentCost);
        }
    }

    public void CreateEnemy()
    {
        int index = Mathf.FloorToInt( UnityEngine.Random.Range(0, creatableEnemies.Length) );
        index = Mathf.Max(index, creatableEnemies.Length - 1);

        if(creatableEnemies[index].currentCount < creatableEnemies[index].maximumCreatableNumber)
        {
            GameObject unitGo = Instantiate<GameObject>(creatableEnemies[index].prefab, enemyGameStart.transform.position, Quaternion.identity);
            creatableEnemies[index].currentCount++;
            unitGo.name = "Enemy" + enemySeqNum++;

            MyBotAi myBotAi = unitGo.GetComponent<MyBotAi>();
            myBotAi.destroyOperationTarget = playerBarrack;

            MyUnit myUnit = unitGo.GetComponent<MyUnit>();
            myUnit.RuleManager.overridedGameRule = new EnemyUnitGameRule(index);
        }

    }

    protected override bool GetInputPermission(InputHandler.Type type)
    {
        switch(type)
        {
            case InputHandler.Type.Player:
                return playerControlEnable;
            case InputHandler.Type.Ai:
                return true;
            default:
                return false;
        }
    }

    protected override void OnStartGunSlinger(GunSlinger gunSlinger)
    {
        GunSlingerGameRule gunSlingerGameRule = new GunSlingerGameRule();
        gunSlinger.RuleManager.overridedGameRule = gunSlingerGameRule;
        gunSlinger.EquippedGun.RuleManager.overridedGameRule = new EquippedGunGameRule(gunSlinger, gunSlingerGameRule);
        gunSlinger.Load();
    }

    protected override void OnHitShotBullet(GunSlinger shooter, Gun usedGun, Bullet bullet, Collider2D collider)
    {
        TriggerVolume triggerVolume = collider.gameObject.GetComponent<TriggerVolume>();
        if (triggerVolume != null)
        {
            switch(triggerVolume.type)
            {
                case TriggerVolume.Type.EnemyBarrack:
                    //Debug.Log(shooter + " hits EnemyBarrack!");
                    bullet.Ricochet();
                    break;
                case TriggerVolume.Type.OurBarrack:
                    //Debug.Log(shooter + " hits OurBarrack!");
                    bullet.Ricochet();
                    break;
                default:
                    break;
            }
        }
        else
        {
            MyUnit hittedUnit = collider.gameObject.GetComponent<MyUnit>();
            MyBotAi hittedAi = collider.gameObject.GetComponent<MyBotAi>();
            if(hittedUnit != null && hittedAi != null && ! hittedUnit.IsDead )
            {
                MyUnit shotUnit = shooter.GetComponent<MyUnit>();
                MyBotAi shotAi = shooter.GetComponent<MyBotAi>();

                if (shotUnit != null && shotAi != null)
                {
                    if (!shotAi.IsSameSide(hittedAi))
                    {
                        bullet.Ricochet();
                        hittedUnit.TakeDamage(0.5f);
                    }
                }
            }

        }
    }

    protected override void OnDeathMyUnit(MyUnit myUnit)
    {
        StartCoroutine(DestroyUnitAfterSeconds(myUnit, 3));
        return;
    }

    IEnumerator DestroyUnitAfterSeconds(MyUnit myUnit, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(myUnit.gameObject);
    }

    protected void OnDeathEnemyUnit(MyUnit myUnit, int creatableEnemyIndex)
    {
        creatableEnemies[creatableEnemyIndex].currentCount--;
        StartCoroutine(DestroyUnitAfterSeconds(myUnit, 1));
        return;
    }

    class EnemyUnitGameRule : MyUnit.IGameRule
    {
        private int creatableEnemyIndex = 0;

        public EnemyUnitGameRule(int creatableEnemyIndex)
        {
            this.creatableEnemyIndex = creatableEnemyIndex;
        }

        void MyUnit.IGameRule.OnDeath(MyUnit myUnit)
        {
            gameMode.OnDeathEnemyUnit(myUnit, creatableEnemyIndex);
        }
    }

    class UnitInfoGuiGameRule : UnitInfoGui.IGameRule
    {
        SelectedSelectableVolumeGameRule ssvgr;

        public UnitInfoGuiGameRule(SelectedSelectableVolumeGameRule ssvgr)
        {
            this.ssvgr = ssvgr;
        }

        void UnitInfoGui.IGameRule.OnDestroy(UnitInfoGui unitInfoGui)
        {
            ssvgr.shouldHideOnDestroy = false;
        }
    }

    class SelectableVolumeGameRule : SelectableVolume.IGameRule
    {
        void SelectableVolume.IGameRule.OnMouseDown(SelectableVolume selectableVolume)
        {
            gameMode.OnMouseDownSelectableVolume(selectableVolume);
        }
        void SelectableVolume.IGameRule.OnDestroy(SelectableVolume selectableVolume)
        {
            HandleDestroy(selectableVolume);
        }

        protected virtual void HandleDestroy(SelectableVolume selectableVolume)
        {
            return;
        }

    }

    public virtual void OnMouseDownSelectableVolume(SelectableVolume selectableVolume)
    {
        switch (selectableVolume.type)
        {
            case SelectableVolume.Type.Barrack:
                HandleBarrackSelection(selectableVolume);
                break;
            case SelectableVolume.Type.Background:
                HandleBackgroundSelection();
                break;
            default:
                break;
        }
    }

    private void HandleBarrackSelection(SelectableVolume selectableVolume)
    {
        SelectedSelectableVolumeGameRule ssvgr = new SelectedSelectableVolumeGameRule();
        selectableVolume.RuleManager.overridedGameRule = ssvgr;
        unitInfoGui.RuleManager.overridedGameRule = new UnitInfoGuiGameRule(ssvgr);

        unitInfoGui.Display(new UnitInfoGui.UnitInfo(selectableVolume.gameObject.name, selectableVolume.gameObject.GetComponent<SpriteRenderer>()));

        List<UnitDefinitionGui.UnitDefinition> CreatableUnits = GetCreatableUnitDefinitions();
        unitCreationGui.Initialize(CreatableUnits.Count);

        IReadOnlyList<UnitDefinitionGui> unitDefGuis = unitCreationGui.GetUnitDefinitionGuis();
        for (int i = 0; i < CreatableUnits.Count; i++)
        {
            unitDefGuis[i].SetUnitDefinition(CreatableUnits[i]);
            unitDefGuis[i].RuleManager.overridedGameRule = new UnitDefinitionGuiGameRule(i);
        }

        freeMover.transform.position = new Vector3(selectableVolume.transform.position.x, selectableVolume.transform.position.y, freeMover.transform.position.z);
    }

    private void HandleBackgroundSelection()
    {
        unitInfoGui.Hide();
        unitCreationGui.Hide();
    }

    class SelectedSelectableVolumeGameRule : SelectableVolumeGameRule
    {
        public bool shouldHideOnDestroy = true;
        protected override void HandleDestroy(SelectableVolume selectableVolume)
        {
            if(shouldHideOnDestroy)
                gameMode.unitInfoGui.Hide();
        }
    }

    class UnitDefinitionGuiGameRule : UnitDefinitionGui.IGameRule
    {
        int creatableUnitIndex;
        public UnitDefinitionGuiGameRule(int creatableUnitIndex)
        {
            this.creatableUnitIndex = creatableUnitIndex;
        }

        float UnitDefinitionGui.IGameRule.GetRemainCoolTime(UnitDefinitionGui unitDefinitionGui)
        {
            return gameMode.creatableUnits[creatableUnitIndex].remainCoolTime;
        }

        void UnitDefinitionGui.IGameRule.OnClick(UnitDefinitionGui unitDefinitionGui)
        {
            gameMode.OnClickUnitDefinitionGui(unitDefinitionGui, creatableUnitIndex);
        }

    }
}
