using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public abstract class SkillBase
{
    protected float skill_Duration = 0f;
    protected float anim_Duration = 0f;
    protected float attack_Radius = 0f;
    protected float attack_length = 0f;
    protected float attack_width = 0f;
    protected List<ISkillEffect> effects = new();
    protected List<GameObject> activeVFXs = new List<GameObject>();
    public virtual bool UseSkill(CreatureController _caster, CreatureController _target) { return false; }


    protected void GetBuffFactory(string _name, float _duration, float _ratio, float _radius = 0, float _interval = 0, CreatureController _owner = null)
    {
        switch (_name)
        {
            case "AttackBuff":
                effects.Add(new BuffEffect((duration, ratio, radius, interval, owner)
                => new AttackBuff(duration, ratio), _duration, _ratio, _radius, _interval, _owner));
                break;
            case "DefenseBuff":
                effects.Add(new BuffEffect((duration, ratio, radius, interval, owner)
                => new DefenseBuff(duration, ratio), _duration, _ratio, _radius, _interval, _owner));
                break;
            case "DotBuff":
                effects.Add(new BuffEffect((duration, ratio, radius, interval, owner)
                => new DotBuff(duration, ratio, interval, owner), _duration, _ratio, _radius, _interval, _owner));
                break;
            case "HealBuff":
                effects.Add(new BuffEffect((duration, ratio, radius, interval, owner)
                => new HealBuff(duration, ratio), _duration, _ratio, _radius, _interval, _owner));
                break;
            case "StatDownBuff":
                effects.Add(new BuffEffect((duration, ratio, radius, interval, owner)
                => new StatDownBuff(duration, ratio), _duration, _ratio, _radius, _interval, _owner));
                break;
            case "SpinAttackBuff":
                effects.Add(new BuffEffect((duration, ratio, radius, interval, owner)
                => new SpinAttackBuff(duration, ratio, radius, interval, owner), _duration, _ratio, _radius, _interval, _owner));
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
            if (!Managers.DataM.SkillEffectDataDic.TryGetValue(buff, out var effectData))
            {
                Debug.Log($"{_caster.name} : 해당 스킬 효과 없음 ]");
                return;
            }

            string className = effectData.SkillEffectType;
            float duration = effectData.SkillDuration;
            float value = effectData.ValueRatio;
            float splashRadius = effectData.Radius;
            float interval = effectData.Interval;

            if (className == "BuffEffect")
            {
                if (!Managers.DataM.BuffTypeDataDic.TryGetValue(effectData.BuffTypeID, out var buffType))
                {
                    Debug.LogError($"[SkillBase] : 버프 이펙트에 해당 번호가 없음");
                    return;
                }

                string name = buffType.BuffName;
                GetBuffFactory(name, duration, value, splashRadius, interval, _caster);

            }
            else
            {
                System.Type type = System.Type.GetType(className);

                if (type == null)
                {
                    Debug.LogError($"[SkillBase] : Type not found for class Name. {className}");
                    return;
                }
                ISkillEffect effectComponent = (ISkillEffect)System.Activator.CreateInstance(type, value, splashRadius);


                effects.Add(effectComponent);
            }

        }
    }

    public async UniTask ResetSkillStateAsync(CreatureController _caster, float _duration)
    {
        await UniTask.WaitForSeconds(_duration);

        if (_caster.isUsingSkill)
        {
            _caster.isUsingSkill = false;
        }
    }

    public void ShowEffect(CreatureController _randPlayer, Data.SkillData _skillData)
    {
        foreach (var data in _skillData.TargetVFX_ID)
        {
            string vfxName = Utils.GetVfxPrefabName(data);
            if (!string.IsNullOrEmpty(vfxName))
            {
                var effect = Managers.ResourceM.Instantiate(vfxName, _pooling: true);
                effect.transform.position = _randPlayer.transform.position;
                effect.transform.SetParent(_randPlayer.transform);
                _randPlayer.vfxs.Add(effect);
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


}
