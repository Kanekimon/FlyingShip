using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{

    public Rigidbody shipBody;
    public float Speed = 5f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;
    public float deceleration = 0.5f;
    public float engineForce = 10f;
    public float fuel = 100f;

    [SerializeField]
    private Vector3 _inputs = Vector3.zero;
    [SerializeField]
    private Vector3 _moveVector = Vector3.zero;
    private bool _isGrounded = true;
    [SerializeField]
    private Transform _groundChecker;
    [SerializeField]
    private Camera cam;
    private bool _keepHeight = false;
    private bool _upwardsMovement;
    [SerializeField]
    private float _maxAcceleration;
    private float _fuelUsage = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        CalcMaxAcc();
    }


    void CalcMaxAcc()
    {
        _maxAcceleration = (Physics.gravity.y * -1) + shipBody.mass + engineForce;
    }

    private void OnValidate()
    {
        CalcMaxAcc();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

        _inputs.x = Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");

        _moveVector = Vector3.zero;

        if (_inputs != Vector3.zero)
        {
            if (_inputs.z < 0)
                _moveVector += cam.transform.forward * -1;
            if (_inputs.z > 0)
                _moveVector += cam.transform.forward;
            if (_inputs.x < 0)
                _moveVector += cam.transform.right * -1;
            if (_inputs.x > 0)
                _moveVector += cam.transform.right;


            //_moveVector.x += _inputs.x;
            //_moveVector.z += _inputs.z;

        }
        else
        {
            if (_moveVector.x > 0 || _moveVector.x < 0)
                _moveVector.x = Mathf.Max(0, _moveVector.x - deceleration);
            if (_moveVector.z > 0 || _moveVector.z < 0)
                _moveVector.z = Mathf.Max(0, _moveVector.z - deceleration);
        }

        _moveVector.y = 0;

        if (Input.GetKey(KeyCode.Space))
        {

            _inputs.y +=  engineForce / ((Physics.gravity.y * -1) * shipBody.mass) ;
            _inputs.y = Mathf.Clamp(_inputs.y, 0f, _maxAcceleration);
            fuel -= _fuelUsage * 4;
            _upwardsMovement = true;
            _keepHeight = false;
        }
        else
        {
            _upwardsMovement = false;
        }


        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _keepHeight = !_keepHeight;
        }

        if (!_upwardsMovement)
        {
            if (_keepHeight && fuel > 0)
            {
                _inputs.y = (Physics.gravity.y *-1);
                fuel -= _fuelUsage;
            }
            else
            {
                _inputs.y = 0f;
                _keepHeight = false;
            }
        }

        if (!_keepHeight && _inputs.y > 0)
            shipBody.AddForce(0, _inputs.y, 0, ForceMode.Force);
        else if (_keepHeight)
            shipBody.velocity = new Vector3(shipBody.velocity.x, 0, shipBody.velocity.z);
        else
            shipBody.AddForce(0, Physics.gravity.y, 0, ForceMode.Force);





    }

    private void FixedUpdate()
    {
        if (_moveVector != Vector3.zero)
            shipBody.MovePosition(shipBody.position + _moveVector * Speed * Time.fixedDeltaTime);
    }
}
