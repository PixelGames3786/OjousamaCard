using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SpeechBubbleManager : MonoBehaviour
{
	[SerializeField] RectTransform LeftSpeechBubble, RightSpeechBubble;
	[SerializeField] TextMeshProUGUI LeftMess, RightMess;
	[SerializeField] ScenarioBattle TextDatas = null;

	private RectTransform TargetBubble=null;
	private TextMeshProUGUI Target=null;
	private string TargetText,AddText;

	private int CurrentSentenceID = 0, NowCharaNum=0,NowLineNum=1;

	public float NeedAddTime;
	private float AddTime;

	private bool Showing, CanNext;

	public void MessageStart(int id)
	{
		CurrentSentenceID = id;
		BattleSentence result = SearchSentence(CurrentSentenceID);

		ShowingMassage(result);
	}

	void Update()
	{
        if (Showing)
        {
			AddTime += Time.deltaTime;

            if (AddTime>=NeedAddTime)
            {
				AddTime = 0;

				AddText += TargetText[NowCharaNum];

				Target.text = AddText;

				NowCharaNum++;

                if (NowCharaNum>NowLineNum*12)
                {
					NowLineNum++;

					TargetBubble.sizeDelta = new Vector2(500,50+(NowLineNum*50));
                }

				//もし全部表示し終わったなら
                if (NowCharaNum>=TargetText.Length)
                {
					AddText = "";

					NowCharaNum = 0;
					NowLineNum = 1;

					Showing = false;
					CanNext = true;
                }
            }
        }
	}


	//文章の続きを表示します。
	public void ReadmoreMessage()
	{
		if (!CanNext) return;

		//文章番号をひとつだけ次に進めます。
		CurrentSentenceID++;

		//文章番号をもとに文を検索します。
		BattleSentence result = SearchSentence(CurrentSentenceID);

		//なんらかの特殊なイベントがないか確認
		IventCheck(result);

		//得られたメッセージを表示します。
		ShowingMassage(result);
	}


	//メッセージを表示させるだけです。
	void ShowingMassage(BattleSentence sentence)
	{
		LeftSpeechBubble.gameObject.SetActive(false);
		RightSpeechBubble.gameObject.SetActive(false);
		LeftMess.gameObject.SetActive(false);
		RightMess.gameObject.SetActive(false);

		switch (sentence.speakerSide)
		{
			case "Left":
				{
					TargetBubble = LeftSpeechBubble;
					TargetText = sentence.message;
					Target = LeftMess;
				}
				break;

			case "Right":
				{
					TargetBubble = RightSpeechBubble;
					TargetText = sentence.message;
					Target = RightMess;
				}
				break;
		}

		Target.text = "";

		TargetBubble.gameObject.SetActive(true);
		Target.gameObject.SetActive(true);

		Showing = true;
		CanNext = false;
	}

	BattleSentence SearchSentence(int Id)
	{
		//シナリオのメッセージ配列の中から文章をIDで検索して取得します。
		BattleSentence result = TextDatas.Sentences.First(
			(BattleSentence line) => { return line.id == Id; }
		);
		return result;
	}

	/*

	//選択肢のメッセージを表示させるだけです。
	//選択肢のUIはボタンとテキストで構成されています。
	void ShowingMassageIsBranch(Sentence sentence)
	{
		if (sentence.branch)
		{
			SwichReadmoreInteractable();
			SwichCoicesActivate();
			yBranchMassage.text = sentence.yesMessage;
			nBranchMassage.text = sentence.noMessage;
		}
	}

	//肯定を選択したとき、ボタンコンポーネントのEventから呼び出されます。
	public void ChooseYes()
	{
		//選択肢を押下したときReadmoreボタンを有効にします。
		SwichReadmoreInteractable();
		//選択肢を非表示にします。
		SwichCoicesActivate();

		Sentence result = SearchSentence(CurrentSentenceID);
		Skip(result.choseYes);
	}

	//否定を選択したとき、ボタンコンポーネントのEventから呼び出されます。
	public void ChooseNo()
	{
		//選択肢を押下したときReadmoreボタンを有効にします。
		SwichReadmoreInteractable();
		//選択肢を非表示にします。
		SwichCoicesActivate();

		Sentence result = SearchSentence(CurrentSentenceID);
		Skip(result.choseNo);
	}

	//文章を読み飛ばします。
	//選択の結果、シナリオが分岐したのち収束するときなどに使用します。
	void Connect(Sentence sentence)
	{
		if (sentence.doConnect)
		{
			Skip(sentence.skipId);
		}
	}

	//別の文章に読み飛ばします。
	void Skip(int id)
	{
		Sentence result = SearchSentence(id);
		CurrentSentenceID = result.id;
		ShowingMassage(result);
	}

	*/

	//最後の文章になったらUIを非表示にします。
	//会話が終わったのにもかかわらず、UIが表示されていたらバグです。
	//だから非表示する必要があります。
	void IventCheck(BattleSentence sentence)
	{
		if (sentence.ivent=="BattleStart")
		{
			//このゼロは現在読んでいる文章をリセットするための数値です。
			CurrentSentenceID = 0;
			Close();

			BattleManager.BM.BattleStart();
		}
	}

	void Close()
    {
		LeftSpeechBubble.gameObject.SetActive(false);
		RightSpeechBubble.gameObject.SetActive(false);
		LeftMess.gameObject.SetActive(false);
		RightMess.gameObject.SetActive(false);

		gameObject.SetActive(false);
	}

	/*
	//Readmoreボタンを押下の可不可を切り替えます。
	//選択肢があるとき、選択肢以外のボタンを押せなくするために使用します。
	void SwichReadmoreInteractable()
	{
		if (readMore.interactable)
		{
			readMore.interactable = false;
		}
		else
		{
			readMore.interactable = true;
		}
	}

	//アクティブ状態を切り替えます。
	void SwichActiveState(GameObject target)
	{
		target.SetActive(!target.activeSelf);

	}

	//選択肢のアクティブ状態を切り替えます。
	void SwichCoicesActivate()
	{
		if (choices.activeSelf)
		{
			choices.SetActive(false);
		}
		else
		{
			choices.SetActive(true);
		}
	}

	//対話UIのアクティブ状態を切り替えます。
	void SwichDialougeActivate()
	{
		if (dialogue.activeSelf)
		{
			dialogue.SetActive(false);
		}
		else
		{
			dialogue.SetActive(true);
		}
	}


	//シーンチェンジします。
	void SceneChange(Sentence sentence)
	{
		if (sentence.sceneChange != "")
		{
			if (sentence.nextBattle != "")
			{
				SaveLoadManager.instance.NextBattle = sentence.nextBattle;
			}

			SceneManager.LoadScene(sentence.sceneChange);
		}
	}
	*/

}
