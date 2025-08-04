using GorillaEntertainmentSystem;
using GorillaLocomotion;
using UnityEngine;

namespace DevHoldableEngine
{
    public class DevHoldable : HoldableObject
    {
        public bool
            InHand = false,
            InLeftHand = false,
            PickUp = true,
            DidSwap = false,
            SwappedLeft = true;

        public static bool CanInput;

        public float
            GrabDistance = 0.175f,
            ThrowForce = 1.75f;

        public virtual void OnGrab(bool isLeft)
        {
            if (Plugin.grab_to_use.Value)
            {
                CanInput = true;
            }
        }

        public virtual void OnDrop(bool isLeft)
        {
            if (Plugin.grab_to_use.Value)
            {
                CanInput = false;
            }
        }

        public void Update()
        {
            float left = ControllerInputPoller.instance.leftControllerGripFloat;
            bool leftGrip = left >= 0.5f;

            float right = ControllerInputPoller.instance.rightControllerGripFloat;
            bool rightGrip = right >= 0.5f;

            var Distance = GrabDistance * GTPlayer.Instance.scale;
            if (DidSwap && (!SwappedLeft ? !leftGrip : !rightGrip))
                DidSwap = false;

            bool pickLeft = PickUp && leftGrip && Vector3.Distance(GTPlayer.Instance.leftControllerTransform.position, transform.position) < Distance && !InHand && EquipmentInteractor.instance.leftHandHeldEquipment == null && !DidSwap;
            bool swapLeft = InHand && leftGrip && rightGrip && !DidSwap && (Vector3.Distance(GTPlayer.Instance.leftControllerTransform.position, transform.position) < Distance) && !SwappedLeft && EquipmentInteractor.instance.leftHandHeldEquipment == null;
            if (pickLeft || swapLeft)
            {
                DidSwap = swapLeft;
                SwappedLeft = true;

                InLeftHand = true;
                InHand = true;

                transform.SetParent(GorillaTagger.Instance.offlineVRRig.leftHandTransform.parent);

                GorillaTagger.Instance.StartVibration(true, 0.1f, 0.05f);
                EquipmentInteractor.instance.leftHandHeldEquipment = this;
                if (DidSwap) EquipmentInteractor.instance.rightHandHeldEquipment = null;

                OnGrab(true);
            }
            else if (!leftGrip && InHand && InLeftHand)
            {
                InLeftHand = true;
                InHand = false;
                transform.SetParent(null);

                EquipmentInteractor.instance.leftHandHeldEquipment = null;
                OnDrop(true);
            }

            bool pickRight = PickUp && rightGrip && Vector3.Distance(GTPlayer.Instance.rightControllerTransform.position, transform.position) < Distance && !InHand && EquipmentInteractor.instance.rightHandHeldEquipment == null && !DidSwap;
            bool swapRight = InHand && leftGrip && rightGrip && !DidSwap && (Vector3.Distance(GTPlayer.Instance.rightControllerTransform.position, transform.position) < Distance) && SwappedLeft && EquipmentInteractor.instance.rightHandHeldEquipment == null;
            if (pickRight || swapRight)
            {
                DidSwap = swapRight;
                SwappedLeft = false;

                InLeftHand = false;
                InHand = true;

                transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform.parent);

                GorillaTagger.Instance.StartVibration(false, 0.1f, 0.05f);
                EquipmentInteractor.instance.rightHandHeldEquipment = this;
                if (DidSwap) EquipmentInteractor.instance.leftHandHeldEquipment = null;

                OnGrab(false);
            }
            else if (!rightGrip && InHand && !InLeftHand)
            {
                InLeftHand = false;
                InHand = false;
                transform.SetParent(null);

                EquipmentInteractor.instance.rightHandHeldEquipment = null;
                OnDrop(false);
            }
        }

        public override void DropItemCleanup() {}
        public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand) {}
        public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand) {}
    }
}
