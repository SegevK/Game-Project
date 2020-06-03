using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;


// Gun Script
public class HandgunScript : NetworkBehaviour
{

	#region PlayerSettings
	
	public PlayerWeapon weapon; //from the weapon class

	private int damage;
	private int range;
	private int ammo;
	private float fireRate;
	private int weaponUsedOnPress = 1;
	private float lastFired;

	[SerializeField]
	private LayerMask mask;

	  //false on start,when the player spawns with the pistol weapon

	//Animator component attached to weapon
	public Animator anim;

	[Header("UI Components")]
	[SerializeField]   //used for the ui ammo counter
	public Text currentAmmoText;

	[Header("Gun Camera")]
	//Main gun camera
	public Camera gunCamera;

	[Header("Gun Camera Options")]
	//How fast the camera field of view changes when aiming 
	[Tooltip("How fast the camera field of view changes when aiming.")]
	public float fovSpeed = 15.0f;
	//Default camera field of view
	[Tooltip("Default value for camera field of view (40 is recommended).")]
	public float defaultFov = 40.0f;

	public float aimFov = 15.0f;

	[Header("UI Weapon Name")]
	[Tooltip("Name of the current weapon, shown in the game UI.")]
	public string weaponName;
	private string storedWeaponName;

	[Header("Weapon Sway")]
	//Enables weapon sway
	[Tooltip("Toggle weapon sway.")]
	public bool weaponSway;  //causes some problems,for now dont activate

	public float swayAmount = 0.02f;
	public float maxSwayAmount = 0.06f;
	public float swaySmoothValue = 4.0f;

	private Vector3 initialSwayPosition;

	[Header("Weapon Settings")]

	public float sliderBackTimer = 1.58f;
	private bool hasStartedSliderBack;

	//Eanbles auto reloading when out of ammo
	[Tooltip("Enables auto reloading when out of ammo.")]
	public bool autoReload;
	//Delay between shooting last bullet and reloading
	public float autoReloadDelay;
	//Check if reloading
	private bool isReloading;

	//Holstering weapon
	private bool hasBeenHolstered = false;
	//If weapon is holstered
	private bool holstered;
	//Check if running
	private bool isRunning;
	//Check if aiming
	private bool isAiming;
	//Check if walking
	private bool isWalking;
	//Check if inspecting weapon
	private bool isInspecting;

	//How much ammo is currently left
	private int currentAmmo;
	//Totalt amount of ammo
	
	//Check if out of ammo
	private bool outOfAmmo;


	// DISABLE BULLET SETTINGS BECAUSE WE USE RAYCASTS AND NOT PHYSICS OBJECTS 
	[Header("Bullet Settings")]
	//Bullet
	[Tooltip("How much force is applied to the bullet when shooting.")]
	public float bulletForce = 400;
	[Tooltip("How long after reloading that the bullet model becomes visible " +
		"again, only used for out of ammo reload aniamtions.")]
	public float showBulletInMagDelay = 0.6f;
	[Tooltip("The bullet model inside the mag, not used for all weapons.")]
	public SkinnedMeshRenderer bulletInMagRenderer;



	[Header("Grenade Settings")]
	public float grenadeSpawnDelay = 0.35f;

	[Header("Muzzleflash Settings")]
	public bool randomMuzzleflash = false;
	//min should always bee 1
	private int minRandomValue = 1;

	[Range(2, 25)]
	public int maxRandomValue = 5;

	private int randomMuzzleflashValue;

	// ADD SOUND TO THE IMPACT EFFECTS(ALREADY EXISTS IN PREFAB)
	public ParticleSystem HitEffect; //the impact prefab effects
	public bool enableMuzzleflash = true;
	public ParticleSystem muzzleParticles;
	public bool enableSparks = true;
	public ParticleSystem sparkParticles;
	public int minSparkEmission = 1;
	public int maxSparkEmission = 7;

