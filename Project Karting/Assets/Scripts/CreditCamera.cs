using System.Collections;
using System.Collections.Generic;
using Handlers;
using UnityEngine;

public class CreditCamera : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float endY;
    private bool end;
    void Update()
    {
        if (!end)
        {
            transform.position -= Vector3.up * speed *Time.deltaTime;
            if (transform.position.y < endY)
            {
                end = true;
                StartCoroutine(LoadMenu());
            }
        }
    }

    IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.instance.LoadMainMenu();
    }
    
}
