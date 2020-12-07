using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State{THREE,TWO,ONE,GO}

public class StartMsgAnimation : MonoBehaviour
{
    [SerializeField] private List<Sprite> icons = null;
    [SerializeField] private AnimationCurve alphaCurve = null;
    [SerializeField] private AnimationCurve rotationCurve = null;
    [SerializeField] private AnimationCurve scaleCurve = null;
    [SerializeField] private AnimationCurve goScaleCurve = null;
    [SerializeField] private float fadeInDuration = 1f;
    private float _elpasedTime;
    [HideInInspector] public float startTime;
    private int _iconIndex;
    private Image _placeholder;
    private float _step;
    private Vector3 _scale;
    public State state
    {
        get
        {
            switch (_iconIndex)
            {
                case 0 : return State.THREE;
                case 1 : return State.TWO;
                case 2 : return State.ONE;
                case 3 : return State.GO;
            }
            return State.THREE; //default
        }
    }

    private void Awake()
    {
        _iconIndex = 0;
        _placeholder = GetComponent<Image>();
    }

    private void Update()
    {
        StartScaleAnimation();
    }

    public void StartScaleAnimation()
    {
      
        if(GameManager.Instance.raceHasBegan()) return;
        _placeholder.sprite = icons[_iconIndex];
        _elpasedTime = Time.time - startTime;
        _step = _elpasedTime / fadeInDuration;
        if (_iconIndex == 3)
        {
            _scale = goScaleCurve.Evaluate(_step) * Vector3.one;
        }
        else
        {
            _scale = scaleCurve.Evaluate(_step) * Vector3.one;
            _placeholder.rectTransform.rotation = Quaternion.Euler(0,0,rotationCurve.Evaluate(_step));
        }
       
        _placeholder.transform.localScale = _scale;
        var tmp = _placeholder.color;
        tmp.a = alphaCurve.Evaluate(_step);
        _placeholder.color = tmp;
        if (_step < 1) return;
        if (_iconIndex > 2)
        {
            StartCoroutine(GameManager.Instance.startRace());
          return;
        }
        _iconIndex = (_iconIndex + 1) % icons.Count;
        startTime = Time.time;
    }
   
}