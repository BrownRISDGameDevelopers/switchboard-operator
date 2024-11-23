using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject volumeManagerObject;

    void Start()
    {
        GameObject vm = Instantiate(volumeManagerObject);
        DontDestroyOnLoad(vm);

        SceneManager.LoadScene((int) Constants.SceneIndexTable.Menu);
    }
}
