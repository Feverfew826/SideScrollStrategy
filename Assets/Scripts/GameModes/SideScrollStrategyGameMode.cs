using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScrollStrategyGameMode : RunAndGunGameMode
{
    public static new SideScrollStrategyGameMode gameMode;

    private void OnEnable()
    {
        gameMode = this;
        JumpAndReachGameMode.gameMode = this;
        RunAndGunGameMode.gameMode = this;
        InjectRule();
    }

    private void OnDisable()
    {
        ClearRule();
    }

    protected override void InjectRule()
    {
        base.InjectRule();
        GameRuleManager<SelectableVolume.IGameRule>.defaultGameRule = new SelectableVolumeGameRule();
        GameRuleManager<UnitDefinitionGui.IGameRule>.defaultGameRule = new UnitDefinitionGuiGameRule();
    }

    protected override void ClearRule()
    {
        base.ClearRule();
    }

    [System.Serializable]
    public struct CreatableUnitDesign
    {
        public GameObject prefab;
        public Sprite image;
    }

    public UnitInfoGui unitInfoGui;
    public UnitCreationGui unitCreationGui;
    public FreeMovement freeMover;
    public CreatableUnitDesign[] creatableUnits;
    public GameObject target;

    private static int unitSeqNum;

    protected override void OnHitShotBullet(GunSlinger shooter, Gun usedGun, Bullet bullet, Collision2D collision)
    {
        TriggerVolume triggerVolume = collision.gameObject.GetComponent<TriggerVolume>();
        if (triggerVolume != null)
        {
            if( triggerVolume.type == TriggerVolume.Type.EnemyBarrack )
            {
                Debug.Log(shooter + " hits EnemyBarrack!");
                bullet.GetComponent<BoxCollider2D>().enabled = false;
                bullet.GetComponent<Rigidbody2D>().gravityScale = 0.5f;
                bullet.GetComponent<Rigidbody2D>().AddForce((UnityEngine.Random.insideUnitCircle + Vector2.up) * 100);
                bullet.GetComponent<Rigidbody2D>().AddTorque(UnityEngine.Random.Range(-1000, 1000));
            }
        }
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
                        index = i,
                        name = creatableUnits[i].prefab.name,
                        image = (creatableUnits[i].image != null) ? creatableUnits[i].image : creatableUnits[i].prefab.GetComponent<SpriteRenderer>().sprite
                    }
                    );
            }

            return retVal;
        }
        else
            return new List<UnitDefinitionGui.UnitDefinition>();
    }

    public void CreateUnit(int index)
    {
        GameObject unitGo = Instantiate<GameObject>(creatableUnits[index].prefab, startPosition.transform.position, Quaternion.identity);
        unitGo.name = "Unit" + unitSeqNum++;
        MyBotAi myBotAi = unitGo.GetComponent<MyBotAi>();
        myBotAi.target = target;
    }

    protected override void OnStartGunSlinger(GunSlinger gunSlinger)
    {
        gunSlinger.EquippedGun.RuleManager.overridedGameRule = new EquippedGunGameRule(gunSlinger);
        gunSlinger.Load();
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
        unitCreationGui.Display(GetCreatableUnitDefinitions().ToArray());
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
        void UnitDefinitionGui.IGameRule.OnClick(UnitDefinitionGui unitDefinitionGui)
        {
            gameMode.CreateUnit(unitDefinitionGui.unitDefinition.index);
        }
    }

}
