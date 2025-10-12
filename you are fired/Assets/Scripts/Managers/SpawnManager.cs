using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    private StageConfig cfg;
    private readonly List<Enemy> alive = new();

    public void Setup(StageConfig config)
    {
        cfg = config;
        alive.Clear();
        // ����������Ԥ�ȵ�һ�������� Update �ﰴʱ���ᴥ��
        //SpawnWave(0);
    }

    //public void SpawnWave(int waveIndex)
    //{
    //    var wave = cfg.waves[waveIndex];
    //    foreach (var item in wave.entries)
    //    {
    //        for (int i = 0; i < item.count; i++)
    //        {
    //            Vector3 pos = PickSpawnPoint(item.spawnArea);
    //            var e = Instantiate(item.enemyPrefab, pos, Quaternion.identity);
    //            alive.Add(e);
    //            e.OnDied += () => alive.Remove(e);
    //        }
    //    }
    //}

    public bool AllObjectivesCleared()
    {
        // Ҳ���Լ���Ƿ����Boss/���浽��ʱ��
        return alive.Count == 0 && /*����������������*/ true;
    }

    private Vector3 PickSpawnPoint(Rect area)
    {
        float x = Random.Range(area.xMin, area.xMax);
        float y = Random.Range(area.yMin, area.yMax);
        return new Vector3(x, y, 0);
    }
}

