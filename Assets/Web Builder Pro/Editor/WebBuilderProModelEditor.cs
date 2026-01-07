using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.IO;

namespace Anvil.WebBuilderPro
{
    [CustomEditor(typeof(WebBuilderProModel))]
    internal sealed class WebBuilderProModelEditor : Editor
    {
        SerializedProperty buildNameProp;
        SerializedProperty outputParentPathProp;
        SerializedProperty isOutputParentPathValidProp;
        SerializedProperty desiredOutPathProp;

        SerializedProperty developmentBuildProp;
        SerializedProperty enhancedWebGLTemplateProp;
        SerializedProperty optimizeForPixelArtProp;
        SerializedProperty disableMouseAccelerationProp;
        SerializedProperty doNotOverRideSettingsProp;

        SerializedProperty errorsPresentProp;
        SerializedProperty errorsCountProp;
        SerializedProperty isNotWebGLBuildTargetProp;
        SerializedProperty isAutoGraphicsAPIWithLinearColorSpaceProp;

        SerializedProperty warningsPresentProp;
        SerializedProperty isDataCachingProp;
        SerializedProperty isDecompressionFallbackProp;

        SerializedProperty currentSelectedBuildPathProp;
        SerializedProperty doesCurrentSelectedBuildPathExistProp;

        SerializedProperty chromeBrowserPathProp;
        SerializedProperty firefoxBrowserPathProp;
        SerializedProperty edgeBrowserPathProp;
        SerializedProperty operaBrowserPathProp;

        SerializedProperty portNumberProp;

        public override VisualElement CreateInspectorGUI()
        {
            // Load SerializedProperties
            serializedObject.Update();
            LoadSerializedProperties();

            // Root container
            var root = new VisualElement();

            // Root container with a specific width to fit the Inspector

            root.style.flexDirection = FlexDirection.Column; // Stack elements vertically
            root.style.flexGrow = 1; // Ensure it takes the available space
            root.style.flexWrap = Wrap.NoWrap; // Prevent wrapping

            // Output Options
            root.Add(CreateHeader("Output Options"));
            root.Add(CreateReadOnlyPropertyField(buildNameProp));
            root.Add(CreateReadOnlyPropertyField(outputParentPathProp));
            root.Add(CreateReadOnlyPropertyField(desiredOutPathProp));
            root.Add(CreateReadOnlyPropertyField(isOutputParentPathValidProp));

            root.Add(new SpaceSmall());

            // Build Options
            root.Add(CreateHeader("Build Options"));
            root.Add(new PropertyField(developmentBuildProp));
            root.Add(new PropertyField(enhancedWebGLTemplateProp));
            root.Add(new PropertyField(optimizeForPixelArtProp));
            root.Add(new PropertyField(disableMouseAccelerationProp));
            root.Add(new PropertyField(doNotOverRideSettingsProp));

            root.Add(new SpaceSmall());

            // Issues: Errors
            root.Add(CreateHeader("Issues: Errors"));
            root.Add(CreateReadOnlyPropertyField(errorsPresentProp));
            root.Add(CreateReadOnlyPropertyField(errorsCountProp));
            root.Add(CreateReadOnlyPropertyField(isNotWebGLBuildTargetProp));
            root.Add(CreateReadOnlyPropertyField(isAutoGraphicsAPIWithLinearColorSpaceProp));

            root.Add(new SpaceSmall());

            // Issues: Warnings
            root.Add(CreateHeader("Issues: Warnings"));
            root.Add(CreateReadOnlyPropertyField(warningsPresentProp));
            root.Add(CreateReadOnlyPropertyField(isDataCachingProp));
            root.Add(CreateReadOnlyPropertyField(isDecompressionFallbackProp));

            root.Add(new SpaceSmall());

            // Review
            root.Add(CreateHeader("Review"));
            root.Add(CreateReadOnlyPropertyField(currentSelectedBuildPathProp));
            root.Add(CreateReadOnlyPropertyField(doesCurrentSelectedBuildPathExistProp));

            root.Add(new SpaceSmall());

            // Browsers
            root.Add(CreateHeader("Browsers"));
            root.Add(CreateBrowserPathField("Chrome Browser Path", chromeBrowserPathProp));
            root.Add(CreateBrowserPathField("Firefox Browser Path", firefoxBrowserPathProp));
            root.Add(CreateBrowserPathField("Edge Browser Path", edgeBrowserPathProp));
            root.Add(CreateBrowserPathField("Opera Browser Path", operaBrowserPathProp));

            root.Add(new SpaceSmall());

            // Local Host
            root.Add(CreateHeader("Local Host"));
            root.Add(CreatePortNumberField());
            root.Add(CreateForceStopLocalHostButton());


            var resetButton = new Button();
            resetButton.text = "Reset";
            resetButton.clicked += ResetModel; // Updated method name

            root.Add(resetButton);


            return root;
        }

