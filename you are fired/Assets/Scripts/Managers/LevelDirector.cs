using System.Collections;
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    [SerializeField] private LevelConfig[] campaign;

    [Header("Services")]
    [SerializeField] private MonoBehaviour cutsceneSvcObj;   // �� CutsceneManager
    [SerializeField] private MonoBehaviour envBuilderObj;    // �� EnvironmentManager
    [SerializeField] private MonoBehaviour spawnerObj;       // �� SpawnManager
    [SerializeField] private MonoBehaviour buildSvcObj;      // �� BuildManager

    ICutsceneService cutscene;
    IEnvironmentBuilder env;
    ISpawner spawner;
    IBuildService build;

    Player player;

    private void Awake()
    {
        cutscene = (ICutsceneService)cutsceneSvcObj;
        env = (IEnvironmentBuilder)envBuilderObj;
        spawner = (ISpawner)spawnerObj;
        build = (IBuildService)buildSvcObj;
    }

    private void Start()
    {
        player = FindObjectOfType<Player>() ?? new GameObject("Player").AddComponent<Player>();
        StartCoroutine(RunCampaign());
    }

    private IEnumerator RunCampaign()
    {
        for (int i = 0; i < campaign.Length;)
        {
            var cur = campaign[i];
            var next = (i + 1 < campaign.Length) ? campaign[i + 1] : null;

            // 1) ��ǰ����
            yield return cutscene.PlaySequence(cur.introCutscenes);

            // 2) ���Ĺ�����ѡװ����
            yield return build.DoCoreBuild(cur, player);

            // 3) �������� + ˢ�ֳ�ʼ��
            env.Build(cur);
            spawner.Init(cur, player);
            build.ResetSession(cur, player);

            // ���ľ����¼� �� С������ֵ�ж�
            spawner.OnKillExp += exp => build.OnGainExp(exp, cur, player);

            // 4) �ؿ�ѭ��
            bool fail = false, clear = false;
            float t = 0f;
            while (true)
            {
                t += Time.deltaTime;
                spawner.Tick(Time.deltaTime);

                // С������ build.OnGainExp �ڲ�������Ҳ������������ build ��ѯ��ֵ������
                // ʧ��/ʤ���ж�
                if (player == null || player.IsDead) { fail = true; break; }
                if (spawner.AllWavesDispatched && spawner.AllCleared) { clear = true; break; }

                yield return null;
            }

            spawner.OnKillExp -= exp => build.OnGainExp(exp, cur, player);

            if (fail)
            {
                // ʧ�ܾ��� �� �����˵�������
                yield return cutscene.PlaySequence(cur.failCutscenes);
                env.Clear();
                yield break;
            }

            // ͨ��
            if (cur.isFinalStage || next == null)
            {
                yield return cutscene.PlaySequence(cur.outroCutscenes);
                env.Clear();
                yield break;
            }

            // �г�����β���飨�ɲ��м�����һ�أ�����������ֱ�Ӹ��ü��ɣ�
            yield return cutscene.PlaySequence(cur.outroCutscenes);

            // ��һ�ؿ������� + ���Ĺ���
            yield return cutscene.PlaySequence(next.introCutscenes);
            yield return build.DoCoreBuild(next, player);

            i++;
        }
    }
}
