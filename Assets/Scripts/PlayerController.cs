using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float moveSpeed;//Velocidad de movimiento, se utiliza float porque vamos a usar un numero y puede ser un decimal
    public float jumpForce; //Salto
    public float gravityScale = 5f; //Modifica la gravedad del personaje, hace que caiga mas lento
    public float bounceForce = 8f; //Modifica la velovidad del bounce
    //
    private Vector3 moveDirection; //Un vector3 significa que queremos 3 ejes (x y z), porque el juego es 3d

    public CharacterController charController;//mover el personaje

    private Camera theCam;//vistas de la camara

    public GameObject playerModel;
    public float rotateSpeed;

    public Animator anim;

    public bool isKnocking;//golpes
    public float knockBackLength = .5f;
    private float knockbackCounter;
    public Vector2 knockbackPower;

    public GameObject[] playerPieces;

    public bool stopMove;//detenerse

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        theCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isKnocking && !stopMove)
        {
            //Movimiento
            float yStore = moveDirection.y;
            moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));
            moveDirection.Normalize();
            moveDirection = moveDirection * moveSpeed;
            moveDirection.y = yStore;

            if (charController.isGrounded)
            {
                moveDirection.y = 0f;

                if (Input.GetButtonDown("Jump"))
                {
                    moveDirection.y = jumpForce;
                    //Debug.Log("Jump");
                }
            }

            moveDirection.y += Physics.gravity.y * Time.deltaTime * gravityScale;

            //transform.position = transform.position + (moveDirection * Time.deltaTime * moveSpeed);

            charController.Move(moveDirection * Time.deltaTime);

            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                transform.rotation = Quaternion.Euler(0f, theCam.transform.rotation.eulerAngles.y, 0f);
                Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
                //playerModel.transform.rotation = newRotation;

                playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
            }
        }


        if(isKnocking)
        {
            knockbackCounter -= Time.deltaTime;

            float yStore = moveDirection.y;
            moveDirection = playerModel.transform.forward * -knockbackPower.x;
            moveDirection.y = yStore;

            if (charController.isGrounded)
            {
                moveDirection.y = 0f;
            }

            moveDirection.y += Physics.gravity.y * Time.deltaTime * gravityScale;

            charController.Move(moveDirection * Time.deltaTime);

            if (knockbackCounter <= 0)
            {
                isKnocking = false;
            }
        }

        if(stopMove)
        {
            moveDirection = Vector3.zero;
            moveDirection.y += Physics.gravity.y * Time.deltaTime * gravityScale;
            charController.Move(moveDirection);
        }

        anim.SetFloat("Speed", Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z));
        anim.SetBool("Grounded", charController.isGrounded);
    }

    public void Knockback() //Retroceso del personaje al chocar con el enemigo
    {
        isKnocking = true;
        knockbackCounter = knockBackLength;  
        moveDirection.y = knockbackPower.y;
        charController.Move(moveDirection * Time.deltaTime);
    }

    public void Bounce() //Lo llamamos cada vez que querramos rebotar por ejemplo en la cabeza de un enemigo y asi hacerle daño
    {
        moveDirection.y = bounceForce; //aplicamos fuerza en el eje y
        charController.Move(moveDirection * Time.deltaTime); //movimiento del personaje
    }
}
