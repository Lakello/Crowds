using System.Collections.Generic;
using UnityEngine;

namespace Anvil.WebBuilderPro
{
    internal static class Global
    {
        public static WebBuilderProModel ProModel { get; private set; }

        // Static dictionary to map LogFilter enum to strings
        public static readonly Dictionary<LogFilter, string> LogFilterToStringMap;

        static Global()
        {
            // Initialize the dictionary with mappings
            LogFilterToStringMap = new Dictionary<LogFilter, string>
            {
                { LogFilter.Model, "Model" },
                { LogFilter.Window, "Window" },
                { LogFilter.Controller, "Controller" },
                { LogFilter.DisableMouseAcceleration, "Disable Mouse Accel" },
                { LogFilter.HtmlTemplate, "Html Service" },
                { LogFilter.LocalHostRunner, "Local Host Runner" }
            };
        }

        public static void SetModel(WebBuilderProModel newProModel)
        {
            ProModel = newProModel;
        }

        // Conditional Logging to Console Pro
        public static void Log(string inLog, LogFilter filter = LogFilter.Normal)
        {
            var inFilterName = LogFilterToStringMap[filter];
            #if CONSOLE_PRO
            Debug.Log(inLog + "\nCPAPI:{\"cmd\":\"Filter\", \"name\":\"" + inFilterName + "\"}");
            #endif
        }
    }

    public enum LogFilter
    {
        Normal,
        Model,
        Window,
        Controller,
        DisableMouseAcceleration,
        HtmlTemplate,
        LocalHostRunner
    }
}