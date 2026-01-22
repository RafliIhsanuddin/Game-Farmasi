using UnityEngine;

public class RangerCanvasManager : MonoBehaviour
{
    [Header("All Canvases")]
    [SerializeField] private GameObject defaultCanvas;
    [SerializeField] private GameObject whiteCanvas;
    [SerializeField] private GameObject blueCanvas;
    [SerializeField] private GameObject pinkCanvas;
    [SerializeField] private GameObject redCanvas;
    [SerializeField] private GameObject greenCanvas;
    [SerializeField] private GameObject yellowCanvas;

    [Header("Settings")]
    [SerializeField] private bool showDefaultOnStart = true; // NEW

    private GameObject[] allCanvases;

    private void Start()
    {
        allCanvases = new GameObject[]
        {
            defaultCanvas,
            whiteCanvas,
            blueCanvas,
            pinkCanvas,
            redCanvas,
            greenCanvas,
            yellowCanvas
        };

        if (showDefaultOnStart)
        {
            ShowDefaultCanvas();
        }
    }

    private void ShowOnly(GameObject canvasToShow)
    {
        foreach (var canvas in allCanvases)
        {
            if (canvas != null)
                canvas.SetActive(canvas == canvasToShow);
        }
    }

    public void ShowDefaultCanvas() => ShowOnly(defaultCanvas);
    public void ShowWhiteCanvas()   => ShowOnly(whiteCanvas);
    public void ShowBlueCanvas()    => ShowOnly(blueCanvas);
    public void ShowPinkCanvas()    => ShowOnly(pinkCanvas);
    public void ShowRedCanvas()     => ShowOnly(redCanvas);
    public void ShowGreenCanvas()   => ShowOnly(greenCanvas);
    public void ShowYellowCanvas()  => ShowOnly(yellowCanvas);
}