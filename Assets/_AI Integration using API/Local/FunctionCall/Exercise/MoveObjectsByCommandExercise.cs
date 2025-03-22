using System.Collections.Generic;
using System.Reflection;
using LLMUnity;
using UnityEngine;
using UnityEngine.UI;

public class MoveObjectsByCommandExercise : MonoBehaviour
{
    public LLMCharacter llmCharacter;
    public InputField playerText;
    public RectTransform blueSquare;
    public RectTransform redSquare;

    void Start()
    {
        // Find LLMCharacter if not set
        if (llmCharacter == null)
        {
            llmCharacter = FindObjectOfType<LLMCharacter>();
            if (llmCharacter == null)
            {
                Debug.LogError("LLMCharacter not found in scene!");
                return;
            }
        }
    }
    string[] GetFunctionNames<T>()
    {
        List<string> functionNames = new List<string>();
        foreach (var function in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)) 
            functionNames.Add(function.Name);
        return functionNames.ToArray();
    }

    // Add the new ConstructColorPrompt method
    string ConstructColorPrompt(string message)
    {
        string[] colorFunctions = GetFunctionNames<ColorFunctions>();

        string prompt = "Command: " + message + "\n\n";
        prompt += "Reply with EXACTLY one line:\n";
        prompt += "Choose ONE of these color functions:\n";
        
        foreach(string col in colorFunctions)
            prompt += col + " ";
        
        prompt += "\n\nExample:\nBlueColor";

        return prompt;
    }

    // Modify ConstructDirectionPrompt to only handle directions
    string ConstructDirectionPrompt(string message)
    {
        string[] directionFunctions = GetFunctionNames<DirectionFunctions>();

        string prompt = "Command: " + message + "\n\n";
        prompt += "Reply with EXACTLY one line:\n";
        prompt += "Choose ONE of these direction functions:\n";
        
        foreach(string dir in directionFunctions)
            prompt += dir + " ";
        
        prompt += "\n\nExample:\nMoveUp";

        return prompt;
    }

    public async void onInputFieldSubmit(string message)
    {
        /* Example prompts and test cases for students:
         * 
         * Test inputs:
         * - "move the blue square up"
         * - "move red square to the right"
         * - "make the blue square go down"
         * - "move the red square left"
         * 
         * Expected AI responses examples:
         * - Direction: "MoveUp", "MoveRight", "MoveDown", "MoveLeft", "NoDirectionsMentioned"
         * - Color: "BlueColor", "RedColor", "NoColorMentioned"
         */

        // TODO: Student Exercise
        // 1. Disable the input field
        // 2. Get direction and color from AI using llmCharacter.Chat
        // 3. Convert AI responses to actual Vector3 and Color using reflection
        //       Color color = (Color)typeof(ColorFunctions).GetMethod(MethodName).Invoke(null, null);
        //       Vector3 direction = (Vector3)typeof(DirectionFunctions).GetMethod(MethodName).Invoke(null, null);
        // 4. Move the correct square in the specified direction
        // 5. Re-enable the input field

         try
        {
            // Check if llmCharacter is available
            if (llmCharacter == null)
            {
                llmCharacter = FindObjectOfType<LLMCharacter>();
                if (llmCharacter == null)
                {
                    Debug.LogError("LLMCharacter not found!");
                    return;
                }
            }

            // Get color first
            string colorResponse = await llmCharacter.Chat(ConstructColorPrompt(message));
            string color = colorResponse.Split('\n')[0];
            Debug.Log($"Color: {color}");

            // Then get direction
            string directionResponse = await llmCharacter.Chat(ConstructDirectionPrompt(message));
            string direction = directionResponse.Split('\n')[0];
            Debug.Log($"Direction: {direction}");

            // Convert to Vector3 and Color using the helper classes
            Vector3 moveDirection = (Vector3)typeof(DirectionFunctions)
                .GetMethod(direction)
                .Invoke(null, null);

            Color squareColor = (Color)typeof(ColorFunctions)
                .GetMethod(color)
                .Invoke(null, null);

            // Move the appropriate square
            RectTransform squareToMove = GetObjectByColor(squareColor);
            if (squareToMove != null)
            {
                squareToMove.position += moveDirection * 50f;
                Debug.Log($"Moving {color} square {direction}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error processing speech command: {e.Message}");
            Debug.LogException(e); // Add full stack trace
        }
}

    private RectTransform GetObjectByColor(Color color)
    {
        if (color == Color.blue)
            return blueSquare;
        else if (color == Color.red)
            return redSquare;
        
        return null;
    }

    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    bool onValidateWarning = true;
    void OnValidate()
    {
        if (onValidateWarning && !llmCharacter.remote && llmCharacter.llm != null && llmCharacter.llm.model == "")
        {
            Debug.LogWarning($"Please select a model in the {llmCharacter.llm.gameObject.name} GameObject!");
            onValidateWarning = false;
        }
    }
} 