using UnityEngine;
using System;
using System.Collections.Generic;

namespace DG.Tweening
{
    public enum RotateMode
    {
        Fast,
        FastBeyond360,
        WorldAxisAdd,
        LocalAxisAdd
    }

    public enum Ease
    {
        Linear,
        InSine, OutSine, InOutSine,
        InQuad, OutQuad, InOutQuad,
        InCubic, OutCubic, InOutCubic,
        InQuart, OutQuart, InOutQuart,
        InQuint, OutQuint, InOutQuint,
        InExpo, OutExpo, InOutExpo,
        InCirc, OutCirc, InOutCirc,
        InElastic, OutElastic, InOutElastic,
        InBack, OutBack, InOutBack,
        InBounce, OutBounce, InOutBounce,
        Flash, InFlash, OutFlash, InOutFlash // Added Flash dummy
    }

    public enum PathType
    {
        Linear,
        CatmullRom
    }

    public class Mover : MonoBehaviour
    {
        private static Mover _instance;
        public static Mover Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("Mover");
                    _instance = go.AddComponent<Mover>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private List<Tween> tweens = new List<Tween>();

        private void Update()
        {
            for (int i = tweens.Count - 1; i >= 0; i--)
            {
                var tween = tweens[i];
                
                if (tween.isKilled)
                {
                    tweens.RemoveAt(i);
                    continue;
                }
                
                // Check target validity for Transform-bound tweens
                if (tween.targetGameObject != null && tween.target == null) // target was destroyed
                {
                    tweens.RemoveAt(i);
                    continue;
                }

                if (tween.delay > 0)
                {
                    tween.delay -= Time.deltaTime;
                    continue;
                }

                if (!tween.isPlaying) tween.isPlaying = true;

                // Handle Sequence manually? No, sequence updates itself via Update(t) if added to tweens?
                // But Sequence Update logic below assumes it's driven. 
                // Let's rely on standard Tween update.
                
                tween.elapsed += Time.deltaTime;
                float t = tween.duration > 0 ? Mathf.Clamp01(tween.elapsed / tween.duration) : 1f;

                float easedT = tween.easeCurve != null 
                    ? tween.easeCurve.Evaluate(t) 
                    : EvaluateEase(tween.ease, t);

                tween.Update(easedT);

                if (tween.elapsed >= tween.duration)
                {
                    tween.onComplete?.Invoke();
                    tweens.RemoveAt(i);
                }
            }
        }

        public void AddTween(Tween tween)
        {
            tweens.Add(tween);
        }

        public void KillTweens(object target)
        {
            foreach (var t in tweens)
            {
                if (t.owner == target || (t.target != null && t.target == target))
                {
                    t.isKilled = true;
                }
            }
        }
        
        public bool IsTweening(object target)
        {
             foreach(var tw in tweens) 
             {
                 if (!tw.isKilled && (tw.owner == target || tw.target == target)) return true;
             }
             return false;
        }

        public float EvaluateEase(Ease ease, float t)
        {
            switch (ease)
            {
                case Ease.Linear: return t;
                case Ease.InQuad: return t * t;
                case Ease.OutQuad: return t * (2 - t);
                case Ease.InOutQuad: return t < .5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
                case Ease.InCubic: return t * t * t;
                case Ease.OutCubic: return (--t) * t * t + 1;
                case Ease.InOutCubic: return t < .5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
                case Ease.InBack: { float s = 1.70158f; return t * t * ((s + 1) * t - s); }
                case Ease.OutBack: { float s = 1.70158f; return --t * t * ((s + 1) * t + s) + 1; }
                case Ease.InOutBack: { float s = 1.70158f * 1.525f; return (t *= 2) < 1 ? 0.5f * (t * t * ((s + 1) * t - s)) : 0.5f * ((t -= 2) * t * ((s + 1) * t + s) + 2); }
                default: return t; 
            }
        }
    }

    public static class DOTween
    {
        public static Sequence Sequence()
        {
            var seq = new Sequence();
            Mover.Instance.AddTween(seq); // Auto-play
            return seq;
        }
        
        public static bool IsTweening(object target)
        {
            return Mover.Instance.IsTweening(target);
        }

        public static void Kill(object target)
        {
            Mover.Instance.KillTweens(target);
        }

        public static void ClearCachedTweens()
        {
            
        }
    }

    public static class DOVirtual
    {
        public static Tween DelayedCall(float delay, Action callback)
        {
            var t = new VirtualTween { duration = delay, onComplete = callback };
            Mover.Instance.AddTween(t);
            return t;
        }