	[Header("Muzzleflash Light Settings")]
	public Light muzzleflashLight;
	public float lightDuration = 0.02f;

	[Header("Audio Source")]
	//Main audio source
	public AudioSource mainAudioSource;
	//Audio source used for shoot sound
	public AudioSource shootAudioSource;


	[System.Serializable]
	public class prefabs
	{
		[Header("Prefabs")]
		public Transform bulletPrefab;
		public Transform casingPrefab;
		public Transform grenadePrefab;
	}
	public prefabs Prefabs;

	[System.Serializable]
	public class spawnpoints
	{
		[Header("Spawnpoints")]
		//Array holding casing spawn points 
		//Casing spawn point array
		public Transform casingSpawnPoint;
		//Bullet prefab spawn from this point
		public Transform bulletSpawnPoint;
		//Grenade prefab spawn from this point
		public Transform grenadeSpawnPoint;
	}
	public spawnpoints Spawnpoints;

	[System.Serializable]
	public class soundClips
	{
		public AudioClip shootSound;
		public AudioClip takeOutSound;
		public AudioClip holsterSound;
		public AudioClip reloadSoundOutOfAmmo;
		public AudioClip reloadSoundAmmoLeft;
		public AudioClip aimSound;
	}
	public soundClips SoundClips;

	private bool soundHasPlayed = false;

	#endregion


	#region InGame
	private void Awake()
	{
		//Set current ammo to total ammo value and setup the weapon(1 or 2)
	    ammo = weapon.ammo1;
		damage = weapon.damage1;
		range = weapon.range1;
		fireRate = weapon.fireRate;
		currentAmmo = ammo;
		muzzleflashLight.enabled = false;
	}

	private void Start()
	{
		//Weapon sway
		initialSwayPosition = transform.localPosition;
		//Set the shoot sound to audio source
		shootAudioSource.clip = SoundClips.shootSound;
	}


	private void SwitchWeapon()    //changes weapon type and resets ammo
	{
		if (weaponUsedOnPress == 1)
		{
			weaponUsedOnPress = 2;
			ammo = weapon.ammo2;
			damage = weapon.damage2;
			range = weapon.range2;
			currentAmmo = 0;
		}
		
		else
		{
			weaponUsedOnPress = 1;
			ammo = weapon.ammo1;
			damage = weapon.damage1;
			range = weapon.range1;
			currentAmmo = 0;
		}
	}


	private void Update()
	{
		if (PauseMenu.IsOn)
		{
			if (isWalking)
			{
				anim.SetBool("Walk", false);
				isWalking = false;
			}

			if (isRunning)
			{
				anim.SetBool("Run", false);
				isRunning = false;
			}

			return;
		}

		currentAmmoText.text = currentAmmo.ToString();  //changes the displayed ammo number every frame

		if ( (Input.GetKeyDown(KeyCode.Alpha2)) && !(weaponUsedOnPress == 2) && (!isReloading) )
			SwitchWeapon();
		
		if ((Input.GetKeyDown(KeyCode.Alpha1)) && !(weaponUsedOnPress == 1) && (!isReloading) )
			SwitchWeapon();

		/*    used for testing mouse cursur in game
				if (Input.GetKeyDown(KeyCode.L))  //press L key to hide the cursur in-game
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
				}
				if (Input.GetKeyDown(KeyCode.U))   //press U key to Unhide the cursur in-game
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
		*/


		//Aiming
		//Toggle camera FOV when right click is held down   -- for zoom in when aiming
		if (Input.GetButton("Fire2") && !isReloading && !isRunning && !isInspecting)
		{
			gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView,
				aimFov, fovSpeed * Time.deltaTime);
			isAiming = true;
			anim.SetBool("Aim", true);

			if (!soundHasPlayed)
			{
				mainAudioSource.clip = SoundClips.aimSound;
				mainAudioSource.Play();
				soundHasPlayed = true;
			}
		}

