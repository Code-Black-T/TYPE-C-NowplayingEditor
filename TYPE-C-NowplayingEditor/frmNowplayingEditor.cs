using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

// MainWindow.TextBoxTweetText.Text とツイート設定をやり取り

namespace TYPE_C_NowplayingEditor
{

    public partial class frmNowplayingEditor : Form
    {

        public frmNowplayingEditor()
        {
            InitializeComponent();
        }

        public const uint WM_GETTEXT = 0x000D;
        public const uint WM_SETTEXT = 0x000C;

        public string tweettextFromEditorToMain;
        public string tweettextFromMainToEditor;

        bool ListChangeFlg = false;

        ToolTip ToolTip1;

        ■int LastSelectionStart;
        ■int LastSelectionLength;

        string TextBoxKeepStr; //ＯＫボタンを押す前のデータを退避

        //ボタンコントロール配列のフィールドを作成
        private System.Windows.Forms.Button[] replaceButtons;

        //フォームのLoadイベントハンドラ
        private void frmNowplayingEditor_Load(object sender, EventArgs e)
        {

            ContextMenu_RCLK_Func(■TextBoxTweetText);  //引数に渡したテキストボックス内での右クリックメニューを定義
            //////////////ContextMenu_RCLK_Func(this.EditBOX);  //引数に渡したテキストボックスの右クリックメニューをセット

            // http://dobon.net/vb/dotnet/control/buttonarray.html
            // http://social.msdn.microsoft.com/Forums/ja-JP/csharpgeneralja/thread/29b6239c-c672-4592-9b03-3784ad366b8c/

            ////ボタンコントロール配列の作成

            this.replaceButtons = new System.Windows.Forms.Button[]
                {this.rButton1,  this.rButton2,  this.rButton3,  this.rButton4,  this.rButton5 ,
                 this.rButton6,  this.rButton7,  this.rButton8,  this.rButton9,  this.rButton10 ,
                 this.rButton11, this.rButton12, this.rButton13, this.rButton14, this.rButton15 ,
                 this.rButton16, this.rButton17, this.rButton18, this.rButton19, this.rButton20 ,
                 this.rButton21, this.rButton22, this.rButton23, this.rButton24, this.rButton25 ,
                 this.rButton26, this.rButton27, this.rButton28, this.rButton29, this.rButton30 ,
                 this.rButton31, this.rButton32, this.rButton33, this.rButton34, this.rButton35
                };

            //イベントハンドラに関連付け（必要な時のみ）
            for (int i = 0; i < this.replaceButtons.Length; i++)
                this.replaceButtons[i].Click +=
                    new EventHandler(this.replaceButtons_Click);

            //http://dobon.net/vb/dotnet/control/showtooltip.html

            //ToolTipを作成する
            //ToolTip1 = new ToolTip(this.components);
            //フォームにcomponentsがない場合
            ToolTip1 = new ToolTip();

            //ToolTipの設定を行う
            //ToolTipが表示されるまでの時間
            ToolTip1.InitialDelay = 700;
            //ToolTipが表示されている時に、別のToolTipを表示するまでの時間
            ToolTip1.ReshowDelay = 1000;
            //ToolTipを表示する時間
            ToolTip1.AutoPopDelay = 10000;
            //フォームがアクティブでない時でもToolTipを表示する
            ToolTip1.ShowAlways = true;

            //Button1～Button34にToolTipが表示されるようにする
            for(int i = 0; i < this.replaceButtons.Length; i++)
            {
                string tempStr;

                tempStr = this.replaceButtons[i].Text;

                if ( tempStr.Contains(" ") == true ){
                    tempStr = tempStr.Substring(0, tempStr.IndexOf(" ", 0));
                }else{
                    //上のtempStr をそのまま使う  //半角スペースが含まれない場合は文字列全体
                }
                ToolTip1.SetToolTip(this.replaceButtons[i], tempStr + " を挿入します");
            }

            readEditData();

            //tweettextFromMainToEditor = 「メインウィンドウ」の TextBox.text
            //tweettextFromMainToEditor = MainWindow.TextBoxTweetText.Text;

            tweettextFromMainToEditor = GetOrSetTextForNowplayingTunes("GET");

            this.EditBOX.Text = tweettextFromMainToEditor; //「メインウィンドウ」側から、「ツイートする文字の設定」を読み込む
            this.EditBOX.Text = this.EditBOX.Text.Replace("$NEWLINE", "\r\n");

            if ( this.ComboBoxEditStr.Items.Count == 0 ){
                this.ComboBoxEditStr.Text = "NowPlaying $TITLE - $ARTIST(Album:$ALBUMNAME) #nowplaying #なうぷれ";
            }else{
                if ( tweettextFromMainToEditor == "" ){
                    this.ComboBoxEditStr.SelectedIndex = 0;
                    this.EditBOX.Text = this.ComboBoxEditStr.Text.Replace("$NEWLINE", "\r\n");
                }
            }

            LastSelectionStart = this.EditBOX.Text.Length;
            ListChangeFlg = false;
            TextBoxKeepStr = this.EditBOX.Text;

            LastSelectionLength = 0;

            this.EditBOX.Focus();
            this.EditBOX.Select( LastSelectionStart, LastSelectionLength ); //現在入力中の位置にカーソルを移動
            this.EditBOX.ScrollToCaret(); //現在入力中の位置にスクロール

        }

