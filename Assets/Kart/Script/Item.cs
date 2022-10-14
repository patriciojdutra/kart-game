using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite image;
    public GameObject bala;
    public float speedBala = 5;
    public float duracao = 5;

    private void Start()
    {
        GetComponent<AudioSource>().Play();
        Destroy(gameObject, (duracao * 50) * Time.deltaTime);
    }

    void FixedUpdate()
    {
        gameObject.transform.Translate(new Vector3(0, 0, (speedBala * 50) * Time.deltaTime));
    }
}
