using UnityEngine;

namespace GorillaEntertainmentSystem.Scripts
{
    public class Buttons : MonoBehaviour
    {
        float last_press = -1f;

        void OnTriggerEnter(Collider col)
        {
            var ind = col.GetComponent<GorillaTriggerColliderHandIndicator>();
            float current = Time.time;
            if (current - last_press < 0.2f) return;
            if (ind == null) return;

            last_press = current;
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, ind.isLeftHand, 0.05f);

            switch (gameObject.name)
            {
                case "Up":
                    Plugin.screen.ChangeIndex(-1);
                    break;
                case "Down":
                    Plugin.screen.ChangeIndex(1);
                    break;
                case "PageUp":
                    Plugin.screen.ChangeIndex(-11);
                    break;
                case "PageDown":
                    Plugin.screen.ChangeIndex(11);
                    break;
                case "Select":
                    Plugin.screen.Select();
                    break;
                case "Settings":
                    Plugin.screen.Settings();
                    break;
                case "Exit":
                    Plugin.screen.Exit();
                    break;
                case "ShowMenu":
                    Plugin.screen.ShowMenu();
                    break;
            }
        }
    }
}
