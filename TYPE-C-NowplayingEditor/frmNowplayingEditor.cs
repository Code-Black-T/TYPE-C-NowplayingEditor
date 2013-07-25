using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TYPE_C_NowplayingEditor
{

    public partial class frmNowplayingEditor : Form
    {
        public frmNowplayingEditor()
        {
            InitializeComponent();
        }

        public string tweettextFromEditorToMain;
        public string tweettextFromMainToEditor;

        bool ListChangeFlg = false;

        ToolTip ToolTip1;

        int LastSelectionStart;
        int LastSelectionLength;

        string TextBoxKeepStr; //ＯＫボタンを押す前のデータを退避

        //ボタンコントロール配列のフィールドを作成
        private System.Windows.Forms.Button[] replaceButtons;

        //フォームのLoadイベントハンドラ
        private void frmNowplayingEditor_Load(object sender, EventArgs e)
        {

            // http://dobon.net/vb/dotnet/control/buttonarray.html
            // http://social.msdn.microsoft.com/Forums/ja-JP/csharpgeneralja/thread/29b6239c-c672-4592-9b03-3784ad366b8c/

            ////ボタンコントロール配列の作成
            //this.replaceButtons = new System.Windows.Forms.Button[35];

            ////ボタンコントロールの配列にすでに作成されているインスタンスを代入
            //this.replaceButtons[0] = this.rButton1;
            //this.replaceButtons[1] = this.rButton2;
            //this.replaceButtons[2] = this.rButton3;
            //this.replaceButtons[3] = this.rButton4;
            //this.replaceButtons[4] = this.rButton5;
            //this.replaceButtons[5] = this.rButton6;
            //this.replaceButtons[6] = this.rButton7;
            //this.replaceButtons[7] = this.rButton8;
            //this.replaceButtons[8] = this.rButton9;
            //this.replaceButtons[9] = this.rButton10;
            //this.replaceButtons[10] = this.rButton11;
            //this.replaceButtons[11] = this.rButton12;
            //this.replaceButtons[12] = this.rButton13;
            //this.replaceButtons[13] = this.rButton14;
            //this.replaceButtons[14] = this.rButton15;
            //this.replaceButtons[15] = this.rButton16;
            //this.replaceButtons[16] = this.rButton17;
            //this.replaceButtons[17] = this.rButton18;
            //this.replaceButtons[18] = this.rButton19;
            //this.replaceButtons[19] = this.rButton20;
            //this.replaceButtons[20] = this.rButton21;
            //this.replaceButtons[21] = this.rButton22;
            //this.replaceButtons[22] = this.rButton23;
            //this.replaceButtons[23] = this.rButton24;
            //this.replaceButtons[24] = this.rButton25;
            //this.replaceButtons[25] = this.rButton26;
            //this.replaceButtons[26] = this.rButton27;
            //this.replaceButtons[27] = this.rButton28;
            //this.replaceButtons[28] = this.rButton29;
            //this.replaceButtons[29] = this.rButton30;
            //this.replaceButtons[30] = this.rButton31;
            //this.replaceButtons[31] = this.rButton32;
            //this.replaceButtons[32] = this.rButton33;
            //this.replaceButtons[33] = this.rButton34;
            //this.replaceButtons[34] = this.rButton35;


            //または、次のようにもできる
            this.replaceButtons = new System.Windows.Forms.Button[]
                {this.rButton1,  this.rButton2,  this.rButton3,  this.rButton4,  this.rButton5 ,
                 this.rButton6,  this.rButton7,  this.rButton8,  this.rButton9,  this.rButton10 ,
                 this.rButton11, this.rButton12, this.rButton13, this.rButton14, this.rButton15 ,
                 this.rButton16, this.rButton17, this.rButton18, this.rButton19, this.rButton20 ,
                 this.rButton21, this.rButton22, this.rButton23, this.rButton24, this.rButton25 ,
                 this.rButton26, this.rButton27, this.rButton28, this.rButton29, this.rButton30 ,
                 this.rButton31, this.rButton32, this.rButton33, this.rButton34, this.rButton35 ,
                 this.rButton36, this.rButton37
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

            //tweettextFromMainToEditor = 「基本設定」の TextBox1.text
            tweettextFromMainToEditor = Form1.TextBox1.Text; //■

            this.EditBOX.Text = tweettextFromMainToEditor; //■「基本設定」側から、「ツイートする文字の設定」を読み込む
            this.EditBOX.Text = this.EditBOX.Text.Replace("$NEWLINE", "\r\n");

            if ( this.ComboBoxEditStr.Items.Count == 0 ){
                this.ComboBoxEditStr.Text = "NowPlaying $TITLE - $ARTIST(Album:$ALBUM) #nowplaying";
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

            //【隠し機能】Ctrlキーを押しながら、ボタンをクリックする → デバッグモード
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

                InsertStrIntoComboBox(tempStr);
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
                        //上のtempStr をそのまま使う  //半角スペースが含まれない場合は文字列全体
                    }
                }

                InsertStrIntoComboBox(tempStr);
            }
        }

        private string AddSpaceToLeftAndRight(String TargetStr)
        {

            //【隠し機能】Ctrlキーを押しながら、ボタンをクリックする → デバッグモード
            //string tempStr;
            //tempStr = ((System.Windows.Forms.Button)sender).Text;

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

        private void InsertStrIntoComboBox(string str)
        {
            TextBoxKeepStr = this.EditBOX.Text;

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
                ( this.EditBOX.Text != "" ) ) {

                if ( MessageBox.Show("最後に編集したデータを履歴に保存しますか？", "確認", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes ) {

                    string ComboStr;

                    ComboStr = this.EditBOX.Text.Replace("\r\n", "$NEWLINE");
                    tweettextFromEditorToMain = this.EditBOX.Text.Replace("\r\n", "$NEWLINE"); //■メイン側へは、置き換え文字 使用

                    //AppSettingEmbeddedXML() //■AppSetting.xmlは、置き換え文字必須！  一度改行が入ると次回以降保存ができない。
                    Form1.TextBox1.Text = tweettextFromEditorToMain; //■

                    // http://www.itlab51.com/?page_id=46
                    int myIDX = ( this.ComboBoxEditStr.Items.IndexOf(ComboStr) );

                    if ( myIDX != -1 ) {
                        this.ComboBoxEditStr.Items.RemoveAt(myIDX); //既に追加しようとしているデータが入っている場合、一旦削除
                    }

                    this.ComboBoxEditStr.Items.Insert(0, ComboStr); //最後にセットしたデータをコンボボックスの一番上へ

                    this.ComboBoxEditStr.Text = ComboStr;

                    TextBoxKeepStr = ComboStr.Replace("$NEWLINE", "\r\n");
                }
            }

            if ( this.ComboBoxEditStr.Items.Count > 20 ) {

                //  http://dobon.net/vb/dotnet/form/msgbox.html

                DialogResult  myResult;

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

            //Application.Exit()
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ButtonDefault_Click(object sender, EventArgs e)
        {

            TextBoxKeepStr = this.EditBOX.Text;

            this.EditBOX.Text = "NowPlaying $TITLE - $ARTIST(Album:$ALBUM) #nowplaying";

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
                tweettextFromEditorToMain = this.EditBOX.Text.Replace("\r\n", "$NEWLINE"); //■メイン側へは、置き換え文字 使用

                //AppSettingEmbeddedXML() //■AppSetting.xmlは、置き換え文字必須！  一度改行が入ると次回以降保存ができない。
                Form1.TextBox1.Text = tweettextFromEditorToMain; //■

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
            this.EditBOX.Text = TextBoxKeepStr;

            LastSelectionStart = TextBoxKeepStr.Length; //UNDO後 初期化
            LastSelectionLength = 0; //UNDO後 初期化

            this.EditBOX.Focus();
            this.EditBOX.Select( LastSelectionStart, LastSelectionLength ); //現在入力中の位置にカーソルを移動
            this.EditBOX.ScrollToCaret(); //現在入力中の位置にスクロール
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

                        this.ComboBoxEditStr.Items.RemoveAt(myIDX); //【隠し機能】Ctrlキーを押しながら、リストをクリックする → 削除
                        //this.ComboBoxEditStr.DroppedDown = false;  //リストを閉じる


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
                                //this.EditBOX.Text = this.ComboBoxEditStr.Text;
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
    }
}
