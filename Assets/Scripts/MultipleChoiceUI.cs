using UnityEngine;
using UnityEngine.UI;
using System;

public class MultipleChoiceUI : MonoBehaviour
{
    public GameObject buttonPrefab;

    private Transform container;

    void Awake()
    {
        // Create the container to hold all the buttons
        GameObject vertical = new GameObject("VerticalLayout", typeof(RectTransform));
        vertical.transform.SetParent(transform, false);

        RectTransform rect = vertical.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        container = vertical.transform;
    }

    public void SetupOptions(Option[] options, Action<string> onOptionSelected)
    {
        // Clear previous children
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        int total = options.Length;

        // Determine how many options will go on the top row
        int topCount = Mathf.CeilToInt(total / 2f);
        int bottomCount = total - topCount;

        // Handle the different configurations based on the number of options
        if (total == 3)
        {
            // For 3 options: single row
            CreateRow(options, 0, total, onOptionSelected, CalculateVerticalSpacing());
        }
        else if (total == 5)
        {
            // For 5 options: top row has 3, bottom row has 2 with alignment
            CreateRow(options, 0, 3, onOptionSelected, CalculateVerticalSpacing());
            float offset = (topCount - bottomCount) * 0.5f;
            CreateRow(options, 3, bottomCount, onOptionSelected, -CalculateVerticalSpacing());
        }
        else if (total % 2 == 1)
        {
            // For odd numbers greater than 5 (7, 9, 11, etc.): split into 2 rows
            CreateRow(options, 0, topCount, onOptionSelected, CalculateVerticalSpacing());
            CreateRow(options, topCount, bottomCount, onOptionSelected, -CalculateVerticalSpacing());
        }
        else
        {
            // For even numbers greater than 3 (6, 8, 10, etc.): split into 2 rows
            CreateRow(options, 0, topCount, onOptionSelected, CalculateVerticalSpacing());
            CreateRow(options, topCount, bottomCount, onOptionSelected, -CalculateVerticalSpacing());
        }
    }

    private void CreateRow(Option[] options, int startIndex, int count, Action<string> onOptionSelected, float yOffset)
    {
        // Calculate button width and spacing
        float buttonWidth = buttonPrefab.GetComponent<RectTransform>().rect.width;
        float buttonSpacing = 10f; // Space between buttons

        // Calculate the total width of all buttons in the row
        float rowWidth = count * (buttonWidth + buttonSpacing) - buttonSpacing;

        // Calculate the starting position (centered horizontally)
        float startPosX = (container.GetComponent<RectTransform>().rect.width - rowWidth) * 0.5f;

        // Create a new row container
        GameObject row = new GameObject("Row", typeof(RectTransform));
        row.transform.SetParent(container, false);

        // Set the vertical offset for the row (calculated spacing)
        RectTransform rowRect = row.GetComponent<RectTransform>();
        rowRect.anchoredPosition = new Vector2(0, yOffset);

        // Loop through and create each button
        for (int i = startIndex; i < startIndex + count; i++)
        {
            Option opt = options[i];

            // Create the button for this option
            GameObject buttonObj = Instantiate(buttonPrefab, row.transform);
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();

            // Position the button correctly in the row (horizontal position)
            float posX = startPosX + (buttonWidth + buttonSpacing) * (i - startIndex);
            buttonRect.anchoredPosition = new Vector2(posX, 0f); // Keep Y at 0 for horizontal positioning

            // Set the button image and text
            Button button = buttonObj.GetComponent<Button>();
            var image = buttonObj.GetComponentInChildren<Image>();
            var text = buttonObj.GetComponentInChildren<TMPro.TMP_Text>();

            if (image) image.sprite = opt.image;
            if (text) text.text = opt.text;

            // Capture answer text and set listener to call the provided action
            string captured = opt.text;
            button.onClick.AddListener(() => onOptionSelected(captured));
        }
    }

    private float CalculateVerticalSpacing()
    {
        // Get the height of the button and the spacing value
        float buttonHeight = buttonPrefab.GetComponent<RectTransform>().rect.height / 2;
        float buttonSpacing = 10f;

        // The vertical spacing between rows
        return buttonHeight + buttonSpacing;
    }
}
