#if SCENE_NAME_GENERATED
namespace _GameResources.Scripts.Network.SceneSystem
{
    using System;
    using System.Collections.Generic;
    using global::SceneSystem;
    using ZLinq;

    public static class SceneNameExtensions
    {
        private static Dictionary<string, SceneName> _allNames;
        
        public static bool ContainsScene(this SceneName[] names, string name)
        {
            return names.AsValueEnumerable().Select(n => n.ToString()).Contains(name);
        }
        
        public static bool ContainsScene(this SceneName[] names, SceneName name)
        {
            return names.AsValueEnumerable().Contains(name);
        }
        
        public static SceneName ToSceneName(this string name)
        {
            _allNames ??= Enum.GetValues(typeof(SceneName))
                .AsValueEnumerable()
                .Cast<SceneName>()
                .ToDictionary(n => n.ToString(), n => n);

            return _allNames[name];
        }
    }
}
#endif
