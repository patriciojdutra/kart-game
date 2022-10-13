using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyCamera : MonoBehaviourPunCallbacks
{
    
    private Camera camera;
    private AudioListener audioSource;

    void Start()
    {
        camera = GetComponent<Camera>();
        audioSource = GetComponent<AudioListener>();

        camera.gameObject.SetActive(photonView.IsMine);
        audioSource.gameObject.SetActive(photonView.IsMine);
    }
}
