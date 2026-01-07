using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Anvil.WebBuilderPro
{
    internal sealed class OutputController : ControllerBase, IDisposable
    {
        private const string BuildNameLabel = "BuildNameLabel";
        private const string BuildNameButton = "BuildNameBrowseButton";
        private const string OutputPathLabel = "OutputPathLabel";
        private const string OutputPathButton = "OutputPathBrowseButton";

        internal OutputController(WebBuilderProModel model, VisualElement root) : base(model, root)
        {
            Global.Log("[OutputController] Initializing OutputController", LogFilter.Controller);
            Root.Q<Label>(BuildNameLabel).BindLabel(ProModel.BuildName);
            Root.Q<Button>(BuildNameButton).clicked += OnBuildNameBrowseeButtonClicked;

            // Bind to the output path
            Root.Q<Label>(OutputPathLabel).BindLabel(ProModel.OutputParentPath);
            Root.Q<Button>(OutputPathButton).clicked += OnOutputPathBrowseButtonClicked;
        }

        public void OnBuildNameBrowseeButtonClicked()
        {
            Global.Log("[OutputController] Opening Project Settings for Build Name", LogFilter.Controller);
            SettingsService.OpenProjectSettings("Project/Player");
        }

        public void OnOutputPathBrowseButtonClicked()
        {
            Global.Log("[OutputController] Opening folder panel for Output Path selection", LogFilter.Controller);
            // TODO: If the path already exists, prompt the user to overwrite or create a new folder. If they don't then they suck.
            string initialPath = ProModel.IsOutputParentPathValid.Value ? ProModel.OutputParentPath.Value : System.IO.Path.GetDirectoryName(Application.dataPath);
            string selectedPath = EditorUtility.OpenFolderPanel("Select Output Folder", initialPath, "");

            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (System.IO.Directory.Exists(selectedPath))
                {
                    Global.Log("[OutputController] Valid output path selected: " + selectedPath, LogFilter.Controller);
                    ProModel.OutputParentPath.Value = selectedPath;
                    ProModel.IsOutputParentPathValid.Value = true;
                }
                else
                {
                    Global.Log("[OutputController] Invalid output path selected", LogFilter.Controller);
                    ProModel.OutputParentPath.Value = "No Valid Path Selected";
                    ProModel.IsOutputParentPathValid.Value = false;
                    Debug.LogWarning("Selected folder does not exist.");
                }
            }
            else
            {
                Global.Log("[OutputController] No output path selected", LogFilter.Controller);
            }
        }

        public void Dispose()
        {
            Global.Log("[OutputController] Disposing OutputController", LogFilter.Controller);
            // Unbind events
            Root.Q<Button>(BuildNameButton).clicked -= OnBuildNameBrowseeButtonClicked;
            Root.Q<Button>(OutputPathButton).clicked -= OnOutputPathBrowseButtonClicked;
        }
    }
}