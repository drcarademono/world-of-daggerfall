//#define WOD_ROCKS_FULL_LOGS

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Utility;
using System.IO;
using FullSerializer;

namespace WODRocksMaterials
{
    [Serializable]
    public class MaterialDefinition
    {
        public int archive;
        public int record;
        public int frame;
    }

    [Serializable]
    public class ClimateMaterials
    {
        public MaterialDefinition[] defaultMaterials = new MaterialDefinition[0];
        public MaterialDefinition[] winterMaterials = new MaterialDefinition[0];
    }

    [Serializable]
    public class ClimateMaterialSettings
    {
        public ClimateMaterials ocean = new ClimateMaterials();
        public ClimateMaterials desert = new ClimateMaterials();
        public ClimateMaterials desert2 = new ClimateMaterials();
        public ClimateMaterials mountain = new ClimateMaterials();
        public ClimateMaterials rainforest = new ClimateMaterials();
        public ClimateMaterials swamp = new ClimateMaterials();
        public ClimateMaterials subtropical = new ClimateMaterials();
        public ClimateMaterials mountainWoods = new ClimateMaterials();
        public ClimateMaterials woodlands = new ClimateMaterials();
        public ClimateMaterials hauntedWoodlands = new ClimateMaterials();
        public ClimateMaterials mountainBalfiera = new ClimateMaterials();
        public ClimateMaterials mountainHammerfell = new ClimateMaterials();
    }

    [ImportedComponent]
    public class WODRocksMaterials : MonoBehaviour
    {
        private ClimateMaterialSettings climateMaterialSettings;
        private MeshRenderer meshRenderer;
        private static readonly fsSerializer _serializer = new fsSerializer();

        static Mod mod;
        static bool WorldOfDaggerfallBiomesModEnabled = false;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            GameObject modGameObject = new GameObject(mod.Title);
            modGameObject.AddComponent<WODRocksMaterials>();

            Mod worldOfDaggerfallBiomesMod = ModManager.Instance.GetModFromGUID("3b4319ac-34bb-411d-aa2c-d52b7b9eb69d");
            WorldOfDaggerfallBiomesModEnabled = worldOfDaggerfallBiomesMod != null && worldOfDaggerfallBiomesMod.Enabled;

