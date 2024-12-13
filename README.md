# Video Speech Summarizer

This application processes a video file by extracting its audio, transcribing the audio into text, and summarizing the transcribed text using a lightweight AI model. It is built using C#, AvaloniaUI, and Python (for the summarization API).

## Features

- Extract audio from video files using FFmpeg.
- Transcribe audio using the Vosk speech recognition model.
- Summarize the transcription with a T5-small AI model using a Python Flask API.
- User-friendly interface built with AvaloniaUI.

## Requirements

### Prerequisites

1. **.NET 9.0 SDK**: Install from [Microsoft .NET Download](https://dotnet.microsoft.com/download/dotnet/9.0).
2. **Python 3.10 or higher**: Install from [Python.org](https://www.python.org/downloads/).
3. **pip**: Python package manager (comes with Python installations).

### Python Dependencies
Install the required Python packages:
```bash
pip install flask transformers torch torchvision torchaudio
```

## Installation

1. **Clone the Repository**
```bash
git clone https://github.com/markbakos/video_speech_summarizer.git
cd video_speech_summarizer
```

2. **Set Up FFmpeg**
   - Download the ffmpeg binary from their website [FFMPEG Binaries](https://ffbinaries.com/downloads).
   - Locate the `bin/Debug/net9.0/ffmpeg` folder in your project directory.
   - Inside, you will find two subfolders: `windows` and `linux`.
   - Place the appropriate FFmpeg binary for your operating system in the corresponding folder:
     - For Windows: Place the `ffmpeg.exe` file in `bin/Debug/net9.0/ffmpeg/windows`.
     - For Linux: Place the `ffmpeg` binary in `bin/Debug/net9.0/ffmpeg/linux`.

3. **Set Up Vosk Model**
   - Download the Vosk model from [Vosk Models](https://alphacephei.com/vosk/models).
   - Extract the model folder into `bin/Debug/net9.0/vosk-model`.

4. **Start the Python Summarization API**
   - Navigate to the `api/` folder:
     ```bash
     cd api
     ```
   - Start the Flask API:
     ```bash
     python summarizer.py
     ```

5. **Run the Application**
   - Navigate back to the main project folder:
     ```bash
     cd ../
     ```
   - Build and run the application:
     ```bash
     dotnet build
     dotnet run
     ```

## Usage

1. Launch the application.
2. Click "Select Video" to choose a video file.
3. Click "Start" to process the video.
   - The progress bar will indicate the processing status. (WIP).
   - A summarized version of the text will be displayed after transcription.

## Project Structure

- **Main Project Folder**
  - `bin/Debug/net9.0/ffmpeg/`: Contains the FFmpeg binaries.
  - `bin/Debug/net9.0/vosk-model/`: Contains the Vosk speech recognition model.
- **API Folder**
  - `summarizer.py`: Python Flask API for text summarization.

## Troubleshooting

1. **FFmpeg Not Found**
   Ensure you placed the FFmpeg binary in the correct `ffmpeg` subfolder for your operating system.

2. **Vosk Model Not Found**
   Ensure the Vosk model is downloaded and extracted into `bin/Debug/net9.0/vosk-model/`.

3. **API Issues**
   - Ensure Python dependencies are installed.
   - Verify that the Flask API is running at the specified port.

4. **Permission Errors**
   On Linux, make the FFmpeg binary executable:
   ```bash
   chmod +x bin/Debug/net9.0/ffmpeg/linux/ffmpeg
   ```

## Contributing
Feel free to fork this repository, make changes, and submit a pull request.

