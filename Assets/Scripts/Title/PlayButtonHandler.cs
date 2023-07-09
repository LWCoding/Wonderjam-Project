using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonHandler : MonoBehaviour
{

    public Sprite unclickedSprite;
    public Sprite clickedSprite;
    private float _initialScale;
    private float _desiredScale;
    private bool _isMouseOver = false;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _initialScale = transform.localScale.x;
        _desiredScale = _initialScale;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnMouseEnter()
    {
        _desiredScale = _initialScale + 1.2f;
        _isMouseOver = true;
    }

    public void OnMouseExit()
    {
        _desiredScale = _initialScale;
        _isMouseOver = false;
    }

    public void OnMouseDown()
    {
        if (!TitleController.Instance.canPlayerInteract) { return; }
        _spriteRenderer.sprite = clickedSprite;
    }

    public void OnMouseUp()
    {
        if (!TitleController.Instance.canPlayerInteract) { return; }
        _spriteRenderer.sprite = unclickedSprite;
        if (_isMouseOver)
        {
            TitleController.Instance.canPlayerInteract = false;
            OverlayManager.Instance.ShowScreenOverlay(1, 1.4f, () =>
            {
                SceneManager.LoadScene("Game");
            });
        }
    }

    public void FixedUpdate()
    {
        float difference = Mathf.Abs(transform.localScale.x - _desiredScale);
        if (difference > 0.11f)
        {
            if (gameObject.transform.localScale.x > _desiredScale)
            {
                if (difference < 0.4f)
                {
                    gameObject.transform.localScale -= new Vector3(0.1f, 0.1f, 0);
                }
                else
                {
                    gameObject.transform.localScale -= new Vector3(0.3f, 0.3f, 0);
                }
            }
            else
            {
                if (difference < 0.4f)
                {
                    gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0);
                }
                else
                {
                    gameObject.transform.localScale += new Vector3(0.3f, 0.3f, 0);
                }
            }
        }
    }

}
