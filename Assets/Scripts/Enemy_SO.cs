using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_", menuName = "Astral/Enemy", order = 1)]
public class Enemy_SO : ScriptableObject //DADOS DOS INIMIGOS
{
    public int ID;
    public string Nome;
    public Nivel Nivel;
    public Sprite Sprite;
    public int Vida;
}

public enum Nivel
{
    FACIL,
    MEDIO,
    DIFICIL,
    BOSS
}
