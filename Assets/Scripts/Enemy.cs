using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Enemy_SO m_enemyInfo; //VARIÁVEL DAS INFORMAÇÕES DO INIMIGO
    [SerializeField]
    private Image m_enemySprite; //SPRITE DO INIMIGO
    [SerializeField]
    private float m_enemyLife; //VIDA DO INIMIGO
    [SerializeField]
    private Image m_enemyHPBar; //UI DA BARRA DA VIDA DO INIMIGO

    public Enemy_SO EnemyInfo => m_enemyInfo;
    public float EnemyLife => m_enemyLife;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //DEFINE AS VARIÁVEIS DO INIMIGO
    public void SetEnemy(Enemy_SO enemyInfo)
    {
        m_enemyInfo = enemyInfo;
        m_enemySprite.sprite = enemyInfo.Sprite;
        m_enemyLife = enemyInfo.Vida;
        AtualizaBarraVida();
    }

    public void TomarDano(int dano)
    {
        //IMPLEMENTE A FUNÇÃO DE TOMAR DANO
        m_enemyLife -= dano;
        AtualizaBarraVida();
    }

    public void AtualizaBarraVida()
    {
        m_enemyHPBar.fillAmount = m_enemyLife / m_enemyInfo.Vida;

        if (m_enemyLife <= 0)
        {
            GameManager.instance.GameWon();
        }
    }
}
