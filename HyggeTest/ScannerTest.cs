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
    }
}
