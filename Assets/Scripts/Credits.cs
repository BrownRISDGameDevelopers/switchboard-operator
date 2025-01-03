using UnityEngine;

public class Credits : MonoBehaviour
{
    public UnityEngine.UI.Button buttonBack;

    void Start()
    {
        // Connect buttons to relevant functions
        buttonBack.onClick.AddListener(onBack);
    }

    void onBack()
    {
        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();
        Destroy(this.gameObject);
    }
}
