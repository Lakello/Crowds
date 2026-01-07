namespace _SOURCE_.Scripts.Modules.UtilsModule.Other
{
    using System.Linq;
    using UnityEditor;

    public static class DefineController
    {
        public static void TryAddDefine(string define)
        {
            if (string.IsNullOrWhiteSpace(define))
                return;

            define = define.Trim();

            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            // Разбиваем, чистим, убираем пустые
            var defines = new System.Collections.Generic.HashSet<string>(
                definesString.Split(';')
                    .Select(d => d.Trim())
                    .Where(d => !string.IsNullOrEmpty(d))
            );

            // Если уже есть — ничего не делаем
            if (!defines.Add(define))
                return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", defines));
        }

        public static void TryRemoveDefine(string define)
        {
            if (string.IsNullOrWhiteSpace(define))
                return;

            define = define.Trim();

            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            var defines = new System.Collections.Generic.HashSet<string>(
                definesString.Split(';')
                    .Select(d => d.Trim())
                    .Where(d => !string.IsNullOrEmpty(d))
            );

            // Если не было — ничего не делаем
            if (!defines.Remove(define))
                return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", defines));
        }
    }
}