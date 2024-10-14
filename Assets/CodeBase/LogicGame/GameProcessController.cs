using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameProcessController : MonoBehaviour, IStateClass
{
    public GameObject uiManagerPrefab;
    public GameObject spawnerPrefab;
    public GameObject tetrisGridManagerPrefab;
    public GameObject cameraControllerPrefab;
    public GameObject player;
    public GameObject musicSerivcePrefab;
    private CharacterController _characterController;
    public UiManager _uiManager;
    public Spawner _spawner;
    public TetrisGridManager _tetrisGridManager;
    public CameraController _cameraController;
    public MusicService _musicService;

    // ������ �������, ����������� ��������� IStateClass
    private List<IStateClass> stateClasses = new List<IStateClass> ();

    public void StartGame( )
    {
        player = AllServices.Container.Single<IGameFactory> ().CreateObject ( "Prefab/Player/Player" );
        _characterController = player.GetComponent<CharacterController> ();
        // ������� ���������� �� ��������
        _uiManager = Instantiate ( uiManagerPrefab , transform ).GetComponent<UiManager> ();
        _uiManager.Initialize (player,this);
        _tetrisGridManager = Instantiate ( tetrisGridManagerPrefab ).GetComponent<TetrisGridManager> ();
        _spawner = Instantiate ( spawnerPrefab ).GetComponent<Spawner> ();
        _musicService = Instantiate ( musicSerivcePrefab ).GetComponent<MusicService> ();
        _cameraController = Instantiate ( cameraControllerPrefab ).GetComponent<CameraController> ();
        _cameraController.Initialize ( _uiManager.GetComponent<VerticalMovementTracker>());
        // ������������� �������� ����� �������
        var tetrominoFactory = AllServices.Container.Single<ITetrominoFactory> ();
        var positionValidator = AllServices.Container.Single<IPositionValidator> ();
        var rotationManager = AllServices.Container.Single<IRotationManager> ();
        _spawner.Initialize ( tetrominoFactory , positionValidator , rotationManager );
        stateClasses.Add ( _uiManager ); // ��������� UI Manager (���� �� ��������� IStateClass)
        stateClasses.Add ( _tetrisGridManager ); // ��������� Tetris Grid Manager (���� �� ��������� IStateClass)
        stateClasses.Add ( _spawner ); // ��������� Spawner (���� �� ��������� IStateClass)
        stateClasses.Add ( _cameraController ); // ��������� Camera Controller (���� �� ��������� IStateClass)
        _musicService.currentTrackIndex = 0;
        _musicService.PlayMusic ();
        _spawner.Restart ();

   
    }

    public void GameOver( )
    {
        Debug.Log ( "GameOver" );
        Time.timeScale = 0f; // ������������� ��� ��������
        _uiManager.ShowPanel ( GamePanel.EndPanel ); // ���������� ������ ���������� ����
    }

    public void StartClass( )
    {
        Debug.Log ( "StartGame" );
        Time.timeScale = 1f; // ���������� ���������� �������� ������� ��� ������ ����

        // �������� ������ ���� (������ 0)
        _musicService.currentTrackIndex = 1;
        _musicService.PlayMusic ();

        foreach ( var stateClass in stateClasses )
        {
            stateClass.StartClass (); // ����� StartClass ��� ���� �������
        }

        _uiManager.HideAllPanels ();
        _uiManager.ShowPanel ( GamePanel.GameHudPanel );
        _spawner.NewTetromino ();
    }

    public void Pause( )
    {
        Debug.Log ( "Pause" );
        foreach ( var stateClass in stateClasses )
        {
            stateClass.Pause (); // ����� Pause ��� ���� �������
        }

        _uiManager.ShowPanel ( GamePanel.PausePanel );
        Time.timeScale = 0f; // ������ ���� �� �����
    }

    public void Resume( )
    {
        Debug.Log ( "Resume" );
        foreach ( var stateClass in stateClasses )
        {
            stateClass.Resume (); // ����� Resume ��� ���� �������
        }

        _uiManager.HideAllPanels ();
        _uiManager.ShowPanel ( GamePanel.GameHudPanel );

        // �������� ������ ���� (������ 1) ��� ������������� ����
        _musicService.currentTrackIndex = 1;
        _musicService.PlayMusic ();

        Time.timeScale = 1f; // ���������� ����� � ���������� ��� ��� ������ �����
    }

    public void Restart( )
    {
        foreach ( var stateClass in stateClasses )
        {
            stateClass.Restart (); // ����� Restart ��� ���� �������
        }
        // ����� ����������� ����
        Debug.Log ( "Restarting Game" );
        Time.timeScale = 0f;

        // ���������� ��������� �����
        if ( _tetrisGridManager != null )
        {
            _tetrisGridManager.Restart (); // ���������� �����
        }

        // ������������� UI
        if ( _uiManager != null )
        {
            _uiManager.HideAllPanels ();
            _uiManager.ShowPanel ( GamePanel.GameHudPanel );
        }
        Time.timeScale = 1f; // ���������� ����� � ���������� ��� ��� ������ �����
        Destroy (_spawner);
        Destroy ( _tetrisGridManager.gameObject );
        Destroy ( _cameraController.gameObject );
        Destroy (_uiManager.gameObject );
        Destroy ( player );
        Destroy(_musicService.gameObject );
        StartGame ();
    }
}
