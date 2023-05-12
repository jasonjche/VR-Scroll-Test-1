using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextureScroll: MonoBehaviour {
    [SerializeField] private float scrollSpeed = 1e-18f;
    [SerializeField] private Material material;
    [SerializeField] private TMP_Text resultText;
    private float previousScrollSpeed;
    private float startTime;
    private float timeTaken;
    private const float MatchLowerBound = 6.78f; 
    private const float MatchUpperBound = 6.82f;
    private const float NegativeMatchLowerBound = -3.22f;
    private const float NegativeMatchUpperBound = -3.18f;

    private void Start() {
        previousScrollSpeed = 4.5f;
        material = GetComponent <Renderer>().sharedMaterial;
        material.SetFloat("_Scrolling_Speed", previousScrollSpeed);
        startTime = Time.time;
    }

    private void Update() {
        previousScrollSpeed = material.GetFloat("_Scrolling_Speed");
        material.SetFloat("_Scrolling_Speed", previousScrollSpeed + (scrollSpeed * Input.mouseScrollDelta.y));

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