using Photon.Pun;
using UnityEngine;

public class Item : MonoBehaviourPun
{
    public Sprite image;
    public GameObject bala;
    public float speedBala = 5;
    public float duracao = 5;

    private void Start()
    {
        Destroy();
    }

    void FixedUpdate()
    {
        gameObject.transform.Translate(new Vector3(0, 0, (speedBala * 50) * Time.deltaTime));
    }


    [PunRPC]
    public void Destroy()
    {
        GetComponent<AudioSource>().Play();
        Destroy(gameObject, (duracao * 50) * Time.deltaTime);
    }
}
