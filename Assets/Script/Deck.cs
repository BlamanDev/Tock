using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Deck : NetworkBehaviour
{
    public List<Card> CardsInDeck;
    public GameObject CardPrefab;

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
        CardsInDeck.Remove(drawedCard);
        GameObject newCardObject = drawedCard.gameObject;
        NetworkServer.Spawn(newCardObject);

        return drawedCard;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="CardColor"></param>
    /// <param name="CardValue"></param>
    /// <returns></returns>
    public Card StrToCard(String CardColor, String CardValue)
    {
        //return (CardsInDeck.FindAll(x => x.Color == (CardsColorsEnum)Enum.Parse(typeof(CardsColorsEnum), CardColor))).Find(x => x.Value == ((CardsValuesEnum)Enum.Parse(typeof(CardsValuesEnum), CardValue)));
        StringBuilder sb = new StringBuilder();
        sb.Append(CardValue).Append('_').Append(CardColor);
        return StrToCard(sb.ToString());

    }

    public Card StrToCard(String cardName)
    {
        return GameObject.Find(cardName).GetComponent<Card>();
    }


}
