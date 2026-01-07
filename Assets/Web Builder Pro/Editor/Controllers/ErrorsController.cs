using System;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Anvil.WebBuilderPro
{
    internal sealed class ErrorsController : ControllerBase, IDisposable
    {
        private const string ErrorLabelName = "ErrorsLabel";

        private const string WebGlBuildTargetName = "WebGLBuildTargetIssue";
        private const string NoScenesErrorName = "NoScenesIssue";
        private const string AutoGraphicsAPIName = "AutoGraphicsAPIWithLinearColorSpaceIssue";
        private const string IssuesContainerName = "IssuesContainer";

        private readonly VisualElement IssuesContainer;
        private readonly Label ErrorLabel;

        private readonly VisualElement WebGlBuildTargetError;
        private readonly VisualElement NoScenesError;
        private readonly VisualElement AutoGraphicsAPIError;
 

        private readonly Button WebGlBuildTargetErrorFixButton;
        private readonly Button NoScenesErrorFixButton;
        private readonly Button AutoGraphicsAPIErrorButton;


        internal ErrorsController(WebBuilderProModel model, VisualElement rootVisualElement) : base(model, rootVisualElement)
        {
            IssuesContainer = Root.Q<VisualElement>(IssuesContainerName);
            IssuesContainer.BindDisplay(ProModel.ErrorsPresent);

            ErrorLabel = IssuesContainer.Q<Label>(ErrorLabelName);
            ErrorLabel.BindLabel(ProModel.ErrorsCount, () => $"Errors: {ProModel.ErrorsCount.Value}");
            ErrorLabel.BindDisplay(ProModel.ErrorsPresent);


            WebGlBuildTargetError = IssuesContainer.Q<VisualElement>(WebGlBuildTargetName);
            WebGlBuildTargetError.BindDisplay(ProModel.IsNotWebGLBuildTarget);
            WebGlBuildTargetErrorFixButton = WebGlBuildTargetError.Q<Button>();
            WebGlBuildTargetErrorFixButton.clicked += FixWebGlBuiltTarget;

            NoScenesError = IssuesContainer.Q<VisualElement>(NoScenesErrorName);
            NoScenesError.BindDisplay(ProModel.IsNoScenes);
            NoScenesErrorFixButton = NoScenesError.Q<Button>();
            NoScenesErrorFixButton.clicked += FixNoScenes;


            AutoGraphicsAPIError = IssuesContainer.Q<VisualElement>(AutoGraphicsAPIName);
            AutoGraphicsAPIError.BindDisplay(ProModel.IsAutoGraphicsAPIWithLinearColorSpace);
            AutoGraphicsAPIErrorButton = AutoGraphicsAPIError.Q<Button>();
            AutoGraphicsAPIErrorButton.clicked += EnsureWebGL2OnlyAPI;
        }

        private void FixNoScenes()
        {
            // open the build settings scene window
            UnityEditor.BuildPlayerWindow.ShowBuildPlayerWindow();
        }

        // TODO ENSURE SCENE EXISTS TO BUILD!@!!


        private void FixWebGlBuiltTarget()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(UnityEditor.BuildTargetGroup.WebGL, UnityEditor.BuildTarget.WebGL);
            ProModel.Update();
        }


        private void EnsureWebGL2OnlyAPI()
        {
            // disable auto graphics api
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, new[] { GraphicsDeviceType.OpenGLES3 });
        }

        public void Dispose()
        {
            WebGlBuildTargetErrorFixButton.clicked -= FixWebGlBuiltTarget;
            AutoGraphicsAPIErrorButton.clicked -= EnsureWebGL2OnlyAPI;
        }
    }
}
