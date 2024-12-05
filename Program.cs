using Avalonia;
using System;
using System.IO;
using System.Diagnostics;
using Vosk;
using Avalonia.Controls;

namespace summarize_video;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

class AudioProcessor
{
    public static void ExtractAudio(string videoFilePath, string audioOutputPath)
    {
        string ffmpegPath = string.Empty;

        if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg", "linux", "ffmpeg");
        }
        else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg", "windows", "ffmpeg.exe");
        }

        if (!File.Exists(ffmpegPath))
            throw new FileNotFoundException($"FFmpeg binary not found at {ffmpegPath}");

        string ffmpegCommand = $"-i \"{videoFilePath}\" -vn -acodec pcm_s16le -ar 16000 -ac 1 \"{audioOutputPath}\"";

        ProcessStartInfo processStartInfo = new()
        {
            FileName = ffmpegPath,
            Arguments = ffmpegCommand,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = Process.Start(processStartInfo);
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            string errorOutput = process.StandardError.ReadToEnd();
            throw new Exception($"FFmpeg error: {errorOutput}");
        }
    }

    public static string TranscribeAudio(string audioFilePath, string modelPath)
    {
        using var model = new Model(modelPath);
        using var recognizer = new VoskRecognizer(model, 16000);
        using var audioStream = File.OpenRead(audioFilePath);

        byte[] buffer = new byte[4096];
        int bytesRead;
        while ((bytesRead = audioStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            recognizer.AcceptWaveform(buffer, bytesRead);
        }

        return recognizer.FinalResult();
    }
}