using System.Collections.Generic;
using UnityEngine;

public class GameProcessController : MonoBehaviour, IStateClass
{
    public GameObject uiManagerPrefab;
    public GameObject spawnerPrefab;
    public GameObject tetrisGridManagerPrefab;
    public GameObject cameraControllerPrefab;

    private UiManager _uiManager;
    private Spawner _spawner;
    private TetrisGridManager _tetrisGridManager;
    private CameraController _cameraController;

    // ������ �������, ����������� ��������� IStateClass
    private List<IStateClass> stateClasses = new List<IStateClass> ();

    public void StartGame( )
    {
        // ������� ���������� �� ��������
        _uiManager = Instantiate ( uiManagerPrefab , transform ).GetComponent<UiManager> ();
        _tetrisGridManager = Instantiate ( tetrisGridManagerPrefab ).GetComponent<TetrisGridManager> ();
        _spawner = Instantiate ( spawnerPrefab ).GetComponent<Spawner> ();
        _cameraController = Instantiate ( cameraControllerPrefab ).GetComponent<CameraController> ();

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

        _spawner.Initialize ( tetrominoFactory , positionValidator , rotationManager );

        // ��������� ������ � ������
   
        stateClasses.Add ( _uiManager ); // ��������� UI Manager (���� �� ��������� IStateClass)
        stateClasses.Add ( _tetrisGridManager ); // ��������� Tetris Grid Manager (���� �� ��������� IStateClass)
        stateClasses.Add ( _spawner ); // ��������� Spawner (���� �� ��������� IStateClass)
        stateClasses.Add ( _cameraController ); // ��������� Camera Controller (���� �� ��������� IStateClass)
        _uiManager.ShowPanel ( GamePanel.StartPanel );
        // StartClass ();
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

        foreach ( var stateClass in stateClasses )
        {
            stateClass.StartClass (); // ����� StartClass ��� ���� �������
        }

        _uiManager.HideAllAndReset ();
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

        _uiManager.HideAllAndReset ();
        _uiManager.ShowPanel ( GamePanel.GameHudPanel );
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
            _uiManager.HideAllAndReset ();
            _uiManager.ShowPanel ( GamePanel.GameHudPanel );
        }

        // ������� ������ ������� � ������������� ����
        Destroy ( _spawner.gameObject );
        Destroy ( _tetrisGridManager.gameObject );
        Destroy ( _cameraController.gameObject );
        Destroy ( _uiManager.gameObject );

        StartGame (); // ������������� ����
    }
}
