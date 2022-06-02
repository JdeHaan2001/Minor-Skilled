using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    [SerializeField] private Text playerNameText;

    [SerializeField] private Slider playerHealthSlider;

    private PlayerManager target;

    private float characterControllerHeight = 0f;
    private Transform targetTransform;
    private Renderer targetRenderer;
    private CanvasGroup _canvasGroup;
    private Vector3 targetPosition;

    private void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        _canvasGroup = this.GetComponent<CanvasGroup>();
    }

    public void SetTarget(PlayerManager pTarget)
    {
        if (pTarget == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        target = pTarget;
        if (playerNameText != null)
            playerNameText.text = target.photonView.Owner.NickName;

        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();
        CharacterController characterController = pTarget.GetComponent<CharacterController>();
        if (characterController != null)
            characterControllerHeight = characterController.height;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        if (playerHealthSlider != null)
            playerHealthSlider.value = target.Health;
    }

    private void LateUpdate()
    {
        if (targetRenderer != null)
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;

        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }
}
