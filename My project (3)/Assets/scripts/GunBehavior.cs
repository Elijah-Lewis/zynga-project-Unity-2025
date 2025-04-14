using UnityEngine;
using TMPro;
using System.Numerics;

public class GunBehavior : MonoBehaviour
{
    //bullet
    public GameObject bullet;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;


    // ammo variables
    [SerializeField] private int bulletsLeft, bulletsShot;
    [SerializeField] private int totalAmmo; //total amt of ammo
    public int maxAmmoCapacity = 30; //max amt of ammo

    //bools
    bool shooting, readyToShoot, reloading;

    //reference
    public Camera fpsCam;
    public Transform attackPoint;
    public float bulletMaxDistance = 100f;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    public TextMeshProUGUI totalAmmoDisplay;

    public bool allowInvoke = true;

    private void Awake()
    {
        //make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;

        //initialization of ammo capacity
        totalAmmo = 0; //scene begins with full ammo
    }

    private void Update()
    {
        MyInput();

        //set ammo display, if it exists
        UpdateAmmoDisplay();
    }

    private void UpdateAmmoDisplay()
    {
        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText($"{bulletsLeft / bulletsPerTap} / {magazineSize / bulletsPerTap}");
        }

        if (totalAmmoDisplay != null)
        {
            totalAmmoDisplay.SetText($"Ammo Left: {totalAmmo}");
        }
    }

    private void MyInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else 
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        //reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading && totalAmmo > 0)
        {
            Reload();
        }

        //reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0 && totalAmmo > 0)
        {
            Reload();
        }

        //shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //set bullets shot to 0
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        Ray ray = fpsCam.ViewportPointToRay(new UnityEngine.Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //check if ray hits something
        UnityEngine.Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else{
            targetPoint = ray.GetPoint(75);
        }

        //calculate direction from attackPoint to targetPoint
        UnityEngine.Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        UnityEngine.Vector3 directionWithSpread = directionWithoutSpread + new UnityEngine.Vector3(x, y, 0);

        //instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, UnityEngine.Quaternion.identity);

        //rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //instantiate muzzle flash if you have one
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, UnityEngine.Quaternion.identity);
        }

        bulletsLeft--;
        bulletsShot++;

        //invoke resetShot function (if not alreafy invoked)
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        //if mroe than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        int bulletsNeeded = magazineSize - bulletsLeft;
        int BulletsAvailable = totalAmmo;
        int bulletsToAdd = Mathf.Min(bulletsNeeded, BulletsAvailable);

        bulletsLeft += bulletsToAdd;
        totalAmmo -= bulletsToAdd;

        reloading = false;

        UpdateAmmoDisplay();

    }

    public void AddAmmo(int amount)
    {
        totalAmmo = Mathf.Min(totalAmmo + amount, maxAmmoCapacity);
    }
}
