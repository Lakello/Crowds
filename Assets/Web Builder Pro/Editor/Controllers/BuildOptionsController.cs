using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Anvil.WebBuilderPro
{
    internal sealed class BuildOptionsController : ControllerBase, IDisposable
    {
        private const string DevelopmentBuildToggle = "DevelopmentBuildToggle";
        private const string EnhancedWebGLTemplateToggle = "EnhancedWebGLTemplateToggle";
        private const string OptimizeForPixelArtToggle = "OptimizeForPixelArtToggle";
        private const string DisableMouseAccelerationToggle = "DisableMouseAccelerationToggle";
        private const string DoNotOverRideSettingsToggle = "DoNotOverRideSettingsToggle";

        private const string developmentBuildHelpUrl = "https://simon-nordon.gitbook.io/webgl-builder-pro/readme/build#use-minimalist-template-recommended-1";
        private const string enhancedWebGLTemplateHelpUrl = "https://simon-nordon.gitbook.io/webgl-builder-pro/readme/build#use-minimalist-template-recommended-2";
        private const string optimizeForPixelArtHelpUrl = "https://simon-nordon.gitbook.io/webgl-builder-pro/readme/build#optimize-for-pixel-art";
        private const string disableMouseAccelerationHelpUrl = "https://simon-nordon.gitbook.io/webgl-builder-pro/readme/build#disable-mouse-acceleration-recommended";
        private const string doNotOverRideSettingsHelpUrl = "https://simon-nordon.gitbook.io/webgl-builder-pro/readme/build#do-not-override-build-settings-advanced-mode";

        private readonly Toggle developmentBuildToggle;
        private readonly Toggle enhancedWebGLTemplateToggle;
        private readonly Toggle optimizeForPixelArtToggle;
        private readonly Toggle disableMouseAccelerationToggle;
        private readonly Toggle doNotOverRideSettingsToggle;

        private readonly Button developmentBuildHelpButton;
        private readonly Button enhancedWebGLTemplateHelpButton;
        private readonly Button optimizeForPixelArtHelpButton;
        private readonly Button disableMouseAccelerationHelpButton;
        private readonly Button doNotOverRideSettingsHelpButton;

        internal BuildOptionsController(WebBuilderProModel model, VisualElement root) : base(model, root)
        {
            // Dev Build Toggle
            developmentBuildToggle = root.Q<Toggle>(DevelopmentBuildToggle);
            developmentBuildToggle.BindToggle(model.DevelopmentBuild);
            developmentBuildHelpButton = developmentBuildToggle.Q<Button>();
            developmentBuildHelpButton.clicked += DevelopmentBuildHelpButtonClicked;

            // Template Toggle
            enhancedWebGLTemplateToggle = root.Q<Toggle>(EnhancedWebGLTemplateToggle);
            enhancedWebGLTemplateToggle.BindToggle(ProModel.EnhancedWebGLTemplate);
            enhancedWebGLTemplateHelpButton = enhancedWebGLTemplateToggle.Q<Button>();
            enhancedWebGLTemplateHelpButton.clicked += EnhancedWebGLTemplateHelpButtonClicked;

            // Pixel Art Toggle
            optimizeForPixelArtToggle = root.Q<Toggle>(OptimizeForPixelArtToggle);
            optimizeForPixelArtToggle.BindToggle(ProModel.OptimizeForPixelArt);
            optimizeForPixelArtHelpButton = optimizeForPixelArtToggle.Q<Button>();
            optimizeForPixelArtHelpButton.clicked += OptimizeForPixelArtHelpButtonClicked;

            // Pixel art is only available when using the enhanced template
            optimizeForPixelArtToggle.BindDisplay(ProModel.EnhancedWebGLTemplate);


            // Mouse Accel Toggle
            disableMouseAccelerationToggle = root.Q<Toggle>(DisableMouseAccelerationToggle);
            disableMouseAccelerationToggle.BindToggle(ProModel.DisableMouseAcceleration);
            disableMouseAccelerationHelpButton = disableMouseAccelerationToggle.Q<Button>();
            disableMouseAccelerationHelpButton.clicked += DisableMouseAccelerationHelpButtonClicked;

            // Do Not OverRide Settings Toggle
            doNotOverRideSettingsToggle = root.Q<Toggle>(DoNotOverRideSettingsToggle);
            doNotOverRideSettingsToggle.BindToggle(ProModel.DoNotOverRideSettings);
            doNotOverRideSettingsHelpButton = doNotOverRideSettingsToggle.Q<Button>();
            doNotOverRideSettingsHelpButton.clicked += DoNotOverRideSettingsHelpButtonClicked;

            doNotOverRideSettingsToggle.BindDisplay(ProModel.AdvancedOptions);
        }



        public void DevelopmentBuildHelpButtonClicked()
        {
            Application.OpenURL(developmentBuildHelpUrl);
        }

        private void EnhancedWebGLTemplateHelpButtonClicked()
        {
            Application.OpenURL(enhancedWebGLTemplateHelpUrl);
        }

        private void OptimizeForPixelArtHelpButtonClicked()
        {
            Application.OpenURL(optimizeForPixelArtHelpUrl);
        }

        private void DisableMouseAccelerationHelpButtonClicked()
        {
            Application.OpenURL(disableMouseAccelerationHelpUrl);
        }

        private void DoNotOverRideSettingsHelpButtonClicked()
        {
            Application.OpenURL(doNotOverRideSettingsHelpUrl);
        }

        public void Dispose()
        {
            developmentBuildToggle.UnBind(ProModel.DevelopmentBuild);
            developmentBuildHelpButton.clicked -= DevelopmentBuildHelpButtonClicked;

            enhancedWebGLTemplateToggle.UnBind(ProModel.EnhancedWebGLTemplate);
            enhancedWebGLTemplateHelpButton.clicked -= EnhancedWebGLTemplateHelpButtonClicked;

            optimizeForPixelArtToggle.UnBind(ProModel.OptimizeForPixelArt);
            optimizeForPixelArtHelpButton.clicked -= OptimizeForPixelArtHelpButtonClicked;

            disableMouseAccelerationToggle.UnBind(ProModel.DisableMouseAcceleration);
            disableMouseAccelerationHelpButton.clicked -= DisableMouseAccelerationHelpButtonClicked;
        }
    }
}