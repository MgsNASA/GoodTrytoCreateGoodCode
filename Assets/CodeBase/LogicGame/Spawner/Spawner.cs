using UnityEngine;

public class Spawner : MonoBehaviour, ISpawnerController, IStateClass
{
    private static Spawner _instance;  // ����������� ���������� ��� �������� ������������� ����������
    private ITetrominoFactory _tetrominoFactory;
    private IPositionValidator _positionValidator;
    private IRotationManager _rotationManager;
    private Vector3 lastSpawnPosition;
    public int maxSpawnShift = 4;

    // �������� ��� ������� � ���������� ���������
    public static Spawner Instance
    {
        get
        {
            if ( _instance == null )
            {
                Debug.LogError ( "Spawner instance �� ���������������! ���������, ��� ������ Spawner ���������� � �����." );
            }
            return _instance;
        }
    }

    private void Awake( )
    {
        // ���������, ���� �� ��� ���������
        if ( _instance != null && _instance != this )
        {
            Destroy ( gameObject );  // ������� ��������
            return;
        }

        _instance = this;  // ������������� ���������
        DontDestroyOnLoad ( gameObject );  // ��������� ������ ����� �������
    }

    // ����� ��� ������������� ����� �����������
    public void Initialize( ITetrominoFactory tetrominoFactory , IPositionValidator positionValidator , IRotationManager rotationManager )
    {
        _tetrominoFactory = tetrominoFactory;
        _positionValidator = positionValidator;
        _rotationManager = rotationManager;
    }

    public void NewTetromino( )
    {
        Vector3 spawnPosition;
        Quaternion randomRotation;

        for ( int i = 0; i < maxSpawnShift * 2; i++ )
        {
            spawnPosition = GenerateNewPosition ();
            randomRotation = _rotationManager.GenerateRandomRotation ();

            GameObject newTetromino = _tetrominoFactory.CreateRandomTetromino ( spawnPosition , randomRotation );

            if ( newTetromino == null )
            {
                Debug.LogError ( "�� ������� ������� ����� ���������!" );
                return;
            }

            // ��������� ����������� �������� (����)
            if ( _positionValidator.ValidMove ( newTetromino.transform , Vector3.down ) )
            {
                lastSpawnPosition = spawnPosition;
                return;
            }
            else
            {
                Destroy ( newTetromino );
            }
        }

        Debug.LogWarning ( "�� ������� ����� ���������� ������� ��� ������ ���������!" );
    }

    private Vector3 GenerateNewPosition( )
    {
        int randomShift;
        Vector3 spawnPosition;

        do
        {
            randomShift = Random.Range ( -maxSpawnShift , maxSpawnShift + 1 );
            spawnPosition = transform.position;
            spawnPosition.x += randomShift;
        }
        while ( spawnPosition == lastSpawnPosition );

        return spawnPosition;
    }

    public void MoveSpawnerUp( float step )
    {
        Vector3 spawnerPosition = transform.position;
        spawnerPosition.y += step; // ����������� Y ���������� ��������
        transform.position = spawnerPosition;
    }

    public void StartClass( )
    {
        lastSpawnPosition = Vector3.zero;  // ����� ��������� ������� ������
        Debug.Log ( "Spawner has started." );
    }

    public void Pause( )
    {
        // ������ ��� �����, ��������, ���������� ������
        Debug.Log ( "Spawner is paused." );
    }

    public void Resume( )
    {
        // ������ ��� �������������, ��������, ��������� ������
        Debug.Log ( "Spawner has resumed." );
    }

    public void Restart( )
    {
        lastSpawnPosition = Vector3.zero;  // ����� ��������� ������� ������
        Debug.Log ( "Spawner has restarted." );
    }
}
