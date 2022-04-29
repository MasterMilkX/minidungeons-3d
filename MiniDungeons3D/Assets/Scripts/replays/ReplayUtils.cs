using UnityEngine;
using System.Collections;

public class ReplayUtils : MonoBehaviour {

    public static long GetEpochMili() {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        long cur_time = (long)(System.DateTime.UtcNow - epochStart).TotalMilliseconds;
        return cur_time;
    }

}
