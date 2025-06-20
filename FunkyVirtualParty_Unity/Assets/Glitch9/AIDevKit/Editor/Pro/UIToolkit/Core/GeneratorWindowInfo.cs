
using UnityEditor;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class GeneratorWindowInfo
    {
        internal string windowName;
        internal string parametersLabel;
        internal string defaultModelId;
        internal string[] promptSamples;

        // static  
        internal static string GetDefaultModelId<TWindow>() where TWindow : EditorWindow
        {
            AIDevKitDebug.Mark($"GetDefaultModelId<{typeof(TWindow).Name}>");
            return GetInfo<TWindow>().defaultModelId;
        }

        internal static GeneratorWindowInfo GetInfo<TWindow>() where TWindow : EditorWindow
        {
            return typeof(TWindow).Name switch
            {
                // Code
                nameof(CodeGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Code Generator",
                    parametersLabel = "Parameters",
                    defaultModelId = AIDevKitSettings.DefaultLLM,
                    promptSamples = _codePromptSamples
                },

                nameof(ComponentGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Unity Component Generator",
                    parametersLabel = "Parameters",
                    defaultModelId = AIDevKitSettings.DefaultLLM,
                    promptSamples = _componentPromptSamples
                },

                nameof(ComponentEditorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Component Editor",
                    parametersLabel = "Parameters",
                    defaultModelId = AIDevKitSettings.DefaultLLM,
                },

                // Image
                nameof(TextureGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Texture Generator",
                    parametersLabel = "Style & Parameters",
                    defaultModelId = AIDevKitSettings.DefaultIMG,
                    promptSamples = _texturePromptSamples
                },

                nameof(IconGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Icon Generator",
                    parametersLabel = "Style & Parameters",
                    defaultModelId = AIDevKitSettings.DefaultIMG,
                    promptSamples = _iconPromptSamples
                },

                nameof(AvatarGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Avatar Generator",
                    parametersLabel = "Style & Parameters",
                    defaultModelId = AIDevKitSettings.DefaultIMG,
                    promptSamples = _avatarPromptSamples
                },

                nameof(BackgroundGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Background Generator",
                    parametersLabel = "Style & Parameters",
                    defaultModelId = AIDevKitSettings.DefaultIMG,
                    promptSamples = _backgroundPromptSamples
                },

                // Audio
                nameof(SpeechGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Speech Generator",
                    parametersLabel = "Style & Parameters",
                    defaultModelId = AIDevKitSettings.DefaultTTS,
                    promptSamples = _speechPromptSamples
                },

                nameof(SoundFXGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Sound Effect Generator",
                    parametersLabel = "Style & Parameters",
                    defaultModelId = null, // SoundFX does not have a default model
                    promptSamples = _soundFXPromptSamples
                },

                // Video
                nameof(VideoGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Video Generator",
                    parametersLabel = "Style & Parameters",
                    defaultModelId = AIDevKitSettings.DefaultVID,
                    promptSamples = _videoPromptSamples
                },

                // Music
                nameof(MusicGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "Music Generator",
                    parametersLabel = "Style & Parameters",
                    //defaultModelId = Lyria.DefaultModelId,
                    promptSamples = _videoPromptSamples // Using video prompts as a placeholder
                },

                // UXML
                nameof(UxmlGeneratorWindow) => new GeneratorWindowInfo
                {
                    windowName = "UXML Generator",
                    parametersLabel = "Style & Parameters",
                    defaultModelId = AIDevKitSettings.DefaultLLM,
                    promptSamples = _uxmlPromptSamples
                },

                // Default case for unknown windows
                _ => new GeneratorWindowInfo
                {
                    windowName = "Unknown",
                    parametersLabel = "Parameters",
                    defaultModelId = AIDevKitSettings.DefaultLLM,
                    promptSamples = new string[0]
                }
            };
        }

        // sample prompts
        private static readonly string[] _componentPromptSamples = new string[]
        {
            "3D player movement script using Rigidbody with walking and jumping support via user input.",
            "Character health system supporting damage, healing, and death event triggering.",
            "Basic inventory system for storing, adding, and removing stackable items.",
            "Real-time day-night cycle system using Unity lighting and Time.deltaTime.",
            "AI enemy logic with waypoint patrol and player chase when in sight range.",
            "Save/load system serializing player stats and position into JSON using Application.persistentDataPath.",
            "Camera follow logic with smooth motion, customizable offset, and damping.",
            "Branching dialogue system parsing JSON conversations and supporting player choices.",
            "Weapon controller with shooting, reloading, and UI-based ammo management.",
            "Weather system featuring randomized rain, snow, and clear sky with effects.",
            "Basic crafting system combining items into new ones via defined recipes.",
            "Interactive door logic opening on player proximity and key press.",
            "2D platformer controller with movement, jump, double jump, and ground check using Rigidbody2D.",
            "Procedural terrain generation using Perlin noise for randomized heightmaps.",
            "Color-matching puzzle system requiring alignment of three or more blocks to score.",
            "Enemy spawner triggering timed spawns at random spawn points.",
            "Basic car controller with acceleration, steering, and braking using Rigidbody.",
            "Destructible object system breaking objects into pieces upon strong impact.",
            "Multiplayer lobby system enabling hosting and joining local sessions using Unity Transport.",
            "Gravity controller with dynamic direction switching for player gravity."
        };


        private static readonly string[] _codePromptSamples = new string[]
        {
            "Generic singleton pattern for global system management with thread-safety and lazy initialization.",
            "Game state manager for handling scene transitions using additive loading and lifecycle callbacks.",
            "Input manager supporting customizable key bindings and remapping via Unity Input System.",
            "Scene loader utility with asynchronous loading and fade-in/fade-out UI transitions.",
            "Data persistence manager using JSON and Application.persistentDataPath for player progress.",
            "Custom event system enabling decoupled communication via delegates and events.",
            "Audio manager managing music, SFX, audio pooling, and volume control by category.",
            "Basic achievement tracker with UI popups and persistent save of unlocked achievements.",
            "Dialogue parser for structured dialogue with UI typing animation support.",
            "Score manager tracking current score, high score, and event-driven updates.",
            "Inventory manager for NPCs with categorization, storage, retrieval, and usage support.",
            "Procedural 2D grid-based map generator with walkable and blocked tiles.",
            "Screen resolution utility managing aspect ratio and resolution based on user/device settings.",
            "Currency manager handling multiple currencies and secure in-game transactions.",
            "Logging utility supporting info, warning, error levels, and optional file output.",
            "Cloud save system utilizing UnityWebRequest and JSON for player data sync.",
            "Dynamic weather system with time-based changes and smooth visual blending.",
            "Resource manager handling Addressables with automatic reference counting.",
            "Difficulty manager adjusting parameters such as enemy health and spawn rate.",
            "UI manager for dynamic menu, button, and HUD panel creation using layout groups and prefabs."
        };


        private static readonly string[] _texturePromptSamples = new string[]
        {
            "Cracked stone floor with moss growing in the gaps.",
            "Seamless sci-fi metal panel with glowing lines.",
            "Cartoon-style grassy field under sunlight.",
            "Ancient parchment paper with subtle stains.",
            "Dark obsidian rock surface with light reflections.",
            "Wooden planks with worn edges and nails.",
            "Pixel art sand texture for a desert biome.",
            "Wet asphalt road with small puddles.",
            "Stylized lava flow with glowing cracks.",
            "Rough concrete wall with graffiti marks.",
            "Fantasy magic circle with glowing runes.",
            "Fabric texture with a soft velvet finish.",
            "Rusty metal sheet with bolts and corrosion.",
            "Cloudy sky with scattered sunlight.",
            "Low-poly stone tiles for dungeon floor.",
            "Marble surface with white and gold veins.",
            "Futuristic carbon fiber pattern with shine.",
            "Stylized ice surface with blue tint and frost.",
            "Smooth skin texture for a fantasy creature.",
            "Underwater coral ground with scattered shells.",
        };

        private static readonly string[] _iconPromptSamples = new string[]
        {
            "A glowing mana potion in a glass bottle with a cork stopper, centered on white background, RPG inventory icon.",
            "A golden key with an ornate handle and mysterious engravings, isolated, fantasy style.",
            "A silver throwing dagger with a curved blade and dark leather grip, game icon style.",
            "A magical spell book with a glowing emblem on the cover, hovering slightly, top-down RPG UI icon.",
            "A heart-shaped gemstone emitting a soft red glow, stylized game UI asset.",
            "A futuristic energy core surrounded by floating rings, sci-fi equipment icon.",
            "A wooden treasure chest slightly open with gold spilling out, stylized and centered.",
            "A pair of enchanted boots with glowing soles and arcane patterns, fantasy inventory item.",
            "A winged emblem made of polished bronze, representing flight or agility, clean white background.",
            "A robotic eye module with blinking lights, sci-fi theme, isolated icon style.",
            "A swirling vortex of water contained in a floating orb, elemental spell icon.",
            "A ghostly mask with hollow eyes and a faint mist trail, centered, dark fantasy style.",
            "A crystal apple with a bite taken out, glowing faintly, fairy tale themed icon.",
            "A pair of crossed banners representing two factions, colorful and detailed.",
            "An ancient coin with a dragon engraving, worn edges, stylized currency icon.",
            "A glowing rune stone hovering with subtle particle effects, top-down view.",
            "A cybernetic glove with cables and embedded lights, sci-fi RPG gear icon.",
            "A floating crown made of starlight and energy, magical legendary item icon.",
            "A golden egg with reflective surface and faint crack lines, mythic item style.",
            "A pair of enchanted glasses with swirling lenses, whimsical, magical support item."
        };

        private static readonly string[] _avatarPromptSamples = new string[]
        {
            "Schoolgirl with twin braids in a sailor uniform, facing forward with a smile",
            "Hacker with glowing goggles and hoodie, eyes filled with focus",
            "Elf archer with silver hair and leaf armor, calm expression",
            "Robot with round eyes and glowing chest, gentle demeanor",
            "Old wizard with white beard and cloak, smiling peacefully",
            "Chef with white hat, thick mustache, and red scarf",
            "Vampire with pale skin, red eyes, and a dramatic cape",
            "Cat girl with pastel hair, big eyes, and a wand in hand",
            "Space pilot holding a helmet, scarred cheek, starry background",
            "Executive in high-tech suit with earpiece, standing confidently",
            "Man with monocle, top hat, and curled mustache",
            "Knight holding a sword, wearing classic armor",
            "Rocker with mohawk, leather jacket, and piercings",
            "Forest spirit with wooden mask and glowing eyes",
            "Barista in apron, holding a steaming cup of coffee",
            "Detective in trench coat and fedora, serious gaze",
            "Priestess with silver tattoos and crescent staff",
            "Ninja with a red visor, standing in misty alley",
            "Bear wearing overalls, waving warmly",
            "Hero with emblem on chest and flowing cape"
        };

        private static readonly string[] _backgroundPromptSamples = new string[]
        {
            "A serene forest clearing with dappled sunlight filtering through the leaves.",
            "A bustling medieval marketplace with colorful tents and villagers.",
            "A futuristic city skyline at night with neon lights and flying vehicles.",
            "A tranquil beach at sunset with gentle waves and palm trees.",
            "An ancient temple ruins overgrown with vines and moss.",
            "A snowy mountain landscape with a clear blue sky and distant peaks.",
            "A dark, misty swamp with twisted trees and glowing fireflies.",
            "A vibrant coral reef underwater scene with colorful fish and plants.",
            "A post-apocalyptic wasteland with abandoned vehicles and crumbling buildings.",
            "A magical fairy glen with glowing mushrooms and sparkling lights."
        };

        private static readonly string[] _soundFXPromptSamples = new string[]
        {
            "A powerful explosion in a sci-fi battlefield.",
            "Futuristic engine ignition for a spaceship takeoff.",
            "Soft ambient rain hitting a window at night.",
            "A magical chime indicating a level-up event.",
            "Footsteps on metal in a large industrial hall.",
            "An eerie wind blowing through a haunted forest.",
            "Computer UI beep for invalid action.",
            "A sword unsheathing followed by a quick slash.",
            "High-pitched zap sound of a teleportation portal.",
            "A dramatic boom for a trailer intro.",
            "Mechanical gears turning slowly in a robot factory.",
            "Creaky wooden door opening in an old house.",
            "Electric shock hit sound for a cyberpunk weapon.",
            "Punch impact in a cartoon-style fight.",
            "Digital glitch burst for a corrupted system.",
            "Bubble popping underwater in a cartoon.",
            "Fantasy spell casting with sparkles and whoosh.",
            "Arcade-style coin pickup chime.",
            "Whistle blow to start a sports match.",
            "Menu selection sound with retro game vibe.",
        };

        private static readonly string[] _speechPromptSamples = new string[]
        {
            "Welcome to our virtual assistant. How can I help you today?",
            "Your meeting is scheduled for 10 AM tomorrow in the main conference room.",
            "The weather today is sunny with a high of 25 degrees Celsius and a low of 15 degrees.",
            "You have three new messages in your inbox. Would you like me to read them?",
            "Turning off the lights. Have a good night!",
            "Your current balance is one thousand two hundred thirty-four dollars and fifty-six cents.",
            "Now playing your favorite playlist. Enjoy the music!",
            "I didn't understand that. Can you please repeat it?",
            "The nearest coffee shop is 5 minutes away by walk. Would you like directions?",
            "Setting an alarm for 7 AM tomorrow morning.",
            "Your package has been delivered to your front door.",
            "This is a reminder to drink water and stay hydrated.",
            "Traffic is heavy on your usual route. Would you like an alternative route?",
            "Your workout is complete. Great job! You've burned 500 calories.",
            "Error 404: The information you requested is not available.",
            "Happy birthday, Sarah! Wishing you a wonderful year ahead.",
            "The movie starts at 8 PM tonight. Shall I book the tickets?",
            "You have reached your destination. Parking is available on your right.",
            "Please enter your password followed by the pound key.",
            "Your flight to Paris is now boarding. Please proceed to gate 32A.",
        };

        private static readonly string[] _videoPromptSamples = new string[]
        {
            "Aerial drone shot flying over a snowy mountain range at sunrise.",
            "Close-up of raindrops falling on a window with city lights in the background.",
            "Time-lapse of a bustling street market transitioning from day to night.",
            "Slow motion of a dog catching a frisbee in a grassy park.",
            "Underwater shot of a scuba diver swimming near colorful coral reefs.",
            "Panoramic view of a futuristic city skyline glowing at night.",
            "Tracking shot through a dense bamboo forest swaying in the wind.",
            "First-person view riding a bicycle through a European old town alley.",
            "Wide-angle shot of fireworks exploding over a calm lake.",
            "Slow pan over an ancient temple hidden in the jungle.",
            "Low-angle shot of skyscrapers with clouds moving fast overhead.",
            "Dramatic scene of a thunderstorm forming over an open field.",
            "Zoom-in on a steaming cup of coffee placed on a rainy window sill.",
            "POV of a person walking through a neon-lit street in a cyberpunk city.",
            "A cat stretching lazily in a sunbeam on a wooden floor.",
            "Helicopter shot circling a medieval castle on a cliff.",
            "Time-lapse of flowers blooming in a field during spring.",
            "First-person view walking along a beach with waves lapping at your feet.",
            "Orbiting shot around a satellite floating above Earth.",
            "Cinematic view of a train passing through a foggy forest at dawn."
        };

        private static readonly string[] _uxmlPromptSamples = new string[]
        {
            "Create a Unity UXML file for a simple button with text 'Click Me' and a background color of blue.",
            "Design a UXML layout for a login form with fields for username and password, and a submit button.",
            "Generate a UXML file for a dropdown menu with three options: 'Option 1', 'Option 2', 'Option 3'.",
            "Create a UXML file for a panel containing an image and a label below it.",
            "Design a UXML layout for a settings menu with toggles for sound and notifications.",
            "Generate a UXML file for a chat interface with an input field at the bottom and a scrollable message area above.",
            "Create a UXML file for a grid layout displaying four buttons in two rows.",
            "Design a UXML layout for an inventory system with slots arranged in a grid format.",
            "Generate a UXML file for a progress bar that fills up as tasks are completed.",
            "Create a UXML file for a modal dialog with title, message, and OK/Cancel buttons."
        };
    }
}