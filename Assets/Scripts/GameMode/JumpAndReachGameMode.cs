using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpAndReachGameMode : MonoBehaviour, 
    InputHandler.GameRule,
    TriggerVolume.GameRule,
    MyPlayer.GameRule
{
    public GameObject startPosition;

    protected virtual void InjectRule()
    {
        InputHandler.gameRule = this;
        TriggerVolume.gameRule = this;
        MyPlayer.gameRule = this;
    }
    protected virtual void ClearRule()
    {
        InputHandler.gameRule = null;
        TriggerVolume.gameRule = null;
        MyPlayer.gameRule = null;
    }

    void OnEnable()
    {
        InjectRule();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GameOverAndRespawnCoroutine(MyPlayer myPlayer)
    {
        playerControlEnable = false;
        Debug.Log("GameOver");
        yield return new WaitForSeconds(3);
        myPlayer.TeleportAt( startPosition.transform.position );
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

    void TriggerVolume.GameRule.OnEnter(TriggerVolume triggerVolume, Collider2D collision)
    {
        switch(triggerVolume.type)
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
    protected void OnEnterDeath(TriggerVolume triggerVolume, Collider2D collision)
    {
        MyPlayer myPlayer = collision.GetComponent<MyPlayer>();
        if(myPlayer != null)
            myPlayer.Die();
    }

    protected void OnEnterWin(TriggerVolume triggerVolume, Collider2D collision)
    {
        MyPlayer myPlayer = collision.GetComponent<MyPlayer>();
        if (myPlayer != null)
        { 
            myPlayer.Ceremony();
            StartCoroutine(GameWinCoroutine());
        }

    }

    public bool playerControlEnable = true;
    bool InputHandler.GameRule.HasPermission(InputHandler inputHandler)
    {
        switch (inputHandler.type)
        {
            case InputHandler.Type.Player:
                return playerControlEnable;
            default:
                return false;
        }
    }

    void MyPlayer.GameRule.OnDeath(MyPlayer myPlayer)
    {
        StartCoroutine(GameOverAndRespawnCoroutine(myPlayer));
    }
}
