using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;

using NeoLux;
using Neo.Cryptography;
using UnityEngine;



//Config Part:
public class Config
{
    public static string serverAddr = "http://53.168.38.139";
    public static int numCars = 3;
    public static string[] prKeyCars = new string[] {
        "9c4f2d9f32146968ccf6a5b49ef434e66b93d3c67acb54298e0e571a48483d67",
        "e9e0a14b7e83c6dcde747f91d2d9e07ae19d3e2dd398589db91bd10a7717b861",
        "5a07886e5f91aa9fbb42800e16b070ea4779ec78e9954bf764ed31591dc70b33"
    };
    public static string keyCompany = "???";
    public static string symbol = "NCT";
    public static double intervalFetch = 2;
    public static double intervalFee = 5;
}


public class Server  {
    static public Server shared = new Server();
    protected List<KeyPair> keysCar = new List<KeyPair>();
    protected List<decimal> balancesCar = new List<decimal>();
    protected List<int> accidentTimes = new List<int>();

    protected KeyPair keyCompany = new KeyPair(Config.keyCompany.HexToBytes());
    protected decimal _balCompany = 0;

    private Timer timer = new Timer();
    private double _cacheIntFetch = 0;
    private double _cacheIntFee = 0;
    private List<decimal> cachedFee = new List<decimal>();
    private static double _updateInt = 0.5;

    protected bool IsToken()
    {
        return (Config.symbol != "NEO") && (Config.symbol != "GAS");
    }

    protected Server()
    {
        for (int i = 0; i< Config.numCars; i++)
        {
            keysCar.Add(new KeyPair(Config.prKeyCars[i].HexToBytes()));
            balancesCar.Add(0);
            accidentTimes.Add(0);
            cachedFee.Add(0);
        }

        timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        timer.Interval = _updateInt * 1000;
        timer.Enabled = true;
    }


    protected static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        Server.shared._cacheIntFetch += _updateInt;
        if(Server.shared._cacheIntFetch > Config.intervalFetch)
        {
            Server.shared._cacheIntFetch -= Config.intervalFetch;
            Server.shared.FetchData();
        }

        if (Server.shared._cacheIntFee > Config.intervalFee)
        {
            Server.shared._cacheIntFee -= Config.intervalFee;
            Server.shared.FetchData();
        }
        int height = NeoRPC.ForPrivateNet("http://52.170.21.212:10332", "Ac6V1vK3bQmpbzDEfpZRxAXWa38W5ugEFb").GetHeight();
        Debug.Log("height" + height);
    }

    protected decimal QueryBalanceOf(KeyPair account)
    {
        return NeoRPC.ForPrivateNet(Config.serverAddr).GetBalancesOf(account, IsToken())[Config.symbol];
    }




    protected void FetchData()
    {
        for (int i = 0; i < Config.numCars; i++)
        {
            balancesCar[i] = QueryBalanceOf(keysCar[i]);
        }
        _balCompany = QueryBalanceOf(keyCompany);
    }
    protected void SubmitFee()
    {
        for (int i = 0; i < Config.numCars; i++)
        {
            NeoRPC.ForPrivateNet(Config.serverAddr).SendAsset(keysCar[i], keyCompany.address, Config.symbol, cachedFee[i]);
            cachedFee[i] = 0;
        }
    }

    ///
    /// Algorithms
    /// 
    protected decimal Reimbursement(int carId, double damage)
    {
        return 100;
    }

    protected decimal Fee(int carId, double distance)
    {
        return (decimal)distance;
    }


    ///
    /// Public Functions
    /// 

    /** Call it when a collision happens on carId*/
    public void Collide(int carId, double damage)
    {
        if (carId >= Config.numCars) return;

        accidentTimes[carId]++;
        NeoRPC.ForPrivateNet(Config.serverAddr).SendAsset(keyCompany, keysCar[carId].address, Config.symbol, Reimbursement(carId, damage));
    }

    public void CarRun(int carId, double distance)
    {
        if (carId >= Config.numCars) return;
        else
        {
            cachedFee[carId] += Fee(carId,distance);
        }
       
    }

    public decimal Balance(int carId)
    {
        return (carId >= Config.numCars) ? 0 : balancesCar[carId];
    }

    public string CarAddr (int carId)
    {
        return (carId >= Config.numCars) ? "N/A": keysCar[carId].address; 
    }

    public decimal BalanceCompany { get { return _balCompany; } }

     
}
