using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.ComplexFileParser
{
    public class BracketFile
    {
        public String text;
        public int bracketType;
        public BracketFile[] _Children;

        public const char bracketType1Open = '{';
        public const char bracketType1Close = '}';
        public const char bracketType2Open = '[';
        public const char bracketType2Close = ']';

        public static BracketFile parseText(String input)
        {
            try
            {
                Console.WriteLine(input.Length);

                var ca = input.ToCharArray();
                int start = 0;

                BracketFile f = BracketFile.parseText(ref ca, "", 0, ref start);

                if (f._Children.Length == 1) return f._Children[0];
                else return f;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Read VRML File Error");
                return null;
            }
        }

        private static BracketFile parseText(ref char[] input, string head, int btype, ref int start)
        {
            BracketFile f = new BracketFile();

            f.bracketType = btype;
            f.text = head;

            var contextstart = start;

            var list = new List<BracketFile>();
            int length = 0;
            char s = ' ';

            while (start < input.Length)
            {
                s = input[start++];

                length = start - contextstart - 1;

                if (false) ;
                else if (s.Equals(bracketType1Open))
                {
                    list.Add(BracketFile.parseText(
                        ref input,
                        length > 0 ? new String(input, contextstart, length) : "",
                        1,
                        ref start));
                    contextstart = start + 1;
                }
                else if (s.Equals(bracketType2Open))
                {
                    list.Add(BracketFile.parseText(
                        ref input,
                        length > 0 ? new String(input, contextstart, length) : "",
                        2,
                        ref start));
                    contextstart = start + 1;
                }
                else if (btype == 1 && s.Equals(bracketType1Close))
                {
                    break;
                }
                else if (btype == 2 && s.Equals(bracketType2Close))
                {
                    break;
                }
            }

            if (start - contextstart > 0)
            {
                BracketFile f2 = new BracketFile();
                f2._Children = new BracketFile[] { };
                f2.text = length > 0 ? new String(input, contextstart, length) : "";
                f2.niceStrings();
                list.Add(f2);
            }

            f.niceStrings();
            f._Children = list.ToArray();

            return f;
        }

        private void niceStrings()
        {
            this.text = this.text.Trim();
            this.text = this.text.Replace('\t', ' ');

            int oldLength = 0;
            while (oldLength != this.text.Length)
            {
                oldLength = this.text.Length;
                this.text = this.text.Replace("  ", " ");
            }
        }

        public void save()
        {
            var x = new List<String>();

            this.save(0, ref x);

            var sf = new System.Windows.Forms.SaveFileDialog();

            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                System.IO.File.WriteAllLines(sf.FileName, x);
        }

        private void save(int level, ref List<string> str)
        {
            String s = level.ToString("00 ");
            for (int i = 0; i < level; i++) s += " ";

            if (this.text.Length < 150) s += this.text;
            else s += " XXXX " + this.text.Length;

            str.Add(s);

            if (this._Children.Length > 0)
            {
                foreach (var f in this._Children) f.save(level + 1, ref str);
            }
        }

        public void display(bool contents)
        {
            this.display(0, contents);
        }

        private void display(int level, bool contents)
        {
            String s = level.ToString("00 ");
            for (int i = 0; i < level; i++) s += " ";

            if ((contents) || (this.text.Length < 150)) s += this.text;
            else s += " XXXX " + this.text.Length;

            Console.WriteLine(s);

            if (this._Children.Length > 0)
            {
                foreach (var f in this._Children) f.display(level + 1, contents);
            }
        }














        public List<BracketFile> getMatches(ref String hit)
        {
            var ls = new List<BracketFile>();

            if (this.text.Equals(hit)) ls.Add(this);
            else foreach (var c in this._Children) ls.AddRange(c.getMatches(ref hit));

            return ls;
        }

        public List<BracketFile> getMatches(ref String hit1, ref String hit2)
        {
            var ls = new List<BracketFile>();

            if (this.text.Equals(hit1)) ls.AddRange(this.getMatches(ref hit2));
            else foreach (var c in this._Children) ls.AddRange(c.getMatches(ref hit1, ref hit2));

            return ls;
        }
    }
}
