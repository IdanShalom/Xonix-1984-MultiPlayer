using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FightPress : MonoBehaviour
{
    public static int _player1Counter;
    public static int _player2Counter;
    [SerializeField] private TextMeshProUGUI _pressCounter;
    [SerializeField] private GameObject _player;
    [SerializeField] private FightTimer _timer;

    
    void Start()
    {
        if (_player.gameObject.CompareTag("Player"))
        {
            _pressCounter.text = "0";
        }
        else
        {
            _pressCounter.text =  "0";
        }
    }

    void Update()
    {
        if (!_timer.IsOver())
        {
            if (_player.gameObject.CompareTag("Player"))
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    _player1Counter++;
                }
                _pressCounter.text = _player1Counter.ToString();
            }
            if (_player.gameObject.CompareTag("Player_2"))
            {
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    _player2Counter++;
                }
                _pressCounter.text = _player2Counter.ToString();
            }
        }
    }
    
    public void setPoints()
    {
        _player1Counter = 0;
        _player2Counter = 0;
    }

}
