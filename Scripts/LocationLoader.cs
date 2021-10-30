using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Loc
{
    public class LocationLoader : MonoBehaviour
    {
        List<LocationInstance> locationInstance = new List<LocationInstance>();
        Dictionary<int, Dictionary<string, Mod>> modRegionFiles = new Dictionary<int, Dictionary<string, Mod>>();

        Dictionary<string, Mod> modLocationPrefabs = new Dictionary<string, Mod>();

        const float TERRAIN_SIZE = 128;
        const float TERRAIN_HEIGHT_MAX = 1923.75f;
        const float TERRAINPIXELSIZE = 819.2f;
        const float OCEAN_SURFACE = 0.018f;
        const float TERRAIN_SIZE_MULTI = TERRAINPIXELSIZE / TERRAIN_SIZE;
        const string locations = "/Locations/";
        private int regionIndex;

        void Awake()
        {
            DaggerfallTerrain.OnPromoteTerrainData += AddLocation;
            PlayerGPS.OnRegionIndexChanged += OnRegionChanged;
            regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
        }

        private void Start()
        {
            CacheLocationPrefabs();
        }

        void CacheLocationPrefabs()
        {
            foreach (Mod mod in ModManager.Instance.Mods)
            {
                if (!mod.Enabled)
                    continue;

                if (mod.AssetBundle && mod.AssetBundle.GetAllAssetNames().Length > 0)
                {
                    string dummyFilePath = mod.AssetBundle.GetAllAssetNames()[0];
                    string modFolderPrefix = dummyFilePath.Substring(17);
                    modFolderPrefix = dummyFilePath.Substring(0, 17 + modFolderPrefix.IndexOf('/'));

                    string regionFolder = modFolderPrefix + "/locations/locationprefab/";

                    foreach (string filename in mod.AssetBundle.GetAllAssetNames()
                        .Where(file => file.StartsWith(regionFolder, System.StringComparison.InvariantCultureIgnoreCase) && file.EndsWith(".txt", System.StringComparison.InvariantCultureIgnoreCase))
                        .Select(file => Path.GetFileName(file)))
                    {
                        modLocationPrefabs[filename] = mod;
                    }
                }
#if UNITY_EDITOR
                else if (mod.IsVirtual && mod.ModInfo.Files.Count > 0)
                {
                    string dummyFilePath = mod.ModInfo.Files[0];
                    string modFolderPrefix = dummyFilePath.Substring(17);
                    modFolderPrefix = dummyFilePath.Substring(0, 17 + modFolderPrefix.IndexOf('/'));

                    string regionFolder = modFolderPrefix + "/Locations/LocationPrefab/";

                    foreach (string filename in mod.ModInfo.Files
                        .Where(file => file.StartsWith(regionFolder, System.StringComparison.InvariantCultureIgnoreCase) && file.EndsWith(".txt", System.StringComparison.InvariantCultureIgnoreCase))
                        .Select(file => Path.GetFileName(file)))
                    {
                        modLocationPrefabs[filename] = mod;
                    }
                }
#endif                
            }

            string looseLocationFolder = Path.Combine(Application.dataPath, LocationHelper.locationInstanceFolder);
            string looseLocationPrefabFolder = Path.Combine(looseLocationFolder, "LocationPrefab");
            bool hasLooseFiles = Directory.Exists(looseLocationFolder) && Directory.Exists(looseLocationPrefabFolder);
            if(hasLooseFiles)
            {
                foreach(string filename in Directory.GetFiles(looseLocationFolder)
                    .Where(file => file.EndsWith(".txt"))
                    .Select(file => Path.GetFileName(file)))
                {
                    modLocationPrefabs[filename] = null;
                }
            }
        }

        void OnRegionChanged(int regionIndex)
        {
            CacheRegionFileNames(regionIndex);

            string looseLocationFolder = Path.Combine(Application.dataPath, LocationHelper.locationInstanceFolder);
            string looseLocationRegionFolder = Path.Combine(looseLocationFolder, regionIndex.ToString());
            bool hasLooseFiles = Directory.Exists(looseLocationFolder) && Directory.Exists(looseLocationRegionFolder);

            Dictionary<string, Mod> regionFiles = modRegionFiles[regionIndex];
            foreach(var kvp in regionFiles)
            {
                string filename = kvp.Key;
                Mod mod = kvp.Value;

                if(hasLooseFiles)
                {
                    string looseFileLocation = Path.Combine(looseLocationRegionFolder, filename);
                    if(File.Exists(looseFileLocation))
                    {
                        locationInstance.AddRange(LocationHelper.LoadLocationInstance(looseFileLocation));
                        continue;
                    }
                }

                locationInstance.AddRange(LocationHelper.LoadLocationInstance(mod, filename));
            }
        }

        void CacheRegionFileNames(int regionIndex)
        {
            if (!modRegionFiles.ContainsKey(regionIndex))
            {
                Dictionary<string, Mod> regionFiles = new Dictionary<string, Mod>();
                modRegionFiles.Add(regionIndex, regionFiles);

                foreach (Mod mod in ModManager.Instance.Mods)
                {
                    if (!mod.Enabled)
                        continue;

                    if (mod.AssetBundle && mod.AssetBundle.GetAllAssetNames().Length > 0)
                    {
                        string dummyFilePath = mod.AssetBundle.GetAllAssetNames()[0];
                        string modFolderPrefix = dummyFilePath.Substring(17);
                        modFolderPrefix = dummyFilePath.Substring(0, 17 + modFolderPrefix.IndexOf('/'));

                        string regionFolder = modFolderPrefix + "/locations/" + regionIndex.ToString();

                        foreach (string filename in mod.AssetBundle.GetAllAssetNames()
                            .Where(file => file.StartsWith(regionFolder, System.StringComparison.InvariantCultureIgnoreCase) && file.EndsWith(".txt", System.StringComparison.InvariantCultureIgnoreCase))
                            .Select(file => Path.GetFileName(file)))
                        {
                            regionFiles[filename] = mod;
                        }
                    }
#if UNITY_EDITOR
                    else if (mod.IsVirtual && mod.ModInfo.Files.Count > 0)
                    {
                        string dummyFilePath = mod.ModInfo.Files[0];
                        string modFolderPrefix = dummyFilePath.Substring(17);
                        modFolderPrefix = dummyFilePath.Substring(0, 17 + modFolderPrefix.IndexOf('/'));

                        string regionFolder = modFolderPrefix + "/Locations/" + regionIndex.ToString();

                        foreach (string filename in mod.ModInfo.Files
                            .Where(file => file.StartsWith(regionFolder, System.StringComparison.InvariantCultureIgnoreCase) && file.EndsWith(".txt", System.StringComparison.InvariantCultureIgnoreCase))
                            .Select(file => Path.GetFileName(file)))
                        {
                            regionFiles[filename] = mod;
                        }
                    }
#endif                   
                }
            }
        }

        void AddLocation(DaggerfallTerrain daggerTerrain, TerrainData terrainData)
        {
            //Destroy old locations by going through all the child objects, but 
            //don't delete the billboard batch (The surrounding vegettion)
            foreach (Transform child in daggerTerrain.gameObject.transform)
            {
                if (!child.GetComponent<DaggerfallBillboardBatch>())
                    Destroy(child.gameObject);
            }

            foreach (LocationInstance loc in locationInstance)
            {
                if (daggerTerrain.MapPixelX != loc.worldX || daggerTerrain.MapPixelY != loc.worldY)
                   continue;

                if (loc.terrainX <= 0 || loc.terrainY <= 0 || (loc.terrainX > 128 || loc.terrainY > 128))
                {
                    Debug.LogWarning("Invalid Location at " + daggerTerrain.MapPixelX + " : " + daggerTerrain.MapPixelY + " : The location pixelX + or/and pixelY must be higher than 0 and lower than 128");
                    continue;
                }

                //Now that we ensured that it is a valid location, then load the locationpreset
                string assetName = loc.prefab + ".txt";

                if(!modLocationPrefabs.TryGetValue(assetName, out Mod mod))
                {
                    Debug.LogWarning("Can't find location Preset: " + loc.prefab);
                    continue;
                }

                LocationPrefab locationPrefab = mod != null
                    ? LocationHelper.LoadLocationPrefab(mod, assetName)
                    : LocationHelper.LoadLocationPrefab(Application.dataPath + LocationHelper.locationPrefabFolder + assetName);

                if (locationPrefab == null)
                {
                    Debug.LogWarning($"Location Preset '{loc.prefab}' could not be parsed");
                    continue;
                }

                if ((loc.terrainX + locationPrefab.height > 128 || loc.terrainY + locationPrefab.width > 128))
                {
                    Debug.LogWarning("Invalid Location at " + daggerTerrain.MapPixelX + " : " + daggerTerrain.MapPixelY + " : The locationpreset exist outside the terrain");
                    continue;
                }

                if ((loc.terrainX + locationPrefab.height > 127 || loc.terrainY + locationPrefab.width > 127))
                {
                    Debug.LogWarning("Invalid Location at " + daggerTerrain.MapPixelX + " : " + daggerTerrain.MapPixelY + " : The locationpreset must be 1 pixel away (both X and Y) from the terrainBorder");
                    continue;
                }

                //Smooth the terrain
                if (loc.type == 0)
                {
                    daggerTerrain.MapData.hasLocation = true;
                    //daggerTerrain.MapData.locationName = loc.name;
                    daggerTerrain.MapData.locationRect = new Rect(loc.terrainX, loc.terrainY, locationPrefab.width, locationPrefab.height);

                    int count = 0;
                    float tmpAverageHeight = 0;

                    for (int x = loc.terrainX; x <= loc.terrainX + locationPrefab.width; x++)
                    {
                        for (int y = loc.terrainY; y <= loc.terrainY + locationPrefab.height; y++)
                        {
                            tmpAverageHeight += daggerTerrain.MapData.heightmapSamples[y, x];
                            count++;
                        }
                    }

                    daggerTerrain.MapData.averageHeight = tmpAverageHeight /= count;
                    //TerrainHelper.BlendLocationTerrain(ref daggerTerrain.MapData); //orginal alternative
                    
                    for (int x = 1; x <= 127; x++)
                        for (int y = 1; y <= 127; y++)
                            daggerTerrain.MapData.heightmapSamples[y, x] = Mathf.Lerp(daggerTerrain.MapData.heightmapSamples[y, x], daggerTerrain.MapData.averageHeight, 1 / (GetDistanceFromRect(daggerTerrain.MapData.locationRect, new Vector2(x, y)) + 1));
                    
                    terrainData.SetHeights(0, 0, daggerTerrain.MapData.heightmapSamples);
                }

                foreach (LocationPrefab.LocationObject obj in locationPrefab.obj)
                {
                    if (!LocationHelper.ValidateValue(obj.type, obj.name))
                        continue;

                    GameObject go = LocationHelper.LoadObject(obj.type, obj.name, daggerTerrain.gameObject.transform,
                    new Vector3((loc.terrainX * TERRAIN_SIZE_MULTI) + obj.pos.x, (daggerTerrain.MapData.averageHeight * TERRAIN_HEIGHT_MAX) + obj.pos.y, (loc.terrainY * TERRAIN_SIZE_MULTI) + obj.pos.z),
                                 obj.rot,
                                 new Vector3(obj.scale.x, obj.scale.y, obj.scale.z), loc.locationID, obj.objectID
                        );

                    if (go.GetComponent<DaggerfallBillboard>())
                    {
                        float tempY = go.transform.position.y;
                        go.GetComponent<DaggerfallBillboard>().AlignToBase();
                        go.transform.position = new Vector3(go.transform.position.x, tempY + ((go.transform.position.y - tempY) * go.transform.localScale.y), go.transform.position.z);
                    }

                    if (!go.GetComponent<DaggerfallLoot>())
                        go.isStatic = true;
                }

                continue;
            }
        }

        private float GetDistanceFromRect(Rect rect, Vector2 point)
        {
            float squared_dist = 0.0f;

            if (point.x > rect.xMax)
                squared_dist += (point.x - rect.xMax) * (point.x - rect.xMax);
            else if (point.x < rect.xMin)
                squared_dist += (rect.xMin - point.x) * (rect.xMin - point.x);

            if (point.y > rect.yMax)
                squared_dist += (point.y - rect.yMax) * (point.y - rect.yMax);
            else if (point.y < rect.yMin)
                squared_dist += (rect.yMin - point.y) * (rect.yMin - point.y);

            return Mathf.Sqrt(squared_dist);
        }

    }
}
