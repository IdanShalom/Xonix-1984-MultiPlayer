using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class PlayerMovement : MonoBehaviour
{
    private const int FIRST_TRAIL_HIT = 1;
    private const int SECOND_TRAIL_HIT = 2;
    private const int PAUSE_SCALE = 0;
    private const int CONTINUE_SCALE = 1;
    private const float MIN_DISTANCE = 0.1f;
    private const float ENEMY_HIT_WAIT_TIME = 0.2f;
    private const float LERP_SPEED = 5f;
    private const int LIMITS_LAYER = 6;
    private const int LAND_LAYER = 9;
    private const int PLAYER_LAYER = 3;
    private const int ENEMY_LAND_LAYER = 12;
    private const float OVERLAP_RADIUS = .2f;
    public static bool PlayerHitPlayer;
    private bool _isLerping = false;
    private Vector3 _startPos;
    private List<Vector3> _enemeiesPositions;
    private bool _isMoving;
    private Vector3 _LocationToMove;
    private bool _arrowUp = false;
    private bool _arrowDown = false;
    private bool _arrowLeft = false;
    private bool _arrowRight = false;
    private bool _isDrawing;
    private Rigidbody2D _rigidbody;
    private List<Vector3> _linePositions;
    private EdgeCollider2D _edgeCollider2D;
    private GameObject[] _onlyInsideEnemies;
    private LineRenderer _lineRenderer;
    private  Vector3 _direction = Vector3.zero;
    private List<(int, Vector3)> _playerPrevoiusPositions;
    [SerializeField] private LayerMask _limitsOfGrid;
    [SerializeField] private Tilemap _seaSpace;
    [SerializeField] private Tilemap _seaSpace_2;
    [SerializeField] private Tilemap _landSpace;
    [SerializeField] private Tile _sea;
    [SerializeField] private GameObject[] _enemies;
    [SerializeField] private GameObject _otherPlayer;
    [SerializeField] private Tilemap _landSpace_2;
    [SerializeField] private Tilemap _enemyLand;
    [SerializeField] private Tile _land;
    [Range(18, 100)] public float speed = 18;


    private void Awake()
    {
        Time.timeScale = CONTINUE_SCALE;
    }

    void Start()
    {
        _startPos = transform.position;
        _edgeCollider2D = GetComponent<EdgeCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        Physics2D.IgnoreLayerCollision(PLAYER_LAYER, LIMITS_LAYER);
        Physics2D.IgnoreLayerCollision(PLAYER_LAYER, LAND_LAYER);
        Physics2D.IgnoreLayerCollision(PLAYER_LAYER,ENEMY_LAND_LAYER);
        _lineRenderer = GetComponent<LineRenderer>();
        _linePositions = new List<Vector3>();
        _LocationToMove = transform.position;
        _enemeiesPositions = new List<Vector3>();
        _enemeiesPositions.Add(_enemies[0].transform.position);
        _enemeiesPositions.Add(_enemies[1].transform.position);
        _enemeiesPositions.Add(_enemies[2].transform.position);
        _onlyInsideEnemies = new GameObject[3];
        _onlyInsideEnemies[0] = _enemies[0];
        _onlyInsideEnemies[1] = _enemies[1];
        _onlyInsideEnemies[2] = _enemies[2];
        _playerPrevoiusPositions = new List<(int, Vector3)>();
    }

    void Update()
    {
        UpdateEnemiesLocations();
        
        if (checkCollisions())
        {
            StartCoroutine("EnemeyHit");
        }
        if (_isMoving)
        {
            Move();
        }
        else
        {
            UpdatePlayerDirections(); 
        }
        if (_landSpace.HasTile(Vector3Int.FloorToInt(transform.position)))
        {
            CheckCollisionsWithTrail();
            _linePositions.Add(transform.position);
            DrawLine();
            setLineCollider();
            _isDrawing = true;
        }
        else
        {
            _playerPrevoiusPositions.Clear();
            if (_isDrawing && !_isLerping)
            {
                FillAreas();
                _linePositions.Clear();
                DrawLine();
                ResetEdgeCollider();
                _isDrawing = false;
            }
        }
    }

    private void UpdatePlayerDirections()
    {
        setMoevmentKeys();
        if (_arrowDown && !_arrowRight && !_arrowLeft && !_arrowUp)
        {
            _direction = Vector3.down;
            if (!Physics2D.OverlapCircle(transform.position + _direction, OVERLAP_RADIUS, _limitsOfGrid))
            {
                _LocationToMove += _direction;
                _isMoving = true;
            }
        }
        if (!_arrowDown && _arrowRight && !_arrowLeft && !_arrowUp)
        {
            _direction = Vector3.right;
            if (!Physics2D.OverlapCircle(transform.position + _direction, OVERLAP_RADIUS, _limitsOfGrid))
            {
                _LocationToMove += _direction;
                _isMoving = true;
            }
        }
        if (!_arrowDown && !_arrowRight && _arrowLeft && !_arrowUp)
        {
            _direction = Vector3.left;
            if (!Physics2D.OverlapCircle(transform.position + _direction, OVERLAP_RADIUS, _limitsOfGrid))
            {
                _LocationToMove += _direction;
                _isMoving = true;
            }
        }
        if (!_arrowDown && !_arrowRight && !_arrowLeft && _arrowUp)
        {
            _direction = Vector3.up;
            if (!Physics2D.OverlapCircle(transform.position + _direction, OVERLAP_RADIUS, _limitsOfGrid))
            {
                _LocationToMove += _direction;
                _isMoving = true;
            }
        }
    }

    private void setMoevmentKeys()
    {
        if (gameObject.CompareTag("Player"))
        {
            _arrowUp = Input.GetKey(KeyCode.UpArrow);
            _arrowDown = Input.GetKey(KeyCode.DownArrow);
            _arrowLeft = Input.GetKey(KeyCode.LeftArrow);
            _arrowRight = Input.GetKey(KeyCode.RightArrow);
        }
        else
        {
            _arrowUp = Input.GetKey(KeyCode.W);
            _arrowDown = Input.GetKey(KeyCode.S);
            _arrowLeft = Input.GetKey(KeyCode.A);
            _arrowRight = Input.GetKey(KeyCode.D);
        }
    }

    private void DrawLine()
    {
        _lineRenderer.positionCount = _linePositions.Count;
        _lineRenderer.SetPositions(_linePositions.ToArray());
    }
    
    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, _LocationToMove, speed * Time.deltaTime);
        if (transform.position == _LocationToMove)
        {
            Vector3 oldDirection = _direction;
            UpdateEnemiesLocations();
            UpdatePlayerDirections();
            if (_direction != oldDirection)
            {
                if((_landSpace.HasTile(Vector3Int.FloorToInt(transform.position)) && _direction == -oldDirection))
                {
                    _isMoving = false;
                    StartCoroutine("EnemeyHit");
                }
            }
            else if (!Physics2D.OverlapCircle(transform.position + _direction, OVERLAP_RADIUS, _limitsOfGrid))
            {
                _LocationToMove = transform.position + _direction;
            }
        }
    }
    
    private void UpdateEnemiesLocations()
    {
        _enemeiesPositions[0] = _enemies[0].transform.position;
        _enemeiesPositions[1] = _enemies[1].transform.position;
        _enemeiesPositions[2] = _enemies[2].transform.position;
    }

    private void FillAreas()
    {
        // Convert line positions to a list of unique, integer-rounded Vector3Int objects
        Vector3Int[] linePositionsToSwap = _linePositions
            .Select(Vector3Int.FloorToInt)
            .Distinct()
            .ToArray();
        // Set the tiles at these positions to be null in the land space and the sea tile in the sea space
        foreach (var pos in linePositionsToSwap)
        {
            _landSpace.SetTile(pos, null);
            _seaSpace.SetTile(pos, _sea);
            _seaSpace_2.SetTile(pos, null);
            _landSpace_2.SetTile(pos, _land);
            _enemyLand.SetTile(pos, null);
        }
        // Get a list of lists, where each list contains the positions of a group of land tiles that are connected
        List<List<Vector3Int>> landTileAreas = GetLandTileAreas();
        // Initialize an empty list to store the positions of the smallest land tile area found so far
        List<Vector3Int> smallestArea = new List<Vector3Int>();
        // Find the smallest land tile area that does not contain any enemies
        foreach (var landTileArea in landTileAreas)
        {
            if (!smallestArea.Any() || smallestArea.Count > landTileArea.Count)
            {
                // Check if any enemy is in this land tile area
                bool enemyExistInArea = _onlyInsideEnemies.Any(x =>
                {
                    // Get the current enemy's position as a Vector3Int
                    Vector3Int currentEnemyPosition = Vector3Int.FloorToInt(x.transform.position);

                    // Check if the current enemy's position is in the land tile area
                    return landTileArea.Any(tile => tile == currentEnemyPosition);
                });
                if (!enemyExistInArea)
                {
                    // If no enemy is in this land tile area, set it as the smallest land tile area found so far
                    smallestArea = landTileArea;
                }
            }
        }
        // If a smallest land tile area was found, set all tiles in it to be null in the land space and the sea tile in the sea space
        if (smallestArea.Any())
        {
            _landSpace.FloodFill(smallestArea.First(), null);
            _seaSpace.FloodFill(smallestArea.First(), _sea);
            foreach (var pos in smallestArea)
            {
                _seaSpace_2.SetTile(pos, null);
                _landSpace_2.SetTile(pos, _land);
                _enemyLand.SetTile(pos, null);
            }
        }
        // Compact the bounds of the land space
        _landSpace.CompressBounds();
    }
    
    private List<List<Vector3Int>> GetLandTileAreas()
    {   
        // Use a hash set to store visited tiles
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        // Pre-calculate the directions to check
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
        // Use a list to store the groups of connected tiles
        List<List<Vector3Int>> groups = new List<List<Vector3Int>>();
        // Iterate over all the tiles in the _landSpace tilemap
        foreach (var tile in _landSpace.cellBounds.allPositionsWithin)
        {
            // Create a new group for this connected area
            List<Vector3Int> group = new List<Vector3Int>();
            // Use a queue to store tiles that need to be visited
            Queue<Vector3Int> visit = new Queue<Vector3Int>();
            // Add the current tile to the queue if it hasn't been visited yet
            if (!visited.Contains(tile))
            {
                visit.Enqueue(tile);
                // Use a breadth-first search algorithm to visit all the connected tiles
                while (visit.Count > 0)
                {
                    Vector3Int currentTile = visit.Dequeue();
                    if (!visited.Contains(currentTile) &&_landSpace.HasTile(currentTile))
                    {
                        // Add the current tile to the group and mark it as visited
                        group.Add(currentTile);
                        visited.Add(currentTile);
                        // Add all the neighboring tiles to the queue if they haven't been visited yet
                        foreach (var direction in directions)
                        {
                            Vector3Int neighborTile = currentTile + direction;
                            if (!visited.Contains(neighborTile))
                            {
                                visit.Enqueue(neighborTile);
                            }
                        }
                    }
                }
            }
            // Add the group to the list if it contains any tiles
            if (group.Count > 0)
            {
                groups.Add(group);
            }
        }
        // Clear the visited tiles set
        visited.Clear();
        return groups;
    }

    private void setLineCollider()
    {
        if (!_linePositions.Any())
        {
            _edgeCollider2D.Reset();
            return;
        }
        var playerPos = transform.position;
        var vector2Points = _linePositions
            .Select(point => new Vector2(point.x - playerPos.x, point.y - playerPos.y))
            .ToList();
        if (vector2Points.Any())
            _edgeCollider2D.SetPoints(vector2Points);
        else
            _edgeCollider2D.SetPoints(new List<Vector2>());
    }

    private bool checkCollisions()
    {
        foreach (var enemy in _enemies)
        {
            var pos = enemy.transform.position;
            if (_linePositions.Contains(pos))
            {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy_1") ||
            col.gameObject.CompareTag("Enemy_2") ||
            col.gameObject.CompareTag("Enemy_3") ||
            col.gameObject.CompareTag("Enemy_4"))
        {
            StartCoroutine("EnemeyHit");
        }
        else if((col.gameObject.CompareTag("Player") && gameObject.CompareTag("Player_2")) ||
                (col.gameObject.CompareTag("Player_2") && gameObject.CompareTag("Player")))
        {
            PlayerHitPlayer = true;
        }
    }

    public IEnumerator EnemeyHit()
    {
        Time.timeScale=PAUSE_SCALE;
        yield return new WaitForSecondsRealtime(ENEMY_HIT_WAIT_TIME);
        _linePositions.Clear();
        DrawLine();
        ResetEdgeCollider();
        _isLerping = true;
        while (Vector3.Distance(transform.position, _startPos) > MIN_DISTANCE)
        {
            transform.position = Vector3.Lerp(transform.position, _startPos, LERP_SPEED*Time.unscaledDeltaTime);
            _linePositions.Clear();
            DrawLine();
            ResetEdgeCollider();
            yield return null;
        }
        transform.position = _startPos;
        _isLerping = false;
        _LocationToMove = _startPos;
        _isMoving = false;
        _isDrawing = false;
        _arrowUp = false;
        _arrowDown = false;
        _arrowLeft = false;
        _arrowRight = false;
        Time.timeScale = CONTINUE_SCALE;
    }
    
    private void ResetEdgeCollider()
    {
        _edgeCollider2D.points = new Vector2[] { Vector2.zero, Vector2.zero };
    }

    private void CheckCollisionsWithTrail()
    {
        if (_playerPrevoiusPositions.Count == 0)
        {
            _playerPrevoiusPositions.Add((FIRST_TRAIL_HIT , transform.position));
            return;
        }
        for (int i = 0; i < _playerPrevoiusPositions.Count; i++)
        {
            if (_playerPrevoiusPositions[i] == (FIRST_TRAIL_HIT, transform.position))
            {
                _playerPrevoiusPositions[i] = (SECOND_TRAIL_HIT, transform.position);
            }
            else if (_playerPrevoiusPositions[i] == (SECOND_TRAIL_HIT, transform.position) && Time.timeScale== CONTINUE_SCALE)
            {
                StartCoroutine("EnemeyHit");
            }
            else if (i == _playerPrevoiusPositions.Count - 1)
            {
                _playerPrevoiusPositions.Add((FIRST_TRAIL_HIT,transform.position));
            }
        }
    }
    
}

