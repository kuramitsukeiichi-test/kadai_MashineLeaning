using UnityEngine;
using System.Collections;

public class ASequenceShooter : MonoBehaviour
{
    public AMover[] movers; // 6つのAを設定
    public float shotInterval = 0.1f;
    public BController bController;

    public IEnumerator FireSequence(Vector3[] points)
    {
        for (int i = 0; i < movers.Length; i++)
        {
            movers[i].startPoint = points[i * 2];
            movers[i].endPoint = points[i * 2 + 1];
            movers[i].StartMove();
            yield return new WaitForSeconds(shotInterval);
        }

        bool isAnyMoving = true;
        while (isAnyMoving)
        {
            isAnyMoving = false;
            foreach (var m in movers)
                if (m.isMoving) isAnyMoving = true;
            yield return null;
        }

        // 🔥 全A終了時にBをリセット
        if (bController != null)
            bController.ResetB();
    }

}
