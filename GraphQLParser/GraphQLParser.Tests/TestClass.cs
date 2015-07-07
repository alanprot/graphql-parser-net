using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLParser.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void FirstTest()
        {
            string input = "{teste5{superInner SupperInner2} teste6}";
            var graphQlToken = new GraphQlToken(input);
            Assert.That(graphQlToken.Tokens.First().Path.Equals("teste5"));
            Assert.That(graphQlToken.Tokens.First().Tokens.First().Path.Equals("superInner"));
            Assert.That(graphQlToken.Tokens.First().Tokens[1].Path.Equals("SupperInner2"));
            Assert.That(graphQlToken.Tokens[1].Path.Equals("teste6"));
        }
    }
}
