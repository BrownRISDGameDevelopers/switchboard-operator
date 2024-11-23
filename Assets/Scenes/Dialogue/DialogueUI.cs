using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Dialogue scriptObj;
    public Image portrait;


    private GameObject visuals;
    float textSpeed = 0.06f;


    public delegate void DialogueDoneDelegate();
    public event DialogueDoneDelegate OnDialogueDone;

    void Start()
    {
        visuals = transform.GetChild(0).gameObject;
        SetVisualsVisible(false);
        // won't work in vaccum
        if (scriptObj)
            StartDialogue();


    }

    public void StartDialogueWithData(Dialogue newDialogue)
    {
        scriptObj = newDialogue;
        StartDialogue();
    }

    void StartDialogue()
    {
        SetVisualsVisible(true);
        StartCoroutine(TypeLine());
    }

    void SetPortrait(CharacterInfo character, DialogueLine line)
    {
        Sprite texture = character.Portrait;
        switch (line.portraitType)
        {
            case PortraitEmotion.DEFAULT:
                texture = character.Portrait;
                break;
            case PortraitEmotion.EXTRA_1:
                texture = character.ExtraPortrait1;
                break;
            case PortraitEmotion.EXTRA_2:
                texture = character.ExtraPortrait2;
                break;

        }
        if (texture == null)
            return;
        portrait.sprite = texture;
    }

    IEnumerator TypeLine()
    {
        for (int i = 0; i < scriptObj.Lines.Length; i++)
        {

            textComponent.text = string.Empty;
            SetPortrait(scriptObj.FromCharacter, scriptObj.Lines[i]);
            foreach (char c in scriptObj.Lines[i].text.ToCharArray())
            {
                textComponent.text += c;
                yield return new WaitForSeconds(textSpeed * scriptObj.Lines[i].speedScale);
            }

            yield return new WaitForSeconds(1);
        }
        SetVisualsVisible(false);
        OnDialogueDone?.Invoke();
    }


    void SetVisualsVisible(bool visible)
    {
        if (visuals == null)
            return;
        visuals.SetActive(visible);
    }
}
