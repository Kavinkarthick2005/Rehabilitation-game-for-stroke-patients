using UnityEngine;

public class Spoon : MonoBehaviour
{
    public float Xp,Yp,Zp,Xr,Yr,Zr;
    public Transform hand; // Drag 'b_r_wrist' here in Unity

    void Update()
    {
        AttachToHand();
    }


    void AttachToHand()
    {
        transform.SetParent(hand);
        transform.localPosition = new Vector3(Xp, Yp, Zp); // Adjust as needed
        transform.localRotation = Quaternion.Euler(Xr, Yr, Zr); // Adjust to make it look natural
    }
}
