namespace _SOURCE_.Scripts.Modules.UtilsModule.Other
{
    using System.IO;
    using UnityEditor;

    public static class PathFinder
    {
        public static string GetThisScriptFolder<T>()
        {
            string scriptFileName = typeof(T).Name + ".cs";
            // Ищем ассет с именем класса/файла (лучше указывать имя файла точно)
            var guids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(scriptFileName) + " t:Script");

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid); // Assets/.../SceneNameGenerator.cs

                if (assetPath.EndsWith("/" + scriptFileName) || assetPath.EndsWith("\\" + scriptFileName))
                    return Path.GetDirectoryName(assetPath).Replace('\\', '/');
            }

            // Если по какой-то причине имя файла отличается — попробуем найти по типу/классу не получится у static class,
            // поэтому просто сообщаем об ошибке.
            throw new FileNotFoundException($"Не найден файл {scriptFileName}. Убедись, что файл так называется и лежит в Assets.");
        }

        public static string GetGeneratedFolderPath<T>()
        {
            var folder = GetThisScriptFolder<T>();
            var generatedFolder = $"{folder}/Generated";

            if (!AssetDatabase.IsValidFolder(generatedFolder))
            {
                AssetDatabase.CreateFolder(folder, "Generated");
                AssetDatabase.Refresh();
            }
            return generatedFolder;
        }
    }
}