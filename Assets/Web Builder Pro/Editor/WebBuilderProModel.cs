using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Anvil.WebBuilderPro
{
    internal sealed class WebBuilderProModel : ScriptableObject
    {
        # region Build

        private readonly List<ObservableBase> observables = new();

        public ObservableBool AdvancedOptions = new(false);

        [Header("Output Options")]
        public ObservableString BuildName;
        public ObservableString OutputParentPath;
        public ObservableBool IsOutputParentPathValid = new(false);

        public ObservableString DesiredOutPath;

        [Header("Build Options")]
        public ObservableBool DevelopmentBuild = new(false);
        public ObservableBool EnhancedWebGLTemplate = new(true);
        public ObservableBool OptimizeForPixelArt = new(true);
        public ObservableBool DisableMouseAcceleration = new(true);
        public ObservableBool DoNotOverRideSettings = new(false);


        [Header("Issues: Errors")]
        public ObservableBool ErrorsPresent = new(false);
        public ObservableInt ErrorsCount = new(0);
        public ObservableBool IsNotWebGLBuildTarget = new(false);
        public ObservableBool IsNoScenes = new(false);
        public ObservableBool IsAutoGraphicsAPIWithLinearColorSpace = new(false);

        [Header("Issues: Warnings")]
        public ObservableBool WarningsPresent = new(false);
        public ObservableBool IsDataCaching = new(false);
        public ObservableBool IsDecompressionFallback = new(false);

        [Header("Build")]
        /// <summary> A check to ensure that our build post processes don't run when the user is just doing a normal build.</summary>
        public ObservableBool IsBuildFromThisAsset = new(false);

        [Header("Review")]
        public ObservableString CurrentSelectedBuildPath;
        public ObservableBool DoesCurrentSelectedBuildPathExist;

        [Header("Browsers")]
        public string chromeBrowserPath;
        public string firefoxBrowserPath;
        public string edgeBrowserPath;
        public string operaBrowserPath;

        [Header("Local Host")]
        public int portNumber = 8001;
        public string LocalHostAddress => $"http://localhost:{portNumber}/";
        # endregion

        # region Report
        public ObservableString ReportPath;
        # endregion

        # region Review

        [Header("Review Options")]
        public string CurrentBuildDirectory;
        public string CurrentBuildFolderWarning;
        public bool DoesCurrentBuildExist;

        # endregion

        private void OnEnable()
        {
            Global.Log("WebGLBuilderProModel.OnEnable()", LogFilter.Model);
            // We unfortunately require global scope for certain build post processing operations.
            Global.SetModel(this);

            // Advanced Options
            observables.Add(AdvancedOptions);

            // Outputs
            observables.Add(BuildName);
            observables.Add(OutputParentPath);
            observables.Add(IsOutputParentPathValid);
            observables.Add(DesiredOutPath);

            // Build Options
            observables.Add(DevelopmentBuild);
            observables.Add(EnhancedWebGLTemplate);
            observables.Add(DisableMouseAcceleration);
            observables.Add(OptimizeForPixelArt);
            observables.Add(DoNotOverRideSettings);

            // Issues: Errors
            observables.Add(ErrorsPresent);
            observables.Add(ErrorsCount);
            observables.Add(IsNotWebGLBuildTarget);
            observables.Add(IsNoScenes);
            observables.Add(IsAutoGraphicsAPIWithLinearColorSpace);

            // Build 
            observables.Add(IsBuildFromThisAsset);

            // Review
            observables.Add(CurrentSelectedBuildPath);
            observables.Add(DoesCurrentSelectedBuildPathExist);


            // Link advanced option to skip override so it's not left on accidently.
            AdvancedOptions.OnValueChanged += (bool value) =>
            {
                if (!value)
                {
                    DoNotOverRideSettings.Value = false;
                }
            };

            SetDefaultOutputDirectory();
            AttemptToGetBrowserPaths();
            Update();
            ForceNotify();
        }

        internal void ConfirmSuccessfulBuild()
        {
            // output path to desired path
            CurrentSelectedBuildPath.Value = DesiredOutPath.Value;
        }

        public void Update()
        {
            // Output Options
            BuildName.Value = Utilities.SanitizePath($"{Application.productName} v{PlayerSettings.bundleVersion}");
            IsOutputParentPathValid.Value = Directory.Exists(OutputParentPath.Value);
            DesiredOutPath.Value = Path.Combine(OutputParentPath.Value, BuildName.Value);

            // Error Checking
            IsNotWebGLBuildTarget.Value = EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL;
            IsAutoGraphicsAPIWithLinearColorSpace.Value = PlayerSettings.colorSpace == ColorSpace.Linear && PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.WebGL);
            IsNoScenes.Value = EditorBuildSettings.scenes.Length == 0;
            ErrorsPresent.Value = IsNotWebGLBuildTarget.Value || IsAutoGraphicsAPIWithLinearColorSpace.Value || IsNoScenes.Value;
            ErrorsCount.Value = (IsNotWebGLBuildTarget.Value ? 1 : 0) + (IsAutoGraphicsAPIWithLinearColorSpace.Value ? 1 : 0) + (IsNoScenes.Value ? 1 : 0);

            // Build is selected.
            DoesCurrentSelectedBuildPathExist.Value = Directory.Exists(CurrentSelectedBuildPath.Value) && File.Exists(Path.Combine(CurrentSelectedBuildPath.Value, "index.html"));
        }

        private void OnDisable()
        {
            Global.Log("WebGLBuilderProModel.OnDisable()", LogFilter.Model);
            UnBind();
        }

        /// <summary>
        /// Force all observables to notify.
        /// </summary>
        public void ForceNotify()
        {

            foreach (var observable in observables)
            {
                observable.ForceNotify();
            }
        }

        /// <summary>
        /// Unbind all observables.
        /// </summary>
        public void UnBind()
        {
            foreach (var observable in observables)
            {
                observable.UnBind();
            }
        }

        private void OnValidate()
        {
            // Allows us to change the model directly in the inspector.
            Update();
            ForceNotify();
        }

        private void SetDefaultOutputDirectory()
        {
            Global.Log("WebGLBuilderProModel.SetDefaultOutputDirectory()", LogFilter.Model);
            // Only create the default.
            if (string.IsNullOrEmpty(OutputParentPath.Value))
            {
                OutputParentPath.Value = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Builds");
                Directory.CreateDirectory(OutputParentPath.Value);
            }
        }

        private void AttemptToGetBrowserPaths()
        {
            Global.Log("WebGLBuilderProModel.AttemptToGetBrowserPaths()", LogFilter.Model);
            if (string.IsNullOrEmpty(chromeBrowserPath))
                chromeBrowserPath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

            if (string.IsNullOrEmpty(edgeBrowserPath))
                edgeBrowserPath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

            if (string.IsNullOrEmpty(firefoxBrowserPath))
                firefoxBrowserPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";

            if (string.IsNullOrEmpty(operaBrowserPath))
                operaBrowserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Opera\opera.exe");
        }

        internal void ForceStopLocalHost()
        {
            Global.Log("WebGLBuilderProModel.ForceStopLocalHost()", LogFilter.Model);
            if (!LocalHostRunner.IsRunning)
            {
                Debug.Log("Local host is not running.");
                return;
            }
            Debug.Log("Stopping local host.");
            LocalHostRunner.Stop();
        }
        internal void ResetValues()
        {
            Global.Log("WebGLBuilderProModel.ResetValues()", LogFilter.Model);
            // Reset the build path.
            CurrentSelectedBuildPath.Value = string.Empty;
            OutputParentPath.Value = string.Empty;
            BuildName.Value = string.Empty;

            // Reset advanced options
            AdvancedOptions.Value = false;

            // Reset output options
            IsOutputParentPathValid.Value = false;
            DesiredOutPath.Value = string.Empty;

            // Reset build options
            DevelopmentBuild.Value = false;
            EnhancedWebGLTemplate.Value = true;
            OptimizeForPixelArt.Value = true;
            DisableMouseAcceleration.Value = true;
            DoNotOverRideSettings.Value = false;

            // Reset issues: errors
            ErrorsPresent.Value = false;
            ErrorsCount.Value = 0;
            IsNotWebGLBuildTarget.Value = false;
            IsAutoGraphicsAPIWithLinearColorSpace.Value = false;

            // Reset issues: warnings
            WarningsPresent.Value = false;
            IsDataCaching.Value = false;
            IsDecompressionFallback.Value = false;

            // Reset build
            IsBuildFromThisAsset.Value = false;

            // Reset review
            DoesCurrentSelectedBuildPathExist.Value = false;

            // Reset browsers
            chromeBrowserPath = string.Empty;
            firefoxBrowserPath = string.Empty;
            edgeBrowserPath = string.Empty;
            operaBrowserPath = string.Empty;

            // Reset local host
            portNumber = 8001;

            // Reset report
            ReportPath.Value = string.Empty;

            // Reset review options
            CurrentBuildDirectory = string.Empty;
            CurrentBuildFolderWarning = string.Empty;
            DoesCurrentBuildExist = false;

            // Mark the ScriptableObject as dirty to ensure it gets serialized
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}
