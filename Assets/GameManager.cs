using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float totalDamage = 0f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddDamage(float dmg)
    {
        totalDamage += dmg;
    }

    public void ResetDamage()
    {
        totalDamage = 0f;
    }
}
