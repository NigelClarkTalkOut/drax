using UnityEngine;

public class Box : MonoBehaviour
{
    public BoxType type;
    public bool isSelectable 
    {
        get
        {
            if (place == null)
            {
                place = GetComponentInParent<Place>();
            }
            return  place == null;
        }
    }
    public BoxStack parentStack;

    Place place = null;
    Animator animator;
    Vector3 initPos;
    BoxCollider2D boxCollider2D;
    Rigidbody2D rigidbody2D;
    public SpriteRenderer spriteRenderer;
    bool isFlashing = false;

    private void Start()
    {
        parentStack = GetComponentInParent<BoxStack>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initPos = transform.localPosition;

        place = GetComponentInParent<Place>();
        if(place != null)
            DisableCollider();
    }

    public void Select()
    {
        if(!isSelectable)
            return;

        if(!GameManager.instance.isSelectBeneathWarningDone)
        {
            if(OnTop())
            {
                GameManager.instance.MoveForkliftToBox(this);
            }
            else
            {
                GameManager.instance.ShowSelectBeneathWarning();
            }
        }
        else
        {
            GameManager.instance.MoveForkliftToBox(this);
        }
    }

    public bool OnTop() => parentStack.IsTop(this);

    public void StartFlash()
    {
        isFlashing = true;
        PlayFlashingAnimation();
    }

    public void StopFlash()
    {
        isFlashing = false;
        
        // if(parentStack != null)
        // {
        //     ResetToInitial();
        //     PlayIdleAnimation();
        //     gameObject.SetActive(false);
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }

        if(!GameManager.instance.isFirstImprovDone)
        {
            Destroy(gameObject);
        }
        else
        {
            GameManager.instance.AddToFlashedBoxList(this);
            PlayIdleAnimation();
            gameObject.SetActive(false);
        }
    }

    public void CancleFlashing()
    {
        isFlashing = false;
        PlayIdleAnimation();
    }

    public void ResetToInitial()
    {
        if(isFlashing)
        {
            CancleFlashing();
        }

        if(TryGetComponent<Rigidbody2D>(out rigidbody2D))
        {
            Destroy(rigidbody2D);
        }
        gameObject.layer = 6;
        spriteRenderer.sortingLayerName = "Box";

        if(parentStack != null)
        {
            transform.parent = parentStack.gameObject.transform;
            transform.localPosition = initPos;
            transform.localRotation = Quaternion.identity;
            EnableCollider();
            spriteRenderer.color = Color.white;
            gameObject.SetActive(true);
        }
        else if(place != null)
        {
            transform.parent = place.transform;
            transform.localPosition = initPos;
            transform.localRotation = Quaternion.identity;
            spriteRenderer.color = Color.white;
            gameObject.SetActive(true);
        }
        
    }

    // public void SetTypeToParent()
    // {
    //     if(type == parentStack.stackType)
    //         return;
        
    //     type = parentStack.stackType;
    //     spriteRenderer.sprite = GameManager.instance.GetSprite(type);
    // }

    public void DisableCollider() => boxCollider2D.enabled = false;

    public void EnableCollider() => boxCollider2D.enabled = true;

    public void PlayIdleAnimation() => animator.Play("Idle");
    public void PlayFlashingAnimation() => animator.Play("Flashing_Start");
}