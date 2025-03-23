using System.IO;
using HuggingFace.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpeechToText : MonoBehaviour
{
    [SerializeField] private Button recordButton;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GeminiAPI geminiAPI;
    [SerializeField] private AutomaticTargetShooter targetShooter;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start()
    {
        if (geminiAPI == null) 
            geminiAPI = FindObjectOfType<GeminiAPI>();

        if (targetShooter == null)
            targetShooter = FindObjectOfType<AutomaticTargetShooter>();

        if (targetShooter == null)
        {
            Debug.LogError("AutomaticTargetShooter not found!");
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

    private async void SendRecording()
    {
        text.color = Color.yellow;
        text.text = "Sending...";
        
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, async response => {
            text.color = Color.white;
            text.text = response;

            try
            {
                // Process targeting and power-up commands
                if (targetShooter != null)
                {
                    targetShooter.ProcessVoiceCommand(response);
                }

                // Send to GeminiAPI for conversation if available
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

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
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