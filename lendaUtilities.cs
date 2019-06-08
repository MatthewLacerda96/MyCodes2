using UnityEngine;

public class lendaUtilities : MonoBehaviour {

    [HideInInspector] public static Mesh cubeMesh;

    public int maxShooters = 12;
    static public int numMaxShooters;

    public static Vector3 cameraPos {
        get {
            return Camera.main.transform.position;
        }
    }

    void Awake() {
        CreateMesh();
        numMaxShooters = maxShooters;
    }

    void CreateMesh() {
        Vector3[] vertices = {  new Vector3 (0, 0, 0), new Vector3 (1, 0, 0), new Vector3 (1, 1, 0), new Vector3 (0, 1, 0),
                                new Vector3 (0, 1, 1), new Vector3 (1, 1, 1), new Vector3 (1, 0, 1), new Vector3 (0, 0, 1)  };

        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        cubeMesh = new Mesh();
        cubeMesh.Clear();
        cubeMesh.vertices = vertices;
        cubeMesh.triangles = triangles;
        cubeMesh.RecalculateNormals();
    }
    
    public static Vector3 NearestPointInLine(Vector3 origin, Vector3 direction, Vector3 point) {

        direction.Normalize();

        Vector3 dist = point - origin;
        float dot = Vector3.Dot(dist, direction);

        return origin + direction * dot;
    }

    public static Vector3 NearestPointInLine(line ln, Vector3 point) {
        ln.direction.Normalize();
        return NearestPointInLine(ln.origin, ln.direction, point);
    }

    public static Vector3 nearest(Vector3 pos, params Vector3[] vetor) {
        Vector3 nearestPos = vetor[0];
        float shortestDist = Vector3.Distance(pos, nearestPos);
        foreach(Vector3 v3 in vetor) {
            float currentDist = Vector3.Distance(v3, pos);
            if(currentDist == 0) {
                return v3;
            }
            if(currentDist < shortestDist) {
                shortestDist = currentDist;
                nearestPos = pos;
            }
        }
        return nearestPos;
    }

    public static Vector3 medianPoint(params Vector3[] points) {
        Vector3 somas = new Vector3(0, 0, 0);

        foreach(Vector3 v3 in points) {
            somas += v3;
        }
        somas /= points.Length;

        return somas;
    }

    public static bool explosionBlast(Vector3 center, float radius, float damage) {

        bool matamoAlgm = false;
        life aux;
        Rigidbody rigidbuddy;
        float dist;

        foreach(Collider col in Physics.OverlapSphere(center, radius)) {
            dist = Vector3.Distance(col.transform.position, center);
            if(dist <= radius) {
                aux = col.transform.GetComponent<life>();

                if(aux != null) {
                    if(col.transform.root.tag == "Player") {
                        aux.danoVindo(damage - (damage * (dist / radius)), center);
                    } else {
                        aux.danosse(damage - (damage * (dist / radius)), col.transform.name, col.transform.position * damage * 80);
                    }

                    if(aux.vida <= 0) {
                        matamoAlgm = true;
                    }
                }

                rigidbuddy = col.GetComponent<Rigidbody>();
                if(rigidbuddy == null) {
                    rigidbuddy = col.GetComponentInParent<Rigidbody>();
                }
                if(rigidbuddy != null) {
                    rigidbuddy.AddExplosionForce(300, col.transform.position, 5.0f);
                }
            }
        }

        return matamoAlgm;
    }

    public static bool isInViewFrustum(Vector3 point, Vector3 viewPoint, Vector3 direction) {
        Vector3 viewDirection = point - viewPoint;

        float angle = Vector3.Angle(viewDirection, direction);
        return angle <= 45 ? true : false;
    }

    public static bool isInViewFrustum(line l1, line l2) {
        float angle = Vector3.Angle(l1.direction, l2.direction);
        return angle <= 45 ? true : false;
    }

    public static bool isPointInLine(Vector3 origin, Vector3 direction, Vector3 point, float lineLength) {
        return (point == NearestPointInLine(origin, direction, point));
    }

    public static bool isPointInLine(line ln, Vector3 point, float lineLength) {
        return isPointInLine(ln.origin, ln.direction, point, lineLength);
    }

    public static float lineDistance(line ln, Vector3 pos) {
        Vector3 closestPoint = NearestPointInLine(ln, pos);
        return Vector3.Distance(closestPoint, pos);
    }

    public static float pathDistance(Vector3 start, UnityEngine.AI.NavMeshPath path, Vector3 finish) {
        if(path.corners.Length == 0) {
            return Vector3.Distance(start, finish);
        }

        float dist = Vector3.Distance(start, path.corners[0]);

        for(int i = 0; i < path.corners.Length - 1; i++) {
            dist += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        dist += Vector3.Distance(path.corners[path.corners.Length - 1], finish);

        return dist;
    }

    //essa função devia servir pra substituir um elemento X num vetor pro um Y, qndo não se sabe o indice de X
    //o vetor pode ser de qualquer tipo de dado
    public static void subst(object antigo, object novo, params object[] vetor) {
        for(int i = 0; i < vetor.Length; i++) {
            if(vetor[i] == antigo) {
                vetor[i] = novo;
            }
        }
    }
}
