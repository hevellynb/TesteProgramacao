using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // === SINGLETON ===
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // opcional: mantém entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Space(10)]
    private GameState m_gameState = GameState.IN_MAP;

    [SerializeField] private GameObject GameOverScreen; // TELA DE DERROTA
    [SerializeField] private GameObject VictoryScreen;  // TELA DE VITÓRIA

    [Header("Variáveis do Jogador")]
    [SerializeField] private float m_playerLife;     // VIDA ATUAL
    [SerializeField] private float m_playerMaxLife;  // VIDA MÁXIMA
    [SerializeField] private float m_playerMana;     // MANA ATUAL
    [SerializeField] private float m_playerMaxMana;  // MANA MÁXIMA

    [Header("Variáveis da UI do Jogador")]
    [SerializeField] private TMP_Text m_playerLifeText;
    [SerializeField] private TMP_Text m_playerManaText;

    [SerializeField] private Image m_playerHPBar; //UI DA BARRA DA VIDA DO JOGADOR
    [SerializeField] private Image m_playerManaBar; //UI DA BARRA DA MANA DO JOGADOR

    public GameState GameState => m_gameState;
    public float PlayerLife => m_playerLife;
    public float PlayerMana => m_playerMana;

    private bool m_doubleDamageNextAttack = false;

    private void Start()
    {
        m_playerLife = m_playerMaxLife;
        m_playerMana = m_playerMaxMana;
        AtualizaPlayerUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // TODO: implementar menu de pause
        }
    }

    // === UI ===
    public void AtualizaPlayerUI()
    {
        m_playerLifeText.text = m_playerLife.ToString();
        m_playerManaText.text = m_playerMana.ToString();

        if (m_playerHPBar != null)
            m_playerHPBar.fillAmount = m_playerLife / m_playerMaxLife;

        if (m_playerManaBar != null)
            m_playerManaBar.fillAmount = m_playerMana / m_playerMaxMana;
    }

    // === FLUXO DE JOGO ===
    public void StartGame()
    {
        m_gameState = GameState.IN_CHALLENGE;
        DeckManager.instance.ShuffleCards();
        DeckManager.instance.StartDeck();
        RoundManager.instance.ResetRounds();
    }

    public void GameOver()
    {
        OnLostGame();
    }

    public void GameWon()
    {
        OnWinGame();
    }

    private void OnWinGame()
    {
        m_gameState = GameState.CHALLENGE_WON;
        VictoryScreen.SetActive(true);
    }

    private void OnLostGame()
    {
        m_gameState = GameState.CHALLENGE_LOST;
        GameOverScreen.SetActive(true);
    }

    public void TryAgain()
    {
        GameOverScreen.gameObject.SetActive(false);
        StartGame();
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ChangeGameState(GameState newState)
    {
        m_gameState = newState;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // === VIDA / MANA ===
    public void TomarDano(float valor)
    {
        m_playerLife -= valor;
        if (m_playerLife <= 0)
        {
            m_playerLife = 0;
            GameOver();
        }
        AtualizaPlayerUI();
    }

    public void GanharMana(float valor)
    {
        m_playerMana = Mathf.Min(m_playerMana + valor, m_playerMaxMana);
        AtualizaPlayerUI();
    }

    public void ConsumirMana(float valor)
    {
        m_playerMana -= valor;
        if (m_playerMana < 0) m_playerMana = 0;
        AtualizaPlayerUI();
    }

    // === BUFFS ===
    public bool DoubleDamageNextAttack
    {
        get => m_doubleDamageNextAttack;
        set => m_doubleDamageNextAttack = value;
    }
}

public enum GameState
{
    IN_CHALLENGE,
    CHALLENGE_WON,
    CHALLENGE_LOST,
    IN_STORE,
    IN_MAP
}