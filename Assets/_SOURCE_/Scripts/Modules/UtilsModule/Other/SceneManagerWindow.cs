#if UNITY_EDITOR
namespace UtilsModule.Other
{
    using System.IO;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;

    public class SceneManagerWindow : EditorWindow
    {
        private string _lastScene;
        private VisualElement _root;
        private bool _playOnFirstScene;
        private Sprite _updateButtonIcon;

        [MenuItem("Window/General/Scene Manager")]
        public static void ShowWindow()
        {
            GetWindow<SceneManagerWindow>("Scene Manager");
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChange;
            EditorApplication.playModeStateChanged += OnEditorApplicationOnplayModeStateChanged;

            _playOnFirstScene = EditorPrefs.GetBool(nameof(_playOnFirstScene), false);
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

            _root.Clear();
            BuildRoot(_root);
        }

        private void OnEditorApplicationOnplayModeStateChanged(PlayModeStateChange change)
        {
            if (_root == null)
            {
                return;
            }

            if (change is PlayModeStateChange.EnteredEditMode)
            {
                _root.Clear();
                BuildRoot(_root);
            }
        }

        private void BuildRoot(VisualElement root)
        {
            var defaultColor = root.style.backgroundColor;

            var topHorizontalScope = new VisualElement();
            topHorizontalScope.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            topHorizontalScope.style.justifyContent = new StyleEnum<Justify>(Justify.SpaceAround);
            topHorizontalScope.style.flexGrow = 1;

            var playToggle = new Toggle();
            playToggle.text = "Play on first scene";
            playToggle.value = _playOnFirstScene;
            topHorizontalScope.Add(playToggle);

            var updateButton = new Button();
            updateButton.style.backgroundImage = new StyleBackground(GetUpdateButtonSprite());
            updateButton.style.minWidth = 10;
            SetOutline(updateButton);
            updateButton.style.backgroundColor = defaultColor;
            topHorizontalScope.Add(updateButton);

            updateButton.clicked += () =>
            {
                _root.Clear();
                BuildRoot(_root);
            };

            playToggle.RegisterValueChangedCallback(valueChange =>
            {
                _playOnFirstScene = valueChange.newValue;
                EditorPrefs.SetBool(nameof(_playOnFirstScene), _playOnFirstScene);
            });

            root.Add(topHorizontalScope);

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = Path.GetFileNameWithoutExtension(path);
                bool isOpenScene = EditorSceneManager.GetSceneByName(sceneName).name != null;

                var isActiveScene = EditorSceneManager.GetActiveScene().name == sceneName && EditorSceneManager.sceneCount == 1;

                var horizontalScope = new VisualElement();
                horizontalScope.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                horizontalScope.style.justifyContent = new StyleEnum<Justify>(Justify.SpaceAround);
                horizontalScope.style.flexGrow = 1;

                var openSceneButton = new Button();
                openSceneButton.text = sceneName;
                openSceneButton.style.flexGrow = 1;
                openSceneButton.style.color = isOpenScene || isActiveScene
                    ? Color.black
                    : Color.white;
                openSceneButton.style.backgroundColor = isActiveScene
                    ? Color.grey
                    : defaultColor;
                openSceneButton.style.borderBottomColor = new StyleColor(Color.black);
                SetOutline(openSceneButton);

                var openAdditiveSceneButton = new Button();
                openAdditiveSceneButton.text = isOpenScene
                    ? "x"
                    : "+";
                openAdditiveSceneButton.style.minWidth = 10;
                SetOutline(openAdditiveSceneButton);
                openAdditiveSceneButton.style.color = isOpenScene || isActiveScene
                    ? Color.black
                    : Color.white;
                openAdditiveSceneButton.style.backgroundColor = isOpenScene || isActiveScene
                    ? Color.grey
                    : defaultColor;

                openSceneButton.clicked += () =>
                {
                    if (SceneManager.GetActiveScene().name == sceneName && SceneManager.sceneCount <= 1) return;

                    if (SceneManager.GetActiveScene().name == "Untitled")
                    {
                        return;
                    }

                    EditorSceneManager.SaveOpenScenes();
                    EditorSceneManager.OpenScene(path);

                    root.Clear();
                    BuildRoot(root);
                };

                var sceneNumber = i;

                openAdditiveSceneButton.clicked += () =>
                {
                    if (isActiveScene)
                    {
                        return;
                    }

                    if (isOpenScene && EditorSceneManager.sceneCount != 1)
                    {
                        EditorSceneManager.SaveOpenScenes();
                        EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByBuildIndex(sceneNumber), true);
                    }
                    else
                    {
                        EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    }

                    root.Clear();
                    BuildRoot(root);
                };

                horizontalScope.Add(openSceneButton);
                horizontalScope.Add(openAdditiveSceneButton);

                root.Add(horizontalScope);
            }
        }

        private Sprite GetUpdateButtonSprite()
        {
            if (_updateButtonIcon != null)
            {
                return _updateButtonIcon;
            }

            _updateButtonIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Doozy/_Extras/Sprites/Icons/arrows-rotate_solid.png");
            return _updateButtonIcon;
        }

        private void OnPlayModeChange(PlayModeStateChange state)
        {
            if (!_playOnFirstScene)
            {
                return;
            }

            if (state == PlayModeStateChange.ExitingEditMode)
            {
                _lastScene = EditorSceneManager.GetActiveScene().path;
                var zeroScene = SceneUtility.GetScenePathByBuildIndex(0);
                EditorSceneManager.SaveOpenScenes();
                EditorSceneManager.OpenScene(zeroScene);
                EditorApplication.EnterPlaymode();
            }

            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorSceneManager.OpenScene(_lastScene);
                _lastScene = null;
            }
        }

        private void SetOutline(VisualElement element)
        {
            element.style.borderBottomColor = new StyleColor(Color.black);
            element.style.borderTopColor = new StyleColor(Color.black);
            element.style.borderLeftColor = new StyleColor(Color.black);
            element.style.borderRightColor = new StyleColor(Color.black);

            var outlineWidth = 2;

            element.style.borderBottomWidth = outlineWidth;
            element.style.borderTopWidth = outlineWidth;
            element.style.borderLeftWidth = outlineWidth;
            element.style.borderRightWidth = outlineWidth;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChange;
            EditorApplication.playModeStateChanged -= OnEditorApplicationOnplayModeStateChanged;
        }
    }
}

#endif