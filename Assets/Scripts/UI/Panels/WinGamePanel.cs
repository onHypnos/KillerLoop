using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinGamePanel : BaseMenuPanel,
    IServiceConsumer<IBeatenEnemyCounter>, IServiceConsumer<ICollectedMoneyCounter>,
    IServiceConsumer<IMultiplierCounter>
{
    #region PrivateFields

    #region Serialized

    [Header("Panel")]
    [SerializeField] private GameObject _panel;

    [Header("Elements")]
    [SerializeField] private Button _collectX2Button;
    [SerializeField] private Button _collectButton;
    [SerializeField] private List<Animation> _animations;
    [SerializeField] private TextMeshProUGUI _enemyBeatenUI;
    [SerializeField] private TextMeshProUGUI _multiplierUI;
    [SerializeField] private TextMeshProUGUI _moneyCollectedUI;

    #endregion

    private bool _waitingToCloseMenu;
    private int _currentMoney;
    private int _moneyCollected;
    private int _enemyBeaten;
    private int _multiplier;
    private int _incrementValue;
    private const float _incrementMultiplier = 0.005f;

    #endregion

    #region PublicMethods

    public override void Initialize()
    {
        ResetValues();
        SetButtonEvents();
    }

    public override void Hide()
    {
        if (!IsShow) return;
        _panel.gameObject.SetActive(false);
        IsShow = false;
        StopAnimations();
        ResetValues();
        StopAllCoroutines();
    }

    public override void Show()
    {
        if (IsShow) return;
        _panel.gameObject.SetActive(true);
        IsShow = true;
        StartAnimations();
        UpdateAllValues();
        EnableButtons();
    }

    #region IServiceConsumer

    public void UseService(IMultiplierCounter service)
    {
        if (service != null)
        {
            SetMultiplier(service.Multiplier);
        }
    }

    public void UseService(IBeatenEnemyCounter service)
    {
        if (service != null)
        {
            SetBeatenEnemyValue(service.EnemyBeaten);
        }
    }

    public void UseService(ICollectedMoneyCounter service)
    {
        if (service != null)
        {
            SetCollectedMoneyValue(service.MoneyCollected);
        }
    }

    #endregion

    #endregion

    #region PrivateMethods

    private void SetMultiplier(float value)
    {
        _multiplier = (int)value;
    }

    private void SetBeatenEnemyValue(int value)
    {
        _enemyBeaten = value;
    }

    private void SetCollectedMoneyValue(int value)
    {
        _moneyCollected = value;
    }

    private void ResetValues()
    {
        _waitingToCloseMenu = false;
        _currentMoney = 0;
    }

    private void SetButtonEvents()
    {
        BindListenerToButton(_collectButton, UIEvents.Current.CollectButton);
        BindListenerToButton(_collectX2Button, UIEvents.Current.CollectX2Button);
        BindListenerToButton(_collectButton, DisableButtons);
        BindListenerToButton(_collectX2Button, DisableButtons);
    }

    private void EnableButtons()
    {
        if (_collectButton)
        {
            _collectButton.enabled = true;
        }
        if (_collectX2Button)
        {
            _collectX2Button.enabled = true;
        }
    }

    private void DisableButtons()
    {
        if (_collectButton)
        {
            _collectButton.enabled = false;
        }
        if (_collectX2Button)
        {
            _collectX2Button.enabled = false;
        }
    }

    private void MoneyIncrement(int value)
    {
        if (_currentMoney + value < _moneyCollected)
        {
            _currentMoney += value;
        }
        else
        {
            _currentMoney = _moneyCollected;
        }
        UpdateMoneyCounterValue(_currentMoney);
    }

    #region UIUpdates

    private void UpdateAllValues()
    {
        UpdateMoneyCounter();
        UpdateMultiplierCounter(_multiplier);
        UpdateEnemyBeatenValue(_enemyBeaten);
    }

    private void UpdateMoneyCounter()
    {
        if (_moneyCollected > _currentMoney)
        {
            StartCoroutine(CountingMoney());
        }
    }

    private void UpdateMultiplierCounter(int value)
    {
        if (_multiplierUI)
        {
            _multiplierUI.text = $"x{value}";
        }
    }

    private void UpdateEnemyBeatenValue(int value)
    {
        if (_enemyBeatenUI)
        {
            _enemyBeatenUI.text = (value).ToString();
        }
    }

    private void UpdateMoneyCounterValue(int value)
    {
        if (_moneyCollectedUI)
        {
            _moneyCollectedUI.text = $"+{value}";
        }
    }

    #endregion

    private void StartAnimations()
    {
        if (_animations != null)
        {
            for (int i = 0; i < _animations.Count; i++)
            {
                _animations[i]?.Play();
            }
        }
    }

    private void StopAnimations()
    {
        if (_animations != null)
        {
            for (int i = 0; i < _animations.Count; i++)
            {
                _animations[i]?.Stop();
            }
        }
    }

    private void OnDestroy()
    {
        RemoveListenersFromButton(_collectButton);
        RemoveListenersFromButton(_collectX2Button);
    }

    #region Coroutine

    private IEnumerator CountingMoney()
    {
        _incrementValue = (int)((_moneyCollected - _currentMoney) * _incrementMultiplier);
        while (_currentMoney < _moneyCollected)
        {
            MoneyIncrement(_incrementValue);
            yield return null;
        }
        if (_waitingToCloseMenu)
        {
            UIEvents.Current.ToMainMenu();
        }
    }

    #endregion

    #endregion
}
