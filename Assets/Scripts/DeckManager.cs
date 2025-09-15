using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;

    [SerializeField]
    private GameManager m_gameManager;

    [SerializeField]
    private int m_maxNumCardsPerDeck = 3;

    [SerializeField]
    private List<Card_SO> m_allCards = new List<Card_SO>();

    [Space(10)]
    [SerializeField]
    private Queue<Card_SO> m_queueOfCards = new Queue<Card_SO>();
    [SerializeField]
    private List<Card_SO> m_currentDeckSO = new List<Card_SO>();
    [SerializeField]
    private List<Card> m_currentDeck = new List<Card>();

    [SerializeField]
    private Transform m_bottomPanelTransform;
    [SerializeField]
    private Transform m_middlePanelTransform;
    [SerializeField]
    private Transform m_cardSpawnTransform;
    [SerializeField]
    private Transform m_cardDeckImageTransform;

    [SerializeField]
    private GameObject m_cardPrefab;
    [SerializeField]
    private GameObject m_deckCheckCardPrefab;

    [SerializeField]
    private bool m_canPickCards = true;


    [Space(10)]
    [SerializeField]
    private Button m_throwButton;
    [SerializeField]
    private Button m_discardButton;

    [SerializeField]
    private List<Card> m_selectedCardView = new List<Card>();
    [SerializeField]
    private bool m_isUsingCards = false;
    [SerializeField]
    private GameObject m_combatCanva;
    [SerializeField]
    private UnityEvent m_executeWhenShow;


    public List<Card> CurrentDeck => m_currentDeck;
    public List<Card> SelectedCards => m_currentDeck.FindAll(card => card.Selected);
    public bool CanPickCards => m_canPickCards;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        ShuffleCards();
    }

    public void ShuffleCards() //EMBARALHA AS CARTAS E AS COLOCA EM FILA
    {
        if (m_currentDeck.Count > 0)
        {
            for (int i = m_currentDeck.Count - 1; i >= 0; i--)
            {
                if (m_currentDeck[i] != null)
                {
                    m_currentDeck[i].DestroySelf();
                }
            }
            m_currentDeck.Clear();
            m_currentDeckSO.Clear();

            m_queueOfCards.Clear();
        }

        List<Card_SO> shuffledDeck = m_allCards.OrderBy(x => Random.value).ToList();

        foreach (Card_SO card in shuffledDeck)
        {
            m_queueOfCards.Enqueue(card);
        }
    }



    private void Update()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        StartDeck();
    }

    public void StartDeck() //MONTA O DECK INICIAL DO JOGADOR
    {
        if (!m_combatCanva.activeSelf)
        {
            m_executeWhenShow.AddListener(() => DrawMoreCards());
        }
        else
        {
            DrawMoreCards();
        }
    }

    private Card_SO GetNextCard()
    {
        if (m_queueOfCards.Count == 0)
        {
            m_gameManager.GameOver();
        }
        return m_queueOfCards.Dequeue();
    }

    private IEnumerator ShowNewCard(List<Card_SO> cardsToShow) //EXIBE UMA NOVA CARTA AO JOGADOR, ADICIONANDO A SEU DECK
    {
        foreach (Card_SO card in cardsToShow)
        {
            Card newCard = ((GameObject)Instantiate(m_cardPrefab, m_cardDeckImageTransform, instantiateInWorldSpace: true)).GetComponent<Card>();
            newCard.SetCard(card);
            m_currentDeck.Add(newCard);
            newCard.transform.SetParent(m_bottomPanelTransform);
            yield return new WaitForEndOfFrame();
            newCard.transform.localScale = new Vector3(1, 1, 1);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ThrowCurrentHand() //JOGA AS CARTAS SELECIONADAS
    {
        List<Card> selectedCards = m_currentDeck.FindAll(card => card.Selected);
        m_selectedCardView = selectedCards;
        StartCoroutine(MoveCurrentHand(selectedCards));
        m_isUsingCards = true;
        m_throwButton.interactable = false;
        m_discardButton.interactable = false;
        SelectCardUI();
    }

    private IEnumerator MoveCurrentHand(List<Card> selectedCards) //MOVIMENTA AS CARTAS JOGADAS
    {
        m_canPickCards = false;
        for (int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].UseCard();
            selectedCards[i].transform.SetParent(m_middlePanelTransform);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].FazAcao();//CHAMA A AÇÃO DA CARTA -> DENTRO DESSA FUNÇÃO, ESTÁ O FUNCIONAMENTO DE CADA CARTA
            yield return new WaitForSeconds(0.7f);
        }
        yield return new WaitForSeconds(0.3f * selectedCards.Count);

        yield return new WaitForSeconds(1f);
        RoundManager.instance.FinishRound();

        StartCoroutine(DestroyThrownCards(selectedCards));
    }

    private IEnumerator DestroyThrownCards(List<Card> selectedCards, bool drawMore = true) //DESTROI AS CARTAS JOGADAS
    {
        for (int i = selectedCards.Count - 1; i >= 0; i--)
        {
            m_currentDeckSO.RemoveAll(card => card == selectedCards[i].CardInfo);
            selectedCards[i].MoveCard(selectedCards[i].transform.position + new Vector3(-5, 0, 0), null, 0.3f, null);
            yield return new WaitForSeconds(0.0f);
        }

        DiscardSelectedCards(selectedCards, drawMore);//NO FUNDO,NADA MAIS É DO QUE DESCARTAS CARTAS DEPOIS DE USÁ-LAS
    }

    public void DiscardSelectedCards(List<Card> selectedCards, bool drawMore = true) //DESCARTA AS CARTAS SELECIONADAS, COM A OPÇÃO DE TRAZER MAIS CARTAS PARA A MÃO DO JOGADOR
    {
        m_selectedCardView = selectedCards;
        if (drawMore)
        {
            m_currentDeck.RemoveAll(card => card == selectedCards.Find(x => x == card));
        }
        Coroutiner.WaitFrames(1, () => m_currentDeck.RemoveAll(card => card.Selected));

        StartCoroutine(DestroyDiscartedCards(selectedCards, drawMore));
        m_isUsingCards = false;
        EventSystem.current.SetSelectedGameObject(m_throwButton.gameObject, null);

    }

    public void DiscardSelectedCards() //DESCARTA AS CARTAS SELECIONADAS AO CLICAR NO BOTÃO DE DESCARTAS
    {
        List<Card> selectedCards = m_currentDeck.FindAll(card => card.Selected);
        m_selectedCardView = selectedCards;
        m_currentDeck.RemoveAll(card => card.Selected);
        Coroutiner.WaitFrames(1, () => m_currentDeck.RemoveAll(card => card.Selected));

        StartCoroutine(DestroyDiscartedCards(selectedCards));
    }


    private IEnumerator DestroyDiscartedCards(List<Card> selectedCards, bool drawMore = true) //DESTROI AS CARTAS DESCARTADAS
    {
        for (int i = selectedCards.Count - 1; i >= 0; i--)
        {
            m_currentDeckSO.RemoveAll(card => card.ID == selectedCards[i].CardID);
            if (selectedCards[i] != null)
            {
                selectedCards[i].MoveCard(selectedCards[i].transform.position + new Vector3(0, -5, 0), null, 0.3f, null);
            }
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = selectedCards.Count - 1; i >= 0; i--)
        {
            selectedCards[i].gameObject.SetActive(false);
            selectedCards[i].DestroySelf();
        }
        if (drawMore)
        {
            DrawMoreCards();
        }
        m_canPickCards = true;
        SelectCardUI();
    }

    public void DrawMoreCards() //PUXA MAIS CARTAS PARA O JOGADOR, APÓS USAR UMA OU MAIS CARTAS
    {
        m_canPickCards = false;
        List<Card_SO> cardToShow = new List<Card_SO>();
        bool isPartial = false;

        isPartial = m_currentDeckSO.Count != 0;

        for (int i = m_currentDeckSO.Count; i < m_maxNumCardsPerDeck; i++)
        {
            Card_SO nextCard = GetNextCard();
            cardToShow.Add(nextCard);
            m_currentDeckSO.Add(nextCard);
        }

        if (isPartial)
        {
            StartCoroutine(ShowNewCard(cardToShow));
        }
        else
        {
            cardToShow = m_currentDeckSO;
            StartCoroutine(ShowNewCard(m_currentDeckSO));
        }
        m_canPickCards = true;
    }

    public Card_SO GetCardInfoByID(int id) //BUSCA UMA CARTA PELO ID DENTRO DE TODAS AS CARTAS POSSÍVEIS
    {
        return m_allCards.Find(x => x.ID == id.ToString());
    }

    public List<Card_SO> GetAllCardsInfo() //RETORNA TODAS AS CARTAS POSSÍVEIS
    {
        return m_allCards;
    }

    public void SelectCardUI()
    {
        if (m_currentDeck != null && m_currentDeck.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(m_currentDeck[0].gameObject);
        }
    }

    public void SetDeck(List<Card_SO> newDeck)
    {
        m_allCards = newDeck;
    }
}
