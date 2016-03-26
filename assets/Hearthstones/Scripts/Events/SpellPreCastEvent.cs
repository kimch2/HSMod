﻿public class SpellPreCastEvent
{
    private bool _willCast = true;

    public Hero Hero;
    public SpellCard Spell;
    // TODO : Target enemy (hero/minion) as the same type (interface IAttackable ?), could be null if notarget spell

    public void Cancel()
    {
        _willCast = false;
    }
}