        //Buttonのクリックイベントハンドラ
        private void replaceButtons_Click(object sender, EventArgs e)
        {
            //クリックされたボタンを表示する
            //MessageBox.Show(((System.Windows.Forms.Button)sender).Text);

            //  http://dobon.net/vb/dotnet/system/modifierkeys.html

            if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {

                //【隠し機能】【デバッグモード】Ctrlキーを押しながら、ボタンをクリックする
                string tempStr;
                tempStr = ((System.Windows.Forms.Button)sender).Text;

                string LeftStr = "";
                string RightStr = "";
                int myIDX = 0;

                //bool LeftSpaceFLG = false;
                //bool RightSpaceFLG = false;

                if ( tempStr.Contains(" ") == true ) {
                    myIDX = tempStr.IndexOf(" ", 0);

                    LeftStr = tempStr.Substring(0, myIDX);
                    RightStr = tempStr.Substring(myIDX + 1, tempStr.Length - myIDX - 1);

                    tempStr = RightStr + "：" + LeftStr;
                    //左右を入れ替えて、区切り文字を：に変更した後、挿入する

                    tempStr = AddSpaceToLeftAndRight(tempStr);
                }else if ( tempStr.Contains("#") == true && tempStr.Length > 1 ) {
                    tempStr = AddSpaceToLeftAndRight(tempStr);
                }else if ( tempStr == "#" ) {
                    tempStr = "#";
                }else if ( tempStr == "space" ) {
                    tempStr = " ";
                }

                InsertStrIntoTextBox_EditBOX(tempStr);
            }else{  //通常の置き換え文字挿入

                string tempStr;
                tempStr = ((System.Windows.Forms.Button)sender).Text;

                if ( tempStr.Contains(" ") == true ) {
                    tempStr = tempStr.Substring(0, tempStr.IndexOf(" ", 0));
                    if ( tempStr == "$NEWLINE" ) {
                        tempStr = "\r\n";
                    }
                }else{
                    if ( tempStr == "space" ) {
                        tempStr = " ";
                    }else{
                        //上のtempStr をそのまま使う（半角スペースが含まれない場合は文字列全体）
                    }
                }

                InsertStrIntoTextBox_EditBOX(tempStr);
            }
        }

        private string AddSpaceToLeftAndRight(String TargetStr)  //左右に半角スペースを１つずつ追加する関数
        {

            bool LeftSpaceFLG = false;
            bool RightSpaceFLG = false;


            if ( this.EditBOX.Text.Length >= 1 && LastSelectionStart >= 1 )
            {
                if ( this.EditBOX.Text.Substring(LastSelectionStart - 1, 1 ) == " " )
                {
                    LeftSpaceFLG = false;
                }
                else
                {
                    LeftSpaceFLG = true;
                }
            }
            else
            {
                LeftSpaceFLG = false;
            }

            if ( this.EditBOX.Text.Length >= 1 )
            {
                if ( this.EditBOX.Text.Length > LastSelectionStart + LastSelectionLength )
                {
                    if ( this.EditBOX.Text.Substring( LastSelectionStart + LastSelectionLength, 1 ) == " " )
                    {
                        RightSpaceFLG = false;
                    }
                    else
                    {
                        RightSpaceFLG = true;
                    }
                }
            }
            else
            {
                RightSpaceFLG = false;
            }

            if ( ListChangeFlg == true )
            {
                if ( this.EditBOX.Text.Substring(this.EditBOX.Text.Length - 1, 1 ) == " " )
                {
                    LeftSpaceFLG = false;
                }
                else
                {
                    LeftSpaceFLG = true;
                }
                RightSpaceFLG = false;
            }


            //左右に半角スペースを１つずつ
            if ( LeftSpaceFLG == true )
            {
                TargetStr = " " + TargetStr;
            }

            if ( RightSpaceFLG == true )
            {
                TargetStr = TargetStr + " ";
            }

            //Console.Beep(); //【警告音】デバッグモード実行中
            //  http://dobon.net/vb/dotnet/vb2cs/vbbeep.html

            return TargetStr;
        }

