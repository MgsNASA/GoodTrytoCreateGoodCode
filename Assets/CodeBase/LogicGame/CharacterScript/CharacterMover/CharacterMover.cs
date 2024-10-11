using UnityEngine;

public class CharacterMover : MonoBehaviour, ICharacterMover
{
    private float moveSpeed;
    [SerializeField]
    private float minX;
    [SerializeField]
    private float maxX;

    // ��������� ���������� ��� ����������� � ����
    public ParticleSystem dust;

    // ��� ���������� ������ ����� �������������� ��� �������� �����������
    public bool isFacingRight = true;

    // ���������� �������� ����������
    public bool IsFacingRight
    {
        get
        {
            return isFacingRight;
        }
    }

    public void OnCharacterDataChanged( CharacterStats stats )
    {
        moveSpeed = stats.moveSpeed;
        minX = stats.minX;
        maxX = stats.maxX;
    }

    public void Move( Vector3 direction , Transform transform )
    {
        Vector3 newPosition = transform.position + ( direction * moveSpeed * Time.deltaTime );
        newPosition.x = Mathf.Clamp ( newPosition.x , minX , maxX );
        transform.position = newPosition;
    }

    // ����� ��� ���������� ���������
    public void Flip( Transform transform )
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // ������ ��� ���������� �����
    public void PlayDust( )
    {
        if ( dust != null )
        {
            dust.Play ();
        }
    }

    public void StopDust( )
    {
        if ( dust != null )
        {
            dust.Stop ();
        }
    }
}
