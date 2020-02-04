using UnityEngine;
using UnityEngine.InputSystem;


public class Gun_script : MonoBehaviour
{	
	public ParticleSystem muzzleFlash;
	public Transform fpsCam;
	public float damage = 10f;
	public float range = 100f;
	public GameObject impacEffect;
	public float fireRate = 7f;  //amount of shots fired PER SEC//
	private float lastFired = 0f;

	void Update()
	{
		Mouse mouse = InputSystem.GetDevice<Mouse>();
		/*
		if (mouse.leftButton.wasPressedThisFrame)   USE THIS FOR SEMI-AUTO FIRE
		{
			Shoot();
		}
		*/
		
		if (mouse.leftButton.isPressed)
		{
			if (Time.time - lastFired > 1 / fireRate)
			{
				lastFired = Time.time;
				Shoot();
			}

		} 

	}
	

	void Shoot()
	{
		muzzleFlash.Play();

		RaycastHit hit;
		if (Physics.Raycast(fpsCam.position, fpsCam.forward, out hit, range))
		{
			Debug.Log(hit.transform.name);

			Target target =  hit.transform.GetComponent<Target>();
			if (target != null)
			{
				target.TakeDamage(damage);
            }

			Instantiate(impacEffect, hit.point, Quaternion.LookRotation(hit.normal));
			
		} 

    }

}
