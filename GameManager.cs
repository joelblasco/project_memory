using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private CardScript[] cards; //array de los objetos de cartas
    public Texture2D cardsDeck; //la imagen de la carta    
    private CardScript revealedCard; //la carta mostrada actualmente.
    private int pairs, pairsNeeded, actions;
    public Text info;
    public GameObject cardPrefab;
    private int cardsToPlay;
    public InputField pairsInput;

    [SerializeField] private float interlineSpace;
    [SerializeField] private int cardsPerLine;
    void Start()
    {
        StartCoroutine(initializeGame());
        
    }
    IEnumerator initializeGame()
    {
        //borrar anteriores
        if (pairsNeeded>0)
        {
            foreach (CardScript c in cards)
            {
                Destroy(c.gameObject);
            }
        }

        pairsNeeded = Int32.Parse(pairsInput.text);
        cardsToPlay = pairsNeeded*2;
        print(cardsToPlay);
        //crear gameobjects
        cards = new CardScript[cardsToPlay];

        if (cardsToPlay > 20) cardsPerLine = cardsToPlay / 4;
        if (cardsToPlay >= 30) cardsPerLine = cardsToPlay / 5;
        if (cardsToPlay >= 75) cardsPerLine = cardsToPlay / 8;
        float x, y;
        x = y = 1;
        for (int i = 0; i <cardsToPlay; i++)
        {
            Vector2 pos = new Vector2();
            float lineA = Mathf.Ceil(i / cardsPerLine); // fila actual
            pos.y = -1.75f * lineA;
            pos.x = -3 + (interlineSpace*i)-(lineA*(cardsPerLine*interlineSpace));
            x += pos.x;
            y += pos.y;
            float xMedia = x / (i+1);
            float yMedia = y / (i+1);
            Vector3 v = new Vector3(xMedia, yMedia,-5-(lineA*lineA*.2f));
            Camera.main.transform.SetPositionAndRotation(v, Quaternion.identity);
            GameObject newCard = Instantiate(cardPrefab,pos,Quaternion.identity);
            cards[i] = newCard.GetComponent<CardScript>();
            yield return new WaitForSeconds(0.05f);
        }
        Sprite[] cardsSprites = Resources.LoadAll<Sprite>(cardsDeck.name); //obtengo todos los slices de cartas
        List<int> usedCards = new List<int>(); //lista de indices usados
        List<int> usableCards = new List<int>(); //lista de indices por usar
        for (int i = 0; i < 52; i++) //4 palos por 13 cartas cada palo.
        {
            usableCards.Add(i);
        }

        for (int i = 0; i < pairsNeeded; i++)
        {
            CardScript c = cards[i]; //accedo a la carta actual
            CardScript c2 = cards[i + pairsNeeded]; //accedo a la pareja

            int r = usableCards[UnityEngine.Random.Range(0, usableCards.Count)]; //cojo un index aleatorio entre los no usados.

            usableCards.Remove(r); //elimino el int de cartas usables
            usedCards.Add(r); //añado el index de la carta a cartas usadas.

            c.front = c2.front = cardsSprites[r]; //añado los sprites de las cartas.            
            c.back = c2.back = cardsSprites[57];

            c.value = c2.value = r;
            c.index = i;
            c2.index = i + pairsNeeded;

            c.init(this);
            c2.init(this);
        }
        for (int i = 0; i < cardsToPlay; i++) //para barajar n veces las cartas
        {
            int r = UnityEngine.Random.Range(0,cardsToPlay); // cojo una al azar
            //intercambio posiciones entre la que estoy barajando y una carta al azar
            Vector3 v = cards[i].transform.position;
            cards[i].transform.SetPositionAndRotation(cards[r].transform.position, Quaternion.identity);
            cards[r].transform.SetPositionAndRotation(v, Quaternion.identity);

        }
        pairs = 0; // no existen parejas hechas.
        actions = 0;
        updateInfo();
    }
    public void showCard(int index)
    {
        actions++;
        cards[index].show(true);
        if (revealedCard != null) // si no tenemos carta mostrada
        {
            if(revealedCard.value == cards[index].value) // si el valor de la carta ya mostrada coincide con la ultima carta mostrada
            {
                pairs++;
                cards[index].foundPair();
                revealedCard.foundPair();
                revealedCard = null;
            }
            else // si no son iguales, las tapamos.
            {
                revealedCard.show(false);
                cards[index].show(false);
                revealedCard = null;
            }
        }
        else revealedCard = cards[index]; //establecer carta mostrada
        updateInfo();
        checkWinCondition();
    }
    void updateInfo()
    {
        info.text = $"Parejas: {pairs}/{pairsNeeded} \nAcciones: {actions}";
    }
    void checkWinCondition()
    {
        if(pairs== pairsNeeded)
        {
            info.text = $"HAS GANADO! \nAcciones realizadas: {actions}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(initializeGame());
    }
}
