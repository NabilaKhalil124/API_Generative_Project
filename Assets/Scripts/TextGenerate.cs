/*using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

[Serializable]
public class TextGenerationRequest
{
    public string inputs;
}

[Serializable]
public class TextGenerationResponse
{
    public GeneratedText[] generated_texts;
}

[Serializable]
public class GeneratedText
{
    public string generated_text;
}

public class TextGenerate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _outputText;
    [SerializeField] private Button _generateButton;
    [SerializeField] private TMP_InputField _inputField;

    private const string API_KEY = "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";
    private const string API_URL = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";

    private void Awake()
    {
        _generateButton.onClick.AddListener(OnButtonClick_GenerateText);
    }

    private void OnDestroy()
    {
        _generateButton.onClick.RemoveListener(OnButtonClick_GenerateText);
    }

    public async Task GenerateText(string inputText)
    {
        try
        {
            _generateButton.interactable = false;

            // ✅ Correct JSON format
            string jsonPayload = $"{{ \"inputs\": \"{inputText}\", \"parameters\": {{ \"max_new_tokens\": 50, \"temperature\": 0.7, \"top_k\": 50, \"top_p\": 0.95 }} }}";
            
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonPayload);

            using UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + API_KEY);

            Debug.Log($"Sending request with payload: {jsonPayload}");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                _outputText.text = $"Error: {request.error}";
                return;
            }

            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"Received response: {jsonResponse}");
            
            // Wrap the response array in a root object for JsonUtility to parse
            string wrappedJson = $"{{\"generated_texts\":{jsonResponse}}}";
            TextGenerationResponse response = JsonUtility.FromJson<TextGenerationResponse>(wrappedJson);
            
            if (response.generated_texts != null && response.generated_texts.Length > 0)
            {
                // Combine all generated texts with line breaks between them
                string allTexts = string.Join("\n\n", 
                    Array.ConvertAll(response.generated_texts, text => text.generated_text));
                _outputText.text = allTexts;
            }
            else
            {
                _outputText.text = "No response from API";
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error generating text: {e.Message}");
            _outputText.text = "Failed to generate text";
        }
        finally
        {
            _generateButton.interactable = true;
        }
    }

    public void OnButtonClick_GenerateText()
    {
        string inputText = _inputField ? _inputField.text : "Please tell me about";
        _ = GenerateText(inputText);
    }
}*/

/*
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

[Serializable]
public class TextGenerationRequest
{
    public string inputs;
}

[Serializable]
public class TextGenerationResponse
{
    public GeneratedText[] generated_texts;
}

[Serializable]
public class GeneratedText
{
    public string generated_text;
}

public class TextGenerate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _outputText;
    [SerializeField] private SpeechToText _speechToText; // Reference to SpeechToText component

    private const string API_KEY = "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";
    private const string API_URL = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";

    private void Awake()
    {
        // Subscribe to speech recognition completion
        if (_speechToText != null)
        {
            _speechToText.OnSpeechRecognized += HandleSpeechRecognized;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from speech recognition
        if (_speechToText != null)
        {
            _speechToText.OnSpeechRecognized -= HandleSpeechRecognized;
        }
    }

    // Handle speech recognition result
    private void HandleSpeechRecognized(string recognizedText)
    {
        _ = GenerateText(recognizedText);
    }

    public async Task GenerateText(string inputText)
    {
        try
        {
            string jsonPayload = $"{{ \"inputs\": \"{inputText}\", \"parameters\": {{ \"max_new_tokens\": 50, \"temperature\": 0.7, \"top_k\": 50, \"top_p\": 0.95 }} }}";
            
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonPayload);

            using UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + API_KEY);

            Debug.Log($"Sending request with payload: {jsonPayload}");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                _outputText.text = $"Error: {request.error}";
                return;
            }

            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"Received response: {jsonResponse}");
            
            string wrappedJson = $"{{\"generated_texts\":{jsonResponse}}}";
            TextGenerationResponse response = JsonUtility.FromJson<TextGenerationResponse>(wrappedJson);
            
            if (response.generated_texts != null && response.generated_texts.Length > 0)
            {
                string allTexts = string.Join("\n\n", 
                    Array.ConvertAll(response.generated_texts, text => text.generated_text));
                _outputText.text = allTexts;
            }
            else
            {
                _outputText.text = "No response from API";
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error generating text: {e.Message}");
            _outputText.text = "Failed to generate text";
        }
    }
}*/