// Assets/Scripts/ChoiceUI.cs
using System;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private Text[] optionLabels;
    private Action<int> onPick;

    public void Open(string[] labels, Action<int> onPick)
    {
        this.onPick = onPick;
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int idx = i;
            bool active = labels != null && i < labels.Length;
            optionButtons[i].gameObject.SetActive(active);
            if (!active) continue;

            if (optionLabels != null && i < optionLabels.Length && optionLabels[i] != null)
                optionLabels[i].text = labels[i];

            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() =>
            {
                this.onPick?.Invoke(idx);
                Destroy(gameObject);
            });
        }
        gameObject.SetActive(true);
    }
}
