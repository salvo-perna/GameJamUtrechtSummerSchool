using BTAI;
using Gamekit2D;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AngerBoss : MonoBehaviour
{

    [System.Serializable]
    public class AngerBossRound
    {
        public float rocksDelay = 2;
        public int bossHP = 10;

    }

    public Transform target;
    public Rock rock;
    public Transform rockSpawnPoint;
    public Vector2 rockLaunchVelocity;
    public Damageable damageable;

    [Space]
    public AngerBossRound[] rounds;

    public UnityEvent onDefeated;

    [Header("Audio")]
    public AudioClip bossDeathClip;
    public AudioClip postBossClip;
    public AudioClip bossMusic;
    [Space]
    public RandomAudioPlayer stepAudioPlayer;
    public RandomAudioPlayer rockAudioPlayer;
    public RandomAudioPlayer takingDamage;
    [Space]
    public AudioSource roundDeathSource;
    public AudioClip startRound2Clip;
    public AudioClip startRound3Clip;
    public AudioClip deathClip;

    [Header("UI")]
    public Slider healthSlider;

    private int m_TotalHealth = 0;
    private int m_CurrentHealth = 0;

    //used to track target movement, to correct for it.
    private Vector2 m_PreviousTargetPosition;

    Animator animator;
    Root ai;
    int round = 0;

    void OnEnable()
    {
        animator = GetComponent<Animator>();

        round = 0;

        ai = BT.Root();
        ai.OpenBranch(
            BT.Wait(3),
            //First Round
            BT.Repeat(rounds.Length).OpenBranch(
                BT.Call(NextRound),
                BT.While(IsAlive).OpenBranch(
                    BT.Wait(rounds[round].rocksDelay),
                    BT.Trigger(animator, "Attack"),
                    BT.Call(ThrowRock) 
                    )
            ),
            BT.Trigger(animator, "Die"),
            BT.SetActive(damageable.gameObject, false),
            BT.Wait(2),
            BT.Call(Cleanup),
            BT.Wait(6),
            BT.Call(Die),
            BT.Terminate()
            );

    //    BackgroundMusicPlayer.Instance.ChangeMusic(bossMusic);
    //    BackgroundMusicPlayer.Instance.Play();
    //    BackgroundMusicPlayer.Instance.Unmute(2.0f);

        //we aggregate the total health to set the slider to the proper value
        //(as the boss is actually "killed" every round and regenerated, we can't use directly its current health)
        for (int i = 0; i < rounds.Length; ++i)
        {
            m_TotalHealth += rounds[i].bossHP;
        }
        m_CurrentHealth = m_TotalHealth;

        healthSlider.maxValue = m_TotalHealth;
        healthSlider.value = m_TotalHealth;

        if (target != null)
            m_PreviousTargetPosition = target.transform.position;
    }

    void Cleanup()
    {
    
        healthSlider.GetComponent<Animator>().Play("BossHealthDefeat");

     //   BackgroundMusicPlayer.Instance.ChangeMusic(postBossClip);
      //  BackgroundMusicPlayer.Instance.PushClip(bossDeathClip);

        roundDeathSource.clip = deathClip;
        roundDeathSource.loop = false;
        roundDeathSource.Play();
        GetComponent<Damager>().enabled = false;
    }

    void Die()
    {
        onDefeated.Invoke();
    }

    void ThrowRock()
    {
        rockAudioPlayer.PlayRandomSound();

        var p = Instantiate(rock);
        p.transform.position = rockSpawnPoint.position;
        p.initialForce = rockLaunchVelocity;
    }

    void Update()
    {
        ai.Tick();
    }

    void NextRound()
    {
        damageable.SetHealth(rounds[round].bossHP);
     //   damageable.EnableInvulnerability(true);
        round++;

        if (round == 2)
        {
            roundDeathSource.clip = startRound2Clip;
            roundDeathSource.loop = true;
            roundDeathSource.Play();
        }
        else if (round == 3)
        {
            roundDeathSource.clip = startRound3Clip;
            roundDeathSource.loop = true;
            roundDeathSource.Play();
        }
    }

    void Disabled()
    {

    }

    void Enabled()
    {

    }

    public void Damaged(Damager damager, Damageable damageable)
    {
        takingDamage.PlayRandomSound();

        m_CurrentHealth -= damager.damage;
        healthSlider.value = m_CurrentHealth;
    }

    bool IsAlive()
    {
        var alive = damageable.CurrentHealth > 0;
        return alive;
    }

#if UNITY_EDITOR
    public BTAI.Root GetAIRoot()
    {
        return ai;
    }
#endif
}
