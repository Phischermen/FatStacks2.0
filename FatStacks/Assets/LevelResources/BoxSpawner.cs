using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public Transform boxGrid;
    public Box[] pool;
    public float maxInterval;
    public float minInterval;
    [Tooltip("If turned on, will use minInterval as rate. If off will use range between maxInterval and minInterval.")]
    public bool fixedRate;
    public bool startActive;
    protected bool on;
    public int amount = 1;
    public bool isFinite;
    public int seed;

    protected Coroutine coroutine;
    protected System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random(seed);
        TurnSpawnerOn(startActive);
    }

    public void TurnSpawnerOn(bool _on = true)
    {
        if(on != _on)
        {
            on = _on;
            if (on)
            {
                coroutine = StartCoroutine("SpawnBox");
            }
        }
    }

    protected virtual IEnumerator SpawnBox()
    {
        while (on && amount > 0)
        {
            GameObject obj = Instantiate(pool[random.Next(pool.Length)].gameObject, transform.position, Quaternion.identity, boxGrid);
            obj.SetActive(true);
            if (isFinite)
                amount -= 1;
            if (fixedRate)
                yield return new WaitForSeconds(minInterval);
            else
                yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }
        StopCoroutine(coroutine);
    }
}
