using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BynaryComparer
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 比較数カウント用
        /// </summary>
        private int ConpareCount = 0;

        /// <summary>
        /// 成功フラグ
        /// </summary>
        private bool IsSuccessFlg;

        /// <summary>
        /// textbox2のデータ数カウント用
        /// </summary>
        private int Data2Count = 0;

        /// <summary>
        /// 比較元データ
        /// </summary>
        private byte[] BinaryData1 = null;

        /// <summary>
        /// 比較先データ
        /// </summary>
        private byte[] BinaryData2 = null;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            //コントロール内にドラッグされたとき実行される
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            else
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            //コントロール内にドロップされたとき実行される
            //ドロップされたすべてのファイル名を取得する
            string[] fileName =
                (string[])e.Data.GetData(DataFormats.FileDrop, false);

            textBox1.Text = fileName[0];
        }

        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            //コントロール内にドラッグされたとき実行される
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            else
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            //コントロール内にドロップされたとき実行される
            //ドロップされたすべてのファイル名を取得する
            string[] fileName =
                (string[])e.Data.GetData(DataFormats.FileDrop, false);

            textBox2.Text = fileName[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                IsSuccessFlg = true;

                ReadFile1();

                ReadFile2();

                BinaryCompare();
            }
        }

        private void ReadFile1()
        {

            byte[] buf = new byte[32768]; // 一時バッファ

            System.IO.FileStream SReader = null;

            // ファイルストリームをオープン
            using (SReader = new System.IO.FileStream(textBox1.Text, System.IO.FileMode.Open))
            {

                // ﾌｧｲﾙｻｲｽﾞがﾒﾓﾘ範囲2GBを超える場合のｴﾗｰ処理
                if (SReader.Length > 0x7fffffff) throw new OutOfMemoryException();

                // 配列生成
                BinaryData1 = new byte[(int)SReader.Length];

                // 配列生成失敗時のｴﾗｰ処理
                if (BinaryData1 == null) throw new OutOfMemoryException();

                // ﾌｧｲﾙ読み込み
                SReader.Read(BinaryData1, 0, BinaryData1.Length);


                // ファイルストリームをクローズ
                SReader.Close();
                SReader = null;

            }

        }

        /// <summary>
        /// 比較先メソッド
        /// </summary>
        private void ReadFile2()
        {
            //TODO ファイルの読み込み2回分
            //TODO バイナリデータをメンバに設定

            byte[] buf = new byte[32768]; // 一時バッファ

            System.IO.FileStream SReader = null;

            // ファイルストリームをオープン
            using (SReader = new System.IO.FileStream(textBox2.Text, System.IO.FileMode.Open))
            {

                // ﾌｧｲﾙｻｲｽﾞがﾒﾓﾘ範囲2GBを超える場合のｴﾗｰ処理
                if (SReader.Length > 0x7fffffff) throw new OutOfMemoryException();

                // 配列生成
                BinaryData2 = new byte[(int)SReader.Length];

                // 配列生成失敗時のｴﾗｰ処理
                if (BinaryData2 == null) throw new OutOfMemoryException();

                // ﾌｧｲﾙ読み込み
                SReader.Read(BinaryData2, 0, BinaryData2.Length);


                // ファイルストリームをクローズ
                SReader.Close();
                SReader = null;

            }

        }

        /// <summary>
        /// バイナリ比較メソッド
        /// </summary>
        private void BinaryCompare()
        {
            while (Data2Count < BinaryData2.Length)
            {
                if (BinaryData1[0] == BinaryData2[Data2Count])
                {
                    ConpareCount++;
                    StartCompare();
                }
                else
                {
                    Data2Count++;
                }

                if (!IsSuccessFlg)
                {
                    break;
                }
            }

            if (IsSuccessFlg)
            {
                MessageBox.Show(ConpareCount.ToString() + "回の比較に成功しました。", "成功", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 比較作業開始
        /// </summary>
        private void StartCompare()
        {
            int count = 1;

            while (count < BinaryData1.Length)
            {
                Data2Count++;
                if (BinaryData1[count] == BinaryData2[Data2Count])
                {
                    count++;
                    continue;
                }
                else
                {
                    MessageBox.Show("比較の途中で失敗しました。失敗するまでのログを出力します。", "失敗", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    IsSuccessFlg = false;

                    for (int i = 0; i < Data2Count; i++)
                    {
                        using (FileStream stream = new FileStream(@"C:\test\Error" + "_" + DateTime.Now.ToString("yyyyMMDDHHmmss") +
                            + ".bin", FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        {
                            using (BinaryWriter writerSync = new BinaryWriter(stream))
                            {
                                writerSync.Write(BinaryData2[i]);
                            }
                        }
                    }
                    break;
                }
            }
        }

    }
}
