using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] PlayerData playerData;
    Animator animator;
    PlayerController controller;
    PlayerDamage plDamage;
    PlayerAniEvent aniEvent;
    private int attackCount = 0;
    private float comboResetTime = 1.0f; // ���� ������ �ʱ�ȭ�ϴ� �ð�
    private float stopAttackTime = 1.25f; // �ð��� ������ ����ī��Ʈ�� �ʱ�ȭ�� �ð�
    //���� �ð�
    private float lastAttackTime;       // ����ð��� ������ ������ �ð�
    private float stopLastAttackTime;   // ����ð��� ������ ������ ���� �ð�
    //��ų �ð�
    [SerializeField] private float level5SkillTime = 10.0f; // ��ų ��Ÿ��
    private float level5LastSkilTime;      // ���� ��ų�� ���� �������ð��� ������ �ð�
    SkillManager skillManager;
    private bool isStop;
    private bool isEquip;
    private bool isInven;
    bool isSword;
    bool isShield;
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        aniEvent = GetComponentInChildren<PlayerAniEvent>();
        controller = GetComponent<PlayerController>();
        plDamage = GetComponent<PlayerDamage>();
        skillManager = SkillManager.skillInst;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isInven && !isEquip)
        {
            if(isSword)
                SwordAttack();
            if(isShield)
                ShieldBlock();
            if(isSword || isShield)
                Skill();
        }
    }

    private void SwordAttack()
    {
        if (Input.GetMouseButton(0) && Time.time - lastAttackTime > comboResetTime)
        {
            attackCount++;
            controller.IsStop(true);
            animator.SetBool("IsAttack", true);
            TriggerAttackAnimation();
            lastAttackTime = Time.time;
            stopLastAttackTime = Time.time;
        }
        else if (Time.time - stopLastAttackTime > stopAttackTime && isStop)
        {
            attackCount = 0;
            controller.IsStop(false);
            animator.SetBool("IsAttack", false);
            stopLastAttackTime = Time.time;
            isStop = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isStop = true;
        }
    }

    private void ShieldBlock()
    {
        if (Input.GetMouseButton(1))
        {
            if (!plDamage.IsHit)
            {
                animator.SetBool("IsShield", true);
                plDamage.IsShieldOnOff(true);
                controller.IsStop(true);
            }
            else
            {
                animator.SetBool("IsShield", false);
                controller.IsStop(true);
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("IsShield", false);
            plDamage.IsShieldOnOff(false);
            controller.IsStop(false);
        }
    }
    private void Skill()
    {
        if(Input.GetKeyDown(KeyCode.Q) && Level05(0))
        {
            Level05SkillCasting();
        }
        else if(Input.GetKeyDown(KeyCode.E) && Level05(1))
        {
            Level05SkillCasting();
        }
        else if (Input.GetKeyDown(KeyCode.R) && Level05(1))
        {
            Level05SkillCasting();
        }

    }

    private void Level05SkillCasting()
    {
        for (int i = 1; i < 4; i++)
        {
            if (playerData.level05SkillIdx == i && Time.time - level5LastSkilTime > level5SkillTime)
            {
                controller.IsStop(true);
                aniEvent.Level05SkillChange(i);
                animator.SetTrigger("CastingTrigger");
                Invoke("IsMove", 1.0f);
                level5LastSkilTime = Time.time;
                break;
            }
        }
    }

    bool Level05(int skillListIdx)
    {
        Debug.Log(skillManager.skillList[skillListIdx].sprite.name);
        Debug.Log(skillManager.skillImageList[playerData.level05SkillIdx - 1].name);
        return skillManager.skillList[skillListIdx].sprite == skillManager.skillImageList[playerData.level05SkillIdx - 1];
    }
    void TriggerAttackAnimation()
    {
        switch (attackCount)
        {
            case 1:
                animator.SetTrigger("AttackDown");
                break;
            case 2:
                animator.SetTrigger("AttackUp");
                break;
            case 3:
                animator.SetTrigger("AttackRotate");
                attackCount = 0;
                break;
        }
    }
    public void IsInventory(bool _isInven)
    {
        isInven = _isInven;
    }
    public void IsEquipment(bool _isEquip)
    {
        isEquip = _isEquip;
    }
    void IsMove()
    {
        controller.IsStop(false);
    }
    public void IsSword(bool _IsSword)
    {
        animator.SetBool("GetSword", _IsSword);
        isSword = _IsSword;
    }
    public void IsShield(bool _IsShield)
    {
        animator.SetBool("GetShield", _IsShield);
        isShield = _IsShield;
    }   
}