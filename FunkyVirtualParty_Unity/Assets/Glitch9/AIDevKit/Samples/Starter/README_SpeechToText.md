### 📢 Speech-to-Text Sample Not Included

I understand that many developers are interested in using Speech-to-Text (STT) functionality.  
However, I **did not include a sample** for it in the Starter package, for the following reasons:

#### ⚠ Why No Sample?

* STT requires complex audio setup:
  * Microphone access
  * Audio recording buffer management
  * File conversion (PCM/WAV)
  * Async upload to AI API

* These steps can be **overwhelming for beginners** and hard to demonstrate in a simple Unity scene.
* STT samples often require **platform-specific permissions** (e.g., Android/iOS), making out-of-the-box demos unreliable.

#### ✅ Pro Users: Just Use `SpeechToText` Component

If you're using the **Pro version**, you can:

* Add the `SpeechToText` component to any GameObject
* Set a few inspector options (e.g., model, language, events)
* Receive transcribed text without writing a single line of code

> 🎙️ Just hook into `onTextGenerated` or `onTranscriptGenerated` events and you’re ready to go!

---

I’m a solo developer building this tool to make AI more accessible in Unity.  
Please join [My Discord Channel](https://discord.gg/hgajxPpJYf) to ask questions, share feedback, or just say hi.  
Thanks for your support!