		else
		{
			//When right click is released
			gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView, defaultFov, fovSpeed * Time.deltaTime);
			isAiming = false;
			anim.SetBool("Aim", false);
		}
		//Aiming end


		//If randomize muzzleflash is true, genereate random int values
		if (randomMuzzleflash == true)
		{
			randomMuzzleflashValue = Random.Range(minRandomValue, maxRandomValue);
		}

		//Continosuly check which animation 
		//is currently playing
		AnimationCheck();

		//If out of ammo
		if (currentAmmo == 0)
		{
			//Toggle bool
			outOfAmmo = true;
			//Auto reload if true
			if (autoReload == true && !isReloading)
			{
				StartCoroutine(AutoReload());
			}

			//Set slider back
			anim.SetBool("Out Of Ammo Slider", true);
			//Increase layer weight for blending to slider back pose
			anim.SetLayerWeight(1, 1.0f);
		}
		else
		{
			//Toggle bool
			outOfAmmo = false;
			//anim.SetBool ("Out Of Ammo", false);
			anim.SetLayerWeight(1, 0.0f);
		}


		//Shooting 
		if (weaponUsedOnPress == 1)
		{
			if (Input.GetMouseButtonDown(0) && !outOfAmmo && !isReloading && !isRunning)
				Shoot();
		}

		else
		{
			if (Input.GetButton("Fire1") && !outOfAmmo && !isReloading && !isRunning)  //automatic fire
			{
				if (Time.time - lastFired > 1 / fireRate)
				{
					lastFired = Time.time;
					Shoot();
				}
					
			}
		}

				/* FEATURES THAT I DIDNT HAVE TIME TO IMPLEMENT INTO THE GAME 
				//Inspect weapon when pressing T key
				if (Input.GetKeyDown(KeyCode.T))
				{
					anim.SetTrigger("Inspect");
				}

				//Toggle weapon holster when pressing E key
				if (Input.GetKeyDown(KeyCode.E) && !hasBeenHolstered)
				{
					holstered = true;

					mainAudioSource.clip = SoundClips.holsterSound;
					mainAudioSource.Play();

					hasBeenHolstered = true;
				}
				else if (Input.GetKeyDown(KeyCode.E) && hasBeenHolstered)
				{
					holstered = false;

					mainAudioSource.clip = SoundClips.takeOutSound;
					mainAudioSource.Play();

					hasBeenHolstered = false;
				}

				//Holster anim toggle
				if (holstered == true)
				{
					anim.SetBool("Holster", true);
				}
				else
				{
					anim.SetBool("Holster", false);
				}
				*/

				//Reload 
		if (Input.GetKeyDown(KeyCode.R) && !isReloading )
		{
			//Reload
			Reload();
			if (!hasStartedSliderBack)
			{
				hasStartedSliderBack = true;
				StartCoroutine(HandgunSliderBackDelay());
			}
		}


		//Walking when pressing down WASD keys
		if (Input.GetKey(KeyCode.W) && !isRunning ||
			Input.GetKey(KeyCode.A) && !isRunning ||
			Input.GetKey(KeyCode.S) && !isRunning ||
			Input.GetKey(KeyCode.D) && !isRunning)
		{
			isWalking = true;
			anim.SetBool("Walk", true);
		}
		else
		{
			isWalking = false;
			anim.SetBool("Walk", false);
		}

		//Running when pressing down W and Left Shift key
		if ((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift)))
		{
			isRunning = true;
		}
		else
		{
			isRunning = false;
		}

		//Run anim toggle
		if (isRunning)
		{
			anim.SetBool("Run", true);
		}
		else
		{
			anim.SetBool("Run", false);
		}
	}

	private IEnumerator HandgunSliderBackDelay()
	{
		//Wait set amount of time
		yield return new WaitForSeconds(sliderBackTimer);
		//Set slider back
		anim.SetBool("Out Of Ammo Slider", false);
		//Increase layer weight for blending to slider back pose
		anim.SetLayerWeight(1, 0.0f);

		hasStartedSliderBack = false;
	}

	private IEnumerator GrenadeSpawnDelay()
	{
		//Wait for set amount of time before spawning grenade
		yield return new WaitForSeconds(grenadeSpawnDelay);
		//Spawn grenade prefab at spawnpoint
		Instantiate(Prefabs.grenadePrefab,
			Spawnpoints.grenadeSpawnPoint.transform.position,
			Spawnpoints.grenadeSpawnPoint.transform.rotation);
	}

	private IEnumerator AutoReload()
	{
		if (!hasStartedSliderBack)
		{
			hasStartedSliderBack = true;

			StartCoroutine(HandgunSliderBackDelay());
		}
		//Wait for set amount of time
		yield return new WaitForSeconds(autoReloadDelay);

		if (outOfAmmo == true)
		{
			//Play diff anim if out of ammo
			anim.Play("Reload Out Of Ammo", 0, 0f);

			mainAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
			mainAudioSource.Play();

			//If out of ammo, hide the bullet renderer in the mag
			//Do not show if bullet renderer is not assigned in inspector
			if (bulletInMagRenderer != null)
			{
				bulletInMagRenderer.GetComponent
				<SkinnedMeshRenderer>().enabled = false;
				//Start show bullet delay
				StartCoroutine(ShowBulletInMag());
			}
		}

		//Restore ammo when reloading
		currentAmmo = ammo;
		outOfAmmo = false;
	}

	//Reload
	private void Reload()
	{

		if (outOfAmmo == true)
		{
			//Play diff anim if out of ammo
			anim.Play("Reload Out Of Ammo", 0, 0f);

			mainAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
			mainAudioSource.Play();

			//If out of ammo, hide the bullet renderer in the mag
			//Do not show if bullet renderer is not assigned in inspector
			if (bulletInMagRenderer != null)
			{
				bulletInMagRenderer.GetComponent
				<SkinnedMeshRenderer>().enabled = false;
				//Start show bullet delay
				StartCoroutine(ShowBulletInMag());
			}
		}
		else
		{
			//Play diff anim if ammo left
			anim.Play("Reload Ammo Left", 0, 0f);

			mainAudioSource.clip = SoundClips.reloadSoundAmmoLeft;
			mainAudioSource.Play();

			//If reloading when ammo left, show bullet in mag
			//Do not show if bullet renderer is not assigned in inspector
			if (bulletInMagRenderer != null)
			{
				bulletInMagRenderer.GetComponent
				<SkinnedMeshRenderer>().enabled = true;
			}
		}
		//Restore ammo when reloading
		currentAmmo = ammo;
		outOfAmmo = false;
	}   //Reload end



	//Enable bullet in mag renderer after set amount of time
	private IEnumerator ShowBulletInMag()
	{
		//Wait set amount of time before showing bullet in mag
		yield return new WaitForSeconds(showBulletInMagDelay);
		bulletInMagRenderer.GetComponent<SkinnedMeshRenderer>().enabled = true;
	}

	//Show light when shooting, then disable after set amount of time
	private IEnumerator MuzzleFlashLight()
	{
		muzzleflashLight.enabled = true;
		yield return new WaitForSeconds(lightDuration);
		muzzleflashLight.enabled = false;
	}

	//Check current animation playing
	private void AnimationCheck()
	{
		//Check if reloading
		//Check both animations
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reload Out Of Ammo") ||
			anim.GetCurrentAnimatorStateInfo(0).IsName("Reload Ammo Left"))
		{
			isReloading = true;
		}
		else
		{
			isReloading = false;
		}

		//Check if inspecting weapon
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Inspect"))
		{
			isInspecting = true;
		}
		else
		{
			isInspecting = false;
		}
	}

	#endregion


	// Might change raycast to server to prevent cheaters,but can cause alot of problems //

	[Client]  //runs only on client sides, and not on the server
	void Shoot()
	{
		if (!isLocalPlayer)   //causes the shoot method to activate for only one player- the local client
		{
			return;
		}
		
		//Remove 1 bullet from ammo
		currentAmmo -= 1;
		shootAudioSource.clip = SoundClips.shootSound;
		shootAudioSource.Play();

		CmdOnShoot();

		RaycastHit shot;
		if (Physics.Raycast(Spawnpoints.bulletSpawnPoint.transform.position, Spawnpoints.bulletSpawnPoint.transform.forward, out shot, range, mask))
		{
			string Name = shot.transform.gameObject.name;
			string Tag = shot.transform.gameObject.tag;
			if (Tag == "Player")
			{
				Debug.Log("The TAG " + Tag);
				Debug.Log("hit " + Name);
				CmdPlayerShot(Name, damage);
				Debug.Log("Ended the Shoot function + sent data to cmd function");
			}

			else
			{
				Debug.Log("hit " + shot.transform.gameObject.name);
			}

			CmdOnBulletHit(shot.point, shot.normal);

		}
	}
	
	//commands are methods/functions that are called/executed only on the server side.
	//runs the function on the server using data on the client,and the client needs to have a NetId to use a command method
	[Command]
	void CmdPlayerShot(string PlayerName, int weaponDmg)
	{
		Debug.Log("This Is the CmdPlayerShot Function");
		Debug.Log(PlayerName + " has been shot.");

		PlayerManager Player = GameManager.GetPlayer(PlayerName);
		Player.RpcTakeDamage(weaponDmg);
	}

	//called on the server side when a player shoots
	[Command]
	void CmdOnShoot()   //this method is used for CALLING ANOTHER METHOD ON ALL THE OTHER CLIENTS,for displaying the weapon gfx(muzzleflash,etc..)
	{
		RpcDoShootGfx();
	}

	//is called on all clients when a player shoots
	//so everyone will see the weapon gfx from each player
	[ClientRpc]
	void RpcDoShootGfx()
	{
		anim.Play("Fire", 0, 0f);
		muzzleParticles.Emit(1);
		//Light flash start
		StartCoroutine(MuzzleFlashLight());

		if (!isAiming) //if not aiming
		{
			anim.Play("Fire", 0, 0f);
			muzzleParticles.Emit(1);

			if (enableSparks == true)
			{
				//Emit random amount of spark particles
				sparkParticles.Emit(Random.Range(1, 6));
			}
		}
		else //if aiming
		{
			anim.Play("Aim Fire", 0, 0f);
			//If random muzzle is false
			if (!randomMuzzleflash)
			{
				muzzleParticles.Emit(1);
				//If random muzzle is true
			}
			else if (randomMuzzleflash == true)
			{
				//Only emit if random value is 1
				if (randomMuzzleflashValue == 1)
				{
					if (enableSparks == true)
					{
						//Emit random amount of spark particles
						sparkParticles.Emit(Random.Range(1, 6));
					}
					if (enableMuzzleflash == true)
					{
						muzzleParticles.Emit(1);
						//Light flash start
						StartCoroutine(MuzzleFlashLight());
					}
				}
			}
		}
	}


	[Command]    //this method is for sending the impact effect to all the players
	void CmdOnBulletHit(Vector3 pos, Vector3 normal)  //the hit position and the normal of the surface that got hit(normal is a vector3 that points up-90 degrees from the surface)
	{
		RpcDoImpactEffect(pos, normal);
	}


	// CHECK OUT OBJECT-PULLING = A MORE EFFICENT WAY TO SPAWN OBJECTS,PERFORMANCE WISE
	[ClientRpc]   //sends the impact effect to all the clients
	void RpcDoImpactEffect(Vector3 pos, Vector3 normal)
	{
		GameObject destroyEffect = Instantiate(HitEffect.gameObject, pos, Quaternion.LookRotation(normal));
		Destroy(destroyEffect, 0.4f); //the float value sent to destroy method is the amount of time that the object stays alive
	}

}

