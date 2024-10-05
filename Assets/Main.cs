using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene((int) Constants.SceneIndexTable.Menu);
    }
}
