using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

// This class handles communication with the Gemini API for generating content based on user prompts.
public class GeminiAPI : MonoBehaviour
{
    #region Enums & Constants
    // Enum to define the types of responses we can expect from the API.
    public enum ResponseMimeType
    {
        PlainText, // Plain text response
        Json       // JSON formatted response
    }

    // Base URL for the Gemini API
    private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta/models/";
    #endregion

    #region Serialized Fields
    [SerializeField] private string _modelName = "gemini-2.0-flash";
    [SerializeField] private string _apiKey;
    [SerializeField] private InputField _promptInputField;  // This will show the speech-to-text result
    [SerializeField] private Text _responseText;
    [SerializeField, TextArea(3, 10)] private string _systemInstructions;
    [SerializeField] private ResponseMimeType _responseMimeType = ResponseMimeType.Json;
    [SerializeField] private bool _enableChatHistory = true;
    [SerializeField] public List<Content> _chatHistory = new List<Content>();
    #endregion

    #region Chat History Management
    // Initializes or resets the chat history with system instructions if provided
    private void InitializeChatHistory()
    {
        _chatHistory.Clear(); // Clear existing chat history
    }

    // Creates a message object for the chat history
    private Content CreateMessageObject(string role, string text)
    {
        return new Content
        {
            role = role, // Role of the message sender (user or model)
            parts = new List<Part> { new Part { text = text } } // Message content
        };
    }

    // Public method to clear chat history
    public void ClearChatHistory()
    {
        InitializeChatHistory(); // Call to initialize chat history
    }
    #endregion

    #region Unity Lifecycle Methods
    // Unity method called when the script instance is being loaded
    private void Awake()
    {
        InitializeChatHistory(); // Initialize chat history on awake
    }

    #endregion

    #region UI Interaction
    // Handles the send button click event
    private async void OnButtonClick_SendPrompt()
    {
        // Check if the input field is empty
        if (string.IsNullOrEmpty(_promptInputField.text))
        {
            return; // Exit if no prompt is provided
        }

        _responseText.text = "Generating response..."; // Indicate processing

        // Generate content asynchronously
        string response = await GenerateContentAsync(_promptInputField.text);
        _responseText.text = response ?? "Failed to generate response"; // Display response or error message
    }
    #endregion

    #region API Communication
    // Structure for the request body sent to the API
    [Serializable]
    private struct GeminiRequestBody
    {
        public List<Content> contents; // List of content messages
        public Content systemInstruction; // System instruction content
        public GenerationConfig generationConfig; // Configuration for generation
    }

    // Structure representing a message in the chat
    [Serializable]
    public struct Content
    {
        public string role; // Role of the message sender
        public List<Part> parts; // Parts of the message
    }

    // Structure representing a part of a message
    [Serializable]
    public struct Part
    {
        public string text; // Text content of the part
    }

    // Structure for generation configuration
    [Serializable]
    public struct GenerationConfig
    {
        public string responseMimeType; // Expected response MIME type
    }

    // Creates the request body for the API call based on the prompt and chat history
    private GeminiRequestBody CreateRequestBody(string prompt)
    {
        var requestBody = new GeminiRequestBody
        {
            contents = new List<Content>(), // Initialize contents list
            generationConfig = new GenerationConfig() // Initialize generation config
        };

        // Add system instructions if provided
        if (!string.IsNullOrEmpty(_systemInstructions))
        {
            requestBody.systemInstruction = new Content
            {
                role = "system", // Role for system instructions
                parts = new List<Part> { new Part { text = _systemInstructions } } // Add instructions
            };
        }

        // Add chat history if enabled
        if (_enableChatHistory)
        {
            requestBody.contents = _chatHistory.Select(msg => new Content
            {
                role = msg.role, // Role from chat history
                parts = new List<Part> { new Part { text = msg.parts[0].text } } // Add message part
            }).ToList();

            // Add the user's current prompt
            requestBody.contents.Add(new Content
            {
                role = "user", // Role for the user
                parts = new List<Part> { new Part { text = prompt } } // Add user prompt
            });
        }
        else
        {
            // If chat history is not enabled, just add the user prompt
            requestBody.contents.Add(new Content
            {
                role = "user", // Role for the user
                parts = new List<Part> { new Part { text = prompt } } // Add user prompt
            });
        }

        // Set the response MIME type based on user selection
        if (_responseMimeType == ResponseMimeType.Json)
        {
            requestBody.generationConfig = new GenerationConfig
            {
                responseMimeType = "application/json" // Set to JSON
            };
        }

        return requestBody; // Return the constructed request body
    }

// Add this method to handle speech-to-text input
    public async void ProcessSpeechInput(string speechText)
    {
        if (string.IsNullOrEmpty(speechText)) return;

        // Update input field with speech text
        if (_promptInputField != null)
        {
            _promptInputField.text = speechText;
        }

        // Show processing status
        if (_responseText != null)
        {
            _responseText.text = "Processing...";
        }

        // Generate response
        string response = await GenerateContentAsync(speechText);
        
        // Update response text
        if (_responseText != null)
        {
            _responseText.text = response ?? "Failed to generate response";
        }
    }
    // Asynchronously generates content based on the provided prompt
    public async Task<string> GenerateContentAsync(string prompt)
    {
        string url = $"{BASE_URL}{_modelName}:generateContent?key={_apiKey}"; // Construct the API URL

        var requestBody = CreateRequestBody(prompt); // Create the request body
        string jsonData = JsonUtility.ToJson(requestBody); // Convert request body to JSON
        Debug.Log($"Sending request: {jsonData}"); // Log the request

        using UnityWebRequest request = new UnityWebRequest(url, "POST"); // Create a new web request
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData); // Convert JSON to byte array
        request.uploadHandler = new UploadHandlerRaw(bodyRaw); // Set the upload handler
        request.downloadHandler = new DownloadHandlerBuffer(); // Set the download handler
        request.SetRequestHeader("Content-Type", "application/json"); // Set content type header

        try
        {
            await request.SendWebRequest(); // Send the request asynchronously

            // Check if the request was successful
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API Request failed: {request.error}\nResponse: {request.downloadHandler.text}"); // Log error
                return null; // Return null on failure
            }

            // Parse the response from the API
            var responseJObject = JObject.Parse(request.downloadHandler.text);
            string aiResponse = responseJObject["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString(); // Extract AI response

            // parse the response and emotion
            JObject jObjectResponse = JObject.Parse(aiResponse);
            string response = jObjectResponse["response"].ToString();
            string emotion = jObjectResponse["emotion"].ToString().ToLower();
            ProcessEmotion(emotion);
            // If chat history is enabled and response is valid, add to chat history
            if (_enableChatHistory && !string.IsNullOrEmpty(aiResponse))
            {
                _chatHistory.Add(CreateMessageObject("model", aiResponse)); // Add AI response to history
            }

            return response; // Return the AI response
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during API request: {e.Message}"); // Log any exceptions
            return null; // Return null on exception
        }
    }
    
    #endregion
private void ProcessEmotion(string emotion){
        switch (emotion)
        {
            case "happy":
                Debug.Log("Happy");
                break;
            case "sad":
                Debug.Log("Sad");
                break;
            case "angry":
                Debug.Log("Angry");
                break;
            case "confused":
                Debug.Log("Confused");
                break;
            case "unsure":
                Debug.Log("Unsure");
                break;
            case "excited":
                Debug.Log("Excited");
                break;
            case "concerned":
                Debug.Log("Concerned");
                break;
            default:
                Debug.Log("Neutral");
                break;
        } 
    }

}
