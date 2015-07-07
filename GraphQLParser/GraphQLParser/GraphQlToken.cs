using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GraphQLParser
{
    public class GraphQlToken
    {
        public string Path { get; set; }
        protected string Source { get; set; }

        protected int CurrentPosition { get; set; }

        public List<GraphQlToken> Tokens { get; set; }

        public GraphQlToken(string source, string path = "")
        {
            Source = source.Trim();
            this.Tokens = new List<GraphQlToken>();
            Path = path.Split(':')[0].Trim();
            Parse();
        }

        public List<string> GetAllPaths()
        {
            List<string> toReturn = new List<string>();

            GetAllPaths(this.Path, toReturn);

            return toReturn;
        }

        public List<string> GetAllPaths(string path, List<string> toReturn)
        {
            foreach (var token in this.Tokens)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    token.GetAllPaths(string.Format("{0}", token.Path), toReturn);
                }
                else
                {
                    token.GetAllPaths(string.Format("{0}.{1}", path, token.Path), toReturn);

                }
            }

            if (this.Tokens.Count() == 0)
            {
                toReturn.Add(path);
            }
            return toReturn;
        }

        public void Parse()
        {
            var count = this.Source.Count();

            while (CurrentPosition < count - 1)
            {
                var path = SeekToNextPosition();

                if (string.IsNullOrWhiteSpace(path))
                {
                    CurrentPosition++;
                    continue;
                }

                if (this.Source[CurrentPosition] != '{')
                {
                    ParseFields(path);
                }
                else
                {
                    int? numberOpenBraklet = null;

                    for (int i = CurrentPosition; i < count; i++)
                    {
                        var c = this.Source[i];
                        if (c.Equals(' '))
                        {
                            continue;
                        }

                        if (c.Equals('{'))
                        {
                            if (!numberOpenBraklet.HasValue)
                            {
                                numberOpenBraklet = 0;
                            }

                            numberOpenBraklet++;
                        }

                        if (c.Equals('}'))
                        {
                            numberOpenBraklet--;
                        }

                        if (numberOpenBraklet.HasValue && numberOpenBraklet == 0)
                        {
                            this.Tokens.Add(new GraphQlToken(this.Source.Substring(CurrentPosition, i - CurrentPosition + 1), path));
                            CurrentPosition = i;
                            break;
                        }
                    }
                }
            }
        }

        private string SeekToNextPosition()
        {
            if (this.Source[CurrentPosition] == '{')
            {
                CurrentPosition++;
            }

            var initialPosition = CurrentPosition;
            var count = this.Source.Count();
            while (CurrentPosition < count - 1 && this.Source[CurrentPosition] != '{' && this.Source[CurrentPosition] != ' ')
            {
                CurrentPosition++;
            }

            while (CurrentPosition < count - 2 && this.Source[CurrentPosition] == ' ')
            {
                CurrentPosition++;
            }

            if (CurrentPosition == count)
            {
                return this.Source.Substring(initialPosition, CurrentPosition - initialPosition - 1);
            }

            return this.Source.Substring(initialPosition, CurrentPosition - initialPosition);
        }

        private void ParseFields(string source)
        {
            int? init = null;
            var i = 0;

            foreach (var item in source)
            {
                if (!init.HasValue)
                {
                    if (char.IsLetterOrDigit(item))
                    {
                        init = i;
                    }
                }
                if (init.HasValue && (!char.IsLetterOrDigit(item) || i == source.Count() - 1))
                {
                    this.Tokens.Add(new GraphQlToken(string.Empty, source.Substring(init.Value, i - init.Value + 1)));
                    init = null;
                }

                i++;
            }
        }
    }
}