            Debug.Log("WODRocksMaterials: Init called.");
        }

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
#if WOD_ROCKS_FULL_LOGS
            Debug.Log($"[WODRocksMaterials] Awake called for {gameObject.name}");
#endif
            LoadClimateMaterialSettings();
        }

        private void Start()
        {
#if WOD_ROCKS_FULL_LOGS
            Debug.Log("[WODRocksMaterials] Start called");
#endif
            UpdateMaterialBasedOnClimateAndSeason();
        }

        private void LoadClimateMaterialSettings()
        {
            string cleanName = gameObject.name.Replace("(Clone)", "").Replace(".prefab", "").Trim();
#if WOD_ROCKS_FULL_LOGS
            Debug.Log($"[WODRocksMaterials] Attempting to load JSON for '{cleanName}'");
#endif

            if (ModManager.Instance.TryGetAsset(cleanName + ".json", clone: false, out TextAsset jsonAsset))
            {
                string json = jsonAsset.text;
#if WOD_ROCKS_FULL_LOGS
                Debug.Log($"[WODRocksMaterials] JSON loaded successfully, contents: {json.Substring(0, Math.Min(json.Length, 500))}...");
#endif

                fsResult result = _serializer.TryDeserialize(fsJsonParser.Parse(json), ref climateMaterialSettings);
                if (!result.Succeeded)
                {
#if WOD_ROCKS_FULL_LOGS
                    Debug.LogError($"[WODRocksMaterials] Deserialization failed: {result.FormattedMessages}");
#endif
                }
                else
                {
#if WOD_ROCKS_FULL_LOGS
                    Debug.Log("[WODRocksMaterials] Deserialization succeeded");
#endif
                }
            }
            else
            {
                Debug.LogError("[WODRocksMaterials] JSON file for material settings not found");
                climateMaterialSettings = new ClimateMaterialSettings(); // Fallback to default
            }
        }

        private Material[] LoadMaterialsFromDefinitions(MaterialDefinition[] definitions)
        {
            if (definitions == null || definitions.Length == 0)
            {
#if WOD_ROCKS_FULL_LOGS
                Debug.LogWarning("No definitions provided to LoadMaterialsFromDefinitions.");
#endif
                return new Material[0]; // Return an empty array.
            }

            if (DaggerfallUnity.Instance == null || DaggerfallUnity.Instance.MaterialReader == null)
            {
#if WOD_ROCKS_FULL_LOGS
                Debug.LogError("DaggerfallUnity.Instance or MaterialReader is not initialized.");
#endif
                return null; // Return null or handle appropriately.
            }

            List<Material> materials = new List<Material>();
            foreach (var def in definitions)
            {
                Material loadedMaterial = null;
                Rect rectOut;

                // This is now safe to call after the null checks
                MaterialReader materialReader = DaggerfallUnity.Instance.MaterialReader;
                
                loadedMaterial = materialReader.GetMaterial(def.archive, def.record, def.frame, 0, out rectOut, 0, false, false);

                if (loadedMaterial == null)
                {
                    if (TextureReplacement.TryImportMaterial(def.archive, def.record, def.frame, out loadedMaterial))
                    {
                        materials.Add(loadedMaterial);
                    }
                    else
                    {
                        Debug.LogWarning($"Could not load material for archive: {def.archive}, record: {def.record}, frame: {def.frame}");
                        // Consider not adding nulls to the list to avoid potential issues downstream
                    }
                }
                else
                {
                    materials.Add(loadedMaterial);
                }
            }
            return materials.ToArray();
        }

        private ClimateMaterials GetMaterialsForClimate(MapsFile.Climates climate, bool isWinter)
        {
            ClimateMaterials selectedMaterials = null;

            if (!WorldOfDaggerfallBiomesModEnabled)
            {
                // Logic when the World of Daggerfall Biomes Mod is not enabled
                switch (climate)
                {
                    case MapsFile.Climates.Desert:
                    case MapsFile.Climates.Mountain:
                    case MapsFile.Climates.Rainforest:
                    case MapsFile.Climates.Swamp:
                    case MapsFile.Climates.Woodlands:
                        selectedMaterials = climateMaterialSettings.GetType().GetField(climate.ToString().ToLower()).GetValue(climateMaterialSettings) as ClimateMaterials;
                        break;
                    default:
                        selectedMaterials = GetFallbackMaterialsForClimate(climate);
                        break;
                }
            }
            else
            {
                // Original logic for selecting materials without restrictions
                selectedMaterials = climateMaterialSettings.GetType().GetField(climate.ToString().ToLower()).GetValue(climateMaterialSettings) as ClimateMaterials;
                if (selectedMaterials == null || (selectedMaterials.defaultMaterials.Length == 0 && selectedMaterials.winterMaterials.Length == 0))
                {
                    selectedMaterials = GetFallbackMaterialsForClimate(climate); // Ensure fallback is used if materials are undefined
                }
            }

            return selectedMaterials ?? climateMaterialSettings.woodlands; // Default to woodlands if no materials found
        }


        private ClimateMaterials GetFallbackMaterialsForClimate(MapsFile.Climates climate)
        {
            switch (climate)
            {
                case MapsFile.Climates.HauntedWoodlands:
                    return climateMaterialSettings.woodlands;
                case MapsFile.Climates.Desert2:
                    return climateMaterialSettings.desert;
                case MapsFile.Climates.MountainWoods:
                    return climateMaterialSettings.mountain;
                case MapsFile.Climates.Ocean:
                case MapsFile.Climates.Subtropical:
                    // Assume a generalized fallback for climates not explicitly handled
                    return climateMaterialSettings.desert;
                default:
                    return climateMaterialSettings.woodlands; // Default fallback
            }
        }

        private void UpdateMaterialBasedOnClimateAndSeason()
        {
            MapsFile.Climates currentClimate = GetCurrentClimate();
            bool isWinter = IsWinter();
            string currentRegionName = GameManager.Instance.PlayerGPS.CurrentRegionName;

            // Start with the default materials for the current climate
            ClimateMaterials materialsForClimate = GetMaterialsForClimate(currentClimate, isWinter);

            // Adjust materials based on specific regions and their climates
            if (currentClimate == MapsFile.Climates.Mountain)
            {
                string[] hammerfellRegions = new string[] { "Alik'r Desert", "Dragontail Mountains", "Dak'fron", "Lainlyn", "Tigonus", "Ephesus", "Santaki" };
                string[] balfieraRegion = new string[] { "Isle of Balfiera" };

                // Check for Balfiera region and apply Balfiera mountains setting regardless of the mod status
                if (balfieraRegion.Contains(currentRegionName) && climateMaterialSettings.mountainBalfiera != null && (climateMaterialSettings.mountainBalfiera.defaultMaterials?.Length > 0 || climateMaterialSettings.mountainBalfiera.winterMaterials?.Length > 0))
                {
                    materialsForClimate = climateMaterialSettings.mountainBalfiera;
                }
                // Apply Hammerfell mountains setting only if the World of Daggerfall - Biomes mod is present
                else if (WorldOfDaggerfallBiomesModEnabled && hammerfellRegions.Contains(currentRegionName) && climateMaterialSettings.mountainHammerfell != null && (climateMaterialSettings.mountainHammerfell.defaultMaterials?.Length > 0 || climateMaterialSettings.mountainHammerfell.winterMaterials?.Length > 0))
                {
                    materialsForClimate = climateMaterialSettings.mountainHammerfell;
                } else if (!WorldOfDaggerfallBiomesModEnabled && hammerfellRegions.Contains(currentRegionName) && climateMaterialSettings.mountainBalfiera != null && (climateMaterialSettings.mountainBalfiera.defaultMaterials?.Length > 0 || climateMaterialSettings.mountainBalfiera.winterMaterials?.Length > 0))
                {
                    materialsForClimate = climateMaterialSettings.mountainBalfiera; // Use Balfiera settings for Hammerfell if World of Daggerfall - Biomes mod not present
                }
            }

            // Load and apply materials based on the definitions
            MaterialDefinition[] definitions = isWinter ? materialsForClimate.winterMaterials : materialsForClimate.defaultMaterials;
            Material[] selectedMaterials = LoadMaterialsFromDefinitions(definitions);

            if (selectedMaterials != null && selectedMaterials.Length > 0 && meshRenderer != null)
            {
                meshRenderer.materials = selectedMaterials;
            }
            else
            {
                Debug.LogError("[WODRocksMaterials] No valid materials found for the current climate and season.");
            }
        }

        private MapsFile.Climates GetCurrentClimate()
        {
            return (MapsFile.Climates)GameManager.Instance.PlayerGPS.CurrentClimateIndex;
        }

        private bool IsWinter()
        {
            DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
            return now.SeasonValue == DaggerfallDateTime.Seasons.Winter;
        }
    }
}

