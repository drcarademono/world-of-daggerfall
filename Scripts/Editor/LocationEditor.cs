using UnityEditor;
using UnityEngine;

/// <summary>
/// Base location editor
/// </summary>
namespace DaggerfallWorkshop.Loc
{
    public class LocationEditor : EditorWindow
    {
        protected GUIStyle emptyBG = new GUIStyle();
        protected GUIStyle lightGrayBG = new GUIStyle();
        protected GUIStyle lightGreenBG = new GUIStyle();
        protected GUIStyle blackBG = new GUIStyle();

        protected static Rect Rect_NewFile { get { return new Rect(16, 8, 96, 16); } }
        protected static Rect Rect_SaveFile { get { return new Rect(96+48, 8, 96, 16); } }
        protected static Rect Rect_LoadFile { get { return new Rect(96+96+80, 8, 96, 16); } }

        protected void CreataGUIStyles()
        {
            lightGrayBG.normal.background = CreateColorTexture(1, 1, new Color(0.9f, 0.9f, 0.9f, 1.0f));
            lightGreenBG.normal.background = CreateColorTexture(1, 1, new Color(0.5f, 0.9f, 0.5f, 1.0f));
            blackBG.normal.background = CreateColorTexture(1, 1, new Color(0.0f, 0.0f, 0.0f, 0.95f));
        }

        /// <summary>
        /// Returns a texture with a specified "width" and "height" of single "color"
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        protected Texture2D CreateColorTexture(int width, int height, Color color)
        {
            Texture2D colorTexture = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    colorTexture.SetPixel(x, y, color);

            colorTexture.Apply();
            return colorTexture;
        }
    }
}
