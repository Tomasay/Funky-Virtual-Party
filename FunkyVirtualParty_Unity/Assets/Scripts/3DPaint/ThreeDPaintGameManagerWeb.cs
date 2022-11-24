using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class ThreeDPaintGameManagerWeb : GameManagerWeb
{
    [DllImport("__Internal")]
    private static extern void OpenInputKeyboard();

    [DllImport("__Internal")]
    private static extern void CloseInputKeyboard();

    [DllImport("__Internal")]
    private static extern void OnKeyboardInput();

    [DllImport("__Internal")]
    private static extern void UpdateInputFieldText(string txt);

    [DllImport("__Internal")]
    private static extern void SetPointerDownOnButton(bool isDown);

    [SerializeField]
    TMP_Text headerText;

    [SerializeField]
    Canvas inputCanvas, guessingCanvas, resultsCanvas;

    [SerializeField]
    GameObject playerInputParent;

    [SerializeField]
    TMP_InputField answerInputField;

    [SerializeField]
    ButtonEvents answerInputButton;

    [SerializeField]
    KeyboardActivator answerInputActivator;

    [SerializeField]
    Button submitButton;

    [SerializeField]
    GameObject answerButtonPrefab, answerButtonParent;

    [SerializeField]
    GameObject playerNameIconPrefab, playerNamesIconParent;

    [SerializeField]
    GameObject drawingModel, linesParent;

    [SerializeField]
    Camera drawingPhaseCamera, guessingPhaseCamera;

    bool typingAnswer = false; //Is player typing their answer?
    bool playersAnswering = false; //Are we still waiting for any player to submit their answer?
    bool guessing = false; //Are players guessing?

    Dictionary<string, string> playerAnswers;

    private void Awake()
    {
        playerAnswers = new Dictionary<string, string>();

        answerInputButton.onPointerDown.AddListener(ButtonPointerDown);
        answerInputButton.onPointerUp.AddListener(ButtonPointerUp);

        if(ClientManagerWeb.instance) ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
        if (ClientManagerWeb.instance) ClientManagerWeb.instance.Manager.Socket.On<string, string>("InfoToXR", InfoReceived);

        inputCanvas.enabled = true;
        guessingCanvas.enabled = false;
        resultsCanvas.enabled = false;
    }

    void Update()
    {
        if (typingAnswer)
        {
            answerInputField.ActivateInputField();
            answerInputField.caretPosition = answerInputField.text.Length;
        }

        if(guessing)
        {
            drawingModel.transform.Rotate(0, Time.deltaTime * 20, 0);
            linesParent.transform.Rotate(0, Time.deltaTime * 20, 0);
        }
    }

    void InfoReceived(string info, string playerID)
    {
        if (playersAnswering)
        {
            playerAnswers.Add(playerID, info);
        }
    }

    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("SetNewPrompt"))
        {
            headerText.text = data; //Set prompt text
            playerInputParent.SetActive(true); //Enable input
            playersAnswering = true;
        }
        else if (methodName.Equals("ShowBlurredView"))
        {
            inputCanvas.enabled = false;
        }
        else if (methodName.Equals("DoneDrawing"))
        {
            playersAnswering = false;
            guessing = true;

            guessingCanvas.enabled = true;

            drawingPhaseCamera.gameObject.SetActive(false);
            guessingPhaseCamera.gameObject.SetActive(true);

            foreach (KeyValuePair<string, string> entry in playerAnswers)
            {
                GameObject ab = Instantiate(answerButtonPrefab, answerButtonParent.transform);
                ab.GetComponentInChildren<TMP_Text>().text = entry.Value;
                ab.GetComponent<Button>().onClick.AddListener(delegate { SubmitGuess(entry.Value, entry.Key); });
            }
        }
        else if(methodName.Equals("GuessedRight"))
        {
            AddPlayerToResults(data, true);
        }
        else if (methodName.Equals("GuessedWrong"))
        {
            AddPlayerToResults(data, false);
        }
        else if (methodName.Equals("GameOver"))
        {
            resultsCanvas.enabled = true;

            StartCoroutine("EndGame");
        }
    }

    public void OpenKeyboard()
    {
        typingAnswer = true;
        OpenInputKeyboard();
    }

    public void UpdateText(string input)
    {
        answerInputField.text = input;
    }

    public void ButtonPointerDown()
    {
        SetPointerDownOnButton(true);
    }

    public void ButtonPointerUp()
    {
        StartCoroutine("ButtonDown");
    }

    IEnumerator ButtonDown()
    {
        yield return new WaitForSeconds(1);
        SetPointerDownOnButton(false);
    }

    public void CheckAnswerInput(string input)
    {
        submitButton.interactable = (input.Length > 0);
    }

    public void SetField(TMP_InputField f)
    {
        UpdateInputFieldText(f.text);
    }

    public void SubmitAnswer()
    {
        if (ClientManagerWeb.instance) ClientManagerWeb.instance.Manager.Socket.Emit("InfoFromPlayer", answerInputField.text);

        typingAnswer = false;
        CloseInputKeyboard();
        answerInputField.DeactivateInputField();

        headerText.text = "Waiting for all players to submit their answer...";
        playerInputParent.SetActive(false);
    }

    void SubmitGuess(string guess, string guessPlayerID)
    {
        if (ClientManagerWeb.instance) ClientManagerWeb.instance.Manager.Socket.Emit("InfoFromPlayer", guessPlayerID);
        guessingCanvas.enabled = false;
    }

    void AddPlayerToResults(string playerID, bool correct)
    {
        GameObject pi = Instantiate(playerNameIconPrefab, playerNamesIconParent.transform);
        pi.GetComponentInChildren<TMP_Text>(true).text = ClientManagerWeb.instance.GetPlayerByID(playerID).PlayerName;
        pi.GetComponentInChildren<Button>(true).interactable = false;
        pi.GetComponent<Image>().color = correct ? Color.green : Color.red;
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5);

        ClientManagerWeb.instance.LoadMainMenu();
    }
}