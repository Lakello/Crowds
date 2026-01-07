using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anvil.WebBuilderPro
{
    internal sealed class WebBuilderProWindow : EditorWindow
    {
        [SerializeField] private WebBuilderProModel model;
        [SerializeField] private VisualTreeAsset treeAsset;

        private CancellationTokenSource cancellationTokenSource;
        private const float updateInterval = 0.2f;

        private Toggle advancedOptionsToggle;

        private OutputController outputController;
        private BuildOptionsController buildOptionsController;
        private ErrorsController errorsController;
        private BuildController buildController;
        private ReviewController reviewController;

        [MenuItem("Tools/Web Builder Pro")]
        public static void ShowWindow()
        {
            if(GetWindow<WebBuilderProWindow>())
            {
                GetWindow<WebBuilderProWindow>().Close();
            }

            var wnd = GetWindow<WebBuilderProWindow>();
            wnd.titleContent = new GUIContent("Web Builder Pro");
            wnd.minSize = new Vector2(240, 100); // Set the minimum size
        }

        public void CreateGUI()
        {
            if(model == null)
                throw new Exception("WebBuilderProModel is null");

            if(treeAsset == null)
                throw new Exception("VisualTreeAsset is null");

            rootVisualElement.Add(treeAsset.CloneTree());
            advancedOptionsToggle = rootVisualElement.Q<Toggle>("AdvancedOptionsToggle");   
            advancedOptionsToggle.BindToggle(model.AdvancedOptions);

            // DEVELOPMENT ONLY:
            rootVisualElement.Q<Button>("RefreshButton").clicked += RefreshWindow;

            outputController = new OutputController(model, rootVisualElement);
            buildOptionsController = new BuildOptionsController(model, rootVisualElement);
            errorsController = new ErrorsController(model, rootVisualElement);
            buildController = new BuildController(model, rootVisualElement);
            reviewController = new ReviewController(model, rootVisualElement);

            StartPollingUpdates();
            model.ForceNotify();
        }

        // Called when the window is destroyed to stop the task
        private void OnDestroy()
        {
            advancedOptionsToggle.UnBind(model.AdvancedOptions);
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            // Dispose controllers.
            outputController.Dispose();
            buildOptionsController.Dispose();
            errorsController.Dispose();
            buildController.Dispose();
            reviewController.Dispose();

            LocalHostRunner.Stop();

            // Unbind all events form the obserables.
            model.UnBind();


        }

        private void StartPollingUpdates()
        {
            cancellationTokenSource = new CancellationTokenSource();
            StartUpdateTask(cancellationTokenSource.Token);
        }

        // Task to periodically call model.Update
        private async void StartUpdateTask(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Call model.Update() every updateInterval seconds
                    model.Update();

                    // Wait for the update interval
                    await Task.Delay(TimeSpan.FromSeconds(updateInterval), cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, handle the exception if necessary
            }
        }

        public void RefreshWindow()
        {
            Close();
            ShowWindow();
        }
    }
}
