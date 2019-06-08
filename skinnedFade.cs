using UnityEngine;

public class skinnedFade : MonoBehaviour {
    
    Color alphaColor;

    [HideInInspector] public bool startedFading = false;

    float timeBeforeFade = 8.0f;

    const float fadingTime = 4.0f;

    [HideInInspector] public Rigidbody[] rigidbuddies;
    [HideInInspector] public SkinnedMeshRenderer mesh;

    void Awake() {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();

        alphaColor = mesh.material.color;
        alphaColor.a = 0;

        rigidbuddies = GetComponentsInChildren<Rigidbody>();
    }

    public void iniciar() {
        this.enabled = true;
        StartCoroutine(waitToDie());
    }
    
    System.Collections.IEnumerator waitToDie() {
        foreach(Rigidbody rb in rigidbuddies) {
            rb.isKinematic = false;
        }
        yield return new WaitForSeconds(timeBeforeFade);
        changeToFade();
    }

    void Update() {
        foreach(Rigidbody rb in rigidbuddies) {
            if(rb.IsSleeping()) {
                rb.isKinematic = true;
                Collider col = rb.GetComponent<Collider>();
                if(col != null) {
                    col.isTrigger = true;
                }
            } else {
                return;
            }
        }
        
        if(!startedFading) {
            return;
        }
        
        foreach(Material mr in mesh.materials) {
            mr.color = Color.Lerp(mesh.material.color, alphaColor, fadingTime * Time.deltaTime);
        }
    }

    void changeToFade() {
        startedFading = true;
        foreach(Material standardShaderMaterial in mesh.materials) {
            standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            standardShaderMaterial.SetInt("_ZWrite", 0);
            standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
            standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
            standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            standardShaderMaterial.renderQueue = 3000;
        }
        Destroy(gameObject, fadingTime);
    }

    void OnDisable() {
        StopCoroutine(waitToDie());

        timeBeforeFade = 8.0f;
        foreach(Rigidbody rb in rigidbuddies) {
            if(rb == null) {
                continue;
            }
            rb.isKinematic = true;

            Collider col = rb.GetComponent<Collider>();
            col.isTrigger = false;
        }
        this.enabled = false;
    }

    void OnCollisionEnter(Collision collision) {
        if(!collision.collider.transform.root.gameObject.isStatic) {
            Physics.IgnoreCollision(collision.contacts[0].thisCollider, collision.contacts[0].otherCollider);
        }
    }
    /*
    //Might be useless but whatever
    void OnDestroy() {
        if(GetComponent<basicS>()) {
            Camera.main.transform.root.GetComponent<tetherSystem>().removeBot(GetComponent<basicS>());
        }
    }
    */
}