# AI-Driven Unity Game with Voice Commands

This project demonstrates the integration of AI-driven personality, speech recognition, and function calling in a Unity game environment. The game features an interactive shooting system where players can control targeting and power-ups through voice commands.

## Project Overview

The game features a dynamic shooting system where players can:
- Target specific enemies (frog and chicken) using voice commands
- Activate special power-ups through voice commands
- Interact with an AI-driven personality that responds to player actions

### Character Personality

The game features an AI-driven personality that:
- Responds to player voice commands with contextual understanding
- Provides feedback on targeting and power-up activations
- Maintains a consistent personality throughout interactions
- Adapts responses based on the current game state

## Implementation Details

### Speech-to-Text System
- Uses HuggingFace's Automatic Speech Recognition API
- Implements a hold-to-record button system
- Processes voice input in real-time
- Converts speech to text for command processing

### AI Integration
- Utilizes LLM (Large Language Model) for command interpretation
- Implements function calling for precise command execution
- Processes natural language commands into game actions
- Maintains context awareness for better command interpretation

### Function Calling System
The project implements a robust function calling system that:
- Maps voice commands to specific game actions
- Handles targeting commands (frog/chicken)
- Manages power-up activations (Double Trouble/Power Shot)
- Provides error handling and feedback

## Available Commands

### Targeting Commands
- "target frog"
- "aim at chicken"
- "shoot at frog"
- "target chicken"

### Power-up Commands
- "double trouble" - Activates dual-targeting mode
- "power shot" - Activates enhanced bullet mode

## Technologies Used

### Core Technologies
- Unity Game Engine
- C# Programming Language
- HuggingFace API for Speech Recognition
- LLM (Large Language Model) for Command Processing

### Key Components
- Speech Recognition System
- AI Command Processing
- Function Calling Framework
- Bullet Physics System
- Power-up Management System

## Project Structure

### Main Scripts
- `SpeechToText.cs`: Handles voice input processing
- `AutomaticTargetShooter.cs`: Manages targeting and shooting mechanics
- `PowerUpFunctions.cs`: Controls power-up functionality
- `Bullet.cs`: Manages bullet behavior and physics
- `Enemy.cs`: Handles enemy behavior and health

### Function Call System
- `PowerUpFunctions.cs`: Static class for power-up management
- `DirectionFunctions.cs`: Handles directional movement
- `ColorFunctions.cs`: Manages color-based interactions

## Setup Instructions

1. Clone the repository
2. Open the project in Unity
3. Ensure all required dependencies are installed
4. Set up the following in the Unity Inspector:
   - Assign bullet prefab
   - Set up spawn points
   - Configure enemy references (frog and chicken)
   - Set up LLMCharacter reference
   - Configure speech recognition settings

## Dependencies

- Unity 2021.3 or later
- HuggingFace API access
- LLM integration package
- Required Unity packages:
  - TextMeshPro
  - Input System
  - Physics2D

## Usage

1. Press and hold the record button to start voice input
2. Speak your command clearly
3. Release the button to process the command
4. Watch for visual feedback on command execution

## Error Handling

The system includes comprehensive error handling for:
- Speech recognition failures
- Invalid commands
- Missing references
- Power-up execution errors
- Targeting system issues

## Future Improvements

- Additional power-up types
- Enhanced AI personality responses
- More complex targeting scenarios
- Improved voice command recognition
- Additional enemy types and behaviors

## Contributing

Feel free to submit issues and enhancement requests! 