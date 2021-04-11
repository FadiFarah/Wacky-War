using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class Snowball_Collision : MonoBehaviourPun
{
    //Assignables
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    //Stats
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    //Damage
    public int explosionDamage;
    public float explosionRange;
    public float explosionFroce;

    //Lifetime
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;
    public bool kill = false;

    int collisions;
    PhysicMaterial physics_mat;

    PhotonView target;

    //FirebaseManager fb;

    void Start()
    {
        Setup();
        target = GetLocalPlayer().GetComponent<PhotonView>();

    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            //When to explode
            if (collisions > maxCollisions) photonView.RPC("Explode", RpcTarget.All);
            //Count down lifetime
            maxLifetime -= Time.deltaTime;
            if (maxLifetime <= 0) photonView.RPC("Explode", RpcTarget.All);
        }
        
    }
    [PunRPC]
    private void Explode()
    {
        //Instantiate explosion
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);

        //Check for enemies
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for(int i = 0; i < enemies.Length; i++)
        {
            //Get component of enemy and call Take Damage

            //Just an example
            //enemies[i].GetComponent<ShootingAi>().TakeDamage(explosionDamage);
        }

        //Add a little delay, just to make sure everything works fine.
        Invoke("Delay", 0.05f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        //string name = fb.user.DisplayName;
        //string id = fb.user.UserId;
        //Don't count collisions with other bullets
        if (collision.collider.CompareTag("Bullet")) return;
        if(collision.transform.tag!="Player") target.GetComponent<inGameSoundsManager>().HitObjectSound();
        if (!gameObject.GetPhotonView().IsMine) return;
        if (collision.collider.GetComponent<PhotonView>()!=null && collision.collider.GetComponent<PhotonView>().IsMine) return;
        if (collision.transform.tag == "Player" && collision.transform.GetComponent<PhotonView>()!=target )
        {
            target.GetComponent<inGameSoundsManager>().HitPlayerSound();
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.player.health <= 0.5f)
            {
                target.GetComponent<PlayerController>().Kill();
            }
            player.photonView.RPC("GetDamage", RpcTarget.AllBuffered, 0.5f);
         
        }

       
        //Count up collisions
        //collisions++;

        //Explode if bullet gits an enemy directly and explodeOnTouch is activated
        if (explodeOnTouch) photonView.RPC("Explode", RpcTarget.All);

    }
    private void Delay()
    {
        Destroy(gameObject);
        //collisions = 0;
        //maxLifetime = 5;
        //GameObject.FindGameObjectWithTag("SnowParticle").GetComponent<Explode>().Invoke("DestroyParticle",1);
    }
     private void Setup()
    {
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
    GameObject GetLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                return player;
            }
        }
        return null;
    }
}
