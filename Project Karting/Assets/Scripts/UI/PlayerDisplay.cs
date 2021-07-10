using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI joinText;
    [SerializeField] private string joinMessage;
    [SerializeField] private string addPlayerMessage;
    [SerializeField] private Image playerSelectionMarker;
    [SerializeField] private float markerMoveSpeed;
    [SerializeField] public Transform displayParent;

    private float markerYPosition;
    private void Awake()
    {
        playerSelectionMarker.enabled = false;
    }

    private void FixedUpdate()
    {
        if (playerSelectionMarker.enabled)
        {
            Vector2 pos = playerSelectionMarker.rectTransform.anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, markerYPosition,
                markerMoveSpeed * Time.fixedDeltaTime);
            playerSelectionMarker.rectTransform.anchoredPosition = pos;
        }
    }

    public void ShowJoinMessage()
    {
        joinText.enabled = true;
        joinText.text = joinMessage;
    }
    public void HideMessage()
    {
        joinText.enabled = false;
    }
    public void HideMarker()
    {
        joinText.enabled = false;
        playerSelectionMarker.enabled = false;
    }
    public void ShowAddPlayerMessage()
    {
        joinText.enabled = true;
        joinText.text = addPlayerMessage;
    }

    public void ShowSelectionMarker(int player)
    {
        playerSelectionMarker.enabled = true;
        SelectPlayer(player);
    }

    public void SelectPlayer(int player)
    {
        markerYPosition = GetXPositionOfMarker(player);
        Vector3 pos = playerSelectionMarker.rectTransform.anchoredPosition3D;
        pos.x = GetXPositionOfMarker(player);
        playerSelectionMarker.rectTransform.DOAnchorPos(pos, markerMoveSpeed, true).SetEase(Ease.InOutCubic);
    }

    private float GetXPositionOfMarker(int player)
    {
        return player*114 -171;
    }

    private void MoveMarker()
    {
        Vector3 pos = playerSelectionMarker.rectTransform.anchoredPosition3D;
    }
}