        private string GetAppPath()
        {
            return System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private void readEditData()
        {
            string myPath = GetAppPath() + "\\" + "EditData.txt";

            if ( System.IO.File.Exists( myPath ) ){
                System.IO.StreamReader TextFile;
                string Line;

                this.ComboBoxEditStr.Items.Clear();

                TextFile = new System.IO.StreamReader( myPath, System.Text.Encoding.UTF8 );
                Line = TextFile.ReadLine();
                while ( Line != null ) {
                    this.ComboBoxEditStr.Items.Add(Line);
                    Line = TextFile.ReadLine();
                }
                TextFile.Close();
            }
        }

        private void writeEditData()
        {
            string myPath = GetAppPath() + "\\" + "EditData.txt";

            System.IO.StreamWriter WS;

            WS = new System.IO.StreamWriter( new System.IO.FileStream( myPath, System.IO.FileMode.Create ), System.Text.Encoding.UTF8 );
            //Create；ファイルを新規作成。すでに存在する場合は上書き

            //  http://symfoware.blog68.fc2.com/blog-entry-784.html
            for (int myIDX = 0; myIDX < this.ComboBoxEditStr.Items.Count; myIDX++)
            {
                if ( myIDX < 20 ) { //履歴は２０件まで
                    this.ComboBoxEditStr.SelectedIndex = myIDX;
                    string buff = this.ComboBoxEditStr.Text;

                    //string buff = this.ComboBoxEditStr.Items.Item(myIDX).ToString;
                    if (buff != "")
                    {
                        WS.Write(buff);          //出力データ
                        WS.WriteLine();            //行終端文
                    }
                }
            }

            WS.Close();
        }

        private void InsertStrIntoTextBox_EditBOX(string str)
        {
            if (!str.Equals("$"))  //タイプした文字が＄だった時、置き換え文字の一文字目と見なして、UNDO用変数に退避しない
            {
                TextBoxKeepStr = this.EditBOX.Text;
            }

            string TextBoxStr;
            TextBoxStr = this.EditBOX.Text;

            if ( TextBoxStr.Length >= ( LastSelectionStart + LastSelectionLength ) && ( TextBoxStr != "" ) )
            {
                this.EditBOX.Text = TextBoxStr.Substring(0, LastSelectionStart) +
                    TextBoxStr.Substring( LastSelectionStart + LastSelectionLength,
                    TextBoxStr.Length - ( LastSelectionStart + LastSelectionLength ) );
            }

            TextBoxStr = this.EditBOX.Text;

            if ( ListChangeFlg == true )
            {
                this.EditBOX.Text = this.EditBOX.Text + str;
                LastSelectionStart = this.EditBOX.Text.Length;
            }
            else if ( TextBoxStr != "" )
            {
                this.EditBOX.Text = TextBoxStr.Insert(LastSelectionStart, str );
                LastSelectionStart = LastSelectionStart + str.Length;  //連続して挿入する場合を考慮
            }
            else
            {
                this.EditBOX.Text = str;
                LastSelectionStart = str.Length; //連続して挿入する場合を考慮
            }

            LastSelectionLength = 0; //挿入後 初期化

            this.EditBOX.Focus();
            this.EditBOX.Select( LastSelectionStart, LastSelectionLength ); //現在入力中の位置にカーソルを移動
            this.EditBOX.ScrollToCaret(); //現在入力中の位置にスクロール

            ListChangeFlg = false;

        }

        private void ButtonQuit_Click(object sender, EventArgs e)  //キャンセルボタン押下
        {
            if (( TextBoxKeepStr != this.EditBOX.Text || this.ComboBoxEditStr.Items.Count == 0 ) &&
                (this.EditBOX.Text != "") && ( this.ComboBoxEditStr.Items.Contains( this.EditBOX.Text.Replace("\r\n", "$NEWLINE") ) == false ) )
            {

                //if ( MessageBox.Show("最後に編集したデータを履歴に保存しますか？", "確認", 
                //    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes ) {

                //    string ComboStr;

                //    ComboStr = this.EditBOX.Text.Replace("\r\n", "$NEWLINE");
                //    tweettextFromEditorToMain = this.EditBOX.Text.Replace("\r\n", "$NEWLINE"); //メイン側へは、置き換え文字 使用

                //    //AppSettingEmbeddedXML()
                //    //MainWindow.TextBoxTweetText.Text = tweettextFromEditorToMain;

                //    // http://www.itlab51.com/?page_id=46
                //    int myIDX = ( this.ComboBoxEditStr.Items.IndexOf(ComboStr) );

                //    if ( myIDX != -1 ) {
                //        this.ComboBoxEditStr.Items.RemoveAt(myIDX); //既に追加しようとしているデータが入っている場合、一旦削除
                //    }

                //    this.ComboBoxEditStr.Items.Insert(0, ComboStr); //最後にセットしたデータをコンボボックスの一番上へ

                //    this.ComboBoxEditStr.Text = ComboStr;

                //    TextBoxKeepStr = ComboStr.Replace("$NEWLINE", "\r\n");
                //}
            }

            if ( this.ComboBoxEditStr.Items.Count > 20 ) {

                ////  http://dobon.net/vb/dotnet/form/msgbox.html

                //DialogResult  myResult;

                //myResult = MessageBox.Show("履歴の件数が２０件を超えました。古い履歴を削除して終了しますか？" + 
                //        "\r\n" + "１件ずつ削除する場合は、「いいえ」を選択後、削除したい履歴をControlを押しながらクリックして下さい。" +
                //        "\r\n" +
                //        "\r\n" + "「はい」　→ 新しい順に２０件残して終了する" +
                //        "\r\n" + "「いいえ」→ リストを開き、１件ずつ削除する", "確認", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                //if ( myResult == System.Windows.Forms.DialogResult.Yes ) {
                //    //空文
                //}else if ( myResult == System.Windows.Forms.DialogResult.No ) {
                //    this.ComboBoxEditStr.Focus();
                //    this.ComboBoxEditStr.DroppedDown = true;  //リストを自動展開
                //    return;
                //}else if ( myResult == System.Windows.Forms.DialogResult.Cancel ) {
                //    return;
                //}
            }

            //writeEditData();

            //Application.Exit()
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ButtonDefault_Click(object sender, EventArgs e)
        {

            TextBoxKeepStr = this.EditBOX.Text;

            this.EditBOX.Text = "NowPlaying $TITLE - $ARTIST(Album:$ALBUMNAME) #nowplaying #なうぷれ";

            LastSelectionStart = this.EditBOX.Text.Length;
            LastSelectionLength = 0;

            this.EditBOX.Focus();
            this.EditBOX.Select( LastSelectionStart, LastSelectionLength ); //現在入力中の位置にカーソルを移動
            this.EditBOX.ScrollToCaret(); //現在入力中の位置にスクロール
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            TextBoxKeepStr = this.EditBOX.Text;
            this.EditBOX.Text = "";

            LastSelectionStart = 0;
            LastSelectionLength = 0;

            this.EditBOX.Focus();
            this.EditBOX.Select( LastSelectionStart, LastSelectionLength ); //現在入力中の位置にカーソルを移動
            this.EditBOX.ScrollToCaret(); //現在入力中の位置にスクロール
        }

        private void EditBOX_MouseMove(object sender, MouseEventArgs e)
        {
            LastSelectionStart = this.EditBOX.SelectionStart;
            LastSelectionLength = this.EditBOX.SelectionLength;

            ListChangeFlg = false;
        }

        private void ButtonSet_Click(object sender, EventArgs e)  //ＯＫボタン押下
        {
            // http://detail.chiebukuro.yahoo.co.jp/qa/question_detail/q1064926230
            // FindStringExact()は、使わない

            if ( this.EditBOX.Text != "" ) {
                string ComboStr;

                ComboStr = this.EditBOX.Text.Replace("\r\n", "$NEWLINE");
                tweettextFromEditorToMain = this.EditBOX.Text.Replace("\r\n", "$NEWLINE"); //メイン側へは、置き換え文字 使用

                //AppSettingEmbeddedXML()
                //MainWindow.TextBoxTweetText.Text = tweettextFromEditorToMain;

                //クリップボードに文字列をコピーする
                Clipboard.SetText(tweettextFromEditorToMain);

                GetOrSetTextForNowplayingTunes("SET");

                // http://www.itlab51.com/?page_id=46
                int myIDX = (this.ComboBoxEditStr.Items.IndexOf(ComboStr));

                if ( myIDX != -1 ) {
                    this.ComboBoxEditStr.Items.RemoveAt(myIDX);  //既に追加しようとしているデータが入っている場合、一旦削除
                }

                this.ComboBoxEditStr.Items.Insert(0, ComboStr); //最後にセットしたデータをコンボボックスの一番上へ
                this.ComboBoxEditStr.SelectedIndex = 0;

                TextBoxKeepStr = ComboStr.Replace("$NEWLINE", "\r\n");

                if (this.ComboBoxEditStr.Items.Count > 20 ) {

                    DialogResult myResult;

                    myResult = MessageBox.Show("履歴の件数が２０件を超えました。古い履歴を削除して終了しますか？" +
                            "\r\n" + "１件ずつ削除する場合は、「いいえ」を選択後、削除したい履歴をControlを押しながらクリックして下さい。" +
                            "\r\n" +
                            "\r\n" + "「はい」　→ 新しい順に２０件残して終了する" +
                            "\r\n" + "「いいえ」→ リストを開き、１件ずつ削除する", "確認", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if ( myResult == System.Windows.Forms.DialogResult.Yes ) {
                        //空文
                    }else if ( myResult == System.Windows.Forms.DialogResult.No ) {
                        this.ComboBoxEditStr.Focus();
                        this.ComboBoxEditStr.DroppedDown = true;  //リストを自動展開
                        return;
                    }else if ( myResult == System.Windows.Forms.DialogResult.Cancel ) {
                        return;
                    }
                }

                writeEditData();
            }

            //Application.Exit()
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void ButtonUNDO_Click(object sender, EventArgs e)
        {
            if (this.EditBOX.Text != TextBoxKeepStr)
            {
                string workStr = this.EditBOX.Text;  // REDO用 退避変数
                this.EditBOX.Text = TextBoxKeepStr;
                TextBoxKeepStr = workStr;

                LastSelectionStart = this.EditBOX.Text.Length; //UNDO・REDO後 初期化
                LastSelectionLength = 0; //UNDO・REDO後 初期化

                this.EditBOX.Focus();
                this.EditBOX.Select( LastSelectionStart, LastSelectionLength ); //現在入力中の位置にカーソルを移動
                this.EditBOX.ScrollToCaret(); //現在入力中の位置にスクロール
            }
        }

        private void ButtonListItemClear_Click(object sender, EventArgs e)
        {
            if (this.ComboBoxEditStr.Items.Count >= 1)
            {
                if (MessageBox.Show("履歴をすべて削除しますか？", "確認", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    this.ComboBoxEditStr.Items.Clear();

                    string myPath = GetAppPath() + "\\" + "EditData.txt";

                    System.IO.StreamWriter WS;

                    WS = new System.IO.StreamWriter(
                          new System.IO.FileStream(myPath, System.IO.FileMode.Create), System.Text.Encoding.UTF8); //Create；ファイルを新規作成。すでに存在する場合は上書き

                    WS.Write("");             //出力データ
                    WS.WriteLine();           //行終端文
                    WS.Close();
                }
            }
            else
            {
                Console.Beep();  //  http://dobon.net/vb/dotnet/vb2cs/vbbeep.html
            }
        }

        private void ComboBoxEditStr_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ユーザーが選択した時にも反応するが、プログラムが選択状態を変更した時にも反応してしまうため、
            //omboBoxEditStr_SelectionChangeCommitted を使う
        }

        private void EditBOX_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyCode == Keys.A && e.Control ){
                ((TextBox)sender).SelectAll();
            }

            if ( e.KeyCode == Keys.Z && e.Control )
            {
                ButtonUNDO_Click(sender,e);
            }

            if ( this.ComboBoxEditStr.Items.Count >= 1 )
            {
                //  http://symfoware.blog68.fc2.com/blog-entry-784.html
                //this.ComboBoxEditStr.AutoCompleteMode = AutoCompleteMode.Suggest;
            }


            //どの修飾子キー(Shift、Ctrl、およびAlt)が押されているか
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                //Console.WriteLine("Shiftキーが押されています。");

                if (e.KeyCode.Equals(Keys.D4))  //キーボードの＄は Shift + 4 , 数字の４は D4
                {
                    //Console.WriteLine(" $ が押されました");

                    //「$」を押した瞬間、メニューが表示されて、「$」の入力がキャンセルされてしまうので挿入
                    ////////////InsertStrIntoTextBox_EditBOX("$");
                    ////////////InsertStrIntoTextBox_Func("$", ■TextBoxTweetText);

                    ////////////TextBoxKeepStr = this.EditBOX.Text;

                    ////////////InsertStrIntoTextBox_Func("$", this.EditBOX);

                    ////////////string ReplaceTextListData =

                    ////////////"$TITLE - 曲名" + "\r\n" +
                    ////////////"$ARTIST - アーティスト名" + "\r\n" +
                    ////////////"$ALBUMARTIST - アルバムアーティスト" + "\r\n" +
                    ////////////"$ALBUMNAME - アルバム名" + "\r\n" +
                    ////////////"$COMMENT - コメント" + "\r\n" +
                    ////////////"$COMPOSER - 作曲家" + "\r\n" +
                    ////////////"$DISCCOUNT - ディスク枚数" + "\r\n" +
                    ////////////"$DISCNUMBER - ディスクナンバー" + "\r\n" +
                    ////////////"$GENRE - ジャンル" + "\r\n" +
                    ////////////"$LASTPLAYED - 最後に再生した日付" + "\r\n" +
                    ////////////"$PLAYEDTIMES - 再生回数" + "\r\n" +
                    ////////////"$RATING - 評価" + "\r\n" +
                    ////////////"$TRACKNUMBER - トラックナンバー" + "\r\n" +
                    ////////////"$YEAR - リリース年" + "\r\n" +
                    ////////////"$NEWLINE - 改行" + "\r\n";

                    ////////////ContextMenu_Func(ReplaceTextListData);
                }
            }

            LastSelectionStart = this.EditBOX.SelectionStart;
            LastSelectionLength = this.EditBOX.SelectionLength;

            ListChangeFlg = false;
        }

        private void ComboBoxEditStr_SelectionChangeCommitted(object sender, EventArgs e)
        {
            LastSelectionStart = 0;
            LastSelectionLength = 0;

            ListChangeFlg = true;

            string wEditBox;
            wEditBox = this.EditBOX.Text.Replace("$NEWLINE", "\r\n");

            string wComboBox;
            wComboBox = this.ComboBoxEditStr.Text.Replace("$NEWLINE", "\r\n");

            this.EditBOX.Text = this.ComboBoxEditStr.Text;
            this.EditBOX.Text = this.EditBOX.Text.Replace("$NEWLINE", "\r\n");

            // http://dobon.net/vb/dotnet/system/modifierkeys.html

            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                int myIDX = this.ComboBoxEditStr.SelectedIndex;

                if (myIDX != -1)
                {
                    if (MessageBox.Show("以下のデータをリストから削除しますか？" + "\r\n" + "\r\n" +
                        this.ComboBoxEditStr.Text, "確認", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {

                        this.ComboBoxEditStr.Items.RemoveAt(myIDX); // Ctrlキーを押しながら、リストをクリックする → 削除

                        if (this.ComboBoxEditStr.Items.Count == 20)
                        {
                            //Console.Beep();  //  http://dobon.net/vb/dotnet/vb2cs/vbbeep.html
                        }

                        writeEditData();

                        if (wComboBox == wEditBox)
                        {
                            this.EditBOX.Text = "";

                            if (this.ComboBoxEditStr.Items.Count >= 1)
                            {
                                this.ComboBoxEditStr.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            //クリアする一個前の値とINDEXで復帰

                            this.EditBOX.Text = wEditBox;
                            myIDX = this.ComboBoxEditStr.FindStringExact(wEditBox.Replace("\r\n", "$NEWLINE"), 0);
                            this.ComboBoxEditStr.SelectedIndex = myIDX;
                        }

                        LastSelectionStart = this.EditBOX.Text.Length;
                        LastSelectionLength = 0;

                        this.EditBOX.Focus();
                        this.EditBOX.Select(LastSelectionStart, LastSelectionLength); //現在入力中の位置にカーソルを移動
                        this.EditBOX.ScrollToCaret(); //現在入力中の位置にスクロール

                    }
                }
            }

            //追加：コンボボックスの値が変更されたら、デフォルトで最終行までスクロール
            LastSelectionStart = this.EditBOX.Text.Length;
            LastSelectionLength = 0;

            this.EditBOX.Focus();
            this.EditBOX.Select(LastSelectionStart, LastSelectionLength); //現在入力中の位置にカーソルを移動
            this.EditBOX.ScrollToCaret(); //現在入力中の位置にスクロール
        }

        private void ComboBoxEditStr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                this.ComboBoxEditStr.DroppedDown = true;  //リストを自動展開
            }
        }

        private void ComboBoxEditStr_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.ComboBoxEditStr.Text == "")
            {
                TextBoxKeepStr = this.EditBOX.Text.Replace("$NEWLINE", "\r\n");
            }
            else
            {
                this.EditBOX.Text = this.ComboBoxEditStr.Text;  //↑↓でリストを選択した際、コンボボックスの値をEditBOXに反映
                this.EditBOX.Text = this.EditBOX.Text.Replace("$NEWLINE", "\r\n");

                LastSelectionStart = this.EditBOX.Text.Length;
                LastSelectionLength = 0;
            }
        }

