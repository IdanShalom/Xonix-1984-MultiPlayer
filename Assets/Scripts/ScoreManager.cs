using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Tilemap _seaSpace;
    [SerializeField] private GameObject _player;
    [SerializeField] private TextMeshProUGUI _percentageTextValue;
    private const int PERCENTAGE_DEV = 22;
    private const int WIN_CONDITION = 50;
    private int _originalCount;
    private int old_value;
    private int _activeTiles;
    private int _previousTilesActive = 0;
    public static int value = 0;
    public static bool player1Win;
    public static bool player2Win;
    
    // Start is called before the first frame update
    void Start()
    {
        _activeTiles = CountActiveTiles();
        _previousTilesActive = _activeTiles;
        _originalCount = _activeTiles;
        _percentageTextValue.text = "Full: 0%";
    }

    public void SetScore()
    {
        value = CalculatePercentage();
        _percentageTextValue.text = $"Full: {value}%";
    }

    private int CountActiveTiles()
    {
        int count = 0;
        foreach (var tilePosition in _seaSpace.cellBounds.allPositionsWithin)
        {
            if (_seaSpace.HasTile(tilePosition)) 
                count++;
        }
        return count;
    }

    private int CalculatePercentage()
    {
        return -(PERCENTAGE_DEV - _activeTiles * PERCENTAGE_DEV / _originalCount);
    }
    
    private void Update()
    {
        if(value>=WIN_CONDITION)
        {
            Win();
        }
        
        _activeTiles = CountActiveTiles();
        if (_activeTiles != _previousTilesActive)
        {
            SetScore();
            _previousTilesActive = _activeTiles;
        }
    }

    public void Win()
    {
        if (_player.gameObject.CompareTag("Player"))
        {
            player2Win = true;
        }
        else 
        {
            player1Win = true;
        }
    }
    
}
