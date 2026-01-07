using UnityEditor;

namespace Anvil.WebBuilderPro
{
    /// <summary>
    /// Reset values when the Editor loads. This is important if the editor crashes during a build process.
    /// </summary>
    public static class ResetValuesOnLoad
    {
        [InitializeOnLoadMethod]
        private static void ResetValues()
        {
            string[] guids = AssetDatabase.FindAssets("t:WebBuilderProModel");
        }
    }
}