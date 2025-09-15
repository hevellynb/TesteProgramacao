using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Card_A-1", menuName = "Astral/Card", order = 1)]
public class Card_SO : ScriptableObject //DADOS DAS CARTAS
{
    public string ID;
    public string Descricao;
    public Tipo Tipo;
    public Sprite Sprite;
    public int Pontos;
    public int Custo;
    public TipoCusto TipoCusto;
}

public enum Tipo
{
    ITEM,
    MOVIMENTO,
    GOLPE
}
public enum TipoCusto
{
    MANA,
    VIDA
}