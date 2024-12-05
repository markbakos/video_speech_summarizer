using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Newtonsoft.Json;
using Vosk;


namespace summarize_video;

public partial class MainWindow : Window
{
    private string selectedVideoPath;
    private string extractedAudioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.wav");
    private string voskModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vosk-model");
    private const string SummarizationApiUrl = "http://127.0.0.1:5000/summarize";

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
        ProgressBar.IsVisible = true;
        ProgressBar.IsIndeterminate = true;

        try
        {

            AudioProcessor.ExtractAudio(selectedVideoPath, extractedAudioPath);
            StatusText.Text = "Audio extracted successfully";

            ProgressBar.IsIndeterminate = false;
            ProgressBar.Value = 0;

            string transcription = AudioProcessor.TranscribeAudio(extractedAudioPath, voskModelPath);
            StatusText.Text = "Transcription completed.";

            string summary = await SummarizeText(transcription);
            TranscriptionOutput.Text = "\n\nSummary:\n" + summary;
            StatusText.Text = "Summarization completed.";

            if(File.Exists(extractedAudioPath))
            {
                File.Delete(extractedAudioPath);
            }

            StartButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
        }
    }


    private async Task<string> SummarizeText(string transcription)
    {
        using (HttpClient client = new HttpClient())
        {
            var payload = new { text = transcription };
            string json = JsonConvert.SerializeObject(payload);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(SummarizationApiUrl, content);
            response.EnsureSuccessStatusCode();

            string resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Dictionary<string,string>>(resultJson);
            return result.ContainsKey("summary") ? result["summary"] : "No summary available.";
        }
    }
}