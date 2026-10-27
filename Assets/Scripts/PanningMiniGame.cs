using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PanningMiniGame : MonoBehaviour
{
    [Header("References")]
    public Slider slider;
    public GameObject visualGoal;
    public TextMeshProUGUI playerGoldText;
    public Image[] guessDisplays;
    public Sprite[] guessSprite; // ?, Check, X
    public Animator swirlAni;

    public PanningSpawner spawner;
    internal GameObject panningSpot;

    [Header("Handle Settings")]
    public GameObject movingGuess;
    public float speed = 2f;
    public float amp = 0.45f;

    [Header("Game Settings")]
    public bool moveSliderHandle = false;
    public float goalValue = 0f; // -64 to 64
    public float goalPercent = 0.5f; // 0 to 1
    public float stoppedPercent;

    public int panAttempts = 0;
    public float delayAfterPan = 2f;

    public float guessOffset = 0.0f;
    public float guessingGrace = 0.03f;

    [Header("Game Score")]
    public float playerGoldAmount = 0;
    float panningGold = 0;
    float goldBuff = 1;
    float goldNerf = 1;
    public bool[] scores = new bool[3];

    PlayerController player;

    private void Start()
    {
        slider = GetComponent<Slider>();
        player = FindAnyObjectByType<PlayerController>();

        playerGoldText.text = (playerGoldAmount + "g");
    }

    private void Update()
    {
        if (player.isPanning && moveSliderHandle)
        {
            // Move slider
            float pos = 0.5f + Mathf.Sin(Time.time * speed) * amp;
            slider.SetValueWithoutNotify(pos);
        }
    }

    private void MovePanGoal()
    {
        //Debug.Log("PanningMiniGame - Move visual goal");
        // Set up goal
        int ran = Random.Range(0, 129);
        goalValue = ran - 64f;
        goalPercent = (ran / 144f) + 0.05f;

        visualGoal.GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(-48 / 16, goalValue / 16f, 1440 / 16), Quaternion.identity);
    }

    public void StartPanning(GameObject spot)
    {
        panningSpot = spot;

        // Update guessing sprites
        foreach (Image i in guessDisplays)
        {
            i.sprite = guessSprite[0];
        }

        // Get gold size
        float temp = Random.Range(0f, 11f);
        panningGold = Mathf.Pow(1.1f, (temp - 10));
        Debug.Log("PanningMiniGame - panning for gold worth " + panningGold);

        goldBuff = 1;
        goldNerf = 1;

        MovePanGoal();

        // Activate moving guess bar
        moveSliderHandle = true;

        // Slider moves in update because bool "isPanning" should be true

        // Wait for player to input again (called from Player Controller)
    }

    public IEnumerator AttemptToPan()
    {
        //Debug.Log("PanningMiniGame - Freeze slider handle");
        moveSliderHandle = false;

        stoppedPercent = slider.value + guessOffset;

        // Round to two decimal place
        float playerGuessRounded = Mathf.Round(stoppedPercent * 100.0f) * 0.01f;
        float goalValueRounded = Mathf.Round(goalPercent * 100.0f) * 0.01f;
        //Debug.Log("Stopped at " + playerGuessRounded + "\nGoal was " + goalValueRounded);

        // Score player
        if (playerGuessRounded - guessingGrace < goalValueRounded && goalValueRounded < playerGuessRounded + guessingGrace)
        {
            scores[panAttempts] = true;
            guessDisplays[panAttempts].sprite = guessSprite[1];
            //Debug.Log("Player got it right!");
        }
        else
        {
            scores[panAttempts] = false;
            guessDisplays[panAttempts].sprite = guessSprite[2];
            //Debug.Log("Player got it wrong");
        }

        swirlAni.SetTrigger("splash");

        yield return new WaitForSeconds(delayAfterPan);

        player.panning = null;
        panAttempts++;

        // You get three attempts
        if (panAttempts >= 3)
        {
            StopPanning();
        }
        else
        {
            moveSliderHandle = true;
            MovePanGoal();
        }
    }

    public void StopPanning()
    {
        // Score Player

        int successfulPans = 0;

        foreach (bool s in scores)
        {
            if (s)
            {
                successfulPans++;
                goldBuff += 0.05f;
            }
            else goldNerf -= 0.1f;
        }

        if(successfulPans > 0)
        {
            float mult = Random.Range(goldNerf, goldBuff + 0.01f);
            panningGold *= mult;
            Debug.Log("PanningMiniGame - player earned " + panningGold + " from mult of " + mult);
            playerGoldAmount += Mathf.Round(panningGold * 10f) * 0.1f;
            playerGoldText.text = (playerGoldAmount + "g");
        }
        // Get no gold if failed all pans

        // Reset panning variables and ui
        moveSliderHandle = false;
        panAttempts = 0;
        player.isPanning = false;
        spawner.DeactivatePanningSpot(panningSpot);
        panningSpot = null;
        
        gameObject.SetActive(false);
    }

}
