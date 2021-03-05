using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AloudBible.Bible;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Utitilties.GenericArray;
using System.Data.SQLite;

namespace AloudBible
{
    public partial class Form1 : Form
    {
        public Work bible = null;
        string path = @"C:\Users\snjp7464\Documents\Research\Bible\和合本修订版";
        //string path = @"C:\Users\snjp7464\Documents\Research\Bible\圣经新译本";
        //string path = @"C:\Documents and Settings\snjp7464\My Documents\Bible\圣经和合本";
        DataSet ds = new DataSet();
        SQLiteConnection connection = null;
        //string dbPath = @"Data Source=C:/temp/bibles.db3";

        public Form1()
        {
            InitializeComponent();

            bible = CulturedConfig.FromFolder(path);
            Chapter lastChapter = bible.Books.Last().Chapters.Last();
            String lastVerseText = lastChapter.Verses.Last().Text;
            String lastChapterText = lastChapter.Text;

            //testJson();

            //using (connection = new SQLiteConnection(dbPath))
            //{
            //    connection.Open();

            //    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM books", connection))
            //    {
            //        using (SQLiteDataReader reader = command.ExecuteReader())
            //        {
            //            DataTable schema = reader.GetSchemaTable();
            //            dataGridView1.DataSource = schema;
            //        }
            //    }
            //}

            //testSqlite();
        }

        private void testSqlite()
        {
            string cs = "Data Source=C:/temp/bibles.db3";

            SQLiteCommand cmd = null;

            using (connection = new SQLiteConnection(cs))
            {
                connection.Open();

                //string stm = "SELECT SQLITE_VERSION()";
                //using(cmd = new SQLiteCommand(stm, con))
                //{
                //    string version = Convert.ToString(cmd.ExecuteScalar());

                //    Console.WriteLine("SQLite version : {0}", version);
                //    this.textBox1.Text = String.Format("SQLite version : {0}", version);
                //}

                using (cmd = new SQLiteCommand("SELECT * FROM books", connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable schema = reader.GetSchemaTable();
                        dataGridView1.DataSource = schema;
                    }
                }
                
                //using (SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM books", con))
                //{
                //    using (new SQLiteCommandBuilder(da))
                //    {
                //        da.Fill(ds, "books");
                //        dataGridView1.DataSource = ds.Tables["books"];
                //    }
                //}
                connection.Close();
            }
            //this.CenterToScreen();

        }

        private void importTxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;

                DateTime dt = DateTime.Now;
                bible = CulturedConfig.FromFolder(path);
                TimeSpan span = DateTime.Now - dt;
                Console.WriteLine("Parsing spends " + span.ToString());

                testJson();

                //dt = DateTime.Now;
                //FileStream fs = new FileStream(path + @"\content.txt", FileMode.OpenOrCreate, FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs);
                //sw.Write(bible.Content);
                //sw.Flush();
                //sw.Close();
                //fs.Close();
                //span = DateTime.Now - dt;
                //Console.WriteLine("Writing to txt file costs " + span.ToString());

                //dt = DateTime.Now;
                //byte[] bytes = System.Text.Encoding.Default.GetBytes(bible.Content);
                //span = DateTime.Now - dt;
                //Console.WriteLine(Encoding.Default.ToString() + " costs " + span.ToString());

                //dt = DateTime.Now;
                //byte[] bytes_utf8 = Encoding.UTF8.GetBytes(bible.Content);
                //span = DateTime.Now - dt;
                //Console.WriteLine(Encoding.UTF8.ToString() + " costs " + span.ToString());

                //dt = DateTime.Now;
                //byte[] bytes_BE = Encoding.BigEndianUnicode.GetBytes(bible.Content);
                //span = DateTime.Now - dt;
                //Console.WriteLine(Encoding.BigEndianUnicode.ToString() + " costs " + span.ToString());


                //dt = DateTime.Now;
                //byte[] bytes_unicode = Encoding.Unicode.GetBytes(bible.Content);
                //span = DateTime.Now - dt;
                //Console.WriteLine(Encoding.Unicode.ToString() + " costs " + span.ToString());

                //dt = DateTime.Now;
                //fs = new FileStream(path + @"\content.gb", FileMode.OpenOrCreate, FileAccess.Write);
                //fs.Write(bytes, 0, bytes.Length);
                //fs.Close();
                //span = DateTime.Now - dt;
                //Console.WriteLine("Writing to bin file costs " + span.ToString());

                //fs = new FileStream(path + @"\content.utf8", FileMode.OpenOrCreate, FileAccess.Write);
                //fs.Write(bytes_utf8, 0, bytes_utf8.Length);
                //fs.Close();
                //fs = new FileStream(path + @"\content.be", FileMode.OpenOrCreate, FileAccess.Write);
                //fs.Write(bytes_BE, 0, bytes_BE.Length);
                //fs.Close();
                //fs = new FileStream(path + @"\content.uni", FileMode.OpenOrCreate, FileAccess.Write);
                //fs.Write(bytes_unicode, 0, bytes_unicode.Length);
                //fs.Close();
            }
        }

        private void testJson()
        {
            if (bible == null)
                return;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Scope));
            string result = null;

            Verse verse = bible[BibleCode.Mark][1][2];
            serializer = new DataContractJsonSerializer(typeof(Verse));
            using (MemoryStream stream = new MemoryStream())
            {
                //JSON序列化  
                serializer.WriteObject(stream, verse);
                result = Encoding.UTF8.GetString(stream.ToArray());
                Console.WriteLine("使用DataContractJsonSerializer序列化后的结果：{0},长度：{1}", result, result.Length);
            }

            Chapter ch = bible[BibleCode.James][1];
            serializer = new DataContractJsonSerializer(typeof(Chapter));
            using (MemoryStream stream = new MemoryStream())
            {
                //JSON序列化  
                serializer.WriteObject(stream, ch);
                result = Encoding.UTF8.GetString(stream.ToArray());
                Console.WriteLine("使用DataContractJsonSerializer序列化后的结果：{0},长度：{1}", result, result.Length);
            }

            Book book = bible[BibleCode.Chronicles1];
            serializer = new DataContractJsonSerializer(typeof(Book));
            using (MemoryStream stream = new MemoryStream())
            {
                //JSON序列化  
                serializer.WriteObject(stream, book);
                result = Encoding.UTF8.GetString(stream.ToArray());
                Console.WriteLine("The length of index file for book#" + book.Number.ToString() + " " + stream.Length.ToString());
            }
            
            serializer = new DataContractJsonSerializer(typeof(Work));
            using (MemoryStream stream = new MemoryStream())
            {
                //JSON序列化  
                serializer.WriteObject(stream, bible);
                byte[] bytes = stream.ToArray();
                result = Encoding.UTF8.GetString(bytes);
                Console.WriteLine("The length of index file for bible is " + stream.Length.ToString());
                FileStream fs = new FileStream(path + @"\index.json", FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toSQLiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chapter firstChapter = bible[BibleCode.Genesis][1];
            String text = firstChapter.Text;
            int utf8Length = Encoding.UTF8.GetByteCount(text);
            int utf16BELength = Encoding.BigEndianUnicode.GetByteCount(text);
            Encoding encoding = utf16BELength < utf8Length ? Encoding.BigEndianUnicode : Encoding.UTF8;
            if (this.bible != null)
            {
                bible.toSQLite("C:/temp/bibles.db", encoding);
            }
        }
    }
}
