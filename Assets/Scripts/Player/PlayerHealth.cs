using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Damagable
{
    public int maxHealth;
    [SerializeField]
    private int currentHealth;

    public Image damageScreen;
    public Text healthText;

    PlayerStates states;
    PlayerCheckpoint checkpoint;
    PlayerTeleport teleport;
    Movement movement;

    Camera cam;
    CameraController camController;

    private void Start()
    {
        states = GetComponent<PlayerStates>();
        checkpoint = GetComponent<PlayerCheckpoint>();
        teleport = GetComponent<PlayerTeleport>();
        movement = GetComponent<Movement>();

        cam = Camera.main;
        camController = cam.GetComponent<CameraController>();

        currentHealth = maxHealth;
        healthText.text = "Health: " + currentHealth;
    }

    public override void TakeDamage(Damage.DamageType type, int amount)
    {
        if (type != Damage.DamageType.Healing && !states.isInvincible)
        {
            currentHealth -= amount;
            healthText.text = "Health: " + currentHealth;
            states.isInvincible = true;
            movement.velocity = Vector2.zero;
            Time.timeScale = 0f;
            StartCoroutine(ResetAfterDamage());
        }

        if (type == Damage.DamageType.Healing)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            healthText.text = "Health: " + currentHealth;
        }
    }

    private void ResetToLastCheckpoint()
    {
        states.isSliding = false;
        states.isGrabbed = false;
        teleport.teleportSpriteRenderer.enabled = false;
        transform.position = checkpoint.lastCheckPoint.position;
        states.direction = Mathf.Sign(Input.GetAxis("Horizontal"));
        transform.localScale = new Vector3(states.direction, 1, 1);
        camController.SnapToPlayer();
        states.isInvincible = false;
    }

    private IEnumerator ResetAfterDamage()
    {
        float duration = 0.3f;
        float smoothness = 20f;
        float delay = 0.5f;
        float timeStep = 0;
        while (timeStep < duration + delay)
        {
            timeStep += duration / smoothness;
            damageScreen.color = Color.Lerp(Color.clear, Color.black, timeStep / duration);
            yield return Utilities.WaitForUnscaledSeconds(duration / smoothness);
        }
        ResetToLastCheckpoint();
        Time.timeScale = 1f;
        StartCoroutine(BlackToClear());
    }


    private IEnumerator BlackToClear()
    {
        float duration = 0.3f;
        float smoothness = 20f;
        float timeStep = 0;
        while (timeStep < duration)
        {
            timeStep += duration / smoothness;
            damageScreen.color = Color.Lerp(Color.black, Color.clear, timeStep / duration);
            yield return Utilities.WaitForUnscaledSeconds(duration / smoothness);
        }
    }
}
