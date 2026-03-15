using Hygge.Scanners;
using Hygge.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeTest
{
    [TestClass]
    public class ScannerTest
    {
        [TestMethod]
        public void Scanner_ExpectedSingleToken()
        {
            string source = "(){},.-+;*";

            TokenType[] expected =
            {
                TokenType.LEFT_PAREN,
                TokenType.RIGHT_PAREN,
                TokenType.LEFT_BRACE,
                TokenType.RIGHT_BRACE,
                TokenType.COMMA,
                TokenType.DOT,
                TokenType.MINUS,
                TokenType.PLUS,
                TokenType.SEMICOLON,
                TokenType.STAR,
                TokenType.EOF
            };


            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            Assert.AreEqual(expected.Length, tokens.Count);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], tokens[i].Type);
            }

        }

        [TestMethod]
        public void Scanner_ExpectedOperator()
        {
            string source = "!= == <= >=";

            TokenType[] expected =
            {
                TokenType.BANG_EQUAL,
                TokenType.EQUAL_EQUAL,
                TokenType.LESS_EQUAL,
                TokenType.GREATER_EQUAL,
                TokenType.EOF
            };


            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            Assert.AreEqual(expected.Length, tokens.Count);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], tokens[i].Type);
            }
        }

        [TestMethod]
        public void Scanner_MultilineString()
        {
            string source = "\"hej\nverden\"";

            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            Assert.AreEqual(TokenType.STRING, tokens[0].Type);
            Assert.AreEqual("hej\nverden", tokens[0].Literal);
        }

        [TestMethod]
        public void Scanner_Numbers()
        {
            string source = "9";

            var scanner = new Scanner(source);
            var token = scanner.ScanTokens();

            Assert.AreEqual(TokenType.NUMBER, token[0].Type);
            Assert.AreEqual(9.0, token[0].Literal);
        }
    }
}
