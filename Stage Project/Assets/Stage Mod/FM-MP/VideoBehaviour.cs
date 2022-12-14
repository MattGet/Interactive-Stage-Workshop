using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FireworksMania.Core.Behaviors.Fireworks.Parts;
using FireworksMania.Core.Persistence;
using System.Threading;
using FireworksMania.Core.Behaviors.Fireworks;
using FireworksMania.Core;

public class VideoBehaviour : BaseFireworkBehavior, IHaveFuse, IIgnitable, IHaveFuseConnectionPoint
{

    [Header("Video Settings")]
    public StageVideoManager StageManager;
    public InputMenu menu;
    private Rigidbody _rigidbody;
    private string localVideoURL;

    public override CustomEntityComponentData CaptureState()
    {

        CustomEntityComponentData entitydata = new CustomEntityComponentData();
        Rigidbody component = this.GetComponent<Rigidbody>();
        entitydata.Add<SerializableVector3>("Position", new SerializableVector3()
        {
            X = this.transform.position.x,
            Y = this.transform.position.y,
            Z = this.transform.position.z
        });
        entitydata.Add<SerializableRotation>("Rotation", new SerializableRotation()
        {
            X = this.transform.rotation.x,
            Y = this.transform.rotation.y,
            Z = this.transform.rotation.z,
            W = this.transform.rotation.w
        });

        entitydata.Add<bool>("IsKinematic", component.isKinematic);
        entitydata.Add<string>("URL", StageManager.VideoURL);
        entitydata.Add<bool>("videoloop", StageManager.player.isLooping);
        entitydata.Add<float>("videoVol", menu.volnumb);
        entitydata.Add<int>("vidQual", (int)StageManager.videoQuality);
        entitydata.Add<float>("VidTime", (float)StageManager.player.time);

        entitydata.Add<bool>("AudioColor", StageManager.Lights.UseAudioColor);
        entitydata.Add<bool>("UseLights", StageManager.Lights.UseLights);
        entitydata.Add<bool>("UseLasers", StageManager.Lights.UseLasers);
        entitydata.Add<float>("AudioBuffer", StageManager.Lights.AudioVisualiser.BufferMultiplier);

        entitydata.Add<float>("SAlpha", StageManager.Lights.AlphaMulti);
        entitydata.Add<float>("SColor", StageManager.Lights.ColorMulti);
        entitydata.Add<float>("SEnhancer", StageManager.Lights.ColorEnhancer);
        entitydata.Add<int>("ColorMode", ((int)StageManager.Lights.colorMode));

        entitydata.Add<float>("LASAlpha", StageManager.Lights.LASAlphaMulti);
        entitydata.Add<float>("LASColor", StageManager.Lights.LASColorMulti);
        entitydata.Add<float>("LASEnhancer", StageManager.Lights.LASColorEnhancer);
        entitydata.Add<int>("LASColorMode", ((int)StageManager.Lights.LAScolorMode));
        //Debug.Log("stored Shell ID = " + TubeID);
        return entitydata;
    }

