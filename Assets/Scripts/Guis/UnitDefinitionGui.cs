using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDefinitionGui : MonoBehaviour
{
    public interface IGameRule
    {
        void OnClick(UnitDefinitionGui unitDefinitionGui);
    }
    readonly GameRuleManager<IGameRule> RuleManager = new GameRuleManager<IGameRule>();

    public struct UnitDefinition
    {
        public int index;
        public string name;
        public Sprite image;
    }

    public UnitDefinition unitDefinition;
    public Text unitName;
    public Image unitImage;

    public void SetUnitDefinition(UnitDefinition unitDefinition)
    {
        this.unitDefinition = unitDefinition;
        unitName.text = unitDefinition.name;
        unitImage.sprite = unitDefinition.image;
        unitImage.preserveAspect = true;
    }

    public void OnClick()
    {
        RuleManager.Rule?.OnClick(this);
    }

}
