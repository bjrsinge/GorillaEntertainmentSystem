using DevHoldableEngine;
using System;
using UnityEngine;

namespace GorillaEntertainmentSystem.Scripts.UNES.Input
{
    public class VRInput : BaseInput
    {
        bool a, b, start, select, swapped;
        Vector2 stick;
        public override void HandlerKeyDown(Action<KeyCode> onKeyDown)
        {
            swapped = Plugin.swap_hands.Value;
            a = swapped ? ControllerInputPoller.instance.leftControllerSecondaryButton : ControllerInputPoller.instance.rightControllerPrimaryButton;
            b = swapped ? ControllerInputPoller.instance.leftControllerPrimaryButton : ControllerInputPoller.instance.rightControllerSecondaryButton;
            start = ControllerInputPoller.instance.leftControllerTriggerButton;
            select = ControllerInputPoller.instance.rightControllerTriggerButton;
            stick = Plugin.stick;

            if (DevHoldable.CanInput)
            {
                if (a) { onKeyDown(KeyCode.A); }
                if (b) { onKeyDown(KeyCode.S); }
                if (start) { onKeyDown(KeyCode.Alpha1); }
                if (select) { onKeyDown(KeyCode.Alpha2); }

                if (stick.x >= 0.5f)
                {
                    onKeyDown(KeyCode.RightArrow);
                }
                if (stick.x <= -0.5f)
                {
                    onKeyDown(KeyCode.LeftArrow);
                }
                if (stick.y >= 0.5f)
                {
                    onKeyDown(KeyCode.UpArrow);
                }
                if (stick.y <= -0.5f)
                {
                    onKeyDown(KeyCode.DownArrow);
                }
            }
        }

        public override void HandlerKeyUp(Action<KeyCode> onKeyUp)
        {
            swapped = Plugin.swap_hands.Value;
            a = swapped ? ControllerInputPoller.instance.leftControllerSecondaryButton : ControllerInputPoller.instance.rightControllerPrimaryButton;
            b = swapped ? ControllerInputPoller.instance.leftControllerPrimaryButton : ControllerInputPoller.instance.rightControllerSecondaryButton;
            start = ControllerInputPoller.instance.leftControllerTriggerButton;
            select = ControllerInputPoller.instance.rightControllerTriggerButton;
            stick = Plugin.stick;

            if (DevHoldable.CanInput)
            {
                if (!a) { onKeyUp(KeyCode.A); }
                if (!b) { onKeyUp(KeyCode.S); }
                if (!start) { onKeyUp(KeyCode.Alpha1); }
                if (!select) { onKeyUp(KeyCode.Alpha2); }

                if (stick.x <= 0.5f)
                {
                    onKeyUp(KeyCode.RightArrow);
                }
                if (stick.x >= -0.5f)
                {
                    onKeyUp(KeyCode.LeftArrow);
                }
                if (stick.y <= 0.5f)
                {
                    onKeyUp(KeyCode.UpArrow);
                }
                if (stick.y >= -0.5f)
                {
                    onKeyUp(KeyCode.DownArrow);
                }
            }
        }
    }
}
