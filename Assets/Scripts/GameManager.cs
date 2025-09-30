using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Space(10)]
    private GameState m_gameState = GameState.IN_MAP;

    [SerializeField]
    private GameObject GameOverScreen; //VARIAVEL PRIVADA DA TELA DE DERROTA
    [SerializeField]
    private GameObject VictoryScreen; // VARIÁVEL PRIVADA DA TELA DE VITÓRIA

    [Header("Variáveis do Jogador")]
    [SerializeField]
    private float m_playerLife; //A VIDA DO JOGADOR
    [SerializeField]
    private float m_playerMaxLife; //O MÁXIMO DE VIDA DO JOGADOR, OU A VIDA INICIAL
    [SerializeField]
    private float m_playerMana; //A MANA DO JOGADOR
    [SerializeField]
    private float m_playerMaxMana; //O MAXIMO DE MANA DO JOGADOR, OU A MANA INICIAL
    [Header("Variáveis da UI do Jogador")]
    [SerializeField]
    private TMP_Text m_playerLifeText; //UI DA VIDA DO JOGADOR
    [SerializeField]
    private TMP_Text m_playerManaText; //UI DA MANA DO JOGADOR

    public GameState GameState => m_gameState; //ESTADOS DO JOGO, SE PRECISAR
    public float PlayerLife => m_playerLife; //VARIAVEL PUBLICA DA VIDA DO JOGADOR
    public float PlayerMana => m_playerMana; //VARIAVEL PUBLIC DA MANA DO JOGADOR

    private void OnEnable()
    {

    }

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
            //O PAUSE OU MENU DE PAUSE VAI AQUI
        }
    }

    public void AtualizaPlayerUI()
    {
        m_playerLifeText.text = m_playerLife.ToString();
        m_playerManaText.text = m_playerMana.ToString();
    }

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

    public void OnWinGame()
    {
        m_gameState = GameState.CHALLENGE_WON;
        VictoryScreen.SetActive(true); // ATIVA A TELA DE VITÓRIA
    }

    private void OnLostGame()
    {
        m_gameState = GameState.CHALLENGE_LOST;
        GameOverScreen.SetActive(true); //ATIVA TELA DE DERROTA
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
        SceneManager.LoadScene("Menu"); //VOLTA PRA TELA DE MENU, MAS NÃO EXISTE TELA DE MENU AINDA
    }

    public void ChangeGameState(GameState newState)
    {
        m_gameState = newState;
    }

    public void QuitGame() //FECHA O JOGO
    {
        Application.Quit();
    }
}
public enum GameState //ESTADOS DO JOGO
{
    IN_CHALLENGE,
    CHALLENGE_WON,
    CHALLENGE_LOST,
    IN_STORE,
    IN_MAP
}