using UnityEngine;

public class FishBowlTrigger : MonoBehaviour
{
    public GameObject foodBall;
    public GameObject Anim_obj;

    private bool hasBeenFed = false;
    private float resetTimer = 0f;
    private bool waitingToReset = false;
    public float maxIdleTime = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenFed) return;

        if (other.name == "Spatula")
        {
            Debug.Log("🎯 Fish fed!");

            hasBeenFed = true;
            Anim_obj.GetComponent<Animator>().SetTrigger("FoodFed");

            foodBall.SetActive(false);

            // Notify GameManager
            GameManager.instance.FishFed();

            // Start timer to reset if player takes too long
            resetTimer = 0f;
            waitingToReset = true;
        }
    }

    void Update()
    {
        if (waitingToReset)
        {
            resetTimer += Time.deltaTime;

            if (resetTimer >= maxIdleTime)
            {
                Debug.Log("⏳ Fish got tired again!");
                hasBeenFed = false;
                Anim_obj.GetComponent<Animator>().SetBool("FoodFed",false);
                Anim_obj.GetComponent<Animator>().SetTrigger("Idle");   
                waitingToReset = false;
            }
        }
    }
}
