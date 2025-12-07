using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GAOptimizer : MonoBehaviour
{
    public AMover aMover;
    public Transform bTransform;
    public Button startButton;
    public int populationSize = 20;
    public int generations = 30;
    public float mutationRate = 0.1f;
    public float spaceSize = 10f;

    public ASequenceShooter shooter;
    private void Start()
    {
        startButton.onClick.AddListener(() => StartCoroutine(RunGA()));
    }

    IEnumerator RunGA()
    {
        int geneCount = 16 * 3; // 8回分の始点・終点 × xyz
        float[][] population = new float[populationSize][];
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = new float[geneCount];
            for (int j = 0; j < geneCount; j++)
                population[i][j] = Random.Range(-spaceSize / 2f, spaceSize / 2f);
        }

        for (int gen = 0; gen < generations; gen++)
        {
            float[] fitness = new float[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                GameManager.instance.ResetDamage();
                bTransform.GetComponent<BController>().ResetB();
                Vector3[] points = ConvertToVector3Array(population[i]);
                points = SnapPointsToWall(points);
                yield return StartCoroutine(shooter.FireSequence(points));
                //Debug.Log(GameManager.instance.totalDamage);
                fitness[i] = GameManager.instance.totalDamage;
            }

            // 次世代作成
            float[][] newPop = new float[populationSize][];
            for (int i = 0; i < populationSize; i++)
            {
                int p1 = Select(fitness);
                int p2 = Select(fitness);
                newPop[i] = Crossover(population[p1], population[p2]);
                Mutate(newPop[i]);
            }

            population = newPop;
            Debug.Log($"世代 {gen + 1} 最大ダメージ: {Max(fitness)}");
            yield return null;
        }

        // 最終結果
        int bestIdx = 0;
        float bestVal = 0f;
        for (int i = 0; i < populationSize; i++)
        {
            GameManager.instance.ResetDamage();
            bTransform.GetComponent<BController>().ResetB();
            Vector3[] points = ConvertToVector3Array(population[i]);
            points = SnapPointsToWall(points);
            yield return StartCoroutine(shooter.FireSequence(points));
            //Debug.Log(GameManager.instance.totalDamage);

            float dmg = GameManager.instance.totalDamage;
            if (dmg > bestVal)
            {
                bestVal = dmg;
                bestIdx = i;
            }
        }

        Debug.Log($"最適化結果 最大ダメージ: {bestVal}");
        Vector3[] bestPointsFinal = ConvertToVector3Array(population[bestIdx]);
        for (int i = 0; i < bestPointsFinal.Length; i++)
            Debug.Log($"Point {i}: {bestPointsFinal[i]}");

        // 最適解の軌道で実際に発射
        GameManager.instance.ResetDamage();
        bTransform.GetComponent<BController>().ResetB();
        yield return StartCoroutine(shooter.FireSequence(bestPointsFinal));
        Debug.Log($"再現ダメージ: {GameManager.instance.totalDamage}");
    }

    Vector3[] ConvertToVector3Array(float[] gene)
    {
        Vector3[] points = new Vector3[gene.Length / 3];
        for (int i = 0; i < points.Length; i++)
            points[i] = new Vector3(gene[i * 3], gene[i * 3 + 1], gene[i * 3 + 2]);
        return points;
    }

    int Select(float[] fitness)
    {
        float total = 0f;
        foreach (var f in fitness) total += f;
        float r = Random.value * total;
        float sum = 0f;
        for (int i = 0; i < fitness.Length; i++)
        {
            sum += fitness[i];
            if (sum >= r) return i;
        }
        return fitness.Length - 1;
    }

    float[] Crossover(float[] p1, float[] p2)
    {
        float[] child = new float[p1.Length];
        int point = Random.Range(0, p1.Length);
        for (int i = 0; i < p1.Length; i++)
            child[i] = (i < point) ? p1[i] : p2[i];
        return child;
    }

    void Mutate(float[] gene)
    {
        for (int i = 0; i < gene.Length; i++)
        {
            if (Random.value < mutationRate)
                gene[i] += Random.Range(-1f, 1f);
        }
    }

    float Max(float[] arr)
    {
        float m = arr[0];
        foreach (var v in arr) if (v > m) m = v;
        return m;
    }

    Vector3 SnapToCubeWall(Vector3 p, float halfSize)
    {
        // halfSize = 5 なら -5〜5 の立方体

        float absX = Mathf.Abs(p.x);
        float absY = Mathf.Abs(p.y);
        float absZ = Mathf.Abs(p.z);

        // 一番外側に寄ってる軸を使う → それが最も近い壁
        if (absX > absY && absX > absZ)
            return new Vector3(Mathf.Sign(p.x) * halfSize, p.y, p.z);
        if (absY > absX && absY > absZ)
            return new Vector3(p.x, Mathf.Sign(p.y) * halfSize, p.z);

        return new Vector3(p.x, p.y, Mathf.Sign(p.z) * halfSize);
    }
    Vector3[] SnapPointsToWall(Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
            points[i] = SnapToCubeWall(points[i], spaceSize / 2f);
        return points;
    }

}
