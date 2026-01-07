using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;
using System.IO.Compression;
using UnityEditor.Build;
using Debug = UnityEngine.Debug;


namespace Anvil.WebBuilderPro
{
    internal class DisableMouseAccelerationAfterBuildService: IPostprocessBuildWithReport
    {

        public DisableMouseAccelerationAfterBuildService()
        {
         
        }

        public int callbackOrder => 10;
     
        public void OnPostprocessBuild(BuildReport report)
        {
            Global.Log("Disable Mouse Acceleration After Build Service()", LogFilter.DisableMouseAcceleration);

            var model = Global.ProModel;

            if (model == null)
            {
                Global.Log("model is null, skipping", LogFilter.DisableMouseAcceleration);
                return;
            }


            if(!model.DisableMouseAcceleration.Value){
                Global.Log("Disable Mouse Acceleration is false, skipping", LogFilter.DisableMouseAcceleration);
                return;
            }
            if (report.summary.platform != BuildTarget.WebGL)
            {
                Global.Log("Build is not WebGL, skipping", LogFilter.DisableMouseAcceleration);
                return;
            }
            // string outputPath = Path.Combine(report.summary.outputPath, "Build/Web/");
            var outputPath = report.summary.outputPath;
            ValidateCompressionType(outputPath);
        }

        private void ValidateCompressionType(string buildPath)
        {
            Global.Log("Validating compression type", LogFilter.DisableMouseAcceleration);

            switch (PlayerSettings.WebGL.compressionFormat)
            {
                case WebGLCompressionFormat.Disabled:
                    Global.Log("\tCompression format is disabled", LogFilter.DisableMouseAcceleration);
                    var jsFiles = Directory.GetFiles(buildPath, "*.js", SearchOption.AllDirectories);
                    foreach (var filePath in jsFiles)
                    {
                        // Avoid modifying .js files that are already covered by .js.gz processing
                        if (filePath.EndsWith(".js.gz") || filePath.EndsWith(".js.br"))
                            continue;

                        ModifyUncompressed(filePath);
                    }
                    break;
                case WebGLCompressionFormat.Gzip:
                    Global.Log("\tCompression format is Gzip", LogFilter.DisableMouseAcceleration);
                    var gzippedFiles = Directory.GetFiles(buildPath, "*.js.gz", SearchOption.AllDirectories);
                    if (gzippedFiles.Length <= 0)
                    {
                        Debug.LogWarning("No Gzip files found. Skipping Gzip modification.");
                        return;
                    }
                    foreach (var filePath in gzippedFiles)
                    {
                        ModifyGzipped(filePath);
                    }
                    break;
                case WebGLCompressionFormat.Brotli:
                    Global.Log("\tCompression format is Brotli", LogFilter.DisableMouseAcceleration);
                    var brotliFiles = Directory.GetFiles(buildPath, "*.js.br", SearchOption.AllDirectories);
                    if (brotliFiles.Length <= 0)
                    {
                        Debug.LogWarning("No Brotli files found. Skipping Brotli modification.");
                        return;
                    }
                    foreach (var filePath in brotliFiles)
                    {
                        ModifyBrotli(filePath);
                    }
                    break;
                default:
                    Debug.LogError("Unknown compression format");
                    break;
            }
        }

private void ModifyUncompressed(string jsPath)
{
    //TODO: This doesn't work anymore? try old version.
    Global.Log($"\t\tModifying uncompressed file: {jsPath}", LogFilter.DisableMouseAcceleration);
    string tempFilePath = jsPath;
    string content = File.ReadAllText(tempFilePath);

    // Define the custom pointer lock function
    string pointerLockFunction = @"
function customRequestPointerLock(element) {
    if (element.requestPointerLock) {
        try {
            element.requestPointerLock({ unadjustedMovement: true });
        } catch {
            console.log('Failed to request pointer lock with unadjustedMovement. Falling back to default.');
            element.requestPointerLock();
        }
    }
}
";

    // Inject the custom function at the beginning of the script
    content = pointerLockFunction + content;

    // Replace all instances of 'canvas.requestPointerLock()' with 'customRequestPointerLock(canvas)'
    content = content.Replace("canvas.requestPointerLock()", "customRequestPointerLock(canvas)");

    Global.Log("\t\t Replacing 'canvas.requestPointerLock()' with 'customRequestPointerLock(canvas)'", LogFilter.DisableMouseAcceleration);

    File.WriteAllText(tempFilePath, content);
}

        
        #region Gzip Modification
        
        private void ModifyGzipped(string gzipJsPath)
        {
            Global.Log($"\t\tModifying Gzip file: {gzipJsPath}", LogFilter.DisableMouseAcceleration);
            // Decompress the file
            var tempFilePath = Path.ChangeExtension(gzipJsPath, ".tmp.js");
            DecompressGzipFile(gzipJsPath, tempFilePath);
            
            // Make changes
            ModifyUncompressed(tempFilePath);
    
            File.Delete(gzipJsPath); // Delete the original gz file as gzip won't overwrite
            CompressGzipFile(tempFilePath, gzipJsPath); // Re-compress the file
            File.Delete(tempFilePath); // Delete the temporary decompressed file
        }
        
        // Example for decompressing GZIP
        private void DecompressGzipFile(string inputFile, string outputFile)
        {
            Global.Log($"\t\tDecompressing Gzip file: {inputFile}", LogFilter.DisableMouseAcceleration);
            using var originalFileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            using var decompressedFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            using var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress);
            decompressionStream.CopyTo(decompressedFileStream);
        }

        private void CompressGzipFile(string inputFile, string outputFile)
        {
            Global.Log($"\t\tCompressing Gzip file: {inputFile}", LogFilter.DisableMouseAcceleration);
            using var originalFileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            using var compressedFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
            originalFileStream.CopyTo(compressionStream);
        }
        
        #endregion

        #region Brotli Modification
        
        private void ModifyBrotli(string brotliJsPath)
        {
            Global.Log($"\t\tModifying Brotli file: {brotliJsPath}", LogFilter.DisableMouseAcceleration);
            // Decompress the file
            var tempFilePath = Path.ChangeExtension(brotliJsPath, ".tmp.js");
            DecompressBrotliFile(brotliJsPath, tempFilePath);
            
            // Makes changes
            ModifyUncompressed(tempFilePath);
            
            // Re-compress the file
            File.Delete(brotliJsPath); // Delete the original gz file as gzip won't overwrite
            CompressBrotliFile(tempFilePath, brotliJsPath); // Re-compress the file
            File.Delete(tempFilePath); // Delete the temporary decompressed file
        }

        private void DecompressBrotliFile(string inputFile, string outputFile)
        {
            Global.Log($"\t\tDecompressing Brotli file: {inputFile}", LogFilter.DisableMouseAcceleration);
            using var originalFileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            using var decompressedFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            using var decompressionStream = new BrotliStream(originalFileStream, CompressionMode.Decompress);
            decompressionStream.CopyTo(decompressedFileStream);
        }

        private void CompressBrotliFile(string inputFile, string outputFile)
        {
            Global.Log($"\t\tCompressing Brotli file: {inputFile}", LogFilter.DisableMouseAcceleration);
            using var originalFileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            using var compressedFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            using var compressionStream = new BrotliStream(compressedFileStream, CompressionMode.Compress);
            originalFileStream.CopyTo(compressionStream);
        }
        
        #endregion
    }
}