using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public abstract class SkillBase
{

    #region SKill Info
    protected Data.SkillData skill_Data;
    protected int skill_AttackCount = 0;
    protected float skill_Duration = 0f;
    protected float skill_Interval = 0f;
    protected float skill_DamageMul = 0f;
    protected float skill_CoolTime = 0f;
    protected float current_CoolTime = 0f;
    protected float skill_Radius = 0f;
    protected float skill_Length = 0f;
    protected float skill_Width = 0f;
    protected float anim_Duration = 0f;
    protected int skill_ProjectileID = 0;
    protected bool isSplash = false;
    #endregion

    protected List<ISkillEffect> effects = new();
    protected List<GameObject> activeVFXs = new List<GameObject>();
    public virtual bool UseSkill(CreatureController _caster, CreatureController _target) { return false; }


    protected void GetBuffFactory(string _name, float _duration, float _ratio, float _interval = 0, CreatureController _owner = null)
    {
        switch (_name)
        {
            case "AttackBuff":
                effects.Add(new BuffEffect((duration, ratio, interval, owner)
                => new AttackBuff(duration, ratio), _duration, _ratio, _interval, _owner));
                break;
            case "DefenseBuff":
                effects.Add(new BuffEffect((duration, ratio, interval, owner)
                => new DefenseBuff(duration, ratio), _duration, _ratio, _interval, _owner));
                break;
            case "DotBuff":
                effects.Add(new BuffEffect((duration, ratio, interval, owner)
                => new DotBuff(duration, ratio, interval, owner), _duration, _ratio, _interval, _owner));
                break;
            case "HealBuff":
                effects.Add(new BuffEffect((duration, ratio, interval, owner)
                => new HealBuff(duration, ratio, interval), _duration, _ratio, _interval, _owner));
                break;
            case "StatDownBuff":
                effects.Add(new BuffEffect((duration, ratio, interval, owner)
                => new StatDownBuff(duration, ratio), _duration, _ratio, _interval, _owner));
                break;
        }
    }


    public virtual void SetSkill(CreatureController _caster = null)
    {
        if (!Managers.DataM.SkillDataDic.TryGetValue(_caster.DATA.SkillDataID, out var skillData))
        {
            Debug.Log($"{_caster.name} : 스킬 데이터 없음");
            return;
        }

        foreach (var buff in skillData.BuffList_ID)
        {
            if (!Managers.DataM.BuffDataDic.TryGetValue(buff, out var effectData))
            {

                return;
            }

            string className = effectData.SkillEffectType;
            float duration = effectData.BuffDuration;
            float value = effectData.ValueRatio;
            float interval = effectData.Interval;

            if (className == "BuffEffect")
            {
                if (!Managers.DataM.BuffTypeDataDic.TryGetValue(effectData.BuffTypeID, out var buffType))
                {

                    return;
                }

                string name = buffType.BuffName;
                GetBuffFactory(name, duration, value, interval, _caster);

            }
            //DamageEffect부눈
            // else
            // {
            //     System.Type type = System.Type.GetType(className);

            //     if (type == null)
            //     {
            //         Debug.LogError($"[SkillBase] : Type not found for class Name. {className}");
            //         return;
            //     }
            //     ISkillEffect effectComponent = (ISkillEffect)System.Activator.CreateInstance(type, value, splashRadius);


            //     effects.Add(effectComponent);
            // }

        }
    }

    public async UniTask ResetSkillStateAsync(CreatureController _caster, float _duration)
    {
        try
        {
            await UniTask.WaitForSeconds(_duration);

            if (_caster != null && _caster.isUsingSkill)
            {
                _caster.isUsingSkill = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[ResetSkillStateAsync] : " + e.Message);
        }



        if (_caster.isUsingSkill)
        {
            _caster.isUsingSkill = false;
        }
    }

    public void ShowEffect(CreatureController _target)
    {
        if (_target == null) return;
        foreach (var data in skill_Data.TargetVFX_ID)
        {
            string vfxName = Utils.GetVfxPrefabName(data);
            if (!string.IsNullOrEmpty(vfxName))
            {
                var effect = Managers.ResourceM.Instantiate(vfxName, _pooling: true);
                if (effect == null) continue;

                effect.transform.position = _target.transform.position;
                if (_target != null)
                {
                    effect.transform.SetParent(_target.transform);
                    _target.vfxs.Add(effect);
                }
                else
                {
                    Managers.ResourceM.Destroy(effect);
                }

            }

        }
    }

    public void ClearSkillVFX()
    {
        if (Managers.ResourceM == null) return;

        foreach (var vfx in activeVFXs)
        {
            if (vfx != null)
            {
                vfx.transform.SetParent(null);
                Managers.ResourceM.Destroy(vfx);

            }
        }

        activeVFXs.Clear();
    }

    public void InitSkillData(CreatureController _caster)
    {
        if (Managers.DataM.SkillDataDic.TryGetValue(_caster.DATA.SkillDataID, out Data.SkillData skillData))
        {
            skill_Data = skillData;
            skill_AttackCount = skillData.SkillAttackCount;
            skill_Duration = skillData.SkillDuration;
            skill_Interval = skillData.SkillInterval;
            skill_DamageMul = skillData.SkillDamageMul;
            skill_CoolTime = skillData.SkillCoolTime;
            skill_Radius = skillData.SkillRadius;
            skill_Length = skillData.SkillLength;
            skill_Width = skillData.SkillWidth;
            anim_Duration = skillData.AnimDuration;
            skill_ProjectileID = skillData.SkillProjectileID;
            isSplash = skillData.IsSplash;
        }

    }
    #region 데미지 계산

    public void SetDamage(CreatureController _caster, CreatureController _target, float _delay = 0f)
    {
        if (_target == null) return;

        if (_delay > 0)
        {
            DelayDamage(_caster, _target, _delay).Forget();
        }
        else
        {
            InstantDamage(_caster, _target);
        }
    }


    public void InstantDamage(CreatureController _caster, CreatureController _target)
    {
        if (_target.IsDead) return;
        double baseDamage = _caster.Damage * skill_DamageMul;
        _target.GetDamage(baseDamage, _caster, _isSkill: true);

        if (isSplash)
        {
            SplashDamage(_caster, _target);
        }
    }
    public async UniTask DelayDamage(CreatureController _caster, CreatureController _target, float _delay)
    {
        await UniTask.WaitForSeconds(_delay);
        if (_target == null || _target.IsDead) return;

        InstantDamage(_caster, _target);
    }

    public void SplashDamage(CreatureController _caster, CreatureController _target)
    {
        List<MonsterController> mList = Managers.ObjectM.mcList;

        double splashDamage = _caster.Damage * skill_DamageMul / 3f;
        Vector3 centerPos = _target.transform.position;
        foreach (MonsterController monster in new List<CreatureController>(mList))
        {
            if (monster == null || monster.IsDead) continue;
            if (monster == _target) continue;

            Vector3 monsterPos = monster.transform.position;

            float dist = Vector3.Distance(centerPos, monsterPos);

            if (dist <= skill_Radius)
                monster.GetDamage(splashDamage, _caster, _isSkill: true);
        }
    }
    #endregion

    public void UpdateCoolTime(float _deltaTime)
    {
        if (current_CoolTime > 0.0f)
        {
            current_CoolTime -= _deltaTime;
            if (current_CoolTime < 0.0f)
            {
                current_CoolTime = 0f;
            }
        }
    }

    public bool IsUseSkill()
    {
        return current_CoolTime <= 0.0f;
    }

    public async UniTask LoopSkill(CreatureController _caster, CreatureController _target = null)
    {
        int totalAttack = skill_AttackCount;
        float interval = skill_Interval;

        for (int i = 0; i < totalAttack; i++)
        {
            if (_target != null)
            {
                CreatureController enemy = Utils.FindRandomPlayer(_caster);
                SetDamage(_caster, enemy);
                ShowEffect(enemy);
            }
            else
            {
                List<CreatureController> enemiesInArea = Utils.FindEnemyInSphereArea(_caster, skill_Radius);

                foreach (CreatureController enemy in enemiesInArea)
                {
                    SetDamage(_caster, enemy, 0.2f);
                }
            }


            if (interval > 0f) await UniTask.Delay((int)(interval * 1000));
        }
    }


}
