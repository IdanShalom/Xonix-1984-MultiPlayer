using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    private const int LIMITS_LAYER = 6;
    private const int ENEMY_IN_SEA_LYAER = 7;
    private const int ENEMY_LAYER = 8;
    private const int LAND_LAYER = 9;
    private const int ENEMY_LAND_LAYER = 12;
    private const float START_VELOCITY_RANGE = 1f;
    private const float CLAMP_MIN = 15;
    private const float CLAMP_MAX = 70;
    private const float X_RAYCAST_DEV = 0.13f;
    private const float Y_RAYCAST_DEV = 0.15f;
    private Rigidbody2D _rigidbody;
    private Vector2 prevVelocity;
    [SerializeField] private float _maxDistance;
    [SerializeField] private LayerMask _seaSpaceLayer;
    [SerializeField] private LayerMask _landSpaceLayer;
    [SerializeField] private LayerMask _backGroundLayer;
    [Range(17, 100)] public float speed = 17;
    
   void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        setVelocity();
        prevVelocity = _rigidbody.velocity.normalized;
        _rigidbody.drag = 0;
        _rigidbody.gravityScale = 0;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        Physics2D.IgnoreLayerCollision(LIMITS_LAYER,ENEMY_IN_SEA_LYAER);
        Physics2D.IgnoreLayerCollision(LAND_LAYER,ENEMY_IN_SEA_LYAER);
        Physics2D.IgnoreLayerCollision(LAND_LAYER,ENEMY_LAYER);
        Physics2D.IgnoreLayerCollision(ENEMY_LAND_LAYER,ENEMY_LAYER);
        Physics2D.IgnoreLayerCollision(ENEMY_LAND_LAYER,LAND_LAYER);
    }

   private void setVelocity()
   {
       _rigidbody.velocity = new Vector2(Random.Range(-START_VELOCITY_RANGE,START_VELOCITY_RANGE), Random.Range(-START_VELOCITY_RANGE,START_VELOCITY_RANGE))*speed;
   }

   private void FixCollisionVelocity(Vector2 contactNormal)
   {
       float angleDeg = Vector2.SignedAngle(contactNormal, -prevVelocity);
       float absDeg = Mathf.Abs(angleDeg);
       float clampedDeg = Mathf.Clamp(absDeg, CLAMP_MIN, CLAMP_MAX);
       if (Mathf.Approximately(absDeg, clampedDeg))
           return;
       float signedRad = clampedDeg * Mathf.Sign(angleDeg) * Mathf.Deg2Rad;
       float normalRad = Mathf.Atan2(contactNormal.y, contactNormal.x);
       float newRad = normalRad + signedRad;
       Vector2 newDir = new Vector2(Mathf.Cos(newRad), Mathf.Sin(newRad));
       float magnitude = prevVelocity.magnitude; // vector’s length
       prevVelocity = newDir * -magnitude;
   }

   private void OnCollisionEnter2D(Collision2D other)
   {
       ContactPoint2D contact = other.contacts[0];
       Vector2 contactNormal = contact.normal;
       FixCollisionVelocity(contactNormal);
       Vector2 newVelocity = Vector2.Reflect(prevVelocity, contactNormal);
       prevVelocity = newVelocity;
       _rigidbody.velocity = (newVelocity*speed);
   }

   private void FixedUpdate()
   {
       _rigidbody.velocity = prevVelocity.normalized * speed;
   }

   private void Update()
   {
       if (gameObject.CompareTag("Enemy_4"))
       {
           CheckRayhHits(true);
       }
       else
       {
           CheckRayhHits(false);
       }
   }

   private void CheckRayhHits(bool isEnemy4)
   {
       RaycastHit2D hitUp;
       RaycastHit2D hitDown;
       RaycastHit2D hitRight;
       RaycastHit2D hitLeft;
       if (isEnemy4)
       {
           // Send raycasts in all four directions
           Vector2 raycastOrigin = transform.position;
           hitUp = Physics2D.Raycast(raycastOrigin, Vector2.up, _maxDistance, _seaSpaceLayer);
           hitDown = Physics2D.Raycast(raycastOrigin, Vector2.down, _maxDistance, _seaSpaceLayer);
           hitRight = Physics2D.Raycast(raycastOrigin, Vector2.right, _maxDistance, _seaSpaceLayer);
           hitLeft = Physics2D.Raycast(raycastOrigin, Vector2.left, _maxDistance, _seaSpaceLayer);
           CheckRayClollisions(hitUp, hitDown, hitRight, hitLeft);
           hitUp = Physics2D.Raycast(raycastOrigin, Vector2.up, _maxDistance, _backGroundLayer);
           hitDown = Physics2D.Raycast(raycastOrigin, Vector2.down, _maxDistance, _backGroundLayer);
           hitRight = Physics2D.Raycast(raycastOrigin, Vector2.right, _maxDistance, _backGroundLayer);
           hitLeft = Physics2D.Raycast(raycastOrigin, Vector2.left, _maxDistance, _backGroundLayer);
           CheckRayClollisions(hitUp, hitDown, hitRight, hitLeft);
       }
       else
       {
           // Send raycasts in all four directions
           Vector2 raycastOrigin = transform.position;
           raycastOrigin.x -= X_RAYCAST_DEV;
           raycastOrigin.y -= Y_RAYCAST_DEV;
           hitUp = Physics2D.Raycast(raycastOrigin, Vector2.up, _maxDistance, _landSpaceLayer);
           hitDown = Physics2D.Raycast(raycastOrigin, Vector2.down, _maxDistance, _landSpaceLayer);
           hitRight = Physics2D.Raycast(raycastOrigin, Vector2.right, _maxDistance, _landSpaceLayer);
           hitLeft = Physics2D.Raycast(raycastOrigin, Vector2.left, _maxDistance, _landSpaceLayer);
           CheckRayClollisions(hitUp, hitDown, hitRight, hitLeft);
       }
   }

   private void CollisionDetecetd(Vector2 contactNormal)
   {
       FixCollisionVelocity(contactNormal);
       Vector2 newVelocity = Vector2.Reflect(prevVelocity, contactNormal);
       prevVelocity = newVelocity;
       _rigidbody.velocity = (newVelocity*speed);
   }

   private void CheckRayClollisions(RaycastHit2D hitUp, RaycastHit2D hitDown,RaycastHit2D hitRight,RaycastHit2D hitLeft)
   {
       if (hitUp.collider != null)
       {
           CollisionDetecetd(hitUp.normal);
       }
       if (hitDown.collider != null)
       {
           CollisionDetecetd(hitDown.normal);
       }
       if (hitLeft.collider != null)
       {
           CollisionDetecetd(hitLeft.normal);
       }
       if (hitRight.collider != null)
       {
           CollisionDetecetd(hitRight.normal);
       }
   }
}



