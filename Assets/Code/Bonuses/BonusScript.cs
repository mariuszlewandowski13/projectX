using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusScript : MonoBehaviour {

    private Bonus bonusData;

    public void SetBonusData(Bonus bonus)
    {
        bonusData = bonus;
        SetBonusGraphics();
    }

    private void SetBonusGraphics()
    {
        GetComponent<Renderer>().material.SetColor("_Color", bonusData.color);
    }
}
