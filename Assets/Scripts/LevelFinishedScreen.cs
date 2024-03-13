using System;
using TMPro;
using UnityEngine;

public class LevelFinishedScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _dayTextUI;
    [SerializeField] private TMP_Text _descriptionTextUI;
    [SerializeField] private float _speed = 1.0f;

    public event EventHandler onScreenIsFinished;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    [ContextMenu("Show")]
    public void Show(string titleText, string description = "")
    {
        _dayTextUI.text = titleText;
        if (description != "")
        {
            _descriptionTextUI.text = description;
        }
        gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("Show");
        GetComponent<Animator>().speed = _speed;
    }

    private void __EventFinished()
    {
        gameObject.SetActive(false);
        onScreenIsFinished?.Invoke(this, new EventArgs());
    }

}
