using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera Camera;
    public Transform Player; // ������ �� ������
    public Spawner Spawner; // ������ �� �������
    public float initialDistanceStep = 5.0f; // ��������� ����������, �� ������� ������ ���������� �����
    public float followThreshold = 10.0f; // ������, �� ������� ������ ������������
    public float followSpeed = 2.0f; // �������� �������� ����������� ������
    public float distanceMultiplier = 1.2f; // ��������� ��� ���������� ����������
    public float spawnerMoveStep = 10.0f; // ��� ����������� �������� �� ��� Y

    private float currentDistanceStep; // ������� ���������� ����������� ������
    private float nextCameraY; // ��������� ��������� ������ �� ��� Y

    private void Awake( )
    {
        Camera = Camera.main;
        currentDistanceStep = initialDistanceStep; // �������������� ��������� ����������
        nextCameraY = Camera.transform.position.y; // ��������� ��������� ������
    }

    private void LateUpdate( )
    {
        if ( Player != null )
        {
            // ���������, ������ �� ����� ������, ����� ������ ������ ���������
            if ( Player.position.y > nextCameraY + followThreshold )
            {
                // ����������� ��������� ������ �� ������������� ����������
                nextCameraY += currentDistanceStep;

                // ����������� ��� �� �������� ���������
                currentDistanceStep *= distanceMultiplier;

                // ���������� ������� ����� �� �������� ���������� ������ (��������, 10)
                MoveSpawnerUp ();
            }

            // ������ ���������� ������ � ����� ������
            Vector3 newPosition = Camera.transform.position;
            newPosition.y = Mathf.Lerp ( Camera.transform.position.y , nextCameraY , followSpeed * Time.deltaTime );

            Camera.transform.position = newPosition;
        }
    }

    // ����� ��� ����������� ��������
    private void MoveSpawnerUp( )
    {
        if ( Spawner != null )
        {
            Vector3 spawnerPosition = Spawner.transform.position;
            spawnerPosition.y += spawnerMoveStep; // ����������� Y ���������� �������� �� 10
            Spawner.transform.position = spawnerPosition;
        }
    }
}
