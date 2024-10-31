using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{
    bool isJump = true;
    bool isDead = false;
    int idMove = 0;
    Animator anim;

    public GameObject Projectile;
    public Vector2 projectileVelocity;
    public Vector2 projectileOffset;
    public float cooldown = 0.5f;
    bool isCanShoot = true;
    // Use this for initialization
    private void Start()
    {
        anim = GetComponent<Animator>();
        isCanShoot = false;
        EnemyController.EnemyKilled = 0;
    }
    // Update is called once per frame 
    void Update()
    {
        //Debug.Log("Jump "+isJump); 
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            Idle();
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            Idle();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Fire();
        }
        Move();
        Dead();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Kondisi ketika menyentuh tanah 
        if (isJump)
        {
            anim.ResetTrigger("Jump");
            if (idMove == 0) anim.SetTrigger("Idle");
            isJump = false;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Kondisi ketika menyentuh tanah
        anim.SetTrigger("Jump");
        anim.ResetTrigger("Run");
        anim.ResetTrigger("Idle");
        isJump = true;
    }
    public void MoveRight()
    {
        idMove = 1;
    }
    public void MoveLeft()
    {
        idMove = 2;
    }
    private void Move()
    {
        if (idMove == 1 && !isDead)
        {
            // Kondisi ketika bergerak ke kekanan
            if (!isJump) anim.SetTrigger("Run");
            transform.Translate(1 * Time.deltaTime * 5f, 0, 0);
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        if (idMove == 2 && !isDead)
        {
            // Kondisi ketika bergerak ke kiri 
            if (!isJump) anim.SetTrigger("Run");
            transform.Translate(-1 * Time.deltaTime * 5f, 0, 0);
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    public void Jump()
    {
        if (!isJump)
        {
            // Kondisi ketika Loncat 
            gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 300f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals("Coin"))
        {
            Data.score += 15;
            Destroy(collision.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Peluru"))
        {
            isCanShoot = true;
        }
        if (collision.transform.tag.Equals("Enemy"))
        {
            SceneManager.LoadScene("Game Over");
            isDead = true;
        }
    }
    public void Idle()
    {
        // kondisi ketika idle/diam 
        if (!isJump)
        {
            anim.ResetTrigger("Jump");
            anim.ResetTrigger("Run");
            anim.SetTrigger("Idle");
        }
        idMove = 0;
    }
    private void Dead()
    {
        if (!isDead)
        {
            if (transform.position.y < -10f)
            {
                // kondisi ketika jatuh 
                isDead = true;
            }
        }
    }
    void Fire()
    {
        // jika karakter dapat menembak 
        if (isCanShoot)
        {
            //Membuat projectile baru 
            GameObject bullet = Instantiate(Projectile, (Vector2)transform.position - projectileOffset * transform.localScale.x, Quaternion.identity);

            // mengatur kecepatan dari projectile 
            Vector2 velocity = new Vector2(projectileVelocity.x * transform.localScale.x, projectileVelocity.y);
            bullet.GetComponent<Rigidbody2D>().velocity = velocity * -1;

            //Menyesuaikan scale dari projectile dengan scale karakter
            Vector3 scale = transform.localScale;
            bullet.transform.localScale = scale * -1;

            StartCoroutine(CanShoot());
            anim.SetTrigger("Shoot");
        }
    }
    IEnumerator CanShoot()
    {
        anim.SetTrigger("Shoot");
        isCanShoot = false;
        yield return new WaitForSeconds(cooldown);
        isCanShoot = true;
    }
}