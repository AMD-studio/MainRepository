using Assets.Scripts.Mechanics.Weapon;
using UnityEngine;

public class Bow : Weapon
{
    [System.Serializable]
    public class BowSettings
    {
        [Header("Arrow Settings")]
        public float arrowCount;
        public Rigidbody arrowPrefab;
        public Transform arrowPos;
        public Transform arrowEquipParent;
        public float arrowForce = 3;

        [Header("Bow Equip & UnEquip Settings")]
        public Transform EquipPos;
        public Transform UnEquipPos;

        public Transform UnEquipParent;
        public Transform EquipParent;

        [Header("Bow String Settings")]
        public Transform bowString;
        public Transform stringInitialPos;
        public Transform stringHandPullPos;
        public Transform stringInitialParent;

        [Header("Bow Audio Settings")]
        public AudioClip pullStringAudio;
        public AudioClip releaseStringAudio;
        public AudioClip drawArrowAudio;
    }

    public BowSettings bowSettings;

    [Header("Crosshair Settings")]
    public GameObject crossHairPrefab;
    GameObject currentCrossHair;

    Rigidbody currentArrow;

    bool canPullString = false;
    bool canFireArrow = false;

    AudioSource bowAudio;

    void Start()
    {
        bowAudio = GetComponent<AudioSource>();
    }

    void Update()
    {

    }

    public void PickArrow()
    {
        bowAudio.PlayOneShot(bowSettings.drawArrowAudio);
        bowSettings.arrowPos.gameObject.SetActive(true);
    }

    public void DisableArrow()
    {
        bowSettings.arrowPos.gameObject.SetActive(false);
    }

    public void PullString()
    {
        bowSettings.bowString.transform.position = bowSettings.stringHandPullPos.position;
        bowSettings.bowString.transform.parent = bowSettings.stringHandPullPos;
    }

    public void ReleaseString()
    {
        bowSettings.bowString.transform.position = bowSettings.stringInitialPos.position;
        bowSettings.bowString.transform.parent = bowSettings.stringInitialParent;
    }

    public void EquipBow()
    {
        transform.SetPositionAndRotation(bowSettings.EquipPos.position, bowSettings.EquipPos.rotation);
        transform.parent = bowSettings.EquipParent;
    }

    public void UnEquipBow()
    {
        transform.SetPositionAndRotation(bowSettings.UnEquipPos.position, bowSettings.UnEquipPos.rotation);
        transform.parent = bowSettings.UnEquipParent;
    }

    public void ShowCrosshair(Vector3 crosshairPos)
    {
        if (!currentCrossHair)
            currentCrossHair = Instantiate(crossHairPrefab);

        currentCrossHair.transform.position = crosshairPos;
        currentCrossHair.transform.LookAt(Camera.main.transform);
    }

    public void RemoveCrosshair()
    {
        if (currentCrossHair)
            Destroy(currentCrossHair);
    }

    public void PullAudio()
    {
        bowAudio.PlayOneShot(bowSettings.pullStringAudio);
    }

    public override void PickWeapon()
    {
        // Implement logic for picking the bow as a weapon
        // For example, play a sound or show a visual effect
        Debug.Log("Picking the bow");
    }

    public override void DisableWeapon()
    {
        // Implement logic for disabling the bow as a weapon
        // For example, play a sound or hide a visual effect
        Debug.Log("Disabling the bow");
    }

    public override void Fire(Vector3 direction)
    {
        // Implement the specific firing logic for the bow
        if (bowSettings.arrowCount < 1)
            return;

        bowAudio.PlayOneShot(bowSettings.releaseStringAudio);

        currentArrow = Instantiate(
            bowSettings.arrowPrefab,
            bowSettings.arrowPos.position,
            bowSettings.arrowPos.rotation);

        currentArrow.AddForce(direction * bowSettings.arrowForce, ForceMode.Force);

        bowSettings.arrowCount -= 1;
    }
}