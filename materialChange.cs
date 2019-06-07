using UnityEngine;

public class materialChange : MonoBehaviour {

    public Collider col;
    public Material[] mats;
    
    public bool destroyLife;

    life myLife;
    MeshRenderer meshRender;

    void Awake () {
        myLife = GetComponent<life>();
        meshRender = GetComponent<MeshRenderer>();

        if(mats.Length != meshRender.materials.Length && meshRender.materials.Length == 1) {
            Debug.LogWarning("mats.length != materials.length porra");
            return;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(myLife.vida <= 0) {
            if(col!=null) {
                Destroy(col);
            }
            if(destroyLife) {
                Destroy(myLife);
            }

            meshRender.materials = mats;

            Destroy(this);
        }
	}
}