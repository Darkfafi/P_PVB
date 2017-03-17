using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPile : BaseFieldPile<CoinData>
{
    private CoinData _coinData = new CoinData();

    protected override CoinData ObjectGrabbing()
    {
        return _coinData;
    }


}

public class CoinData
{

}