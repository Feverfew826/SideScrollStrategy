using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSource : MonoBehaviour
{
    public enum Button
    {
        Cross,
        Circle,
        Square,
        Triangle
    }

    private Dictionary<Button, string> buttonStrings;

    private void Awake()
    {
        buttonStrings = new Dictionary<Button, string>()
        {
            {Button.Cross, "Jump" },
            {Button.Circle, "Fire2" },
            {Button.Square, "Fire1" },
            {Button.Triangle, "" }
        };
    }

    public virtual bool GetButtonDown(Button button)
    {
        return Input.GetButtonDown(buttonStrings[button]);
    }

    public virtual float GetHorizontalAxisLeftStick()
    {
        return Input.GetAxis("Horizontal");
    }
    public virtual float GetVerticalAxisLeftStick()
    {
        return Input.GetAxis("Vertical");
    }
}
