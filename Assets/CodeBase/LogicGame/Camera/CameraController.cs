using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour, ICameraController, IStateClass
{
    [SerializeField] private float initialDistanceStep = 10.0f; // �������� ������ 10 ������
    [SerializeField] private float followSpeed = 2.0f;
    [SerializeField] private float distanceMultiplier = 20f;
    [SerializeField] private float shakeDuration = 0.5f; // ������������ ��������
    [SerializeField] private float shakeMagnitude = 0.2f; // ���� ��������

    private Spawner spawnerController;
    private Camera mainCamera;
    private float nextCameraY;
    private Vector3 initialCameraPosition; // ������ ��������� ������� ������
    private Coroutine shakeCoroutine; // ������ ������ �� �������� ��������
    private Coroutine moveCoroutine; // ������ ������ �� �������� �������� ������

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
        spawnerController.MoveSpawnerUp ( newThreshold );
        initialDistanceStep += distanceMultiplier;

        if ( moveCoroutine != null )
        {
            StopCoroutine ( moveCoroutine ); // ������������� ���������� ��������, ���� ��� ����������
        }
        moveCoroutine = StartCoroutine ( MoveCameraSmoothly () ); // ��������� ������� �������� ������

        // ������� ������������ � ���� �������� ��� ������ ������
        TriggerCameraShake ( shakeDuration , shakeMagnitude ); // ��������, ����������� �������� �� ���������, ������� ��� ������ � ������

    }

    private IEnumerator MoveCameraSmoothly( )
    {
        while ( Mathf.Abs ( mainCamera.transform.position.y - nextCameraY ) > 0.01f )
        {
            Vector3 newPosition = mainCamera.transform.position;
            newPosition.y = Mathf.Lerp ( mainCamera.transform.position.y , nextCameraY , followSpeed * Time.deltaTime );
            mainCamera.transform.position = newPosition;
            yield return null; // ���� ��������� ����
        }

        mainCamera.transform.position = new Vector3 ( mainCamera.transform.position.x , nextCameraY , mainCamera.transform.position.z );
        moveCoroutine = null;
    }

    public void TriggerCameraShake( float duration , float magnitude )
    {
        if ( shakeCoroutine != null )
        {
            StopCoroutine ( shakeCoroutine ); // ��������� ������� ��������, ���� ��� ��� ��������
        }
        shakeCoroutine = StartCoroutine ( CameraShakeCoroutine ( duration , magnitude ) ); // ������ ����� �������� � �����������
    }

    private IEnumerator CameraShakeCoroutine( float duration , float magnitude )
    {
        float elapsed = 0.0f;

        while ( elapsed < duration )
        {
            Vector3 shakeOffset = Random.insideUnitSphere * magnitude;
            shakeOffset.z = 0; // ��������� �������� ������ �� ���� X � Y
            mainCamera.transform.position = initialCameraPosition + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null; // ���� �� ���������� �����
        }

        mainCamera.transform.position = initialCameraPosition; // ������� ������ � �������� �������
        shakeCoroutine = null; // �������� ������ �� ��������
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
        if ( shakeCoroutine != null )
        {
            StopCoroutine ( shakeCoroutine ); // ��������� �������� ��� �����������
        }
        if ( moveCoroutine != null )
        {
            StopCoroutine ( moveCoroutine ); // ��������� �������� �������� ��� �����������
        }
        mainCamera.transform.position = initialCameraPosition; // ���������� ������ �� ��������� �������
        nextCameraY = initialCameraPosition.y; // ��������� ������� ������� ������
        initialDistanceStep = 10.0f; // ���������� ��� ��������
    }
}
