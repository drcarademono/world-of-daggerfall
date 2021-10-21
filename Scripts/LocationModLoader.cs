using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;   //required for modding features

public class LocationModLoader : MonoBehaviour
{
    public static Mod mod { get; private set; }

    [Invoke(StateManager.StateTypes.Start, 0)]
    public static void Init(InitParams initParams)
    {
        // Get mod
        mod = initParams.Mod;
        // Add script to the scene.

        mod.UnloadAssetBundle(true);

        new GameObject("LocationLoader").AddComponent<DaggerfallWorkshop.Loc.LocationLoader>(); ;
        mod.IsReady = true;
    }
}
