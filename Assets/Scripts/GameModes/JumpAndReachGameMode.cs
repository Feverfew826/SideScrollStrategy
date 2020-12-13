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

    public GameObject startPosition;

    protected virtual void InjectRule()
    {
        GameRuleManager<InputHandler.IGameRule>.defaultGameRule = new InputHandlerGameRule();
        GameRuleManager<TriggerVolume.IGameRule>.defaultGameRule = new TriggerVolumeGameRule();
        GameRuleManager<MyPlayer.IGameRule>.defaultGameRule = new MyPlayerGameRule();
    }
    protected virtual void ClearRule()
    {
        GameRuleManager<InputHandler.IGameRule>.defaultGameRule = null;
        GameRuleManager<TriggerVolume.IGameRule>.defaultGameRule = null;
        GameRuleManager<MyPlayer.IGameRule>.defaultGameRule = null;
    }


    public bool playerControlEnable = true;

    IEnumerator GameOverAndRespawnCoroutine(MyPlayer myPlayer)
    {
        playerControlEnable = false;
        Debug.Log("GameOver");
        yield return new WaitForSeconds(3);
        myPlayer.TeleportAt(startPosition.transform.position);
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
                default:
                    break;
            }
        }
    }

    protected void OnEnterDeath(TriggerVolume triggerVolume, Collider2D collision)
    {
        MyPlayer myPlayer = collision.GetComponent<MyPlayer>();
        if (myPlayer != null)
            myPlayer.Die();
    }

    protected void OnEnterWin(TriggerVolume triggerVolume, Collider2D collision)
    {
        MyPlayer myPlayer = collision.GetComponent<MyPlayer>();
        if (myPlayer != null && myPlayer.CompareTag("Player"))
        {
            myPlayer.Ceremony();
            gameMode.StartCoroutine(gameMode.GameWinCoroutine());
        }
    }

    protected class TriggerVolumeGameRule : TriggerVolume.IGameRule
    {
        void TriggerVolume.IGameRule.OnEnter(TriggerVolume triggerVolume, Collider2D collision)
        {
            gameMode.OnEnterTriggerVolume(triggerVolume, collision);
        }

    }

    class MyPlayerGameRule : MyPlayer.IGameRule
    {
        void MyPlayer.IGameRule.OnDeath(MyPlayer myPlayer)
        {
            gameMode.StartCoroutine(gameMode.GameOverAndRespawnCoroutine(myPlayer));
        }
    }

    class InputHandlerGameRule : InputHandler.IGameRule
    {
        bool InputHandler.IGameRule.HasPermission(InputHandler inputHandler)
        {
            switch(inputHandler.type)
            {
                case InputHandler.Type.Player:
                    return gameMode.playerControlEnable;
                default:
                    return false;
            }
        }
    }

}
