
using UnityEngine;

public class GameHudPanel : UIPanel
{
    public override void Show( )
    {
        gameObject.SetActive ( true );
        button.onClick.AddListener ( OnStartButtonClicked );
    }

    public override void Hide( )
    {
        gameObject.SetActive ( false );
        button.onClick.RemoveListener ( OnStartButtonClicked );
    }

    private void OnStartButtonClicked( )
    {
        // ����� �� �������� ����� StartGame � GameProcessController
        FindObjectOfType<GameProcessController> ().Pause ();
    }
}
