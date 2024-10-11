using UnityEngine;

public class CameraController : MonoBehaviour, ICameraController, IStateClass
{
    [SerializeField] private float initialDistanceStep = 10.0f; // �������� ������ 10 ������
    [SerializeField] private float followSpeed = 2.0f;
    [SerializeField] private float distanceMultiplier = 1.2f;

    private Spawner spawnerController;
    private Camera mainCamera;
    private float nextCameraY;
    private Vector3 initialCameraPosition; // ������ ��������� ������� ������

    private void Awake( )
    {
        mainCamera = Camera.main;
        initialCameraPosition = mainCamera.transform.position; // ��������� ��������� �������
        nextCameraY = initialCameraPosition.y;
        spawnerController = Spawner.Instance.GetComponent<Spawner> ();

        VerticalMovementTracker verticalMovementTracker = FindObjectOfType<VerticalMovementTracker> ();
        if ( verticalMovementTracker != null )
        {
            VerticalMovementTracker.OnThresholdReached += MoveCameraUp;
        }
        else
        {
            Debug.LogError ( "VerticalMovementTracker �� ������ � �����!" );
        }
    }

    private void OnDestroy( )
    {
        VerticalMovementTracker.OnThresholdReached -= MoveCameraUp;
    }

    private void MoveCameraUp( int newThreshold )
    {
        nextCameraY = newThreshold;
        spawnerController.MoveSpawnerUp ( initialDistanceStep );
        initialDistanceStep *= distanceMultiplier;
    }

    private void Update( )
    {
        Vector3 newPosition = mainCamera.transform.position;
        newPosition.y = Mathf.Lerp ( mainCamera.transform.position.y , nextCameraY , followSpeed * Time.deltaTime );
        mainCamera.transform.position = newPosition;
    }

    public void FollowTarget( Transform target )
    {
        // ����������, ���� �����������
    }

    public void StartClass( )
    {
        // �������������� �������������, ���� �����������
    }

    public void Pause( )
    {
        // ������ �����, ���� �����������
    }

    public void Resume( )
    {
        // ������ �������������, ���� �����������
    }

    public void Restart( )
    {
        mainCamera.transform.position = initialCameraPosition; // ���������� ������ �� ��������� �������
        nextCameraY = initialCameraPosition.y; // ��������� ������� ������� ������
        initialDistanceStep = 10.0f; // ���������� ��� ��������
    }
}
