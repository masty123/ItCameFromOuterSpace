﻿using UnityEngine;
using System.Collections;

/**
 * Controller for weaponUI.
 * @author Theeruth Borisuth, Charin Tantrakul.
 */
public class Weapon : MonoBehaviour {
	public float fireRate = 0;
	public int Damage = 10;  
	public LayerMask whatToHit; 		//It will tell us what we want to hit.

    public int weaponNumber;
	public Transform BulletTrailPrefab;
    public Transform HitPrefab;
	public Transform MuzzleFlashPrefab;
	float timeToSpawnEffect = 0 ;
	public float effectSpawnRate = 10;

    //Handle camera shaking
    public float camShakeAmt = 0.05f;
    public float camShakeLength = 0.1f;
    CameraShake camShake;

    public string weaponShootSound = "DefaultShot";

    private KeyCode keyCode;
	float timeToFire = 0 ;  
	Transform firePoint; 	//Store firePoint.
	Transform endPoint;     // gun transform


    //Caching
    AudioManager audioManager;

     
	// Use this for initialization
	void Awake () {
		firePoint = transform.Find("FirePoint");
		endPoint = transform.Find("EndPoint");

		if (firePoint == null || endPoint == null) Debug.LogError ("No firepoint?? dafuq?");

	
	}

    void Start()
    {
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if (camShake == null) Debug.LogError("No CameraShake script found in object");

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audiomanager found in scene");
        }
    }

	//If left click, the gun will fire
	void Update () {
        if(weaponNumber== 2)
        {
            keyCode = KeyCode.RightControl;
        }
        else if(weaponNumber == 1)
        {
            keyCode = KeyCode.G;
        }
		if (fireRate == 0) {
			if (Input.GetKeyDown (keyCode)){
				Shoot();
			}
		}
		else {
				if (Input.GetKeyDown(keyCode) && Time.time > timeToFire){
					timeToFire = Time.time + 1/fireRate;
					Shoot();
				}
		}
	}

	//Make the bullet come out of the gun.
	void Shoot () {
		//Vector2 mousePosition = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint  (Input.mousePosition).y);
		Vector2 mousePosition = new Vector2 (endPoint.position.x,endPoint.position.y);

        
		Vector2 firePointPosition = new Vector2 (firePoint.position.x, firePoint.position.y);
		RaycastHit2D hit = Physics2D.Raycast (firePointPosition, mousePosition - firePointPosition, 100, whatToHit);
	

		Debug.DrawLine (firePointPosition,  (mousePosition - firePointPosition) * 100, Color.cyan );
		if (hit.collider != null) {
			Debug.DrawLine (firePointPosition, hit.point, Color.red);
			//Debug.Log ("We hit "+hit.collider.name + " and did " + Damage + " damage.");
			Enemy enemy = hit.collider.GetComponent<Enemy>();
			if(enemy != null){
				enemy.DamageEnemy(Damage);
			}
		}	

        if (Time.time >= timeToSpawnEffect)
        {
            Vector3 hitPos;
            Vector3 hitNormal;

            if (hit.collider == null)
            {
                hitPos = (mousePosition - firePointPosition) * 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }
            else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;

            }

            Effect(hitPos, hitNormal);
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

	//Effect of bullet, and the muzzle flash.
	void Effect (Vector3 hitPos, Vector3 hitNormal) {
		Transform trail = Instantiate (BulletTrailPrefab, firePoint.position, firePoint.rotation) as Transform;
        LineRenderer lr = trail.GetComponent<LineRenderer>();
        if (lr != null)
        {
            //SET POSITIONS
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }

        Destroy(trail.gameObject, 0.04f);

        if (hitNormal !=  new Vector3 (9999, 9999, 9999))
        {
            Transform hitParticle = Instantiate(HitPrefab, hitPos, Quaternion.FromToRotation(Vector3.right, hitNormal)) as Transform;
            Destroy(hitParticle.gameObject, 1f);
        }


        Transform clone = Instantiate (MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
		clone.parent = firePoint;
		float size = Random.Range (0.6f, 0.9f);
		clone.localScale = new Vector3 (size, size, size);
		Destroy (clone.gameObject, 0.02f);

        //Shake da camera
        camShake.Shake(camShakeAmt, camShakeLength);

        //Play shoot sound 
        audioManager.PlaySound(weaponShootSound);
	}


}
