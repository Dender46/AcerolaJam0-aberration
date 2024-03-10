using System;
using TMPro;
using UnityEngine;

public class LevelFinishedScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _dayTextUI;

    public event EventHandler onScreenIsFinished;

    public static LevelFinishedScreen instance { private set; get; }

    private void Awake()
    {
        gameObject.SetActive(false);
        instance = this;
    }

    [ContextMenu("Show")]
    public void Show()
    {
        _dayTextUI.text = "Day " + (LevelManager.instance.currentLevel + 1);
        gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("Show");
    }

    private void __EventFinished()
    {
        gameObject.SetActive(false);
        onScreenIsFinished?.Invoke(this, new EventArgs());
    }

}
