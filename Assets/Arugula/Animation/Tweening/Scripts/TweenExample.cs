using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arugula.Animation;

public class TweenExample : MonoBehaviour
{
    public float height = 1;
    public float delay = 2;

    public Color colorA;
    public Color colorB;

    public Light targetLight;

    [Range(0, 1)]
    public float manualTweenControl = 0;
    public Transform manualTarget;
    ITween<Vector3> manualTween;

    void Start()
    {
        //ping-ponging a struct and overwriting the original source using OnUpdatedValue event which passes a generic value to make in-line functions like this possible.
        var tween = colorA.FadeTo(colorB, new Tween.Options(duration: 3, loop: LoopMode.PingPong));
        tween.OnUpdatedValue += (value) => colorA = value;

        //lets keep this around
        Color originalLightColor = targetLight.color;

        //another way of reading and applying the current value of a tween
        tween.OnUpdated += () => targetLight.color = tween.Current;

        //uh oh, this tween now has anonymous delegates that rely on TargetLight not being null, lets make sure the Tween aborts if the light is destroyed for some reason.
        tween.Context = targetLight;
        //and do something if it happens
        tween.OnContextLost += () => Debug.LogWarning("Poof!");

        //stop it!
        tween.OnLooped += (count) => { if (count == 5) tween.Finish(); };

        //return to original color using only the creepiest inline lamdas possible.
        tween.OnFinished += (complete) => targetLight.color.FadeTo(originalLightColor, new Tween.Options(8)).OnUpdatedValue += (value) => targetLight.color = value;


        //simple relative ping-ponging tween while position and rotation are controlled by the Bounce function
        transform.ScaleTo(transform.localScale * 1.5f, new Tween.Options(duration: 2, easing: Easing.easeInOutBack, loop: LoopMode.PingPong));

        //create a manual tween that is not added to the update queue
        //manualTween = new Vector3Tween(manualTarget.position, manualTarget.position + Vector3.up, 3, 1, Tweener.GetCurve(Easing.easeOutElastic), LoopMode.None, null);
        manualTween = new Vector3Tween(manualTarget.position, manualTarget.position + Vector3.up, new Tween.Options(duration: 3, delay: 1, easing: Easing.easeOutElastic));

        //bind the updated value to something
        manualTween.OnUpdatedValue += (position) => manualTarget.position = position;

        //tweening without adding to the update queue still fires events, but it won't destruct.
        manualTween.OnStarted += () => Debug.Log("Manual Tween Started");
        manualTween.OnFinished += (complete) => Debug.Log("Manual Tween Finished");

        //or we could just add the tween to the update queue at any time
        //Tweener.Enqueue(manualTween);

        //boing
        Bounce();
    }

    private void Update()
    {
        //push updates to a tween whenever you want
        manualTween.NormalizedTime = manualTweenControl;
    }

    void Bounce()
    {
        Vector3 origin = transform.localPosition;

        float fallDuration = CalcFallDuration(height);
        //anonymous in-line compound tween
        transform.MoveTo(origin + new Vector3(0, height, 0), new Tween.Options(duration: 1, delay: delay, easing: Easing.easeOutQuadratic)).OnFinished +=
            (complete) => transform.RotateTo(transform.localEulerAngles + new Vector3(0, 480, 0), new Tween.Options(duration: fallDuration * 1.5f)).OnStarted +=
            () => transform.MoveTo(origin, new Tween.Options(duration: fallDuration, easing: Easing.easeOutBounce)).OnFinished +=
            (complete2) => Bounce();  //anonymous recursion!?  the hell is wrong with me...
    }

    float CalcFallDuration(float height)
    {
        if (height == 0)
            return 0;
        else if (Physics.gravity.y == 0)
            return 1;
        //2.5 is an arbitrary constant for bounce easing curve
        return Mathf.Sqrt(height / (0.5f * Mathf.Abs(Physics.gravity.y))) * 2.5f;
    }
}
