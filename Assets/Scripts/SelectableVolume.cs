using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableVolume : MonoBehaviour
{
    public enum Type
    {
        Background,
        Barrack,
    }

    public Type type;

    public interface IGameRule
    {
        void OnMouseDown(SelectableVolume selectableVolume);
        void OnDestroy(SelectableVolume selectableVolume);
    }
    public GameRuleManager<IGameRule> RuleManager { private set; get; } = new GameRuleManager<IGameRule>();

    private void OnMouseDown()
    {
        if(! EventSystem.current.IsPointerOverGameObject())
            RuleManager.Rule?.OnMouseDown(this);
    }

    private void OnDestroy()
    {
        RuleManager.Rule?.OnDestroy(this);
    }
}
