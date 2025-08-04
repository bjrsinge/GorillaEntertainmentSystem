using BepInEx;
using BepInEx.Configuration;
using GorillaNetworking;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;
using DevHoldableEngine;
using GorillaEntertainmentSystem.Scripts;

namespace GorillaEntertainmentSystem
{
    [BepInPlugin("com.bjrsinge.gorillatag.gorillaentertainmentsystem", "GorillaEntertainmentSystem", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static GameObject asset;
        public static Scripts.Screen screen;
        public static ConfigEntry<bool> swap_hands;
        public static ConfigEntry<bool> grab_to_use;
        public static Vector2 stick;
        bool steam, init, stick_click, last_stick_click;
        public static bool grab, swapped;
        public static RenderTexture screen_texture;
        public static Material screen_material;

        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }

        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
            swap_hands = Config.Bind("Controls", "Swap Hands", false, "Swaps the inputs so that the right stick controls movement and the left controller face buttons are for A and B. Start and Select are still the same. (Left and right trigger respectively)");
            grab_to_use = Config.Bind("Controls", "Grab to Use", true, "Whether you need to be holding the NES screen in order for inputs to register");
        }

        void OnEnable()
        {
            if (init)
            {
                asset.SetActive(true);
            }
        }

        void OnDisable()
        {
            if (init)
            {
                asset.SetActive(false);
                Scripts.Screen.unes._rendererRunning = false;
                Scripts.Screen.unes.Renderer?.End();
            }
        }

        void BundleSetup()
        {
            var bundle = LoadAssetBundle("GorillaEntertainmentSystem.nes_screen");
            asset = Instantiate(bundle.LoadAsset<GameObject>("nesscreen"));
            screen_texture = bundle.LoadAsset<RenderTexture>("nes");
            screen_material = bundle.LoadAsset<Material>("nesmart");

            asset.transform.position = new Vector3(-63.2749f, 12.3997f, -82.9012f);
            asset.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
            asset.transform.localEulerAngles = new Vector3(0f, 90f, 0f);

            screen = asset.transform.Find("Menu/Screen").AddComponent<Scripts.Screen>();
            screen.Initialize();
            asset.AddComponent<Rigidbody>().isKinematic = true;
            asset.AddComponent<DevHoldable>();

            asset.transform.Find("Buttons/ShowMenu").AddComponent<Buttons>().gameObject.layer = 18;
            foreach (Transform button in asset.transform.Find("Menu/Buttons"))
            {
                button.AddComponent<Buttons>().gameObject.layer = 18;
            }
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            BundleSetup();
            init = true;
            Config.SaveOnConfigSet = true;
            steam = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";
            swapped = swap_hands.Value;
            DevHoldable.CanInput = !grab_to_use.Value;
        }

        void Update()
        {
            if (!init) return;

            if (steam && !swapped) { stick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.axis; }
            else if (!steam && !swapped) { ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out stick); }
            else { stick = ControllerInputPoller.instance.rightControllerPrimary2DAxis; }

            if (steam) { stick_click = SteamVR_Actions.gorillaTag_LeftJoystickClick.state; }
            else if (!steam) { ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out stick_click); }

            if (stick_click && !last_stick_click)
            {
                asset.transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
            last_stick_click = stick_click;
        }
    }
}