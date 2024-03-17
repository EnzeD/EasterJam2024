using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform))]
public class SpriteSorterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Transform transform = (Transform)target;

        if (GUILayout.Button("Sort Sprite By Y"))
        {
            foreach (Transform child in transform)
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = Mathf.RoundToInt(child.position.y * -100);
                }
            }
        }
    }
}