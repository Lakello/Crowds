using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Anvil.WebBuilderPro
{
	/// <summary>
	/// Can Create, Update and Delete a temporary Minimal WebGL Template.
	/// </summary>
	internal static class HtmlTemplateService
	{
		private const string WebGlTemplatePath = "Assets/WebGLTemplates";
		private const string TemplatePath = WebGlTemplatePath + "/EnhancedTemplate";
		private const string TemplateName = "EnhancedTemplate";
		private const string TemplateIndexHtml = TemplatePath + "/index.html";
		private static bool WebGlTemplateExists => System.IO.Directory.Exists("Assets/WebGLTemplates");

		public static void GenerateTemplateFile(WebBuilderProModel proModel = null)
		{
			Global.Log("HtmlTemplateService.GenerateTemplateFile()", LogFilter.HtmlTemplate);
			if (!System.IO.Directory.Exists(TemplatePath))
			{
				Global.Log($"\tCreating WebGL template directory: {TemplatePath}", LogFilter.HtmlTemplate);
				System.IO.Directory.CreateDirectory(TemplatePath);
			}
			// Write the file
			var htmlContent = TemplateIndexHtml;

			if (proModel != null)
			{
				htmlContent = proModel.OptimizeForPixelArt.Value ?
					HtmlContent.Replace("{{{ OPTIMIZE_FOR_PIXEL_ART }}}", "true") :
					HtmlContent.Replace("{{{ OPTIMIZE_FOR_PIXEL_ART }}}", "false");
			}

			Global.Log($"\tWriting {TemplateIndexHtml}", LogFilter.HtmlTemplate);
			var htmlFile = System.IO.File.CreateText(TemplateIndexHtml);
			htmlFile.Write(htmlContent);
			htmlFile.Close();
		}

		public static void SetTemplateSettings()
		{
			Global.Log("HtmlTemplateService.SetTemplateSettings()", LogFilter.HtmlTemplate);
			PlayerSettings.WebGL.template = $"PROJECT:{TemplateName}";
		}

		public static void RemoveTemplateFile()
		{
			Global.Log("HtmlTemplateService.RemoveTemplateFile()", LogFilter.HtmlTemplate);
			try
			{
				if (WebGlTemplateExists)
				{
					if (System.IO.Directory.Exists(TemplatePath))
					{
						System.IO.Directory.Delete(TemplatePath, true);

						// check if the meta file exists
						var metaFile = TemplatePath + ".meta";
						if (System.IO.File.Exists(metaFile))
						{
							System.IO.File.Delete(metaFile);
						}
					}
				}
				else
				{
					if (System.IO.Directory.Exists(WebGlTemplatePath))
					{
						// If you're deleting files individually before this, ensure the directory is empty and then delete it.
						System.IO.Directory.Delete(WebGlTemplatePath, true);

						// check if the meta file exists
						var metaFile = WebGlTemplatePath + ".meta";
						if (System.IO.File.Exists(metaFile))
						{
							System.IO.File.Delete(metaFile);
						}
					}
				}

				// Now if the WebGlTemplatePath is empty, delete it.
				if (System.IO.Directory.Exists(WebGlTemplatePath))
				{
					if (!System.IO.Directory.EnumerateFileSystemEntries(WebGlTemplatePath).Any())
					{
						System.IO.Directory.Delete(WebGlTemplatePath, false);

						// check if the meta file exists
						var metaFile = WebGlTemplatePath + ".meta";
						if (System.IO.File.Exists(metaFile))
						{
							System.IO.File.Delete(metaFile);
						}
					}
				}

			}
			catch (Exception ex)
			{
				Debug.LogError($"Failed to delete file or directory: {ex.Message}");
			}

			// Refresh asset database
			AssetDatabase.Refresh();
		}

		// It's just safer to pack the html content in the script, can't get lost that way!
		private const string HtmlContent = @"<!DOCTYPE html>
<html lang=""en-us"">

<head>
	<meta charset=""utf-8"">
	<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
	<title>{{{ PRODUCT_NAME }}}</title>
	<style>
		html,
		body {
			background: #000;
			width: 100%;
			height: 100%;
			overflow: visible;
			padding: 0;
			margin: 0;
		}

		div#gameContainer {
			background: transparent !important;
			position: absolute;
		}

		div#gameContainer canvas {
			position: absolute;
		}

		div#gameContainer canvas[data-pixel-art=""true""] {
			position: absolute;
			image-rendering: optimizeSpeed;
			image-rendering: -webkit-crisp-edges;
			image-rendering: -moz-crisp-edges;
			image-rendering: -o-crisp-edges;
			image-rendering: crisp-edges;
			image-rendering: -webkit-optimize-contrast;
			image-rendering: optimize-contrast;
			image-rendering: pixelated;
			-ms-interpolation-mode: nearest-neighbor;
		}
	</style>
</head>

<body>
	<div id=""gameContainer"">
		<canvas id=""unity-canvas"" data-pixel-art=""{{{ OPTIMIZE_FOR_PIXEL_ART }}}""></canvas>
		<script src=""Build/{{{ LOADER_FILENAME }}}""></script>
		<script>
			var canvas = document.querySelector(""#unity-canvas"");
			var config = {
				dataUrl: ""Build/{{{ DATA_FILENAME }}}"",
				frameworkUrl: ""Build/{{{ FRAMEWORK_FILENAME }}}"",
				codeUrl: ""Build/{{{ CODE_FILENAME }}}"",
#if MEMORY_FILENAME
				memoryUrl: ""Build/{{{ MEMORY_FILENAME }}}"",
#endif
#if SYMBOLS_FILENAME
				symbolsUrl: ""Build/{{{ SYMBOLS_FILENAME }}}"",
#endif
				streamingAssetsUrl: ""StreamingAssets"",
				companyName: ""{{{ COMPANY_NAME }}}"",
				productName: ""{{{ PRODUCT_NAME }}}"",
				productVersion: ""{{{ PRODUCT_VERSION }}}"",
			};
			var scaleToFit = true;
			function progressHandler(progress) {
				var percent = progress * 100 + '%';
				canvas.style.background = 'linear-gradient(to right, white, white ' + percent + ', transparent ' + percent + ', transparent) no-repeat center';
				canvas.style.backgroundSize = '100% 1rem';
			}
			function onResize() {
				var container = canvas.parentElement;
				var w;
				var h;

				if (scaleToFit) {
					w = window.innerWidth;
					h = window.innerHeight;

					var r = {{{ HEIGHT }}} / {{{ WIDTH }}};

					if (w * r > window.innerHeight) {
						w = Math.min(w, Math.ceil(h / r));
					}
					h = Math.floor(w * r);
				} else {
					w = {{{ WIDTH }}};
					h = {{{ HEIGHT }}};
				}

				container.style.width = canvas.style.width = w + ""px"";
				container.style.height = canvas.style.height = h + ""px"";
				container.style.top = Math.floor((window.innerHeight - h) / 2) + ""px"";
				container.style.left = Math.floor((window.innerWidth - w) / 2) + ""px"";
			}
			createUnityInstance(canvas, config, progressHandler).then(function (instance) {
				canvas = instance.Module.canvas;
				onResize();
			});
			window.addEventListener('resize', onResize);
			onResize();

			if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
				// Mobile device style: fill the whole browser client area with the game canvas:
				const meta = document.createElement('meta');
				meta.name = 'viewport';
				meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
				document.getElementsByTagName('head')[0].appendChild(meta);
			}
		</script>
	</div>
</body>

</html>
";
	}
}