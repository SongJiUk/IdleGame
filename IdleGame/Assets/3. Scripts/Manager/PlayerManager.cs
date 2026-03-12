using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public void ExpUp()
    {
        float relicBouns = Managers.RelicM.GetPassiveBonus(Define.RelicName.GoldenRing.ToString());
        float smeltBonus = Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.Exp);
        double baseExp = Utils.Datas.levelData.Exp();

        float totalBonusPercent = (relicBouns + smeltBonus) / 100f;
        double finalExp = baseExp * (1.0f + totalBonusPercent);

        Managers.GameM.Exp += finalExp;

        if (Managers.GameM.Exp >= Utils.Datas.levelData.MaxExp())
        {
            double remainExp = Managers.GameM.Exp - Utils.Datas.levelData.MaxExp();
            LevelUp(remainExp);
        }
    }

    public float GetExpUpPercent()
    {
        float relicBouns = Managers.RelicM.GetPassiveBonus(Define.RelicName.GoldenRing.ToString());
        float smeltBonus = Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.Exp);

        float bonus = relicBouns + smeltBonus;

        return bonus;
    }

    public float ExpPercent()
    {
        float exp = (float)Utils.Datas.levelData.MaxExp();
        double myExp = Managers.GameM.Exp;

        return (float)myExp / exp;
    }

    public float NextExp()
    {
        float maxexp = (float)Utils.Datas.levelData.MaxExp();
        double myExp = (float)Utils.Datas.levelData.Exp();

        float relicBonus = Managers.RelicM.GetPassiveBonus(Define.RelicName.GoldenRing.ToString());
        float mul = 1.0f + (relicBonus / 100f);


        float exp = (float)myExp * mul;

        return (exp / maxexp) * 100.0f;
    }

    public double GetAttack(Define.Grade _grade, CharacterHolder _holder, CreatureController _owner = null)
    {
        var damage = Utils.Datas.levelData.Damage() * ((int)_grade + 1);
        float level = (float)_holder.holder.Level * 10 / (float)100;
        var realDamage = damage + (damage * level);

        float baseStatPercent = (float)Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.Damage) + (float)Managers.QuestM.Achievement_Status_Data.damage;

        float buffValue = 0f;
        if (_owner != null)
        {
            buffValue += _owner.GetBuffValue(Define.BuffEffectType.AttackEffect);
            buffValue -= _owner.GetBuffValue(Define.BuffEffectType.StatDownEffect);
        }



        realDamage *= 1.0 + ((baseStatPercent + buffValue) / 100);
        realDamage *= Managers.BuffM.GetAttackBuffMul();

        return realDamage;
    }
    public double GetDefense(Define.Grade _grade, CharacterHolder _holder, CreatureController _owner)
    {
        double baseDefense = (float)_holder.data.BaseDefense;
        float levelModifier = (float)_holder.holder.Level * 10 / (float)100;
        double realDefense = baseDefense + (baseDefense * levelModifier);

        float buffValue = 0f;
        if (_owner != null)
        {
            buffValue += _owner.GetBuffValue(Define.BuffEffectType.DefenseEffect);
            buffValue -= _owner.GetBuffValue(Define.BuffEffectType.StatDownEffect);
        }

        realDefense *= 1.0 + (buffValue / 100);

        return realDefense;
    }


    public double GetHP(Define.Grade _grade, CharacterHolder _holder)
    {
        var hp = Utils.Datas.levelData.HP() * ((int)_grade + 1);
        float level = (float)_holder.holder.Level * 10 / (float)100;
        var realHp = hp + hp * level;
        realHp *= 1.0f + (Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.HP) / 100f);

        realHp *= 1.0f + (Managers.QuestM.Achievement_Status_Data.hp / 100f);
        return realHp;
    }


    public void LevelUp(double _remainExp)
    {
        Managers.GameM.UpgradeCount++;
        Managers.GameM.gameData.damage += Utils.Datas.levelData.Damage();
        Managers.GameM.gameData.hp += Utils.Datas.levelData.HP();

        Managers.GameM.Level++;
        Managers.GPGSM.SaveScore(Managers.GameM.Level);
        Managers.GameM.Exp = _remainExp;
        Managers.QuestM.GetMission(Define.MissionTarget.LevelUp).Progress++;
        Managers.CharacterM.LevelUpPlayer();

        for (int i = 0; i < Managers.CharacterM.players.Length; i++)
        {
            if (Managers.CharacterM.players[i] == null) continue;

            Managers.CharacterM.players[i].InitStat();
        }

    }

    public double MainAttack()
    {
        double attack = GetAttack(Managers.GameM.mPlayer.DATA.CharacterGrade, Managers.GameM.gameData.Characters_Data[Managers.GameM.mPlayer.DATA.Name]);
        int value = 1;
        for (int i = 0; i < Managers.CharacterM.Characters.Length; i++)
        {
            if (Managers.CharacterM.Characters[i] != null)
            {
                var data = Managers.CharacterM.Characters[i].data;
                attack += GetAttack(data.CharacterGrade, Managers.GameM.gameData.Characters_Data[data.Name]);
                value++;
            }
        }

        return attack / value;
    }


    public double MainHP()
    {
        double hp = GetHP(Managers.GameM.mPlayer.DATA.CharacterGrade, Managers.GameM.gameData.Characters_Data[Managers.GameM.mPlayer.DATA.Name]);
        int value = 1;
        for (int i = 0; i < Managers.CharacterM.Characters.Length; i++)
        {
            if (Managers.CharacterM.Characters[i] != null)
            {
                var data = Managers.CharacterM.Characters[i].data;
                hp += GetHP(data.CharacterGrade, Managers.GameM.gameData.Characters_Data[data.Name]);
                value++;
            }
        }

        return hp / value;
    }



    public float GoldDrop()
    {
        float smeltPercent = Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.Money);
        float achievementPercent = (float)Managers.QuestM.Achievement_Status_Data.money;
        float relicBouns = Managers.RelicM.GetPassiveBonus(Define.RelicName.SilverRing.ToString());

        float statMul = 1f + (smeltPercent / 100f);
        float achievementMul = 1f + (achievementPercent / 100f);
        float relicMul = 1f + (relicBouns / 100f);


        float buffMul = Managers.BuffM.GetGoldBuffMul();

        return statMul * achievementMul * relicMul * buffMul;
    }

    public float ItemDrop()
    {
        float smelt = Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.Item);
        float achievement = (float)Managers.QuestM.Achievement_Status_Data.item;
        return smelt + achievement;
    }
    public float GetAttackSpeedBonusPercent()
    {
        float smelt = Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.AttackSpeed) * 0.01f;
        float achievement = (float)Managers.QuestM.Achievement_Status_Data.attackSpeed * 0.01f;

        return (smelt + achievement) * 100f;
    }

    public float AttackSpeed(float _baseSpeed = 0.75f)
    {
        float smelt = Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.AttackSpeed) * 0.01f;
        float achievement = (float)Managers.QuestM.Achievement_Status_Data.attackSpeed * 0.01f;

        float total = _baseSpeed * (1.0f + smelt + achievement);
        return Mathf.Clamp(total, 0.5f, 5.0f);
    }

    public float CriticalChance()
    {

        float baseValue = 20.0f;
        float smelt = Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.CriticalP);
        float achievement = (float)Managers.QuestM.Achievement_Status_Data.criticalP;

        float buff = Managers.BuffM.GetCriticalBuffMul();

        return baseValue + smelt + achievement + buff;
    }

    public float CriticalDamage()
    {
        float baseValue = 140.0f;
        float smelt = Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.CriticalD);
        float achievement = (float)Managers.QuestM.Achievement_Status_Data.criticalD;

        return baseValue + smelt + achievement;
    }

    public double AverageCombatPower()
    {
        double damage = MainAttack() + MainHP();

        return damage;
    }

}
