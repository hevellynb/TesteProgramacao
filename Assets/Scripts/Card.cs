using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    private float m_animSpeed = 0.7f;
    private Color m_stunnedColor = new Color(0.6f, 0.3f, 0.3f, 1);
    private Color m_alfaColor = new Color(0, 0, 0, 0);

    [SerializeField]
    protected string m_cardID;
    [SerializeField]
    private Card_SO m_cardInfo;
    [SerializeField]
    protected GameObject m_cardVisual;
    [SerializeField]
    protected Image m_cardSprite;
    [SerializeField]
    protected TMP_Text m_cardDescription;
    [SerializeField]
    protected GameObject m_hoverSprite;
    [SerializeField]
    protected Image m_glowSprite;
    [SerializeField]
    protected bool m_isCardCheck = false;

    [Header("States")]
    [SerializeField]
    private bool m_hovering = false;
    [SerializeField]
    protected bool m_selected = false;
    [SerializeField]
    protected bool m_used = false;
    [SerializeField]
    private bool m_stunned = false;

    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card> PointerClickEvent;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;

    public string CardID => m_cardID;
    public Card_SO CardInfo => m_cardInfo;
    public bool IsCardCheck => m_isCardCheck;
    public bool Selected => m_selected;
    public bool Stunned => m_stunned;
    public bool Hovering => m_hovering;
    public bool Used => m_used;

    public void SetCard(Card_SO cardInfo)
    {
        m_cardID = cardInfo.ID;
        m_cardSprite.sprite = cardInfo.Sprite;
        m_cardInfo = cardInfo;
        m_cardDescription.text = cardInfo.Descricao;
    }

    public void MoveCard(Vector2 newPosition, Action onComplete, float newAnimSpeed = 0, Transform newParent = null)
    {
        float currentAnimSpeed = 0;

        if (newAnimSpeed > 0)
        {
            currentAnimSpeed = newAnimSpeed;
        }
        else
        {
            currentAnimSpeed = m_animSpeed;
        }

        if (transform != null)
        {
            transform.DOMove(newPosition, currentAnimSpeed).SetEase(Ease.OutCubic).OnComplete(() => transform.SetParent(newParent)).OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            transform.DOMove(newPosition, currentAnimSpeed).SetEase(Ease.OutCubic).OnComplete(() => onComplete?.Invoke());
        }
    }

    public virtual void FazAcao()
    {
        if (CardInfo.Custo > 0)
        {
            switch (CardInfo.TipoCusto)
            {
                case TipoCusto.MANA:
                    if (GameManager.instance.PlayerMana >= CardInfo.Custo)
                    {
                        GameManager.instance.ConsumirMana(CardInfo.Custo);
                    }
                    else
                    {
                        Debug.Log("Mana insuficiente!");
                        return;
                    }
                    break;

                case TipoCusto.VIDA:
                    GameManager.instance.TomarDano(CardInfo.Custo);
                    break;
            }
        }

        switch (CardInfo.ID)
        {
            case "estocada":
                int dano = CardInfo.Pontos;
                if (GameManager.instance.DoubleDamageNextAttack)
                {
                    dano *= 2;
                    GameManager.instance.DoubleDamageNextAttack = false;
                }
                RoundManager.instance.CurrentEnemy.TomarDano(dano);
                break;

            case "hemorragia":
                RoundManager.instance.CurrentEnemy.TomarDano(CardInfo.Pontos);
                break;

            case "passo":
                GameManager.instance.DoubleDamageNextAttack = true;
                Debug.Log("Proximo golpe tera dano dobrado!");
                break;

            case "salto":
                break;

            case "frasco":
                GameManager.instance.GanharMana(CardInfo.Pontos);
                break;
        }

        if (CardInfo.Tipo == Tipo.GOLPE)
        {
            DoAttackAnimation(m_cardVisual.transform);
        }
    }

    protected void DoScaleAnimation(Transform target)
    {
        target.transform.DOScale(new Vector3(2, 2, 2), 0.2f).OnComplete(() => target.transform.DOScale(new Vector3(1, 1, 1), 0.2f));

    }
    private void DoAttackAnimation(Transform target)
    {
        target.transform.DOMoveX(transform.position.x + 2f, 0.3f).SetEase(Ease.InOutCirc).OnComplete(() => target.transform.DOMoveX(transform.position.x - 1f, 0.2f));
    }

    private void Update()
    {
        if (!m_selected)
        {
            if (DeckManager.instance != null)
            {
                if (DeckManager.instance.CanPickCards)
                {
                    m_cardSprite.color = Color.white;
                }
                else if (!m_isCardCheck)
                {
                    m_cardSprite.color = Color.gray;
                }
            }
        }

        if (m_stunned)
        {
            m_cardSprite.color = m_stunnedColor;
        }

    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (m_used)
        {
            return;
        }

        if (!DeckManager.instance) return;

        if (DeckManager.instance.CanPickCards && !m_isCardCheck)
        {
            PointerClickEvent.Invoke(this);
            m_selected = !m_selected;
            if (m_selected)
            {
                m_cardVisual.transform.DOMoveY(transform.position.y + 0.2f, 0.3f).SetEase(Ease.OutCubic);
            }
            else
            {
                m_cardVisual.transform.DOMoveY(transform.position.y - 0.2f, 0.3f).SetEase(Ease.OutCubic);
            }
        }
        else
        {
            if (m_selected)
            {
                PointerClickEvent.Invoke(this);
                m_selected = !m_selected;
                if (m_selected)
                {
                    m_cardVisual.transform.DOMoveY(transform.position.y + 0.2f, 0.3f).SetEase(Ease.OutCubic);
                }
                else
                {
                    m_cardVisual.transform.DOMoveY(transform.position.y - 0.2f, 0.3f).SetEase(Ease.OutCubic);
                }
            }
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (m_used)
        {
            return;
        }

        PointerEnterEvent.Invoke(this);

        if (!m_isCardCheck)
        {
            m_hoverSprite.SetActive(true);
        }
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        m_hoverSprite.SetActive(false);
        PointerExitEvent.Invoke(this);
    }

    public void Stun()
    {
        m_stunned = true;
        m_cardSprite.color = new Color(0.6f, 0.3f, 0.3f, 1);
    }
    public void ClearStun()
    {
        m_stunned = false;
        m_cardSprite.color = Color.white;
        m_cardSprite.transform.rotation = Quaternion.identity;
    }

    public void UseCard()
    {
        m_used = true;
    }

    public int ParentIndex()
    {
        int index = DeckManager.instance.CurrentDeck.FindIndex(card => card == this);
        return index;
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        OnPointerEnter(null);
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        OnPointerExit(null);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        OnPointerClick(null);
    }

    public void SetCardCheck()
    {
        m_isCardCheck = true;
    }
}

