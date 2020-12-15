using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State
{
    THREE,
    TWO,
    ONE,
    GO
}

public class StartMsgAnimation : MonoBehaviour
{
    [SerializeField] private List<Sprite> icons = null;
    [SerializeField] private AnimationCurve alphaCurve = null;
    [SerializeField] private AnimationCurve rotationCurve = null;
    [SerializeField] private AnimationCurve scaleCurve = null;
    [SerializeField] private AnimationCurve goScaleCurve = null;
    [SerializeField] private AnimationCurve goAlphaCurve = null;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float goFadeInDuration = 1f;
    [SerializeField] private float startSFX = 0.2f;
    private float _elpasedTime;
    [HideInInspector] public float _startTime;
    private int _iconIndex;
    private Image _placeholder;
    private float _step;
    private Vector3 _scale;
    private AudioSource _audioSource;

    public State state
    {
        get
        {
            switch (_iconIndex)
            {
                case 0: return State.THREE;
                case 1: return State.TWO;
                case 2: return State.ONE;
                case 3: return State.GO;
            }

            return State.THREE; //default
        }
    }

    private void Awake()
    {
        _iconIndex = 0;
        _placeholder = GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        StartScaleAnimation();
    }

    public void StartScaleAnimation()
    {
        _placeholder.sprite = icons[_iconIndex];
        _elpasedTime = Time.time - _startTime;
        _step = _elpasedTime / fadeInDuration;
        var tmp = _placeholder.color;

        // "Go!" icon got different animation curves and sfx
        if (_iconIndex == 3)
        {
            _scale = goScaleCurve.Evaluate(_step) * Vector3.one;
            _placeholder.rectTransform.rotation = Quaternion.identity;
            _audioSource.pitch = 1.65f;
            _step = _elpasedTime / goFadeInDuration;
            tmp.a = goAlphaCurve.Evaluate(_step);
        }
        else
        {
            _scale = scaleCurve.Evaluate(_step) * Vector3.one;
            _placeholder.rectTransform.rotation = Quaternion.Euler(0, 0, rotationCurve.Evaluate(_step));
            _audioSource.pitch = 1;
            tmp.a = alphaCurve.Evaluate(_step);
        }
        _placeholder.transform.localScale = _scale;
        _placeholder.color = tmp;

        if (_elpasedTime > startSFX && _elpasedTime < _audioSource.clip.length  && !_audioSource.isPlaying)
        {
            _audioSource.Play();
        }

        // execution repeat until we reach the end of animation curves
        if (_step < 1 && _iconIndex < 3) return;
        // if it display "GO!" the race begun
        if (_iconIndex == 3)
        {
            if (!GameManager.Instance.raceHasBegan()) GameManager.Instance.StartRace();
            if (_step < 2) return; // execution repeat until we reach the end of animation curves
            Destroy(gameObject);
        }

        // otherwise we are displaying the next icon and restart animation time.
        _iconIndex = (_iconIndex + 1) % icons.Count;
        _startTime = Time.time;
    }
}