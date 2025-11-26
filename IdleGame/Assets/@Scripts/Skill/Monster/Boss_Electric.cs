using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public class Boss_Electric : SkillBase
{

    CreatureController cc;
    public override void SetSkill(CreatureController _cc = null)
    {
        base.SetSkill();
        if (_cc != null) cc = _cc;
        Skills().Forget();
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

            player.GetDamage(10, cc);
            await UniTask.WaitForSeconds(0.2f);

        }
    }
}
