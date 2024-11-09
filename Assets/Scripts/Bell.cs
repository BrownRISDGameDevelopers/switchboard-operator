using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator wiggle(){
        Vector3 targetPosition = new Vector3(transform.position.x + 0.02f, transform.position.y, transform.position.z);
        Vector3 originalPosition = transform.position;
        for(int i=0; i<3; i++){
            float time = 0;
            while (time < 1f) {
                time += Time.deltaTime * 20;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 20 * Time.deltaTime);
                yield return null;
            }
            time = 0;
            while (time < 1f) {
                time += Time.deltaTime * 20;
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, 20 * Time.deltaTime);
                yield return null;
            }
        }
        
    }
}
