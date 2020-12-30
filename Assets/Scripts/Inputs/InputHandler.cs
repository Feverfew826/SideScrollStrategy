using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public InputSource inputSource;

    public enum Type
    {
        Player,
        Ai
    }
    public interface IGameRule
    {
        bool HasPermission(InputHandler inputHandler);
    }
    public GameRuleManager<IGameRule> RuleManager { private set; get; } = new GameRuleManager<IGameRule>();

    public Type type;

    public interface IInputReceiver
    {
        void OnHorizontalAxis(float horizontalAxis);
        void OnVerticalAxis(float verticalAxis);
        void OnJumpButtonDown();
        void OnFireButtonDown();
        void OnUnequipButtonDown();
    }
    private HashSet<IInputReceiver> inputReceivers;
    private HashSet<IInputReceiver> inputReceiverRegisterAwaiters;
    private HashSet<IInputReceiver> inputReceiverUnregisterAwaiters;

    private void Awake()
    {
        inputReceivers = new HashSet<IInputReceiver>();
        inputReceiverRegisterAwaiters = new HashSet<IInputReceiver>();
        inputReceiverUnregisterAwaiters = new HashSet<IInputReceiver>();
    }

    // Update is called once per frame
    void Update()
    {
        inputReceivers.UnionWith(inputReceiverRegisterAwaiters);
        inputReceiverRegisterAwaiters.Clear();
        inputReceivers.ExceptWith(inputReceiverUnregisterAwaiters);
        inputReceiverUnregisterAwaiters.Clear();

        if(RuleManager.Rule?.HasPermission(this) ?? false)
        {
            foreach(var receiver in inputReceivers)
            {
                receiver.OnHorizontalAxis(inputSource.GetHorizontalAxisLeftStick());
                receiver.OnVerticalAxis(inputSource.GetVerticalAxisLeftStick());
                
                if (inputSource.GetButtonDown(InputSource.Button.Cross))
                    receiver.OnJumpButtonDown();

                if (inputSource.GetButtonDown(InputSource.Button.Square))
                    receiver.OnFireButtonDown();

                if (inputSource.GetButtonDown(InputSource.Button.Circle))
                    receiver.OnUnequipButtonDown();
            }
        }
    }

    public void AddInputReceiverRegisterAwaiter(IInputReceiver inputReceiver)
    {
        inputReceiverRegisterAwaiters.Add(inputReceiver);
    }

    public void AddInputReceiverUnregisterAwaiter(IInputReceiver inputReceiver)
    {
        inputReceiverUnregisterAwaiters.Add(inputReceiver);
    }

}
