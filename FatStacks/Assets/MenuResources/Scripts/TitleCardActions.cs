using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TitleCardActions : MonoBehaviour
{
    public GameObject MainMenu;
    public Image InMemory;
    public Image DinoDecaf;
    public Image Logo;
    public AmbiantTrack crickets;

    private float timer = 0f;
    private IEnumerator Start()
    {
        //Play ambiance
        MusicManager.singleton.PlayAmbiance(crickets);

        //Display InMemory
        InMemory.gameObject.SetActive(true);
        timer = 0f;
        yield return new WaitUntil(() => timer > 3f || Input.anyKey);
        yield return Fade(new CanvasRenderer[] { InMemory.canvasRenderer }, 1f, false);

        //Display Logo
        Logo.gameObject.SetActive(true);
        DinoDecaf.gameObject.SetActive(true);
        yield return Fade(new CanvasRenderer[] { Logo.canvasRenderer, DinoDecaf.canvasRenderer }, 1f, true);
        timer = 0f;
        yield return new WaitUntil(() => timer > 3f || Input.anyKey);
        yield return Fade(new CanvasRenderer[] { Logo.canvasRenderer, DinoDecaf.canvasRenderer }, 1f, false);

        MainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(CanvasRenderer[] canvasRenderers, float time, bool fadeIn = true)
    {
        for (float i = 100f; i > 0f; --i)
        {
            foreach(CanvasRenderer canvasRenderer in canvasRenderers)
            {
                canvasRenderer.SetAlpha(fadeIn ? i : i / 100f);
            }
            yield return new WaitForSeconds(time * 0.01f);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }
}
