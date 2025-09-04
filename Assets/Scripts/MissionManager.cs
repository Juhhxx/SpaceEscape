using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class MissionManager : MonoBehaviour
{
    [SerializeField] private int numberOfPlayers = 4;
    [SerializeField] private GameObject playerPrefab;
    private List<GameObject> players = new List<GameObject>();
    [SerializeField] private float spacingBetweenPlayers = 3f;
    [SerializeField] private string[] names;
    private List<string> namesFromInput;

    public GameObject trianglePrefab;
    public int triangleCount = 8;

    [SerializeField] private Transform centerOfTriangles;

    [SerializeField] private Mission[] missions;

    [SerializeField] private Transform trianglesParent;

    [SerializeField] private float radiusAdjustmentFactor = 0.8f;

    private List<MissionSelection> triangleMissions = new List<MissionSelection>();


    [Header("Mission Selection")]
    [SerializeField] private GameObject selectAMissionUI;
    [Header("Mission unlocked")]
    [SerializeField] private GameObject missionUnlockedUI;
    [SerializeField] private TextMeshProUGUI missionTitle;
    [SerializeField] private TextMeshProUGUI missionDescription;
    [SerializeField] private Button buttonToStartMission;
    [Header("Mission locked")]
    [SerializeField] private GameObject missionLockedUI;


    [SerializeField] private GameObject gradient;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject gameSetupObject;
    [SerializeField] private GameObject nameAddedToTripulation;

    private Sequence popSeq;

    //[SerializeField] private game
    private GameObject currentOpenUI = null;

    void Start()
    {
        namesFromInput = new List<string>();

        Screen.orientation = ScreenOrientation.Portrait;
        //InstanciatePlayers();
    }

    public void AddName()
    {
        string name = inputField.text;
        if (!string.IsNullOrEmpty(name))
        {
            namesFromInput.Add(name);
            Debug.Log(name);
            inputField.text = ""; // Clear the input field
            PopUpAndDownUI(nameAddedToTripulation.GetComponent<RectTransform>(), nameAddedToTripulation.GetComponent<TextMeshProUGUI>(), $"{name} juntou-se à tripulação!");
        }

        if(namesFromInput.Count >= 4)
        {
            StartTheGame();
        }
    }


    public void PopUpAndDownUI(
        RectTransform targetUI,
        TextMeshProUGUI targetText,
        string message,
        float popScale = 1f,
        float popDuration = 0.5f,
        System.Action onComplete = null)
    {
        if (targetUI == null) return;

        if (targetText != null)
            targetText.text = message;

        targetUI.localScale = Vector3.zero;

        popSeq.Kill();

        targetUI.gameObject.SetActive(true);
        popSeq = DOTween.Sequence();
        popSeq.Append(targetUI.DOScale(Vector2.one, popDuration * 0.5f).SetEase(Ease.OutBack))
            .AppendInterval(4f)
              .Append(targetUI.DOScale(Vector2.zero, popDuration * 0.5f).SetEase(Ease.InOutBack))
              .OnComplete(() =>
              {
                  targetUI.gameObject.SetActive(false);
                  onComplete?.Invoke();
              });
    }

    public void StartTheGame()
    {
        if (namesFromInput.Count <= 0)
            return;

        Screen.orientation = ScreenOrientation.LandscapeLeft;
        numberOfPlayers = namesFromInput.Count;
        GradientTransformation();
    }

    private void GradientTransformation()
    {
        // Set initial state
        gradient.transform.localScale = new Vector3(1f, 1f, 1f);
        gradient.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        gameSetupObject.transform.DOScale(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            gameSetupObject.SetActive(false);
        });

        // Scale up while rotating
        gradient.transform.DOScale(new Vector3(2f, 2f, 2f), 1f).SetEase(Ease.InOutSine);
        gradient.transform.DORotate(new Vector3(0f, 0f, 0f), 1f, RotateMode.Fast)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // Scale down after rotation finishes
                gradient.transform.DOScale(new Vector3(1f, 0.5f, 1f), 1f).SetEase(Ease.InOutSine);
                InstanciatePlayers();
            });
    }

    public void ToggleSelectMissionUI(bool show)
    {
        if (show)
        {
            CloseCurrentUI(() =>
            {
                selectAMissionUI.SetActive(true);
                AnimateUIIn(selectAMissionUI);
                currentOpenUI = selectAMissionUI;
            });
        }
        else if (selectAMissionUI.activeSelf)
        {
            AnimateUIOut(selectAMissionUI, () =>
            {
                selectAMissionUI.SetActive(false);
                if (currentOpenUI == selectAMissionUI)
                    currentOpenUI = null;
            });
        }
    }

    public void ShowUnlockedMissionUI(string title, string description, UnityEngine.Events.UnityAction startCallback)
    {
        CloseCurrentUI(() =>
        {
            missionUnlockedUI.SetActive(true);

            missionTitle.text = title;
            missionDescription.text = description;

            buttonToStartMission.onClick.RemoveAllListeners();
            buttonToStartMission.onClick.AddListener(startCallback);

            AnimateUIIn(missionUnlockedUI);
            currentOpenUI = missionUnlockedUI;
        });
    }

    public void ShowLockedMissionUI()
    {
        CloseCurrentUI(() =>
        {
            missionLockedUI.SetActive(true);
            AnimateUIIn(missionLockedUI);
            currentOpenUI = missionLockedUI;
        });
    }

    private void CloseCurrentUI(System.Action onComplete)
    {
        if (currentOpenUI != null && currentOpenUI.activeSelf)
        {
            AnimateUIOut(currentOpenUI, () =>
            {
                currentOpenUI.SetActive(false);
                currentOpenUI = null;
                onComplete?.Invoke();
            });
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    public void CloseAllUIs(System.Action onComplete = null)
    {
        // If one is open, close it and then call onComplete
        if (currentOpenUI != null && currentOpenUI.activeSelf)
        {
            AnimateUIOut(currentOpenUI, () =>
            {
                currentOpenUI.SetActive(false);
                currentOpenUI = null;
                onComplete?.Invoke();
            });
        }
        else
        {
            // Nothing open, just invoke the callback
            onComplete?.Invoke();
        }
    }


    private Dictionary<GameObject, Vector2> originalPositions = new Dictionary<GameObject, Vector2>();

    private void AnimateUIIn(GameObject ui)
    {
        CanvasGroup group = GetOrAddCanvasGroup(ui);
        RectTransform rect = ui.GetComponent<RectTransform>();

        if (!originalPositions.ContainsKey(ui))
            originalPositions[ui] = rect.anchoredPosition; // Cache original anchored position

        Vector2 originalPos = originalPositions[ui];

        group.alpha = 0;
        rect.localScale = Vector3.one * 0.8f;
        rect.anchoredPosition = originalPos + Vector2.down * 100f;

        DOTween.Kill(ui);

        DOTween.Sequence()
            .Append(rect.DOAnchorPos(originalPos, 0.4f).SetEase(Ease.OutExpo))
            .Join(group.DOFade(1f, 0.3f))
            .Join(rect.DOScale(1f, 0.4f).SetEase(Ease.OutBack))
            .SetTarget(ui);
    }

    private void AnimateUIOut(GameObject ui, System.Action onComplete)
    {
        CanvasGroup group = GetOrAddCanvasGroup(ui);
        RectTransform rect = ui.GetComponent<RectTransform>();

        Vector2 originalPos = originalPositions.ContainsKey(ui) ? originalPositions[ui] : rect.anchoredPosition;

        DOTween.Kill(ui);

        DOTween.Sequence()
            .Append(rect.DOAnchorPos(originalPos + Vector2.down * 100f, 0.3f).SetEase(Ease.InBack))
            .Join(group.DOFade(0f, 0.3f))
            .Join(rect.DOScale(0.8f, 0.3f).SetEase(Ease.InBack))
            .SetTarget(ui)
            .OnComplete(() => onComplete?.Invoke());
    }

    private CanvasGroup GetOrAddCanvasGroup(GameObject go)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = go.AddComponent<CanvasGroup>();
        return cg;
    }



    void SpawnTriangles(Vector2 centerPosition = default)
    {
        if (trianglePrefab == null)
        {
            Debug.LogError("No triangle prefab assigned!");
            return;
        }

        // If no center position is provided, use Vector2.zero (0,0)
        if (centerPosition == default)
        {
            centerPosition = Vector2.zero;
        }

        // Get the scale of the triangle prefab
        float triangleScale = trianglePrefab.transform.localScale.x;

        // Base length should account for the scale
        float triangleBase = 1f * triangleScale;
        float angleStep = 360f / triangleCount;

        // Calculate radius based on the actual size of triangles
        float radius = (triangleBase / 2) / Mathf.Sin(Mathf.PI / triangleCount);

        // Reduce the radius to bring triangles closer to the center
        radius *= radiusAdjustmentFactor;

        // Clear the list before adding new triangles
        triangleMissions.Clear();

        // Store the original prefab scale to preserve it
        Vector3 originalScale = trianglePrefab.transform.localScale;

        // Spawn triangles clockwise instead of counter-clockwise
        for (int i = 0; i < triangleCount; i++)
        {
            // Use negative angle for clockwise direction
            float angle = -i * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            float xPos = centerPosition.x + Mathf.Cos(radians) * radius;
            float yPos = centerPosition.y + Mathf.Sin(radians) * radius;
            Vector2 spawnPosition = new Vector2(xPos, yPos);

            GameObject triangle = Instantiate(trianglePrefab, trianglesParent);
            triangle.GetComponent<MissionSelection>().InitializeStartPosition(spawnPosition);

            // Set initial position to the center point (not zero)
            triangle.transform.position = centerPosition;

            // Start with zero scale for animation, but remember the original scale
            triangle.transform.localScale = Vector3.zero;

            // Set final rotation
            float rotationAngle = angle + 90f;
            triangle.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

            // Add component to list
            triangleMissions.Add(triangle.GetComponent<MissionSelection>());

            // Animate the triangle with DOTween
            // First animate to position with slight delay based on index
            float delay = i * 0.05f;
            triangle.transform.DOMove(spawnPosition, 0.5f).SetDelay(delay).SetEase(Ease.OutBack);

            // Animate scale with the same delay, but preserve original scale proportions
            triangle.transform.DOScale(originalScale, 0.5f).SetDelay(delay).SetEase(Ease.OutBack);
        }

        for (int i = 0; i < triangleMissions.Count; i++)
        {
            triangleMissions[i].triangleParent = trianglesParent;
            triangleMissions[i].missionManager = this;
            if (i < missions.Length && missions[i] != null)
            {
                triangleMissions[i].SetMission(missions[i]);
                if (i == 0)
                {
                    triangleMissions[i].GetComponent<SpriteRenderer>().color = Color.white;
                    triangleMissions[i].missionLocked = false;
                    triangleMissions[i].missionSelectionThatIsUnlockedOnComplete = triangleMissions[i + 1];
                }
                else if ( i < triangleMissions.Count - 1)
                {
                    triangleMissions[i].GetComponent<SpriteRenderer>().color = Color.red;
                    triangleMissions[i].missionLocked = true;
                    triangleMissions[i].missionSelectionThatIsUnlockedOnComplete = triangleMissions[i + 1];
                }
                else
                {
                    triangleMissions[i].GetComponent<SpriteRenderer>().color = Color.red;
                    triangleMissions[i].missionLocked = true;
                    
                }
            }
            else
            {
                triangleMissions[i].GetComponent<SpriteRenderer>().color = Color.black;
                Debug.Log("No more missions available");
            }
        }

        ToggleSelectMissionUI(true);
    }

    public void InstanciatePlayers()
    {
        float totalWidth = (numberOfPlayers - 1) * spacingBetweenPlayers;
        float verticalOffset = 2f;
        float duration = 4f;
        int numberOfRepeats = 0;
        List<Tweener> moveTweens = new List<Tweener>();

        for (int i = 0; i < numberOfPlayers; i++)
        {
            float xPos = (i * spacingBetweenPlayers) - (totalWidth / 2f);
            Vector2 spawnPosition = new Vector2(xPos, transform.position.y);
            Vector2 startPosition = spawnPosition + Vector2.up * verticalOffset;

            GameObject playerTemp = Instantiate(playerPrefab, startPosition, Quaternion.identity);

            if (i - (numberOfRepeats * namesFromInput.Count) >= namesFromInput.Count)
                numberOfRepeats++;

            players.Add(playerTemp);

            Person person = playerTemp.GetComponent<Person>();
            person.nameOfPerson.text = namesFromInput[i - (numberOfRepeats * namesFromInput.Count)];

            if (person.spriteRendererCircle != null)
            {
                Color col = person.spriteRendererCircle.color;
                col.a = 0f;
                person.spriteRendererCircle.color = col;
                person.spriteRendererCircle.DOFade(1f, duration);
            }

            if (person.spriteRendererImage != null)
            {
                Color col = person.spriteRendererImage.color;
                col.a = 0f;
                person.spriteRendererImage.color = col;
                person.spriteRendererImage.DOFade(1f, duration);
            }

            if (person.nameOfPerson != null)
            {
                Color col = person.nameOfPerson.color;
                col.a = 0f;
                person.nameOfPerson.color = col;
                person.nameOfPerson.DOFade(1f, duration);
            }

            Tweener moveTween = playerTemp.transform.DOMove(spawnPosition, duration).SetEase(Ease.OutElastic);
            moveTweens.Add(moveTween);
        }

        Sequence sequence = DOTween.Sequence();
        foreach (var tween in moveTweens)
        {
            sequence.Join(tween);
        }

        sequence.OnComplete(() =>
        {
            ApplySpecialEffectsToRandomPlayer();
        });
    }


    private void ApplySpecialEffectsToRandomPlayer()
    {
        if (players.Count == 0) return;

        int selectedIndex = Random.Range(0, players.Count);
        float initialDelay = 0.05f;
        float delayIncrement = 0.05f;
        float currentDelay = initialDelay;

        int previousIndex = -1;

        // Set all titles to Crewmate first
        foreach (var player in players)
        {
            Person p = player.GetComponent<Person>();
            if (p != null && p.nameOfTitle != null)
            {
                p.nameOfTitle.text = "Crewmate";
                p.nameOfTitle.fontSize = 30f;
                p.nameOfTitle.fontStyle = FontStyles.Normal;
                p.nameOfTitle.color = Color.white;
                p.nameOfTitle.alpha = 0f;
            }
        }

        Sequence rouletteSeq = DOTween.Sequence();

        for (int i = 0; i <= selectedIndex + players.Count * 3; i++)  // Loop for effect
        {
            int currentIndex = i % players.Count;
            bool isFinal = (i == selectedIndex + players.Count * 3);

            rouletteSeq.AppendCallback(() =>
            {
                if (previousIndex >= 0 && previousIndex != currentIndex)
                    UnhighlightPlayer(previousIndex); // Will show "Crewmate"

                HighlightPlayer(currentIndex, isFinal, currentDelay); // Will show "Captain"
                previousIndex = currentIndex;
            });

            rouletteSeq.AppendInterval(currentDelay);
            currentDelay += delayIncrement;
        }

        rouletteSeq.AppendInterval(1f);
        rouletteSeq.AppendCallback(() =>
        {
            DetransitionAndSpawnTriangles();
        });
    }



    private void HighlightPlayer(int index, bool isFinal, float duration)
    {
        GameObject player = players[index];
        if (player == null) return;

        Person person = player.GetComponent<Person>();
        if (person == null) return;

        SpriteRenderer circle = person.spriteRendererCircle;

        // TITLE = Show Captain temporarily
        if (person.nameOfTitle != null)
        {
            person.nameOfTitle.gameObject.SetActive(true);
            person.nameOfTitle.text = "CAPITÃ";
            person.nameOfTitle.fontSize = 40f;
            person.nameOfTitle.fontStyle = FontStyles.Bold;
            Color targetColor = circle.material.GetColor("_OutlineColor");
            person.nameOfTitle.color = targetColor;
            Color titleColor = person.nameOfTitle.color;
            titleColor.a = 0f;
            person.nameOfTitle.color = titleColor;
            person.nameOfTitle.DOFade(1f, duration);
        }

        // OUTLINE + DISTORTION
        if (circle != null)
        {
            Material mat = new Material(circle.material);
            circle.material = mat;

            if (mat.HasProperty("_Outline")) mat.SetFloat("_Outline", 1f);
            if (mat.HasProperty("_OutlineThickness")) mat.SetFloat("_OutlineThickness", 0f);
            if (mat.HasProperty("_Distortion")) mat.SetFloat("_Distortion", 1f);
            if (mat.HasProperty("_DistortionStrength")) mat.SetFloat("_DistortionStrength", 0f);

            DOTween.To(() => mat.GetFloat("_OutlineThickness"), x => mat.SetFloat("_OutlineThickness", x), 0.05f, 0.4f);
            DOTween.To(() => mat.GetFloat("_DistortionStrength"), x => mat.SetFloat("_DistortionStrength", x), 0.03f, 0.4f);

            if (mat.HasProperty("_PixelResolution"))
                mat.SetFloat("_PixelResolution", 250f);
        }

        if (person.spriteRendererImage != null)
        {
            Material imgMat = new Material(person.spriteRendererImage.material);
            person.spriteRendererImage.material = imgMat;

            if (imgMat.HasProperty("_PixelResolution"))
                imgMat.SetFloat("_PixelResolution", 250f);
        }
    }


    private void UnhighlightPlayer(int index)
    {
        if (index < 0 || index >= players.Count) return;

        GameObject player = players[index];
        if (player == null) return;

        Person person = player.GetComponent<Person>();
        if (person == null) return;

        if (person.nameOfTitle != null)
        {
            person.nameOfTitle.text = "";
            person.nameOfTitle.fontSize = 30f;
            person.nameOfTitle.fontStyle = FontStyles.Normal;
            person.nameOfTitle.color = Color.white;
            person.nameOfTitle.alpha = 0f;
            person.nameOfTitle.DOFade(1f, 0.2f);
        }

        SpriteRenderer circle = person.spriteRendererCircle;
        if (circle != null)
        {
            Material mat = circle.material;

            if (mat.HasProperty("_OutlineThickness"))
                mat.SetFloat("_OutlineThickness", 0f);

            if (mat.HasProperty("_DistortionStrength"))
                mat.SetFloat("_DistortionStrength", 0f);

            if (mat.HasProperty("_PixelResolution"))
                mat.SetFloat("_PixelResolution", 0f);
        }

        SpriteRenderer image = person.spriteRendererImage;
        if (image != null)
        {
            Material mat = image.material;

            if (mat.HasProperty("_PixelResolution"))
                mat.SetFloat("_PixelResolution", 0f);
        }
    }




    private void DetransitionAndSpawnTriangles()
    {
        float fadeDuration = 1.2f;
        int completed = 0;

        foreach (GameObject player in players)
        {
            if (player == null) continue;

            Person person = player.GetComponent<Person>();
            if (person == null) continue;

            SpriteRenderer circle = person.spriteRendererCircle;
            SpriteRenderer image = person.spriteRendererImage;
            TextMeshProUGUI text = person.nameOfPerson;
            TextMeshProUGUI title = person.nameOfTitle;  // Reference the title text

            Material circleMaterial = null;
            Material imageMaterial = null;

            if (circle != null)
            {
                circleMaterial = new Material(circle.material);
                circle.material = circleMaterial;

                if (circleMaterial.HasProperty("_Distortion"))
                    circleMaterial.SetFloat("_Distortion", 1f);

                if (circleMaterial.HasProperty("_DistortionStrength"))
                    DOTween.To(() => circleMaterial.GetFloat("_DistortionStrength"), x => circleMaterial.SetFloat("_DistortionStrength", x), 0.1f, fadeDuration);

                if (circleMaterial.HasProperty("_Pixelation"))
                    circleMaterial.SetFloat("_Pixelation", 1f);

                // Apply Pixelation for the circle sprite
                if (circleMaterial.HasProperty("_PixelResolution"))
                    DOTween.To(() => circleMaterial.GetFloat("_PixelResolution"), x => circleMaterial.SetFloat("_PixelResolution", x), 0f, fadeDuration);
            }

            if (image != null)
            {
                imageMaterial = new Material(image.material);
                image.material = imageMaterial;

                if (imageMaterial.HasProperty("_Pixelation"))
                    imageMaterial.SetFloat("_Pixelation", 1f);

                // Apply Pixelation for the image sprite
                if (imageMaterial.HasProperty("_PixelResolution"))
                    DOTween.To(() => imageMaterial.GetFloat("_PixelResolution"), x => imageMaterial.SetFloat("_PixelResolution", x), 0f, fadeDuration);
            }

            int fadeComponents = 0;

            if (circle != null)
            {
                fadeComponents++;
                circle.DOFade(0f, fadeDuration).OnComplete(() => { CheckDone(); });
            }

            if (image != null && image != circle)
            {
                fadeComponents++;
                image.DOFade(0f, fadeDuration).OnComplete(() => { CheckDone(); });
            }

            if (text != null)
            {
                fadeComponents++;
                text.DOFade(0f, fadeDuration).OnComplete(() => { CheckDone(); });
            }

            if (title != null)
            {
                fadeComponents++;
                title.DOFade(0f, fadeDuration).OnComplete(() => { CheckDone(); });
            }

            void CheckDone()
            {
                fadeComponents--;
                if (fadeComponents <= 0)
                {
                    Destroy(player);
                    completed++;

                    if (completed >= players.Count)
                    {
                        players.Clear();
                        SpawnTriangles(centerOfTriangles.position);
                    }
                }
            }
        }
    }



}
