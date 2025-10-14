using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public GameObject shoot_effect;
	public GameObject hit_effect;
	public GameObject firing_ship;
	
	public float speed = 15f;
	public float lifetime = 10f;
	
	private Rigidbody2D rb; // THÊM DÒNG NÀY
	
	void Start () {
    rb = GetComponent<Rigidbody2D>();
    
    // KIỂM TRA NULL TRƯỚC KHI DÙNG
    if (firing_ship != null) {
        Collider2D shipCollider = firing_ship.GetComponent<Collider2D>();
        Collider2D projectileCollider = GetComponent<Collider2D>();
        
        if (shipCollider != null && projectileCollider != null) {
            Physics2D.IgnoreCollision(projectileCollider, shipCollider);
        }
    }
    
    if (rb != null) {
        rb.linearVelocity = Vector2.up * speed;
    }
    
    GameObject obj = (GameObject) Instantiate(shoot_effect, transform.position - new Vector3(0,0,5), Quaternion.identity);
    obj.transform.parent = firing_ship.transform;
    Destroy(gameObject, lifetime);
}
	
	void Update () {
		// XÓA code Translate đi, dùng Rigidbody2D
	}
	
	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject != firing_ship && col.gameObject.tag != "Projectile") {
			Instantiate(hit_effect, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}
	}
}