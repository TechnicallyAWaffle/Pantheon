using UnityEngine;

public class InputManager : MonoBehaviour
{
    public AK.Wwise.Event myWwiseEvent;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (myWwiseEvent == null)
                return;
            myWwiseEvent.Post(gameObject);
        }
    }
}
