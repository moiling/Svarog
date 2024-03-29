﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DamageHandler : MonoBehaviour {
    public DamageType DamageType;

    public GameObject AttackSmallEffect;
    public GameObject AttackKnifeEffect;
    public GameObject AttackCenterDownEffect;
    
    private PlayerStateManager _states;
    private PlayerAnimationHandler _animation;
    private MovementHandler _movement;
    
    private List<Collider2D> _hurtColliders = new List<Collider2D>();
    private Collider2D _defenseCollider;

    private float _onTriggerTimer;
    private float _isDefensed;
    
    private bool _doDamage = true;

    public void Start() {
        _states = GetComponentInParent<PlayerStateManager>();
        _animation = GetComponentInParent<PlayerAnimationHandler>();
        _movement = GetComponentInParent<MovementHandler>();
    }

    private void Update() {

        if (_onTriggerTimer <= 0) { // 一次碰撞完成，list中包含这次碰撞的所有碰撞体

            if (_defenseCollider != null && _hurtColliders.Count > 0) {
                StartDamage(_defenseCollider);
            } else if (_hurtColliders.Count == 1) {
                StartDamage(_hurtColliders[0]);
            } else if (_hurtColliders.Count > 1) {
                var head = _hurtColliders.Find(c => c.name == "Head");
                var body = _hurtColliders.Find(c => c.name == "Body");
                var foot = _hurtColliders.Find(c => c.name == "Foot");
                
                // 规则：头>脚>身
                if (head) {
                    StartDamage(head);
                } else if (foot) {
                    StartDamage(foot);
                } else if (body) {
                    StartDamage(body);
                }
            }

            // 清空
            _defenseCollider = null;
            _hurtColliders.Clear();
        } else {
            _onTriggerTimer -= Time.deltaTime;
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        
        //Debug.Log(other.name);

        
        if (!_doDamage) {
            return;
        }
        
        if (other.GetComponentInParent<PlayerStateManager>() && other.CompareTag("HurtCollider")) {
            var otherState = other.GetComponentInParent<PlayerStateManager>();
            if (otherState != _states) {                  
                _hurtColliders.Add(other);
            }        
        }

        if (other.GetComponentInParent<PlayerStateManager>() && other.CompareTag("DefenseCollider")) {
            
            var otherState = other.GetComponentInParent<PlayerStateManager>();

            if (otherState != _states) {

                if (!otherState.Attacks.Any(attack => attack.Attack)) { // 对方没有攻击才可以防御成功
                    if (_onTriggerTimer < 0) {                          // 一段时间能的首次碰撞
                        _onTriggerTimer = 0.01f;
                    }

                    if (_onTriggerTimer < 0) { // 一段时间能的首次碰撞
                        _onTriggerTimer = 0.01f;
                    }

                    _defenseCollider = other;
                }
            }
        }
    }

    private void StartDamage(Collider2D other) {
        Debug.Log("Attack-Start:" + other.name);
        
        var otherState = other.GetComponentInParent<PlayerStateManager>();
        
        if (other.CompareTag("DefenseCollider")) {
            otherState.TakeDamage(0.01f, DamageType.Defensed);
            BeatBack(other.GetComponentInParent<BorderHandler>().CloseToWall, 1, other);
        } else if (_animation.Animator.GetBool(AnimatorBool.IS_FIRE_PUNCH)) {
            otherState.TakeDamage(6, DamageType.FirePunch);
        } else if (_animation.Animator.GetBool(AnimatorBool.IS_HEAVY_ATTACK)) {
            otherState.TakeDamage(5, DamageType.Heavy);
            BeatBack(other.GetComponentInParent<BorderHandler>().CloseToWall, 2, other);
        } else {
            otherState.TakeDamage(3, DamageType);
            BeatBack(other.GetComponentInParent<BorderHandler>().CloseToWall, 1, other);
        }
        
        // 攻击特效
        if (_animation.Animator.GetBool(AnimatorBool.ATTACK_SMALL_EFFECT)) {
            Instantiate(
                AttackSmallEffect,
                transform.position + 
                new Vector3(GetComponent<BoxCollider2D>().offset.x * (_states.LookRight ? 1 : -1), GetComponent<BoxCollider2D>().offset.y, 0),
                Quaternion.Euler(new Vector3(0, _states.LookRight ? 180 : 0, 0)));
        } else if (_animation.Animator.GetBool(AnimatorBool.ATTACK_KNIFE_EFFECT)) {
            Instantiate(
                AttackKnifeEffect,
                transform.position + 
                new Vector3(GetComponent<BoxCollider2D>().offset.x * (_states.LookRight ? 1 : -1), GetComponent<BoxCollider2D>().offset.y, 0),
                Quaternion.Euler(new Vector3(0, _states.LookRight ? 180 : 0, 0)));
        } else if (_animation.Animator.GetBool(AnimatorBool.ATTACK_CENTER_DOWM_EFFECT)) {
            Instantiate(
                AttackCenterDownEffect,
                transform.position + 
                new Vector3(GetComponent<BoxCollider2D>().offset.x * (_states.LookRight ? 1 : -1), GetComponent<BoxCollider2D>().offset.y, 0),
                Quaternion.Euler(new Vector3(0, _states.LookRight ? 0 : 180, 0)));
        }

        
            
        // 顿帧
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        _doDamage = false;

        // _animation.Stop(0.2f);
        _states.StateStop(0.2f);
        Invoke("ResetDoDamage", 0.2f);
        
        // 告诉状态机已经攻击到敌人，可以取消后摇
        _animation.Animator.SetBool(AnimatorBool.ATTACKED, true);
        
        _states.HitText.GetHit();
    }

    private void BeatBack(bool hitWall, float force, Collider2D other) {
        if (hitWall) {
            _movement.BeBeatBack(force);
        } else {
            other.GetComponentInParent<MovementHandler>().BeBeatBack(force);
        }
    }

    private void ResetDoDamage() {

        _doDamage = true;
    }
}