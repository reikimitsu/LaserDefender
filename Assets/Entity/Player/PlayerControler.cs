﻿using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {

	public GameObject projectile;
	public float health;
	public float speed;
	public float projectileSpeed;
	public float firingRate;
	public AudioClip fireSound;

	private float xmin;
	private float xmax;
	private float ymin;
	private float ymax;

	// Use this for initialization
	void Start () {
		restrictPosition ();
	}

	void restrictPosition () {
		float distance = transform.position.z - Camera.main.transform.position.z;

		Vector3 leftmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance));
		Vector3 rightmost = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distance));
		Vector3 upmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, distance));
		Vector3 downmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance));

		xmin = leftmost.x + 0.6f;
		xmax = rightmost.x - 0.6f;
		ymin = downmost.y + 0.5f;
		ymax = upmost.y - 0.5f;
	}

	// Update is called once per frame
	void Update () {
		movement ();
		shootProjectile ();
	}

	void movement () {
		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) { // Left movement
			gameObject.transform.position += Vector3.left * speed * Time.deltaTime;
		} else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) { // Right movement
			gameObject.transform.position += Vector3.right * speed * Time.deltaTime;
		}

		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W)) { // Up movement
			gameObject.transform.position += Vector3.up * speed * Time.deltaTime;
		} else if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S)) { // Down movement
			gameObject.transform.position += Vector3.down * speed * Time.deltaTime;
		}

		// Restrict the player to the gamespace
		float newX = Mathf.Clamp (transform.position.x, xmin, xmax);
		float newY = Mathf.Clamp (transform.position.y, ymin, ymax);

		transform.position = new Vector3 (newX, newY, transform.position.z);
	}

	void shootProjectile () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			InvokeRepeating ("Fire", 0.000001f, firingRate);
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			CancelInvoke ("Fire");
		}
	}

	void Fire () {
		GameObject beam = Instantiate (projectile, transform.position, Quaternion.identity) as GameObject;
		beam.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, projectileSpeed,0);

		AudioSource.PlayClipAtPoint (fireSound, transform.position);
	}

	void OnTriggerEnter2D (Collider2D col) {
		Projectile missile = col.gameObject.GetComponent<Projectile> ();

		if (missile) {
			health -= missile.GetDamage ();
			missile.Hit ();
			if (health <= 0) {
				Die ();
			}
		}
	}

	private void Die () {
		LevelManager man = GameObject.Find ("LevelManager").GetComponent<LevelManager>();
		man.LoadLevel ("Win");
		Destroy (gameObject);
	}
}
