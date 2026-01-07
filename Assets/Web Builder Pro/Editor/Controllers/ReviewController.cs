using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace Anvil.WebBuilderPro
{
    internal sealed class ReviewController : ControllerBase, IDisposable
    {
        private readonly Label buildOutputPathLabel;
        private readonly Button buildOutputPathBrowseButton;

        private readonly Label buildoutputPathWarning;

        private readonly Button systemBrowserButton;
        private readonly Button browserChromeButton;
        private readonly Button browserEdgeButton;
        private readonly Button browserFirefoxButton;
        private readonly Button browserOperaButton;


        internal ReviewController(WebBuilderProModel model, VisualElement root) : base(model, root)
        {
            Global.Log("Initializing ReviewController", LogFilter.Controller);

            buildOutputPathLabel = Root.Q<Label>("BuildOutputPathLabel");
            buildOutputPathLabel.BindLabel(model.CurrentSelectedBuildPath);

            buildOutputPathBrowseButton = Root.Q<Button>("BuildOutputPathBrowseButton");
            buildOutputPathBrowseButton.clicked += OnOutputPathBrowseButtonClicked;

            buildoutputPathWarning = Root.Q<Label>("BuildOutputPathWarning");
            buildoutputPathWarning.BindDisplay(model.DoesCurrentSelectedBuildPathExist, true);

            systemBrowserButton = Root.Q<Button>("system-browser-button");
            systemBrowserButton.clicked += OnSystemBrowserButtonClicked;
            systemBrowserButton.BindButtonState(model.DoesCurrentSelectedBuildPathExist);

            browserChromeButton = Root.Q<Button>("chrome-button");
            browserChromeButton.clicked += OnChromeBrowserButtonClicked;
            browserChromeButton.BindButtonState(model.DoesCurrentSelectedBuildPathExist);

            browserEdgeButton = Root.Q<Button>("edge-button");
            browserEdgeButton.clicked += OnEdgeBrowserButtonClicked;
            browserEdgeButton.BindButtonState(model.DoesCurrentSelectedBuildPathExist);

            browserFirefoxButton = Root.Q<Button>("firefox-button");
            browserFirefoxButton.clicked += OnFirefoxBrowserButtonClicked;
            browserFirefoxButton.BindButtonState(model.DoesCurrentSelectedBuildPathExist);

            browserOperaButton = Root.Q<Button>("opera-button");
            browserOperaButton.clicked += OnOperaBrowserButtonClicked;
            browserOperaButton.BindButtonState(model.DoesCurrentSelectedBuildPathExist);

            Global.Log("ReviewController initialized", LogFilter.Controller);
        }

        private void OnSystemBrowserButtonClicked()
        {
            Global.Log("System browser button clicked", LogFilter.Controller);
            if (!LocalHostRunner.IsRunning)
            {
                LocalHostRunner.Start(ProModel.CurrentSelectedBuildPath.Value);
            }
            Process.Start(new ProcessStartInfo(ProModel.LocalHostAddress) { UseShellExecute = true });
        }

        private void LaunchBrowser(string browserName, ref string browserPath, string downloadUrl)
        {
            Global.Log($"Launching browser: {browserName}", LogFilter.Controller);
            if (!File.Exists(browserPath))
            {
                var download = EditorUtility.DisplayDialog($"{browserName} not found", $"Could not find {browserName}. Locate it on your computer, or download and install.", "Download", "Locate");

                if (download)
                {
                    UnityEngine.Application.OpenURL(downloadUrl);
                }
                else
                {
                    var newBrowserPath = EditorUtility.OpenFilePanel($"Select {browserName} Executable", "", "exe");
                    if (string.IsNullOrEmpty(newBrowserPath)) return;
                    browserPath = newBrowserPath;
                }
            }

            if (!LocalHostRunner.IsRunning)
            {
                LocalHostRunner.Start(ProModel.CurrentSelectedBuildPath.Value);
            }

            Process.Start(new ProcessStartInfo(browserPath, ProModel.LocalHostAddress) { UseShellExecute = true });
        }

        private void OnChromeBrowserButtonClicked()
        {
            Global.Log("Chrome browser button clicked", LogFilter.Controller);
            LaunchBrowser("Chrome", ref ProModel.chromeBrowserPath, "https://www.google.com/chrome/");
        }

        private void OnEdgeBrowserButtonClicked()
        {
            Global.Log("Edge browser button clicked", LogFilter.Controller);
            LaunchBrowser("Edge", ref ProModel.edgeBrowserPath, "https://www.microsoft.com/en-us/edge");
        }

        private void OnFirefoxBrowserButtonClicked()
        {
            Global.Log("Firefox browser button clicked", LogFilter.Controller);
            LaunchBrowser("Firefox", ref ProModel.firefoxBrowserPath, "https://www.mozilla.org/en-US/firefox/new/");
        }

        private void OnOperaBrowserButtonClicked()
        {
            Global.Log("Opera browser button clicked", LogFilter.Controller);
            LaunchBrowser("Opera", ref ProModel.operaBrowserPath, "https://www.opera.com/");
        }


        private void OnOutputPathBrowseButtonClicked()
        {
            Global.Log("Output path browse button clicked", LogFilter.Controller);
            var newBuildPath = EditorUtility.OpenFolderPanel("Select Build Path", ProModel.CurrentSelectedBuildPath.Value, "");
            if (string.IsNullOrEmpty(newBuildPath)) return;
            ProModel.CurrentSelectedBuildPath.Value = newBuildPath;

            // We must stop the local host because the user is attempting to run a server in a new folder.
            if (LocalHostRunner.IsRunning)
            {
                LocalHostRunner.Stop();
            }
            ProModel.Update();
        }

        public void Dispose()
        {
            Global.Log("Disposing ReviewController", LogFilter.Controller);
            // ensure local runner is stopped
            if (LocalHostRunner.IsRunning)
            {
                LocalHostRunner.Stop();
            }
        }
    }
}