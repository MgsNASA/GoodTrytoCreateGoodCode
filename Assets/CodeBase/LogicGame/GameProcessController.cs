    using UnityEngine;

    public class GameProcessController : MonoBehaviour
    {
        public GameObject uiManagerPrefab;
        public GameObject spawnerPrefab;
        public GameObject tetrisGridManagerPrefab;

        private UiManager uiManager;
        private Spawner spawner;
        private TetrisGridManager tetrisGridManager;

    private void Awake( )
    {
        // ������� ���������� �� ��������
        uiManager = Instantiate ( uiManagerPrefab , transform ).GetComponent<UiManager> ();
        tetrisGridManager = Instantiate ( tetrisGridManagerPrefab ).GetComponent<TetrisGridManager> ();
        spawner = Instantiate ( spawnerPrefab ).GetComponent<Spawner> ();

        

        // ������������� �������� ����� �������
        var tetrominoFactory = AllServices.Container.Single<ITetrominoFactory> ();
        var positionValidator = AllServices.Container.Single<IPositionValidator> ();
        var rotationManager = AllServices.Container.Single<IRotationManager> ();

        // �������� ������������
        if ( tetrominoFactory == null || positionValidator == null || rotationManager == null )
        {
            Debug.LogError ( "���� ��� ��������� ������������ �� ����������������!" );
            return;
        }

        spawner.Initialize ( tetrominoFactory , positionValidator , rotationManager );
        GameStart ();
    }




    public void GameOver( )
        {
            Debug.Log ( "GameOver" );
            Time.timeScale = 0f; // ������������� ��� ��������
        }

        public void GameStart( )
        {
            Debug.Log ( "StartGame" );
            Time.timeScale = 1f; // ���������� ���������� �������� ������� ��� ������ ����
            spawner.NewTetromino ();
        }

        public void GamePause( )
        {
            Debug.Log ( "Pause" );
            Time.timeScale = 0f; // ������ ���� �� �����
        }

        public void GameResume( )
        {
            Debug.Log ( "Resume" );
            Time.timeScale = 1f; // ���������� ����� � ���������� ��� ��� ������ �����
        }
    }