        private void ComboBoxEditStr_Enter(object sender, EventArgs e)
        {
            if (this.ComboBoxEditStr.Items.Contains(this.EditBOX.Text.Replace("\r\n", "$NEWLINE")) == false)
            {
                TextBoxKeepStr = this.EditBOX.Text.Replace("$NEWLINE", "\r\n");
            }
        }

        private string GetOrSetTextForNowplayingTunes(String myMode)
        {
            IntPtr hwnd = IntPtr.Zero;
            IntPtr hwndChild = IntPtr.Zero;

            //アプリケーションのハンドル取得
      
            hwnd = FindWindow(null, "なうぷれTunes");

            //hwnd = FindWindow("CalcFrame", "電卓");

            if (hwnd == IntPtr.Zero)
            {
                //MessageBox.Show("アプリが起動されていません。",
                //                    "警告",
                //                    MessageBoxButtons.OK);

                string myPath = GetAppPath() + "\\" + "なうぷれTunes.exe";
                if (System.IO.File.Exists(myPath))
                {
                    System.Diagnostics.Process.Start(myPath);

                    //メッセージキューに現在あるWindowsメッセージをすべて処理する
                    //System.Windows.Forms.Application.DoEvents();

                    System.Threading.Thread.Sleep(1500);

                    //Get a handle for the Calculator Application main window           
                    hwnd = FindWindow(null, "なうぷれTunes");

                }
                else
                {
                    myPath = GetAppPath() + "\\" + "NowplayingTunes.exe";
                    if (System.IO.File.Exists(myPath))
                    {
                        System.Diagnostics.Process.Start(myPath);

                        //メッセージキューに現在あるWindowsメッセージをすべて処理する
                        //System.Windows.Forms.Application.DoEvents();

                        System.Threading.Thread.Sleep(1500);

                        //Get a handle for the Calculator Application main window           
                        hwnd = FindWindow(null, "なうぷれTunes");
                    }
                    else
                    {
                        if (myMode.Equals("SET"))
                        {
                            MessageBox.Show("クリップボードに ツイート設定 をコピーしました。" + "\r\n" 
                                + "「なうぷれTunes」を起動してペーストして下さい。" + "\r\n" + "\r\n"
                                + "\r\n"
                                + "※なうぷれTunes が起動中であれば、アプリ名から" + "\r\n"
                                + "　ウィンドウを特定し、当アプリで編集したデータ" + "\r\n"
                                + "　を自動でセットします。" + "\r\n"
                                + "\r\n"
                                + "　また当エディターアプリをなうぷれTunesと同じ" + "\r\n" 
                                + "　フォルダにいれると、当アプリ起動時になうぷれ" + "\r\n"
                                + "　Tunesを起動できるランチャー機能があります。",
                                                "通知",
                                                MessageBoxButtons.OK);
                        }

                        return "";
                    }
                }

            }


            IntPtr iTunesHwnd = IntPtr.Zero;
            iTunesHwnd = FindWindow(null, "iTunes");

            if (iTunesHwnd == IntPtr.Zero) //iTunesが起動されていない場合（かつ、なうぷれTunesが起動されている場合）
            {
                if (System.IO.File.Exists("C:\\Program Files (x86)\\iTunes\\iTunes.exe"))
                {
                    System.Diagnostics.Process.Start("C:\\Program Files (x86)\\iTunes\\iTunes.exe"); //iTunesを起動
                }
            }

            // http://oshiete.goo.ne.jp/qa/7220109.html

            //メインウィンドウのハンドル取得
            hwnd = FindWindowEx(hwnd, IntPtr.Zero, null, "");

            //タブウィンドウのハンドル取得
            hwnd = FindWindowEx(hwnd, IntPtr.Zero, null, "基本設定");

            //フレームのハンドル取得
            hwnd = FindWindowEx(hwnd, IntPtr.Zero, null, "ツイート設定");

            //テキストボックスのハンドル取得
            hwndChild = FindWindowEx(hwnd, IntPtr.Zero, null, "");


            if (hwndChild == IntPtr.Zero)
            {
                //MessageBox.Show("対象オブジェクトが見つかりませんでした。",
                //                    "警告",
                //                    MessageBoxButtons.OK);          

                return "";
            }

            if (myMode.Equals("GET"))
            {

                StringBuilder sb1 = new StringBuilder(1024);
                SendMessage(hwndChild, WM_GETTEXT, (IntPtr)sb1.Capacity, sb1);

                this.EditBOX.Text = sb1.ToString();
                this.EditBOX.Text = this.EditBOX.Text.Replace("$NEWLINE", "\r\n");

                //MessageBox.Show(sb1.ToString(),
                //                    "テスト",
                //                    MessageBoxButtons.OK);

                return this.EditBOX.Text;
            }
            else if(myMode.Equals("SET"))
            {
                //hwnd = FindWindow(null, "なうぷれTunes");
                //WakeupWindow(hwnd);

                this.EditBOX.Text = this.EditBOX.Text.Replace("\r\n", "$NEWLINE");

                StringBuilder sb2 = new StringBuilder(this.EditBOX.Text);
                SendMessage(hwndChild, WM_SETTEXT, IntPtr.Zero, sb2);

                return this.EditBOX.Text;
            }else{
                MessageBox.Show("プログラミングエラーです。引数にGETかSETを指定して下さい。",
                                    "警告",
                                    MessageBoxButtons.OK);
                return "";
            }

        }

