using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sparkle idle sur toute la surface du bandeau (état Prêt !).
/// Images UI : le HUD Overlay masque les ParticleSystem monde.
/// </summary>
public static class SaleChannelUnlockableSparkleVfx
{
    private const string SparkleChildName = "UnlockableSparkle";
    private const int SparkleCount = 8;
    private const float MinSizePixels = 16f;
    private const float MaxSizePixels = 28f;
    private const float MinCycleSeconds = 0.7f;
    private const float MaxCycleSeconds = 1.4f;
    private const float MinPauseSeconds = 0.05f;
    private const float MaxPauseSeconds = 0.35f;

    private static readonly Color SparkleColor = new(1f, 0.95f, 0.7f, 0.9f);

    public static void StartOn(
        MonoBehaviour host,
        RectTransform surface,
        Sprite sprite,
        List<Coroutine> running)
    {
        if (host == null || surface == null || sprite == null || running == null)
            return;

        StretchToParent(surface);
        surface.SetAsFirstSibling();
        HideLegacyStaticSparkles(surface);
        EnsurePool(surface, sprite);

        for (int i = 0; i < surface.childCount; i++)
        {
            Transform child = surface.GetChild(i);
            if (child == null || child.name != SparkleChildName)
                continue;

            RectTransform rt = child as RectTransform;
            Image image = child.GetComponent<Image>();
            if (rt == null || image == null)
                continue;

            running.Add(host.StartCoroutine(LoopTwinkle(rt, image, surface)));
        }
    }

    public static void Stop(MonoBehaviour host, List<Coroutine> running, RectTransform surface)
    {
        if (host != null && running != null)
        {
            for (int i = 0; i < running.Count; i++)
            {
                if (running[i] != null)
                    host.StopCoroutine(running[i]);
            }

            running.Clear();
        }

        if (surface == null)
            return;

        for (int i = 0; i < surface.childCount; i++)
        {
            Transform child = surface.GetChild(i);
            if (child != null && child.name == SparkleChildName)
                child.gameObject.SetActive(false);
        }
    }

    private static IEnumerator LoopTwinkle(RectTransform rt, Image image, RectTransform surface)
    {
        while (rt != null && surface != null && surface.gameObject.activeInHierarchy)
        {
            yield return AnimateTwinkle(rt, image, surface.rect);
            yield return new WaitForSecondsRealtime(Random.Range(MinPauseSeconds, MaxPauseSeconds));
        }
    }

    private static IEnumerator AnimateTwinkle(RectTransform rt, Image image, Rect area)
    {
        float size = Random.Range(MinSizePixels, MaxSizePixels);
        rt.gameObject.SetActive(true);
        rt.sizeDelta = new Vector2(size, size);
        rt.anchoredPosition = new Vector2(
            Random.Range(area.xMin, area.xMax),
            Random.Range(area.yMin, area.yMax));
        rt.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 45f));

        float duration = Random.Range(MinCycleSeconds, MaxCycleSeconds);
        float elapsed = 0f;

        while (elapsed < duration && rt != null)
        {
            elapsed += Time.unscaledDeltaTime;
            float wave = Mathf.Sin(Mathf.Clamp01(elapsed / duration) * Mathf.PI);
            rt.localScale = Vector3.one * Mathf.Lerp(0.35f, 1f, wave);

            Color c = SparkleColor;
            c.a = SparkleColor.a * wave;
            image.color = c;
            yield return null;
        }

        if (rt != null)
            rt.gameObject.SetActive(false);
    }

    private static void StretchToParent(RectTransform surface)
    {
        surface.anchorMin = Vector2.zero;
        surface.anchorMax = Vector2.one;
        surface.pivot = new Vector2(0.5f, 0.5f);
        surface.offsetMin = Vector2.zero;
        surface.offsetMax = Vector2.zero;
        surface.localScale = Vector3.one;
    }

    private static void HideLegacyStaticSparkles(RectTransform surface)
    {
        for (int i = 0; i < surface.childCount; i++)
        {
            Transform child = surface.GetChild(i);
            if (child == null || child.name == SparkleChildName)
                continue;

            child.gameObject.SetActive(false);
        }
    }

    private static void EnsurePool(RectTransform surface, Sprite sprite)
    {
        int existing = 0;
        for (int i = 0; i < surface.childCount; i++)
        {
            if (surface.GetChild(i).name == SparkleChildName)
                existing++;
        }

        for (int i = existing; i < SparkleCount; i++)
        {
            var go = new GameObject(
                SparkleChildName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(surface, false);
            rt.localScale = Vector3.one;

            Image image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = SparkleColor;
            go.SetActive(false);
        }
    }
}
