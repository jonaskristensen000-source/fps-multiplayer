using UnityEngine;
using UnityEngine.AI;

public class ColonyNavigation : MonoBehaviour
{
    NavMeshPath path;
    Vector3[] corners;
    int corner;
    float replan;
    public Vector3 Direction(Vector3 destination)
    {
        replan -= Time.deltaTime;
        if (path == null) path = new NavMeshPath();
        if (replan <= 0f)
        {
            replan = .65f;
            if (NavMesh.SamplePosition(transform.position, out var start, 2f, NavMesh.AllAreas) && NavMesh.SamplePosition(destination, out var end, 3f, NavMesh.AllAreas) && NavMesh.CalculatePath(start.position, end.position, NavMesh.AllAreas, path))
            { corners = path.corners; corner = 1; }
        }
        Vector3 target = destination;
        if (corners != null && corner < corners.Length)
        {
            target = corners[corner];
            if (Vector3.Distance(new Vector3(transform.position.x, target.y, transform.position.z), target) < .5f && corner < corners.Length - 1) target = corners[++corner];
        }
        Vector3 dir = target - transform.position; dir.y = 0;
        return dir.normalized;
    }
}
