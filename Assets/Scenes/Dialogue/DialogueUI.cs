using System.Collections;
using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

public class DialogueUI : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Dialogue scriptObj;
    public GameObject portrait;
    float textSpeed = 0.06f;

    void Start()
    {
        // won't work in vaccum
        //        StartDialogue();
    }

    public void StartDialogueWithData(Dialogue newDialogue)
    {
        scriptObj = newDialogue;
        StartDialogue();
    }

    void StartDialogue()
    {
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        for (int i = 0; i < scriptObj.Lines.Length; i++)
        {
            textComponent.text = string.Empty;

            foreach (char c in scriptObj.Lines[i].text.ToCharArray())
            {
                textComponent.text += c;
                yield return new WaitForSeconds(textSpeed * scriptObj.Lines[i].speedScale);
            }

            yield return new WaitForSeconds(1);
        }
    }
}