    public override void RestoreState(CustomEntityComponentData customComponentData)
    {
        SerializableVector3 serializableVector3 = customComponentData.Get<SerializableVector3>("Position");
        SerializableRotation serializableRotation = customComponentData.Get<SerializableRotation>("Rotation");
        bool flag = customComponentData.Get<bool>("IsKinematic");
        this.transform.position = new Vector3(serializableVector3.X, serializableVector3.Y, serializableVector3.Z);
        this.transform.rotation = new Quaternion(serializableRotation.X, serializableRotation.Y, serializableRotation.Z, serializableRotation.W);
        Rigidbody component = this.GetComponent<Rigidbody>();
        if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
            return;
        component.isKinematic = flag;

        string temp = customComponentData.Get<string>("URL");
        localVideoURL = temp;

        int temp2 = customComponentData.Get<int>("vidQual");
        menu.Quality.value = temp2;
        StageManager.player.isLooping = customComponentData.Get<bool>("videoloop");
        menu.volnumb = customComponentData.Get<float>("videoVol");
        StageManager.Quality(temp2);
        StageManager.OK(temp);

        float time = customComponentData.Get<float>("VidTime");
        if (time != 0f && temp2 != 3)
        {
            StageManager.SetVideoStartTime(time);
        }

        if (StageManager.Lights.UseAudioColor != customComponentData.Get<bool>("AudioColor")) menu.ToggleColorMode();
        if (StageManager.Lights.UseLights != customComponentData.Get<bool>("UseLights")) menu.ToggleLights();
        if (StageManager.Lights.UseLasers != customComponentData.Get<bool>("UseLasers")) menu.ToggleLasers();
        menu.SBuffer.value = customComponentData.Get<float>("AudioBuffer");

        menu.SAlpha.value = customComponentData.Get<float>("SAlpha");
        menu.SColor.value = customComponentData.Get<float>("SColor");
        menu.SEnhancer.value = customComponentData.Get<float>("SEnhancer");
        menu.ColorMode.value = customComponentData.Get<int>("ColorMode");

        menu.SLAlpha.value = customComponentData.Get<float>("LASAlpha");
        menu.SLColor.value = customComponentData.Get<float>("LASColor");
        menu.SLEnhancer.value = customComponentData.Get<float>("LASEnhancer");
        menu.SLColorMode.value = customComponentData.Get<int>("LASColorMode");
    }


    // Update is called once per frame
    void Update()
    {

    }



    protected override void Awake()
    {
        base.Awake();
        this._rigidbody = this.GetComponent<Rigidbody>();
        if ((UnityEngine.Object)this._rigidbody == (UnityEngine.Object)null)
            Debug.LogError((object)"Missing Rigidbody", (UnityEngine.Object)this);
        if ((UnityEngine.Object)this.StageManager == (UnityEngine.Object)null)
            Debug.LogError((object)"Missing Video Player in VideoBehaviour!");
        //Debug.Log("SID = " + SaveableComponentTypeId);
    }

    protected override void Start()
    {
        base.Start();
        StageManager.gotURL.AddListener(reciveURL);
        // Debug.Log(gameObject + " initial speed = " + LaunchForce);
        //Debug.Log(gameObject + " is at position " + gameObject.transform.position.ToString("F2") + " on spawn\n");
    }

    private void reciveURL(string url)
    {
        localVideoURL = url;
    }

    protected override void OnValidate()
    {
        if (Application.isPlaying)
            return;
        base.OnValidate();
        if (!((UnityEngine.Object)this.StageManager == (UnityEngine.Object)null))
            return;
        Debug.LogError((object)("FMVP: Missing Video Player on gameobject '" + this.gameObject.name + "' on component 'VideoBehaviour'!"), (UnityEngine.Object)this.gameObject);


    }

    protected override async UniTask LaunchInternalAsync(CancellationToken token)
    {
        if (StageManager.player.isPaused)
        {
            StageManager.player.Play();
            if (menu.lights.UseLights) menu.lights.ToggleLights(true);
            if (menu.lights.UseLasers) menu.lights.ToggleLasers(true);
            if (menu.lights.UseLasers && !StageManager.IsAnimating) StageManager.ToggleAnimations();
            StageManager.isPlaying = true;
        }
        else if (StageManager.player.isPrepared)
        {
            StageManager.PlayVideo();
            if (menu.lights.UseLights) menu.lights.ToggleLights(true);
            if (menu.lights.UseLasers) menu.lights.ToggleLasers(true);
            if (menu.lights.UseLasers && !StageManager.IsAnimating) StageManager.ToggleAnimations();
            StageManager.isPlaying = true;
        }
        await UniTask.WaitWhile(() => StageManager.isPlaying == true, PlayerLoopTiming.Update, token);
        token.ThrowIfCancellationRequested();
        if (!CoreSettings.AutoDespawnFireworks)
            return;
        await this.DestroyFireworkAsync(token);
    }

}
