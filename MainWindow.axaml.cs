using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Vosk;

namespace summarize_video;

public partial class MainWindow : Window
{
    private string selectedVideoPath;
    private string extractedAudioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.wav");
    private string voskModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vosk-model");

    public MainWindow()
    {
        InitializeComponent();
        SelectVideoButton.Click += SelectVideoButton_Click;
        StartButton.Click += StartButton_Click;
    }

    private async void SelectVideoButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var options = new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("Video Files")
                {
                    Patterns = new[] { "*.mp4", "*.avi", "*.mkv", "*.mov" }
                }
            }
        };

        var result = await this.StorageProvider.OpenFilePickerAsync(options);

        if (result.Count > 0)
        {
            selectedVideoPath = result[0].Path.LocalPath;
            StatusText.Text = $"Selected Video: {Path.GetFileName(selectedVideoPath)}";
        }
        else
        {
            StatusText.Text = "No video selected";
        }
    }



    private async void StartButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
{
    if (string.IsNullOrEmpty(selectedVideoPath))
    {
        StatusText.Text = "Please select a video file first.";
        return;
    }

    if(File.Exists(extractedAudioPath)){
            File.Delete(extractedAudioPath);
    }

    StatusText.Text = "Processing...";
    StartButton.IsEnabled = false;

    try
    {
        string transcription = await Task.Run(() =>
        {
            AudioProcessor.ExtractAudio(selectedVideoPath, extractedAudioPath);

            if (File.Exists(extractedAudioPath))
            {
                return AudioProcessor.TranscribeAudio(extractedAudioPath, voskModelPath);
            }
            else
            {
                throw new FileNotFoundException("Audio file not found after extraction.");
            }
        });

        if(File.Exists(extractedAudioPath)){
            File.Delete(extractedAudioPath);
        }

        StartButton.IsEnabled = true;
        StatusText.Text = "Transcription completed.";
        TranscriptionOutput.Text = transcription;
    }
    catch (Exception ex)
    {
        StatusText.Text = $"Error: {ex.Message}";
    }
}
}