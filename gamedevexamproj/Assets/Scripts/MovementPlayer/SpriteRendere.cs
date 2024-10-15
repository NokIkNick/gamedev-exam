using UnityEngine;

public class SpriteRendere : MonoBehaviour {
    private SpriteRenderer dashSpriteRenderer;
    void Awake() {
        dashSpriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void EnableSpriteRenderer() {
        dashSpriteRenderer.enabled = true;
    }

    public void DisableSpriteRenderer() {
        dashSpriteRenderer.enabled = false;
    }
}
