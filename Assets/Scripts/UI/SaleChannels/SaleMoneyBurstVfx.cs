using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Feedback monnaie vente : burst pièces + billets au-dessus du HUD Overlay.
/// Prefab Bezy <c>SaleMoneyBurst</c> = ref ParticleSystem (Simulate / Phase 3 sprites) ;
/// ancre runtime = bouton Confirmer/Valider du <c>ShopItemPopup</c> ;
/// runtime = Images UI (Overlay masque les PS monde).
/// </summary>
public static class SaleMoneyBurstVfx
{
    private const int DefaultCoinCount = 10;
    private const int DefaultBillCount = 4;
    private const float DefaultDestroyDelaySeconds = 1.5f;
    private const int OverlaySortingOrderBoost = 50;
    private const float CoinSizePixels = 28f;
    private const float BillWidthPixels = 44f;
    private const float BillHeightPixels = 24f;

    private static readonly Color CoinColor = new(1f, 0.835f, 0.29f, 1f);
    private static readonly Color BillColor = new(0.482f, 0.776f, 0.494f, 1f);

    /// <summary>
    /// Joue le burst centré sur <paramref name="anchor"/> (bouton Confirmer / Valider).
    /// </summary>
    public static void Play(
        MonoBehaviour coroutineHost,
        RectTransform anchor,
        float destroyDelaySeconds = DefaultDestroyDelaySeconds)
    {
        if (coroutineHost == null || anchor == null)
            return;

        Canvas canvas = anchor.GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        RectTransform canvasRect = canvas.rootCanvas != null
            ? canvas.rootCanvas.transform as RectTransform
            : canvas.transform as RectTransform;
        if (canvasRect == null)
            return;

        Canvas rootCanvas = canvas.rootCanvas != null ? canvas.rootCanvas : canvas;

        var hostGo = new GameObject("SaleMoneyBurstUi", typeof(RectTransform));
        RectTransform host = hostGo.GetComponent<RectTransform>();

        // Position exacte du bouton, puis reparent canvas (worldPositionStays).
        host.SetParent(anchor, false);
        host.anchorMin = new Vector2(0.5f, 0.5f);
        host.anchorMax = new Vector2(0.5f, 0.5f);
        host.pivot = new Vector2(0.5f, 0.5f);
        host.anchoredPosition = Vector2.zero;
        host.localRotation = Quaternion.identity;
        host.localScale = Vector3.one;
        host.sizeDelta = Vector2.zero;

        host.SetParent(canvasRect, true);
        host.SetAsLastSibling();

        Canvas hostCanvas = hostGo.AddComponent<Canvas>();
        hostCanvas.overrideSorting = true;
        hostCanvas.sortingOrder = rootCanvas.sortingOrder + OverlaySortingOrderBoost;

        float delay = destroyDelaySeconds > 0f ? destroyDelaySeconds : DefaultDestroyDelaySeconds;
        coroutineHost.StartCoroutine(RunBurst(coroutineHost, host, delay));
    }

    private static IEnumerator RunBurst(MonoBehaviour coroutineHost, RectTransform host, float destroyDelaySeconds)
    {
        SpawnPieces(coroutineHost, host, DefaultCoinCount, CoinColor, CoinSizePixels, CoinSizePixels, isBill: false);
        SpawnPieces(coroutineHost, host, DefaultBillCount, BillColor, BillWidthPixels, BillHeightPixels, isBill: true);

        yield return new WaitForSecondsRealtime(destroyDelaySeconds);

        if (host != null)
            Object.Destroy(host.gameObject);
    }

    private static void SpawnPieces(
        MonoBehaviour coroutineHost,
        RectTransform host,
        int count,
        Color color,
        float width,
        float height,
        bool isBill)
    {
        for (int i = 0; i < count; i++)
        {
            var go = new GameObject(isBill ? "Bill" : "Coin", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(host, false);
            rt.sizeDelta = new Vector2(width, height);
            rt.anchoredPosition = Vector2.zero;

            Image image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;

            float angle = Random.Range(0f, Mathf.PI * 2f);
            float speed = isBill ? Random.Range(180f, 320f) : Random.Range(260f, 480f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle) + 0.35f) * speed;
            float gravity = isBill ? 520f : 900f;
            float lifetime = Random.Range(0.45f, 0.75f);
            float spin = isBill ? Random.Range(-90f, 90f) : Random.Range(-360f, 360f);

            coroutineHost.StartCoroutine(AnimatePiece(rt, image, velocity, gravity, lifetime, spin));
        }
    }

    private static IEnumerator AnimatePiece(
        RectTransform rt,
        Image image,
        Vector2 velocity,
        float gravity,
        float lifetime,
        float spinDegrees)
    {
        float elapsed = 0f;
        Color startColor = image.color;

        while (elapsed < lifetime && rt != null)
        {
            float dt = Time.unscaledDeltaTime;
            elapsed += dt;

            velocity.y -= gravity * dt;
            rt.anchoredPosition += velocity * dt;
            rt.Rotate(0f, 0f, spinDegrees * dt);

            float t = Mathf.Clamp01(elapsed / lifetime);
            float alpha = t < 0.7f ? 1f : 1f - ((t - 0.7f) / 0.3f);
            float scale = Mathf.Lerp(1f, 0.55f, t);
            rt.localScale = new Vector3(scale, scale, 1f);

            Color c = startColor;
            c.a = startColor.a * alpha;
            image.color = c;

            yield return null;
        }

        if (rt != null)
            Object.Destroy(rt.gameObject);
    }
}