        void LoadSerializedProperties()
        {
            // Initialize SerializedProperties
            buildNameProp = serializedObject.FindProperty("BuildName");
            outputParentPathProp = serializedObject.FindProperty("OutputParentPath");
            isOutputParentPathValidProp = serializedObject.FindProperty("IsOutputParentPathValid");
            desiredOutPathProp = serializedObject.FindProperty("DesiredOutPath");

            developmentBuildProp = serializedObject.FindProperty("DevelopmentBuild");
            enhancedWebGLTemplateProp = serializedObject.FindProperty("EnhancedWebGLTemplate");
            optimizeForPixelArtProp = serializedObject.FindProperty("OptimizeForPixelArt");
            disableMouseAccelerationProp = serializedObject.FindProperty("DisableMouseAcceleration");
            doNotOverRideSettingsProp = serializedObject.FindProperty("DoNotOverRideSettings");

            errorsPresentProp = serializedObject.FindProperty("ErrorsPresent");
            errorsCountProp = serializedObject.FindProperty("ErrorsCount");
            isNotWebGLBuildTargetProp = serializedObject.FindProperty("IsNotWebGLBuildTarget");
            isAutoGraphicsAPIWithLinearColorSpaceProp =
                serializedObject.FindProperty("IsAutoGraphicsAPIWithLinearColorSpace");

            warningsPresentProp = serializedObject.FindProperty("WarningsPresent");
            isDataCachingProp = serializedObject.FindProperty("IsDataCaching");
            isDecompressionFallbackProp = serializedObject.FindProperty("IsDecompressionFallback");

            currentSelectedBuildPathProp = serializedObject.FindProperty("CurrentSelectedBuildPath");
            doesCurrentSelectedBuildPathExistProp = serializedObject.FindProperty("DoesCurrentSelectedBuildPathExist");

            chromeBrowserPathProp = serializedObject.FindProperty("chromeBrowserPath");
            firefoxBrowserPathProp = serializedObject.FindProperty("firefoxBrowserPath");
            edgeBrowserPathProp = serializedObject.FindProperty("edgeBrowserPath");
            operaBrowserPathProp = serializedObject.FindProperty("operaBrowserPath");

            portNumberProp = serializedObject.FindProperty("portNumber");
        }

        private void ResetModel()
        {
            var model = (WebBuilderProModel)target;
            model.ResetValues();
            serializedObject.Update();
        }

        VisualElement CreateHeader(string title)
        {
            var header = new Label(title);
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.marginTop = 10;
            header.style.marginBottom = 5;
            return header;
        }

        VisualElement CreateReadOnlyPropertyField(SerializedProperty property)
        {
            var field = new PropertyField(property);
            field.SetEnabled(false);
            return field;
        }

        VisualElement CreateBrowserPathField(string label, SerializedProperty pathProp)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.flexGrow = 1; // Allow the container to grow to fit

            // Create a read-only PropertyField
            var field = new PropertyField(pathProp, label);
            field.SetEnabled(false);
            field.style.flexGrow = 1; // Allow the field to take the remaining space
            field.style.minWidth = 150; // Set a minimum width for the field

            // Create a button to open a file dialog for selecting the browser path with a fixed width
            var button = new Button(() =>
            {
                string initialPath = pathProp.stringValue;
                if (string.IsNullOrEmpty(initialPath) || !File.Exists(initialPath))
                {
                    initialPath = Application.dataPath;
                }

                string path = EditorUtility.OpenFilePanel("Select Browser Executable", initialPath, "exe");
                if (!string.IsNullOrEmpty(path))
                {
                    pathProp.stringValue = path;
                    pathProp.serializedObject.ApplyModifiedProperties(); // Update the serialized property value
                    serializedObject.ApplyModifiedProperties(); // Ensure changes are applied in the serialized object
                }
            })
            {
                text = "..."
            };
            button.style.width = 30; // Set a fixed width for the button
            button.style.marginLeft = 5;

            container.Add(field); // Add the PropertyField to the container
            container.Add(button); // Add the button to the container

            return container;
        }


        VisualElement CreatePortNumberField()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.flexGrow = 1; // Allow the container to grow to fit

            // Create a read-only PropertyField for the port number
            var portField = new PropertyField(portNumberProp, "Port Number");
            portField.SetEnabled(false); // Make it read-only
            portField.style.flexGrow = 1; // This allows it to take the remaining space
            portField.style.minWidth = 100; // Set a minimum width to prevent it from being too small

            // Create a button to randomize the port number with a fixed width
            var button = new Button(() =>
            {
                portNumberProp.intValue = GetRandomPortNumber();
                portNumberProp.serializedObject.ApplyModifiedProperties(); // Apply changes to the serialized object
                serializedObject.ApplyModifiedProperties(); // Ensure changes are applied
            })
            {
                text = "Randomize Port"
            };
            button.style.width = 100; // Set a fixed width for the button
            button.style.marginLeft = 5;

            container.Add(portField);
            container.Add(button);

            return container;
        }


        int GetRandomPortNumber()
        {
            System.Random rand = new System.Random();
            return rand.Next(1024, 65535);
        }

        VisualElement CreateForceStopLocalHostButton()
        {
            var button = new Button(() => OnForceStopLocalHostClicked())
            {
                text = "Force Stop Local Host"
            };
            button.style.marginTop = 10;
            return button;
        }

        void OnForceStopLocalHostClicked()
        {
            var model = (Anvil.WebBuilderPro.WebBuilderProModel)target;
            model.ForceStopLocalHost();
        }

        // Helper class for spacing
        class SpaceSmall : VisualElement
        {
            public SpaceSmall()
            {
                style.height = 10;
            }
        }
    }
}