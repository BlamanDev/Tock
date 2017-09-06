using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Deck : NetworkBehaviour
{
    public List<Card> CardsInDeck;
    public GameObject CardPrefab;

    public delegate void OnCardDrawed(CardsColorsEnum CardColor, CardsValuesEnum CardValue);
    [SyncEvent]
    public static event OnCardDrawed EventOnCardDrawed;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnStartServer()
    {
        BuildDeck();

    }


    public void BuildDeck()
    {
        foreach (string CardColor in Enum.GetNames(typeof(CardsColorsEnum)))
        {
            foreach (string CardValue in Enum.GetNames(typeof(CardsValuesEnum)))
            {
                if (((CardsValuesEnum)Enum.Parse(typeof(CardsValuesEnum), CardValue))!= CardsValuesEnum.JOKER)
                {
                    CardsInDeck.Add(CreateCard(((CardsColorsEnum)Enum.Parse(typeof(CardsColorsEnum), CardColor)), ((CardsValuesEnum)Enum.Parse(typeof(CardsValuesEnum), CardValue))));
                }
            }
        }
        CardsInDeck.Add(CreateCard(CardsColorsEnum.CLUB, CardsValuesEnum.JOKER));
        CardsInDeck.Add(CreateCard(CardsColorsEnum.DIAMOND, CardsValuesEnum.JOKER));
    }

    private Card CreateCard(CardsColorsEnum CardColor, CardsValuesEnum CardValue)
    {
        GameObject newCardObject = Instantiate(CardPrefab);
        Card newCard = newCardObject.GetComponent<Card>();
        newCard.Initialize(CardColor,CardValue);
        return newCard;
    }

    public Card DrawACard()
    {
        System.Random pickACard = new System.Random();
        Card drawedCard = CardsInDeck[pickACard.Next(CardsInDeck.Count)];
        //EventOnCardDrawed(drawedCard.Color, drawedCard.Value);
        CardsInDeck.Remove(drawedCard);
        GameObject newCardObject = drawedCard.gameObject;
        NetworkServer.Spawn(newCardObject);
        return drawedCard;
    }

    public Card StrToCard(String CardColor, String CardValue)
    {
        return (CardsInDeck.FindAll(x => x.Color == (CardsColorsEnum)Enum.Parse(typeof(CardsColorsEnum), CardColor))).Find(x => x.Value == ((CardsValuesEnum)Enum.Parse(typeof(CardsValuesEnum), CardValue)));

    }


}
