using UnityEngine;

public class DestoryOnClick : MonoBehaviour
{
    [SerializeField] GameObject objToDestory;
    [SerializeField] bool objectTo = false;

    // button will destory the game object or the object selected
    public void __DestroyButton ()
    {
        // if the refrance for the object is null then just destroy the game object the script is resting on 
        if (objToDestory == null)
        {
            Destroy(gameObject);
            return;
        }
        else if (objToDestory != null)
        {
            Destroy(objToDestory);
            if (objectTo)
                Destroy(gameObject);
        }
    }
}