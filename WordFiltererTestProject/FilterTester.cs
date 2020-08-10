using CalastoneWordFilterer;
using CalastoneWordFilterer.Factories;
using CalastoneWordFilterer.FileReaders;
using CalastoneWordFilterer.Consoles;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CalastoneWordFilterer.Filters;
using System.Reflection;

namespace WordFiltererTestProject
{
    [TestClass]
    public class FilterTester
    {
        [DataTestMethod]
        [DataRow("a", true)]
        [DataRow("b", false)]
        [DataRow("bAb", true)]
        [DataRow("bab", true)]
        [DataRow("bEb", true)]
        [DataRow("beb", true)]
        [DataRow("bIb", true)]
        [DataRow("bib", true)]
        [DataRow("bOb", true)]
        [DataRow("bob", true)]
        [DataRow("bUb", true)]
        [DataRow("bub", true)]
        [DataRow("bAbb", true)]
        [DataRow("babb", true)]
        [DataRow("bEbb", true)]
        [DataRow("bebb", true)]
        [DataRow("bIbb", true)]
        [DataRow("bibb", true)]
        [DataRow("bObb", true)]
        [DataRow("bobb", true)]
        [DataRow("bUbb", true)]
        [DataRow("bubb", true)]
        [DataRow("bbAb", true)]
        [DataRow("bbab", true)]
        [DataRow("bbEb", true)]
        [DataRow("bbeb", true)]
        [DataRow("bbIb", true)]
        [DataRow("bbib", true)]
        [DataRow("bbOb", true)]
        [DataRow("bbob", true)]
        [DataRow("bbUb", true)]
        [DataRow("bbub", true)]
        [DataRow("bbb", false)]
        [DataRow("bbbb", false)]
        public void ExcludeCentreVowelFilter_ExcludesWordsWhereCentreCharacterIsAVowel(string input, bool expectedExclusion)
        {
            ExcludeCentreVowelFilter filter = new ExcludeCentreVowelFilter();

            Assert.AreEqual(expectedExclusion, filter.ExcludeWord(input));
        }

        [DataTestMethod]
        [DataRow(1, "a", false)]
        [DataRow(2, "a", true)]
        [DataRow(2, "aa", false)]
        [DataRow(3, "aa", true)]
        [DataRow(44, "pneumonoultramicroscopicsilicovolcanoconiosis", false)]
        [DataRow(45, "pneumonoultramicroscopicsilicovolcanoconiosis", false)]
        [DataRow(46, "pneumonoultramicroscopicsilicovolcanoconiosis", true)]
        [DataRow(47, "pneumonoultramicroscopicsilicovolcanoconiosis", true)]
        public void ExcludeShortWordsFilter_ExcludesWordsWhereWordIsShorterThanDefinedLength(int minlength, string input, bool expectedExclusion)
        {
            ExcludeShortWordsFilter filter = new ExcludeShortWordsFilter(minlength);

            Assert.AreEqual(expectedExclusion, filter.ExcludeWord(input));
        }

        [DataTestMethod]
        [DataRow('a', "a", true)]
        [DataRow('a', "b", false)]
        [DataRow(' ', "qwertyuiopasdjh qwygisdaygh", true)]
        [DataRow(' ', "qwertyuiopasdjhqwygisdaygh", false)]
        public void ExcludeWordsContainingCharFilter_ExcludesWordsWhereWordContainsGivenChar(char excludechar, string input, bool expectedExclusion)
        {
            ExcludeWordsContainingCharFilter filter = new ExcludeWordsContainingCharFilter(excludechar);

            Assert.AreEqual(expectedExclusion, filter.ExcludeWord(input));
        }
    }
}
