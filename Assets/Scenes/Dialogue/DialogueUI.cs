using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Dialogue scriptObj;
    float textSpeed = 0.06f;

    void Start()
    {
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
