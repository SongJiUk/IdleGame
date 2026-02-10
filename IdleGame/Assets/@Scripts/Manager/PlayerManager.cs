using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public void ExpUp()
    {
        Managers.GameM.Exp += Utils.Datas.levelData.Exp();
        Managers.GameM.UpgradeCount++;
        Managers.GameM.gameData.damage += Utils.Datas.levelData.Damage();
        Managers.GameM.gameData.hp += Utils.Datas.levelData.HP();

        if (Managers.GameM.Exp >= Utils.Datas.levelData.MaxExp())
        {
            LevelUp();
        }

        for (int i = 0; i < Managers.CharacterM.players.Length; i++)
        {
            if (Managers.CharacterM.players[i] == null) continue;

            Managers.CharacterM.players[i].InitStat();
        }
    }

    public float ExpPercent()
    {
        float exp = (float)Utils.Datas.levelData.MaxExp();
        double myExp = Managers.GameM.Exp;

        return (float)myExp / exp;
    }

    public float NextExp()
    {
        float exp = (float)Utils.Datas.levelData.MaxExp();
        float myExp = (float)Utils.Datas.levelData.Exp();

        return (myExp / exp) * 100.0f;
    }

    public double GetAttack(Define.Grade _grade, CharacterHolder _holder)
    {
        var damage = Utils.Datas.levelData.Damage() * ((int)_grade + 1);
        float level = (float)_holder.holder.Level * 10 / (float)100;
        var realDamage = damage + damage * level;
        realDamage *= 1.0 + (Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.Damage) / 100f);

        realDamage *= 1.0 + (Managers.QuestM.Achievement_Status_Data.damage / 100f);
        realDamage *= Managers.BuffM.GetAttackBuffMul();

        return realDamage;
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
    public void LevelUp()
    {
        Managers.GameM.Level++;
        Managers.GameM.Exp = 0;
        Managers.QuestM.GetMission(Define.MissionTarget.LevelUp).Progress++;
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

        float statMul = 1f + (smeltPercent / 100f);
        float achievementMul = 1f + (achievementPercent / 100f);
        float buffMul = Managers.BuffM.GetGoldBuffMul();

        return statMul * achievementMul * buffMul;
    }

    public float ItemDrop()
    {
        float smelt = Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.Item);
        float achievement = (float)Managers.QuestM.Achievement_Status_Data.item;
        return smelt + achievement;
    }

    public float AttackSpeed()
    {
        float smelt = Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.AttackSpeed);
        float achievement = (float)Managers.QuestM.Achievement_Status_Data.attackSpeed;
        return 1.0f + (smelt / 100f) + (achievement / 100f);
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
