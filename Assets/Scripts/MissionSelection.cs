using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class MissionSelection : MonoBehaviour
{
    public float moveDistance = 1f; // Distance to move up
    public float moveDuration = 1f; // Time to move up/down
    public Ease easingType = Ease.InOutSine; // Smoother animation
    private Vector3 startPosition;

    private Mission missionSO;
    public void SetMission(Mission mission) => missionSO = mission;

    public Transform triangleParent;

    private EventHandler onMissionClicked;

    private delegate void OnMissionClicked(int num);

    private bool isUp = false;

    public bool missionLocked;
    public bool missionComplete;
    public MissionSelection missionSelectionThatIsUnlockedOnComplete;

    public MissionManager missionManager;

    private GameObject gamePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void InitializeStartPosition(Vector3 startPosition)
    {
        this.startPosition = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);
            //Debug.Log($"Touch detected at: {touch.position} - Phase: {touch.phase}");
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0; // Keep it on the same plane

            switch (touch.phase)
            {
                case TouchPhase.Began:

                    RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        GoUp();
                        Debug.Log("Start Clicking");
                    }
                    break;

                case TouchPhase.Moved:

                    hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject && !isUp)
                    {
                        GoUp();
                        Debug.Log("Not Clicked");
                    }

                    if ((hit.collider == null || hit.collider.gameObject != gameObject) && isUp)
                    {
                        GoDown();
                        Debug.Log("Not Clicked");
                    }
                    break;

                case TouchPhase.Ended:
                    Debug.Log("Ended");

                    hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        GoDown();

                        //if(missionSO != null)
                        //    if (missionSO.missionPrefab != null)
                        //    {  
                        //        Instantiate(missionSO.missionPrefab);
                        //        triangleParent.gameObject.SetActive(false);
                        //    }

                        if (missionLocked)
                        {
                            if (!missionComplete)
                                missionManager.ShowLockedMissionUI();
                            else
                                Debug.Log("Mission Complete");
                        }
                        else if (!missionLocked)
                        {
                            if (missionSO != null)
                                missionManager.ShowUnlockedMissionUI(missionSO.missionTitle, missionSO.missionDescription, SelectMission);
                            else
                                missionManager.ShowUnlockedMissionUI("Test title", "Test Description Test Description Test Description Test Description Test Description ", SelectMission);
                        }
                        Debug.Log("Clicked");
                    }

                    break;
            }
        }

    }

    void SelectMission()
    {
        if (missionSO != null)
            if (missionSO.missionPrefab != null)
            {
                GameObject missionClone = Instantiate(missionSO.missionPrefab);
                gamePrefab = missionClone;
                Popup(gamePrefab);
                missionClone.GetComponent<IPuzzle>().StartPuzzle(this);
                missionManager.CloseAllUIs();
                triangleParent.gameObject.SetActive(false);
            }
        Debug.Log("Mission Select");
    }



    public void CompleteMission()
    {
        missionLocked = true;

        missionComplete = true;
        missionSelectionThatIsUnlockedOnComplete.missionLocked = false;
        missionSelectionThatIsUnlockedOnComplete.GetComponent<SpriteRenderer>().color = Color.white;
        //Popdown(gamePrefab, duration: 1.0f, onCompleteCallback: DeactivatePrefab);


    }

    private void DeactivatePrefab()
    {
        gamePrefab.SetActive(false);
        triangleParent.gameObject.SetActive(true);

        missionManager.ToggleSelectMissionUI(true);
    }

    // Method to trigger the popup effect (scaling up)
    public void Popup(GameObject target, float duration = 0.4f, float targetScale = 1f, UnityAction onCompleteCallback = null)
    {
        // Start with a smaller scale and animate to the target scale
        target.transform.localScale = Vector3.zero;
        target.SetActive(true);  // Ensure the object is active before starting the animation
        target.transform.DOScale(targetScale, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => onCompleteCallback?.Invoke());
    }

    public void PopdownWithoutCompletion()
    {
        Popdown(gamePrefab, onCompleteCallback: DeactivatePrefab);
    }

    // Method to trigger the popdown effect (scaling down)
    public void Popdown(GameObject target, float duration = 0.4f, float targetScale = 0f, UnityAction onCompleteCallback = null)
    {
        // Animate to a smaller scale
        target.transform.DOScale(targetScale, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                target.SetActive(false);  // Disable object after animation
                onCompleteCallback?.Invoke();  // Call the method after the animation is complete
            });
    }

    void GoUp()
    {
        isUp = true;
        Vector3 moveDirection = transform.up * moveDistance; // Move in local up direction

        transform.DOMove(startPosition - moveDirection, moveDuration)
            .SetEase(easingType); // Once finished, call GoDown
    }

    void GoDown()
    {
        Vector3 moveDirection = transform.up * moveDistance; // Move in local up direction

        transform.DOMove(startPosition, moveDuration)
            .SetEase(easingType).OnComplete(() => isUp = false);
    }
}
