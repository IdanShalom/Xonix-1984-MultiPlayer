using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private const float SCREEN_BLINK_TIME = 0.6f;
    private const float WAIT_BEFORE_WIN = 0.5f;
    private const float SHOW_WIN_FIGHT_TIME = 0.6f;
    private const float SHAKE_TIME = 0.5f;
    private const int SLIDER_DEV = 50;
    private const int PAUSE_SCALE = 0;
    private const int CONTINUE_SCALE = 1;
    private bool _inFight;
    private string _winner;
    private bool _gameOver;
    private Coroutine _cameraShake;
    private Vector3 _originalCameraPos;
    [SerializeField] private Animator _enterFight;
    [SerializeField] private Image _player1Win;
    [SerializeField] private Image _player2Win;
    [SerializeField] private Image _fight;
    [SerializeField] private GameObject _player1;
    [SerializeField] private GameObject _player2;
    [SerializeField] private AudioSource _musicInGame;
    [SerializeField] private AudioSource _musicInFight;
    [SerializeField] private AudioSource _musicInWin;
    [SerializeField] private Slider _fightSlider;
    [SerializeField] private GameObject _camera;
    
    
    
    void Start()
    {
        _originalCameraPos = _camera.transform.localPosition;
        _musicInFight.Stop();
        _musicInWin.Stop();
        _musicInGame.Play();
        _fight.gameObject.SetActive(false);
    }

    void Update()
    {
        if (_gameOver)
        {
            if(Input.GetKey(KeyCode.Y))
            {
                _musicInWin.Stop();
                _gameOver = false;
                setGameOver();
                Time.timeScale = CONTINUE_SCALE;
                _musicInGame.Play();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (Input.GetKey(KeyCode.N))
            {
                _musicInWin.Stop();
                _gameOver = false;
                setGameOver();
                Time.timeScale = CONTINUE_SCALE;
                SceneManager.LoadScene("Scenes/BeforeGame");
            }
        }
        else
        {
            if (ScoreManager.player1Win)
            {
                Time.timeScale = PAUSE_SCALE;
                _gameOver = true;
                StartCoroutine(Win(_player1Win));
            }
            if (ScoreManager.player2Win)
            {
                Time.timeScale = PAUSE_SCALE;
                _gameOver = true;
                StartCoroutine(Win(_player2Win));
            }
            if (PlayerMovement.PlayerHitPlayer)
            {
                Time.timeScale = PAUSE_SCALE;
                _enterFight.SetTrigger("EnterFight");
                StartCoroutine("ScreenBlink");
            }
            if (_inFight && FightTimer.timer != 0)
            {
                _fightSlider.value = SLIDER_DEV + FightPress._player2Counter - FightPress._player1Counter;
            }
            if (_inFight && FightTimer.timer == 0)
            {
                StopCoroutine(_cameraShake);
                _camera.transform.position = _originalCameraPos;
                if (FightPress._player1Counter == FightPress._player2Counter)
                {
                    _musicInFight.Play();
                    _musicInGame.Stop();
                    PlayerMovement.PlayerHitPlayer = false;
                    setPoints();
                    GetComponent<FightTimer>().setTimer();
                    Fight(); 
                }
                else if (FightPress._player1Counter > FightPress._player2Counter)
                {
                    _inFight = false;
                    _player2.GetComponent<PlayerMovement>().StartCoroutine("EnemeyHit");
                    _winner = "Player 1";
                    StartCoroutine(WaitForResetPlayer(_winner));
                }
                else 
                {
                    _inFight = false;
                    _player1.GetComponent<PlayerMovement>().StartCoroutine("EnemeyHit");
                    _winner = "Player 2";
                    StartCoroutine(WaitForResetPlayer(_winner));
                }
            }
        }
    }

    private void Fight()
    {
        _fight.gameObject.SetActive(true);
        _cameraShake = StartCoroutine(CameraShake.Shared.StartShake(SHAKE_TIME, SHAKE_TIME));
    }

    IEnumerator WaitForResetPlayer(string winner)
    {
        GetComponent<FightTimer>().ShowWinner(winner);
        yield return new WaitForSecondsRealtime(SHOW_WIN_FIGHT_TIME);
        _fight.gameObject.SetActive(false);
        _musicInFight.Stop();
        _musicInGame.Play();
        setPoints();
        GetComponent<FightTimer>().setTimer();
    }

    IEnumerator Win(Image winner)
    {
        yield return new WaitForSecondsRealtime(WAIT_BEFORE_WIN);
        _musicInGame.Stop();
        _musicInWin.Play();
        winner.gameObject.SetActive(true);
    }

    private void setGameOver()
    {
        _player1Win.gameObject.SetActive(false);
        _player2Win.gameObject.SetActive(false);
        _gameOver = false;
        ScoreManager.player1Win = false;
        ScoreManager.player2Win = false;
        setPoints();
        GetComponent<FightTimer>().setTimer();
        ScoreManager.value = 0;
    }

    IEnumerator ScreenBlink()
    {
        _inFight = true;
        _musicInFight.Play();
        _musicInGame.Stop();
        PlayerMovement.PlayerHitPlayer = false;
        setPoints();
        GetComponent<FightTimer>().setTimer();
        yield return new WaitForSecondsRealtime(SCREEN_BLINK_TIME);
        Fight();
    }
    

    private void setPoints()
    {
        var fightPress = GetComponents<FightPress>();
        foreach (var script in fightPress)
        {
            script.setPoints();
        }
    }
    
}
