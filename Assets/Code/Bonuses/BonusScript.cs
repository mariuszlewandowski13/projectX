using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusScript : MonoBehaviour {

    private Bonus bonusData;

    public bool canWork;
    
    public void SetBonusData(Bonus bonus)
    {
        bonusData = bonus;
        canWork = true;
        SetBonusGraphics();
    }

    private void SetBonusGraphics()
    {
        GetComponent<Renderer>().material.SetColor("_Color", bonusData.color);
    }

    void OnDestroy()
    {
        if (canWork)
        {
            BonusManager.UseBonusOnBallDestroy(bonusData);
        }
        
    }
}
