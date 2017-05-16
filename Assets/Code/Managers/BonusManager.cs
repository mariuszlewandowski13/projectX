using UnityEngine;
using System.Collections.Generic;

public enum BonusKind
{
    slower,
    rollBack,
    colorDestroy,
    ballsNeighboursDestroy,
    ballsSphereDestroy
}

public class Bonus
{
    public Color color;

    public BonusKind bonusKind;

    public float bonusEndTime;

    public Color ballColor;

    public int range;

    public Bonus(Color newColor, BonusKind newKind, Color ballColor, int range)
    {
        color = newColor;
        bonusKind = newKind;
        bonusEndTime = 0.0f;
        this.ballColor = ballColor;
        this.range = range;
    }
}

public class BonusManager : MonoBehaviour {

    public GameObject bonusObjectPrefab;

    private Color[] bonusesColors = new Color[] { Color.black, Color.gray, Color.magenta, Color.cyan };

    private  System.Random ran = new System.Random();
    private  System.Random ranColors = new System.Random();
    private System.Random ranRange = new System.Random();
    private  int counter;

    private  int counterTreshold;
    private  int randTreshold = 10;

    private static List<Bonus> actualBonuses;

    private static float bonusDurationTime = 6.0f;

    private static object useBonusLock = new object();

    void Start()
    {
        GameManagerScript.ballCreated += AddBonus;
        actualBonuses = new List<Bonus>();
    }

    void Update()
    {
        CheckEndRevertBonuses();
    }

    private void CheckEndRevertBonuses()
    {
        if (actualBonuses.Count > 0)
        {
            Bonus bonusToRevert = null;
            foreach (Bonus bonus in actualBonuses)
            {
                if (Time.time > bonus.bonusEndTime)
                {
                    bonusToRevert = bonus;
                    break;
                }
            }

            if (bonusToRevert != null)
            {
                RevertBonus(bonusToRevert);
                actualBonuses.Remove(bonusToRevert);
            }
        }
    }

    private void RevertBonus(Bonus bonus)
    {
        if (bonus.bonusKind == BonusKind.slower)
        {
            BallObject.IncreaseNormalSpeedLevel(3);
        }

        //if (bonus.bonusKind == BonusKind.rollBack)
        //{
        //    GameObject.Find("GameManager").GetComponent<GameManagerScript>().RevertBallsDirection();
        //}

        
    }

    private void AddBonus(GameObject ball)
    {
        if (counterTreshold == 0)
        {
            counterTreshold = ran.Next(2, randTreshold);
        }
        if (counter >= counterTreshold)
        {
            counter = 0;
            counterTreshold = 0;
            RandNewBonus(ball);
        }
        counter++;
    }

    private void RandNewBonus(GameObject ball)
    {
        int number = ranColors.Next(0, bonusesColors.Length);
        Color bonusColor = bonusesColors[number];
        GameObject bonus = Instantiate(bonusObjectPrefab, ball.transform.position, new Quaternion());
        bonus.name = "Bonus";
        bonus.transform.parent = ball.transform;
        int range = ranRange.Next(2, 5);
        bonus.GetComponent<BonusScript>().SetBonusData(new Bonus(bonusColor, (BonusKind)number, ball.GetComponent<BallScript>().ballObj.color, range));
    }

    void OnDestroy()
    {
        GameManagerScript.ballCreated -= AddBonus;
    }


    public static void UseBonusOnBallDestroy(Bonus newBonus)
    {
        lock(useBonusLock)
        {
            if (CheckBonusCanBeUsed(newBonus))
            {
                UseBonus(newBonus);
            }
        }
    }

    private static void UseBonus(Bonus bonus)
    {
        if (GameManagerScript.playing)
        {
            if (bonus.bonusKind == BonusKind.slower)
            {
                BallObject.DecreaseNormalSpeedLevels(3);
            }

            //if (bonus.bonusKind == BonusKind.rollBack)
            //{
            //    GameObject.Find("GameManager").GetComponent<GameManagerScript>().RevertBallsDirection();
            //}

            if (bonus.bonusKind == BonusKind.colorDestroy)
            {
                GameObject.Find("GameManager").GetComponent<GameManagerScript>().DestroyBallsWithSpecificColor(bonus.ballColor);
            }

            if (bonus.bonusKind <= BonusKind.rollBack)
            {
                bonus.bonusEndTime = Time.time + bonusDurationTime;
                actualBonuses.Add(bonus);
            }
        }
        
    }

    private static bool CheckBonusCanBeUsed(Bonus bonus)
    {
        if (bonus.bonusKind <= BonusKind.rollBack)
        {
            if (CheckIfListContainsBonus(bonus))
            {
                return false;
            }
        }
        return true;
    }

    private static bool CheckIfListContainsBonus(Bonus bonus)
    {
        foreach (Bonus bon in actualBonuses)
        {
            if (bon.bonusKind == bonus.bonusKind)
            {
                return true;
            }
        }

        return false;
    }

}
