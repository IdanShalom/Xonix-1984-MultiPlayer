using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FightTimer : MonoBehaviour
{
    public  static float timer = 0;
    private const int TIMER_START = 7;
    private bool _showingWinner;
    [SerializeField] private TextMeshProUGUI _timerText;
    
    // Start is called before the first frame update
    void Start()
    {
        _timerText.text = "Fight!" + "\n" +"  " + Mathf.Floor(timer);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            UpdateTimer();
        }
        else
        {
            timer = 0;
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        if (!_showingWinner)
        {
            _timerText.text = "Fight!" + "\n" + "  " + Mathf.Floor(timer); 
        }
    }

    public void setTimer()
    {
        _showingWinner = false;
        timer = TIMER_START;
        _timerText.text = "Fight!" + "\n" +"  " + Mathf.Floor(timer);
    }

    public void ShowWinner(string winner)
    {
        _showingWinner = true;
        _timerText.text = winner + "\nWINS!";
    }

    public bool IsOver()
    {
        return timer <= 1;
    }

}
