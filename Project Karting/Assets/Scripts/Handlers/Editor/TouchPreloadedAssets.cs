using UnityEditor;

namespace Handlers.Editor {
    [InitializeOnLoad]
    public class TouchPreloadedAssets
    {
        static TouchPreloadedAssets()
        {
            GetPreloadedAssets();
        }

        private static void GetPreloadedAssets()
        {
            PlayerSettings.GetPreloadedAssets();
        }
    }
}