        public static Tween Float(float from, float to, float duration, Action<float> onVirtualUpdate)
        {
            var t = new VirtualTween(from, to, duration, onVirtualUpdate);
            Mover.Instance.AddTween(t);
            return t;
        }
    }

    public class VirtualTween : Tween
    {
        float startValue;
        float endValue;
        Action<float> onUpdate;

        public VirtualTween() { }
        public VirtualTween(float startValue, float endValue, float duration, Action<float> onUpdate)
        {
            this.startValue = startValue;
            this.endValue = endValue;
            this.duration = duration;
            this.onUpdate = onUpdate;
        }

        public override void Update(float t)
        {
            onUpdate?.Invoke(Mathf.LerpUnclamped(startValue, endValue, t));
        }
    }

    public abstract class Tween
    {
        public Transform target; 
        public GameObject targetGameObject; 
        public object owner; 
        
        public float duration;
        public float elapsed;
        public Ease ease = Ease.Linear;
        public AnimationCurve easeCurve;
        public Action onComplete;
        public bool isKilled;
        public float delay;
        public bool isPlaying;

        public Tween SetEase(Ease easeType) { this.ease = easeType; return this; }
        public Tween SetEase(AnimationCurve curve) { this.easeCurve = curve; return this; }
        public Tween OnComplete(Action action) { this.onComplete = action; return this; }
        public Tween SetDelay(float delayTime) { this.delay = delayTime; return this; }
        public void Play() { this.isPlaying = true; }
        
        public virtual Tween From() { return this; }
        public virtual Tween From(float value) { return this; }
        public virtual Tween From(Vector3 value) { return this; }

        public abstract void Update(float t);
        
        public void Kill() { this.isKilled = true; }
    }

    public class Sequence : Tween
    {
        private Queue<Tween> queue = new Queue<Tween>();
        private Tween current;

        public Sequence()
        {
            this.duration = float.MaxValue; 
        }

        public void Append(Tween t)
        {
            t.isKilled = true; // Stop it from running on main loop
            t.isKilled = false; // Revive for sequence
            queue.Enqueue(t);
        }
        
        public void Join(Tween t) { Append(t); } 

        public override void Update(float t)
        {
            if (current == null)
            {
                if (queue.Count > 0)
                {
                    current = queue.Dequeue();
                    current.elapsed = 0;
                }
                else
                {
                    // Sequence finished
                    this.isKilled = true; 
                    this.onComplete?.Invoke();
                    return;
                }
            }

            if (current != null)
            {
                if (current.delay > 0)
                {
                    current.delay -= Time.deltaTime;
                }
                else
                {
                    current.elapsed += Time.deltaTime;
                    float progress = current.duration > 0 ? Mathf.Clamp01(current.elapsed / current.duration) : 1f;
                    
                    // Use Mover's ease evaluator
                    float easedProgress = Mover.Instance.EvaluateEase(current.ease, progress);
                    current.Update(easedProgress);
                }

                if (current.elapsed >= current.duration && current.delay <= 0)
                {
                    current.onComplete?.Invoke();
                    current = null;
                }
            }
        }
    }

    public class MoveTween : Tween
    {
        Vector3 startPos;
        Vector3 endPos;
        bool isLocal;
        bool xOnly;
        bool zOnly;

        public MoveTween(Transform target, Vector3 endPos, float duration, bool isLocal = false)
        {
            this.target = target;
            this.targetGameObject = target.gameObject;
            this.owner = target;
            this.duration = duration;
            this.isLocal = isLocal;
            this.startPos = isLocal ? target.localPosition : target.position;
            this.endPos = endPos;
        }
        
        public MoveTween(Transform target, float endCoord, float duration, bool isX) 
        {
            this.target = target;
            this.targetGameObject = target.gameObject;
            this.owner = target;
            this.duration = duration;
            this.startPos = target.position;
            this.endPos = target.position;
            if (isX) { this.endPos.x = endCoord; xOnly = true; }
            else { this.endPos.z = endCoord; zOnly = true; }
        }

        public override void Update(float t)
        {
            if (target == null) return;
            
            if (xOnly)
            {
                Vector3 p = target.position;
                p.x = Mathf.LerpUnclamped(startPos.x, endPos.x, t);
                target.position = p;
            }
            else if (zOnly)
            {
                Vector3 p = target.position;
                p.z = Mathf.LerpUnclamped(startPos.z, endPos.z, t);
                target.position = p;
            }
            else
            {
                Vector3 newPos = Vector3.LerpUnclamped(startPos, endPos, t);
                if (isLocal) target.localPosition = newPos;
                else target.position = newPos;
            }
        }

