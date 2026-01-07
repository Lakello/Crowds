using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Anvil.WebBuilderPro
{
    internal static class LocalHostRunner
    {
        public static bool IsRunning => _httpListenerThread?.IsAlive ?? false;

        private static HttpListener _httpListener;
        private static Thread _httpListenerThread;
        private static CancellationTokenSource _cancellationTokenSource;

        public static event Action OnServerStarted;
        public static event Action OnServerStopped;

        public static void Start(string buildPath)
        {
            Global.Log("LocalHostRunner.Start()", LogFilter.LocalHostRunner);
            Stop();
            // Set the URL for the local host server
            string prefix = Global.ProModel.LocalHostAddress;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(prefix);

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            _httpListenerThread = new Thread(() =>
{
    var listener = _httpListener; // Local copy
    try
    {
        listener.Start();
    }
    catch (HttpListenerException ex)
    {
        UnityEngine.Debug.LogError($"Failed to start HTTP listener: {ex.Message}");
        return;
    }

    while (listener.IsListening && !token.IsCancellationRequested)
    {
        try
        {
            var contextTask = listener.GetContextAsync();
            contextTask.Wait(token);
            var context = contextTask.Result;

            // *** Decode the URL-encoded path ***
            var relativeUrl = Uri.UnescapeDataString(context.Request.Url.AbsolutePath).TrimStart('/');
            var filePath = Path.Combine(buildPath, relativeUrl);

            // Fallback to index.html if the requested file is not found
            if (!File.Exists(filePath))
                filePath = Path.Combine(buildPath, "index.html");

            var buffer = File.ReadAllBytes(filePath);
            context.Response.ContentLength64 = buffer.Length;

            // Content-Type and Content-Encoding Handling
            SetContentTypeAndEncoding(context.Response, filePath);

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown; no action needed
            Global.Log($"Shutting Down: operatuion cancelled", LogFilter.LocalHostRunner);
        }
        catch (HttpListenerException ex) when (ex.ErrorCode == 995 || ex.ErrorCode == 64)
        {
            // Error code 995: ERROR_OPERATION_ABORTED (The I/O operation has been aborted because of either a thread exit or an application request.)
            // Error code 64: The specified network name is no longer available.
            // These exceptions are expected when stopping the listener; no action needed.
            Global.Log($"Shutting Down {ex}", LogFilter.LocalHostRunner);
            break;
        }
        catch (ObjectDisposedException)
        {
             Global.Log($"Shutting Down: object disposed objection.", LogFilter.LocalHostRunner);
            break;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to process request: {e.Message}");
        }
    }
});

            Global.Log($"Starting LocalHostRunner on {prefix}", LogFilter.LocalHostRunner);
            _httpListenerThread.Start();
            OnServerStarted?.Invoke();
        }


        public static void Stop()
        {
            Global.Log("LocalHostRunner.Stop()", LogFilter.LocalHostRunner);
            if (_httpListener != null)
            {
                try
                {
                    if (_httpListener.IsListening)
                    {
                        _httpListener.Stop();
                    }
                    _httpListener.Close();
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Error stopping HTTP listener: {ex.Message}");
                }
                finally
                {
                    _httpListener = null;
                }
            }

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            if (_httpListenerThread != null && _httpListenerThread.IsAlive)
            {
                _httpListenerThread.Join();
                _httpListenerThread = null;
            }

            // Add a short delay to ensure socket is released
            Thread.Sleep(100);

            OnServerStopped?.Invoke();
        }


          // *** New Method to Set Content-Type and Content-Encoding ***
          private static void SetContentTypeAndEncoding(HttpListenerResponse response, string filePath)
          {
              Global.Log($"Setting Content-Type and Encoding for file: {filePath}", LogFilter.LocalHostRunner);
            
              string extension = Path.GetExtension(filePath); // e.g., .br, .gz, .js, etc.

              string contentType = "application/octet-stream"; // Default content type

              // Check if the file is compressed
              if (extension == ".br" || extension == ".gz")
              {
                  // Set Content-Encoding header
                  string encoding = extension == ".br" ? "br" : "gzip";
                  response.AddHeader("Content-Encoding", encoding);
                  Global.Log($"Content-Encoding set to: {encoding}", LogFilter.LocalHostRunner);

                  // Remove the compression extension to get the original file extension
                  string fileWithoutCompressionExtension = Path.GetFileNameWithoutExtension(filePath);
                  string originalExtension = Path.GetExtension(fileWithoutCompressionExtension);

                  contentType = GetContentTypeByExtension(originalExtension);
              }
              else
              {
                  contentType = GetContentTypeByExtension(extension);
              }

              response.ContentType = contentType;
              Global.Log($"Content-Type set to: {contentType}", LogFilter.LocalHostRunner);
          }

        private static string GetContentTypeByExtension(string extension)
        {
            switch (extension)
            {
                case ".html":
                    return "text/html";
                case ".js":
                    return "application/javascript";
                case ".wasm":
                    return "application/wasm";
                case ".json":
                    return "application/json";
                case ".data":
                    return "application/octet-stream";
                case ".symbols":
                    return "text/plain";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".svg":
                    return "image/svg+xml";
                case ".css":
                    return "text/css";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
