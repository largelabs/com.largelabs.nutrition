using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    InterpolatorsManager i;
    ITypedAnimator<float> x;
    ITypedAnimator<Vector2> y;
    ITypedAnimator<Vector3> z;
    ITypedAnimator<Color> w;
    public float e = 0f;
    public float time = 2f;
    // Start is called before the first frame update
    void Start()
    {
        i = gameObject.GetComponent<InterpolatorsManager>();
        x = i.Animate(0, e, time,new AnimationMode(AnimationType.Linear));
        y = i.Animate(new Vector2(0, 0), new Vector2(e, e), time, new AnimationMode(AnimationType.Bounce));
        z = i.Animate(new Vector3(0, 0,0), new Vector3(2,2,2), time, new AnimationMode(AnimationType.Ease_In_Out));
        AnimationCurve curve = new AnimationCurve(new Keyframe(0,1), new Keyframe(1,0));
        w = i.Animate(new Color(0,0,0,0), new Color(1,1,1,0), time, new AnimationMode(curve));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(x.Current,y.Current.x,y.Current.x);
        transform.localScale= z.Current;
        Debug.Log(w.Current.ToString());
    }
}
