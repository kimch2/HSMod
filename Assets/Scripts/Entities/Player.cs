﻿using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Hero Hero;

    public Player Enemy;

    public List<BaseCard> Hand = new List<BaseCard>();
    public List<BaseCard> Deck = new List<BaseCard>();
    public List<Minion> Minions = new List<Minion>(7);
    public List<SpellCard> Secrets = new List<SpellCard>();
    public WeaponCard Weapon = null;

    public HeroController HeroController;
    public HandController HandController;
    
    public List<SpellCard> UsedSpells = new List<SpellCard>();
    public List<MinionCard> DeadMinions = new List<MinionCard>();

    public Vector3 HeroPosition;
    public Vector3 HandPosition;
    
    public int MaxCardsInHand = 10;
    public int MaxCardsInDeck = 60;

    public int MaximumMana = 10;
    public int TurnMana = 0;
    public int OverloadedMana = 0;
    public int AvailableMana = 0;

    public int Fatigue = 0;

    #region Constructor

    private Player() { }

    public static Player Create(HeroClass heroClass, Vector3 heroPosition, Vector3 handPosition)
    {
        GameObject playerObject = new GameObject("Player_" + heroClass.Name());
        
        Player player = playerObject.AddComponent<Player>();

        player.Hero = new Hero()
        {
            Player = player,
            Class = heroClass,
            HeroPower = new RaiseGhoul(player.Hero)
        };

        player.HeroPosition = heroPosition;
        player.HeroController = HeroController.Create(player.Hero, heroPosition);

        player.HandPosition = handPosition;
        player.HandController = HandController.Create(player, handPosition);

        return player;
    }

    #endregion

    #region Methods

    public void ReplaceHero(Hero newHero)
    {
        // TODO
    }

    public void RefillMana()
    {
        AvailableMana = TurnMana - OverloadedMana;
        OverloadedMana = 0;
    }

    public List<BaseCard> Draw(int draws)
    {
        List<BaseCard> drawnCards = new List<BaseCard>();

        for (int i = 0; i < draws; i++)
        {
            BaseCard drawnCard = Draw();

            if (drawnCard != null)
            {
                drawnCards.Add(drawnCard);
            }
        }

        return drawnCards;
    }

    public BaseCard Draw()
    {
        if (Deck.Count > 0)
        {
            if (Hand.Count < MaxCardsInHand)
            {
                // Getting the first card in the Deck
                BaseCard drawnBaseCard = Deck[0];

                // Moving the card to the Hand
                Hand.Add(drawnBaseCard);
                Deck.Remove(drawnBaseCard);

                // Creating the visual controller for the card
                CardController drawnCardController = CardController.Create(drawnBaseCard);
                drawnBaseCard.Controller = drawnCardController;

                HandController.Add(drawnCardController);

                // Firing OnDrawn events
                drawnBaseCard.OnDrawn();
                EventManager.Instance.OnCardDrawn(this, drawnBaseCard);
                
                return drawnBaseCard;
            }
            else
            {
                // TODO : Discard the card

                return null;
            }
        }
        else
        {
            Fatigue++;

            Hero.TryDamage(null, Fatigue);

            return null;
        }
    }

    public void EquipWeapon(WeaponCard weapon)
    {
        DestroyWeapon();

        Weapon = weapon;

        Weapon.Battlecry();
    }

    public void DestroyWeapon()
    {
        if (Weapon != null)
        {
            Weapon.Deathrattle();

            // TODO : Animation

            Weapon = null;
        }
    }

    public void UpdateGlows()
    {
        ResetGlows();

        if (HasWeapon() || Hero.CurrentAttack > 0)
        {
            switch (Hero.CurrentTurnAttacks)
            {
                case 0:
                    HeroController.SetGreenRenderer(true);
                    break;

                case 1:
                    if (Weapon.Windfury)
                    {
                        HeroController.SetGreenRenderer(true);
                    }
                    break;
            }
        }

        foreach (BaseCard card in Hand)
        {
            if (card.CurrentCost <= AvailableMana)
            {
                card.Controller.SetGreenRenderer(true);
            }
        }

        foreach (Minion minion in Minions)
        {
            if (minion.IsFrozen == false && minion.IsSleeping == false)
            {
                if (minion.CanAttack())
                {
                    switch (minion.CurrentTurnAttacks)
                    {
                        case 0:
                            minion.Controller.SetGreenRenderer(true);
                            break;

                        case 1:
                            if (minion.HasWindfury)
                            {
                                minion.Controller.SetGreenRenderer(true);
                            }
                            break;
                    }
                }
            }
        }
    }

    public void ResetGlows()
    {
        HeroController.SetGreenRenderer(false);

        foreach (BaseCard card in Hand)
        {
            card.Controller.SetGreenRenderer(false);
        }

        foreach (Minion minion in Minions)
        {
            minion.Controller.SetGreenRenderer(false);
        }
    }

    public void UpdateAll()
    {
        HeroController.UpdateSprites();
        HeroController.UpdateNumbers();

        if (HasWeapon())
        {
            Weapon.Controller.UpdateSprites();
            Weapon.Controller.UpdateNumbers();
        }
    }

    #endregion

    #region Getter Methods
    
    public int GetSpellPower()
    {
        int spellPower = 0;

        foreach (Minion minion in Minions)
        {
            spellPower += minion.SpellPower;
        }

        return spellPower;
    }

    public int GetManaUsedThisTurn()
    {
        return TurnMana - OverloadedMana - AvailableMana;
    }

    #endregion

    #region Condition Checkers

    public bool HasWeapon()
    {
        return (Weapon != null);
    }

    public bool HasMinions()
    {
        return (Minions.Count > 0);
    }

    public bool HasTauntMinions()
    {
        foreach (Minion minion in Minions)
        {
            if (minion.HasTaunt)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}