        public override Tween From()
        {
            Vector3 temp = startPos;
            startPos = endPos;
            endPos = temp;
            
            if (xOnly)
            {
               Vector3 p = target.position;
               p.x = startPos.x;
               target.position = p;
            }
            else if (zOnly)
            {
                Vector3 p = target.position;
                p.z = startPos.z;
                target.position = p;
            }
            else
            {
                if (isLocal) target.localPosition = startPos;
                else target.position = startPos;
            }
            return this;
        }

        public override Tween From(Vector3 value)
        {
            startPos = value;
            
            if (xOnly)
            {
               Vector3 p = target.position;
               p.x = startPos.x;
               target.position = p;
            }
            else if (zOnly)
            {
                Vector3 p = target.position;
                p.z = startPos.z;
                target.position = p;
            }
            else
            {
                if (isLocal) target.localPosition = startPos;
                else target.position = startPos;
            }
            return this;
        }
    }

    public class ScaleTween : Tween
    {
        Vector3 startScale;
        Vector3 endScale;

        public ScaleTween(Transform target, Vector3 endScale, float duration)
        {
            this.target = target;
            this.targetGameObject = target.gameObject;
            this.owner = target;
            this.duration = duration;
            this.startScale = target.localScale;
            this.endScale = endScale;
        }

        public override void Update(float t)
        {
             if (target == null) return;
             target.localScale = Vector3.LerpUnclamped(startScale, endScale, t);
        }

        public override Tween From()
        {
            Vector3 temp = startScale;
            startScale = endScale;
            endScale = temp;
            if(target != null) target.localScale = startScale;
            return this;
        }

        public override Tween From(float value)
        {
            startScale = new Vector3(value, value, value);
            if (target != null) target.localScale = startScale;
            return this;
        }

        public override Tween From(Vector3 value)
        {
            startScale = value;
            if (target != null) target.localScale = startScale;
            return this;
        }
    }

    public class RotateTween : Tween
    {
        Quaternion startRot;
        Quaternion endRot;
        Vector3 startEuler;
        Vector3 endEuler;
        bool isLocal;
        RotateMode mode;

        public RotateTween(Transform target, Vector3 endValue, float duration, bool isLocal = false, RotateMode mode = RotateMode.Fast)
        {
            this.target = target;
            this.targetGameObject = target.gameObject;
            this.owner = target;
            this.duration = duration;
            this.isLocal = isLocal;
            this.mode = mode;

            if (mode == RotateMode.WorldAxisAdd || mode == RotateMode.LocalAxisAdd)
            {
                this.startEuler = isLocal ? target.localEulerAngles : target.eulerAngles;
                // Fix startEuler to handle wrapping if necessary, but typically standard Euler is fine for add.
                // However, Unity's eulerAngles property returns 0-360. 
                // To rotate nicely, we just take it as is.
                this.endEuler = startEuler + endValue;
            }
            else
            {
                this.startRot = isLocal ? target.localRotation : target.rotation;
                this.endRot = Quaternion.Euler(endValue);
            }
        }

        public override void Update(float t)
        {
            if (target == null) return;
            
            if (mode == RotateMode.WorldAxisAdd || mode == RotateMode.LocalAxisAdd)
            {
                 Vector3 current = Vector3.LerpUnclamped(startEuler, endEuler, t);
                 if (isLocal) target.localEulerAngles = current;
                 else target.eulerAngles = current;
            }
            else
            {
                Quaternion newRot = Quaternion.SlerpUnclamped(startRot, endRot, t);
                if (isLocal) target.localRotation = newRot;
                else target.rotation = newRot;
            }
        }

        public override Tween From()
        {
            if (mode == RotateMode.WorldAxisAdd || mode == RotateMode.LocalAxisAdd)
            {
                 Vector3 temp = startEuler;
                 startEuler = endEuler;
                 endEuler = temp;
                 Update(0);
            }
            else
            {
                Quaternion temp = startRot;
                startRot = endRot;
                endRot = temp;
                Update(0);
            }
            return this;
        }

