using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpAndReachGameMode : MonoBehaviour
{
    public static JumpAndReachGameMode gameMode;
    void OnEnable()
    {
        gameMode = this;
        InjectRule();
    }

    [Header("Instances")]
    public GameObject gameStartPosition;

    [Header("Controls")]
    public bool playerControlEnable = true;

    protected virtual void InjectRule()
    {
        GameRuleManager<InputHandler.IGameRule>.defaultGameRule = new InputHandlerGameRule();
        GameRuleManager<TriggerVolume.IGameRule>.defaultGameRule = new TriggerVolumeGameRule();
        GameRuleManager<MyUnit.IGameRule>.defaultGameRule = new MyUnitGameRule();
    }
    protected virtual void ClearRule()
    {
        GameRuleManager<InputHandler.IGameRule>.defaultGameRule = null;
        GameRuleManager<TriggerVolume.IGameRule>.defaultGameRule = null;
        GameRuleManager<MyUnit.IGameRule>.defaultGameRule = null;
    }

    IEnumerator GameOverAndRespawnCoroutine(MyUnit myUnit)
    {
        playerControlEnable = false;
        Debug.Log("GameOver");
        yield return new WaitForSeconds(3);
        myUnit.TeleportAt(gameStartPosition.transform.position);
        playerControlEnable = true;
    }

    IEnumerator GameWinCoroutine()
    {
        playerControlEnable = false;
        Debug.Log("GameWin");
        yield return new WaitForSeconds(3);
        MyGameInstance.instance.hasCleared = true;
        SceneManager.LoadScene(0);
    }

    protected virtual bool GetInputPermission(InputHandler.Type type)
    {
        switch (type)
        {
            case InputHandler.Type.Player:
                return gameMode.playerControlEnable;
            default:
                return false;
        }
    }

    protected virtual void OnEnterTriggerVolume(TriggerVolume triggerVolume, Collider2D collision)
    {
        switch (triggerVolume.type)
        {
            case TriggerVolume.Type.Death:
                OnEnterDeath(triggerVolume, collision);
                break;
            case TriggerVolume.Type.Win:
                OnEnterWin(triggerVolume, collision);
                break;
            default:
                break;
        }
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
                default:
                    break;
            }
        }
    }

    protected void OnEnterDeath(TriggerVolume triggerVolume, Collider2D collision)
    {
        MyUnit myUnit = collision.GetComponent<MyUnit>();
        if (myUnit != null)
            myUnit.Die();
    }

    protected void OnEnterWin(TriggerVolume triggerVolume, Collider2D collision)
    {
        MyUnit myUnit = collision.GetComponent<MyUnit>();
        if (myUnit != null && myUnit.CompareTag("Player"))
        {
            myUnit.Ceremony();
            gameMode.StartCoroutine(gameMode.GameWinCoroutine());
        }
    }
    protected virtual void OnDeathMyUnit(MyUnit myUnit)
    {
        gameMode.StartCoroutine(gameMode.GameOverAndRespawnCoroutine(myUnit));
    }

    protected class TriggerVolumeGameRule : TriggerVolume.IGameRule
    {
        void TriggerVolume.IGameRule.OnEnter(TriggerVolume triggerVolume, Collider2D collision)
        {
            gameMode.OnEnterTriggerVolume(triggerVolume, collision);
        }

    }

    class MyUnitGameRule : MyUnit.IGameRule
    {
        void MyUnit.IGameRule.OnDeath(MyUnit myUnit)
        {
            gameMode.OnDeathMyUnit(myUnit);
        }
    }

    class InputHandlerGameRule : InputHandler.IGameRule
    {
        bool InputHandler.IGameRule.HasPermission(InputHandler inputHandler)
        {
            return gameMode.GetInputPermission(inputHandler.type);
        }
    }

}
