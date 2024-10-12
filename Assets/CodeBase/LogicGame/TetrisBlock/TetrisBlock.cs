using System.Collections;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    public Vector3 RotationPoint
    {
        get; private set;
    }
    private float fallTime = 0; // ����� ����� ���������

    public event System.Action OnBlockStopped;
    public event System.Action OnNewTetrominoRequested;

    private IBlockMover blockMover;
    private IBlockRotator blockRotator;

    // �����������, ������� ��������� �����������


    void Awake( )
    {
        ITetrisGridManager gridManager = FindObjectOfType<TetrisGridManager> ();

        blockMover = new BlockMover ( gridManager );
        blockRotator = new BlockRotator ( gridManager );

    }

    void Start( )
    {
        CalculateRotationPoint ();
        StartCoroutine ( FallRoutine () ); // ������ �������� ��� �������
    }


    private IEnumerator FallRoutine( )
    {
        while ( true )
        {
            yield return new WaitForSeconds ( fallTime ); // �������� ����� ��������� ��������

            // �������� ������������ �������� ����� ������������
            if ( blockMover.ValidMove ( transform , Vector3.down ) )
            {
                // ���� �������� ���������, ���������� ���� ����
                yield return StartCoroutine ( blockMover.Move ( Vector3.down , transform ) );
            }
            else
            {
                // ���� �������� �����������, ���� ���������������
                BlockStopped ();

                // ��������� ���� � ����� ����� ���������
                ITetrisGridManager gridManager = FindObjectOfType<TetrisGridManager> ();
                gridManager.AddToGrid ( gameObject );

                Spawner.Instance.NewTetromino ();
                yield break; // ������������� �������
            }
        }
    }




    private void BlockStopped( )
    {
        OnBlockStopped?.Invoke ();
        OnNewTetrominoRequested?.Invoke (); // ������ �� �������� ������ ���������
        this.enabled = false; // ��������� ��������� ����� ���������
    }

    private void CalculateRotationPoint( )
    {
        Vector3 totalPosition = Vector3.zero;
        int childCount = 0;

        foreach ( Transform child in transform )
        {
            totalPosition += child.localPosition;
            childCount++;
        }

        if ( childCount > 0 )
        {
            RotationPoint = totalPosition / childCount;
        }
    }
}
