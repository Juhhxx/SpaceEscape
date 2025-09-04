using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class CustomizeCharacter : MonoBehaviour
{
    [Header("Hair UI Elements")]
    public Image hairSelectionImage;    // UI image to preview/select hair
    public Image hairDisplayImage;      // Character's hair display image

    [Header("Clothing UI Elements")]
    public Image clothingSelectionImage; // UI image to preview/select clothing
    public Image clothingDisplayImage;   // Character's clothing display image

    [Header("Hair Options")]
    public Sprite[] hairOptions;        // Hair sprites (used for both selection and display)

    [Header("Clothing Options")]
    public Sprite[] clothingOptions;    // Clothing selection preview sprites
    public Sprite[] clothingNormal;     // Actual in-game clothing sprites (different visuals)

    private int currentHairIndex = 0;
    private int currentClothingIndex = 0;

    [SerializeField] private QuestionManager questionManager; // Reference to the QuestionManager for interaction

    [SerializeField] private TMP_Text nameInput;

    [SerializeField] private TMP_Text noNameInput;

    [SerializeField] private GameObject gameView;
    [SerializeField] private GameObject customizationView;

    [SerializeField] private MainMenuTransition gateTransition; // Reference to the gate transition script

    void Start()
    {
        UpdateHair();
        UpdateClothing();
    }

    private bool isStartingGame = false;

    public void StartGame()
    {
        if (isStartingGame) return; // Prevent starting if cooldown is active

        if (nameInput.text.Length <= 1)
        {
            // Pop animation using DOTween
            noNameInput.transform.localScale = Vector3.zero; // Start small
            noNameInput.gameObject.SetActive(true); // Make sure it's visible
            noNameInput.transform.DOScale(Vector3.one, 0.4f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    // Optional: fade out or scale down again after a delay
                    DOVirtual.DelayedCall(1.5f, () =>
                    {
                        noNameInput.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
                    });
                });

            return; // Prevent game from starting
        }

        isStartingGame = true; // Activate cooldown

        gateTransition.TransitionToMission();

        Debug.Log(nameInput.text + "count: " + nameInput.text.Length);

        // Valid name input, start the game
        questionManager.StartGame(
            hairOptions[currentHairIndex],
            clothingNormal[currentClothingIndex],
            nameInput.text
        );

        // Reset cooldown after 3 seconds
        DOVirtual.DelayedCall(3f, () => isStartingGame = false);
    }

    public void NextHair()
    {
        currentHairIndex = (currentHairIndex + 1) % hairOptions.Length;
        UpdateHair();
    }

    public void PreviousHair()
    {
        currentHairIndex = (currentHairIndex - 1 + hairOptions.Length) % hairOptions.Length;
        UpdateHair();
    }

    public void NextClothing()
    {
        currentClothingIndex = (currentClothingIndex + 1) % clothingOptions.Length;
        UpdateClothing();
    }

    public void PreviousClothing()
    {
        currentClothingIndex = (currentClothingIndex - 1 + clothingOptions.Length) % clothingOptions.Length;
        UpdateClothing();
    }

    private void UpdateHair()
    {
        Sprite selectedHair = hairOptions[currentHairIndex];
        hairSelectionImage.sprite = selectedHair;
        hairDisplayImage.sprite = selectedHair;
    }

    private void UpdateClothing()
    {
        Sprite selectedPreview = clothingOptions[currentClothingIndex];
        Sprite selectedNormal = clothingNormal[currentClothingIndex];

        clothingSelectionImage.sprite = selectedPreview;
        clothingDisplayImage.sprite = selectedNormal;
    }
}
