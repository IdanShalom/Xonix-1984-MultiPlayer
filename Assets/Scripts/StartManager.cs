using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [SerializeField] private AudioSource _startMusic;
    [SerializeField] private TextMeshProUGUI _pressSpace;
    private float _timer;

    private void Start()
    {
        _startMusic.Play();
        StartCoroutine(GameOverFlickerRoutine());
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StopCoroutine(GameOverFlickerRoutine());
            _startMusic.Stop();
            SceneManager.LoadScene("Multiplayer");
        }

    }
    
    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _pressSpace.enabled = true;
            yield return new WaitForSeconds(0.35f);
            _pressSpace.enabled = false;
            yield return new WaitForSeconds(0.35f);
            if (Input.GetKey(KeyCode.Space))
            {
                break;
            }
        }
    }
}
