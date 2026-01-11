#if UNITY_EDITOR
namespace UtilsModule.Other
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    namespace SceneSystem.Editor
    {
        public class SceneNameGenerator : EditorWindow
        {
            private const string SceneNameDefine = "SCENE_NAME_GENERATED";
            private const string SaveKey = nameof(SceneNameGenerator);

            private VisualElement _root;
            private Dictionary<string, bool> _generateScenes;

            [Serializable]
            private struct SaveData
            {
                public string SceneName;
                public bool IsGenerateScene;
            }

            [MenuItem("Tools/UpdateSceneNames")]
            public static void ShowWindow()
            {
                GetWindow<SceneNameGenerator>("Update Scene Names");
            }

            private void OnDisable()
            {
                Save();
            }

            private void CreateGUI()
            {
                var root = rootVisualElement;

                var scroll = new ScrollView(ScrollViewMode.Vertical);
                scroll.style.flexGrow = 1;
                scroll.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f));
                scroll.elasticity = 5;
                root.Add(scroll);
                _root = scroll;

                if (EditorApplication.isPlaying)
                {
                    _root.Clear();

                    var info = new HelpBox
                    {
                        text = "In PlayMode"
                    };

                    _root.Add(info);

                    return;
                }

                BuildRoot(_root);
            }

            private void BuildRoot(VisualElement root)
            {
                root.Clear();
                
                RefreshSceneList();

                AddSpace(height: 10);

                // = SHOW SCENES =

                if (_generateScenes.Count == 0)
                {
                    Label noFoundLabel = new Label();
                    noFoundLabel.text = "No scenes found in Build Settings.";
                    noFoundLabel.style.alignSelf = new StyleEnum<Align>(Align.Center);
                    root.Add(noFoundLabel);
                }
                else
                {
                    var sceneNameScope = new VisualElement();
                    sceneNameScope.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                    sceneNameScope.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
                    sceneNameScope.style.alignSelf = new StyleEnum<Align>(Align.Stretch);
                    sceneNameScope.style.flexGrow = 1;
                    SetOutline(sceneNameScope);

                    AddSpace(height: 10, r: sceneNameScope);

                    Label title = new Label();
                    title.text = "Scenes in Build Settings:";
                    title.style.fontSize = 18;
                    title.style.alignSelf = new StyleEnum<Align>(Align.Center);

                    sceneNameScope.Add(title);

                    AddSpace(height: 10, r: sceneNameScope);

                    foreach (var sceneName in _generateScenes.Keys)
                    {
                        var horizontalScope = new VisualElement();
                        horizontalScope.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                        horizontalScope.style.justifyContent = new StyleEnum<Justify>(Justify.SpaceAround);
                        horizontalScope.style.flexGrow = 1;
                        horizontalScope.style.minHeight = 30;
                        SetOutline(horizontalScope, Color.darkGray, 1);

                        AddSpace(width: 10, r: horizontalScope);

                        Toggle generateSceneMark = new Toggle();
                        generateSceneMark.style.minWidth = 10;
                        generateSceneMark.style.alignSelf = new StyleEnum<Align>(Align.Center);
                        generateSceneMark.value = _generateScenes[sceneName];
                        generateSceneMark.RegisterValueChangedCallback(valueChange =>
                        {
                            _generateScenes[sceneName] = valueChange.newValue;
                            Save();
                        });

                        horizontalScope.Add(generateSceneMark);

                        Label sceneNameLabel = new Label();
                        sceneNameLabel.text = $"• {sceneName}";
                        sceneNameLabel.style.flexGrow = 1;
                        sceneNameLabel.style.fontSize = 14;
                        sceneNameLabel.style.alignSelf = new StyleEnum<Align>(Align.Center);
                        horizontalScope.Add(sceneNameLabel);

                        sceneNameScope.Add(horizontalScope);
                    }

                    root.Add(sceneNameScope);
                }

                // =======

                AddSpace(height: 20);

                // = BOTTOM BUTTONS =

                var verticalScope = new VisualElement();
                verticalScope.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                verticalScope.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
                verticalScope.style.alignSelf = new StyleEnum<Align>(Align.Stretch);
                verticalScope.style.flexGrow = 1;

                var generateButton = new Button();
                generateButton.text = "GENERATE";
                generateButton.style.minWidth = 20;
                generateButton.style.minHeight = 30;
                SetOutline(generateButton);
                generateButton.style.color = Color.white;
                generateButton.clicked += GenerateSceneEnum;

                verticalScope.Add(generateButton);

                var refreshButton = new Button();
                refreshButton.text = "REFRESH";
                refreshButton.style.minWidth = 20;
                refreshButton.style.minHeight = 30;
                SetOutline(refreshButton);
                refreshButton.style.color = Color.white;
                refreshButton.clicked += () => BuildRoot(_root);

                verticalScope.Add(refreshButton);

                root.Add(verticalScope);

                // =======

                return;

                void AddSpace(int height = 1, int width = 1, VisualElement r = null)
                {
                    VisualElement space = new VisualElement();
                    space.style.height = height;
                    space.style.width = width;

                    r ??= root;

                    r.Add(space);
                }
            }

            private void RefreshSceneList()
            {
                Load();

                var sceneNames = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                    .ToArray();

                foreach (var sceneName in sceneNames)
                {
                    _generateScenes.TryAdd(sceneName, true);
                }

                List<string> removeNames = new List<string>();

                foreach (var sceneName in _generateScenes.Keys)
                {
                    if (sceneNames.Contains(sceneName) == false)
                    {
                        removeNames.Add(sceneName);
                    }
                }

                foreach (var sceneName in removeNames)
                {
                    _generateScenes.Remove(sceneName);
                }

                Save();
            }

            private void GenerateSceneEnum()
            {
                if (_generateScenes.Count == 0)
                {
                    EditorUtility.DisplayDialog("Error", "No scenes are enabled in Build Settings.", "OK");
                    return;
                }

                EnumGenerator.GenerateEnum(
                    _generateScenes
                        .Where(d => d.Value)
                        .Select(d => d.Key)
                        .ToArray(),
                    "SceneName",
                    PathFinder.GetGeneratedFolderPath<SceneNameGenerator>(),
                    "SceneSystem");

                DefineController.TryAddDefine(SceneNameDefine);
            }

            private void SetOutline(VisualElement element, int outlineWidth = 2)
            {
                SetOutline(element, Color.black, outlineWidth);
            }

            private void SetOutline(VisualElement element, Color color, int outlineWidth = 2)
            {
                StyleColor styleColor = new StyleColor(color);

                element.style.borderBottomColor = styleColor;
                element.style.borderTopColor = styleColor;
                element.style.borderLeftColor = styleColor;
                element.style.borderRightColor = styleColor;

                element.style.borderBottomWidth = outlineWidth;
                element.style.borderTopWidth = outlineWidth;
                element.style.borderLeftWidth = outlineWidth;
                element.style.borderRightWidth = outlineWidth;
            }

            private void Save()
            {
                if (_generateScenes == null)
                {
                    return;
                }

                SaveData[] data = _generateScenes
                    .Select(s => new SaveData
                    {
                        SceneName = s.Key,
                        IsGenerateScene = s.Value,
                    }).ToArray();

                EditorPrefs.SetString(SaveKey, JsonConvert.SerializeObject(data));
            }

            private void Load()
            {
                string data = EditorPrefs.GetString(SaveKey, null);

                if (string.IsNullOrEmpty(data) == false)
                {
                    SaveData[] saves = JsonConvert.DeserializeObject<SaveData[]>(data);

                    _generateScenes = saves.ToDictionary(s => s.SceneName, s => s.IsGenerateScene);
                }
                else
                {
                    _generateScenes = new Dictionary<string, bool>();
                }
            }
        }
    }
}
#endif