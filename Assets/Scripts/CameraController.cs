using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private float SmoothTime, MaxSpeed;
    [SerializeField] private float ZoomSpeed;

    [SerializeField] private float MinCameraDist, MaxCameraDist;

    public Camera MainCamera { get; private set; }

    private Transform Target;
    private Vector3 TargetPosition;
    private bool UsePositionAsTarget;

    private Coroutine CameraShakeCoroutine;
    private bool SmoothToTarget;
    private Vector3 TargetToCameraDir;
    private float CameraDist;
    private bool InCutscene;

    private Coroutine CutsceneCoroutine;

    protected override void Awake()
    {
        base.Awake();
        MainCamera = Camera.main;
        TargetToCameraDir = MainCamera.transform.localPosition.normalized;
        CameraDist = MainCamera.transform.localPosition.magnitude;
    }

    void LateUpdate()
    {
        if(Target)
        {
            float DistToTarget = Vector3.Distance(transform.position, GetTargetPosition());

            if(SmoothToTarget)
            {
                Vector3 vel = Vector3.zero;
                transform.position = Vector3.SmoothDamp(transform.position, GetTargetPosition(), ref vel, SmoothTime, MaxSpeed);
            }
            else
            {
                transform.position = GetTargetPosition();
            }
            
            if(DistToTarget < 0.2f)
            {
                SmoothToTarget = false;
            }

            if(Input.mouseScrollDelta.y != 0)
            {
                Zoom(Input.mouseScrollDelta.y);
            }

            MainCamera.transform.localPosition = 
                Vector3.Lerp(MainCamera.transform.localPosition, TargetToCameraDir * CameraDist, Time.deltaTime * 5);
        }
    }

    public void SetTarget(Transform Target)
    {
        this.Target = Target;
        SmoothToTarget = true;
        UsePositionAsTarget = false;
    }

    public void SetTarget(Vector3 Position)
    {
        this.TargetPosition = Position;
        SmoothToTarget = true;
        UsePositionAsTarget = true;
    }

    private Vector3 GetTargetPosition()
    {
        return UsePositionAsTarget ? TargetPosition : Target.position;
    }

    public void ShakeCamera(float Distance, float Interval, float Duration)
    {
        if(CameraShakeCoroutine != null) StopCoroutine(CameraShakeCoroutine);
        StartCoroutine(CameraShakeIEnum(Distance, Interval, Duration));
    }

    private IEnumerator CameraShakeIEnum(float Distance, float Interval, float Duration)
    {
        float TimeRemaining = Duration;
        while(TimeRemaining > 0)
        {
            TimeRemaining -= Interval;
            float OffsetX = Random.Range(0, Distance * (TimeRemaining / Duration));
            float OffsetY = Random.Range(0, Distance * (TimeRemaining / Duration));
            MainCamera.transform.localPosition = new Vector2(OffsetX, OffsetY);
            yield return new WaitForSeconds(Interval);
        }
        MainCamera.transform.localPosition = Vector3.zero;
    }

    public void Zoom(float delta)
    {
        CameraDist = Mathf.Clamp(CameraDist - delta * Time.deltaTime * ZoomSpeed, MinCameraDist, MaxCameraDist);
    }

    public void StartCutscene(Vector3 Position, float InitialDelay, float Duration, UnityAction Callback)
    {
        if(CutsceneCoroutine != null) StopCoroutine(CutsceneCoroutine);
        CutsceneCoroutine = StartCoroutine(StartCutsceneIEnum(Position, InitialDelay, Duration, Callback));
    }

    private IEnumerator StartCutsceneIEnum(Vector3 Position, float InitialDelay, float Duration, UnityAction Callback)
    {
        InCutscene = true;

        yield return new WaitForSeconds(InitialDelay);

        Transform PrevTarget = Target;
        SetTarget(Position);

        while(SmoothToTarget) yield return null;

        Callback.Invoke();

        yield return new WaitForSeconds(Duration);

        SetTarget(PrevTarget);
        InCutscene = false;
    }

    public bool IsInCutscene()
    {
        return InCutscene;
    }
}