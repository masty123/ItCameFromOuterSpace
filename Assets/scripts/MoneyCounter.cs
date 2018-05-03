﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCounter : MonoBehaviour {

    private Text MoneyText;


    void Awake()
    {
        MoneyText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        MoneyText.text = "MONEY: " + GameMaster.Money;
    }
}
