using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Anvil.WebBuilderPro
{
    internal sealed class BuildController : ControllerBase, IDisposable
    {
        private const string BuildButtonName = "BuildButton";
        private readonly Button buildButton;

        private BuildSettings buildSettings;

        internal BuildController(WebBuilderProModel proModel, VisualElement root) : base(proModel, root)
        {
            Global.Log("BuildController constructor", LogFilter.Controller);
            buildButton = Root.Q<Button>(BuildButtonName);
            buildButton.clicked += Build;

            proModel.IsOutputParentPathValid.OnValueChanged += DetermineButtonState;
            proModel.DevelopmentBuild.OnValueChanged += DetermineButtonState;
            proModel.ErrorsPresent.OnValueChanged += DetermineButtonState;


            proModel.DevelopmentBuild.OnValueChanged += UpdateBuildButtonTooltip;
            proModel.ErrorsPresent.OnValueChanged += UpdateBuildButtonTooltip;
            UpdateBuildButtonTooltip(false);
        }

        private void DetermineButtonState(bool Value)
        {
            Global.Log("BuildController.DetermineButtonState", LogFilter.Controller);
            buildButton.SetEnabled(ProModel.IsOutputParentPathValid.Value && !ProModel.ErrorsPresent.Value);

            buildButton.text = GetBuildButtonText();
        }

        private string GetBuildButtonText()
        {
            Global.Log("BuildController.GetBuildButtonText", LogFilter.Controller);
            if (!ProModel.IsOutputParentPathValid.Value)
                return "Build (Select Output Path)";

            if (ProModel.ErrorsPresent.Value)
                return "Build (Errors Present)";

            if (ProModel.DevelopmentBuild.Value)
                return "Build (Development)";

            return "Build";
        }

        private void UpdateBuildButtonTooltip(bool Value)
        {
            Global.Log("BuildController.UpdateBuildButtonTooltip", LogFilter.Controller);
            string tooltip = !ProModel.DevelopmentBuild.Value ?
                "Create an optimized WebGL build. Will result in small file size, but build will take longer." :
                "Create a fast WebGL build. Will result in a quick build time, but the build size will be mucher larger.";

            if (ProModel.ErrorsPresent.Value)
            {
                tooltip += "\n\nPlease fix any outstanding errors before building.";
            }

            if (!ProModel.IsOutputParentPathValid.Value)
            {
                tooltip += "\n\nPlease select a valid output path to place the build into!";
            }

            buildButton.tooltip = tooltip;
        }

        private void Build()
        {
            Global.Log("BuildController.Build()", LogFilter.Controller);
            // Stop the local host if it's still running.
            if (LocalHostRunner.IsRunning)
            {
                LocalHostRunner.Stop();
            }

            // First, check if there's already a folder.
            if (Directory.Exists(ProModel.DesiredOutPath.Value))
            {
                Global.Log("\tBuildController: Directory Exists, Asking to overwrite.", LogFilter.Controller);

                bool overwrite = EditorUtility.DisplayDialog(
                    "Overwrite Existing Folder",
                    "The output folder already exists. Would you like to overwrite it?",
                    "Yes",
                    "No"
                );

                if (!overwrite)
                {
                    return;
                }
                Global.Log("\tBuildController: Deleting Old Directory.", LogFilter.Controller);
                // delete the folder
                Directory.Delete(ProModel.DesiredOutPath.Value, true);
            }

            Global.Log($"\tBuildController: Creating Directory at {ProModel.DesiredOutPath.Value}.", LogFilter.Controller);
            // Create the folder
            Directory.CreateDirectory(ProModel.DesiredOutPath.Value);

            SetBuildSettings();

            if (ProModel.EnhancedWebGLTemplate.Value)
            {
                SetupEnhancedWebGLTemplate();
            }

            // Build the project to output path
            // Get all enabled scenes from the Build Settings
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            var buildPlayerOptions = new BuildPlayerOptions
            {
                // scene is active scene
                scenes = scenes,
                locationPathName = ProModel.DesiredOutPath.Value,
                target = EditorUserBuildSettings.activeBuildTarget,
                options = BuildOptions.None
            };

            // Mark the build as being from this asset.
            ProModel.IsBuildFromThisAsset.Value = true;

            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = buildReport.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Global.ProModel.ConfirmSuccessfulBuild();
                Global.Log("BuildController: Build succeeded", LogFilter.Controller);
                LoadBuildSettings();
                // TODO: Build is complete, save and store the build report.
            }
            else
            {
                Global.Log("BuildController: Build failed", LogFilter.Controller);
                LoadBuildSettings();
                Debug.LogError("Build failed: " + summary.outputPath);
                // if the build fails, remove the directory we created for it
                Directory.Delete(ProModel.DesiredOutPath.Value, true);
                HtmlTemplateService.RemoveTemplateFile();
                PlayerSettings.WebGL.template = "Default";
                Global.ProModel.IsBuildFromThisAsset.Value = false;
            }
            
        }

        [PostProcessBuild(2)]
        public static void OnBuildCompleted(BuildTarget target, string pathToBuildProject)
        {
            Global.Log("BuildController.OnBuildCompleted", LogFilter.Controller);
	        HtmlTemplateService.RemoveTemplateFile();
	        // Reset the template to default
	        PlayerSettings.WebGL.template = "Default";

            // se the model path to the path To Build Project
            
            Global.ProModel.IsBuildFromThisAsset.Value = false;
        }


        private void SetupEnhancedWebGLTemplate()
        {
            Global.Log($"\tBuildController: Setting up enhanced webgl template.", LogFilter.Controller);
            HtmlTemplateService.GenerateTemplateFile(ProModel);
            HtmlTemplateService.SetTemplateSettings();
        }

        private void SetBuildSettings()
        {
            Global.Log("BuildController.SetBuildSettings", LogFilter.Controller);
            if(ProModel.DoNotOverRideSettings.Value)
            {
                Debug.Log("Do not override settings");
                return;
            }

            SaveBuildSettings();

            if (!ProModel.DevelopmentBuild.Value)
            {
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
                // UM WTF THIS BREAKS WEBGL IF ITS TRUE
                PlayerSettings.WebGL.nameFilesAsHashes = false;
                PlayerSettings.WebGL.decompressionFallback = true;
                PlayerSettings.WebGL.dataCaching = true;

                #if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, ManagedStrippingLevel.Medium);
                #else
                PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.Medium);
                #endif
                PlayerSettings.stripUnusedMeshComponents = true;
                PlayerSettings.mipStripping = true;
                PlayerSettings.bakeCollisionMeshes = true;

                // TODO: Vertex Compression.
            }
            else
            {
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
                PlayerSettings.WebGL.nameFilesAsHashes = false;
                PlayerSettings.WebGL.decompressionFallback = false;
                PlayerSettings.WebGL.dataCaching = false;
                #if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, ManagedStrippingLevel.Disabled);
                #else
                PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.Disabled);
                #endif
                PlayerSettings.stripUnusedMeshComponents = false;
                PlayerSettings.mipStripping = false;
                PlayerSettings.bakeCollisionMeshes = false;
            }
        }

        private void SaveBuildSettings()
        {
            Global.Log("BuildController.SaveBuildSettings", LogFilter.Controller);
            buildSettings = new BuildSettings{
                CompressionFormat = PlayerSettings.WebGL.compressionFormat,
                NameFilesAsHashes = PlayerSettings.WebGL.nameFilesAsHashes,
                DecompressionFallback = PlayerSettings.WebGL.decompressionFallback,
                DataCaching = PlayerSettings.WebGL.dataCaching,
                #if UNITY_2023_1_OR_NEWER
                ManagedStrippingLevel = PlayerSettings.GetManagedStrippingLevel(NamedBuildTarget.WebGL),
                #else
                ManagedStrippingLevel = PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.WebGL),
                #endif
                StripUnusedMeshComponents = PlayerSettings.stripUnusedMeshComponents,
                MipStripping = PlayerSettings.mipStripping,
                BakeCollisionMeshes = PlayerSettings.bakeCollisionMeshes
            };
        }

        private void LoadBuildSettings()
        {
            Global.Log("BuildController.LoadBuildSettings", LogFilter.Controller);
            PlayerSettings.WebGL.compressionFormat = buildSettings.CompressionFormat;
            PlayerSettings.WebGL.nameFilesAsHashes = buildSettings.NameFilesAsHashes;
            PlayerSettings.WebGL.decompressionFallback = buildSettings.DecompressionFallback;
            PlayerSettings.WebGL.dataCaching = buildSettings.DataCaching;
            #if UNITY_2023_1_OR_NEWER
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, buildSettings.ManagedStrippingLevel);
            #else
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, buildSettings.ManagedStrippingLevel);
            #endif
            PlayerSettings.stripUnusedMeshComponents = buildSettings.StripUnusedMeshComponents;
            PlayerSettings.mipStripping = buildSettings.MipStripping;
            PlayerSettings.bakeCollisionMeshes = buildSettings.BakeCollisionMeshes;
            // TODO: Vertex Compression.
        }

        public void Dispose()
        {
            Global.Log("BuildController.Dispose", LogFilter.Controller);
            // Just in case we crash out during the build, we still want to clean up after ourselves.
            HtmlTemplateService.RemoveTemplateFile();
            ProModel.DevelopmentBuild.OnValueChanged -= UpdateBuildButtonTooltip;
            ProModel.ErrorsPresent.OnValueChanged -= UpdateBuildButtonTooltip;
        }
    }

    public struct BuildSettings
    {
        public WebGLCompressionFormat CompressionFormat;
        public bool NameFilesAsHashes;
        public bool DecompressionFallback;
        public bool DataCaching;
        public ManagedStrippingLevel ManagedStrippingLevel;
        public bool StripUnusedMeshComponents;
        public bool MipStripping;
        public bool BakeCollisionMeshes;
    }
}