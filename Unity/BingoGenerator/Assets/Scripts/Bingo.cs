using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Bingo : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Title;
    public GridLayoutGroup BingoSquareGrid;

    public GameObject ParentToHide;
    public GameObject ParentToShow;
    public TMPro.TMP_InputField InputField_Title;
    public TMPro.TMP_InputField InputField_BingoChoices;

    public Button Button_Shuffle;
    public Button Button_PrintToPng;

    List<TMPro.TextMeshProUGUI> bingoTexts;

    const string PicturesFolderName = "Pictures";

    void Start()
    {
        bingoTexts = BingoSquareGrid.GetComponentsInChildren<TMPro.TextMeshProUGUI>().ToList();

        Button_Shuffle.onClick.AddListener(OnClick_Shuffle);
        Button_PrintToPng.onClick.AddListener(OnClick_PrintoToPng);

        InputField_Title.text = "My Bingo";
        InputField_BingoChoices.text = "Square 1\r\nSquare 2";
    }
    void OnClick_Shuffle()
    {
        var lines = InputField_BingoChoices.text.Split("\r\n").ToList();

        if(lines.Count == 1)
        {
            lines = lines[0].Split("\n").ToList();
        }

        lines = lines.OrderBy(line => UnityEngine.Random.value).ToList();
        InputField_BingoChoices.SetTextWithoutNotify(string.Join("\r\n", lines));
    }

    void OnClick_PrintoToPng()
    {
        StartCoroutine(TakePictureCoroutine());
    }

    IEnumerator TakePictureCoroutine()
    {
        ParentToHide.SetActive(false);
        ParentToShow.SetActive(true);

        SetSquares();

        yield return new WaitForEndOfFrame();

        const string SaveFileNameFormat = "pic-{0:D2}_{1:D2}_{2:D4}-{3:D2}_{4:D2}_{5:D2}.png";
        var time = DateTime.Now;
        string filename = string.Format(SaveFileNameFormat, time.Month, time.Day, time.Year, time.Hour, time.Minute, time.Second);

        string folder = Path.Combine(Application.persistentDataPath, PicturesFolderName);
        string path = Path.Combine(Application.persistentDataPath, PicturesFolderName, filename).Replace("/", "\\");

        if(!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        ScreenCapture.CaptureScreenshot(path);

        yield return new WaitForEndOfFrame();

        ParentToHide.SetActive(true);
        ParentToShow.SetActive(false);
        OpenFolder();
    }

    void SetSquares()
    {
        Title.text = InputField_Title.text;
        Title.ForceMeshUpdate();

        var lines = InputField_BingoChoices.text.Split("\r\n").ToList();
        for(int i = 0; i < bingoTexts.Count; i++)
        {
            if (i < lines.Count)
            {
                bingoTexts[i].text = lines[i];
            }
            else
            {
                bingoTexts[i].text = string.Empty;
            }

            bingoTexts[i].ForceMeshUpdate();
        }
    }

    void OpenFolder()
    {
        string folder = Path.Combine(Application.persistentDataPath, PicturesFolderName);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        System.Diagnostics.Process.Start(folder);
    }

    void SaveTexture2DToFile(Texture2D tex, string filePath, SaveTextureFileFormat fileFormat, int jpgQuality = 95)
    {
        switch (fileFormat)
        {
            case SaveTextureFileFormat.EXR:
                System.IO.File.WriteAllBytes(filePath + ".exr", tex.EncodeToEXR());
                break;
            case SaveTextureFileFormat.JPG:
                System.IO.File.WriteAllBytes(filePath + ".jpg", tex.EncodeToJPG(jpgQuality));
                break;
            case SaveTextureFileFormat.PNG:
                System.IO.File.WriteAllBytes(filePath + ".png", tex.EncodeToPNG());
                break;
            case SaveTextureFileFormat.TGA:
                System.IO.File.WriteAllBytes(filePath + ".tga", tex.EncodeToTGA());
                break;
        }
    }
    public enum SaveTextureFileFormat
    {
        EXR, JPG, PNG, TGA
    };
}
