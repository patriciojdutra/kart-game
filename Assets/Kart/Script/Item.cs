using Photon.Pun;
using UnityEngine;

public class Item : MonoBehaviourPun
{
    public Sprite image;
    public GameObject bala;
    public float speedBala = 5;
    public float duracao = 5;
    private int multi = 50;

    private void Start()
    {
        Destroy(duracao, multi);
    }

    void FixedUpdate()
    {
        gameObject.transform.Translate(new Vector3(0, 0, (speedBala * 50) * Time.deltaTime));
    }


    [PunRPC]
    public void Destroy(float duration = 0, int x = 0)
    {
        GetComponent<AudioSource>().Play();
        Destroy(gameObject, (duration * x) * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision col)
    {
        Destroy();
    }

}
