using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Deck script
/// </summary>
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

    /// <summary>
    /// Build the deck, create a card for each color and value
    /// </summary>
    public void BuildDeck()
    {
        foreach (string CardColor in Enum.GetNames(typeof(CardsColorsEnum)))
        {
            foreach (string CardValue in Enum.GetNames(typeof(CardsValuesEnum)))
            {
                //IF this is not the Joker (only two Joker in the game so I prefered to make them out of this loop
                if (((CardsValuesEnum)Enum.Parse(typeof(CardsValuesEnum), CardValue))!= CardsValuesEnum.JOKER)
                {
                    CardsInDeck.Add(CreateCard(((CardsColorsEnum)Enum.Parse(typeof(CardsColorsEnum), CardColor)), ((CardsValuesEnum)Enum.Parse(typeof(CardsValuesEnum), CardValue))));
                }
            }
        }
        //Add the two Joker
        CardsInDeck.Add(CreateCard(CardsColorsEnum.CLUB, CardsValuesEnum.JOKER));
        CardsInDeck.Add(CreateCard(CardsColorsEnum.DIAMOND, CardsValuesEnum.JOKER));
    }

    /// <summary>
    /// Create a new card with the given color and value
    /// </summary>
    /// <param name="CardColor"></param>
    /// <param name="CardValue"></param>
    /// <returns>Card : the new card created</returns>
    private Card CreateCard(CardsColorsEnum CardColor, CardsValuesEnum CardValue)
    {
        GameObject newCardObject = Instantiate(CardPrefab);
        Card newCard = newCardObject.GetComponent<Card>();
        newCard.Initialize(CardColor,CardValue);
        return newCard;
    }

    /// <summary>
    /// Draw a Card from the deck and spawn it over the network
    /// </summary>
    /// <returns></returns>
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
    /// Retrieve a card with its color and value
    /// </summary>
    /// <param name="CardColor">string : color of the card</param>
    /// <param name="CardValue">string : value of the card</param>
    /// <returns></returns>
    public Card StrToCard(String CardColor, String CardValue)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(CardValue).Append('_').Append(CardColor);
        return StrToCard(sb.ToString());

    }

    /// <summary>
    /// Retrieve a card with its color and value
    /// </summary>
    /// <param name="cardName">string : name of the card composed of value + card</param>
    /// <returns></returns>
    public Card StrToCard(String cardName)
    {
        return GameObject.Find(cardName).GetComponent<Card>();
    }


}
