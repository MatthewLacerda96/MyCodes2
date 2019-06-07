using UnityEngine;

public class meshFader : MonoBehaviour {

    public float timeBeforeFade = 8.0f;
    
    Color alphaColor;
    Collider col;
    MeshRenderer mesh;
    Rigidbody rigidbuddy;
    skinnedFade skinFade;
    
    float fadingTime = 3.0f, timer;

    void Awake() {
        col = GetComponentInChildren<Collider>();

        skinFade = GetComponentInParent<skinnedFade>();

        rigidbuddy = GetComponentInChildren<Rigidbody>();

        mesh = GetComponentInChildren<MeshRenderer>();

        alphaColor = mesh.material.color;
        alphaColor.a = 0;
    }

    void OnEnable() {
        rigidbuddy.isKinematic = false;
    }

    void Update() {
        if(col.isTrigger) {
            if(rigidbuddy != null) {
                if(rigidbuddy.IsSleeping()) {
                    col = GetComponent<Collider>();
                    if(col == null) {
                        Debug.Log("excuse me wtf?");
                    }
                    rigidbuddy.isKinematic = true;
                    col.isTrigger = true;
                    gameObject.isStatic = true;
                } else {
                    return;
                }
            }
        }

        if(skinFade.startedFading) {
            changeToFade();
        } else {
            return;
        }

        foreach(Material mr in mesh.materials) {
            mr.color = Color.Lerp(mesh.material.color, alphaColor, fadingTime * Time.deltaTime);
        }
        timer += Time.deltaTime;
        if(timer >= fadingTime) {
            Destroy(transform.root.gameObject);
        }
    }

    void changeToFade() {
        foreach(Material standardShaderMaterial in mesh.materials) {
            standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            standardShaderMaterial.SetInt("_ZWrite", 0);
            standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
            standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
            standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            standardShaderMaterial.renderQueue = 3000;
        }
    }

    void OnDisable() {
        col.isTrigger = false;
        gameObject.isStatic = false;
        
        timeBeforeFade = 8.0f;
        fadingTime = 3.0f;
    }
}