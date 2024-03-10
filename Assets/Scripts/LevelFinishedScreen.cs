using System;
using System.Collections;
using UnityEngine;

public class LevelFinishedScreen : MonoBehaviour
{
    [SerializeField] private float _showForSeconds = 2.0f;

    public event EventHandler onScreenIsShown;

    public static LevelFinishedScreen instance { private set; get; }

    private void Awake()
    {
        gameObject.SetActive(false);
        instance = this;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        yield return new WaitForSeconds(_showForSeconds);
        gameObject.SetActive(false);
        onScreenIsShown?.Invoke(this, new EventArgs());
    }

}
