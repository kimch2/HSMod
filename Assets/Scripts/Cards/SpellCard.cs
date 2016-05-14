﻿public class SpellCard : BaseCard
{
    public TargetType TargetType;

    public void InitializeSpell()
    {
        InitializeCard();
    }
    
    public void OnCast(Character target)
    {
        SpellPreCastEvent spellPreCastEvent = EventManager.Instance.OnSpellPreCast(Player, this);

        if (spellPreCastEvent.Status != PreStatus.Cancelled)
        {
            Cast(target);
        }

        EventManager.Instance.OnSpellCasted(Player, this);
    }

    public virtual void Cast(Character target) { }

    public virtual bool CanTarget(Character target)
    {
        // The target is a Hero
        if (target.IsHero())
        {
            // The target is the own Hero
            if (Player.Hero == target.As<Hero>())
            {
                // True for AllCharacters
                return (TargetType == TargetType.AllCharacters || TargetType == TargetType.FriendlyCharacters);
            }

            // The target is the enemy Hero
            else
            {
                // True for AllCharacters and EnemyCharacters types
                return (TargetType == TargetType.AllCharacters || TargetType == TargetType.EnemyCharacters);
            }
        }

        // The target is a Minion
        else
        {
            // The target is friendly
            if (Player == target.As<MinionCard>().Player)
            {
                return (TargetType == TargetType.AllCharacters || TargetType == TargetType.AllMinions || TargetType == TargetType.FriendlyMinions);
            }

            // The target is enemy
            else
            {
                return (TargetType == TargetType.AllCharacters || TargetType == TargetType.AllMinions || TargetType == TargetType.EnemyMinions);
            }
        }
    }
}