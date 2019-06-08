using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class tetherSystem : MonoBehaviour {

    //Este bloco é para debug visual e não para build
    public bool debuggar;
    public GameObject visualAccessPoint, visualProbe;
    public Material green, red;

    [HideInInspector] public tetherProbe playerDestination = new tetherProbe();
    
    tetherProbe[] probes;

    const int numAccesses = 8;
    public tetherFlank[] flankProbes = new tetherFlank[numAccesses];
    Vector3[] directions = new Vector3[numAccesses];

    NavMeshHit auxNavMeshHit;
    Vector3 lastPos, velocity;

    const int rayBudget = 16;  //Número de raytraces por frame. 16 é um ótimo balanço entre performance e delay

    void Awake() {
        if(this.enabled == false) {
            Destroy(this);
            return;
        }

        List<tetherProbe> tetherList = new List<tetherProbe>();

        LightProbeGroup lpg = FindObjectOfType<lendaUtilities>().GetComponent<LightProbeGroup>();

        foreach(Vector3 probePos in lpg.probePositions) {
            Vector3 auxProbePos = probePos + lpg.transform.position;

            if(NavMesh.SamplePosition(auxProbePos, out auxNavMeshHit, 0.6f, NavMesh.AllAreas)) {
                tetherProbe auxTP = new tetherProbe();
                auxTP.position = auxProbePos + new Vector3(0, 0.9f, 0);
                auxTP.areaMask = auxNavMeshHit.mask;

                GameObject aux = Instantiate(visualProbe, auxTP.position - new Vector3(0, 0.9f, 0), Quaternion.identity);

                auxTP.debugger = aux;
                auxTP.red[0] = red;
                auxTP.green[0] = green;
                auxTP.mr = aux.GetComponent<MeshRenderer>();

                tetherList.Add(auxTP);
            }
        }

        tetherList = tetherList.OrderBy(tp => tp.position.y).ThenBy(tp => -tp.position.z).ThenBy(tp => tp.position.x).ToList();

        for(int i = 0; i < tetherList.Count - 1; i++) {
            bool xIgual = tetherList[i].position.x == tetherList[i + 1].position.x;
            bool zIgual = tetherList[i].position.z == tetherList[i + 1].position.z;
            bool yIgual = tetherList[i].position.y == tetherList[i + 1].position.y;

            if(xIgual && zIgual && !yIgual) {
                tetherList.Remove(tetherList[i + 1]);
            }
        }

        probes = tetherList.ToArray();

        List<tetherFlank> listaccess = new List<tetherFlank>();
        float degree = 360 / numAccesses;
        for(int i = 0; i < numAccesses; i++) {
            listaccess.Add(new tetherFlank());
            float rad = degree * i * Mathf.Deg2Rad;

            directions[i] = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
            directions[i] *= 10;

            GameObject aux = Instantiate(visualAccessPoint, transform.position + directions[i], Quaternion.identity);
            aux.transform.name = "probe" + i;
            listaccess[i].debugger = aux;

        }

        flankProbes = listaccess.ToArray();

        float minXgeral = 0, maxXgeral = 0, minZgeral = 0, maxZgeral = 0;
        foreach(tetherProbe tp in probes) {
            if(minXgeral > tp.position.x) {
                minXgeral = tp.position.x;
            } else if(maxXgeral < tp.position.x) {
                maxXgeral = tp.position.x;
            }

            if(minZgeral > tp.position.z) {
                minZgeral = tp.position.z;
            } else if(maxZgeral < tp.position.z) {
                maxZgeral = tp.position.z;
            }
        }

        if(debuggar) {
            int frameNum = ((probes.Length / rayBudget) + (probes.Length % rayBudget != 0 ? 1 : 0));
            float frameNumFloat = frameNum;
            float frameTime = (frameNumFloat / 60);

            Debug.Log(probes.Length + " tethersProbes. " + "Refresh time: " + frameNum + " frames, " + frameTime + "s");
        }
    }

    void Start() {
        lastPos = transform.position;
        velocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update() {
        numShooters = shooters;

        if(Time.timeScale == 0) {
            return;
        }

        velocity = transform.position - lastPos;
        velocity /= Time.deltaTime;
        lastPos = transform.position;

        refreshProbes();
        refreshFlanks();

        predictPlayerProbeDestination();

        foreach(tetherFlank frank in flankProbes) {
            frank.botsAround = 0;
            frank.botsDestinated = 0;
        }

        Vector3[] flankPos = new Vector3[flankProbes.Length];
        for(int i = 0; i < flankProbes.Length; i++) {
            flankPos[i] = flankProbes[i].position;
        }
    }

    public Vector3 nearestFlankPos(Vector3 pos, int mask) {
        return nearestFlank(pos, mask).position;
    }

    public Vector3 nearestProbePos(Vector3 sample, int mask) {
        return nearestProbe(sample, mask).position;
    }

    public Vector3 nearestProbePos(Vector3 sample, int mask, bool visibility) {
        return nearestProbe(sample, mask, visibility).position;
    }

    public tetherFlank nearestFlank(Vector3 pos, int mask) {
        tetherFlank nearest = flankProbes[0];
        float nearestDist = Vector3.Distance(nearest.position, pos);

        foreach(tetherFlank access in flankProbes) {

            float current = Vector3.Distance(access.position, pos);
            if(current < nearestDist && access.areaMask == mask) {
                nearest = access;
                nearestDist = current;
            }
        }

        return nearest;
    }

    public tetherProbe nearestProbe(Vector3 sample, int mask) {
        tetherProbe nearest = probes[0];
        float nearestDist = Vector3.Distance(nearest.position, sample);

        foreach(tetherProbe tp in probes) {
            if(tp.areaMask == mask) {
                float currentDist = Vector3.Distance(tp.position, sample);
                if(currentDist <= 0.25f) {//vamo aproximar logo
                    return tp;
                }
                if(currentDist < nearestDist) {
                    nearestDist = currentDist;
                    nearest = tp;
                }
            }
        }

        return nearest;
    }

    public tetherProbe nearestProbe(Vector3 sample, int mask, bool visibility) {
        tetherProbe nearest = probes[0];
        float nearestDist = Vector3.Distance(nearest.position, sample);

        foreach(tetherProbe tp in probes) {
            if(tp.areaMask == mask && tp.isVisible == visibility) {
                float currentDist = Vector3.Distance(tp.position, sample);
                if(currentDist <= 0.25f) {//vamo aproximar logo
                    return tp;
                }
                if(currentDist < nearestDist) {
                    nearestDist = currentDist;
                    nearest = tp;
                }
            }
        }

        return nearest;
    }

    void predictPlayerProbeDestination() {

        if(velocity.magnitude == 0) {
            return;
        }

        Vector3 final = transform.position + (velocity.normalized * 50);

        NavMeshHit posOnNavMesh;
        NavMesh.SamplePosition(transform.position, out posOnNavMesh, 3.0f, NavMesh.AllAreas);
        if(NavMesh.Raycast(transform.position, final, out auxNavMeshHit, NavMesh.AllAreas)) {
            playerDestination.position = nearestProbePos(auxNavMeshHit.position, NavMesh.AllAreas, true);
        } else {
            playerDestination.position = nearestProbePos(final, NavMesh.AllAreas, true);
        }
    }

    int indexRefresh = 0;
    void refreshProbes() {
        for(int i = 0; i < rayBudget; i++) {
            probes[indexRefresh].traceToPlayer();

            indexRefresh++;

            if(indexRefresh > probes.Length - 1) {
                indexRefresh = 0;
            }
        }
    }

    void refreshFlanks() {
        for(int i = 0; i < numAccesses; i++) {

            NavMesh.Raycast(transform.position, transform.position + directions[i], out auxNavMeshHit, NavMesh.AllAreas);
            flankProbes[i].position = auxNavMeshHit.position;
            flankProbes[i].path = new line(transform.position, flankProbes[i].position);

            flankProbes[i].closestCover = nearestProbePos(flankProbes[i].position, NavMesh.AllAreas, false);

            if(debuggar) {
                flankProbes[i].debugger.transform.position = auxNavMeshHit.position;
                flankProbes[i].debugger.transform.position = flankProbes[i].position;
            }
        }
    }
}
