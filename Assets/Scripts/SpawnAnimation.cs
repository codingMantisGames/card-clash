using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnAnimation : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private float dropTime;
    [SerializeField] private Ease dropEase;
    [SerializeField] private float scaleTime;
    [SerializeField] private Ease scaleEase;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        transform.DOLocalMove(Vector3.zero, dropTime).SetEase(dropEase).OnComplete(() =>
        {
            float y = transform.localScale.y;

            CameraShake.instance.ShakeCamera(0.5f, 0.5f);

            if (gameObject.TryGetComponent<AudioSource>(out AudioSource audioSource))
                audioSource.Play();

            transform.DOScaleY(transform.localScale.y - 0.1f, scaleTime).SetEase(scaleEase).OnComplete(() =>
            {
                transform.DOScaleY(y, scaleTime).SetEase(scaleEase);
            });
        });
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS

    #endregion
}
