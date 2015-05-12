/*******************************************************************************
 * Copyright 2012-2014 One Platform Foundation
 *
 *       Licensed under the Apache License, Version 2.0 (the "License");
 *       you may not use this file except in compliance with the License.
 *       You may obtain a copy of the License at
 *
 *           http://www.apache.org/licenses/LICENSE-2.0
 *
 *       Unless required by applicable law or agreed to in writing, software
 *       distributed under the License is distributed on an "AS IS" BASIS,
 *       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *       See the License for the specific language governing permissions and
 *       limitations under the License.
 ******************************************************************************/

using UnityEngine;
using OnePF;
using System.Collections.Generic;

/**
 * Example of OpenIAB usage
 */ 
public class OpenIABTest : MonoBehaviour
{
	class PaidItem
	{
		public int		m_gem;

		public PaidItem(int gem)
		{
			m_gem = gem;
		}
	}

	YGUISystem.GUIButton[]	m_piadItemButtons = new YGUISystem.GUIButton[3];
	YGUISystem.GUIButton	m_closeButton;
	Dictionary<string, PaidItem>	m_paidItems = new Dictionary<string, PaidItem>();

    string _label = "";
    bool _isInitialized = false;
	bool m_progressing = false;
    Inventory _inventory = null;
    

    private void Start()
    {
		// Listen to all events for illustration purposes
		OpenIABEventManager.billingSupportedEvent += billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent += purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;

        Init();
    }

	void Init()
	{
		m_paidItems.Add("gem.1000", new PaidItem(1000));
		m_paidItems.Add("gem.3000", new PaidItem(3500));
		m_paidItems.Add("gem.5000", new PaidItem(6000));

		m_closeButton = new YGUISystem.GUIButton(transform.Find("CloseButton").gameObject, ()=>{return true;});

		m_piadItemButtons[0] = new YGUISystem.GUIButton(transform.Find("PaidItemButton0").gameObject, ()=>{
			return _isInitialized && m_progressing == false;
		});

		m_piadItemButtons[1] = new YGUISystem.GUIButton(transform.Find("PaidItemButton1").gameObject, ()=>{
			return _isInitialized && m_progressing == false;
		});

		m_piadItemButtons[2] = new YGUISystem.GUIButton(transform.Find("PaidItemButton2").gameObject, ()=>{
			return _isInitialized && m_progressing == false;
		});

		foreach(KeyValuePair<string, PaidItem> pair in m_paidItems)
		{
			OpenIAB.mapSku(pair.Key, OpenIAB_Android.STORE_GOOGLE, pair.Key);
		}

		var googlePublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEApi71KrlP+P+IWL1HtgDigZ5VUd9KdvSasR/Q2ONnVSmtsGrE0abo11IXodJHeDQWfGn2KCHC1qrjUW0lX8dK/2syDFmjnvF4jHXjyAl7NZqQZzlu68XI/nBF9csCJ7eRtPG5VOdmY4LDe3skx3Re0mjDi1wnHmc5gtz8Tisa6krDNq3V0lqW9rLD1aAA/TWtXcfFQOdZVrdrFsBzizJIbz9vqBAYmh8PedBcufYH/ToRUaokdNKgjh9l+2L2zNbYi1MIQC1rUv52MKzCgJv3BUEF6pfd+2iBXSfMcDzElz8VqXRUtRcdexcTYBX29cpiNTfXznW4y+StTd2TLvbiowIDAQAB";

		var options = new Options();
		options.checkInventoryTimeoutMs = Options.INVENTORY_CHECK_TIMEOUT_MS * 2;
		options.discoveryTimeoutMs = Options.DISCOVER_TIMEOUT_MS * 2;
		options.checkInventory = false;
		options.verifyMode = OptionsVerifyMode.VERIFY_ONLY_KNOWN;
		options.prefferedStoreNames = new string[] { OpenIAB_Android.STORE_GOOGLE };
		options.availableStoreNames = new string[] { OpenIAB_Android.STORE_GOOGLE };
		options.storeKeys = new Dictionary<string, string> { {OpenIAB_Android.STORE_GOOGLE, googlePublicKey} };
		options.storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT;
		
		// Transmit options and start the service
		OpenIAB.init(options);
	}

	public void OnClickClose()
	{
		gameObject.SetActive(false);
	}

	public void OnClickPurchase(string sku)
	{
		if (_isInitialized == false)
			return;

		m_progressing = true;
		OpenIAB.purchaseProduct(sku, "ok marine");
	}


    private void billingSupportedEvent()
    {
        _isInitialized = true;
		OpenIAB.queryInventory();
		m_closeButton.Text.Lable = "billingSupportedEvent";


    }
    
	private void billingNotSupportedEvent(string error)
    {
		m_closeButton.Text.Lable = "billingNotSupportedEvent: " + error;
		gameObject.SetActive(false);
    }

    private void queryInventorySucceededEvent(Inventory inventory)
    {
		m_closeButton.Text.Lable = "queryInventorySucceededEvent: " + inventory;
        if (inventory != null)
        {
            _label = inventory.ToString();
            _inventory = inventory;

			foreach(Purchase purchase in _inventory.GetAllPurchases())
			{
				OpenIAB.consumeProduct(purchase);
			}
        }
    }
    
	private void queryInventoryFailedEvent(string error)
    {
		m_closeButton.Text.Lable = "queryInventoryFailedEvent: " + error;
        _label = error;
    }
    
	private void purchaseSucceededEvent(Purchase purchase)
    {
		m_closeButton.Text.Lable = "purchaseSucceededEvent: " + purchase;
        _label = "PURCHASED:" + purchase.ToString();

		OpenIAB.consumeProduct(purchase);
    }
    private void purchaseFailedEvent(int errorCode, string errorMessage)
    {
		m_closeButton.Text.Lable = "purchaseFailedEvent: " + errorMessage;
        _label = "Purchase Failed: " + errorMessage;
		
		m_progressing = false;
    }
    private void consumePurchaseSucceededEvent(Purchase purchase)
    {
		m_closeButton.Text.Lable = "consumePurchaseSucceededEvent: " + purchase.Sku;
        _label = "CONSUMED: " + purchase.ToString();

		PaidItem paidItem = null;
		if (true == m_paidItems.TryGetValue(purchase.Sku, out paidItem))
		{
			Warehouse.Instance.Gem.Item.Count += paidItem.m_gem;
		}

		m_progressing = false;
    }
    private void consumePurchaseFailedEvent(string error)
    {
		m_closeButton.Text.Lable = "consumePurchaseFailedEvent: " + error;
        _label = "Consume Failed: " + error;
		m_progressing = false;
    }
}