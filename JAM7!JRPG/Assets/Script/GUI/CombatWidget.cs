﻿using UnityEngine;
using UnityEngine.UI;

public class CombatWidget : GUIWidget
{
    [Header("References")]
    [SerializeField] private Text magazineText;
    [SerializeField] private Transform health;

    private PlayerCombatController player;

    public override void Initialize(params object[] args)
    {
        player = (PlayerCombatController)args[0];
    }

    public override void Show()
    {
        base.Show();

        UpdateMagazine(player.Magazine);

        player.OnMagazineUpdate.AddListener(UpdateMagazine);
        player.OnHpChange.AddListener(UpdateHealth);
    }

    public override void Hide()
    {
        base.Hide();

        player.OnMagazineUpdate.RemoveListener(UpdateMagazine);
        player.OnHpChange.RemoveListener(UpdateHealth);
    }

    private void UpdateMagazine(int n)
    {
        magazineText.text = n.ToString();
    }

    private void UpdateHealth(int hp, int maxHp)
    {
        float p = (hp * 5) / (float)maxHp;

        for (int i = 0; i < 5; ++i)
        {
            float d = p - i;
            if (d > 0.5)
                health.GetChild(i).GetComponent<SpriteRenderer>().sprite = GUIManager.Singleton.fullHeart;
            else if (d > 0)
                health.GetChild(i).GetComponent<SpriteRenderer>().sprite = GUIManager.Singleton.halfHeart;
            else
                health.GetChild(i).GetComponent<SpriteRenderer>().sprite = GUIManager.Singleton.emptyHeart;
        }
    }

    private void OnDestroy()
    {
        player.OnMagazineUpdate.RemoveListener(UpdateMagazine);
        player.OnHpChange.RemoveListener(UpdateHealth);
    }
}
