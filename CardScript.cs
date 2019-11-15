using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    public Sprite front, back;
    public int value, index;
    private SpriteRenderer mySpriteRenderer;
    private bool paired;
    private GameManager gm;
    // Start is called before the first frame update
    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        if (paired) return;
        print($"Value: {value} and Index: {index}");
        if(gm)gm.showCard(index);
    }
    public void init(GameManager _gm)
    {
        mySpriteRenderer.sprite = back;
        print($"Initiated card {index}");
        gm = _gm;
    }
    public void foundPair()
    {
        paired = true;
    }
    public void show(bool b)
    {
        if (b) mySpriteRenderer.sprite = front;
        else StartCoroutine(hideCard());
    }
    IEnumerator hideCard()
    {
        if (paired) yield break;
        yield return new WaitForSeconds(1);
        mySpriteRenderer.sprite = back;
    }
}
