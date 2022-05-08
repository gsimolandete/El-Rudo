using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Moralis.Web3Api.Models;
using MoralisWeb3ApiSdk;
using static GlobalVariables;
using Moralis.Platform.Queries;
using Moralis.Platform.Objects;
using System.Threading.Tasks;

public class Testing : MonoBehaviour
{
    int i;
    public void test1()
    {
        NextMove();
    }
    IEnumerator NextMove()
    {
        print("next move iteration " + i);

        float timee = Time.timeSinceLevelLoad;

        MoveTo();

        yield return new WaitUntil(() => { return 3 < Time.timeSinceLevelLoad-timee; });

        print("next move ended iteration: " + i + " time: " + Time.timeSinceLevelLoad);
    }

    void MoveTo()
    {
        print("move to iteration " + i);
    }

    public void test2()
    {

    }
}