        public override Tween From(Vector3 value)
        {
             if (mode == RotateMode.WorldAxisAdd || mode == RotateMode.LocalAxisAdd)
            {
                 // For additive, From(value) implies starting from 'value' relative to target? 
                 // Or absolute value?
                 // Standard From(value) sets the start value to 'value'.
                 // But since we are additive, 'value' might be the offset?
                 // Let's assume absolute start value for simplicity or ignore specific complex From logic for additive.
                 // Actually, if From(Vector3) is called, it usually means 'start from this Euler rotation'.
                 startEuler = value;
                 // But endEuler was calculated based on original start + offset.
                 // If we change start, should we keep end relative? 
                 // Usually From changes start, end remains fixed (which is original start).
                 // In DOTween From() swaps start and end. From(val) sets start to val, end to original start.
                 
                 // If we used Additive, original intent was Current -> Current + Offset.
                 // From turns it into Current + Offset -> Current.
                 
                 // From(val) turns it into Val -> Current. 
                 // So endEuler should be original startEuler (captured before swap?).
                 // But we initialized endEuler = start + offset.
                 
                 // Let's just set startEuler = value. And keep endEuler as is (target).
                 // But wait, if From(val) is used, we expect to animate From Val To Target (where Target is object's position BEFORE tween started).
                 // So we need to ensure endEuler is the object's current state.
                 // Since we initialized endEuler = start + offset... that's for TO tween.
                 
                 // If From is used, logic usually handled by Tween settings.
                 // Here manual From implementation:
                 endEuler = isLocal ? (isLocal ? target.localEulerAngles : target.eulerAngles) : startEuler; // Actually we want to animate TO original state.
                 startEuler = value;
            }
            else
            {
                startRot = Quaternion.Euler(value);
                // And end should be original start? Yes.
                 // But we already set startRot = current, endRot = target.
                 // If From(val) is called: start = val, end = current.
                 endRot = isLocal ? target.localRotation : target.rotation; // Re-read current state as end
            }
            
            Update(0);
            return this;
        }
    }

    public class CanvasGroupFadeTween : Tween
    {
        CanvasGroup canvasGroup;
        float startAlpha;
        float endAlpha;

        public CanvasGroupFadeTween(CanvasGroup canvasGroup, float endAlpha, float duration)
        {
            this.canvasGroup = canvasGroup;
            this.targetGameObject = canvasGroup.gameObject;
            this.owner = canvasGroup;
            this.duration = duration;
            this.startAlpha = canvasGroup.alpha;
            this.endAlpha = endAlpha;
        }

        public override void Update(float t)
        {
            Debug.Log("Lerping alpha: " + t);
            if (canvasGroup == null) return;
            canvasGroup.alpha = Mathf.LerpUnclamped(startAlpha, endAlpha, t);
        }

        public override Tween From()
        {
            float temp = startAlpha;
            startAlpha = endAlpha;
            endAlpha = temp;
            if (canvasGroup != null) canvasGroup.alpha = startAlpha;
            return this;
        }
    }

    public class PathTween : Tween
    {
        Vector3[] path;
        PathType pathType;
        Vector3 startPos;
        List<Vector3> crPath;

        public PathTween(Transform target, Vector3[] path, float duration, PathType pathType)
        {
            this.target = target;
            this.targetGameObject = target.gameObject;
            this.owner = target;
            this.duration = duration;
            this.path = path;
            this.pathType = pathType;
            this.startPos = target.position;
            
            if (pathType == PathType.CatmullRom)
            {
                 List<Vector3> fullPath = new List<Vector3>();
                fullPath.Add(startPos);
                fullPath.AddRange(path);
                 
                 crPath = new List<Vector3>(fullPath);
                 crPath.Insert(0, fullPath[0]);
                 crPath.Add(fullPath[fullPath.Count - 1]);
            }
        }

        public override void Update(float t)
        {
            if (target == null) return;

            if (pathType == PathType.CatmullRom && crPath != null && crPath.Count >= 4)
            {
                 int numSections = crPath.Count - 3;
                 float currPt = t * numSections;
                 int currInt = (int)currPt;
                 if (currInt >= numSections) currInt = numSections - 1;
                 float u = currPt - currInt;
                 
                 Vector3 p0 = crPath[currInt];
                 Vector3 p1 = crPath[currInt + 1];
                 Vector3 p2 = crPath[currInt + 2];
                 Vector3 p3 = crPath[currInt + 3];

                 target.position = 0.5f * (
                    (2f * p1) +
                    (-p0 + p2) * u +
                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * u * u +
                    (-p0 + 3f * p1 - 3f * p2 + p3) * u * u * u
                );
            }
            else
            {
                 Vector3 endP = (path != null && path.Length > 0) ? path[path.Length-1] : startPos;
                  target.position = Vector3.Lerp(startPos, endP, t);
            }
        }
    }

    public class JumpTween : Tween
    {
        Vector3 startPos;
        Vector3 endPos;
        float jumpPower;
        int numJumps;
        bool isLocal;
        bool snapping;

