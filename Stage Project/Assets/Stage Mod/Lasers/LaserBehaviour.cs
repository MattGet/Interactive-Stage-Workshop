using Cysharp.Threading.Tasks;
using Cysharp;
using System;
using System.Threading;
using UnityEngine;
using FireworksMania.Core.Behaviors.Fireworks;
using sourcenity;

public class LaserBehaviour : BaseFireworkBehavior
{
    [Header("Laser Settings")]
    public ShowLaserEffect laser;
    public AnimationClip animationClip;
    [SerializeField]
    private Animator _animation;
    private Rigidbody _rigidbody;
    private bool playing;


    protected override void Awake()
    {
        base.Awake();
        this._rigidbody = this.GetComponent<Rigidbody>();
        if ((UnityEngine.Object)this._rigidbody == (UnityEngine.Object)null)
            Debug.LogError((object)"Missing Rigidbody", (UnityEngine.Object)this);
        if ((UnityEngine.Object)this._animation == (UnityEngine.Object)null)
            Debug.LogError((object)"Missing effects inLaserBehavior!");
    }

    protected override void Start()
    {
        base.Start();
        laser.gameObject.SetActive(false);
    }

    protected override void OnValidate()
    {
        if (Application.isPlaying)
            return;
        base.OnValidate();
        if (!((UnityEngine.Object)this._animation == (UnityEngine.Object)null))
            return;
        Debug.LogError((object)("Missing Laser effect on gameobject '" + this.gameObject.name + "' on component 'LaserBehavior'!"), (UnityEngine.Object)this.gameObject);
    }

    protected override async UniTask LaunchInternalAsync(CancellationToken token)
    {
        ToggleAnim();
        await UniTask.WaitWhile(() => true, cancellationToken: token);
        token.ThrowIfCancellationRequested();
        await this.DestroyFireworkAsync(token);
    }

    public void ToggleAnim()
    {
        if (playing)
        {
            _animation.StopPlayback();
            laser.gameObject.SetActive(false);
            playing = false;
        }
        else
        {
            laser.gameObject.SetActive(true);
            _animation.Play(animationClip.name, 0, 0.0f);
            playing = true;
        }

    }
}
