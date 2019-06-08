using UnityEngine;

public class tetherProbe {
    //Remover este bloco, e fazer position e areamask virar constantes na finalBuild
    public GameObject debugger;
    public Material[] red = new Material[1], green = new Material[1];
    public MeshRenderer mr;

    public Vector3 position;

    public float distToPlayer {
        get;
        private set;
    }

    public bool isVisible {
        get;
        private set;
    }

    public int areaMask;

    //public uint botsAround;
    //public uint finalDestion;

    RaycastHit hit;

    public void traceToPlayer() {
        Vector3 direction = Camera.main.transform.position - position;
        if(Physics.Raycast(position, direction, out hit, Vector3.Distance(Camera.main.transform.root.position, position))) {
            if(hit.transform.root.tag == "Player" /*|| hit.transform.gameObject.isStatic == false */) {
                isVisible = true;
                distToPlayer = hit.distance;
                Debug.Log("player traced");
            } else {
                isVisible = false;
                distToPlayer = Vector3.Distance(Camera.main.transform.root.position, position);
            }
        } else {
            isVisible = true;
            distToPlayer = Vector3.Distance(Camera.main.transform.root.position, position);
        }

        if(debugger != null) {
            if(isVisible) {
                mr.materials = green;
            } else {
                mr.materials = red;
            }
        }
    }
}