        private void ■TextBoxTweetText_MouseMove(object sender, MouseEventArgs e)
        {
            LastSelectionStart = ■TextBoxTweetText.SelectionStart;
            LastSelectionLength = ■TextBoxTweetText.SelectionLength;
        }

        private void ■TextBoxTweetText_KeyDown(object sender, KeyEventArgs e)
        {

            LastSelectionStart = ■TextBoxTweetText.SelectionStart;
            LastSelectionLength = ■TextBoxTweetText.SelectionLength;

            //どの修飾子キー(Shift、Ctrl、およびAlt)が押されているか
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                //Console.WriteLine("Shiftキーが押されています。");

                if (e.KeyCode.Equals(Keys.D4))  //キーボードの＄は Shift + 4 , 数字の４は D4
                {
                    //Console.WriteLine(" $ が押されました");

                    // TextBoxKeepStr = TextBoxTweetText.Text; //UNDO用に退避

                    //「$」を押した瞬間、メニューが表示されて、「$」の入力がキャンセルされてしまうので挿入
                    InsertStrIntoTextBox_Func("$",■TextBoxTweetText); //ユーザー関数

                    UI.ReplaceTextList dialog = new UI.ReplaceTextList();
                    ContextMenu_Func(dialog.Text);
                }
            }
        }


        ■private void InsertStrIntoTextBox_Func(string ReplaceText, TextBox objTextBox)
        {
            objTextBox.Focus();

            string TextBoxStr = objTextBox.Text;
            objTextBox.Text = TextBoxStr.Substring(0, LastSelectionStart) +
                TextBoxStr.Substring(LastSelectionStart + LastSelectionLength,
                TextBoxStr.Length - (LastSelectionStart + LastSelectionLength)); //選択状態にある文字を削除

            TextBoxStr = objTextBox.Text;

            objTextBox.Text = TextBoxStr.Insert(LastSelectionStart, ReplaceText);

            LastSelectionStart = LastSelectionStart + ReplaceText.Length;  //連続して挿入する場合を考慮
            LastSelectionLength = 0; //挿入後 初期化

            objTextBox.Focus();
            objTextBox.Select(LastSelectionStart, LastSelectionLength); //現在入力中の位置にカーソルを移動
            objTextBox.ScrollToCaret(); //現在入力中の位置にスクロール
        }

        ■public void ContextMenu_Func(String ReplaceTextListData)  //「＄」が押されたときに出すメニューを生成
        {

            ReplaceTextListData = ReplaceTextListData.Replace("\r\n", "\n");
            ReplaceTextListData = ReplaceTextListData.Replace("\r", "\n");  //リッチテキストボックス等で、改行コードがLFだった時の対処

            int t1;
            string[] s1;

            //Regex.Splitで分割する
            t1 = System.Environment.TickCount;
            s1 = System.Text.RegularExpressions.Regex.Split(ReplaceTextListData, "\n");
            t1 = System.Environment.TickCount - t1;

            ContextMenuStrip cntmenu = new ContextMenuStrip();

            for (int myIDX = 0; myIDX < s1.Length ; myIDX++)
            {
                if (!s1[myIDX].Equals(""))
                {
                    ToolStripMenuItem newcontitem = new ToolStripMenuItem();
                    newcontitem.Text = s1[myIDX];
                    newcontitem.Click += delegate
                    {
                        String ReplaceText = newcontitem.Text;

                        //挿入した「$」をスキップして、２文字目から「 - 」までの文字を挿入
                        ReplaceText = ReplaceText.Substring( 1, ReplaceText.IndexOf(" - ", 0) - 1 );

                        InsertStrIntoTextBox_Func(ReplaceText, ■TextBoxTweetText); //ユーザー関数

                        ////////////InsertStrIntoTextBox_EditBOX(ReplaceText); //ユーザー関数
                    };
                    cntmenu.Items.Add(newcontitem);
                }
            }

            ////▼メインフォーム右に張り付くようにメニューを表示▼
            //Point form_p = Point.Empty;

            //form_p.X = this.Left + this.Width;
            //form_p.Y = this.Top;
            //cntmenu.Show(form_p);



            ////▼マウスカーソルの位置にメニューを表示▼
            //Point mp = Control.MousePosition;  //マウスカーソルの位置を画面座標で取得

            ////ContextMenuを表示しているコントロールの「クライアント」座標に変換
            //Point cp = cntmenu.PointToClient(mp);

            //cntmenu.Show(cp);


            ////▼＄が押された右下にメニューを表示▼
            Point text_p = Point.Empty;
            GetCaretPos(out text_p);

            //ContextMenuを表示しているコントロールの「スクリーン」座標に変換
            text_p = ■TextBoxTweetText.PointToScreen(text_p);
            text_p.Y += 20;

            cntmenu.Show(text_p);



            ////////////////▼＄が押された右下にメニューを表示▼
            ////////////Point text_p = Point.Empty;
            ////////////GetCaretPos(out text_p);

            //////////////ContextMenuを表示しているコントロールの「スクリーン」座標に変換
            ////////////text_p = this.EditBOX.PointToScreen(text_p);
            ////////////text_p.Y += 20;

            ////////////cntmenu.Show(text_p);
        }

        ■public ContextMenuStrip ContextMenu_RCLK_Func(TextBox objTextBox)  //右クリックしたときに出すメニューを生成
                                                                                //form_loadでコール
        {

            ////右クリックメニュー に、既存の「コピー」「切り取り」「貼り付け」「元に戻す」「削除」に自作メニューを追加
            //http://d.hatena.ne.jp/kabacsharp/20131006/1381046053

            ContextMenuStrip cntmenu = new ContextMenuStrip();
            
            //Copy
            ToolStripMenuItem mCopy = new ToolStripMenuItem();
            mCopy.Text = "コピー(&C)";
            mCopy.Click += delegate
            {
                if (!string.IsNullOrEmpty(objTextBox.SelectedText))
                {
                    Clipboard.SetText(objTextBox.SelectedText);
                }
                else if (!string.IsNullOrEmpty(objTextBox.Text))
                {
                    Clipboard.SetText(objTextBox.Text);
                }
            };
            cntmenu.Items.Add(mCopy);

            //Cut
            ToolStripMenuItem mCut = new ToolStripMenuItem();
            mCut.Text = "切り取り(&X)";
            mCut.Click += delegate
            {
                if (!string.IsNullOrEmpty(objTextBox.SelectedText))
                {
                    Clipboard.SetText(objTextBox.SelectedText);
                    objTextBox.SelectedText = "";
                }
                else if (!string.IsNullOrEmpty(objTextBox.Text))
                {
                    Clipboard.SetText(objTextBox.Text);
                    objTextBox.Text = "";
                }
            };
            cntmenu.Items.Add(mCut);

            //Paste
            ToolStripMenuItem mPaste = new ToolStripMenuItem();
            mPaste.Text = "貼り付け(&V)";
            mPaste.Click += delegate
            {
                //if (!string.IsNullOrEmpty(objTextBox.SelectedText))
                //{
                //    objTextBox.SelectedText = Clipboard.GetText();
                //}
                //else
                //{
                //    objTextBox.Text = Clipboard.GetText();
                //}
                InsertStrIntoTextBox_Func(Clipboard.GetText(), objTextBox);  //ユーザー関数
            };
            cntmenu.Items.Add(mPaste);

            //Delete
            ToolStripMenuItem mDelete = new ToolStripMenuItem();
            mDelete.Text = "選択文字列の削除";
            mDelete.Click += delegate
            {
                InsertStrIntoTextBox_Func("", objTextBox);  //ユーザー関数  //選択範囲を""で上書き
            };
            cntmenu.Items.Add(mDelete);


            ToolStripSeparator itemSeparator = new ToolStripSeparator();    //セパレータの作成
            cntmenu.Items.Add(itemSeparator);

            ////////////string ReplaceTextListData =

            ////////////        "$TITLE - 曲名" + "\r\n" +
            ////////////        "$ARTIST - アーティスト名" + "\r\n" +
            ////////////        "$ALBUMARTIST - アルバムアーティスト" + "\r\n" +
            ////////////        "$ALBUMNAME - アルバム名" + "\r\n" +
            ////////////        "$COMMENT - コメント" + "\r\n" +
            ////////////        "$COMPOSER - 作曲家" + "\r\n" +
            ////////////        "$DISCCOUNT - ディスク枚数" + "\r\n" +
            ////////////        "$DISCNUMBER - ディスクナンバー" + "\r\n" +
            ////////////        "$GENRE - ジャンル" + "\r\n" +
            ////////////        "$LASTPLAYED - 最後に再生した日付" + "\r\n" +
            ////////////        "$PLAYEDTIMES - 再生回数" + "\r\n" +
            ////////////        "$RATING - 評価" + "\r\n" +
            ////////////        "$TRACKNUMBER - トラックナンバー" + "\r\n" +
            ////////////        "$YEAR - リリース年" + "\r\n" +
            ////////////        "$NEWLINE - 改行" + "\r\n";

            string ReplaceTextListData = "";

            UI.ReplaceTextList dialog = new UI.ReplaceTextList();
            ReplaceTextListData = dialog.Text;

            
            ReplaceTextListData = ReplaceTextListData.Replace("\r\n", "\n");
            ReplaceTextListData = ReplaceTextListData.Replace("\r", "\n");  //リッチテキストボックス等で、改行コードがLFだった時の対処

            int t1;
            string[] s1;

            //Regex.Splitで分割する
            t1 = System.Environment.TickCount;
            s1 = System.Text.RegularExpressions.Regex.Split(ReplaceTextListData, "\n");
            t1 = System.Environment.TickCount - t1;

            for (int myIDX = 0; myIDX < s1.Length; myIDX++)
            {
                if (!s1[myIDX].Equals(""))
                {
                    ToolStripMenuItem newcontitem = new ToolStripMenuItem();
                    newcontitem.Text = s1[myIDX];
                    newcontitem.Click += delegate
                    {

                        // TextBoxKeepStr = TextBoxTweetText.Text; //UNDO用に退避

                        String ReplaceText = newcontitem.Text;

                        //【×】挿入した「$」をスキップして、２文字目から「 - 」までの文字を挿入
                        //【○】右クリックでは「$」は挿入されないので、スキップしない
                        ReplaceText = ReplaceText.Substring(0, ReplaceText.IndexOf(" - ", 0));

                        InsertStrIntoTextBox_Func(ReplaceText, ■TextBoxTweetText);  //ユーザー関数

                        ////////////InsertStrIntoTextBox_EditBOX(ReplaceText);  //ユーザー関数
                    };
                    cntmenu.Items.Add(newcontitem);
                }
            }

            objTextBox.ContextMenuStrip = cntmenu;  //objTextBoxの右クリックメニューをオーバーライドして、規定のメニューにする

            return cntmenu;


            ////▼マウスカーソルの位置にメニューを表示▼
            //Point mp = Control.MousePosition;  //マウスカーソルの位置を画面座標で取得

            ////ContextMenuを表示しているコントロールの「クライアント」座標に変換
            //Point cp = cntmenu.PointToClient(mp);

            //cntmenu.Show(cp);

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr ptr, StringBuilder lParam);


        // 外部プロセスのウィンドウを起動する
        public static void WakeupWindow(IntPtr hWnd)
        {
             // メイン・ウィンドウが最小化されていれば元に戻す
             if (IsIconic(hWnd))
             {
                 ShowWindowAsync(hWnd, SW_RESTORE);
             }

             // メイン・ウィンドウを最前面に表示する
             SetForegroundWindow(hWnd);
         }
         // 外部プロセスのメイン・ウィンドウを起動するためのWin32 API
         [DllImport("user32.dll")]
         private static extern bool SetForegroundWindow(IntPtr hWnd);
         [DllImport("user32.dll")]
         private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
         [DllImport("user32.dll")]
         private static extern bool IsIconic(IntPtr hWnd);
         // ShowWindowAsync関数のパラメータに渡す定義値
         private const int SW_RESTORE = 9;  // 画面を元の大きさに戻す


         ■[DllImport("user32.dll")]
         ■private extern static int GetCaretPos(out Point p);
         //[DllImport("user32.dll")]
         //private extern static int SetCaretPos(int x, int y);
         //[DllImport("user32.dll")]
         //private extern static bool ShowCaret(IntPtr hwnd);
         //[DllImport("user32.dll")]
         //private extern static int CreateCaret(IntPtr hwnd, IntPtr hBitmap, int width, int height);
    }

}
