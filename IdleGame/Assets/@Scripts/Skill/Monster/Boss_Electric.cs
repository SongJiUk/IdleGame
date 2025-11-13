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
        for(int i =0; i<5; i++)
        {
            PlayerController player = players[Random.Range(0, players.Count)];
            var go = Managers.ResourceM.Instantiate("Boss_Electric",_pooling: true);
            go.transform.position = player.transform.position;

            player.GetDamage(10, cc);
            await UniTask.WaitForSeconds(0.2f);
            
        }
    }
}
