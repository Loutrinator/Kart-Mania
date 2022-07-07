using Handlers;
using Items;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ItemWheelSlot : MonoBehaviour {
        [SerializeField] private Image image;

        private const float Rotation = 200f;
        private const float ResetAngleOffset = 45f;
        private bool _updated;
        private bool _hidden;
        private ItemData _item;

        private void Start() {
            UpdateItem();
        }

        private void Update() {
            if (!_updated) {
                //Debug.Log("transform.eulerAngles.z " + transform.eulerAngles.z);
                if (transform.rotation.eulerAngles.z > Rotation) {
                    _updated = true;
                    UpdateItem();
                }
            }
            else if (transform.eulerAngles.z < ResetAngleOffset) {
                _updated = false;
            }

            if (!_hidden) {
                if ((transform.eulerAngles.z + 180) % 360 > 350) {
                    _hidden = true;
                    image.enabled = false;
                }
            }
            else if ((transform.eulerAngles.z + 90) % 360 < 45) {
                _hidden = false;
                image.enabled = true;
            }
        }

        public bool IsPointedByArrow(int numberOfSubdivisions, float arrowAngle) {
            float angleOffset = 360f / (numberOfSubdivisions * 2f);
            float angle = (transform.rotation.eulerAngles.z + 360) % 360;
            float borneMin = (arrowAngle - angleOffset + 360) % 360;
            float borneMax = (arrowAngle + angleOffset + 360) % 360;
            return angle >= borneMin && angle < borneMax;
        }

        public ItemData GetItem() {
            return _item;
        }

        private void UpdateItem() {
            _item = RaceManager.Instance.itemManager.GetRandomItem(0);
            image.sprite = _item.GetIcon();
        }

        public void Reset() {
            _hidden = false;
            image.enabled = true;
        }
    }
}