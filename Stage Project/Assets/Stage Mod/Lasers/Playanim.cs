using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireworksMania.Core.Behaviors.Fireworks.Parts;
using sourcenity;

public class Playanim : MonoBehaviour
{
    [Header("Laser Settings")]
    public ShowLaserEffect laser;
    public AnimationClip animationClip;
    [SerializeField]
    private Animator _animation;
    public bool playing;


    protected void Awake()
    {
        if ((UnityEngine.Object)this._animation == (UnityEngine.Object)null)
            Debug.LogError((object)"Missing effects inLaserBehavior!");
    }

    protected void Start()
    {
        laser.gameObject.SetActive(false);
    }

    protected void OnValidate()
    {
        if (!((UnityEngine.Object)this._animation == (UnityEngine.Object)null))
        {
            _animation = this.gameObject.GetComponent<Animator>();
        }
           
        if (this.laser == null)
        {
            laser = this.GetComponentInChildren<ShowLaserEffect>();
        }
    }



    public void ToggleAnim(AnimationClip clip)
    {
        if (playing)
        {
            _animation.StopPlayback();
            laser.gameObject.SetActive(false);
            playing = false;
        }
        else
        {
            if (animationClip != null)
            {
                clip = animationClip;
            }
            laser.gameObject.SetActive(true);
            _animation.Play(clip.name, 0, 0.0f);
            playing = true;
        }
    }
}
