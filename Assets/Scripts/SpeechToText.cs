using System.IO;
using HuggingFace.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*public class SpeechToText : MonoBehaviour
{
 [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;

    // Event for speech recognition completion
    public event System.Action<string> OnSpeechRecognized;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start()
    {
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        stopButton.interactable = false;
    }

    private void Update()
    {
        if (recording && Microphone.GetPosition(null) >= clip.samples)
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        text.color = Color.white;
        text.text = "Recording...";
        startButton.interactable = false;
        stopButton.interactable = true;
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private void SendRecording()
    {
        text.color = Color.yellow;
        text.text = "Sending...";
        stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            text.color = Color.white;
            text.text = response;
            startButton.interactable = true;
            
            // Trigger the event with recognized text
            OnSpeechRecognized?.Invoke(response);
        }, error => {
            text.color = Color.red;
            text.text = error;
            startButton.interactable = true;
        });
    }

    // Rest of the code remains the same
    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        // Existing EncodeAsWAV implementation...
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}*/

/*public class SpeechToText : MonoBehaviour
{
    [SerializeField] private Button recordButton;
    [SerializeField] private TextMeshProUGUI text;

    public event System.Action<string> OnSpeechRecognized;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start()
    {
        // Add button hold events
        recordButton.GetComponent<Button>().onClick.AddListener(() => { });  // Empty listener needed
        EventTrigger trigger = recordButton.gameObject.AddComponent<EventTrigger>();

        // Hold begin
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { StartRecording(); });
        trigger.triggers.Add(pointerDown);

        // Hold end
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { StopRecording(); });
        trigger.triggers.Add(pointerUp);
    }

    private void StartRecording()
    {
        text.color = Color.white;
        text.text = "Recording...";
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private void StopRecording()
    {
        if (!recording) return;

        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private void SendRecording()
    {
        text.color = Color.yellow;
        text.text = "Sending...";
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            text.color = Color.white;
            text.text = response;
            OnSpeechRecognized?.Invoke(response);
        }, error => {
            text.color = Color.red;
            text.text = error;
        });
    }

    // Rest of the code remains the same
    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        // Existing EncodeAsWAV implementation...
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}*/
/*public class SpeechToText : MonoBehaviour
{
    [SerializeField] private Button recordButton;
    [SerializeField] private TextMeshProUGUI text;
    private MoveObjectsByCommandExercise moveObjectsExercise; // Remove SerializeField

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start()
    {
        // Find the MoveObjectsByCommandExercise in the scene
        moveObjectsExercise = FindObjectOfType<MoveObjectsByCommandExercise>();
        if (moveObjectsExercise == null)
        {
            Debug.LogError("Could not find MoveObjectsByCommandExercise in the scene!");
            return;
        }

        // Add button hold events
        recordButton.GetComponent<Button>().onClick.AddListener(() => { });
        EventTrigger trigger = recordButton.gameObject.AddComponent<EventTrigger>();

        // Hold begin
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { StartRecording(); });
        trigger.triggers.Add(pointerDown);

        // Hold end
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { StopRecording(); });
        trigger.triggers.Add(pointerUp);
    }

private void StartRecording()
    {
        text.color = Color.white;
        text.text = "Recording...";
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private void StopRecording()
    {
        if (!recording) return;

        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private void SendRecording()
    {
        if (moveObjectsExercise == null)
        {
            moveObjectsExercise = FindObjectOfType<MoveObjectsByCommandExercise>();
            if (moveObjectsExercise == null)
            {
                Debug.LogError("MoveObjectsByCommandExercise not found in scene!");
                return;
            }
        }

        text.color = Color.yellow;
        text.text = "Sending...";
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            text.color = Color.white;
            text.text = response;
            
            // Process the command automatically
            if (moveObjectsExercise != null)
            {
                moveObjectsExercise.onInputFieldSubmit(response);
            }
            else
            {
                Debug.LogError("MoveObjectsByCommandExercise reference not set!");
            }
        }, error => {
            text.color = Color.red;
            text.text = error;
        });
    }

    // Keep existing EncodeAsWAV method
    // Rest of the code remains the same
    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        // Existing EncodeAsWAV implementation...
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}*/

public class SpeechToText : MonoBehaviour
{
    [SerializeField] private Button recordButton;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GeminiAPI geminiAPI; // Add GeminiAPI reference
    private MoveObjectsByCommandExercise moveObjectsExercise;
    [SerializeField] private AutomaticTargetShooter targetShooter;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start()
    {
        // Find references if not set
        moveObjectsExercise = FindObjectOfType<MoveObjectsByCommandExercise>();
        if (geminiAPI == null) geminiAPI = FindObjectOfType<GeminiAPI>();

        if (moveObjectsExercise == null || geminiAPI == null)
        {
            Debug.LogError("Required components not found!");
            return;
        }

        // Find the shooter if not assigned
        if (targetShooter == null)
        {
            targetShooter = FindObjectOfType<AutomaticTargetShooter>();
        }

        // Add button hold events
        recordButton.GetComponent<Button>().onClick.AddListener(() => { });
        EventTrigger trigger = recordButton.gameObject.AddComponent<EventTrigger>();

        // Hold begin
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { StartRecording(); });
        trigger.triggers.Add(pointerDown);

        // Hold end
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { StopRecording(); });
        trigger.triggers.Add(pointerUp);
    }

    private void StartRecording()
    {
        text.color = Color.white;
        text.text = "Recording...";
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private void StopRecording()
    {
        if (!recording) return;

        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private async void SendRecording()
    {
        if (moveObjectsExercise == null)
        {
            moveObjectsExercise = FindObjectOfType<MoveObjectsByCommandExercise>();
        }
        if (geminiAPI == null)
        {
            geminiAPI = FindObjectOfType<GeminiAPI>();
        }

        text.color = Color.yellow;
        text.text = "Sending...";
        
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, async response => {
            text.color = Color.white;
            text.text = response;

            try
            {
                // Process movement commands
                moveObjectsExercise.onInputFieldSubmit(response);

                // Process targeting commands
                ProcessTargetingCommand(response);

                // Send to GeminiAPI for conversation
                if (geminiAPI != null)
                {
                    geminiAPI.ProcessSpeechInput(response);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error processing responses: {e.Message}");
                text.color = Color.red;
                text.text = "Error processing command";
            }
        }, error => {
            text.color = Color.red;
            text.text = error;
        });
    }

    private void ProcessTargetingCommand(string command)
    {
        if (targetShooter == null) return;

        command = command.ToLower();
        
        // Check for targeting commands
        if (command.Contains("target") || command.Contains("shoot") || command.Contains("aim"))
        {
            if (command.Contains("blue"))
            {
                targetShooter.SetTarget("blue");
            }
            else if (command.Contains("red"))
            {
                targetShooter.SetTarget("red");
            }
        }
    }

    // Keep existing EncodeAsWAV method
    // Rest of the code remains the same
    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        // Existing EncodeAsWAV implementation...
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}