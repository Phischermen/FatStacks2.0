using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LogEntry : MonoBehaviour
{
    // Start is called before the first frame update
    public IEnumerator Start()
    {
        Text text = GetComponent<Text>();
        yield return new WaitForSeconds(2f);
        float alpha = 1;
        for(float i = 100; i > 0; --i)
        {
            alpha = i / 100f;
            text.canvasRenderer.SetAlpha(alpha);
            yield return new WaitForSeconds(0.01f);
        }
        ItemLog.singleton.log.RemoveLast();
        Destroy(gameObject);
    }

}
