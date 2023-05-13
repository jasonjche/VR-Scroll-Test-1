using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextureScroll : MonoBehaviour {
    [SerializeField] private float scrollSpeed = 1e-18f;
    [SerializeField] private Material material;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private float scrollDeceleration = 5f;
    private float previousScrollSpeed;  
    private float targetScrollSpeed;
    private float startTime;
    private float timeTaken;
    private float inactivityTimer;
    private const float inactivityThreshold = 0.5f;
    private const float MatchLowerBound = 2.3f;
    private const float MatchUpperBound = 2.5f;
    private const float NegativeMatchLowerBound = -7.7f;
    private const float NegativeMatchUpperBound = -7.5f;

    private void Start() {
        previousScrollSpeed = 9f;
        targetScrollSpeed = previousScrollSpeed;
        material = GetComponent<Renderer>().sharedMaterial;
        material.SetFloat("_Scrolling_Speed", previousScrollSpeed);
        startTime = Time.time;
        inactivityTimer = 0f;
    }

    private void Update() {
        Debug.Log(previousScrollSpeed);
        previousScrollSpeed = material.GetFloat("_Scrolling_Speed");

        if (Input.mouseScrollDelta.y != 0) {
            targetScrollSpeed = previousScrollSpeed + (scrollSpeed * Input.mouseScrollDelta.y);
            inactivityTimer = 0f;
        } else {
            inactivityTimer += Time.deltaTime;
        }

        if (inactivityTimer >= inactivityThreshold) {
            previousScrollSpeed = Mathf.Lerp(previousScrollSpeed, targetScrollSpeed, Time.deltaTime * scrollDeceleration);
        } else {
            previousScrollSpeed = targetScrollSpeed;
        }
        material.SetFloat("_Scrolling_Speed", previousScrollSpeed);

        if (IsScrollInRange(previousScrollSpeed)) {
            timeTaken = Time.time - startTime;
            Invoke("CheckMatch", 1f);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Quit();
        }
    }

    private bool IsScrollInRange(float scrollValue) {
        return (scrollValue >= 0 && scrollValue % 10 > MatchLowerBound && scrollValue % 10 < MatchUpperBound) ||
            (scrollValue < 0 && scrollValue % 10 < NegativeMatchUpperBound && scrollValue % 10 > NegativeMatchLowerBound);
    }

    private void CheckMatch() {
        if (IsScrollInRange(previousScrollSpeed)) {
            resultText.text = string.Format("You took {0} seconds to get to the end", timeTaken);
            resultText.color = Color.green;
            Invoke("Quit", 5f);
            this.enabled = false;
        }
    }

    public void Quit() {
        #if UNITY_STANDALONE
        Application.Quit();
        #endif
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
