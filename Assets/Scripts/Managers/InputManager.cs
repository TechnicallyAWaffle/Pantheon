using UnityEngine;

public class InputManager : MonoBehaviour
{
    public AK.Wwise.Event myWwiseEvent;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (myWwiseEvent == null)
                return;
            myWwiseEvent.Post(gameObject);
        }
    }
}
