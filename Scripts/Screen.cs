using BepInEx;
using DevHoldableEngine;
using GorillaEntertainmentSystem.Scripts.UNES;
using GorillaEntertainmentSystem.Scripts.UNES.Controller;
using GorillaEntertainmentSystem.Scripts.UNES.Input;
using System.IO;
using TMPro;
using UnityEngine;

namespace GorillaEntertainmentSystem.Scripts
{
    public class Screen : MonoBehaviour
    {
        GameObject emulator;
        public static UNESBehaviour unes;
        int rom_index, setting_index;
        string[] rom_files, settings = { $"Swap hands : {Plugin.swap_hands.Value}", $"Grab to use : {Plugin.grab_to_use.Value}" };
        string rom_path, selected_rom;
        bool in_settings, no_roms;
        TextMeshPro header_text, screen_text;

        public void Initialize()
        {
            header_text = Plugin.asset.transform.Find("Menu/Screen/header_text").GetComponent<TextMeshPro>();
            screen_text = Plugin.asset.transform.Find("Menu/Screen/screen_text").GetComponent<TextMeshPro>();
            header_text.font = GorillaTagger.Instance.offlineVRRig.playerText1.font;
            screen_text.font = GorillaTagger.Instance.offlineVRRig.playerText1.font;

            emulator = Plugin.asset.transform.Find("UNES").gameObject;
            unes = emulator.AddComponent<UNESBehaviour>();
            unes.RenderTexture = Plugin.screen_texture;
            unes.FilterMode = FilterMode.Bilinear;
            unes.KeyConfig = new KeyConfig();
            unes.Input = new VRInput();

            rom_path = Path.Combine(Paths.PluginPath, "GorillaEntertainmentSystem", "roms");

            if (!Directory.Exists(rom_path))
            {
                Directory.CreateDirectory(rom_path);
                return;
            }

            rom_files = Directory.GetFiles(rom_path, "*.nes");
            if (rom_files.Length == 0) no_roms = true;
            UpdateScreen();
        }

        void ReloadEmu(byte[] bytes)
        {
            Destroy(emulator);
            unes._rendererRunning = false;
            unes.Renderer?.End();
            emulator = new GameObject("UNES");
            emulator.transform.SetParent(Plugin.asset.transform);
            unes = emulator.AddComponent<UNESBehaviour>();
            unes.RenderTexture = Plugin.screen_texture;
            unes.FilterMode = FilterMode.Bilinear;
            unes.KeyConfig = new KeyConfig();
            unes.Input = new VRInput();
            Plugin.asset.transform.Find("NESScreen").GetComponent<Renderer>().material = Plugin.screen_material;
            unes.Boot(bytes);
        }

        public void ChangeIndex(int increment)
        {
            if (in_settings) { setting_index = (setting_index + increment + settings.Length) % settings.Length; }
            else { rom_index = (rom_index + increment + rom_files.Length) % rom_files.Length; }
            UpdateScreen();
        }

        public void Select()
        {
            if (in_settings)
            {
                switch (setting_index)
                {
                    case 0:
                        Plugin.swap_hands.Value = !Plugin.swap_hands.Value;
                        Plugin.swapped = Plugin.swap_hands.Value;
                        break;
                    case 1:
                        Plugin.grab_to_use.Value = !Plugin.grab_to_use.Value;
                        DevHoldable.CanInput = !Plugin.grab_to_use.Value;
                        break;
                }

                settings = new string[] { $"Swap hands : {Plugin.swap_hands.Value}", $"Grab to use : {Plugin.grab_to_use.Value}" };
                UpdateScreen();
            }
            else
            {
                selected_rom = rom_files[rom_index];
                ReloadEmu(File.ReadAllBytes(selected_rom));
            }
        }

        public void Exit()
        {
            transform.parent.gameObject.SetActive(false);
        }

        public void ShowMenu()
        {
            transform.parent.gameObject.SetActive(!transform.parent.gameObject.activeSelf);
        }

        public void Settings()
        {
            in_settings = !in_settings;
            UpdateScreen();
        }

        void UpdateScreen()
        {
            header_text.text = in_settings ? "Settings" : "Roms";
            screen_text.text = "";

            if (in_settings)
            {
                for (int i = 0; i < settings.Length; i++)
                {
                    screen_text.text += i == setting_index ? $"> {settings[i]}\n" : $"{settings[i]}\n";
                }
            }
            else
            {
                if (no_roms) { screen_text.text = "No ROMs found"; return; }
                int roms_per_page = 11;
                int page = rom_index / roms_per_page;
                int page_start = page * roms_per_page;
                int page_end = Mathf.Min(page_start + roms_per_page, rom_files.Length);

                for (int i = page_start; i < page_end; i++)
                {
                    string rom = Path.GetFileNameWithoutExtension(rom_files[i]);
                    if (rom.Length > 30) { rom =  rom.Substring(0, 30) + "..."; }
                    screen_text.text += i == rom_index ? $"> {rom}\n" : $"{rom}\n";
                }
            }
        }
    }
}