        public JumpTween(Transform target, Vector3 endPos, float jumpPower, int numJumps, float duration, bool isLocal, bool snapping)
        {
            this.target = target;
            this.targetGameObject = target.gameObject;
            this.owner = target;
            this.duration = duration;
            this.startPos = isLocal ? target.localPosition : target.position;
            this.endPos = endPos;
            this.jumpPower = jumpPower;
            this.numJumps = numJumps;
            this.isLocal = isLocal;
            this.snapping = snapping;
        }

        public override void Update(float t)
        {
            if (target == null) return;

            float val = t * numJumps;
            float frac = val - Mathf.Floor(val);
            if (val >= numJumps && t >= 1f) frac = 0f;

            float height = 4 * jumpPower * frac * (1 - frac);

            Vector3 currentPos = Vector3.LerpUnclamped(startPos, endPos, t);
            currentPos.y += height;

            if (snapping)
            {
                currentPos.x = Mathf.Round(currentPos.x);
                currentPos.y = Mathf.Round(currentPos.y);
                currentPos.z = Mathf.Round(currentPos.z);
            }

            if (isLocal) target.localPosition = currentPos;
            else target.position = currentPos;
        }

        public override Tween From()
        {
            Vector3 temp = startPos;
            startPos = endPos;
            endPos = temp;

            if (isLocal) target.localPosition = startPos;
            else target.position = startPos;

            return this;
        }

        public override Tween From(Vector3 value)
        {
            startPos = value;
            if (isLocal) target.localPosition = startPos;
            else target.position = startPos;
            return this;
        }
    }

    public static class MoverExtensions
    {
        public static Tween DOMove(this Transform t, Vector3 endValue, float duration)
        {
            var tween = new MoveTween(t, endValue, duration);
            Mover.Instance.AddTween(tween);
            return tween;
        }
        
        public static Tween DOLocalMove(this Transform t, Vector3 endValue, float duration)
        {
            var tween = new MoveTween(t, endValue, duration, true);
            Mover.Instance.AddTween(tween);
            return tween;
        }
        
        public static Tween DOMoveX(this Transform t, float endValue, float duration)
        {
            var tween = new MoveTween(t, endValue, duration, true);
            Mover.Instance.AddTween(tween);
            return tween;
        }
        
        public static Tween DOMoveZ(this Transform t, float endValue, float duration)
        {
            var tween = new MoveTween(t, endValue, duration, false);
            Mover.Instance.AddTween(tween);
            return tween;
        }

        public static Tween DORotate(this Transform t, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast)
        {
            var tween = new RotateTween(t, endValue, duration, false, mode);
            Mover.Instance.AddTween(tween);
            return tween;
        }

        public static Tween DOLocalRotate(this Transform t, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast)
        {
            var tween = new RotateTween(t, endValue, duration, true, mode);
            Mover.Instance.AddTween(tween);
            return tween;
        }

        public static Tween DOScale(this Transform t, Vector3 endValue, float duration)
        {
            var tween = new ScaleTween(t, endValue, duration);
            Mover.Instance.AddTween(tween);
            return tween;
        }

        public static Tween DOScale(this Transform t, float endValue, float duration)
        {
            var tween = new ScaleTween(t, new Vector3(endValue, endValue, endValue), duration);
            Mover.Instance.AddTween(tween);
            return tween;
        }

        public static Tween DOPath(this Transform t, Vector3[] path, float duration, PathType pathType = PathType.Linear)
        {
             var tween = new PathTween(t, path, duration, pathType);
             Mover.Instance.AddTween(tween);
             return tween;
        }

        public static Tween DOJump(this Transform t, Vector3 endValue, float jumpPower, int numJumps, float duration, bool snapping = false)
        {
            var tween = new JumpTween(t, endValue, jumpPower, numJumps, duration, false, snapping);
            Mover.Instance.AddTween(tween);
            return tween;
        }

        public static Tween DOLocalJump(this Transform t, Vector3 endValue, float jumpPower, int numJumps, float duration, bool snapping = false)
        {
            var tween = new JumpTween(t, endValue, jumpPower, numJumps, duration, true, snapping);
            Mover.Instance.AddTween(tween);
            return tween;
        }

        public static void DOKill(this Transform t)
        {
            Mover.Instance.KillTweens(t);
        }

        public static Tween DOFade(this CanvasGroup cg, float endValue, float duration)
        {
            var tween = new CanvasGroupFadeTween(cg, endValue, duration);
            Mover.Instance.AddTween(tween);
            return tween;
        }
    }
}
