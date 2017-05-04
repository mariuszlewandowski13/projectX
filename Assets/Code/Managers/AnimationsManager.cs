using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsManager: MonoBehaviour {

    private static GameObject animationObject;

    public static bool levelEndedAnimation;
    public static bool levelStartAnimation;
    public static bool gameEndedAnimation;

    private static float animationDuration = 2.0f;
    private static float animationStarted;

    private static bool isPlaying;

    void Start()
    {
        animationObject = gameObject;
    }

	void Update () {
        if (isPlaying)
        {
            if (levelEndedAnimation && ((Time.time - animationStarted) > animationDuration))
            {
                StopLevelEndedAnimation();
            }

            if (levelStartAnimation && ((Time.time - animationStarted) > animationDuration))
            {
                StopLevelStartAnimation();
            }
        }
	}

    public static void PlayLevelEndedAnimation(int level)
    {
        if (!isPlaying)
        {
            isPlaying = true;
            animationStarted = Time.time;
            if (animationObject != null)
            {
                animationObject.transform.FindChild("LevelCompleted").gameObject.SetActive(true);
                animationObject.transform.FindChild("LevelCompleted").FindChild("LevelNumber").GetComponent<TextMesh>().text = (level + 1).ToString();
            }
               
            levelEndedAnimation = true;
        }
    }

    private static void StopLevelEndedAnimation()
    {
        if (isPlaying)
        {
            isPlaying = false;
            if (animationObject != null)
            {
                animationObject.transform.FindChild("LevelCompleted").gameObject.SetActive(false);
            }
            levelEndedAnimation = false;
        }
    }

    public static void PlayLevelStartAnimation(int level)
    {
        if (!isPlaying)
        {
            isPlaying = true;
            animationStarted = Time.time;
            if (animationObject != null)
            {
                animationObject.transform.FindChild("LevelStart").gameObject.SetActive(true);
                animationObject.transform.FindChild("LevelStart").FindChild("LevelNumber").GetComponent<TextMesh>().text = (level + 1).ToString();
            }

            levelStartAnimation = true;
        }
    }

    private static void StopLevelStartAnimation()
    {
        if (isPlaying)
        {
            isPlaying = false;
            if (animationObject != null)
            {
                animationObject.transform.FindChild("LevelStart").gameObject.SetActive(false);
            }
            levelStartAnimation = false;
        }
    }

    public static void PlayGameWonAnimation()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            animationStarted = Time.time;
            if (animationObject != null)
            {
                animationObject.transform.FindChild("GameWon").gameObject.SetActive(true);
            }

            gameEndedAnimation = true;
        }
    }

    private static void StopGameWonAnimation()
    {
        if (isPlaying)
        {
            isPlaying = false;
            if (animationObject != null)
            {
                animationObject.transform.FindChild("GameWon").gameObject.SetActive(false);
            }
            gameEndedAnimation = false;
        }
    }

    public static void PlayGameLostAnimation()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            animationStarted = Time.time;
            if (animationObject != null)
            {
                animationObject.transform.FindChild("GameLost").gameObject.SetActive(true);
            }

            gameEndedAnimation = true;
        }
    }

    private static void StopGameLostAnimation()
    {
        if (isPlaying)
        {
            isPlaying = false;
            if (animationObject != null)
            {
                animationObject.transform.FindChild("GameLost").gameObject.SetActive(false);
            }
            gameEndedAnimation = false;
        }
    }


    public static void ClearAnimations()
    {

        if (animationObject)
        {
            foreach (Transform child in animationObject.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        isPlaying = false;
    }
}
