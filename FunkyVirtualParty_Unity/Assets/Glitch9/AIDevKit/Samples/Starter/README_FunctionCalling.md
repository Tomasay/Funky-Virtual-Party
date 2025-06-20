### ⚙️ Function Calling Sample Not Included

I understand that many developers are interested in using **Function Calling** with AI models (e.g., GPT-4o).
However, I **did not include a sample** for it in the Starter package, for the following reasons:

#### ⚠ Why No Sample?

* Function calling depends on:
  * Creating meaningful methods with parameters
  * Defining each parameter's schema manually
  * Converting methods to Function Calls
  * Handling return values or async logic (e.g., API calls)

* This setup is too **project-specific** and **hard to demonstrate in a generic sample**.
* Many developers may want to call their own gameplay or UI logic, which can’t be shown in a minimal scene.

#### ✅ Pro Users: Use `FunctionManager` with Zero Code

If you're using the **Pro version**, you can:

* Add the `FunctionManager` component to a GameObject
* Register your existing MonoBehaviour methods from the Inspector
* Define input parameter types, enum values, and descriptions
* AI will be able to call these methods via `function_call` requests
* Return values are automatically sent back as JSON

> 🛠️ Just implement the method, describe it, and let AI call it dynamically.

---

I’m a solo developer building this tool to make AI more accessible in Unity.
Please join [My Discord Channel](https://discord.gg/hgajxPpJYf) to ask questions, share feedback, or just say hi.
Thanks for your support!
