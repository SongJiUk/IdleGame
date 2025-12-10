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

    public override bool UseSkill(CreatureController _caster, CreatureController _target)
    {
        InitSkillData(_caster);



        for(int i =0; i<skill_AttackCount; i++)
        {
            CreatureController enemy = Utils.FindRandomPlayer(_caster);
            if (enemy == null || enemy.IsDead) continue;

            if (enemy != null)
            {
                SetDamage(_caster, _target);
                ShowEffect(enemy);
            }
            else
            {
                Debug.Log("[Boss_Skill] : 유효한 적 없음");
                continue;
            }
        }

        return true;
    }

    async UniTask Skills()
    {

        players = Managers.SpawnM.players.ToList();
        if (players.Count == 0) return;
        for (int i = 0; i < 5; i++)
        {
            players = Managers.SpawnM.players.ToList();
            if (players.Count == 0) break;

            PlayerController player = players[Random.Range(0, players.Count)];

            if (player == null || player.IsDead) continue;

            var go = Managers.ResourceM.Instantiate("Boss_Electric", _pooling: true);
            go.transform.position = player.transform.position;

            await Managers.CameraM.CameraShake();

            if (player == null || player.IsDead) continue;

            player.GetDamage(cc.Damage * 1.2, cc);
            await UniTask.WaitForSeconds(0.2f);

        }
    }
}
