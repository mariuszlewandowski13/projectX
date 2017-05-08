using UnityEngine;

public struct Bonus
{
    public Color color;

    public Bonus(Color newColor)
    {
        color = newColor;
    }
}

public class BonusManager : MonoBehaviour {

    public GameObject bonusObjectPrefab;

    private Color[] bonusesColors = new Color[] { Color.black, Color.gray, Color.magenta, Color.cyan };

    private System.Random ran = new System.Random();
    private  System.Random ranColors = new System.Random();
    private  int counter;

    private  int counterTreshold;
    private  int randTreshold = 15;

    void Start()
    {
        GameManagerScript.ballCreated += AddBonus;
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
        Color bonusColor = bonusesColors[ranColors.Next(0, bonusesColors.Length - 1)];
        GameObject bonus = Instantiate(bonusObjectPrefab, ball.transform.position, new Quaternion());
        bonus.transform.parent = ball.transform;
        bonus.GetComponent<BonusScript>().SetBonusData(new Bonus(bonusColor));
    }

    void OnDestroy()
    {
        GameManagerScript.ballCreated -= AddBonus;
    }


}
