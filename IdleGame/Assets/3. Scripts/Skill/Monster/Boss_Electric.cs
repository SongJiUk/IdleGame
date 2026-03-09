using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public class Boss_Electric : SkillBase
{

    CreatureController cc;
    List<PlayerController> players = new List<PlayerController>();

    public Boss_Electric() { }

    public override async UniTask<bool> UseSkill(CreatureController _caster, CreatureController _target)
    {

        if (!IsUseSkill())
        {
            Debug.Log("쿨타임");
            return false;
        }

        InitSkillData(_caster);

        CreatureController enemy = Utils.FindRandomPlayer(_caster);
        if (enemy == null || enemy.IsDead) return false;

        if (enemy != null)
        {
            LoopSkill(_caster, enemy).Forget();
            current_CoolTime = skill_CoolTime;
        }
        else
        {
            Debug.Log("[Boss_Skill] : 유효한 적 없음");
            return false;
        }

        return true;
    }

    async UniTask Skills()
    {

        players = Managers.CharacterM.AlivePlayers;
        if (players.Count == 0) return;

        for (int i = 0; i < 5; i++)
        {
            players = Managers.CharacterM.AlivePlayers;
            if (players.Count == 0) break;

            PlayerController player = players[Random.Range(0, players.Count)];

            if (player == null || player.IsDead) continue;

            var go = Managers.ResourceM.Instantiate("Boss_Electric", _pooling: true);
            go.transform.position = player.transform.position;
            

            await Managers.CameraM.CameraShake();

            if (player != null && !player.IsDead)
            {

                player.GetDamage(cc.Damage * 1.2, cc);
            }
            
            await UniTask.WaitForSeconds(0.2f);

        }
    }
}
